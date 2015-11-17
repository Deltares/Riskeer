using System.Collections.Generic;

namespace Core.Common.Base.Tests.TestObjects
{
    public class Parent : Observable
    {
        public readonly IList<Child> Children = new List<Child>();
        public string Name { get; set; }
    }
}