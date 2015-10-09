using System.Collections.Generic;

namespace DelftTools.Tests.TestObjects
{
    public class Parent
    {
        public IList<Child> Children = new List<Child>();
        public string Name { get; set; }
    }
}