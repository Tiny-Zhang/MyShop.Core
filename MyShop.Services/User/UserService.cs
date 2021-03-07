using MyShop.Model.Entitys;
using MyShop.Repository;
using System;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository _userRepository)
        {
            userRepository = _userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
        }

        public Users QueryUserInfo(string name)
        {
            return userRepository.QueryUserInfo(name);
        }

        public async Task<Users> QueryUserInfoAsync(string name)
        {
            return await userRepository.QueryUserInfoAsync(name);
        }


    }
}
