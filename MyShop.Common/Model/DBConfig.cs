namespace MyShop.Common
{
    public class DBConfig
    {
        /// <summary>
        /// 数据库类型  mysql、mssql、oracle
        /// </summary>
        public string DBType { get; set; }
        /// <summary>
        /// 链接字符串 主库
        /// </summary>
        public string DBConnectionString { get; set; }
        /// <summary>
        /// 链接字符串只读库
        /// </summary>
        public string DBReadOnlyConString { get; set; }
        /// <summary>
        /// 链接超时时间
        /// </summary>
        public string DBTimeout { get; set; }
        /// <summary>
        /// redis链接字符串
        /// </summary>
        public string RedisConnectionString { get; set; }

    }
}