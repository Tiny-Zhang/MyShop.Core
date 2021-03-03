using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public interface IMessageNotice
    {
        string Send(string arg);  //发送通知
        Task<string> SendAsync(string arg);  //异步发送通知
    }
}
