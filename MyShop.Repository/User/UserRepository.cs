using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyShop.Common;
using MyShop.Model.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataBase database;
        private readonly ILogger<UserRepository> logger;

        public UserRepository(IDataBase _database, ILogger<UserRepository> _logger)
        {
            database = _database;
            logger = _logger;
        }

        public Users QueryUserInfo(string name)
        {
            string sql = @"SELECT top 1 [Userid],[Username],[Password],[TrueName],[Sex],[Email],
                           [Birthday],[Head],[Mobile],[status],[islock],[regtime]
                           FROM [Users](nolock)
                           where Username=@Username";
            try
            {
                var result = database.QueryFirst<Users>(sql, new { Username = name });
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError($"同步获取用户信息异常：{ex.Message},参数：{name}");
                return null;
            }
        }

        public async Task<Users> QueryUserInfoAsync(string name)
        {
            string sql = @"SELECT top 1 [Userid],[Username],[Password],[TrueName],[Sex],[Email],
                           [Birthday],[Head],[Mobile],[status],[islock],[regtime]
                           FROM [Users](nolock)
                           where Username=@Username";
            try
            {
                var result = await database.QueryFirstAsync<Users>(sql, new { Username = name });
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError($"异步获取用户信息异常：{ex.Message},参数：{name}");
                return null;
            }
        }

        /// <summary>
        /// 通过用户名密码获取用户信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task<Users> QueryUserInfoAsync(string name, string pwd)
        {
            string sql = @"SELECT [Userid],[Username],[Password],[TrueName],[Sex],[Email],
                           [Birthday],[Head],[Mobile],[status],[islock],[regtime]
                           FROM [Users](nolock)
                           where islock=0 AND status=1 and Username=@Username AND Password=@Password";
            try
            {
                var result = await database.QueryFirstAsync<Users>(sql, new { Username = name, Password = pwd });
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError($"通过用户名密码获取用户信息异常：{ex.Message},参数：{name}");
                return null;
            }
        }
    }
}
