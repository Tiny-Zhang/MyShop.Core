using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.EntitysDto
{
    public class UsersDto
    {
        public string Username { get; set; }
        public string TrueName { get; set; }
        public string Email { get; set; }
        public string Head { get; set; }
        public string Mobile { get; set; }
        /// <summary>
        /// 0未锁定  1锁定
        /// </summary>
        public string Status { get; set; }
        public string CreatTime { get; set; }
    }
}
