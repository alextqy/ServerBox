using Service;

namespace ConfigHelper
{
    public class Base
    {
        public Base() { }
    }

    /// <summary>
    /// 系统配置
    /// </summary>
    public class AppSettingsObject
    {
        /// <summary>
        /// 项目地址
        /// </summary>
        /// <value></value>
        public string URL { get; set; }

        /// <summary>
        /// UDP端口
        /// </summary>
        /// <value></value>
        public int UDPPort { get; set; }

        /// <summary>
        /// 数据库
        /// </summary>
        /// <value></value>
        public string DataBase { get; set; }

        /// <summary>
        /// token生命周期(小时)
        /// </summary>
        public int TokenPeriod { get; set; }

        // 配置项实体
        public AppSettingsObject()
        {
            this.URL = "";
            this.UDPPort = 0;
            this.DataBase = "";
            this.TokenPeriod = 0;
        }
    }
}
