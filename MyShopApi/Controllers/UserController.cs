using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyShop.Common;
using MyShop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShopApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IOptionsSnapshot<DBConfig> dbConfig;   //通过注入方式读取配置文件

        public UserController(IUserService _userService, IOptionsSnapshot<DBConfig> _dbConfig)
        {
            userService = _userService;
            dbConfig = _dbConfig;
        }

        public async Task<IActionResult> GetUserInfo()
        {
            var result = await userService.QueryInfoAsync("Hello World");
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            var result = await Task.Run(() =>
            {
                //方式1读取配置文件  不会实施更新内容
                var dbType = SystemContext.dbConfig.DBType.ToLower();
                return dbType;
            });

            //方式2 读取配置文件  可以实施更新内容
            var result2 = dbConfig.Value.DBType.ToLower();

            return Ok("方式1：" + result + "，方式2：" + result2);
        }




    }
}
