using System.ComponentModel;

namespace DelftTools.Utils.Tests.Collections.Generic
{
    public class MockClassWithTwoProperties : INotifyPropertyChange
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
        private string stringField;

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

        bool INotifyPropertyChange.HasParent { get; set; }
    }
}