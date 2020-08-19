﻿using CacheManager.Core;
using Masuit.MyBlogs.Core.Common;
using Masuit.MyBlogs.Core.Extensions;
using Masuit.MyBlogs.Core.Infrastructure.Services.Interface;
using Masuit.MyBlogs.Core.Models.DTO;
using Masuit.MyBlogs.Core.Models.Entity;
using Masuit.MyBlogs.Core.Models.Enum;
using Masuit.MyBlogs.Core.Models.ViewModel;
using Masuit.Tools;
using Masuit.Tools.Core.Net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Masuit.MyBlogs.Core.Controllers
{
    /// <summary>
    /// 站内搜索
    /// </summary>
    public class SearchController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        public ISearchDetailsService SearchDetailsService { get; set; }
        public IPostService PostService { get; set; }

        public ICacheManager<string> CacheManager { get; set; }

        /// <summary>
        /// 搜索页
        /// </summary>
        /// <param name="wd"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [Route("s/{wd?}/{page:int?}/{size:int?}")]
        public async Task<ActionResult> Search(string wd = "", [Range(1, int.MaxValue, ErrorMessage = "页码必须大于0")] int page = 1, [Range(1, 50, ErrorMessage = "页大小必须在0到50之间")] int size = 15)
        {
            wd = wd?.Trim();
            ViewBag.PageSize = size;
            ViewBag.Keyword = wd;
            string key = "Search:" + ClientIP;
            if (CacheManager.Exists(key) && CacheManager.Get(key) != wd)
            {
                var hotSearches = RedisHelper.Get<List<KeywordsRank>>("SearchRank:Week").Take(10).ToList();
                ViewBag.hotSearches = hotSearches;
                ViewBag.ErrorMsg = "10秒内只能搜索1次！";
                return View(new SearchResult<PostDto>());
            }

            if (!string.IsNullOrWhiteSpace(wd) && !wd.Contains("锟斤拷"))
            {
                if (!HttpContext.Session.TryGetValue("search:" + wd, out _) && !HttpContext.Request.IsRobot())
                {
                    SearchDetailsService.AddEntity(new SearchDetails
                    {
                        Keywords = wd,
                        SearchTime = DateTime.Now,
                        IP = ClientIP
                    });
                    await SearchDetailsService.SaveChangesAsync();
                    HttpContext.Session.Set("search:" + wd, wd.ToByteArray());
                }

                var posts = PostService.SearchPage(page, size, wd);
                if (posts.Total > 1)
                {
                    CacheManager.AddOrUpdate(key, wd, s => wd);
                    CacheManager.Expire(key, TimeSpan.FromSeconds(10));
                    ViewBag.Ads = AdsService.GetByWeightedPrice(AdvertiseType.PostList);
                }

                ViewBag.hotSearches = new List<KeywordsRank>();
                return View(posts);
            }

            ViewBag.hotSearches = RedisHelper.Get<List<KeywordsRank>>("SearchRank:Week").Take(10).ToList();
            return View(new SearchResult<PostDto>());
        }

        /// <summary>
        /// 关键词搜索记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [MyAuthorize, HttpPost, ResponseCache(Duration = 600, VaryByQueryKeys = new[] { "page", "size", "search" }, VaryByHeader = "Cookie")]
        public ActionResult SearchList([Range(1, int.MaxValue, ErrorMessage = "页码必须大于0")] int page = 1, [Range(1, 50, ErrorMessage = "页大小必须在0到50之间")] int size = 15, string search = "")
        {
            var where = string.IsNullOrEmpty(search) ? (Expression<Func<SearchDetails, bool>>)(s => true) : s => s.Keywords.Contains(search);
            var pages = SearchDetailsService.GetPages<DateTime, SearchDetailsDto>(page, size, where, s => s.SearchTime, false);
            foreach (var item in pages.Data)
            {
                item.SearchTime = item.SearchTime.ToTimeZone(HttpContext.Session.Get<string>(SessionKey.TimeZone));
            }

            return Ok(pages);
        }

        /// <summary>
        /// 热词
        /// </summary>
        /// <returns></returns>
        [MyAuthorize, HttpPost, ResponseCache(Duration = 600, VaryByHeader = "Cookie")]
        public ActionResult HotKey()
        {
            return ResultData(new
            {
                month = RedisHelper.Get<List<KeywordsRank>>("SearchRank:Month"),
                week = RedisHelper.Get<List<KeywordsRank>>("SearchRank:Week"),
                today = RedisHelper.Get<List<KeywordsRank>>("SearchRank:Today")
            });
        }

        /// <summary>
        /// 删除搜索记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, MyAuthorize]
        public async Task<ActionResult> Delete(int id)
        {
            bool b = await SearchDetailsService.DeleteByIdSavedAsync(id) > 0;
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }
    }
}