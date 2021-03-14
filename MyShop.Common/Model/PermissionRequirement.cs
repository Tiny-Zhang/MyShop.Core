using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Common
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// 认证授权类型(是基于角色、还是订单、还是其他等等)
        /// </summary>
        public string ClaimType { get; set; }
        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 订阅人
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan Expiration { get; set; }
        /// <summary>
        /// 签名验证
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }

        //用户权限列表
        public List<string> RolesList { get; set; }


        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="claimType">声明类型</param>
        /// <param name="issuer">发行人</param>
        /// <param name="audience">订阅人</param>
        /// <param name="signingCredentials">签名验证实体</param>
        /// <param name="expiration">过期时间</param>
        public PermissionRequirement(string claimType,
            string issuer,
            string audience,
            SigningCredentials signingCredentials,
            TimeSpan expiration,
            List<string> rolesList)
        {
            ClaimType = claimType;
            Issuer = issuer;
            Audience = audience;
            Expiration = expiration;
            SigningCredentials = signingCredentials;
            RolesList = rolesList;
        }
    }
}
