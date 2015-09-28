using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace DelftTools.Shell.Gui
{
    public class GuiPluginConfigurationSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {            
            if(serializer == null)
            {
                serializer = new XmlSerializer(typeof(guiPlugin));                
            }

            var reader = new XmlNodeReader(section);          
            return serializer.Deserialize(reader);
        }

        private static XmlSerializer serializer;
    }
}