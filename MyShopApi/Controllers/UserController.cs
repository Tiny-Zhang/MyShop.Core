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
        private readonly IUserService userService;
        private readonly ILogger<UserController> logger;

        public UserController(IUserService _userService, ILogger<UserController> _logger)
        {
            userService = _userService;
            logger = _logger;
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
            //ConcurrentDictionary<string, Users> dict = new ConcurrentDictionary<string, Users>();
            List<Task> list = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                var result = Task.Run(() =>
                {
                    logger.LogDebug("当前线程：" + Thread.CurrentThread.ManagedThreadId.ToString());
                    return userService.QueryUserInfo(name);
                });
                list.Add(result);
                //dict.GetOrAdd(i.ToString(), result.Result);
            }
            Task.WaitAll(list.ToArray());
            return Ok(list);
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
