using Autofac;
using MyShop.Services;
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
        //重写Load函数
        protected override void Load(ContainerBuilder builder)
        {
            //注册电话接口
            builder.RegisterType<MobileNotice>().Named<IMessageNotice>("mobile");

            //注册微信接口
            builder.RegisterType<WebChatNotice>().Named<IMessageNotice>("webchat");

            //注册管理类 自己注册自己
            //builder.RegisterType<NoticeManager>();

            //批量注册程序集 没有接口，并且以Manager结尾的类,可以代替上边的 NoticeManager类注册
            var myshopapibll = Assembly.Load("MyShopApi");
            builder.RegisterAssemblyTypes(myshopapibll)
            .Where(t => t.Name.EndsWith("Manager"));


            //批量注册程序集  有接口
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
            .InstancePerDependency();    //瞬时模式
        }
    }
}
