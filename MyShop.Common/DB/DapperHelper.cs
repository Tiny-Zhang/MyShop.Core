using System.Collections.Generic;
using Dapper;
using System.Linq;
using System.Data.Common;

namespace MyShop.Common
{
    public class DapperHelper : IDatabase
    {
        //private static DbConnection conn;
        //public DapperHelper()
        //{
        //    conn = DbHelper.GetDbInstance(connection);
        //}

        /// <summary>
        /// 执行记录 (增、删、改)
        /// </summary>
        public int Execute(string connection, string sql, object param)
        {
            using (var conn = DbHelper.GetDbInstance(connection))
            {
                var result = conn.Execute(sql, param);
                return result;
            }
        }

        /// <summary>
        /// 集合查询 (dynamic 动态类型)
        /// </summary>
        public List<dynamic> Query(string connection, string sql, object param)
        {
            using (var conn = DbHelper.GetDbInstance(connection))
            {
                var result = conn.Query(sql, param);
                return result.ToList();
            }
        }

        /// <summary>
        /// 集合查询 (实例对象)
        /// </summary>
        public List<T> Query<T>(string connection, string sql, object param)
        {
            using (var conn = DbHelper.GetDbInstance(connection))
            {
                var result = conn.Query<T>(sql, param);
                return result.ToList();
            }
        }

        /// <summary>
        /// 单条数据查询 (返回一条对象实例)
        /// </summary>
        public T QueryFirst<T>(string connection, string sql, object param)
        {
            using (var conn = DbHelper.GetDbInstance(connection))
            {
                var data = conn.Query<T>(sql, param);
                if (data.Count() > 0)
                    return data.First();
                else
                    return default(T);
            }
        }

        /// <summary>
        /// 分页返回结果
        /// </summary>
        public List<T> QueryPageList<T>(string connection, string sql, object param, out int totalCount)
        {
            using (var conn = DbHelper.GetDbInstance(connection))
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

        /// <summary>
        /// 获取查询结果中的第1行第1列 (返回自增Id、数量统计、其他统计函数)
        /// </summary>
        public object QueryScalar(string connection, string sql, object param)
        {
            using (var conn = DbHelper.GetDbInstance(connection))
            {
                var result = conn.ExecuteScalar(sql, param);
                return result;
            }
        }
    }
}
