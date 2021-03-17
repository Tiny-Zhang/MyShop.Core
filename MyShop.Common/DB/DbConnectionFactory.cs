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
    /// 私有构造函数不能被实例化
    /// sealed 保证当前类不会被继承
    /// 或：继承一个接口，使用单例注册的模式调用也一样
    /// </summary>
    public sealed class DbConnectionFactory //: IDbConnectionFactory
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DbConnectionFactory));

        private DbConnectionFactory()
        {
            //log.Debug($"A看看我实例了几次：{Thread.CurrentThread.ManagedThreadId}");
        }

        private static readonly object objlock = new object();
        private static DbConnectionFactory dbConnectionFactory = new DbConnectionFactory();
        private static DbConnectionFactory GetInstance
        {
            get
            {
                if (dbConnectionFactory == null)
                {
                    lock (objlock)
                    {
                        if (dbConnectionFactory == null)
                        {
                            //log.Debug($"B看看我实例了几次：{Thread.CurrentThread.ManagedThreadId}");
                            dbConnectionFactory = new DbConnectionFactory();
                        }
                    }
                }
                return dbConnectionFactory;
            }
        }

        /// <summary>
        /// 获取数据库连接（读写分离）
        /// </summary>
        /// <param name="connectionType">读写分离</param>
        /// <returns></returns>
        public static IDbConnection CetDbConnection(DbConnectionType connectionType)
        {
            if (connectionType == DbConnectionType.Write)
            {
                return GetInstance.GetWriteDbConnection();
            }
            else
            {
                return GetInstance.GetReadDbConnection();
            }
        }

        /// <summary>
        /// 写库（主库）
        /// </summary>
        private DbConnection GetWriteDbConnection()
        {
            return GetConnection(SystemContext.dbConfig.DBConnectionString);
        }
        /// <summary>
        /// 只读库（从库）
        /// 多个从库用逗号分隔，随机分配从库
        /// </summary>
        private DbConnection GetReadDbConnection()
        {
            int sc = SystemContext.dbConfig.DbReadConnetionConString.Count;
            if (sc > 0)
            {
                Random random = new Random();
                int index = random.Next(0, sc);
                return GetConnection(SystemContext.dbConfig.DbReadConnetionConString[index]);
            }
            else
            {
                return GetWriteDbConnection();
            }
        }
        /// <summary>
        /// 返回连接
        /// </summary>
        private DbConnection GetConnection(string connectionStr)
        {
            DbConnection conn = null;
            var dbtype = GetDataBaseType(SystemContext.dbConfig.DBType);
            if (conn == null)
            {
                switch (dbtype)
                {
                    case DatabaseType.SqlServer:
                        conn = new SqlConnection(connectionStr);
                        break;
                    case DatabaseType.MySQL:
                        conn = new MySqlConnection(connectionStr);
                        break;
                    default:
                        throw new NotImplementedException("暂不支持当前类型数据库");
                }
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }

        /// <summary>
        /// 转换数据库类型
        /// </summary>
        /// <param name="dbtype">数据库类型字符串</param>
        /// <returns>数据库类型</returns>
        private DatabaseType GetDataBaseType(string dbtype)
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
