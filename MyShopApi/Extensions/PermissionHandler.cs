using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using MyShop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyShopApi.Extensions
{
    /// <summary>
    /// 过滤权限认证授权是否通过
    /// 在这里写业务处理逻辑
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor accessor;
        private readonly IAuthenticationSchemeProvider schemes;

        //public PermissionHandler(IHttpContextAccessor _accessor, IAuthenticationSchemeProvider _schemes)
        //{
        //    accessor = _accessor ?? throw new ArgumentNullException(nameof(_accessor));
        //    schemes = _schemes ?? throw new ArgumentNullException(nameof(_schemes));
        //}

        public PermissionHandler(IHttpContextAccessor _accessor)
        {
            accessor = _accessor ?? throw new ArgumentNullException(nameof(_accessor));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var httpContext = accessor.HttpContext;
            if (httpContext != null)
            {
                var questUrl = httpContext.Request.Path.Value.ToLower();

                //判断请求是否拥有凭据，即有没有登录
                //var defaultAuthenticate = await schemes.GetDefaultAuthenticateSchemeAsync();
                //if (defaultAuthenticate != null)
                //{
                //    var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                //    //result?.Principal不为空即登录成功
                //    if (result?.Principal != null)
                //    {
                //        //模拟用户当前角色，实际逻辑应该从数据库查询
                //        var userRoles = new List<string> { "Admin", "User" };

                //        //校验客户端传过来的角色权限
                //        var currentUserRoles = (from item in httpContext.User.Claims
                //                                where item.Type == requirement.ClaimType
                //                                select item.Value).ToList();
                //        var getUserRoels = userRoles.Where(o => currentUserRoles.Contains(o));

                //        //验证权限，不符合就返回
                //        if (currentUserRoles.Count <= 0)
                //        {
                //            context.Fail();
                //            return;
                //        }

                //        //验证过期 true表示有效
                //        var isExp = await Task.Run(() =>
                //         {
                //             return (httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) != null &&
                //                    DateTime.Parse(httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) >= DateTime.Now;
                //         });
                //        if (isExp)
                //        {
                //            context.Succeed(requirement);
                //        }
                //        else
                //        {
                //            context.Fail();
                //            return;
                //        }
                //        return;
                //    }
                //}


                //模拟用户当前角色，实际逻辑应该从数据库查询
                var userRoles = new List<string> { "Admin", "User" };

                //校验客户端传过来的角色权限
                var currentUserRoles = (from item in httpContext.User.Claims
                                        where item.Type == requirement.ClaimType
                                        select item.Value).ToList();
                var getUserRoels = userRoles.Where(o => currentUserRoles.Contains(o));

                //验证权限，不符合就返回
                if (currentUserRoles.Count <= 0)
                {
                    context.Fail();
                    return;
                }

                //验证过期 true表示有效
                var isExp = await Task.Run(() =>
                {
                    return (httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) != null &&
                           DateTime.Parse(httpContext.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Expiration)?.Value) >= DateTime.Now;
                });
                if (isExp)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                    return;
                }
                return;
                //判断没有登录时，是否访问登录的url,并且是Post请求
                //if (!(questUrl.Equals("/api/login/GetJwtToken", StringComparison.Ordinal) && (!httpContext.Request.Method.Equals("POST"))))
                //{
                //    context.Fail();
                //    return;
                //}


            }

        }
    }
}
