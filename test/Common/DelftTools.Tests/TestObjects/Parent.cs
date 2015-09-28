using System.Collections.Generic;

namespace DelftTools.Tests.TestObjects
{
    public class Parent
    {
        public string Name { get; set; }
        public IList<Child> Children = new List<Child>();
    }
}