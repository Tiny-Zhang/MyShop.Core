using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Common
{
    public class SqlParamterOption
    {
        public string Sql { get; set; }   //需要执行的sql
        public DynamicParameters DynamicParameters { get; set; }//参数对象
        public bool IsNeedCheckResult { get; set; } = true;   //是否需要检测执行结果  默认查询
    }
}
