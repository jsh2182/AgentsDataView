using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AgentsDataView.Data.Helper
{
    public static class DynamicFilterHelper
    {
        public static IQueryable<T> ApplyFilters<T, F>(IQueryable<T> query, F filterModel)
        {
            if (filterModel == null) return query;

            var param = Expression.Parameter(typeof(T), "x");
            Expression? predicate = null;
            var qryPropNames = typeof(T).GetProperties().Select(p => p.Name).ToHashSet();

            foreach (var prop in typeof(F).GetProperties())
            {
                var value = prop.GetValue(filterModel);
                if (value is null) continue;

                var (targetName, filterType) = ParseFilterProperty(prop.Name);
                if (!qryPropNames.Contains(targetName)) continue;

                var exp = BuildExpression<T>(param, targetName, filterType, value);
                if (exp != null)
                    predicate = predicate == null ? exp : Expression.AndAlso(predicate, exp);
            }

            return predicate == null
                ? query
                : query.Where(Expression.Lambda<Func<T, bool>>(predicate, param));
        }

        private static (string PropertyName, string FilterType) ParseFilterProperty(string propName)
        {
            string[] suffixes = ["From", "To", "StartsWith", "EndsWith", "Equals"];
            foreach (var s in suffixes)
            {
                if (propName.EndsWith(s))
                    return (propName.Substring(0, propName.Length - s.Length), s);
            }
            return (propName, string.Empty);
        }

        private static Expression? BuildExpression<T>(ParameterExpression param, string propertyName, string filterType, object value)
        {
            var column = Expression.Property(param, propertyName);

            // بررسی انواع عددی، تاریخی و بولی (nullable هم شامل می‌شود)
            var type = value.GetType();
            if (type == typeof(int) || type == typeof(int?) ||
                type == typeof(long) || type == typeof(long?) ||
                type == typeof(double) || type == typeof(double?) ||
                type == typeof(decimal) || type == typeof(decimal?) ||
                type == typeof(DateTime) || type == typeof(DateTime?) ||
                type == typeof(bool) || type == typeof(bool?))
            {
                return BuildNumericOrDateExpression(column, filterType, value);
            }

            // رشته‌ای
            if (value is string s && !string.IsNullOrWhiteSpace(s))
            {
                return BuildStringExpression(column, filterType, s);
            }

            return null;
        }

        private static BinaryExpression BuildNumericOrDateExpression(MemberExpression column, string filterType, object value)
        {
            var constant = Expression.Constant(value, column.Type);

            return filterType switch
            {
                "From" => Expression.GreaterThanOrEqual(column, constant),
                "To" => value is DateTime dt ? Expression.LessThanOrEqual(column, Expression.Constant(dt.AddDays(1), column.Type)) : Expression.LessThanOrEqual(column, constant),
                _ => Expression.Equal(column, constant)
            };
        }

        private static MethodCallExpression BuildStringExpression(MemberExpression column, string filterType, string value)
        {
            // تبدیل ستون و مقدار به Lower برای case-insensitive
            var left = Expression.Call(column, nameof(string.ToLower), Type.EmptyTypes);
            string lowerValue = value.ToLower();
            var right = Expression.Constant(lowerValue);

            // تعیین الگوی LIKE
            string pattern = filterType switch
            {
                "Equals" => lowerValue,
                "StartsWith" => lowerValue + "%",
                "EndsWith" => "%" + lowerValue,
                _ => "%" + lowerValue + "%"
            };

            // متد DbFunctionsExtensions.Like
            var likeMethod = typeof(DbFunctionsExtensions).GetMethod(
                nameof(DbFunctionsExtensions.Like),
                [typeof(DbFunctions), typeof(string), typeof(string)])!;

            var efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

            return Expression.Call(likeMethod, efFunctions, left, Expression.Constant(pattern));
        }
    }
}