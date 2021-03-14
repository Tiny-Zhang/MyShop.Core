using MyShop.Model.Entitys;
using MyShop.Model.EntitysDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public interface IUserService
    {
        UsersDto QueryUserInfo(string name);
        Task<UsersDto> QueryUserInfoAsync(string name);

        Task<UsersDto> QueryUserInfoAsync(string name, string pwd);

    }
}
