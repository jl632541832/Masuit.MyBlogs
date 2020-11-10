using Masuit.LuceneEFCore.SearchEngine;
using Masuit.MyBlogs.Core.Models.Enum;
using Masuit.MyBlogs.Core.Models.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masuit.MyBlogs.Core.Models.Entity
{
    /// <summary>
    /// ����
    /// </summary>
    [Table("Post")]
    public class Post : BaseEntity
    {
        public Post()
        {
            Comment = new HashSet<Comment>();
            PostDate = DateTime.Now;
            ModifyDate = DateTime.Now;
            IsFixedTop = false;
            Status = Status.Pending;
            Seminar = new HashSet<SeminarPost>();
            PostMergeRequests = new HashSet<PostMergeRequest>();
        }

        /// <summary>
        /// ����
        /// </summary>
        [Required(ErrorMessage = "���±��ⲻ��Ϊ�գ�"), LuceneIndex]
        public string Title { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [Required, MaxLength(24, ErrorMessage = "�������֧��24���ַ���"), LuceneIndex]
        public string Author { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [Required(ErrorMessage = "�������ݲ���Ϊ�գ�"), SubmitCheck(20, 1000000, false), LuceneIndex(IsHtml = true)]
        public string Content { get; set; }

        /// <summary>
        /// �ܱ���������
        /// </summary>
        [LuceneIndex(IsHtml = true)]
        public string ProtectContent { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime PostDate { get; set; }

        /// <summary>
        /// �޸�ʱ��
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// �Ƿ��ö�
        /// </summary>
        [DefaultValue(false)]
        public bool IsFixedTop { get; set; }

        /// <summary>
        /// ����id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [Required(ErrorMessage = "�������䲻��Ϊ�գ�"), EmailAddress, LuceneIndex]
        public string Email { get; set; }

        /// <summary>
        /// �޸�������
        /// </summary>
        [LuceneIndex]
        public string Modifier { get; set; }

        /// <summary>
        /// �޸�������
        /// </summary>
        [LuceneIndex]
        public string ModifierEmail { get; set; }

        /// <summary>
        /// ��ǩ
        /// </summary>
        [StringLength(256, ErrorMessage = "��ǩ�������255���ַ�"), LuceneIndex]
        public string Label { get; set; }

        /// <summary>
        /// ���¹ؼ���
        /// </summary>
        [StringLength(256, ErrorMessage = "���¹ؼ����������255���ַ�"), LuceneIndex]
        public string Keyword { get; set; }

        /// <summary>
        /// ֧����
        /// </summary>
        [DefaultValue(0), ConcurrencyCheck]
        public int VoteUpCount { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [DefaultValue(0), ConcurrencyCheck]
        public int VoteDownCount { get; set; }

        /// <summary>
        /// ÿ��ƽ��������
        /// </summary>
        [ConcurrencyCheck]
        public double AverageViewCount { get; set; }

        /// <summary>
        /// �ܷ�����
        /// </summary>
        [ConcurrencyCheck]
        public int TotalViewCount { get; set; }

        /// <summary>
        /// �ύ��IP��ַ
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// ��ֹ����
        /// </summary>
        public bool DisableComment { get; set; }

        /// <summary>
        /// ��ֹת��
        /// </summary>
        public bool DisableCopy { get; set; }

        /// <summary>
        /// ����ģʽ
        /// </summary>
        public PostLimitMode? LimitMode { get; set; }

        /// <summary>
        /// ���Ƶ��������ŷָ�
        /// </summary>
        public string Regions { get; set; }

        /// <summary>
        /// �����ų����������ŷָ�
        /// </summary>
        public string ExceptRegions { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public virtual Category Category { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public virtual ICollection<Comment> Comment { get; set; }

        /// <summary>
        /// ר��
        /// </summary>
        public virtual ICollection<SeminarPost> Seminar { get; set; }

        /// <summary>
        /// ������ʷ�汾
        /// </summary>
        public virtual ICollection<PostHistoryVersion> PostHistoryVersion { get; set; }

        /// <summary>
        /// �����޸�����
        /// </summary>
        public virtual ICollection<PostMergeRequest> PostMergeRequests { get; set; }
    }

    /// <summary>
    /// ��������
    /// </summary>
    public enum PostLimitMode
    {
        [Description("����")]
        All,
        [Description("ָ�������ɼ���{0}")]
        AllowRegion,
        [Description("ָ���������ɼ���{0}")]
        ForbidRegion,
        [Description("�ɼ�������{0}���ų�������{1}")]
        AllowRegionExceptForbidRegion,
        [Description("���ɼ�������{0}���ų�������{1}")]
        ForbidRegionExceptAllowRegion
    }
}