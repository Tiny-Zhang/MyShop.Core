using Castle.DynamicProxy;
using log4net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyShopApi
{
    /// <summary>
    /// Aop日志拦截类
    /// AOP是切面，是面向的service层的所有方法,不面向控制器，记录控制器用中间件
    /// 1、继承接口IInterceptor
    /// 2、实现接口IINterceptor的唯一方法Intercept
    /// 3、添加一个日志调用，用来记录被拦截的方法/参数等信息
    /// 4、服务层的方法调用前处理逻辑，用日志记录下来
    /// 5、调用服务层方法 void Proceed()，表示执行当前日志中记录的方法
    /// 6、object ReturnValue { get; set; }执行后调用，
    /// 7、object[] Arguments参数对象
    /// 8、将MyShopLogAop拦截器注入Autofac容器，代理服务层
    /// </summary>
    public class MyShopLogAop : IInterceptor
    {
        private readonly ILogger<MyShopLogAop> log;

        public MyShopLogAop(ILogger<MyShopLogAop> _log)
        {
            log = _log;
        }

        //private static readonly ILog log = LogManager.GetLogger(typeof(MyShopLogAop));
        public void Intercept(IInvocation invocation)
        {
            //服务方法调用前处理逻辑，用日志记录下来
            var methodName = invocation.Method.Name;
            var paramsList = string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray());
            var interceptinfo = $"【当前执行方法】：{methodName}\r\n" + $"【携带的参数有】：{paramsList}\r\n";

            try
            {
                //这句话表示在被拦截的方法执行完毕后 继续执行当前方法，注意是被拦截的是异步的
                invocation.Proceed();

                //事后处理: 在service层被执行了以后,做相应的处理,这里是输出到日志文件
                if (IsAsyncMethod(invocation.Method))
                {
                    //异步执行
                    var type = invocation.Method.ReturnType;
                    var resultProperty = type.GetProperty("Result");
                    //var typearg = invocation.Method.ReturnType.GenericTypeArguments[0];
                    interceptinfo += $"【执行完成结果】：{JsonConvert.SerializeObject(resultProperty.GetValue(invocation.ReturnValue))}";
                }
                else
                {
                    //同步执行
                    interceptinfo += $"【执行完成结果】：{invocation.ReturnValue}";
                }
                //输出到日志
                Task.Run(() => { log.LogWarning(interceptinfo); });
            }
            catch (Exception ex)
            {
                interceptinfo += $"【执行异常】：{ex.Message + "," + ex.InnerException}";
                log.LogError(interceptinfo);
            }



        }


        public static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }
    }
}
