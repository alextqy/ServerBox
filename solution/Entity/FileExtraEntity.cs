using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity
{
    [Table("file_extra")]
    public class FileExtraEntity : Base
    {
        [Key]
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 文件夹ID
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("FileID")]
        public int FileID { get; set; }

        /// <summary>
        /// 扩展描述
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("ExtraDesc")]
        public string ExtraDesc { get; set; }

        /// <summary>
        /// 扩展类型
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ExtraType")]
        public int ExtraType { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("ExtraValue")]
        public string ExtraValue { get; set; }

        public FileExtraEntity()
        {
            this.ID = 0;
            this.FileID = 0;
            this.ExtraDesc = "";
            this.ExtraType = 0;
            this.ExtraValue = "";
        }
    }
}
