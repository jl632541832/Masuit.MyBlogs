using Masuit.LuceneEFCore.SearchEngine;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masuit.MyBlogs.Core.Models.Entity
{
    /// <summary>
    /// ������ϸ��¼
    /// </summary>
    [Table("SearchDetails")]
    public class SearchDetails : LuceneIndexableBaseEntity
    {
        public SearchDetails()
        {
            SearchTime = DateTime.Now;
        }

        /// <summary>
        /// �ؼ���
        /// </summary>
        [Required(ErrorMessage = "�ؼ��ʲ���Ϊ��")]
        public string Keywords { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime SearchTime { get; set; }

        /// <summary>
        /// ������IP
        /// </summary>
        public string IP { get; set; }
    }

    public class SearchRank
    {
        public string Keywords { get; set; }
        public int Count { get; set; }
    }
}