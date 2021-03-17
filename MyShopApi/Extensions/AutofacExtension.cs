using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.AspNetCore.Authorization;
using MyShop.Common;
using MyShop.Services;
using MyShopApi.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyShopApi
{
    public class AutofacExtension : Autofac.Module
    {
        /// <summary>
        /// 重写Load函数
        /// Autofac.Extras.DynamicProxy（Autofac的动态代理，它依赖Autofac，所以可以不用单独引入Autofac）
        /// Autofac.Extensions.DependencyInjection（Autofac的扩展）
        /// InstancePerLifetimeScope 在一次Api请求范围内获取到的实例都是一样的(请求开始-请求结束 在这次请求中获取的对象都是同一个)
        /// SingleInstance  全局单例
        /// InstancePerDependency  瞬时模式
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            #region 单独注册
            //注册电话接口
            //builder.RegisterType<MobileNotice>().Named<IMessageNotice>("mobile");

            //注册微信接口
            //builder.RegisterType<WebChatNotice>().Named<IMessageNotice>("webchat");

            //注册管理类 自己注册自己
            //builder.RegisterType<NoticeManager>();

            //数据库接口
            builder.RegisterType<DapperHelper>().As<IDataBase>();

            //Aop接口
            var aopType = new List<Type>();
            builder.RegisterType<MyShopLogAop>();  //执行日志拦截
            aopType.Add(typeof(MyShopLogAop));

            #endregion

            #region  批量注册

            //批量注册程序集 没有接口，并且以Manager结尾的类,可以代替上边的 NoticeManager类注册
            var myshopapibll = Assembly.Load("MyShopApi");
            builder.RegisterAssemblyTypes(myshopapibll)
            .Where(t => t.Name.EndsWith("Manager"));


            //批量注册程序集  有接口  服务层
            var basedir = AppContext.BaseDirectory;
            var bllservice = Path.Combine(basedir, "MyShop.Services.dll");
            if (!File.Exists(bllservice))
            {
                var msg = "MyShop.Services.dll不存在";
                throw new Exception(msg);
            }
            var assemblysServices = Assembly.LoadFrom(bllservice);  //Assembly.Load("MyShop.Services");
            builder.RegisterAssemblyTypes(assemblysServices)   //多个程序集用逗号","分割
            .AsImplementedInterfaces()   //批量关联，让所有注册类型自动与其继承的接口进行关联
            .InstancePerLifetimeScope()    //在一次请求范围内获取到的实例都是一样的
            .EnableInterfaceInterceptors()      //对目标类型启用接口拦截,需要引用：Autofac.Extras.DynamicProxy
            .InterceptedBy(aopType.ToArray());  //AOP拦截器集合,将拦截器添加到要注入容器的接口或者类之上


            //批量注册程序集  有接口  仓储层
            var dalservice = Path.Combine(basedir, "MyShop.Repository.dll");
            if (!File.Exists(dalservice))
            {
                var msg = "MyShop.Repository.dll不存在";
                throw new Exception(msg);
            }
            var assemblysRepository = Assembly.LoadFrom(dalservice);
            builder.RegisterAssemblyTypes(assemblysRepository)   //多个程序集用逗号","分割
            .AsImplementedInterfaces()   //批量关联，让所有注册类型自动与其继承的接口进行关联
            .InstancePerLifetimeScope();    //在一次请求范围内获取到的实例都是一样的

            #endregion

        }
    }
}
