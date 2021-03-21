using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShopApi.Middlewares
{
    /// <summary>
    /// 中间件 帮助类
    /// 任何请求的最外层，还没有到控制器 Action这些信息，不适合业务逻辑
    /// 可以做：请求过滤记录、IP过滤、授权、异常处理、防盗链等业务
    /// </summary>
    public static class MiddlewareHelpers
    {
        //请求响应中间件
        public static IApplicationBuilder UseRequestResponseLogs(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UserRequestRepMidd>();
        }


        //请求异常处理中间件
        //放到UseEndpoints 上边即可,主要过滤权限和请求信息异常
        //这里不记录业务逻辑异常，业务逻辑异常在Filter过滤器中记录

    }
}
