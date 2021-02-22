using Masuit.MyBlogs.Core.Models.Enum;
using Masuit.Tools.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Masuit.MyBlogs.Core.Models.Entity
{
    /// <summary>
    /// �����˵�
    /// </summary>
    [Table("Menu")]
    public class Menu : BaseEntity, ITree<Menu>
    {
        public Menu()
        {
            ParentId = 0;
            Status = Status.Available;
            Children = new List<Menu>();
        }

        /// <summary>
        /// ����
        /// </summary>
        [Required(ErrorMessage = "�˵�������Ϊ�գ�")]
        public string Name { get; set; }

        /// <summary>
        /// ���ڵ�
        /// </summary>
        public virtual Menu Parent { get; set; }

        /// <summary>
        /// �Ӽ�
        /// </summary>
        public virtual ICollection<Menu> Children { get; set; }

        /// <summary>
        /// ͼ��
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        [Required(ErrorMessage = "�˵���URL����Ϊ�գ�")]
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
        public virtual MenuType MenuType { get; set; }

        /// <summary>
        /// �Ƿ����±�ǩҳ��
        /// </summary>
        public bool NewTab { get; set; }
    }
}