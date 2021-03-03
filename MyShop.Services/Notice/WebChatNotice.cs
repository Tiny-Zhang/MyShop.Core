using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    /// <summary>
    /// 微信通知
    /// </summary>
    public class WebChatNotice : IMessageNotice
    {
        //普通发送
        public string Send(string arg)
        {
            return  $"微信通知系统，通知内容：{arg}";
        }

        //异步发送
        public Task<string> SendAsync(string arg)
        {
            return Task.Run(() => { return $"微信通知系统，通知内容：{arg}"; });
        }

    }
}
