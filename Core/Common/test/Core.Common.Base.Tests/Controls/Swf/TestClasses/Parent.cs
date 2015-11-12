using Core.Common.Utils.Aop;
using Core.Common.Utils.Collections.Generic;

namespace Core.Common.Base.Tests.Controls.Swf.TestClasses
{
    [Entity]
    public class Parent
    {
        public Parent()
        {
            Children = new EventedList<Child>();
        }

        public IEventedList<Child> Children { get; set; }

        public string Name { get; set; }
    }
}