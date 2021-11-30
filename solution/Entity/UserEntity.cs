using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entity
{
    [Table("user")]
    public class UserEntity : Base
    {
        [Key]
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("Account")]
        public string Account { get; set; }

        /// <summary>
        /// 名字
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("Password")]
        public string Password { get; set; }

        /// <summary>
        /// 秘钥
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("Secret")]
        public int Secret { get; set; }

        /// <summary>
        /// 状态 1正常 2禁用
        /// </summary>
        [Column(TypeName = "int(1)")]
        [MaxLength(1)]
        [JsonPropertyName("Status")]
        public int Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("Createtime")]
        public int Createtime { get; set; }

        /// <summary>
        /// 是否所在部门的管理员 1否 2是
        /// </summary>
        [Column(TypeName = "int(1)")]
        [MaxLength(1)]
        [JsonPropertyName("Admin")]
        public int Admin { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [Column(TypeName = "varchar(2097152)")]
        [MaxLength(2097152)]
        [JsonPropertyName("Avatar")]
        public string Avatar { get; set; }

        /// <summary>
        /// 桌面背景
        /// </summary>
        [Column(TypeName = "varchar(65535)")]
        [MaxLength(65535)]
        [JsonPropertyName("Wallpaper")]
        public string Wallpaper { get; set; }

        /// <summary>
        /// 个人权限(逗号分隔) 1新建 2读取 3修改 4删除 5下载 6上传 7复制 8移动 9分享
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("Permission")]
        public string Permission { get; set; }

        /// <summary>
        /// 是否超级管理员 1否 2是
        /// </summary>
        [Column(TypeName = "int(1)")]
        [MaxLength(1)]
        [JsonPropertyName("Master")]
        public int Master { get; set; }

        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("DepartmentID")]
        public int DepartmentID { get; set; }

        public UserEntity()
        {
            this.ID = 0;
            this.Account = "";
            this.Name = "";
            this.Password = "";
            this.Secret = 0;
            this.Status = 0;
            this.Createtime = 0;
            this.Admin = 0;
            this.Avatar = "";
            this.Wallpaper = "";
            this.Permission = "1,2,3,4,5,6,7,8"; // 1新建 2读取 3修改(名称及内容) 4删除 5下载 6上传 7复制 8移动(剪切)
            this.Master = 0;
            this.DepartmentID = 0;
        }
    }
}
