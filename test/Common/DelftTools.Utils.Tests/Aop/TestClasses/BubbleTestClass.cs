using System.ComponentModel;
using DelftTools.Utils.Aop;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    [Entity(FireOnCollectionChange=false)]
    internal class BubbleTestClass 
    {
        private INotifyPropertyChanged privateField;
        protected INotifyPropertyChanged ProtectedField;
        public INotifyPropertyChanged PublicField;

        public INotifyPropertyChanged AutoProperty { get; set; }

        public void SetPrivateField(INotifyPropertyChanged value)
        {
            privateField = value;
        }

        public void SetProtectedField(INotifyPropertyChanged value)
        {
            ProtectedField = value;
        }
        //public event PropertyChangedEventHandler PropertyChanged;
    }
}