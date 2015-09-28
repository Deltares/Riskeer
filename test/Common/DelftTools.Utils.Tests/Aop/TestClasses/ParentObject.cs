using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.Data;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    [Entity(FireOnCollectionChange=false)]
    public class ParentObject : EditableObjectUnique<long>
    {
        public ChildObject Child { get; set; }

        public IEventedList<ChildObject> Children { get; set; }

        public ParentObject()
        {
            Children = new EventedList<ChildObject>();
        }

        /// <summary>
        /// a bastard is a child that was privately set ;)
        /// </summary>
        public ChildObject Bastard { get; private set; }

        public void SetBastard(ChildObject childObject)
        {
            Bastard = childObject;
        }
    }
}