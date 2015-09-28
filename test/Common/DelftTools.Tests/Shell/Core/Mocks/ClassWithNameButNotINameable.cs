using System.ComponentModel;
using DelftTools.Utils;

namespace DelftTools.Tests.Shell.Core.Mocks
{
    public class ClassWithNameButNotINameable : INotifyPropertyChange
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this,new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        bool INotifyPropertyChange.HasParent
        {
            get; set;
        }
    }
}