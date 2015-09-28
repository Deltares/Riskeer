using System;
using System.ComponentModel;
using DelftTools.Utils;
using DelftTools.Utils.Data;

namespace DeltaShell.Plugins.ProjectExplorer.Tests.TestClasses
{
    /// <summary>
    /// Test data object which can be contained in the model / project (wrapperd by DataItem implementation). 
    /// </summary>
    public class TestDataObject: Unique<long>, INameable,INotifyPropertyChange, ICloneable, IEquatable<TestDataObject> //manually implemented INotifyPC don't need PostSharp for such a small assembly/class
    {
        private string field1;
        private string name;
        private static string defaultName = "test";

        public TestDataObject():this(defaultName)
        {
        }

        public TestDataObject(string name)
        {
            this.name = name;
        }

        public virtual string Field1
        {
            get { return field1; }
            set
            {
                OnPropertyChanging("Field1");
                field1 = value;
                OnPropertyChanged("Field1");
            }
        }
        
        public string Name
        {
            get { return name; }
            set
            {
                OnPropertyChanging("Name");
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this,new PropertyChangingEventArgs(propertyName));
            }
        }


        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
        
        bool INotifyPropertyChange.HasParent { get; set; }

        public object Clone()
        {
            return new TestDataObject
                {
                    Field1 = field1,
                    Name = name
                };
        }

        public override bool Equals(IUnique<long> other)
        {
            var testDataObject = other as TestDataObject;
            if (testDataObject != null)
            {
                return Equals(testDataObject);
            }
            return base.Equals(other);
        }

        public bool Equals(TestDataObject other)
        {
            if (other == null) return false;

            return field1 == other.Field1 &&
                   name == other.Name;
        }
    }
}