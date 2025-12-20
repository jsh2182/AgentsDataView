using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace AgentsDataView.Data
{
    public static class EfBulkExtensions
    {
        private static readonly ConcurrentDictionary<Type, EntityMapping> _entityMappingCache = new();
        private static readonly ConcurrentDictionary<Type, ColumnMapping> _keyCache = new();

        private const int DefaultBatchSize = 5000;
        private const int DefaultBulkCopyTimeout = 0;// 120; // seconds

        #region Bulk Insert
        public static async Task BulkInsertAsync<T>(this ApplicationDbContext context, IEnumerable<T> data, CancellationToken cancellationToken)
        {
            await using var conn = (SqlConnection)context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(cancellationToken);
            await using var transaction =(SqlTransaction)( await conn.BeginTransactionAsync(cancellationToken));
            await context.BulkInsertAsync(data, transaction, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        public static async Task BulkInsertAsync<T>(this ApplicationDbContext context, IEnumerable<T> data, SqlTransaction transaction, CancellationToken cancellationToken)
        {
            if (data == null || !data.Any()) return;

            var mapping = GetEntityMapping(context, typeof(T), out ColumnMapping? keyCol);
            var table = new DataTable();
            foreach (var col in mapping.Columns)
                table.Columns.Add(col.DbColumnName, Nullable.GetUnderlyingType(col.PropertyType) ?? col.PropertyType);

            var getters = mapping.Columns.Select(c => c.Getter).ToArray();

            foreach (var item in data)
            {
                var values = new object[mapping.Columns.Count];
                for (int i = 0; i < mapping.Columns.Count; i++)
                {
                    var val = item != null ?getters[i]?.Invoke(item):null;
                    values[i] = val ?? DBNull.Value;
                }
                table.Rows.Add(values);
            }

            using var bulkCopy = new SqlBulkCopy(transaction.Connection, SqlBulkCopyOptions.Default, transaction);
            bulkCopy.DestinationTableName = $"[{mapping.Schema}].[{mapping.TableName}]";
            bulkCopy.BatchSize = Math.Min(DefaultBatchSize, Math.Max(1, table.Rows.Count));
            bulkCopy.BulkCopyTimeout = DefaultBulkCopyTimeout;
            foreach (var col in mapping.Columns)
                bulkCopy.ColumnMappings.Add(col.DbColumnName, col.DbColumnName);

            await bulkCopy.WriteToServerAsync(table, cancellationToken);
        }

        public static async Task BulkInsertWithOutputIdsAsync<T>(this ApplicationDbContext context, IList<T> data,CancellationToken cancellationToken, SqlTransaction? transaction = null)
        {
            if (data == null || !data.Any()) return;

            bool externalTransaction = transaction != null;
            SqlConnection conn = (SqlConnection)(transaction != null ? transaction.Connection : context.Database.GetDbConnection());
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(cancellationToken);
            transaction ??= (SqlTransaction)(await conn.BeginTransactionAsync(cancellationToken));

            var mapping = GetEntityMapping(context, typeof(T), out ColumnMapping? keyCol);
            if (keyCol == null)
                throw new InvalidOperationException($"Entity {typeof(T).Name} Has No [Key] Property.");

            string tableName = $"[{mapping.Schema}].[{mapping.TableName}]";
            string tempColumn = "TempGuid";

            string addColSql = $"ALTER TABLE {tableName} ADD [{tempColumn}] UNIQUEIDENTIFIER NULL;";
            await using (var addCmd = new SqlCommand(addColSql, conn, transaction))
                await addCmd.ExecuteNonQueryAsync(cancellationToken);

            try
            {
                var table = new DataTable();
                table.Columns.Add(tempColumn, typeof(Guid));
                foreach (var col in mapping.Columns)
                    table.Columns.Add(col.DbColumnName, Nullable.GetUnderlyingType(col.PropertyType) ?? col.PropertyType);

                var guidList = new List<Guid>();
                var getters = mapping.Columns.Select(c => c.Getter).ToArray();
                for (int idx = 0; idx < data.Count; idx++)
                {
                    var item = data[idx];
                    var guid = Guid.NewGuid();
                    guidList.Add(guid);

                    var values = new object[1 + mapping.Columns.Count];
                    values[0] = guid;
                    for (int i = 0; i < mapping.Columns.Count; i++)
                    {
                        var val = getters[i]?.Invoke(item);
                        values[1 + i] = val ?? DBNull.Value;
                    }
                    table.Rows.Add(values);
                }

                string tempTableName = $"#TempInsert_{mapping.TableName}_{Guid.NewGuid():N}";
                string createTempSql = $@"
                CREATE TABLE {tempTableName} (
                    [{tempColumn}] UNIQUEIDENTIFIER NOT NULL,
                    {string.Join(", ", mapping.Columns.Select(c => $"[{c.DbColumnName}] {c.SqlType} {c.CollateStr} NULL"))}
                )";

                await using (var createCmd = new SqlCommand(createTempSql, conn, transaction))
                    await createCmd.ExecuteNonQueryAsync(cancellationToken);

                using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.DestinationTableName = tempTableName;
                    bulkCopy.BatchSize = Math.Min(DefaultBatchSize, Math.Max(1, table.Rows.Count));
                    bulkCopy.BulkCopyTimeout = DefaultBulkCopyTimeout;

                    bulkCopy.ColumnMappings.Add(tempColumn, tempColumn);
                    foreach (var col in mapping.Columns)
                        bulkCopy.ColumnMappings.Add(col.DbColumnName, col.DbColumnName);

                    await bulkCopy.WriteToServerAsync(table, cancellationToken);
                }

                var insertColumns = string.Join(", ", mapping.Columns.Select(c => $"[{c.DbColumnName}]")) + $", [{tempColumn}]";
                var selectColumns = string.Join(", ", mapping.Columns.Select(c => $"src.[{c.DbColumnName}]")) + $", src.[{tempColumn}]";

                string insertSql = $@"
                INSERT INTO {tableName} ({insertColumns})
                OUTPUT inserted.[{keyCol.DbColumnName}], inserted.[{tempColumn}]
                SELECT {selectColumns}
                FROM {tempTableName} AS src;
            ";

                var idMap = new Dictionary<Guid, object>();
                await using (var cmd = new SqlCommand(insertSql, conn, transaction))
                {
                    cmd.CommandTimeout = 0;
                    await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var insertedId = reader.GetValue(0);
                        var tempGuid = reader.GetGuid(1);
                        idMap[tempGuid] = insertedId;
                    }
                }

                for (int i = 0; i < data.Count; i++)
                {
                    var guid = guidList[i];
                    if (idMap.TryGetValue(guid, out var id))
                        keyCol.Property.SetValue(data[i], Convert.ChangeType(id, keyCol.PropertyType));
                }

                await using var dropTemp = new SqlCommand($"DROP TABLE {tempTableName};", conn, transaction);
                await dropTemp.ExecuteNonQueryAsync(cancellationToken);
            }
            catch
            {
                if (!externalTransaction)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                throw;
            }
            finally
            {
                await TryDropColumnIfExistsAsync(conn, transaction, mapping.Schema, mapping.TableName, tempColumn, cancellationToken);

                if (!externalTransaction)
                {
                    await transaction.CommitAsync(cancellationToken);
                    await conn.CloseAsync();
                    await conn.DisposeAsync();
                }
            }
        }
        #endregion

        #region Bulk Update
        public static async Task BulkUpdateAsync<T>(this ApplicationDbContext context, IEnumerable<T> data, CancellationToken cancellationToken)
        {
            await using var conn = (SqlConnection)context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(cancellationToken);
            await using var transaction =(SqlTransaction)( await conn.BeginTransactionAsync(cancellationToken));
            await context.BulkUpdateAsync(data, transaction, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        public static async Task BulkUpdateAsync<T>(this ApplicationDbContext context, IEnumerable<T> data, SqlTransaction transaction, CancellationToken cancellationToken)
        {
            if (data == null || !data.Any()) return;

            var mapping = GetEntityMapping(context, typeof(T), out ColumnMapping? keyCol);
            if (keyCol == null)
                throw new InvalidOperationException($"No Primary Key Defined For Entity {typeof(T).Name}");

            if (!mapping.Columns.Any(c => c.DbColumnName == keyCol.DbColumnName))
                mapping.Columns.Add(keyCol);

            string tableName = $"[{mapping.Schema}].[{mapping.TableName}]";
            string tempTableName = $"#Temp_{mapping.TableName}_{Guid.NewGuid():N}";

            var table = new DataTable();
            foreach (var col in mapping.Columns)
                table.Columns.Add(col.DbColumnName, Nullable.GetUnderlyingType(col.PropertyType) ?? col.PropertyType);

            var getters = mapping.Columns.Select(c => c.Getter).ToArray();

            foreach (var item in data)
            {
                var values = new object[mapping.Columns.Count];
                for (int i = 0; i < mapping.Columns.Count; i++)
                {
                    object? val = item != null ? getters[i]?.Invoke(item):null;
                    values[i] = val ?? DBNull.Value;
                }
                table.Rows.Add(values);
            }

            var createTempSql = $"CREATE TABLE {tempTableName} ({string.Join(", ", mapping.Columns.Select(c => $"[{c.DbColumnName}] {c.SqlType} {c.CollateStr} NULL"))})";
            await using (var cmd = new SqlCommand(createTempSql, transaction.Connection, transaction))
                await cmd.ExecuteNonQueryAsync(cancellationToken);

            using (var bulkCopy = new SqlBulkCopy(transaction.Connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkCopy.DestinationTableName = tempTableName;
                bulkCopy.BatchSize = Math.Min(DefaultBatchSize, Math.Max(1, table.Rows.Count));
                bulkCopy.BulkCopyTimeout = DefaultBulkCopyTimeout;
                foreach (var col in mapping.Columns)
                    bulkCopy.ColumnMappings.Add(col.DbColumnName, col.DbColumnName);
                await bulkCopy.WriteToServerAsync(table, cancellationToken);
            }

            var setClauses = mapping.Columns
                .Where(c => c.DbColumnName != keyCol.DbColumnName)
                .Select(c =>
                {
                    string sqlCompare = GetSqlComparison(c.PropertyType, c.DbColumnName);
                    return $"Target.[{c.DbColumnName}] = CASE WHEN {sqlCompare} THEN Source.[{c.DbColumnName}] ELSE Target.[{c.DbColumnName}] END";
                });

            string updateSql = $@"
                                  UPDATE Target
                                  SET {string.Join(", ", setClauses)}
                                  FROM {tableName} AS Target
                                  INNER JOIN {tempTableName} AS Source
                                  ON Target.[{keyCol.DbColumnName}] = Source.[{keyCol.DbColumnName}]";

            await using (var cmd = new SqlCommand(updateSql, transaction.Connection, transaction))
            {
                cmd.CommandTimeout = 0;
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }
        #endregion

        #region Bulk Delete
        public static async Task BulkDeleteAsync<T>(this ApplicationDbContext context, IEnumerable<T> data, CancellationToken cancellationToken)
        {
            await using var conn = (SqlConnection)context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync(cancellationToken);
            await using var transaction =(SqlTransaction)( await conn.BeginTransactionAsync(cancellationToken));
            await context.BulkDeleteAsync(data, transaction, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        public static async Task BulkDeleteAsync<T>(this ApplicationDbContext context, IEnumerable<T> data, SqlTransaction transaction, CancellationToken cancellationToken)
        {
            if (data == null || !data.Any()) return;

            var mapping = GetEntityMapping(context, typeof(T), out ColumnMapping? keyColumn);
            if (keyColumn == null)
                throw new InvalidOperationException($"No Primary Key Defined For Entity {typeof(T).Name}");

            string tableName = $"[{mapping.Schema}].[{mapping.TableName}]";
            var keyValues = data.Select(d => keyColumn.Property.GetValue(d)).ToList();
            if (keyValues.Count == 0) return;

            string tempTableName = $"#TempDelete_{mapping.TableName}_{Guid.NewGuid():N}";
            var table = new DataTable();
            table.Columns.Add(keyColumn.DbColumnName, Nullable.GetUnderlyingType(keyColumn.PropertyType) ?? keyColumn.PropertyType);
            foreach (var val in keyValues)
                table.Rows.Add(val ?? DBNull.Value);

            string createTempSql = $"CREATE TABLE {tempTableName} ([{keyColumn.DbColumnName}] {keyColumn.SqlType} NULL)";
            await using (var cmd = new SqlCommand(createTempSql, transaction.Connection, transaction))
                await cmd.ExecuteNonQueryAsync(cancellationToken);

            using (var bulkCopy = new SqlBulkCopy(transaction.Connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkCopy.DestinationTableName = tempTableName;
                bulkCopy.BatchSize = Math.Min(DefaultBatchSize, Math.Max(1, table.Rows.Count));
                bulkCopy.BulkCopyTimeout = DefaultBulkCopyTimeout;
                bulkCopy.ColumnMappings.Add(keyColumn.DbColumnName, keyColumn.DbColumnName);
                await bulkCopy.WriteToServerAsync(table, cancellationToken);
            }

            string deleteSql = $@"
                                  DELETE Target
                                  FROM {tableName} AS Target
                                  INNER JOIN {tempTableName} AS Source
                                  ON Target.[{keyColumn.DbColumnName}] = Source.[{keyColumn.DbColumnName}]";

            await using (var cmd = new SqlCommand(deleteSql, transaction.Connection, transaction))
            {
                cmd.CommandTimeout = 0;
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }
        #endregion

        #region Helpers
        // سایر Helperها بدون تغییر
        private static string GetSqlComparison(Type type, string columnName)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(string)) return $"ISNULL(Target.[{columnName}], '') <> ISNULL(Source.[{columnName}], '')";
            if (type == typeof(int) || type == typeof(long) || type == typeof(decimal) || type == typeof(double)) return $"ISNULL(Target.[{columnName}], 0) <> ISNULL(Source.[{columnName}], 0)";
            if (type == typeof(bool)) return $"ISNULL(Target.[{columnName}], 0) <> ISNULL(Source.[{columnName}], 0)";
            if (type == typeof(DateTime)) return $"ISNULL(Target.[{columnName}], '1900-01-01') <> ISNULL(Source.[{columnName}], '1900-01-01')";
            if (type == typeof(Guid)) return $"ISNULL(Target.[{columnName}], '00000000-0000-0000-0000-000000000000') <> ISNULL(Source.[{columnName}], '00000000-0000-0000-0000-000000000000')";
            throw new NotSupportedException($"Type {type.Name} not supported for comparison");
        }

        private static async Task TryDropColumnIfExistsAsync(SqlConnection conn, SqlTransaction tx, string schema, string table, string column, CancellationToken cancellationToken)
        {
            string checkSql = @"SELECT COUNT(1) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @table AND COLUMN_NAME = @column";
            await using var checkCmd = new SqlCommand(checkSql, conn, tx);
            checkCmd.Parameters.AddWithValue("@schema", schema);
            checkCmd.Parameters.AddWithValue("@table", table);
            checkCmd.Parameters.AddWithValue("@column", column);
            var exists = (int)await checkCmd.ExecuteScalarAsync(cancellationToken) > 0;
            if (exists)
            {
                await using var dropCmd = new SqlCommand($"ALTER TABLE [{schema}].[{table}] DROP COLUMN [{column}];", conn, tx);
                await dropCmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        private static EntityMapping GetEntityMapping(ApplicationDbContext context, Type entityType, out ColumnMapping? keyCol)
        {
            keyCol = null;

            if (_entityMappingCache.TryGetValue(entityType, out var cached))
            {
                _keyCache.TryGetValue(entityType, out keyCol);
                return cached;
            }

            entityType = GetUnderlyingType(entityType);

            var entityTypeMeta = context.Model.FindEntityType(entityType)
                ?? throw new InvalidOperationException($"Cannot Find Mapping For Entity {entityType.Name}");

            string schema = entityTypeMeta.GetSchema() ?? "dbo";
            string tableName = entityTypeMeta.GetTableName() ?? entityType.Name;
            var columnMappings = new List<ColumnMapping>();

            foreach (var prop in entityTypeMeta.GetProperties())
            {
                var clrProp = prop.PropertyInfo;
                if (clrProp == null) continue;

                string dbColumnName = prop.GetColumnName(StoreObjectIdentifier.Table(tableName, schema)) ?? clrProp.Name;
                string sqlType = prop.GetColumnType() ?? GetSqlType(null, clrProp.PropertyType);
                int? maxLength = prop.GetMaxLength();
                bool isKey = prop.IsPrimaryKey();

                var col = new ColumnMapping
                {
                    Property = clrProp,
                    DbColumnName = dbColumnName,
                    PropertyType = clrProp.PropertyType,
                    SqlType = sqlType,
                    MaxLength = maxLength,
                    Getter = CreateGetterDelegate(clrProp)
                };

                if (isKey)
                {
                    keyCol = new ColumnMapping
                    {
                        Property = clrProp,
                        DbColumnName = dbColumnName,
                        PropertyType = clrProp.PropertyType,
                        SqlType = sqlType,
                        Getter = col.Getter
                    };
                    _keyCache[entityType] = keyCol;
                }

                var notMapped = clrProp.GetCustomAttribute<NotMappedAttribute>() != null;
                var dbGen = clrProp.GetCustomAttribute<DatabaseGeneratedAttribute>() != null;
                var valueGenerated = prop.ValueGenerated != ValueGenerated.Never;
                var storeGeneratedIsIdentityOrComputed = valueGenerated;

                if (notMapped || dbGen) continue;
                if (storeGeneratedIsIdentityOrComputed) continue;

                if (maxLength.HasValue)
                    col.MaxLength = maxLength;

                columnMappings.Add(col);
            }

            if (columnMappings.Count == 0)
            {
                columnMappings = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => !Attribute.IsDefined(p, typeof(NotMappedAttribute)))
                    .Select(p =>
                    {
                        var colAttr = p.GetCustomAttribute<ColumnAttribute>();
                        string dbName = colAttr?.Name ?? p.Name;
                        var cm = new ColumnMapping
                        {
                            Property = p,
                            DbColumnName = dbName,
                            PropertyType = p.PropertyType,
                            SqlType = GetSqlType(null, p.PropertyType),
                            Getter = CreateGetterDelegate(p)
                        };
                        return cm;
                    }).ToList();
            }

            var mapping = new EntityMapping
            {
                Schema = schema,
                TableName = tableName,
                Columns = columnMappings
            };

            _entityMappingCache[entityType] = mapping;
            return mapping;
        }

        private static Func<object, object?> CreateGetterDelegate(PropertyInfo prop)
        {
            return obj => prop.GetValue(obj);
        }

        private static Type GetUnderlyingType(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                return Nullable.GetUnderlyingType(t)!;
            return t;
        }

        private static string GetSqlType(ApplicationDbContext? context, Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(string)) return "NVARCHAR(MAX)";
            if (type == typeof(int)) return "INT";
            if (type == typeof(long)) return "BIGINT";
            if (type == typeof(decimal)) return "DECIMAL(18,2)";
            if (type == typeof(double)) return "FLOAT";
            if (type == typeof(bool)) return "BIT";
            if (type == typeof(DateTime)) return "DATETIME2";
            if (type == typeof(Guid)) return "UNIQUEIDENTIFIER";
            throw new NotSupportedException($"Type {type.Name} not supported");
        }

        private class EntityMapping
        {
            public string Schema { get; set; } = string.Empty;
            public string TableName { get; set; } = string.Empty;
            public List<ColumnMapping> Columns { get; set; } = new();
        }

        private class ColumnMapping
        {
            public PropertyInfo Property { get; set; } = null!;
            public string DbColumnName { get; set; } = null!;
            public Type PropertyType { get; set; } = null!;
            public string SqlType { get; set; } = "NVARCHAR(MAX)";
            public int? MaxLength { get; set; }
            public Func<object, object?>? Getter { get; set; }
            public string VarCharLengthStr => MaxLength.HasValue ? $"({MaxLength.Value})" : "";
            public string CollateStr => SqlType?.Contains("nvarchar", StringComparison.CurrentCultureIgnoreCase) ?? false ? " COLLATE Arabic_CI_AS":"";
        }
        #endregion
    }
}
