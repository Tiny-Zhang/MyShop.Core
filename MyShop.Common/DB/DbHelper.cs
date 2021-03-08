using log4net;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;

namespace MyShop.Common
{
    /// <summary>
    /// private 保证当前类不会被实例化
    /// sealed 保证当前类不会被继承
    /// </summary>
    public sealed class DbHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DbHelper));
        private static Lazy<DbConnection> connection = null;

        private DbHelper() { }

        public static DbConnection GetDbInstance(string strconn)
        {
            if (string.IsNullOrWhiteSpace(strconn))
                throw new ArgumentNullException("缺少数据库连接字符串!");
            if (connection != null)
            {
                return connection.Value;
            }
            log.Debug("看看我被调用了几次：" + Thread.CurrentThread.ManagedThreadId.ToString());
            return CreateConnection(strconn);
        }

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="dbType">dbtype</param>
        /// <param name="strconn">数据库连接字符串</param>
        /// <returns>数据库连接</returns>
        private static DbConnection CreateConnection(string strconn)
        {
            DatabaseType _dbtype = GetDataBaseType(SystemContext.dbConfig.DBType);
            switch (_dbtype)
            {
                case DatabaseType.SqlServer:
                    connection = new Lazy<DbConnection>(new SqlConnection(strconn));
                    break;
                case DatabaseType.MySQL:
                    connection = new Lazy<DbConnection>(new MySqlConnection(strconn));
                    break;
                default:
                    throw new ArgumentNullException($"还不支持{_dbtype}数据库类型");
            }
            if (connection.Value.State == ConnectionState.Closed)
            {
                connection.Value.Open();
            }
            return connection.Value;
        }


        /// <summary>
        /// 转换数据库类型
        /// </summary>
        /// <param name="dbtype">数据库类型字符串</param>
        /// <returns>数据库类型</returns>
        private static DatabaseType GetDataBaseType(string dbtype)
        {
            if (string.IsNullOrWhiteSpace(dbtype))
                throw new ArgumentNullException("缺少数据库类型!");

            DatabaseType returnValue = DatabaseType.SqlServer;
            foreach (DatabaseType dbType in Enum.GetValues(typeof(DatabaseType)))
            {
                if (dbType.ToString().Equals(dbtype, StringComparison.OrdinalIgnoreCase))
                {
                    returnValue = dbType;
                    break;
                }
            }
            return returnValue;
        }


    }
}
