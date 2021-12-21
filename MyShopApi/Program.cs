using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MyShopApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// 默认初始化系统内置的配置
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())   //autofac 依赖注入
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    //.UseUrls("http://*:5000")    //表示设置当前应用端口是5000,可以从配置文件读取
                    .ConfigureLogging((hostingContext, builder) =>
                    {
                        //1.过滤掉系统默认的一些日志
                        //2.这里配置后，appsettings.json中的Logging配置将失效
                        builder.AddFilter("System", LogLevel.Error);     //只有错误时才打印日志
                        builder.AddFilter("Microsoft", LogLevel.Error);  //只有错误时才打印日志
                        builder.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);  //只有错误时才打印日志
                        builder.AddFilter("MyShopApi", LogLevel.Debug);  //使用项目命名空间 自定义项目过滤级别

                        //清除console控制台打印的日志
                        //builder.ClearProviders();

                        //3.统一设置  不知道为什么这里没有生效
                        //builder.SetMinimumLevel(LogLevel.Warning);

                        //默认log4net.confg
                        //Windows不区分大小写，这里这么写为了适应Linux平台
                        builder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config"));
                    });
                });
    }
}
