﻿using Masuit.MyBlogs.Core.Extensions;
using Masuit.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Masuit.MyBlogs.Core.Controllers
{
    public class ShortController : Controller
    {
        [HttpGet("short"), MyAuthorize]
        public IActionResult Short(string url)
        {
            var id = url.Crc32().FromBinary(16).ToBinary(62);
            RedisHelper.Set("shorturl:" + id, url);
            return Ok(id);
        }

        [HttpGet("{key}")]
        public ActionResult RedirectTo(string key)
        {
            var url = RedisHelper.Get("shorturl:" + key) ?? throw new NotFoundException("链接未找到");
            return Redirect(url);
        }
    }
}