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
        /// Ĭ�ϳ�ʼ��ϵͳ���õ�����
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())   //autofac ����ע��
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .ConfigureLogging((hostingContext, builder) =>
                    {
                        // 1.���˵�ϵͳĬ�ϵ�һЩ��־
                        builder.AddFilter("System", LogLevel.Error);
                        builder.AddFilter("Microsoft", LogLevel.Error);

                        // 2.Ҳ������appsettings.json�����ã�LogLevel�ڵ�

                        // 3.ͳһ����
                        builder.SetMinimumLevel(LogLevel.Error);

                        // Ĭ��log4net.confg
                        builder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config"));
                    });
                });
    }
}
