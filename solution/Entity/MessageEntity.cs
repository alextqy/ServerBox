using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity
{
    [Table("message")]
    public class MessageEntity : Base
    {
        [Key]
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Column(TypeName = "varchar(65535)")]
        [MaxLength(65535)]
        [JsonPropertyName("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 发送者ID
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("SenderID")]
        public int SenderID { get; set; }

        /// <summary>
        /// 接收者ID
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ReceiverID")]
        public int ReceiverID { get; set; }

        /// <summary>
        /// 状态 1未读 2已读
        /// </summary>
        [Column(TypeName = "int(1)")]
        [MaxLength(1)]
        [JsonPropertyName("State")]
        public int State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("Createtime")]
        public int Createtime { get; set; }

        public MessageEntity()
        {
            this.ID = 0;
            this.Title = "";
            this.Content = "";
            this.SenderID = 0;
            this.ReceiverID = 0;
            this.State = 0;
            this.Createtime = 0;
        }
    }
}
