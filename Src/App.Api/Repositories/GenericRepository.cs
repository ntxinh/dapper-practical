using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using App.Api.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;

namespace App.Api.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : BaseEntityAudit
    {
        // #1
        // private readonly string _tableName;

        // #2
        public abstract string _tableName { get; }

        private readonly IConfiguration _config;
        private const string PrimaryKey = nameof(BaseEntity.Id);
        private const string SoftDeletedColumn = nameof(BaseEntity.IsDeleted);

        // #1
        // protected GenericRepository(string tableName, IConfiguration config)
        // {
        //     _tableName = tableName;
        //     _config = config;
        // }

        // #2
        protected GenericRepository(IConfiguration config)
        {
            _config = config;
        }

        #region Private method

        /// <summary>
        /// Generate new connection based on connection string
        /// </summary>
        /// <returns></returns>
        private SqlConnection SqlConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        /// <summary>
        /// Open new connection and return it for use
        /// </summary>
        /// <returns></returns>
        private IDbConnection CreateConnection()
        {
            var conn = SqlConnection();
            conn.Open();
            return conn;
        }

        private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();

        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where (attributes.Length <= 0
                        || (attributes[0] as DescriptionAttribute)?.Description != "ignore")
                        // && prop.Name != PrimaryKey
                    select prop.Name).ToList();
        }

        private string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");

            insertQuery.Append("(");

            var properties = GenerateListOfProperties(GetProperties);
            properties.Remove(PrimaryKey);

            properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            return insertQuery.ToString();
        }

        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");

            var properties = GenerateListOfProperties(GetProperties);
            properties.Remove(PrimaryKey);
            properties.Remove(SoftDeletedColumn);
            properties.Remove(nameof(BaseEntityAudit.CreatedAt));
            properties.Remove(nameof(BaseEntityAudit.CreatedBy));

            properties.ForEach(property =>
            {
                updateQuery.Append($"{property}=@{property},");
            });

            updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma
            updateQuery.Append($" WHERE {PrimaryKey}=@{PrimaryKey}");

            return updateQuery.ToString();
        }

        enum State
        {
            Added,
            Modified,
        }

        private void UpdateSoftDelete(T entity, bool isDeleted)
        {
            entity.IsDeleted = isDeleted;
        }

        private void UpdateAudits(T entity, State state)
        {
            // TODO: Get real current user id
            var currentUserId = 1;
            var now = DateTime.Now;

            if (state == State.Added)
            {
                entity.CreatedAt = now;
                entity.CreatedBy = currentUserId;
            }

            entity.UpdatedAt = now;
            entity.UpdatedBy = currentUserId;
        }

        private object GenerateParamById(int id)
        {
            // return new { Id = id };

            // var dynamicObject = new ExpandoObject() as IDictionary<string, Object>;
            // dynamicObject.Add(PrimaryKey, id);
            // return dynamicObject;

            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add(PrimaryKey, id);
            return dynamicParameters;
        }

        #endregion

        #region Basic operations

        public virtual async Task<T> FirstOrDefaultByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                // return await connection.GetAsync<T>(id);
                return await connection.QueryFirstOrDefaultAsync<T>($"SELECT TOP 1 * FROM {_tableName} WHERE {PrimaryKey}=@{PrimaryKey}", GenerateParamById(id));
            }
        }

        public virtual async Task<T> SingleOrDefaultByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                // var result = await connection.GetAsync<T>(id);
                var result = await connection.QuerySingleOrDefaultAsync<T>($"SELECT TOP 1 * FROM {_tableName} WHERE {PrimaryKey}=@{PrimaryKey}", GenerateParamById(id));
                if (result == null)
                    throw new KeyNotFoundException($"{_tableName} with id [{id}] could not be found.");

                return result;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                // return await connection.GetAllAsync<T>();
                return await connection.QueryAsync<T>($"SELECT * FROM {_tableName} WHERE {SoftDeletedColumn} = 0");
            }
        }

        public virtual async Task<int> InsertAsync(T entity)
        {
            entity.Id = default(int);
            UpdateSoftDelete(entity, false);
            UpdateAudits(entity, State.Added);

            var insertQuery = GenerateInsertQuery();

            using (var connection = CreateConnection())
            {
                // return await connection.InsertAsync(entity);
                return await connection.ExecuteAsync(insertQuery, entity);
            }
        }

        public virtual async Task<int> UpdateByIdAsync(T entity)
        {
            UpdateAudits(entity, State.Modified);

            var updateQuery = GenerateUpdateQuery();

            using (var connection = CreateConnection())
            {
                // return await connection.UpdateAsync(entity);
                return await connection.ExecuteAsync(updateQuery, entity);
            }
        }

        public virtual async Task<int> DeleteByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                // return await connection.DeleteAsync(id);
                return await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE {PrimaryKey}=@{PrimaryKey}", GenerateParamById(id));
            }
        }

        #endregion

        #region Advanced operations

        public virtual async Task<int> DeleteSoftByIdAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync($"UPDATE {_tableName} SET {SoftDeletedColumn} = 1 WHERE {PrimaryKey}=@{PrimaryKey}", GenerateParamById(id));
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllSoftDeletedAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<T>($"SELECT * FROM {_tableName} WHERE {SoftDeletedColumn} = 1");
            }
        }

        public virtual async Task<int> InsertRangeAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                UpdateSoftDelete(entity, false);
                UpdateAudits(entity, State.Added);
            }

            var inserted = 0;
            var query = GenerateInsertQuery();
            using (var connection = CreateConnection())
            {
                // return await connection.InsertAsync(entities);
                // var result = await connection.UpdateAsync(entities);
                // var result = await connection.DeleteAsync(entities);
                // var result = await connection.DeleteAllAsync<T>();
                inserted += await connection.ExecuteAsync(query, entities);
            }

            return inserted;
        }

        public virtual async Task<IEnumerable<T>> QueryAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
            }
        }

        public virtual async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
            }
        }

        public virtual async Task<int> ExecuteSpAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return await ExecuteAsync(sql, param, transaction, commandTimeout, commandType: CommandType.StoredProcedure);
        }

        #endregion
    }
}
