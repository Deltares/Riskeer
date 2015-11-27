using System.Collections.Generic;

namespace Core.Common.Base.Test.TestObjects
{
    public class Parent : Observable
    {
        public readonly IList<Child> Children = new List<Child>();
        public string Name { get; set; }
    }
}