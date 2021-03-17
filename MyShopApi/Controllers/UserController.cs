using log4net;
using Microsoft.AspNetCore.Authorization;
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
    //[Authorize]   //认证授权
    public class UserController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UserController));
        private readonly IUserService userService;

        public UserController(IUserService _userService)
        {
            userService = _userService;
        }

        /// <summary>
        /// FromForm 是以表单的形式接收
        /// FromBody 是以Json的形式接收
        /// </summary>
        /// <param name="usersDto"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Roles = "Admin")]    //Admin角色才可以访问该接口
        //[Authorize(Policy = "CustomPolicy")]   //自定义授权
        public async Task<IActionResult> GetUserInfo([FromBody] UsersDto usersDto)
        {
            var name = usersDto.Username ?? throw new ArgumentNullException("Username不能为Null");

            //Parallel.For 可以并行循环执行https://blog.csdn.net/woaipangruimao/article/details/79800587
            //ConcurrentDictionary<string, UsersDto> dict = new ConcurrentDictionary<string, UsersDto>();
            //ParallelLoopResult paraResult = Parallel.For(0, 100, async (i, state) =>
            //{
            //    log.Debug($"迭代次数：{i},任务ID:{Task.CurrentId},线程ID:{Thread.CurrentThread.ManagedThreadId}");
            //    dict.GetOrAdd(i.ToString(), await userService.QueryUserInfoAsync(name));
            //});
            //if (paraResult.IsCompleted) return Ok(dict);

            var result = await userService.QueryUserInfoAsync(name);
            return Ok(result);
        }

        /// <summary>
        /// 获取安全密钥 
        /// length = 16/32
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSecretKey(int? length)
        {
            var name = length ?? throw new ArgumentNullException("参数length不能为Null");
            var result = await Task.Run(() =>
            {
                return Encryption.GetEncrytionKey((int)length);
            });
            return Ok(result);
        }




    }
}
