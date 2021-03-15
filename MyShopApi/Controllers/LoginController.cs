using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyShop.Common;
using MyShop.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyShopApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly PermissionRequirement requirement;
        private readonly IUserService userService;

        public LoginController(PermissionRequirement _requirement, IUserService _userService)
        {
            requirement = _requirement ?? throw new ArgumentNullException(nameof(_requirement));
            userService = _userService ?? throw new ArgumentNullException(nameof(_userService));
        }

        /// <summary>
        /// 获取token  函数作废，使用GetJwtToken代替
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetToken(string name, string pwd)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pwd))
            {
                return Ok(new
                {
                    success = false,
                    msg = "用户名或密码不能为空",
                });
            }

            //pwd = Encryption.MD5(pwd);
            //然后从数据库查询用户信息
            //如果存在用户信息, 

            var Uid = 123567;
            var symmetricKeyAsBase64 = SystemContext.jwtConfig.Secret;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var Issuer = SystemContext.jwtConfig.Issuer;    //发布人
            var Audience = SystemContext.jwtConfig.Audience; //订阅人
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);  //加密方式
            var exp = 20;  //有效时长(秒) 60s过期
            var claims = new List<Claim>()
            {
                new Claim("Name", name),  //用户名
                new Claim("Uid", Uid.ToString()),   //用户Id
                new Claim(ClaimTypes.Role, "Admin"),   //角色指定为Admin,可以根据用户信息动态指定角色
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(exp).ToString())
            };

            var jwt = new JwtSecurityToken(
               issuer: Issuer,
               audience: Audience,
               claims: claims,
               expires: DateTime.Now.AddSeconds(exp),
               signingCredentials: creds
            );

            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenStr = jwtHandler.WriteToken(jwt);

            return Ok(new
            {
                success = true,
                token = tokenStr,
                msg = "获取成功"
            });
        }


        /// <summary>
        /// 获取Jwt生成的Token
        /// 升级版
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetJwtToken(string name, string pwd)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pwd))
            {
                return Ok(new
                {
                    success = false,
                    msg = "用户名或密码不能为空",
                });
            }

            pwd = Encryption.MD5(pwd);
            var getuserInfo = await userService.QueryUserInfoAsync(name, pwd);
            if (getuserInfo != null && getuserInfo.Userid > 0)
            {
                //模拟用户角色，实际逻辑应该从数据库查询
                var userRoles = new List<string> { "Admin", "User" };  

                var claims = new List<Claim>()
                {
                    new Claim("Name", getuserInfo.Username),  //用户名
                    new Claim("Uid", getuserInfo.Userid.ToString()),   //用户Id
                    new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(requirement.Expiration.TotalSeconds).ToString())
                    //其他信息
                };
                claims.AddRange(userRoles.Select(o => new Claim("Roles", o)));  //加入角色

                var tokenStr = JwtTokenHelper.BuildJwtToken(claims.ToArray(), requirement);
                return Ok(new
                {
                    success = true,
                    token = tokenStr,
                    msg = "获取成功"
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    msg = "获取失败"
                });
            }

        }

    }
}
