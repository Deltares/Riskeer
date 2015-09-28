using System.Collections;
using System.Collections.Generic;

namespace DelftTools.Utils.Collections.Generic
{
    public interface IEnumerableList<T> : IList<T>, IList
    {
        IEnumerable<T> Enumerable { get; set; }

        IEnumerableListEditor Editor { get; set; }
    }
}