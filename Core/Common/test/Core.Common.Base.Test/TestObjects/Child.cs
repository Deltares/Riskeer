using System.Collections.Generic;
using Core.Common.Utils.Collections.Generic;

namespace Core.Common.Base.Tests.TestObjects
{
    public class Child : Observable
    {
        public Child()
        {
            Children = new EventedList<Child>();
        }

        public string Name { get; set; }
        public IList<Child> Children { get; set; }
    }
}