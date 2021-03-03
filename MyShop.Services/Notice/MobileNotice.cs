using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    /// <summary>
    /// 实现类1 电话通知
    /// </summary>
    public class MobileNotice : IMessageNotice
    {
        public string Send(string arg)
        {
            return $"电话通知系统，通知内容：{arg}";
        }

        public Task<string> SendAsync(string arg)
        {
            return Task.Run(() => { return $"电话通知系统，通知内容：{arg}"; });
        }
    }
}
