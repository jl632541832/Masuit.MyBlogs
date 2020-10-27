﻿using AutoMapper;
using Masuit.MyBlogs.Core.Common;
using Masuit.MyBlogs.Core.Configs;
using Masuit.MyBlogs.Core.Extensions;
using Masuit.MyBlogs.Core.Extensions.Firewall;
using Masuit.MyBlogs.Core.Infrastructure.Services.Interface;
using Masuit.MyBlogs.Core.Models.DTO;
using Masuit.MyBlogs.Core.Models.Enum;
using Masuit.MyBlogs.Core.Models.ViewModel;
using Masuit.Tools.Core.Net;
using Masuit.Tools.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Net;

namespace Masuit.MyBlogs.Core.Controllers
{
    /// <summary>
    /// 基本父控制器
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true), ServiceFilter(typeof(FirewallAttribute))]
    public class BaseController : Controller
    {
        /// <summary>
        /// UserInfoService
        /// </summary>
        public IUserInfoService UserInfoService { get; set; }

        /// <summary>
        /// MenuService
        /// </summary>
        public IMenuService MenuService { get; set; }

        /// <summary>
        /// LinksService
        /// </summary>
        public ILinksService LinksService { get; set; }

        public IAdvertisementService AdsService { get; set; }

        public UserInfoDto CurrentUser => HttpContext.Session.Get<UserInfoDto>(SessionKey.UserInfo) ?? new UserInfoDto();

        /// <summary>
        /// 客户端的真实IP
        /// </summary>
        public string ClientIP
        {
            get
            {
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var trueip = Request.Headers[AppConfig.TrueClientIPHeader].ToString();
                if (!string.IsNullOrEmpty(trueip) && ip != trueip)
                {
                    ip = trueip;
                }
                return ip;
            }
        }

        /// <summary>
        /// 普通访客是否token合法
        /// </summary>
        public bool VisitorTokenValid => Request.Cookies["Email"].MDString3(AppConfig.BaiduAK).Equals(Request.Cookies["FullAccessToken"]);


        public IMapper Mapper { get; set; }
        public MapperConfiguration MapperConfig { get; set; }

        /// <summary>
        /// 响应数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="success">响应状态</param>
        /// <param name="message">响应消息</param>
        /// <param name="isLogin">登录状态</param>
        /// <param name="code">http响应码</param>
        /// <returns></returns>
        public ActionResult ResultData(object data, bool success = true, string message = "", bool isLogin = true, HttpStatusCode code = HttpStatusCode.OK)
        {
            return Ok(new
            {
                IsLogin = isLogin,
                Success = success,
                Message = message,
                Data = data,
                code
            });
        }

        /// <summary>在调用操作方法前调用。</summary>
        /// <param name="filterContext">有关当前请求和操作的信息。</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var user = filterContext.HttpContext.Session.Get<UserInfoDto>(SessionKey.UserInfo);
#if DEBUG
            user = UserInfoService.GetByUsername("masuit").Mapper<UserInfoDto>();
            filterContext.HttpContext.Session.Set(SessionKey.UserInfo, user);
#endif
            if (CommonHelper.SystemSettings.GetOrAdd("CloseSite", "false") == "true" && user?.IsAdmin != true)
            {
                filterContext.Result = RedirectToAction("ComingSoon", "Error");
            }

            if (Request.Method == HttpMethods.Post && !Request.Path.Value.Contains("get", StringComparison.InvariantCultureIgnoreCase) && CommonHelper.SystemSettings.GetOrAdd("DataReadonly", "false") == "true" && !filterContext.Filters.Any(m => m.ToString().Contains(nameof(MyAuthorizeAttribute))))
            {
                filterContext.Result = ResultData("网站当前处于数据写保护状态，无法提交任何数据，如有疑问请联系网站管理员！", false, "网站当前处于数据写保护状态，无法提交任何数据，如有疑问请联系网站管理员！", user != null, HttpStatusCode.BadRequest);
            }

            if (user == null && Request.Cookies.Any(x => x.Key == "username" || x.Key == "password")) //执行自动登录
            {
                string name = Request.Cookies["username"];
                string pwd = Request.Cookies["password"]?.DesDecrypt(AppConfig.BaiduAK);
                var userInfo = UserInfoService.Login(name, pwd);
                if (userInfo != null)
                {
                    Response.Cookies.Append("username", name, new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        SameSite = SameSiteMode.Lax
                    });
                    Response.Cookies.Append("password", Request.Cookies["password"], new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(7),
                        SameSite = SameSiteMode.Lax
                    });
                    filterContext.HttpContext.Session.Set(SessionKey.UserInfo, userInfo);
                }
            }

            if (ModelState.IsValid) return;
            var errmsgs = ModelState.SelectMany(kv => kv.Value.Errors.Select(e => e.ErrorMessage)).ToList();
            if (errmsgs.Any())
            {
                for (var i = 0; i < errmsgs.Count; i++)
                {
                    errmsgs[i] = i + 1 + ". " + errmsgs[i];
                }
            }

            filterContext.Result = ResultData(errmsgs, false, "数据校验失败，错误信息：" + errmsgs.Join(" | "), user != null, HttpStatusCode.BadRequest);
        }

        /// <summary>在调用操作方法后调用。</summary>
        /// <param name="filterContext">有关当前请求和操作的信息。</param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is ViewResult)
            {
                ViewBag.menus = MenuService.GetQueryFromCache<MenuDto>(m => m.Status == Status.Available).OrderBy(m => m.Sort).ToList(); //菜单
                var model = new PageFootViewModel //页脚
                {
                    Links = LinksService.GetQueryFromCache<LinksDto>(l => l.Status == Status.Available).OrderByDescending(l => l.Recommend).ThenByDescending(l => l.Weight).ThenByDescending(l => new Random().Next()).Take(30).ToList()
                };
                ViewBag.Footer = model;
            }
            base.OnActionExecuted(filterContext);
        }
    }
}