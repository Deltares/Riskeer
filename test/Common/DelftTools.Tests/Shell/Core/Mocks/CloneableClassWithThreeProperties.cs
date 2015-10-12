using System;
using System.ComponentModel;
using DelftTools.Utils;

namespace DelftTools.Tests.Shell.Core.Mocks
{
    /// <summary>
    /// added INameable to test for name updates after linking / unlinking dataitem
    /// </summary>
    public class CloneableClassWithThreeProperties : INotifyPropertyChange, INameable, ICloneable
    {
        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        private string stringField;
        private string name;

        public int IntField { get; set; }

        public string StringProperty
        {
            get
            {
                return stringField;
            }
            set
            {
                if (PropertyChanging != null)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs("StringProperty"));
                }

                stringField = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("StringProperty"));
                }
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (PropertyChanging != null)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs("Name"));
                }

                name = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        bool INotifyPropertyChange.HasParent { get; set; }

        public object Clone()
        {
            return new CloneableClassWithThreeProperties
            {
                IntField = IntField,
                Name = Name,
                StringProperty = StringProperty
            };
        }
    }
}