using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyShop.Common;
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

        /// <summary>
        /// 获取token
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
                new Claim(ClaimTypes.Name, name),  //用户名
                new Claim(ClaimTypes.Sid, Uid.ToString()),   //用户Id
                new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(exp).ToString()),
                new Claim(JwtRegisteredClaimNames.Iss,Issuer),
                new Claim(JwtRegisteredClaimNames.Aud,Audience)
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



    }
}
