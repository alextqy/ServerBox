using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Entity
{
    [Table("department")]
    public class DepartmentEntity : Base
    {
        [Key]
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)]
        [JsonPropertyName("DepartmentName")]
        public string DepartmentName { get; set; }

        /// <summary>
        /// 部门上级ID
        /// </summary>
        [Column(TypeName = "int(10)")]
        [MaxLength(10)]
        [JsonPropertyName("ParentID")]
        public int ParentID { get; set; }

        /// <summary>
        /// 部门状态 1正常 2禁用
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

        public DepartmentEntity()
        {
            this.ID = 0;
            this.DepartmentName = "";
            this.ParentID = 0;
            this.State = 0;
            this.Createtime = 0;
        }
    }
}
