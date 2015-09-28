using System;
using System.Collections.Generic;

namespace DeltaShell.Plugins.CommonTools.Gui.Property
{
    public class AttributeProperties<T>
    {
        private readonly IDictionary<string, T> dictionary;
        private readonly string key;

        public AttributeProperties(IDictionary<string, T> dictionary, string key)
        {
            this.dictionary = dictionary;
            this.key = key;
        }

        public override string ToString()
        {
            return string.Format("{0} : ({1})", key, dictionary[key]);
        }

        public string Key
        {
            get { return key; }
        }

        public string Value
        {
            get
            {
                return dictionary[key].ToString();
            }
            set
            {
                dictionary[key] = (T) Convert.ChangeType(value, typeof (T));
            }
        }
    }
}