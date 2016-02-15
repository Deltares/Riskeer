// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;

using Core.Common.Gui.Properties;

using log4net;

//snatched from http://www.codeproject.com/KB/vb/CustomSettingsProvider.aspx

namespace Core.Common.Gui.Settings
{
    /// <summary>
    /// <see cref="SettingsProvider"/> that allows for library settings (<c>*.config</c>)
    /// to be stored at a different location then next to the corresponding library *.dll.
    /// </summary>
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

        /// <summary>
        /// Gets or sets the file location where the settings should be stored.
        /// </summary>
        public static string SettingsFilePath { get; set; }

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
                SettingsXML.Save(SettingsFilePath);
            }
            catch
            {
                log.ErrorFormat(Resources.PortableSettingsProvider_SetPropertyValues_Error_storing_settings_to_0_, SettingsFilePath);
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
                    var filePath = SettingsFilePath;
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
                throw new NotImplementedException(String.Format(Resources.PortableSettingsProvider_ThrowIfRoaming_Setting_0_is_roaming_This_is_not_supported, prop.Name));
            }
        }
    }
}