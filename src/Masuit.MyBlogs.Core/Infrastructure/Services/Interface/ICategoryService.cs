﻿using Masuit.MyBlogs.Core.Models.Entity;
using System.Threading.Tasks;

namespace Masuit.MyBlogs.Core.Infrastructure.Services.Interface
{
    public partial interface ICategoryService : IBaseService<Category>
    {
        /// <summary>
        /// 删除分类，并将该分类下的文章移动到新分类下
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mid"></param>
        /// <returns></returns>
        Task<bool> Delete(int id, int mid);
    }
}