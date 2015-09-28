using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections.Generic;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    [Entity]
    public class ProblemParentObject
    {
        public ProblemChildObject Child { get; set; }

        public IEventedList<ProblemChildObject> Children { get; set; }

        public ProblemParentObject()
        {
            Children = new EventedList<ProblemChildObject>();
        }

        /// <summary>
        /// a bastard is a child that was privately set ;)
        /// </summary>
        public ProblemChildObject Bastard { get; private set; }
        
    }
}