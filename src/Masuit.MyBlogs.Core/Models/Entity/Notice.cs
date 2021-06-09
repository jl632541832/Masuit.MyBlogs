using Masuit.MyBlogs.Core.Models.Enum;
using Masuit.MyBlogs.Core.Models.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masuit.MyBlogs.Core.Models.Entity
{
    /// <summary>
    /// ��վ����
    /// </summary>
    [Table("Notice")]
    public class Notice : BaseEntity
    {
        public Notice()
        {
            PostDate = DateTime.Now;
            ModifyDate = DateTime.Now;
            Status = Status.Display;
        }

        /// <summary>
        /// ����
        /// </summary>
        [Required(ErrorMessage = "������ⲻ��Ϊ�գ�")]
        public string Title { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [Required(ErrorMessage = "�������ݲ���Ϊ�գ�"), SubmitCheck(3000, false)]
        public string Content { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime PostDate { get; set; }

        /// <summary>
        /// �޸�ʱ��
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// ��Чʱ��
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// ʧЧʱ��
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// ����״̬
        /// </summary>
        public NoticeStatus NoticeStatus { get; set; }
    }

    public enum NoticeStatus
    {
        UnStart,
        Normal,
        Expired,
    }
}