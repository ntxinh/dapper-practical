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
    public abstract class GenericContribRepository<T> : IGenericContribRepository<T> where T : BaseEntityAudit
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
        protected GenericContribRepository(IConfiguration config)
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

        public async Task<T> GetAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.GetAsync<T>(id);
            }
        }

        public async Task<T> GetAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return await GetAsync(entity.Id);
        }

        public async Task<IEnumerable<T>> GetAsync(params T[] entities)
        {
            var ids = entities.Select(x => x.Id);
            return await GetAsync(ids);
        }

        public async Task<IEnumerable<T>> GetAsync(params int[] ids)
        {
            return await GetAsync(ids.AsEnumerable());
        }

        public async Task<IEnumerable<T>> GetAsync(IEnumerable<T> entities)
        {
            var ids = entities.Select(x => x.Id);
            return await GetAsync(ids);
        }

        public async Task<IEnumerable<T>> GetAsync(IEnumerable<int> ids)
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<T>($"SELECT * FROM {_tableName} WHERE {SoftDeletedColumn} = 0 AND {PrimaryKey} IN @Ids", new { Ids = ids });
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            using (var connection = CreateConnection())
            {
                // return await connection.GetAllAsync<T>();
                return await connection.QueryAsync<T>($"SELECT * FROM {_tableName} WHERE {SoftDeletedColumn} = 0");
            }
        }

        public async Task<int> AddAsync(T entity)
        {
            entity.Id = default(int);
            UpdateSoftDelete(entity, false);
            UpdateAudits(entity, State.Added);

            using (var connection = CreateConnection())
            {
                return await connection.InsertAsync(entity);
            }
        }

        public async Task<int> AddAsync(params T[] entities)
        {
            return await AddAsync(entities.AsEnumerable());
        }

        public async Task<int> AddAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.Id = default(int);
                UpdateSoftDelete(entity, false);
                UpdateAudits(entity, State.Added);
            }

            var inserted = 0;
            var query = GenerateInsertQuery();
            using (var connection = CreateConnection())
            {
                inserted += await connection.ExecuteAsync(query, entities);
            }

            return inserted;
        }

        public async Task<int> DeleteAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE {PrimaryKey}=@{PrimaryKey}", GenerateParamById(id));
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            using (var connection = CreateConnection())
            {
                return await connection.DeleteAsync(entity);
            }
        }

        public async Task<int> DeleteAsync(params T[] entities)
        {
            var ids = entities.Select(x => x.Id);
            return await DeleteAsync(ids);
        }

        public async Task<int> DeleteAsync(params int[] ids)
        {
            return await DeleteAsync(ids.AsEnumerable());
        }

        public async Task<int> DeleteAsync(IEnumerable<T> entities)
        {
            var ids = entities.Select(x => x.Id);
            return await DeleteAsync(ids);
        }

        public async Task<int> DeleteAsync(IEnumerable<int> ids)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE {PrimaryKey} IN @Ids", new { Ids = ids });
            }
        }

        public async Task<bool> DeleteAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.DeleteAllAsync<T>();
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            UpdateAudits(entity, State.Modified);

            using (var connection = CreateConnection())
            {
                return await connection.UpdateAsync(entity);
            }
        }

        public Task<int> UpdateAsync(params T[] entities)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(IEnumerable<T> entities)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        #endregion

        #region Advanced operations

        public async Task<int> DeleteSoftAsync(int id)
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync($"UPDATE {_tableName} SET {SoftDeletedColumn} = 1 WHERE {PrimaryKey}=@{PrimaryKey}", GenerateParamById(id));
            }
        }

        public async Task<int> DeleteSoftAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return await DeleteSoftAsync(entity.Id);
        }

        public async Task<int> DeleteSoftAsync(params T[] entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            var ids = entities.Select(x => x.Id);

            return await DeleteSoftAsync(ids);
        }

        public async Task<int> DeleteSoftAsync(params int[] ids)
        {
            if (ids == null)
                throw new ArgumentNullException("entities");

            return await DeleteSoftAsync(ids.AsEnumerable());
        }

        public async Task<int> DeleteSoftAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            var ids = entities.Select(x => x.Id);

            return await DeleteSoftAsync(ids);
        }

        public async Task<int> DeleteSoftAsync(IEnumerable<int> ids)
        {
            if (ids == null)
                throw new ArgumentNullException("ids");

            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync($"UPDATE {_tableName} SET {SoftDeletedColumn} = 1 WHERE {PrimaryKey} IN @Ids", new { Ids = ids });
            }
        }

        public async Task<int> DeleteSoftAllAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync($"UPDATE {_tableName} SET {SoftDeletedColumn} = 1");
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllSoftDeletedAsync()
        {
            using (var connection = CreateConnection())
            {
                return await connection.QueryAsync<T>($"SELECT * FROM {_tableName} WHERE {SoftDeletedColumn} = 1");
            }
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
