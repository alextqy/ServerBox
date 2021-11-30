using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity
{
    [Table("file")]
    public class FileEntity : Base
    {
        [Key]
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("FileName")]
        public string FileName { get; set; }

        /// <summary>
        /// 所属用户ID
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("UserID")]
        public int UserID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(TypeName = "int(13)")]
        [MaxLength(13)]
        [JsonPropertyName("Createtime")]
        public long Createtime { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("FileType")]
        public string FileType { get; set; }

        /// <summary>
        /// 文件状态 1正在上传 2正常 3锁定 4回收站
        /// </summary>
        [Column(TypeName = "int(1)")]
        [MaxLength(1)]
        [JsonPropertyName("State")]
        public int State { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [Column(TypeName = "varchar(16)")]
        [MaxLength(16)]
        [JsonPropertyName("FileSize")]
        public string FileSize { get; set; }

        /// <summary>
        /// 总文件分片数
        /// </summary>
        [Column(TypeName = "int(65535)")]
        [MaxLength(65535)]
        [JsonPropertyName("BlockSize")]
        public int BlockSize { get; set; }

        /// <summary>
        /// 已上传分片数
        /// </summary>
        [Column(TypeName = "int(65535)")]
        [MaxLength(65535)]
        [JsonPropertyName("UploadBlockSize")]
        public long UploadBlockSize { get; set; }

        /// <summary>
        /// 服务器存储路径
        /// </summary>
        [Column(TypeName = "varchar(65535)")]
        [MaxLength(65535)]
        [JsonPropertyName("ServerStoragePath")]
        public string ServerStoragePath { get; set; }

        /// <summary>
        /// 上传路径
        /// </summary>
        [Column(TypeName = "varchar(65535)")]
        [MaxLength(65535)]
        [JsonPropertyName("UploadPath")]
        public string UploadPath { get; set; }

        /// <summary>
        /// 归属文件夹ID
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("DirID")]
        public int DirID { get; set; }

        /// <summary>
        /// md5
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("MD5")]
        public string MD5 { get; set; }

        public FileEntity()
        {
            this.ID = 0;
            this.FileName = "";
            this.UserID = 0;
            this.Createtime = 0;
            this.FileType = "";
            this.State = 0;
            this.FileSize = "";
            this.BlockSize = 0;
            this.UploadBlockSize = 0;
            this.ServerStoragePath = "";
            this.UploadPath = "";
            this.DirID = 0;
            this.MD5 = "";
        }
    }
}
