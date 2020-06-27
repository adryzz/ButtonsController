using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ButtonsController
{
    [Serializable()]
    public class Configuration
    {
        public string PortName = "COM1";
        public int BaudRate = 57600;
        public bool RunActionsOnNewThread = true;

        public Configuration()
        {
            //throw new NotImplementedException();
        }

        public void Save(string fileName)
        {
            XmlSerializer xml_serializer = new XmlSerializer(typeof(Configuration));

            using (StringWriter string_writer = new StringWriter())
            {
                // Serialize.
                xml_serializer.Serialize(string_writer, this);

                File.WriteAllText(fileName, string_writer.ToString());
            }
        }

        public static Configuration FromFile(string fileName)
        {
            string serialized = File.ReadAllText(fileName);

            XmlSerializer xml_serializer = new XmlSerializer(typeof(Configuration));

            Configuration cfg;

            using (StringReader string_reader = new StringReader(serialized))
            {
                cfg = (Configuration)(xml_serializer.Deserialize(string_reader));
            }

            return cfg;
        }
    }
}
