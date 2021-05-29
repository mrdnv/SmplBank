using SmplBank.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmplBank.Infrastructure.Repository
{
    public static class QueryHelper
    {
        /// <summary>
        /// Generates insert command for the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <example>
        /// INSERT INTO [dbo].[Entity]
        /// ([Column1], [Column2])
        /// VALUES (@Column1, @Column2);
        /// SELECT SCOPE_IDENTITY();
        /// </example>
        /// <returns></returns>
        public static string GenerateInsertQuery<T>() where T : Entity
        {
            var entityType = typeof(T);
            var properties = entityType.GetProperties()
                .Where(property => property.Name != nameof(Entity.Id) && property.Name != nameof(Entity.RowVersion));

            var columns = properties.Select(property => property.Name);
            var columnsList = string.Join(", ", columns.Select(c => $"[{c}]"));
            var columnsVariable = string.Join(", ", columns.Select(c => $"@{c}"));

            var query = new StringBuilder();
            query.Append($"INSERT INTO [dbo].[{entityType.Name}]");
            query.AppendLine($"({columnsList})");
            query.AppendLine($"VALUES ({columnsVariable});");
            query.AppendLine("SELECT SCOPE_IDENTITY();");

            return query.ToString();
        }

        /// <summary>
        /// Generates update by Id command with version controlling for the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <example>
        /// UPDATE [dbo].[Entity]
        /// SET [Column] = @Column
        /// WHERE [Id] = @Id
        /// AND [RowVersion] = @RowVersion
        /// </example>
        /// <returns></returns>
        public static string GenerateUpdateQuery<T>() where T : Entity
        {
            var entityType = typeof(T);
            var properties = entityType.GetProperties()
                .Where(property => property.Name != nameof(Entity.Id) && property.Name != nameof(Entity.RowVersion) && property.Name != nameof(Entity.CreatedOn));

            var query = new StringBuilder();
            query.Append($"UPDATE [dbo].[{entityType.Name}]");

            var columns = properties.Select(property => property.Name);
            var setCommand = new StringBuilder();
            setCommand.Append(Environment.NewLine);
            setCommand.Append("SET ");
            var setColumnList = new List<string>();

            foreach (var column in columns)
            {
                setColumnList.Add($"[{column}] = @{column}");
            }

            setCommand.Append(string.Join(", ", setColumnList));
            setCommand.Append(Environment.NewLine);
            setCommand.Append($"OUTPUT INSERTED.{nameof(Entity.RowVersion)}");
            query.Append(Environment.NewLine);
            query.AppendLine(setCommand.ToString());

            var whereCondition = $"WHERE [{nameof(Entity.Id)}] = @{nameof(Entity.Id)} AND [{nameof(Entity.RowVersion)}] = @{nameof(Entity.RowVersion)}";
            
            query.Append(Environment.NewLine);
            query.Append(whereCondition);

            return query.ToString();
        }

        /// <summary>
        /// Generates find by Id command for the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <example>
        /// SELECT [Id], [Column1], [Column2], [Column3], [CreatedOn], [UpdatedOn], [RowVersion]
        /// FROM [dbo].[Entity]
        /// WHERE [Id] = @Id
        /// </example>
        /// <returns></returns>
        public static string GenerateFindQuery<T>() where T : Entity
        {
            var entityType = typeof(T);
            var columns = entityType.GetProperties()
                .Select(property => property.Name);
            var selectQuery = string.Join(", ", columns.Select(c => $"[{c}]"));

            return $"SELECT {selectQuery} FROM [dbo].[{entityType.Name}] WHERE [{nameof(Entity.Id)}] = @{nameof(Entity.Id)}";
        }
    }
}
