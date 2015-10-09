using System;

namespace DelftTools.Utils.Tests.Xml.Serialization.TestClasses
{
    [Serializable]
    //[XmlRoot("derivedclass", Namespace = "DelftTools.Utils.TestClasses", IsNullable = false)]
    public class DerivedClass : BaseClass
    {
        //private int id;
        protected int anotherId;

        public override int Id
        {
            get
            {
                return id;
            }
        }

        public int AnotherId
        {
            get
            {
                return anotherId;
            }
            set
            {
                anotherId = value;
            }
        }
    }
}