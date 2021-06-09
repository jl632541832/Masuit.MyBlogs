﻿using CacheManager.Core;
using Hangfire;
using Masuit.MyBlogs.Core.Common;
using Masuit.MyBlogs.Core.Common.Mails;
using Masuit.MyBlogs.Core.Extensions;
using Masuit.MyBlogs.Core.Infrastructure.Services.Interface;
using Masuit.MyBlogs.Core.Models.Command;
using Masuit.MyBlogs.Core.Models.DTO;
using Masuit.MyBlogs.Core.Models.Entity;
using Masuit.MyBlogs.Core.Models.Enum;
using Masuit.MyBlogs.Core.Models.ViewModel;
using Masuit.Tools;
using Masuit.Tools.Core.Net;
using Masuit.Tools.Html;
using Masuit.Tools.Logging;
using Masuit.Tools.Models;
using Masuit.Tools.Strings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Masuit.MyBlogs.Core.Controllers
{
    /// <summary>
    /// 评论管理
    /// </summary>
    public class CommentController : BaseController
    {
        public ICommentService CommentService { get; set; }
        public IPostService PostService { get; set; }
        public IWebHostEnvironment HostEnvironment { get; set; }
        public ICacheManager<int> CommentFeq { get; set; }

        /// <summary>
        /// 发表评论
        /// </summary>
        /// <param name="messageService"></param>
        /// <param name="mailSender"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit([FromServices] IInternalMessageService messageService, [FromServices] IMailSender mailSender, CommentCommand dto)
        {
            var match = Regex.Match(dto.NickName + dto.Content.RemoveHtmlTag(), CommonHelper.BanRegex);
            if (match.Success)
            {
                LogManager.Info($"提交内容：{dto.NickName}/{dto.Content}，敏感词：{match.Value}");
                return ResultData(null, false, "您提交的内容包含敏感词，被禁止发表，请检查您的内容后尝试重新提交！");
            }
            var error = await ValidateEmailCode(mailSender, dto.Email, dto.Code);
            if (!string.IsNullOrEmpty(error))
            {
                return ResultData(null, false, error);
            }

            Post post = await PostService.GetByIdAsync(dto.PostId) ?? throw new NotFoundException("评论失败，文章未找到");
            if (post.DisableComment)
            {
                return ResultData(null, false, "本文已禁用评论功能，不允许任何人回复！");
            }

            dto.Content = dto.Content.Trim().Replace("<p><br></p>", string.Empty);
            if (CommentFeq.GetOrAdd("Comments:" + ClientIP, 1) > 2)
            {
                CommentFeq.Expire("Comments:" + ClientIP, TimeSpan.FromMinutes(1));
                return ResultData(null, false, "您的发言频率过快，请稍后再发表吧！");
            }

            var comment = dto.Mapper<Comment>();
            if (Regex.Match(dto.NickName + dto.Content, CommonHelper.ModRegex).Length <= 0)
            {
                comment.Status = Status.Published;
            }

            comment.CommentDate = DateTime.Now;
            var user = HttpContext.Session.Get<UserInfoDto>(SessionKey.UserInfo);
            if (user != null)
            {
                comment.NickName = user.NickName;
                comment.Email = user.Email;
                if (user.IsAdmin)
                {
                    comment.Status = Status.Published;
                    comment.IsMaster = true;
                }
            }
            comment.Content = await dto.Content.HtmlSantinizerStandard().ClearImgAttributes();
            comment.Browser = dto.Browser ?? Request.Headers[HeaderNames.UserAgent];
            comment.IP = ClientIP;
            comment.Location = Request.Location();
            comment = CommentService.AddEntitySaved(comment);
            if (comment == null)
            {
                return ResultData(null, false, "评论失败");
            }

            Response.Cookies.Append("NickName", comment.NickName, new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddYears(1),
                SameSite = SameSiteMode.Lax
            });
            WriteEmailKeyCookie(dto.Email);
            CommentFeq.AddOrUpdate("Comments:" + ClientIP, 1, i => i + 1, 5);
            CommentFeq.Expire("Comments:" + ClientIP, TimeSpan.FromMinutes(1));
            var emails = new HashSet<string>();
            var email = CommonHelper.SystemSettings["ReceiveEmail"]; //站长邮箱
            emails.Add(email);
            var content = new Template(await System.IO.File.ReadAllTextAsync(HostEnvironment.WebRootPath + "/template/notify.html"))
                .Set("title", post.Title)
                .Set("time", DateTime.Now.ToTimeZoneF(HttpContext.Session.Get<string>(SessionKey.TimeZone)))
                .Set("nickname", comment.NickName)
                .Set("content", comment.Content);
            if (comment.Status == Status.Published)
            {
                if (!comment.IsMaster)
                {
                    await messageService.AddEntitySavedAsync(new InternalMessage()
                    {
                        Title = $"来自【{comment.NickName}】在文章《{post.Title}》的新评论",
                        Content = comment.Content,
                        Link = Url.Action("Details", "Post", new { id = comment.PostId, cid = comment.Id }) + "#comment"
                    });
                }
                if (comment.ParentId == 0)
                {
                    emails.Add(post.Email);
                    emails.Add(post.ModifierEmail);
                    //新评论，只通知博主和楼主
                    foreach (var s in emails)
                    {
                        BackgroundJob.Enqueue(() => CommonHelper.SendMail(Request.Host + "|博客文章新评论：", content.Set("link", Url.Action("Details", "Post", new { id = comment.PostId, cid = comment.Id }, Request.Scheme) + "#comment").Render(false), s, ClientIP));
                    }
                }
                else
                {
                    //通知博主和上层所有关联的评论访客
                    var parent = await CommentService.GetByIdAsync(comment.ParentId);
                    emails.AddRange(parent.Root().Flatten().Select(c => c.Email).ToArray());
                    emails.AddRange(post.Email, post.ModifierEmail);
                    emails.Remove(comment.Email);
                    string link = Url.Action("Details", "Post", new { id = comment.PostId, cid = comment.Id }, Request.Scheme) + "#comment";
                    foreach (var s in emails)
                    {
                        BackgroundJob.Enqueue(() => CommonHelper.SendMail($"{Request.Host}{CommonHelper.SystemSettings["Title"]}文章评论回复：", content.Set("link", link).Render(false), s, ClientIP));
                    }
                }
                return ResultData(null, true, "评论发表成功，服务器正在后台处理中，这会有一定的延迟，稍后将显示到评论列表中");
            }

            foreach (var s in emails)
            {
                BackgroundJob.Enqueue(() => CommonHelper.SendMail(Request.Host + "|博客文章新评论(待审核)：", content.Set("link", Url.Action("Details", "Post", new { id = comment.PostId, cid = comment.Id }, Request.Scheme) + "#comment").Render(false) + "<p style='color:red;'>(待审核)</p>", s, ClientIP));
            }

            return ResultData(null, true, "评论成功，待站长审核通过以后将显示");
        }

        /// <summary>
        /// 评论投票
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CommentVote(int id)
        {
            if (HttpContext.Session.Get("cm" + id) != null)
            {
                return ResultData(null, false, "您刚才已经投过票了，感谢您的参与！");
            }

            var cm = await CommentService.GetAsync(c => c.Id == id && c.Status == Status.Published) ?? throw new NotFoundException("评论不存在！");
            cm.VoteCount++;
            bool b = await CommentService.SaveChangesAsync() > 0;
            if (b)
            {
                HttpContext.Session.Set("cm" + id, id.GetBytes());
            }

            return ResultData(null, b, b ? "投票成功" : "投票失败");
        }

        /// <summary>
        /// 获取评论
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetComments(int? id, [Range(1, int.MaxValue, ErrorMessage = "页码必须大于0")] int page = 1, [Range(1, 50, ErrorMessage = "页大小必须在0到50之间")] int size = 15, int cid = 0)
        {
            if (cid != 0)
            {
                var comment = await CommentService.GetByIdAsync(cid) ?? throw new NotFoundException("评论未找到");
                var single = new[] { comment.Root() };
                foreach (var c in single.Flatten())
                {
                    c.CommentDate = c.CommentDate.ToTimeZone(HttpContext.Session.Get<string>(SessionKey.TimeZone));
                }

                return ResultData(new
                {
                    total = 1,
                    parentTotal = 1,
                    page,
                    size,
                    rows = single.Mapper<IList<CommentViewModel>>()
                });
            }

            var parent = await CommentService.GetPagesAsync(page, size, c => c.PostId == id && c.ParentId == 0 && (c.Status == Status.Published || CurrentUser.IsAdmin), c => c.CommentDate, false);
            if (!parent.Data.Any())
            {
                return ResultData(null, false, "没有评论");
            }
            int total = parent.TotalCount; //总条数，用于前台分页
            parent.Data.Flatten().ForEach(c =>
            {
                c.CommentDate = c.CommentDate.ToTimeZone(HttpContext.Session.Get<string>(SessionKey.TimeZone));
            });
            if (total > 0)
            {
                return ResultData(new
                {
                    total,
                    parentTotal = total,
                    page,
                    size,
                    rows = parent.Data.Mapper<IList<CommentViewModel>>()
                });
            }

            return ResultData(null, false, "没有评论");
        }

        /// <summary>
        /// 审核评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MyAuthorize]
        public async Task<ActionResult> Pass(int id)
        {
            Comment comment = await CommentService.GetByIdAsync(id) ?? throw new NotFoundException("评论不存在！");
            comment.Status = Status.Published;
            Post post = await PostService.GetByIdAsync(comment.PostId);
            bool b = await CommentService.SaveChangesAsync() > 0;
            if (b)
            {
                var content = new Template(await System.IO.File.ReadAllTextAsync(Path.Combine(HostEnvironment.WebRootPath, "template", "notify.html")))
                    .Set("title", post.Title)
                    .Set("time", DateTime.Now.ToTimeZoneF(HttpContext.Session.Get<string>(SessionKey.TimeZone)))
                    .Set("nickname", comment.NickName)
                    .Set("content", comment.Content);
                var root = comment.Root();
                var emails = root.Flatten().Select(c => c.Email).Append(post.ModifierEmail).Except(new List<string> { comment.Email, CurrentUser.Email }).ToHashSet();
                var link = Url.Action("Details", "Post", new
                {
                    id = comment.PostId,
                    cid = root.Id
                }, Request.Scheme) + "#comment";
                foreach (var email in emails)
                {
                    BackgroundJob.Enqueue(() => CommonHelper.SendMail($"{Request.Host}{CommonHelper.SystemSettings["Title"]}文章评论回复：", content.Set("link", link).Render(false), email, ClientIP));
                }

                return ResultData(null, true, "审核通过！");
            }

            return ResultData(null, false, "审核失败！");
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [MyAuthorize]
        public ActionResult Delete(int id)
        {
            var b = CommentService.DeleteById(id);
            return ResultData(null, b, b ? "删除成功！" : "删除失败！");
        }

        /// <summary>
        /// 获取未审核的评论
        /// </summary>
        /// <returns></returns>
        [MyAuthorize]
        public async Task<ActionResult> GetPendingComments([Range(1, int.MaxValue, ErrorMessage = "页码必须大于0")] int page = 1, [Range(1, 50, ErrorMessage = "页大小必须在0到50之间")] int size = 15)
        {
            var pages = await CommentService.GetPagesAsync<DateTime, CommentDto>(page, size, c => c.Status == Status.Pending, c => c.CommentDate, false);
            foreach (var item in pages.Data)
            {
                item.CommentDate = item.CommentDate.ToTimeZone(HttpContext.Session.Get<string>(SessionKey.TimeZone));
            }

            return Ok(pages);
        }
    }
}