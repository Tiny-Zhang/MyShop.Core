using AutoMapper;
using MyShop.Model.Entitys;
using MyShop.Model.EntitysDto;
using MyShop.Repository;
using System;
using System.Threading.Tasks;

namespace MyShop.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UserService(IUserRepository _userRepository,IMapper _mapper)
        {
            userRepository = _userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
            mapper = _mapper;
        }

        public UsersDto QueryUserInfo(string name)
        {
            return mapper.Map<UsersDto>(userRepository.QueryUserInfo(name));
        }

        public async Task<UsersDto> QueryUserInfoAsync(string name)
        {
            var result= mapper.Map<UsersDto>(await userRepository.QueryUserInfoAsync(name));
            return result;
        }


    }
}
