using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyShopApi.Middlewares
{
    /// <summary>
    /// 记录用户请求响应信息
    /// 必须要有： Invoke 或 InvokeAsync  函数
    /// </summary>
    public class UserRequestRepMidd
    {
        private readonly RequestDelegate next;
        private readonly ILogger<UserRequestRepMidd> logger;

        public UserRequestRepMidd(RequestDelegate _next, ILogger<UserRequestRepMidd> _logger)
        {
            next = _next ?? throw new ArgumentNullException(nameof(_next));
            logger = _logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var getpath = context.Request.Path.Value;
            if (getpath.Contains("api"))
            {
                context.Request.EnableBuffering(); //允许多次读取Body
                var request = context.Request;
                var originalResponseBody = context.Response.Body;   //声明一个变量，接收返回响应之前的数据(这时候还没有响应值)
                try
                {
                    //请求内容
                    var sr = new StreamReader(request.Body);
                    var data = await sr.ReadToEndAsync();
                    var logContent = $"QueryData:{request.Path + request.QueryString}\r\n BodyData:{data}";
                    logger.LogInformation($"Request Data: \r\n {logContent}");
                    request.Body.Seek(0, SeekOrigin.Begin);  //设置流的读取位置从0开始

                    //请求头
                    StringBuilder header = new StringBuilder($"请求头:\r\n");
                    foreach (var item in request.Headers)
                    {
                        header.Append($"{item.Key}:{item.Value}\r\n");
                    }
                    logger.LogInformation(header.ToString());

                    //请求IP
                    var ip = request.Headers["X-Forwarded-For"].FirstOrDefault();  //请求端真实IP
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    }
                    logger.LogInformation($"请求IP：\r\n{ip}");

                    //响应数据
                    using (var ms = new MemoryStream())
                    {
                        context.Response.Body = ms;

                        await next(context);  //将请求传递到下一个中间件

                        //响应数据 响应以及返回
                        //ms.Position = 0;     //设置流的读取位置从0开始
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        var responseBody =await new StreamReader(ms).ReadToEndAsync();

                        // 去除 Html
                        var reg = "<[^>]+>";
                        var isHtml = Regex.IsMatch(responseBody, reg);

                        //记录日志
                        logger.LogInformation($"Response Data：\r\n{responseBody}");

                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                        await ms.CopyToAsync(originalResponseBody);   //将响应复制到最开始的声明当中
                    }

                }
                catch (Exception ex)
                {
                    logger.LogError($"请求响应异常：{ex.Message}\r\n{ex.InnerException}");
                }
                finally
                {
                    context.Response.Body = originalResponseBody;
                }
            }
            else
            {
                await next(context);
            }
        }


    }
}
