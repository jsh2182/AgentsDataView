
using AgentsDataView.Common.Utilities;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;

namespace AgentsDataView.Common
{
    public static class IdentityExtensions
    {
        public static string? FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            return identity?.FindFirst(claimType)?.Value;
        }
        public static T? FindFirstValue<T>(this ClaimsIdentity identity, string claimType) where T:IConvertible
        {
            string? stringValue = identity?.FindFirst(claimType)?.Value;
            return stringValue.HasValue() ? (T?)Convert.ChangeType(stringValue, typeof(T), CultureInfo.InvariantCulture) : default;
        }
        public static string? FindFirstValue(this IIdentity identity, string claimType)
        {
            ClaimsIdentity? claimsIdentity = identity as ClaimsIdentity;
            return claimsIdentity?.FindFirstValue(claimType);
        }
        public static T? FindFirstValue<T>(this IIdentity identity, string claimType) where T : IConvertible
        {
            ClaimsIdentity? claimsIdentity = identity as ClaimsIdentity;
            string? stringValue = claimsIdentity?.FindFirst(claimType)?.Value;
            return stringValue.HasValue() ? (T?)Convert.ChangeType(stringValue, typeof(T), CultureInfo.InvariantCulture) : default;
        }
        public static int GetCompanyId(this IIdentity identity)
        {
            ClaimsIdentity? claimIndentity = identity as ClaimsIdentity;
            string? value = claimIndentity?.FindFirstValue("C_Id");
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("شناسه ارتباطی شرکت معتبر نیست.");
            }
            return value.ToInt();
        }
        public static string? GetUserId(this IIdentity identity)
        {
            return identity?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static T? GetUserId<T>(this IIdentity identity) where T : IConvertible
        {
            var userId = identity?.GetUserId();
            return userId.HasValue()
                ? (T?)Convert.ChangeType(userId, typeof(T), CultureInfo.InvariantCulture)
                : default;
        }

        public static string? GetUserName(this IIdentity identity)
        {
            return identity?.FindFirstValue(ClaimTypes.Name);
        }
    }
}
