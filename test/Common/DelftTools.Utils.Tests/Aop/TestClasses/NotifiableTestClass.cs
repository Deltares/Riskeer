using DelftTools.Utils.Aop;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    /// <summary>
    /// Adding NotifyPropertyChanged attribute to the class declaration will ensure that INotifyPropertyChanged will be 
    /// injected on compilation time as additional aspect. During runtime it can be accessed in the following way:
    /// 
    /// INotifyPropertyChanged observable = Post.Cast<T, INotifyPropertyChanged>(o); 
    /// 
    /// It makes things a bit more implicit but also more clean (hopefully not too clean :)).
    /// </summary>
    [Entity(FireOnCollectionChange=false)]
    public class NotifiableTestClass
    {
        private string name = "some name";

        public virtual string Name
        {
            set { name = value; }
            get { return name; }
        }

        public virtual string AutoProperty { get; set; }

        public void SetNameUsingPrivateMethod(string name)
        {
            this.name = name;
        }

        public string PrivateSetPublicGet { get; private set; }
        public void SetPropertyWithPrivateSetter(string value)
        {
            PrivateSetPublicGet = value;
        }
               

        private string PrivateProperty { get; set; }
        public void SetPrivateProperty(string value)
        {
            PrivateProperty = value;
        }
        
    }

    [Entity(FireOnCollectionChange=false)]
    public class NotifiableTestSubClass:NotifiableTestClass
    {
        //just a 'redundant override' here
        public override string Name
        {
            set { base.Name= value; }
            get { return base.Name; }
        }
    }
}