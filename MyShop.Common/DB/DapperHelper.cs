using System.Collections.Generic;
using Dapper;
using System.Linq;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Data;
using System;

namespace MyShop.Common
{
    public class DapperHelper : IDataBase
    {

        /// <summary>
        /// 执行记录 (增、删、改)
        /// </summary>
        public int Execute(string sql, object param)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Write))
            {
                var result = conn.Execute(sql, param);
                return result;
            }
        }
        public async Task<int> ExecuteAsync(string sql, object param)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Write))
            {
                var result = await conn.ExecuteAsync(sql, param);
                return result;
            }
        }

        /// <summary>
        /// 集合查询 (dynamic 动态类型)
        /// </summary>
        public List<dynamic> Query(string sql, object param)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Read))
            {
                var result = conn.Query(sql, param);
                return result.ToList();
            }
        }
        public async Task<List<dynamic>> QueryAsync(string sql, object param)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Read))
            {
                var result = await conn.QueryAsync(sql, param);
                return result.ToList();
            }
        }

        /// <summary>
        /// 集合查询 (实例对象)
        /// </summary>
        public List<T> Query<T>(string sql, object param)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Read))
            {
                var result = conn.Query<T>(sql, param);
                return result.ToList();
            }
        }
        public async Task<List<T>> QueryAsync<T>(string sql, object param)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Read))
            {
                var result = await conn.QueryAsync<T>(sql, param);
                return result.ToList();
            }
        }

        /// <summary>
        /// 单条数据查询 (返回一条对象实例)
        /// </summary>
        public T QueryFirst<T>(string sql, object param)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Read))
            {
                var data = conn.Query<T>(sql, param);
                if (data.Count() > 0)
                    return data.FirstOrDefault();
                else
                    return default(T);
            }
        }
        public async Task<T> QueryFirstAsync<T>(string sql, object param)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Read))
            {
                var data = await conn.QueryAsync<T>(sql, param);
                if (data.Count() > 0)
                    return data.FirstOrDefault();
                else
                    return default(T);
            }
        }

        /// <summary>
        /// 分页返回结果
        /// </summary>
        public List<T> QueryPageList<T>(string sql, object param, out int totalCount)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Read))
            {
                using (var multi = conn.QueryMultiple(sql, param))
                {
                    if (multi.IsConsumed)   //True表示已经释放
                    {
                        totalCount = 0;
                        return null;
                    }
                    totalCount = multi.Read<int>().SingleOrDefault();
                    var pageList = multi.Read<T>().ToList();
                    return pageList;
                }
            }
        }

        public async Task<PagesList<T>> QueryPageListAsync<T>(string sql, object param, PagesList<T> ts) where T : class
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Read))
            {
                using (var multi = await conn.QueryMultipleAsync(sql, param))
                {
                    if (multi.IsConsumed)   //True表示已经释放
                    {
                        ts.totalCount = 0;
                        return null;
                    }
                    ts.totalCount = multi.Read<int>().SingleOrDefault();
                    ts.pageList = multi.Read<T>().ToList();
                    return ts;
                }
            }
        }

        /// <summary>
        /// 获取查询结果中的第1行第1列 (返回自增Id、数量统计、其他统计函数)
        /// </summary>
        public object QueryScalar(string sql, object param)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Read))
            {
                var result = conn.ExecuteScalar(sql, param);
                return result;
            }
        }

        public async Task<object> QueryScalarAsync(string sql, object param)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Read))
            {
                var result = await conn.ExecuteScalarAsync(sql, param);
                return result;
            }
        }

        /// <summary>
        /// 异步执行事务
        /// </summary>
        /// <param name="sqlparamOptions"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteTransactionAsync(ConcurrentDictionary<int, SqlParamterOption> sqlparamOptions)
        {
            using (var conn = DbConnectionFactory.CetDbConnection(DbConnectionType.Write))
            {
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    foreach (var paramsOption in sqlparamOptions.Values)
                    {
                        bool result = await conn.ExecuteAsync(paramsOption.Sql, paramsOption.DynamicParameters, transaction) > 0;
                        if (paramsOption.IsNeedCheckResult && !result)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
            return true;
        }


    }
}
