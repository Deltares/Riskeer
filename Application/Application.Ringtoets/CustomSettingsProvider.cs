using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using Application.Ringtoets.Properties;
using log4net;

//snatched from http://www.codeproject.com/KB/vb/CustomSettingsProvider.aspx

namespace Application.Ringtoets
{
    public class PortableSettingsProvider : SettingsProvider
    {
        //XML Root Node
        private const string SETTINGSROOT = "Settings";
        private static readonly ILog log = LogManager.GetLogger(typeof(PortableSettingsProvider));

        private XmlDocument m_SettingsXML = null;

        public override string ApplicationName
        {
            get
            {
                if (System.Windows.Forms.Application.ProductName.Trim().Length > 0)
                {
                    return System.Windows.Forms.Application.ProductName;
                }

                var fi = new FileInfo(System.Windows.Forms.Application.ExecutablePath);
                return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
            }
            //Do nothing
            set {}
        }

        public static string SettingsFileName { get; set; }

        public override void Initialize(string name, NameValueCollection col)
        {
            base.Initialize(ApplicationName, col);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals)
        {
            //Iterate through the settings to be stored
            //Only dirty settings are included in propvals, and only ones relevant to this provider
            foreach (SettingsPropertyValue propval in propvals)
            {
                SetValue(propval);
            }

            try
            {
                SettingsXML.Save(SettingsFileName);
            }
            catch
            {
                log.ErrorFormat(Resources.PortableSettingsProvider_SetPropertyValues_Error_storing_settings_to__0_, SettingsFileName);
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            //Create new collection of values
            var values = new SettingsPropertyValueCollection();

            //Iterate through the settings to be retrieved

            foreach (SettingsProperty setting in props)
            {
                var value = new SettingsPropertyValue(setting)
                {
                    IsDirty = false,
                    SerializedValue = GetValue(setting)
                };
                values.Add(value);
            }
            return values;
        }

        private XmlDocument SettingsXML
        {
            get
            {
                //If we dont hold an xml document, try opening one.  
                //If it doesnt exist then create a new one ready.
                if (m_SettingsXML == null)
                {
                    m_SettingsXML = new XmlDocument();
                    var filePath = SettingsFileName;
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            m_SettingsXML.Load(filePath);
                        }
                        catch (Exception)
                        {
                            //Create new document
                            CreateNewSettingsXml();
                        }
                    }
                    else
                    {
                        CreateNewSettingsXml();
                    }
                }

                return m_SettingsXML;
            }
        }

        private void CreateNewSettingsXml()
        {
            XmlDeclaration dec = m_SettingsXML.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
            m_SettingsXML.AppendChild(dec);

            XmlNode nodeRoot;

            nodeRoot = m_SettingsXML.CreateNode(XmlNodeType.Element, SETTINGSROOT, "");
            m_SettingsXML.AppendChild(nodeRoot);
        }

        private string GetValue(SettingsProperty setting)
        {
            ThrowIfRoaming(setting);
            var selectSingleNode = SettingsXML.SelectSingleNode("Settings/" + setting.Name);
            if (selectSingleNode != null)
            {
                return selectSingleNode.InnerText;
            }
            return (setting.DefaultValue != null) ? setting.DefaultValue.ToString() : "";
        }

        private void SetValue(SettingsPropertyValue propVal)
        {
            ThrowIfRoaming(propVal.Property);

            XmlElement SettingNode;

            try
            {
                SettingNode = (XmlElement) SettingsXML.SelectSingleNode(SETTINGSROOT + "/" + propVal.Name);
            }
            catch (Exception)
            {
                SettingNode = null;
            }

            //Check to see if the node exists, if so then set its new value
            if ((SettingNode != null))
            {
                SettingNode.InnerText = propVal.SerializedValue.ToString();
            }
            else
            {
                //Store the value as an element of the Settings Root Node
                SettingNode = SettingsXML.CreateElement(propVal.Name);
                SettingNode.InnerText = propVal.SerializedValue.ToString();
                SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(SettingNode);
            }
        }

        private static void ThrowIfRoaming(SettingsProperty prop)
        {
            //maybe we should treat everything as roaming and simlpy not throw here?

            //Determine if the setting is marked as Roaming
            if ((from DictionaryEntry d in prop.Attributes select (Attribute) d.Value).OfType<SettingsManageabilityAttribute>().Any())
            {
                throw new NotImplementedException(String.Format(Resources.PortableSettingsProvider_ThrowIfRoaming_Setting__0__is_roaming__This_is_not_supported, prop.Name));
            }
        }
    }
}