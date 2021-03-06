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
                        // 1.过滤掉系统默认的一些日志
                        builder.AddFilter("System", LogLevel.Warning);
                        builder.AddFilter("Microsoft", LogLevel.Warning);

                        // 2.也可以在appsettings.json中配置，LogLevel节点


                        // 3.统一设置
                        //builder.SetMinimumLevel(LogLevel.Warning);

                        // 默认log4net.confg
                        builder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config"));
                    });
                });
    }
}
