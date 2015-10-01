using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.Data;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    /// <summary>
    /// See also <see cref="ClassWithoutAspects"/> to compare amount when aspects are not used.
    /// </summary>
    [Entity]
    public class ClassWithAspects : EditableObjectUnique<long>
    {
        public ClassWithAspects()
        {
            Children = new EventedList<ClassWithAspects>();
        }

        public IEventedList<ClassWithAspects> Children { get; set; }

        public ClassWithAspects Child { get; set; }

        public string Name { get; set; }
    }
}