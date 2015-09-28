using DelftTools.Utils.Aop;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    [Entity(FireOnCollectionChange=false)]
    public class ProblemChildObject 
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return Name ?? "";
        }
    }
}