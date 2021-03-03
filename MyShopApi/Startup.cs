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
        ///  侧重于注册或者添加支持某个组件
        ///  当你需要用到某个第三方组件时，在这里添加之后就可以使项目支持了
        ///  比如：要想使项目支持跨域时， 添加相应包之后，就需要在这里使用services.AddCors 添加组件支持
        ///  再比如：做认证授权时，想要使用第三方JWT组件生成token之类的内容时，也需要在这里添加相关支持
        ///  再比如：读取配置文件时，也需要在这里做相应操作
        ///  等等。。。
        ///  它就类似一台组装台式机，当需要硬盘时，在这里安装硬盘的注册支持就可以使用；当需要内存条时，同样在这里注册支持才能使用
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //方式1
            //读取配置文件appsettings.json，将自定义节点绑定到DBConfig类中，在任意地方直接使用
            //配置文件更新，绑定的值不会实时更新
            SystemContext.dbConfig = Configuration.GetSection("DBConfig").Get<DBConfig>();

            //方式2 
            //将DBConfig对象注册到Service中，这样可以在Controller中注入使用
            //配置文件更新，绑定的值会实时更新
            services.Configure<DBConfig>(Configuration.GetSection("DBConfig"));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// 侧重于客户端的http请求过滤，或者说请求拦截，配置如何去处理http请求
        /// 当客户端发起一个http请求时，会进入该方法，按照从上到下的顺序依次执行，
        /// 当执行到某个业务时，如果不符合业务规则，会直接返回，并根据你设置的程序逻辑给出相应提示！
        /// 比如：原来MVC中的路由配置直接挪到了这里
        /// 比如：当你做认证授权过滤时，就要在这里配置一下，然后登录时会自动先经过这里做逻辑验证
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
        /// Autofac引用
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacExtension());
        }
    }
}
