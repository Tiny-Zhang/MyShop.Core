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
        private readonly IDatabase database;
        private readonly IOptionsSnapshot<DBConfig> dbConfig;

        public UserRepository(IDatabase _database, IOptionsSnapshot<DBConfig> _dbConfig)
        {
            database = _database;
            dbConfig = _dbConfig;
        }

        public Users QueryUserInfo(string name)
        {
            string sql = @"SELECT top 1 [Userid],[Username],[Password],[TrueName],[Sex],[Email],
                           [Birthday],[Head],[Mobile],[status],[islock],[regtime]
                           FROM [Users](nolock)
                           where Username=@Username";
            try
            {
                var result = database.Query<Users>(dbConfig.Value.DBReadOnlyConString, sql, new { Username = name}).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                //日志
                return null;
            }
        }

        public Task<Users> QueryUserInfoAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
