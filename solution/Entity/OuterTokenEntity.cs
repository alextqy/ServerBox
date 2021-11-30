using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity
{
    [Table("outer_token")]
    public class OuterTokenEntity : Base
    {
        [Key]
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("UserID")]
        public int UserID { get; set; }

        /// <summary>
        /// 外部token
        /// </summary>
        [Column(TypeName = "varchar(65535)")]
        [MaxLength(65535)]
        [JsonPropertyName("OuterToken")]
        public string OuterToken { get; set; }

        /// <summary>
        /// 外部token描述
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("TokenDesc")]
        public string TokenDesc { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("Createtime")]
        public int Createtime { get; set; }

        public OuterTokenEntity()
        {
            this.ID = 0;
            this.UserID = 0;
            this.OuterToken = "";
            this.TokenDesc = "";
            this.Createtime = 0;
        }
    }
}
