using DelftTools.Utils.Aop;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    [Entity(FireOnCollectionChange=false)]
    public class ChildObject
    {
        [NoNotifyPropertyChange]
        private ParentObject parent;

        public string Name { get; set; }

        [NoNotifyPropertyChange]
        public string NameWithoutAspect { get; set; }

        [NoNotifyPropertyChange] // skip notification bubbling
        public ParentObject Parent
        {
            get { return parent; }
            set { parent = value; }
        }
    }
}