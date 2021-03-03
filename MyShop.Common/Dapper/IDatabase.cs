using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Common
{
    /// <summary>
    /// 数据接口基类
    /// </summary>
    public interface IDatabase
    {
        /// <summary>
        /// 执行记录 (增、删、改)
        /// </summary>
        int Execute(string connection, string sql, object param);

        /// <summary>
        /// 集合查询 (dynamic 动态类型)
        /// </summary>
        List<dynamic> Query(string connection, string sql, object param);

        /// <summary>
        /// 集合查询 (实例对象)
        /// </summary>
        List<T> Query<T>(string connection, string sql, object param);

        /// <summary>
        /// 单条数据查询 (返回一条对象实例)
        /// </summary>
        T QueryFirst<T>(string connection, string sql, object param);

        /// <summary>
        /// 获取查询结果中的第1行第1列 (返回自增Id、数量统计、其他统计函数)
        /// </summary>
        object QueryScalar(string connection, string sql, object param);

        /// <summary>
        /// 分页返回结果
        /// </summary>
        List<T> QueryPageList<T>(string connection, string sql, object param, out int totalCount);

    }
}
