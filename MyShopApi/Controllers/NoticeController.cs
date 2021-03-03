using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyShop.Services;

namespace MyShopApi.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        private readonly NoticeManager messageNotice;
        public NoticeController(NoticeManager _messageNotice)
        {
            messageNotice = _messageNotice;
        }


        /// <summary>
        /// 通知
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> MobileNotice()
        {
            string content = "下午不用来上班";
            var getlist = await Task.Run(() => { return messageNotice.SendNotice(content); });
            return Ok(getlist);
        }





    }
}