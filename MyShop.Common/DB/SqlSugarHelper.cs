using log4net;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Common
{
    /// <summary>
    /// 此类不能被继承，不能被实例化
    /// </summary>
    public sealed class SqlSugarHelper : IDataBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SqlSugarHelper));

        private SqlSugarHelper() { }
        private static readonly object objlock = new object();
        private static SqlSugarHelper dbConnectionFactory = new SqlSugarHelper();
        public static SqlSugarHelper GetInstance
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
                            dbConnectionFactory = new SqlSugarHelper();
                        }
                    }
                }
                return dbConnectionFactory;
            }
        }

        static readonly List<SlaveConnectionConfig> listConfig_Slave = new List<SlaveConnectionConfig> {
            new SlaveConnectionConfig(){ConnectionString="127.0.0.1:6379,password=123456,abortConnect=false"},
            new SlaveConnectionConfig(){ConnectionString="127.0.0.1:6379,password=123456,abortConnect=false"},
            new SlaveConnectionConfig(){ConnectionString="127.0.0.1:6379,password=123456,abortConnect=false"}
        };

        /// <summary>
        /// 如果配置了SlaveConnectionConfigs那就是主从模式,
        /// 所有的写入删除更新都走主库，查询走从库，事务内都走主库，
        /// HitRate表示权重 值越大执行的次数越高，如果想停掉哪个连接可以把HitRate设为0
        /// </summary>
        /// <returns></returns>
        private SqlSugarClient CetDbConnection()
        {
            return new SqlSugarClient(
             new ConnectionConfig()
             {
                 ConnectionString = "127.0.0.1:6379,password=123456,abortConnect=false",
                 SlaveConnectionConfigs = listConfig_Slave,
                 DbType = DbType.SqlServer,
                 IsAutoCloseConnection = true,   //自动释放数据库
                 AopEvents = new AopEvents
                 {
                     OnLogExecuting = (sql, p) =>
                     {
                         //打印异常日志
                         log.Fatal("Sql异常-->" + string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                     }
                 }
             });
        }

        private SqlSugarClient GetDbClinet
        {
            get
            {
                var db = GetInstance.CetDbConnection();
                db.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
                {
                    //获取执行后的总毫秒数
                    double sqlExecutionTotalMilliseconds = db.Ado.SqlExecutionTime.TotalMilliseconds;
                    log.Info($"执行sql：{sql};\r\n执行总毫秒数：{sqlExecutionTotalMilliseconds}");
                };
                db.Aop.OnLogExecuting = (sql, pars) => //SQL执行前事件
                {
                    //查看赋值后的sql
                    string tempSQL = LookSQL(sql, pars);
                    log.Info($"赋值后的sql：{tempSQL}");
                };
                db.Aop.OnError = (exp) =>//执行SQL 错误事件
                {
                    //exp.sql exp.parameters 可以拿到参数和错误Sql  
                    StringBuilder sb_SugarParameterStr = new StringBuilder("###SugarParameter参数为:");
                    var parametres = exp.Parametres as SugarParameter[];
                    if (parametres != null)
                    {
                        sb_SugarParameterStr.Append(JsonConvert.SerializeObject(parametres));
                    }

                    StringBuilder sb_error = new StringBuilder();
                    sb_error.AppendLine("执行sql异常信息:" + exp.Message);
                    sb_error.AppendLine("###带参数的sql:" + exp.Sql);
                    sb_error.AppendLine("###参数信息:" + sb_SugarParameterStr.ToString());
                    sb_error.AppendLine("###StackTrace信息:" + exp.StackTrace);
                    log.Error(sb_error.ToString());
                };
                return db;
            }
        }

        public int Execute(string sql, object param)
        {
            using (var conn = GetDbClinet)
            {
                var result = conn.Ado.ExecuteCommand(sql, param);
                return result;
            }
        }

        public async Task<int> ExecuteAsync(string sql, object param)
        {
            using (var conn = GetDbClinet)
            {
                var result = await conn.Ado.ExecuteCommandAsync(sql, param);
                return result;
            }
        }

        public Task<bool> ExecuteTransactionAsync(ConcurrentDictionary<int, SqlParamterOption> sqlparamOptions)
        {
            throw new NotImplementedException();
        }

        public List<dynamic> Query(string sql, object param)
        {
            throw new NotImplementedException();
        }
        public Task<List<dynamic>> QueryAsync(string sql, object param)
        {
            throw new NotImplementedException();
        }

        public List<T> Query<T>(string sql, object param)
        {
            using (var conn = GetDbClinet)
            {
                var result = conn.Ado.SqlQuery<T>(sql, param);
                return result;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string sql, object param)
        {
            using (var conn = GetDbClinet)
            {
                var result = await conn.Ado.SqlQueryAsync<T>(sql, param);
                return result;
            }
        }

        public T QueryFirst<T>(string sql, object param)
        {
            using (var conn = GetDbClinet)
            {
                var result = conn.Ado.SqlQuerySingle<T>(sql, param);
                return result;
            }
        }

        public async Task<T> QueryFirstAsync<T>(string sql, object param)
        {
            using (var conn = GetDbClinet)
            {
                var result = await conn.Ado.SqlQuerySingleAsync<T>(sql, param);
                return result;
            }
        }

        public List<T> QueryPageList<T>(string sql, object param, out int totalCount)
        {
            throw new NotImplementedException();
        }

        public Task<PagesList<T>> QueryPageListAsync<T>(string sql, object param, PagesList<T> ts) where T : class
        {
            throw new NotImplementedException();
        }

        public object QueryScalar(string sql, object param)
        {
            throw new NotImplementedException();
        }

        public Task<object> QueryScalarAsync(string sql, object param)
        {
            throw new NotImplementedException();
        }

        private static string LookSQL(string sql, SugarParameter[] pars)
        {
            if (pars == null || pars.Length == 0) return sql;

            StringBuilder sb_sql = new StringBuilder(sql);
            var tempOrderPars = pars.Where(p => p.Value != null).OrderByDescending(p => p.ParameterName.Length).ToList();//防止 @par1错误替换@par12
            for (var index = 0; index < tempOrderPars.Count; index++)
            {
                sb_sql.Replace(tempOrderPars[index].ParameterName, "'" + tempOrderPars[index].Value.ToString() + "'");
            }

            return sb_sql.ToString();
        }
    }
}
