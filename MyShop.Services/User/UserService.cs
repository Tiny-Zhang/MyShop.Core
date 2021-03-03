using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public class UserService : IUserService
    {
        public string QueryInfo(string name)
        {
            return name;
        }

        public Task<string> QueryInfoAsync(string name)
        {
            return Task.Run(() => { return name; });
        }
    }
}
