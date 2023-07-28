using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDS4Admin.Tool
{
    public class IDS4Config
    {
        public string STSUri { get; set; } = "http://127.0.0.1:4000";

        public string AdminUri { get; set; } = "http://127.0.0.1:4001";

        public string DbType { get; set; } = "MySql";

        public string DbConnString { get; set; } = "Data Source=127.0.0.1;port=3306;Database=IdentityServer4Admin;User ID=root;Password=abc@123;pooling=true;min pool size=5;max pool size=512;sslmode=none;CharSet=utf8mb4;Convert Zero Datetime=True;Allow Zero Datetime=True;AllowLoadLocalInfile=true;";

        public string IsDocker { get; set; } = "N";
        public void SaveToFile(string path)
        {
            var filename = Path.Combine(path, "IDS4config.xml");

            var x = new XmlSerializer(this.GetType());
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            x.Serialize(sw, this);
            File.WriteAllText(filename, sw.ToString());
        }

        public static IDS4Config Load(string path)
        {
            var filename = Path.Combine(path, "IDS4config.xml");
            if (File.Exists(filename))
            {
                var content = File.ReadAllText(filename);
                using (var srd1 = new StringReader(content))
                {
                    var xd1 = new XmlSerializer(typeof(IDS4Config));
                    var obj = xd1.Deserialize(srd1) as IDS4Config;
                    if(obj != null) return obj;
                }
            }
            return new IDS4Config();
        }
    }
}
