using MyShop.Model.Entitys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public interface IUserService
    {
        Users QueryUserInfo(string name);
        Task<Users> QueryUserInfoAsync(string name);

    }
}
