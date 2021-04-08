using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShopApi.Filter
{
    /// <summary>
    /// 全局异常过滤器
    /// 初始化时，AddControllers中进行全局注册
    /// </summary>
    public class CustomExceptionsFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment env;
        private readonly ILogger<CustomExceptionsFilter> logger;

        public CustomExceptionsFilter(IWebHostEnvironment _env, ILogger<CustomExceptionsFilter> _logger)
        {
            env = _env ?? throw new ArgumentNullException(nameof(_env));
            logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
        }


        public void OnException(ExceptionContext context)
        {
            var json = new ErrorResponse();
            json.Message = context.Exception.Message;                 //错误信息
            var errorAudit = "Unable to resolve service for";
            if (!string.IsNullOrEmpty(json.Message) && json.Message.Contains(errorAudit))
            {
                json.Message = json.Message.Replace(errorAudit, $"（新添加服务,需要重新编译项目）{errorAudit}");
            }

            //开发环境记录堆栈信息
            if (env.IsDevelopment())
            {
                json.StackMessage = context.Exception.StackTrace;//堆栈信息
            }

            //返回给客户端是信息
            context.Result = new InternalServerErrorObjectResult(json);

            logger.LogError(
                $"【异常类型】：{context.Exception.GetType().Name}\r\n" +
                $"【异常信息】：{json.Message}\r\n " +
                $"【堆栈信息】：{context.Exception.StackTrace}");
        }



    }


    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object value) : base(value)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    public class ErrorResponse
    {
        /// <summary>
        /// 生产环境的消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 开发环境的消息
        /// </summary>
        public string StackMessage { get; set; }
    }

}
