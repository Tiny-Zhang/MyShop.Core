using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShopApi.Extensions
{
    /// <summary>
    /// AutoMapper 全局注册类
    /// </summary>
    public class AutoMapperConfig
    {
        public static MapperConfiguration AutoMapperRegister()
        {
            return new MapperConfiguration(cfg =>
             {
                 cfg.AddProfile(new EntityToDtoProfile());
             });
        }
    }
}
