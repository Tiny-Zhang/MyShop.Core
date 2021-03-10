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

            services.AddControllers()
            //ȫ������Json
            .AddNewtonsoftJson(options =>
            {
                //����ѭ������
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //��ʹ���շ���ʽ��key
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //���ñ���ʱ�����UTCʱ��
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            });

            #region �����ļ�
            //��ʽ1
            //��ȡ�����ļ�appsettings.json�����Զ���ڵ�󶨵�DBConfig���У�������ط�ֱ��ʹ��
            //�����ļ����£��󶨵�ֵ����ʵʱ����
            SystemContext.dbConfig = Configuration.GetSection("DBConfig").Get<DBConfig>();
            SystemContext.jwtConfig = Configuration.GetSection("JwtAuthorizeConfig").Get<JwtAuthorizeConfig>();

            //��ʽ2 
            //��DBConfig����ע�ᵽService�У�����������Controller��ע��ʹ��
            //�����ļ����£��󶨵�ֵ��ʵʱ����
            services.Configure<DBConfig>(Configuration.GetSection("DBConfig"));

            #endregion

            //ע����֤(ʹ��JWT��֤)
            var symmetricKeyAsBase64 = SystemContext.jwtConfig.Secret;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var Issuer = SystemContext.jwtConfig.Issuer;  //������
            var Audience = SystemContext.jwtConfig.Audience; //������
            var exp = 20;  //��Чʱ��(��) 60s����
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opetion =>
             {
                 opetion.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,   //�Ƿ�����Կ��֤
                     IssuerSigningKey = signingKey,     //��Կ

                     ValidateIssuer = true,
                     ValidIssuer = Issuer,     //������

                     ValidateAudience = true,
                     ValidAudience = Audience,  //������

                     ValidateLifetime = true,   //��֤��������
                     ClockSkew = TimeSpan.FromSeconds(exp),   //��Чʱ��
                     RequireExpirationTime = true   //��֤����ʱ��
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
                         // ������ڣ����<�Ƿ����>��ӵ�������ͷ��Ϣ��
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

            //������֤
            app.UseAuthentication();

            //������Ȩ
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
