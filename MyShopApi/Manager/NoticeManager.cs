using Autofac;
using MyShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShopApi
{
    /// <summary>
    /// 通知管理类
    /// </summary>
    public class NoticeManager
    {
        private readonly ILifetimeScope lifetimeScope;                //ILifetimeScope 是Autofac内置类
        private readonly IEnumerable<IMessageNotice> messageNotices;  //使用IEnumerable<IMessageNotice>格式注入接口
        public NoticeManager(ILifetimeScope _lifetimeScope, IEnumerable<IMessageNotice> _messageNotices)
        {
            lifetimeScope = _lifetimeScope ?? throw new ArgumentNullException(nameof(_lifetimeScope));
            messageNotices = _messageNotices ?? throw new ArgumentNullException(nameof(_messageNotices));
        }


        //发送通知
        public List<string> SendNotice(string arg)
        {
            var list = new List<string>();

            //方式1：使用 ResolveNamed<T>方法区分
            //var mobile = lifetimeScope.ResolveNamed<IMessageNotice>("mobile");   //电话通知
            //var webchat = lifetimeScope.ResolveNamed<IMessageNotice>("webchat");   //微信通知


            //方式2 使用IEnumerable<IMessageNotice> 获取实例
            var mobile = messageNotices.Where(t=>t.GetType()==typeof(MobileNotice)).FirstOrDefault();   //电话通知
            var webchat = messageNotices.Where(t => t.GetType() == typeof(WebChatNotice)).FirstOrDefault();   //微信通知


            list.Add(mobile.Send(arg));   //发送电话通知
            list.Add(webchat.Send(arg));  //发送微信通知

            return list;
        }
    }
}
