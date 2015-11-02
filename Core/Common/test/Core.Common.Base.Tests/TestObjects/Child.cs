using System.Collections.Generic;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Collections.Generic;

namespace Core.Common.DelftTools.Tests.TestObjects
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