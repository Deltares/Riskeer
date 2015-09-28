using System.Collections;
using System.Collections.Generic;

namespace DelftTools.Utils
{
    public class DefaultListEnumerator<T> : IEnumerator<T>
    {
        private IList<T> list;
        private int index;
        private readonly int count;

        public DefaultListEnumerator(IList<T> list)
        {
            this.list = list;
            count = this.list.Count;
            index = -1;
        }

        public bool MoveNext()
        {
            if (++index >= count)
                return false;
            Current = list[index];
            return true;
        }

        public void Reset()
        {
            index = -1;
        }

        public void Dispose()
        {
            list = null;
        }

        public T Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}