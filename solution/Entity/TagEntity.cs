using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity
{
    [Table("tag")]
    public class TagEntity : Base
    {
        [Key]
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        [Column(TypeName = "varchar(68)")]
        [MaxLength(68)]
        [JsonPropertyName("TagName")]
        public string TagName { get; set; }

        /// <summary>
        /// 标签备注
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("TagMemo")]
        public string TagMemo { get; set; }

        /// <summary>
        /// 所属人员
        /// </summary>
        [Column(TypeName = "varchar(10)")]
        [MaxLength(10)]
        [JsonPropertyName("UserID")]
        public int UserID { get; set; }

        public TagEntity()
        {
            this.ID = 0;
            this.TagName = "";
            this.TagMemo = "";
            this.UserID = 0;
        }
    }
}
