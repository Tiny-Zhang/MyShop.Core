using System;
using System.Collections.Generic;
using System.Text;

namespace MyShop.Model.Entitys
{
    public class Users
    {
        public int Uid { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TrueName { get; set; }
        public string Email { get; set; }
        public string Head { get; set; }
        public string Mobile { get; set; }
        /// <summary>
        /// 0未锁定  1锁定
        /// </summary>
        public int Status { get; set; }
        public string UserToken { get; set; }
        public DateTime CreatTime { get; set; }

    }
}
