using System.ComponentModel;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    internal class TestPublisher : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            set
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
    }
}