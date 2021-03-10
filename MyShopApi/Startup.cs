using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MyShop.Common;
using MyShop.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

            services.AddControllers()
            //全局配置Json
            .AddNewtonsoftJson(options =>
            {
                //忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //不使用驼峰样式的key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //设置本地时间而非UTC时间
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            });

            #region 配置文件
            //方式1
            //读取配置文件appsettings.json，将自定义节点绑定到DBConfig类中，在任意地方直接使用
            //配置文件更新，绑定的值不会实时更新
            SystemContext.dbConfig = Configuration.GetSection("DBConfig").Get<DBConfig>();
            SystemContext.jwtConfig = Configuration.GetSection("JwtAuthorizeConfig").Get<JwtAuthorizeConfig>();

            //方式2 
            //将DBConfig对象注册到Service中，这样可以在Controller中注入使用
            //配置文件更新，绑定的值会实时更新
            services.Configure<DBConfig>(Configuration.GetSection("DBConfig"));

            #endregion

            //注册认证(使用JWT认证)
            var symmetricKeyAsBase64 = SystemContext.jwtConfig.Secret;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var Issuer = SystemContext.jwtConfig.Issuer;  //发布人
            var Audience = SystemContext.jwtConfig.Audience; //订阅人
            var exp = 20;  //有效时长(秒) 60s过期
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opetion =>
             {
                 opetion.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,   //是否开启密钥认证
                     IssuerSigningKey = signingKey,     //密钥

                     ValidateIssuer = true,
                     ValidIssuer = Issuer,     //发行人

                     ValidateAudience = true,
                     ValidAudience = Audience,  //订阅人

                     ValidateLifetime = true,   //验证生命周期
                     ClockSkew = TimeSpan.FromSeconds(exp),   //有效时长
                     RequireExpirationTime = true   //验证过期时间
                 };
                 opetion.Events = new JwtBearerEvents
                 {
                     OnChallenge = context =>
                     {
                         context.Response.Headers.Add("Token-Error", context.ErrorDescription);
                         return Task.CompletedTask;
                     },
                     OnAuthenticationFailed = context =>
                     {
                         var token = context.Request.Headers["Authorization"].ToString().Trim().Replace("Bearer ", "");
                         var jwtToken = (new JwtSecurityTokenHandler()).ReadJwtToken(token);

                         if (jwtToken.Issuer != Issuer)
                         {
                             context.Response.Headers.Add("Issuer", "issuer is wrong!");
                         }

                         if (jwtToken.Audiences.FirstOrDefault() != Audience)
                         {
                             context.Response.Headers.Add("Audience", "Audience is wrong!");
                         }
                         // 如果过期，则把<是否过期>添加到，返回头信息中
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                         {
                             context.Response.Headers.Add("Expired", "Token Expired!");
                         }
                         return Task.CompletedTask;
                     }
                 };
             });


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

            //开启认证
            app.UseAuthentication();

            //开启授权
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
