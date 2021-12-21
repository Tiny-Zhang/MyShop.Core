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
                    //.UseUrls("http://*:5000")    //��ʾ���õ�ǰӦ�ö˿���5000,���Դ������ļ���ȡ
                    .ConfigureLogging((hostingContext, builder) =>
                    {
                        //1.���˵�ϵͳĬ�ϵ�һЩ��־
                        //2.�������ú�appsettings.json�е�Logging���ý�ʧЧ
                        builder.AddFilter("System", LogLevel.Error);     //ֻ�д���ʱ�Ŵ�ӡ��־
                        builder.AddFilter("Microsoft", LogLevel.Error);  //ֻ�д���ʱ�Ŵ�ӡ��־
                        builder.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);  //ֻ�д���ʱ�Ŵ�ӡ��־
                        builder.AddFilter("MyShopApi", LogLevel.Debug);  //ʹ����Ŀ�����ռ� �Զ�����Ŀ���˼���

                        //���console����̨��ӡ����־
                        //builder.ClearProviders();

                        //3.ͳһ����  ��֪��Ϊʲô����û����Ч
                        //builder.SetMinimumLevel(LogLevel.Warning);

                        //Ĭ��log4net.confg
                        //Windows�����ִ�Сд��������ôдΪ����ӦLinuxƽ̨
                        builder.AddLog4Net(Path.Combine(Directory.GetCurrentDirectory(), "Log4net.config"));
                    });
                });
    }
}
