using Masuit.MyBlogs.Core.Models.Enum;
using Masuit.Tools.Core.Validator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masuit.MyBlogs.Core.Models.Entity
{
    /// <summary>
    /// ������ʷ�汾
    /// </summary>
    [Table("PostHistoryVersion")]
    public class PostHistoryVersion : BaseEntity
    {
        public PostHistoryVersion()
        {
            ModifyDate = DateTime.Now;
            Status = Status.Pending;
            Seminar = new HashSet<Seminar>();
        }

        /// <summary>
        /// ����
        /// </summary>
        [Required, StringLength(128)]
        public string Title { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// �ܱ���������
        /// </summary>
        public string ProtectContent { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        [DefaultValue(0)]
        public int ViewCount { get; set; }

        /// <summary>
        /// �޸�ʱ��
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// ����id
        /// </summary>
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        /// <summary>
        /// ����id
        /// </summary>
        [ForeignKey("Post")]
        public int PostId { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [StringLength(255), IsEmail]
        public string Email { get; set; }

        /// <summary>
        /// �޸�������
        /// </summary>
        public string Modifier { get; set; }

        /// <summary>
        /// �޸�������
        /// </summary>
        public string ModifierEmail { get; set; }

        /// <summary>
        /// ��ǩ
        /// </summary>
        [StringLength(255)]
        public string Label { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public virtual Category Category { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public virtual Post Post { get; set; }

        /// <summary>
        /// ר��
        /// </summary>
        public virtual ICollection<Seminar> Seminar { get; set; }
    }
}