using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity
{
    [Table("offline_task")]
    public class OfflineTaskEntity : Base
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
        /// 任务地址
        /// </summary>
        [Column(TypeName = "varchar(65535)")]
        [MaxLength(65535)]
        [JsonPropertyName("URL")]
        public string URL { get; set; }

        /// <summary>
        /// 状态 1未处理 2处理中 3处理完成 4任务异常
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

        public OfflineTaskEntity()
        {
            this.ID = 0;
            this.UserID = 0;
            this.URL = "";
            this.Status = 0;
            this.Createtime = 0;
        }
    }
}
