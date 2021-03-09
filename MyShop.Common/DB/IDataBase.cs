using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShop.Common
{
    /// <summary>
    /// 数据接口基类
    /// </summary>
    public interface IDataBase
    {
        /// <summary>
        /// 执行记录 (增、删、改)
        /// </summary>
        int Execute(string sql, object param);
        Task<int> ExecuteAsync(string sql, object param);

        /// <summary>
        /// 集合查询 (dynamic动态类型)
        /// </summary>
        List<dynamic> Query(string sql, object param);
        Task<List<dynamic>> QueryAsync(string sql, object param);

        /// <summary>
        /// 集合查询 (实例对象)
        /// </summary>
        List<T> Query<T>(string sql, object param);
        Task<List<T>> QueryAsync<T>(string sql, object param);

        /// <summary>
        /// 单条数据查询 (返回一条对象实例)
        /// </summary>
        T QueryFirst<T>(string sql, object param);
        Task<T> QueryFirstAsync<T>(string sql, object param);

        /// <summary>
        /// 获取查询结果中的第1行第1列 (返回自增Id、数量统计、其他统计函数)
        /// </summary>
        object QueryScalar(string sql, object param);
        Task<object> QueryScalarAsync(string sql, object param);

        /// <summary>
        /// 分页返回结果
        /// </summary>
        List<T> QueryPageList<T>(string sql, object param, out int totalCount);
        Task<PagesList<T>> QueryPageListAsync<T>(string sql, object param, PagesList<T> ts) where T : class;

        /// <summary>
        /// 异步执行事务
        /// </summary>
        Task<bool> ExecuteTransactionAsync(ConcurrentDictionary<int, SqlParamterOption> sqlparamOptions);

    }
}
