using System.Collections.Generic;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections.Generic;

namespace DelftTools.Tests.TestObjects
{
    [Entity]
    public class Child
    {
        public Child()
        {
            Children = new EventedList<Child>();
        }

        public string Name { get; set; }

        public IList<Child> Children { get; set; }
    }
}