using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MyShop.Common;
using MyShopApi.Extensions;
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

            #region JWT��֤
            //ע����֤(ʹ��JWT��֤)
            var symmetricKeyAsBase64 = SystemContext.jwtConfig.Secret;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var Issuer = SystemContext.jwtConfig.Issuer;  //������
            var Audience = SystemContext.jwtConfig.Audience; //������
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                //o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opetion =>
             {
                 //��֤����
                 opetion.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,   //�Ƿ�����Կ��֤
                     IssuerSigningKey = signingKey,     //��Կ

                     ValidateIssuer = true,
                     ValidIssuer = Issuer,     //������

                     ValidateAudience = true,
                     ValidAudience = Audience,  //������

                     ValidateLifetime = true,   //��֤��������
                     ClockSkew = TimeSpan.FromSeconds(30),  //exp����ʱ�����ƫ��ʱ��(Ĭ��5����)
                     RequireExpirationTime = true   //��֤����ʱ��
                 };
                 opetion.Events = new JwtBearerEvents
                 {
                     //δ��Ȩʱ����
                     OnChallenge = context =>
                     {
                         context.Response.Headers.Add("Token-Error", context.ErrorDescription);
                         return Task.CompletedTask;
                     },
                     //��֤ʧ��ʱ����
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
                         // ������ڣ���ӵ�����ͷ��Ϣ��
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                         {
                             context.Response.Headers.Add("Expired", "Token Expired!");
                         }
                         return Task.CompletedTask;
                     }
                     //��Token��֤ͨ�������
                     //OnTokenValidated = context =>
                     //{
                     //    return Task.CompletedTask;
                     //}
                 };
             });
            #endregion

            #region JWT��Ȩ
            // ��ɫ��ӿڵ�Ȩ��Ҫ�����
            var rolesList = new List<string>();   //��ɫ�б���¼ʱ�ٸ�ֵ
            var roletype = "Roles";
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var permissionRequirement = new PermissionRequirement(
                claimType: roletype,   //�Զ�����Ȩ����
                issuer: Issuer,      //������
                audience: Audience,    //����
                signingCredentials: signingCredentials,//ǩ��ƾ��
                expiration: TimeSpan.FromSeconds(60 * 5),  //�ӿڵĹ���ʱ��
                rolesList: rolesList);
            services.AddAuthorization(options =>
            {
                //��ɫ��Ȩ��ʹ�÷�ʽ[Authorize(Policy="AdminPolicy")]
                //��Ҫ������token��claims��ָ����ɫ���磺new Claim(ClaimTypes.Role, "Admin")
                options.AddPolicy("AdminPolicy", o =>
                {
                    o.RequireRole("Admin").Build();
                });

                //�Զ�����Ȩ����,ʹ�÷�ʽ [Authorize(Policy="Permission")]
                options.AddPolicy("CustomPolicy", o =>
                {
                    o.Requirements.Add(permissionRequirement);
                });
            });

            services.AddScoped<IAuthorizationHandler, PermissionHandler>();  //ע��Ȩ�޴���
            services.AddSingleton(permissionRequirement);  //����ע��
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #endregion

            #region ����

            //֧�ֶ�������˿ڣ�ע��˿ںź�Ҫ��/б�ˣ�����localhost:8000/���Ǵ��
            //--http://127.0.0.1:1818 �� --http://localhost:1818 �ǲ�һ���ģ�����д����
            var origins = new List<string> { "http://127.0.0.1:8080", "http://localhost:8080", "http://192.168.0.7:8088" };
            services.AddCors(c =>
            {
                // ���ò���
                c.AddPolicy("CorsRequests", policy =>
                {
                    policy
                    .WithOrigins(origins.ToArray())
                    .AllowAnyHeader()   //��������ͷ
                    .AllowAnyMethod();  //�������ⷽ��
                });
            });

            #endregion

            #region AutoMapper
            services.AddAutoMapper(typeof(AutoMapperConfig));
            AutoMapperConfig.AutoMapperRegister();
            #endregion

            //SqlSugar ע��
            //services.AddSqlSugar(new IocConfig()
            //{
            //    ConnectionString =Configuration["SqlServerString"],
            //    DbType = IocDbType.SqlServer,
            //    IsAutoCloseConnection = true    //�Զ��ر�����
            //});


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

            //���Cors �����м��
            app.UseCors("CorsRequests");

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
