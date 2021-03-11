using AutoMapper;
using MyShop.Model.Entitys;
using MyShop.Model.EntitysDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShopApi.Extensions
{
    public class EntityToDtoProfile : Profile
    {
        /// <summary>
        /// 实体类-->Dto类
        /// </summary>
        public EntityToDtoProfile()
        {
            CreateMap<Users, UsersDto>()
                .ForMember(item => item.CreatTime, d => d.MapFrom(s => s.regtime.ToString("yyyy-MM-dd hh:mm:ss")))
                .ForMember(item => item.Status, d => d.MapFrom(s => s.status == 0 ? "未锁定" : "锁定"));
        }
    }
}
