﻿using EFCoreSecondLevelCacheInterceptor;
using Masuit.MyBlogs.Core.Infrastructure.Repository.Interface;
using Masuit.MyBlogs.Core.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Masuit.MyBlogs.Core.Infrastructure.Repository
{
    public partial class PostRepository : BaseRepository<Post>, IPostRepository
    {
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="t">需要添加的实体</param>
        /// <returns>添加成功</returns>
        public override Post AddEntity(Post t)
        {
            DataContext.Add(t);
            return t;
        }

        /// <summary>
        /// 获取第一条数据，优先从缓存读取
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public override Task<Post> GetAsync(Expression<Func<Post, bool>> @where)
        {
            return EF.CompileAsyncQuery((DataContext ctx) => ctx.Post.Include(p => p.Category).Include(p => p.Seminar).FirstOrDefault(@where))(DataContext);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合，优先从二级缓存读取
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public override List<Post> GetQueryFromCache(Expression<Func<Post, bool>> @where)
        {
            return DataContext.Post.Include(p => p.Category).Where(where).Cacheable().ToList();
        }

        /// <summary>
        /// 基本查询方法，获取一个集合
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public override IOrderedQueryable<Post> GetQuery<TS>(Expression<Func<Post, bool>> @where, Expression<Func<Post, TS>> @orderby, bool isAsc = true)
        {
            return isAsc ? DataContext.Post.Include(p => p.Category).Where(where).OrderBy(orderby) : DataContext.Post.Include(p => p.Category).Where(where).OrderByDescending(orderby);
        }
    }
}