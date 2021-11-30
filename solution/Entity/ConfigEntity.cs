using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity
{
    [Table("config")]
    public class ConfigEntity : Base
    {
        [Key]
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 键名
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("ConfigKey")]
        public string ConfigKey { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("ConfigDesc")]
        public string ConfigDesc { get; set; }

        /// <summary>
        /// 类型 1input 2textarea 3select
        /// </summary>
        [Column(TypeName = "int(1)")]
        [MaxLength(1)]
        [JsonPropertyName("ConfigType")]
        public int ConfigType { get; set; }

        /// <summary>
        /// 键值
        /// </summary>
        [Column(TypeName = "varchar(65535)")]
        [MaxLength(65535)]
        [JsonPropertyName("ConfigValue")]
        public string ConfigValue { get; set; }

        public ConfigEntity()
        {
            this.ID = 0;
            this.ConfigKey = "";
            this.ConfigDesc = "";
            this.ConfigType = 0;
            this.ConfigValue = "";
        }
    }
}
