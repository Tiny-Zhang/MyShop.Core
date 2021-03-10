using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Common
{
    public class SystemContext
    {
        /// <summary>
        /// 读取数据库配置文件
        /// </summary>
        public static DBConfig dbConfig { get; set; }

        public static JwtAuthorizeConfig jwtConfig { get; set; }


    }
}
