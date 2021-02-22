﻿using Masuit.MyBlogs.Core.Common;
using Masuit.MyBlogs.Core.Extensions;
using Masuit.MyBlogs.Core.Models.ViewModel;
using Masuit.Tools.AspNetCore.ResumeFileResults.Extensions;
using Masuit.Tools.Files;
using Masuit.Tools.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masuit.MyBlogs.Core.Controllers
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    [Route("[controller]/[action]")]
    public class FileController : AdminController
    {
        public IWebHostEnvironment HostEnvironment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ISevenZipCompressor SevenZipCompressor { get; set; }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ActionResult GetFiles(string path)
        {
            var files = Directory.GetFiles(HostEnvironment.WebRootPath + path).OrderByDescending(s => s).Select(s => new
            {
                filename = Path.GetFileName(s),
                path = s
            }).ToList();
            return ResultData(files);
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public ActionResult Read(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                string text = System.IO.File.ReadAllText(filename);
                return ResultData(text);
            }
            return ResultData(null, false, "文件不存在！");
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ActionResult Save(string filename, string content)
        {
            try
            {
                System.IO.File.WriteAllText(filename, content);
                return ResultData(null, message: "保存成功");
            }
            catch (IOException e)
            {
                LogManager.Error(GetType(), e);
                return ResultData(null, false, "保存失败");
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Upload(string destination)
        {
            var form = await Request.ReadFormAsync();
            foreach (var t in form.Files)
            {
                string path = Path.Combine(HostEnvironment.ContentRootPath, CommonHelper.SystemSettings["PathRoot"].TrimStart('\\', '/'), destination.TrimStart('\\', '/'), t.FileName);
                await using var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                await t.CopyToAsync(fs);
            }
            return Json(new
            {
                result = new List<object>()
            });
        }

        /// <summary>
        /// 操作文件
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Handle([FromBody] FileRequest req)
        {
            var list = new List<object>();
            var root = Path.Combine(HostEnvironment.ContentRootPath, CommonHelper.SystemSettings["PathRoot"].TrimStart('\\', '/'));
            switch (req.Action)
            {
                case "list":
                    var path = Path.Combine(root, req.Path.TrimStart('\\', '/'));
                    var dirs = Directory.GetDirectories(path);
                    var files = Directory.GetFiles(path);
                    list.AddRange(dirs.Select(s => new DirectoryInfo(s)).Select(dirinfo => new FileList
                    {
                        date = dirinfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        name = dirinfo.Name,
                        size = 0,
                        type = "dir"
                    }).Union(files.Select(s => new FileInfo(s)).Select(info => new FileList
                    {
                        name = info.Name,
                        size = info.Length,
                        date = info.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        type = "file"
                    })));
                    break;
                case "remove":
                    req.Items.ForEach(s =>
                    {
                        s = Path.Combine(root, s.TrimStart('\\', '/'));
                        try
                        {
                            System.IO.File.Delete(s);
                        }
                        catch
                        {
                            Directory.Delete(s, true);
                        }
                    });
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;
                case "rename":
                case "move":
                    string newpath;
                    if (!string.IsNullOrEmpty(req.Item))
                    {
                        newpath = Path.Combine(root, req.NewItemPath?.TrimStart('\\', '/'));
                        path = Path.Combine(root, req.Item.TrimStart('\\', '/'));
                        try
                        {
                            System.IO.File.Move(path, newpath);
                        }
                        catch
                        {
                            Directory.Move(path, newpath);
                        }
                    }
                    else
                    {
                        newpath = Path.Combine(root, req.NewPath.TrimStart('\\', '/'));
                        req.Items.ForEach(s =>
                        {
                            try
                            {
                                System.IO.File.Move(Path.Combine(root, s.TrimStart('\\', '/')), Path.Combine(newpath, Path.GetFileName(s)));
                            }
                            catch
                            {
                                Directory.Move(Path.Combine(root, s.TrimStart('\\', '/')), Path.Combine(newpath, Path.GetFileName(s)));
                            }
                        });
                    }
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;
                case "copy":
                    if (!string.IsNullOrEmpty(req.Item))
                    {
                        System.IO.File.Copy(Path.Combine(root, req.Item.TrimStart('\\', '/')), Path.Combine(root, req.NewItemPath.TrimStart('\\', '/')));
                    }
                    else
                    {
                        newpath = Path.Combine(root, req.NewPath.TrimStart('\\', '/'));
                        req.Items.ForEach(s => System.IO.File.Copy(Path.Combine(root, s.TrimStart('\\', '/')), !string.IsNullOrEmpty(req.SingleFilename) ? Path.Combine(newpath, req.SingleFilename) : Path.Combine(newpath, Path.GetFileName(s))));
                    }
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;
                case "edit":
                    System.IO.File.WriteAllText(Path.Combine(root, req.Item.TrimStart('\\', '/')), req.Content, Encoding.UTF8);
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;
                case "getContent":
                    return Json(new
                    {
                        result = System.IO.File.ReadAllText(Path.Combine(root, req.Item.TrimStart('\\', '/')), Encoding.UTF8)
                    });
                case "createFolder":
                    list.Add(new
                    {
                        success = Directory.CreateDirectory(Path.Combine(root, req.NewPath.TrimStart('\\', '/'))).Exists.ToString()
                    });
                    break;
                case "changePermissions":
                    //todo:文件权限修改
                    break;
                case "compress":
                    var filename = Path.Combine(Path.Combine(root, req.Destination.TrimStart('\\', '/')), Path.GetFileNameWithoutExtension(req.CompressedFilename) + ".zip");
                    SevenZipCompressor.Zip(req.Items.Select(s => Path.Combine(root, s.TrimStart('\\', '/'))).ToList(), filename);
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;
                case "extract":
                    var folder = Path.Combine(Path.Combine(root, req.Destination.TrimStart('\\', '/')), req.FolderName.Trim('/', '\\'));
                    var zip = Path.Combine(root, req.Item.TrimStart('\\', '/'));
                    SevenZipCompressor.Extract(zip, folder);
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;
            }
            return Json(new
            {
                result = list
            });
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="items"></param>
        /// <param name="toFilename"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Handle(string path, string[] items, string toFilename)
        {
            path = path?.TrimStart('\\', '/') ?? "";
            var root = CommonHelper.SystemSettings["PathRoot"].TrimStart('\\', '/');
            var file = Path.Combine(HostEnvironment.ContentRootPath, root, path);
            switch (Request.Query["action"])
            {
                case "download":
                    if (System.IO.File.Exists(file))
                    {
                        return this.ResumePhysicalFile(file, Path.GetFileName(file));
                    }
                    break;
                case "downloadMultiple":
                    var buffer = SevenZipCompressor.ZipStream(items.Select(s => Path.Combine(HostEnvironment.ContentRootPath, root, s.TrimStart('\\', '/'))).ToList()).ToArray();
                    return this.ResumeFile(buffer, Path.GetFileName(toFilename));
            }

            throw new NotFoundException("文件未找到");
        }
    }
}