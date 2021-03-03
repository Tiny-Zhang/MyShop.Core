using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public interface IUserService
    {
        string QueryInfo(string name);
        Task<string> QueryInfoAsync(string name);

    }
}
