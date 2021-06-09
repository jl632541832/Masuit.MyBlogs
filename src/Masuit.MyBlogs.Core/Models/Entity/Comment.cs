using Masuit.MyBlogs.Core.Models.Enum;
using Masuit.Tools.Core.Validator;
using Masuit.Tools.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masuit.MyBlogs.Core.Models.Entity
{
    /// <summary>
    /// ���۱�
    /// </summary>
    [Table("Comment")]
    public class Comment : BaseEntity, ITreeChildren<Comment>, ITreeParent<Comment>
    {
        public Comment()
        {
            Status = Status.Pending;
            IsMaster = false;
            Children = new List<Comment>();
        }

        /// <summary>
        /// �ǳ�
        /// </summary>
        [Required(ErrorMessage = "��ȻҪ���ۣ���������ô���أ�"), MaxLength(24, ErrorMessage = "���֣���������̫���˰ɣ�"), MinLength(2, ErrorMessage = "�ǳ�����2���֣�")]
        public string NickName { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [IsEmail]
        public string Email { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [Required(ErrorMessage = "�������ݲ���Ϊ�գ�")]
        public string Content { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CommentDate { get; set; }

        /// <summary>
        /// ������汾
        /// </summary>
        [StringLength(255)]
        public string Browser { get; set; }

        /// <summary>
        /// ����ϵͳ�汾
        /// </summary>
        [StringLength(255)]
        public string OperatingSystem { get; set; }

        /// <summary>
        /// �Ƿ��ǲ���
        /// </summary>
        [DefaultValue(false)]
        public bool IsMaster { get; set; }

        /// <summary>
        /// ֧����
        /// </summary>
        public int VoteCount { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public int AgainstCount { get; set; }

        /// <summary>
        /// �ύ��IP��ַ
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public string Location { get; set; }

        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }

        /// <summary>
        /// �Ӽ�
        /// </summary>
        public virtual ICollection<Comment> Children { get; set; }

        /// <summary>
        /// ���ڵ�
        /// </summary>
        public virtual Comment Parent { get; set; }
    }
}