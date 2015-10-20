using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using DelftTools.Utils.Aop;

namespace DelftTools.Utils
{
    /// <summary>
    /// Web link, url.
    /// </summary>
    [Entity(FireOnCollectionChange = false)]
    public class Url : ICloneable, INameable, IXmlSerializable
    {
        private string path;

        private string name;

        public Url(string name, string path)
        {
            this.name = name;
            this.path = path;
        }

        public virtual string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
            }
        }

        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public object Clone()
        {
            return new Url(name, path);
        }

        [Obsolete("Xml serialization is optional for DeltaShell and is supported for backward compatibility with the old projects of some DeltaShell plugins.")]
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        [Obsolete("Xml serialization is optional for DeltaShell and is supported for backward compatibility with the old projects of some DeltaShell plugins.")]
        public void ReadXml(XmlReader reader)
        {
            name = reader.GetAttribute("name");
            path = reader.GetAttribute("path");
        }

        [Obsolete("Xml serialization is optional for DeltaShell and is supported for backward compatibility with the old projects of some DeltaShell plugins.")]
        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}