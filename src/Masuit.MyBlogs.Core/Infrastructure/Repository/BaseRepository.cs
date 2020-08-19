﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Masuit.MyBlogs.Core.Infrastructure.Repository.Interface;
using Masuit.Tools;
using Masuit.Tools.Core.AspNetCore;
using Masuit.Tools.Models;
using Masuit.Tools.Systems;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Masuit.MyBlogs.Core.Infrastructure.Repository
{
    /// <summary>
    /// DAL基类
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public abstract class BaseRepository<T> : Disposable, IBaseRepository<T> where T : class, new()
    {
        public virtual DataContext DataContext { get; set; }

        public MapperConfiguration MapperConfig { get; set; }
        public static MemoryCacheEntryOptions CacheOtions => new MemoryCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
        };

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IQueryable<T> GetAll()
        {
            return DataContext.Set<T>();
        }

        /// <summary>
        /// 获取所有实体（不跟踪）
        /// </summary>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IQueryable<T> GetAllNoTracking()
        {
            return DataContext.Set<T>().AsNoTracking();
        }

        /// <summary>
        /// 从二级缓存获取所有实体
        /// </summary>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IEnumerable<T> GetAllFromCache()
        {
            return DataContext.Set<T>().FromCache(CacheOtions);
        }

        /// <summary>
        /// 从二级缓存获取所有实体
        /// </summary>
        /// <returns>还未执行的SQL语句</returns>
        public Task<IEnumerable<T>> GetAllFromCacheAsync()
        {
            return DataContext.Set<T>().FromCacheAsync(CacheOtions);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="TDto">映射实体</typeparam>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IQueryable<TDto> GetAll<TDto>() where TDto : class
        {
            return DataContext.Set<T>().AsNoTracking().ProjectTo<TDto>(MapperConfig);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="TDto">映射实体</typeparam>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IEnumerable<TDto> GetAllFromCache<TDto>() where TDto : class
        {
            return DataContext.Set<T>().AsNoTracking().ProjectTo<TDto>(MapperConfig).FromCache(CacheOtions);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="TDto">映射实体</typeparam>
        /// <returns>还未执行的SQL语句</returns>
        public Task<IEnumerable<TDto>> GetAllFromCacheAsync<TDto>() where TDto : class
        {
            return DataContext.Set<T>().AsNoTracking().ProjectTo<TDto>(MapperConfig).FromCacheAsync(CacheOtions);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IOrderedQueryable<T> GetAll<TS>(Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            return isAsc ? DataContext.Set<T>().OrderBy(orderby) : DataContext.Set<T>().OrderByDescending(orderby);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IOrderedQueryable<T> GetAllNoTracking<TS>(Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            return isAsc ? DataContext.Set<T>().AsNoTracking().OrderBy(orderby) : DataContext.Set<T>().AsNoTracking().OrderByDescending(orderby);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IEnumerable<T> GetAllFromCache<TS>(Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            return GetAll(orderby, isAsc).FromCache(CacheOtions);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public Task<IEnumerable<T>> GetAllFromCacheAsync<TS>(Expression<Func<T, TS>> @orderby, bool isAsc = true)
        {
            return GetAll(orderby, isAsc).FromCacheAsync(CacheOtions);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <typeparam name="TDto">映射实体</typeparam>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IQueryable<TDto> GetAll<TS, TDto>(Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
        {
            return GetAllNoTracking(orderby, isAsc).ProjectTo<TDto>(MapperConfig);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <typeparam name="TDto">映射实体</typeparam>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IEnumerable<TDto> GetAllFromCache<TS, TDto>(Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
        {
            return GetAllNoTracking(orderby, isAsc).ProjectTo<TDto>(MapperConfig).FromCache(CacheOtions);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <typeparam name="TDto">映射实体</typeparam>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public Task<IEnumerable<TDto>> GetAllFromCacheAsync<TS, TDto>(Expression<Func<T, TS>> @orderby, bool isAsc = true) where TDto : class
        {
            return GetAllNoTracking(orderby, isAsc).ProjectTo<TDto>(MapperConfig).FromCacheAsync(CacheOtions);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IQueryable<T> GetQuery(Expression<Func<T, bool>> where)
        {
            return DataContext.Set<T>().Where(where);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IOrderedQueryable<T> GetQuery<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            return isAsc ? DataContext.Set<T>().Where(where).OrderBy(orderby) : DataContext.Set<T>().Where(where).OrderByDescending(orderby);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合，优先从二级缓存读取
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IEnumerable<T> GetQueryFromCache(Expression<Func<T, bool>> where)
        {
            return DataContext.Set<T>().Where(where).FromCache(CacheOtions);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合，优先从二级缓存读取
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public Task<IEnumerable<T>> GetQueryFromCacheAsync(Expression<Func<T, bool>> @where)
        {
            return DataContext.Set<T>().Where(where).FromCacheAsync(CacheOtions);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合，优先从二级缓存读取
        /// </summary>
        /// <typeparam name="TS">排序字段</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序方式</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IEnumerable<T> GetQueryFromCache<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            return GetQuery(where, orderby, isAsc).FromCache(CacheOtions);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合，优先从二级缓存读取
        /// </summary>
        /// <typeparam name="TS">排序字段</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序方式</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public Task<IEnumerable<T>> GetQueryFromCacheAsync<TS>(Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true)
        {
            return GetQuery(where, orderby, isAsc).FromCacheAsync(CacheOtions);
        }

        /// <summary>
        /// 基本查询方法，获取一个被AutoMapper映射后的集合，优先从二级缓存读取
        /// </summary>
        /// <typeparam name="TS">排序字段</typeparam>
        /// <typeparam name="TDto">输出类型</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序方式</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public Task<IEnumerable<TDto>> GetQueryFromCacheAsync<TS, TDto>(Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true) where TDto : class
        {
            return GetQueryNoTracking(where, orderby, isAsc).ProjectTo<TDto>(MapperConfig).FromCacheAsync(CacheOtions);
        }

        /// <summary>
        /// 基本查询方法，获取一个集合（不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IQueryable<T> GetQueryNoTracking(Expression<Func<T, bool>> where)
        {
            return DataContext.Set<T>().Where(where).AsNoTracking();
        }

        /// <summary>
        /// 基本查询方法，获取一个集合（不跟踪实体）
        /// </summary>
        /// <typeparam name="TS">排序字段</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序方式</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IOrderedQueryable<T> GetQueryNoTracking<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            return isAsc ? DataContext.Set<T>().Where(where).AsNoTracking().OrderBy(orderby) : DataContext.Set<T>().Where(where).AsNoTracking().OrderByDescending(orderby);
        }

        /// <summary>
        /// 基本查询方法，获取一个被AutoMapper映射后的集合（不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IQueryable<TDto> GetQuery<TDto>(Expression<Func<T, bool>> where) where TDto : class
        {
            return DataContext.Set<T>().Where(where).AsNoTracking().ProjectTo<TDto>(MapperConfig);
        }

        /// <summary>
        /// 基本查询方法，获取一个被AutoMapper映射后的集合（不跟踪实体）
        /// </summary>
        /// <typeparam name="TS">排序字段</typeparam>
        /// <typeparam name="TDto">输出类型</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序方式</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IQueryable<TDto> GetQuery<TS, TDto>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
        {
            return GetQueryNoTracking(where, orderby, isAsc).ProjectTo<TDto>(MapperConfig);
        }

        /// <summary>
        /// 基本查询方法，获取一个被AutoMapper映射后的集合，优先从二级缓存读取(不跟踪实体)
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体集合</returns>
        public virtual IEnumerable<TDto> GetQueryFromCache<TDto>(Expression<Func<T, bool>> where) where TDto : class
        {
            return DataContext.Set<T>().Where(where).AsNoTracking().ProjectTo<TDto>(MapperConfig).FromCache(CacheOtions);
        }

        /// <summary>
        /// 基本查询方法，获取一个被AutoMapper映射后的集合，优先从二级缓存读取
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>还未执行的SQL语句</returns>
        public Task<IEnumerable<TDto>> GetQueryFromCacheAsync<TDto>(Expression<Func<T, bool>> @where) where TDto : class
        {
            return DataContext.Set<T>().Where(where).AsNoTracking().ProjectTo<TDto>(MapperConfig).FromCacheAsync(CacheOtions);
        }

        /// <summary>
        /// 基本查询方法，获取一个被AutoMapper映射后的集合，优先从二级缓存读取(不跟踪实体)
        /// </summary>
        /// <typeparam name="TS">排序字段</typeparam>
        /// <typeparam name="TDto">输出类型</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序方式</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual IEnumerable<TDto> GetQueryFromCache<TS, TDto>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
        {
            return GetQueryNoTracking(where, orderby, isAsc).ProjectTo<TDto>(MapperConfig).FromCache(CacheOtions);
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public virtual T Get(Expression<Func<T, bool>> where)
        {
            return DataContext.Set<T>().FirstOrDefault(where);
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public virtual T GetFromCache(Expression<Func<T, bool>> where)
        {
            return DataContext.Set<T>().Where(where).DeferredFirstOrDefault().Execute();
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public Task<T> GetFromCacheAsync(Expression<Func<T, bool>> @where)
        {
            return DataContext.Set<T>().Where(where).DeferredFirstOrDefault().ExecuteAsync();
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>实体</returns>
        public virtual T Get<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            return isAsc ? DataContext.Set<T>().OrderBy(orderby).FirstOrDefault(where) : DataContext.Set<T>().OrderByDescending(orderby).FirstOrDefault(where);
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>实体</returns>
        public virtual T GetFromCache<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            return isAsc ? DataContext.Set<T>().OrderBy(orderby).Where(where).DeferredFirstOrDefault().Execute() : DataContext.Set<T>().OrderByDescending(orderby).Where(where).DeferredFirstOrDefault().Execute();
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>实体</returns>
        public Task<T> GetFromCacheAsync<TS>(Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true)
        {
            return isAsc ? DataContext.Set<T>().OrderBy(orderby).Where(where).DeferredFirstOrDefault().ExecuteAsync() : DataContext.Set<T>().OrderByDescending(orderby).Where(where).DeferredFirstOrDefault().ExecuteAsync();
        }

        /// <summary>
        /// 获取第一条被AutoMapper映射后的数据
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <typeparam name="TDto">映射实体</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>映射实体</returns>
        public Task<TDto> GetAsync<TS, TDto>(Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true) where TDto : class
        {
            return isAsc ? DataContext.Set<T>().Where(where).OrderBy(orderby).AsNoTracking().ProjectTo<TDto>(MapperConfig).FirstOrDefaultAsync() : DataContext.Set<T>().Where(where).OrderByDescending(orderby).AsNoTracking().ProjectTo<TDto>(MapperConfig).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 获取第一条被AutoMapper映射后的数据
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <typeparam name="TDto">映射实体</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>映射实体</returns>
        public virtual TDto GetFromCache<TS, TDto>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
        {
            return isAsc ? DataContext.Set<T>().Where(where).OrderBy(orderby).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().Execute() : DataContext.Set<T>().Where(where).OrderByDescending(orderby).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().Execute();
        }

        /// <summary>
        /// 获取第一条被AutoMapper映射后的数据
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <typeparam name="TDto">映射实体</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>映射实体</returns>
        public Task<TDto> GetFromCacheAsync<TS, TDto>(Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc = true) where TDto : class
        {
            return isAsc ? DataContext.Set<T>().Where(where).OrderBy(orderby).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().ExecuteAsync() : DataContext.Set<T>().Where(where).OrderByDescending(orderby).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().ExecuteAsync();
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public virtual Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            return DataContext.Set<T>().FirstOrDefaultAsync(@where);
        }

        /// <summary>
        /// 获取第一条数据
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>实体</returns>
        public virtual async Task<T> GetAsync<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            return isAsc ? await DataContext.Set<T>().OrderBy(orderby).FirstOrDefaultAsync(where).ConfigureAwait(true) : await DataContext.Set<T>().OrderByDescending(orderby).FirstOrDefaultAsync(where).ConfigureAwait(true);
        }

        /// <summary>
        /// 获取第一条数据（不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public virtual T GetNoTracking(Expression<Func<T, bool>> where)
        {
            return DataContext.Set<T>().AsNoTracking().FirstOrDefault(where);
        }

        /// <summary>
        /// 获取第一条数据（不跟踪实体）
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>实体</returns>
        public virtual T GetNoTracking<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            return isAsc ? DataContext.Set<T>().OrderBy(orderby).AsNoTracking().FirstOrDefault(where) : DataContext.Set<T>().OrderByDescending(orderby).AsNoTracking().FirstOrDefault(where);
        }

        /// <summary>
        /// 获取第一条被AutoMapper映射后的数据（不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public virtual TDto Get<TDto>(Expression<Func<T, bool>> where) where TDto : class
        {
            return DataContext.Set<T>().Where(where).AsNoTracking().ProjectTo<TDto>(MapperConfig).FirstOrDefault();
        }

        /// <summary>
        /// 获取第一条被AutoMapper映射后的数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public virtual TDto GetFromCache<TDto>(Expression<Func<T, bool>> where) where TDto : class
        {
            return DataContext.Set<T>().Where(where).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().Execute();
        }

        /// <summary>
        /// 获取第一条被AutoMapper映射后的数据
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public Task<TDto> GetFromCacheAsync<TDto>(Expression<Func<T, bool>> @where) where TDto : class
        {
            return DataContext.Set<T>().Where(where).ProjectTo<TDto>(MapperConfig).DeferredFirstOrDefault().ExecuteAsync();
        }

        /// <summary>
        /// 获取第一条被AutoMapper映射后的数据（不跟踪实体）
        /// </summary>
        /// <typeparam name="TDto">映射实体</typeparam>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>实体</returns>
        public virtual TDto Get<TS, TDto>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
        {
            return isAsc ? DataContext.Set<T>().Where(where).OrderBy(orderby).AsNoTracking().ProjectTo<TDto>(MapperConfig).FirstOrDefault() : DataContext.Set<T>().Where(where).OrderByDescending(orderby).AsNoTracking().ProjectTo<TDto>(MapperConfig).FirstOrDefault();
        }

        /// <summary>
        /// 获取第一条数据（异步，不跟踪实体）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public virtual Task<T> GetNoTrackingAsync(Expression<Func<T, bool>> where)
        {
            return DataContext.Set<T>().AsNoTracking().FirstOrDefaultAsync(@where);
        }

        /// <summary>
        /// 获取第一条数据（异步，不跟踪实体）
        /// </summary>
        /// <typeparam name="TS">排序</typeparam>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns>实体</returns>
        public virtual async Task<T> GetNoTrackingAsync<TS>(Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            return isAsc ? await DataContext.Set<T>().OrderBy(orderby).AsNoTracking().FirstOrDefaultAsync(where).ConfigureAwait(true) : await DataContext.Set<T>().OrderByDescending(orderby).AsNoTracking().FirstOrDefaultAsync(where).ConfigureAwait(true);
        }

        /// <summary>
        /// 根据ID找实体
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns>实体</returns>
        public virtual T GetById(object id)
        {
            return DataContext.Set<T>().Find(id);
        }

        /// <summary>
        /// 根据ID找实体(异步)
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns>实体</returns>
        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await DataContext.Set<T>().FindAsync(id).ConfigureAwait(true);
        }

        /// <summary>
        /// 高效分页查询方法
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual PagedList<T> GetPages<TS>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc)
        {
            var temp = DataContext.Set<T>().Where(where);
            return isAsc ? temp.OrderBy(orderby).ToPagedList(pageIndex, pageSize) : temp.OrderByDescending(orderby).ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// 高效分页查询方法，优先从二级缓存读取
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual PagedList<T> GetPagesFromCache<TS>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc)
        {
            var temp = DataContext.Set<T>().Where(where);
            return isAsc ? temp.OrderBy(orderby).ToCachedPagedList(pageIndex, pageSize) : temp.OrderByDescending(orderby).ToCachedPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// 高效分页查询方法，优先从二级缓存读取
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns>还未执行的SQL语句</returns>
        public Task<PagedList<T>> GetPagesFromCacheAsync<TS>(int pageIndex, int pageSize, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc)
        {
            var temp = DataContext.Set<T>().Where(where);
            return isAsc ? temp.OrderBy(orderby).ToCachedPagedListAsync(pageIndex, pageSize) : temp.OrderByDescending(orderby).ToCachedPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// 高效分页查询方法，优先从二级缓存读取，取出被AutoMapper映射后的数据集合
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns>还未执行的SQL语句</returns>
        public Task<PagedList<TDto>> GetPagesFromCacheAsync<TS, TDto>(int pageIndex, int pageSize, Expression<Func<T, bool>> @where, Expression<Func<T, TS>> @orderby, bool isAsc) where TDto : class
        {
            var temp = DataContext.Set<T>().Where(where).AsNoTracking();
            return isAsc ? temp.OrderBy(orderby).ToCachedPagedListAsync<T, TDto>(pageIndex, pageSize, MapperConfig) : temp.OrderByDescending(orderby).ToCachedPagedListAsync<T, TDto>(pageIndex, pageSize, MapperConfig);
        }

        /// <summary>
        /// 高效分页查询方法（不跟踪实体）
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual PagedList<T> GetPagesNoTracking<TS>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true)
        {
            var temp = DataContext.Set<T>().Where(where).AsNoTracking();
            return isAsc ? temp.OrderBy(orderby).ToPagedList(pageIndex, pageSize) : temp.OrderByDescending(orderby).ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// 高效分页查询方法，取出被AutoMapper映射后的数据集合（不跟踪实体）
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual PagedList<TDto> GetPages<TS, TDto>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
        {
            var temp = DataContext.Set<T>().Where(where).AsNoTracking();
            return isAsc ? temp.OrderBy(orderby).ToPagedList<T, TDto>(pageIndex, pageSize, MapperConfig) : temp.OrderByDescending(orderby).ToPagedList<T, TDto>(pageIndex, pageSize, MapperConfig);
        }

        /// <summary>
        /// 高效分页查询方法，取出被AutoMapper映射后的数据集合，优先从缓存读取（不跟踪实体）
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="pageIndex">第几页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="where">where Lambda条件表达式</param>
        /// <param name="orderby">orderby Lambda条件表达式</param>
        /// <param name="isAsc">升序降序</param>
        /// <returns>还未执行的SQL语句</returns>
        public virtual PagedList<TDto> GetPagesFromCache<TS, TDto>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TS>> orderby, bool isAsc = true) where TDto : class
        {
            var temp = DataContext.Set<T>().Where(where).AsNoTracking();
            return isAsc ? temp.OrderBy(orderby).ToCachedPagedList<T, TDto>(pageIndex, pageSize, MapperConfig) : temp.OrderByDescending(orderby).ToCachedPagedList<T, TDto>(pageIndex, pageSize, MapperConfig);
        }

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id">实体id</param>
        /// <returns>删除成功</returns>
        public virtual bool DeleteById(object id)
        {
            T t = GetById(id);
            if (t != null)
            {
                DataContext.Entry(t).State = EntityState.Deleted;
                DataContext.Remove(t);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="t">需要删除的实体</param>
        /// <returns>删除成功</returns>
        public virtual bool DeleteEntity(T t)
        {
            DataContext.Entry(t).State = EntityState.Unchanged;
            DataContext.Entry(t).State = EntityState.Deleted;
            DataContext.Remove(t);
            return true;
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>删除成功</returns>
        public virtual int DeleteEntity(Expression<Func<T, bool>> where)
        {
            var query = DataContext.Set<T>().Where(where);
            DataContext.RemoveRange(query);
            return DataContext.SaveChanges();
        }

        /// <summary>
        /// 根据条件删除实体（异步）
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>删除成功</returns>
        public virtual async Task<int> DeleteEntityAsync(Expression<Func<T, bool>> where)
        {
            var query = DataContext.Set<T>().Where(where);
            DataContext.RemoveRange(query);
            return await DataContext.SaveChangesAsync();
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="t">需要添加的实体</param>
        /// <returns>添加成功</returns>
        public abstract T AddEntity(T t);

        /// <summary>
        /// 添加或更新实体
        /// </summary>
        /// <param name="key">更新键规则</param>
        /// <param name="t">需要保存的实体</param>
        /// <returns>保存成功</returns>
        public T AddOrUpdate<TKey>(Expression<Func<T, TKey>> key, T t)
        {
            DataContext.Set<T>().AddOrUpdate(key, t);
            return t;
        }

        /// <summary>
        /// 添加或更新实体
        /// </summary>
        /// <param name="key">更新键规则</param>
        /// <param name="entities">需要保存的实体</param>
        /// <returns>保存成功</returns>
        public void AddOrUpdate<TKey>(Expression<Func<T, TKey>> key, params T[] entities)
        {
            DataContext.Set<T>().AddOrUpdate(key, entities);
        }

        /// <summary>
        /// 添加或更新实体
        /// </summary>
        /// <param name="key">更新键规则</param>
        /// <param name="entities">需要保存的实体</param>
        /// <returns>保存成功</returns>
        public void AddOrUpdate<TKey>(Expression<Func<T, TKey>> key, IEnumerable<T> entities)
        {
            DataContext.Set<T>().AddOrUpdate(key, entities);
        }

        /// <summary>
        /// 统一保存数据
        /// </summary>
        /// <returns>受影响的行数</returns>
        public virtual int SaveChanges()
        {
            return DataContext.SaveChanges();
        }

        /// <summary>
        /// 统一保存数据（异步）
        /// </summary>
        /// <returns>受影响的行数</returns>
        public virtual async Task<int> SaveChangesAsync()
        {
            return await DataContext.SaveChangesAsync().ConfigureAwait(true);
        }

        /// <summary>
        /// 判断实体是否在数据库中存在
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>是否存在</returns>
        public virtual bool Any(Expression<Func<T, bool>> where)
        {
            return DataContext.Set<T>().Any(where);
        }

        /// <summary>
        /// 符合条件的个数
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns>是否存在</returns>
        public virtual int Count(Expression<Func<T, bool>> where)
        {
            return DataContext.Set<T>().Count(where);
        }

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns>删除成功</returns>
        public virtual bool DeleteEntities(IEnumerable<T> list)
        {
            list.ForEach(t =>
            {
                DeleteEntity(t);
            });
            return true;
        }

        /// <summary>
        /// 添加多个实体
        /// </summary>
        /// <param name="list">实体集合</param>
        /// <returns>添加成功</returns>
        public virtual IEnumerable<T> AddEntities(IList<T> list)
        {
            return list.Select(AddEntity);
        }

        public override void Dispose(bool disposing)
        {
            DataContext?.Dispose();
            DataContext = null;
        }
    }

    public static class QueryableExt
    {
        /// <summary>
        /// 生成分页集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">当前页</param>
        /// <param name="size">页大小</param>
        /// <returns></returns>
        public static PagedList<T> ToCachedPagedList<T>(this IOrderedQueryable<T> query, int page, int size) where T : class, new()
        {
            var totalCount = query.Count();
            if (page * size > totalCount)
            {
                page = (int)Math.Ceiling(totalCount / (size * 1.0));
            }

            if (page <= 0)
            {
                page = 1;
            }

            var list = query.Skip(size * (page - 1)).Take(size).FromCache(BaseRepository<T>.CacheOtions).ToList();
            return new PagedList<T>(list, page, size, totalCount);
        }

        /// <summary>
        /// 生成分页集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">当前页</param>
        /// <param name="size">页大小</param>
        /// <returns></returns>
        public static async Task<PagedList<T>> ToCachedPagedListAsync<T>(this IOrderedQueryable<T> query, int page, int size) where T : class, new()
        {
            var totalCount = query.Count();
            if (page * size > totalCount)
            {
                page = (int)Math.Ceiling(totalCount / (size * 1.0));
            }

            if (page <= 0)
            {
                page = 1;
            }

            var list = await query.Skip(size * (page - 1)).Take(size).FromCacheAsync(BaseRepository<T>.CacheOtions);
            return new PagedList<T>(list.ToList(), page, size, totalCount);
        }

        /// <summary>
        /// 生成分页集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">当前页</param>
        /// <param name="size">页大小</param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static PagedList<TDto> ToPagedList<T, TDto>(this IOrderedQueryable<T> query, int page, int size, MapperConfiguration mapper)
        {
            var totalCount = query.Count();
            if (page * size > totalCount)
            {
                page = (int)Math.Ceiling(totalCount / (size * 1.0));
            }

            if (page <= 0)
            {
                page = 1;
            }

            var list = query.Skip(size * (page - 1)).Take(size).ProjectTo<TDto>(mapper).ToList();
            return new PagedList<TDto>(list, page, size, totalCount);
        }

        /// <summary>
        /// 生成分页集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">当前页</param>
        /// <param name="size">页大小</param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static PagedList<TDto> ToCachedPagedList<T, TDto>(this IOrderedQueryable<T> query, int page, int size, MapperConfiguration mapper) where TDto : class where T : class, new()
        {
            var totalCount = query.Count();
            if (page * size > totalCount)
            {
                page = (int)Math.Ceiling(totalCount / (size * 1.0));
            }

            if (page <= 0)
            {
                page = 1;
            }

            var list = query.Skip(size * (page - 1)).Take(size).ProjectTo<TDto>(mapper).FromCache(BaseRepository<T>.CacheOtions).ToList();
            return new PagedList<TDto>(list, page, size, totalCount);
        }

        /// <summary>
        /// 生成分页集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">当前页</param>
        /// <param name="size">页大小</param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static async Task<PagedList<TDto>> ToCachedPagedListAsync<T, TDto>(this IOrderedQueryable<T> query, int page, int size, MapperConfiguration mapper) where TDto : class where T : class, new()
        {
            var totalCount = query.Count();
            if (page * size > totalCount)
            {
                page = (int)Math.Ceiling(totalCount / (size * 1.0));
            }

            if (page <= 0)
            {
                page = 1;
            }

            var list = await query.Skip(size * (page - 1)).Take(size).ProjectTo<TDto>(mapper).FromCacheAsync(BaseRepository<T>.CacheOtions);
            return new PagedList<TDto>(list.ToList(), page, size, totalCount);
        }
    }
}