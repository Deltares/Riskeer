using System.ComponentModel;

namespace DelftTools.Utils.Tests.Collections.Generic
{
    public class MockClassWithTwoProperties : INotifyPropertyChange
    {
        private int intField;
        private string stringField;

        public int IntField
        {
            get { return intField; }
            set { intField = value; }
        }

        public string StringProperty
        {
            get { return stringField; }
            set
            {
                if (PropertyChanging != null)
                {
                    PropertyChanging(this, new PropertyChangingEventArgs("StringProperty"));
                }
                stringField = value;
                if(PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("StringProperty"));
                }
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
        
        bool INotifyPropertyChange.HasParent { get; set; }
    }
}