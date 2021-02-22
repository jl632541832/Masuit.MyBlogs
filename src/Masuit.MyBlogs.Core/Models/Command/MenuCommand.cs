using Masuit.MyBlogs.Core.Models.Entity;
using Masuit.MyBlogs.Core.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace Masuit.MyBlogs.Core.Models.Command
{
    /// <summary>
    /// �����˵�����ģ��
    /// </summary>
    public class MenuCommand : BaseEntity
    {
        public MenuCommand()
        {
            ParentId = 0;
            Status = Status.Available;
        }

        /// <summary>
        /// ����
        /// </summary>
        [Required(ErrorMessage = "�˵�������Ϊ�գ�"), MaxLength(16, ErrorMessage = "�˵����֧��16���ַ���"), MinLength(2, ErrorMessage = "�˵���������Ҫ2���ַ���")]
        public string Name { get; set; }

        /// <summary>
        /// ͼ��
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        [Required(ErrorMessage = "�˵���URL����Ϊ�գ�"), StringLength(256, ErrorMessage = "URL�֧��256���ַ���")]
        public string Url { get; set; }

        /// <summary>
        /// �����
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// ����ID
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// �˵�����
        /// </summary>
        [Required]
        public MenuType MenuType { get; set; }

        /// <summary>
        /// �Ƿ����±�ǩҳ��
        /// </summary>
        public bool NewTab { get; set; }
    }
}