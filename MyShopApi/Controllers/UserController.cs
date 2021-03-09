using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyShop.Common;
using MyShop.Model.Entitys;
using MyShop.Model.EntitysDto;
using MyShop.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyShopApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserController));
        private readonly IUserService userService;
        //private readonly ILogger<UserController> logger;

        public UserController(IUserService _userService)
        {
            userService = _userService;
        }

        /// <summary>
        /// FromForm 是以表单的形式接收
        /// </summary>
        /// <param name="usersDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetUserInfo([FromForm] UsersDto usersDto)
        {
            var name = usersDto.Username ?? throw new ArgumentNullException("Username不能为Null");
            ConcurrentDictionary<string, Users> dict = new ConcurrentDictionary<string, Users>();
            for (int i = 0; i < 100; i++)
            {
                log.Fatal($"我是Fatal日志。。。{i}");
                log.Error($"我是Error日志。。。{i}");
                log.Info($"我是Info日志。。。{i}");
                log.Debug($"我是Debug日志。。。{i}");
                dict.GetOrAdd(i.ToString(), await userService.QueryUserInfoAsync(name));
            }
            return Ok(dict);
        }

        /// <summary>
        /// FromBody 是以Json的形式接收
        /// </summary>
        /// <param name="usersDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetUserInfo2([FromBody] UsersDto usersDto)
        {
            var name = usersDto.Username ?? throw new ArgumentNullException("Username不能为Null");
            var result = await Task.Run(() =>
            {
                return userService.QueryUserInfo(name);
            });
            return Ok(result);
        }




    }
}
