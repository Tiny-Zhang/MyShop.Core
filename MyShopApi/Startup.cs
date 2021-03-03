using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyShop.Common;
using MyShop.Services;

namespace MyShopApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        /// <summary>
        ///  This method gets called by the runtime. Use this method to add services to the container.
        ///  ������ע��������֧��ĳ�����
        ///  ������Ҫ�õ�ĳ�����������ʱ�����������֮��Ϳ���ʹ��Ŀ֧����
        ///  ���磺Ҫ��ʹ��Ŀ֧�ֿ���ʱ�� �����Ӧ��֮�󣬾���Ҫ������ʹ��services.AddCors ������֧��
        ///  �ٱ��磺����֤��Ȩʱ����Ҫʹ�õ�����JWT�������token֮�������ʱ��Ҳ��Ҫ������������֧��
        ///  �ٱ��磺��ȡ�����ļ�ʱ��Ҳ��Ҫ����������Ӧ����
        ///  �ȵȡ�����
        ///  ��������һ̨��װ̨ʽ��������ҪӲ��ʱ�������ﰲװӲ�̵�ע��֧�־Ϳ���ʹ�ã�����Ҫ�ڴ���ʱ��ͬ��������ע��֧�ֲ���ʹ��
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //��ʽ1
            //��ȡ�����ļ�appsettings.json�����Զ���ڵ�󶨵�DBConfig���У�������ط�ֱ��ʹ��
            //�����ļ����£��󶨵�ֵ����ʵʱ����
            SystemContext.dbConfig = Configuration.GetSection("DBConfig").Get<DBConfig>();

            //��ʽ2 
            //��DBConfig����ע�ᵽService�У�����������Controller��ע��ʹ��
            //�����ļ����£��󶨵�ֵ��ʵʱ����
            services.Configure<DBConfig>(Configuration.GetSection("DBConfig"));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// �����ڿͻ��˵�http������ˣ�����˵�������أ��������ȥ����http����
        /// ���ͻ��˷���һ��http����ʱ�������÷��������մ��ϵ��µ�˳������ִ�У�
        /// ��ִ�е�ĳ��ҵ��ʱ�����������ҵ����򣬻�ֱ�ӷ��أ������������õĳ����߼�������Ӧ��ʾ��
        /// ���磺ԭ��MVC�е�·������ֱ��Ų��������
        /// ���磺��������֤��Ȩ����ʱ����Ҫ����������һ�£�Ȼ���¼ʱ���Զ��Ⱦ����������߼���֤
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        /// <summary>
        /// Autofac����
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacExtension());
        }
    }
}
