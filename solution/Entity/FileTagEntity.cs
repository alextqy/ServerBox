using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity
{
    [Table("file_tag")]
    public class FileTagEntity : Base
    {
        [Key]
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 文件ID
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("FileID")]
        public int FileID { get; set; }

        /// <summary>
        /// 标签ID
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("TagID")]
        public int TagID { get; set; }

        public FileTagEntity()
        {
            this.ID = 0;
            this.FileID = 0;
            this.TagID = 0;
        }
    }
}
