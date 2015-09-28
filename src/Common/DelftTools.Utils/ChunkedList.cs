using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DelftTools.Utils
{
    /// <summary>
    /// Implements IList but internally splits the list in several chunks. Although this decreases the performance
    /// of the list, it does help for large lists where allocating the entire list as a continuous block of memory 
    /// can be a problem. Note that if the list is fixed size, ChunkedArray provides a more performant alternative.
    /// 
    /// You can supply the type of the inner lists to be used; default is List<T>. You can also supply the desired
    /// chunk size (in elements, not in bytes); larger is generally more efficient in terms of performance.
    /// </summary>
    /// <typeparam name="TInnerList"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class ChunkedList<TInnerList, T> : IList<T>, IList 
        where TInnerList : class, IList<T>, new()
    {
        private readonly int DesiredChunkSize;
        private readonly int ChunkSplitSize;

        private readonly IList<TInnerList> chunks = new List<TInnerList>();
        private TInnerList tailChunk;
        private int count;

        private readonly IList<int> startIndices = new List<int>();
        private int startIndicesDirtyFrom = 1; //first added chunk is never dirty
        private int numChunks = 0;

        public ChunkedList(int desiredChunkSize = 65536)
        {
            DesiredChunkSize = desiredChunkSize;
            ChunkSplitSize = (int)(1.5 * DesiredChunkSize);
        }

        public T this[int index]
        {
            get
            {
                if (numChunks == 1) // fast paths
                    return tailChunk[index];

                var chunkIndex = GetChunkIndex(index);
                if (chunkIndex.OuterIndex < 0)
                    throw new ArgumentOutOfRangeException();
                return chunkIndex.Chunk[chunkIndex.IndexInChunk];
            }
            set
            {
                if (numChunks == 1) // fast paths
                    tailChunk[index] = value;

                var chunkIndex = GetChunkIndex(index);
                chunkIndex.Chunk[chunkIndex.IndexInChunk] = value;
            }
        }

        public void Add(T item)
        {
            if (tailChunk == null || tailChunk.Count >= DesiredChunkSize)
            {
                var chunk = new TInnerList();
                chunks.Add(chunk);
                numChunks++;
                startIndices.Add(count);
                startIndicesDirtyFrom = Math.Min(startIndicesDirtyFrom, numChunks);
                tailChunk = chunk;
            }
            tailChunk.Add(item);
            count++;
        }

        public void Clear()
        {
            chunks.Clear();
            numChunks = 0;
            startIndices.Clear();
            startIndicesDirtyFrom = 1;
            tailChunk = null;
            count = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var localIndex = 0;
            for (int i = arrayIndex; i < array.Length; i++)
            {
                array[i] = this[localIndex];
                localIndex++;
            }
        }

        public int IndexOf(T item)
        {
            return IndexOfCore(item).OuterIndex;
        }

        public void Insert(int index, T item)
        {
            if (index == count)
            {
                Add(item);
                return;
            }

            count++;
            var chunkIndex = GetChunkIndex(index);
            var chunk = chunkIndex.Chunk;
            chunk.Insert(chunkIndex.IndexInChunk, item);
            startIndicesDirtyFrom = Math.Min(startIndicesDirtyFrom, chunkIndex.IndexOfChunk + 1);

            var chunkCount = chunk.Count;
            if (chunkCount > ChunkSplitSize)
                SplitChunk(chunk, chunkCount);
        }

        private void SplitChunk(TInnerList chunk, int chunkCount)
        {
            var indexOfChunk = chunks.IndexOf(chunk);
            var newSize = chunkCount/2;
            var newChunk = new TInnerList();
            chunks.Insert(indexOfChunk + 1, newChunk);
            numChunks++;
            startIndices.Insert(indexOfChunk + 1, startIndices[indexOfChunk] + newSize);
            startIndicesDirtyFrom = Math.Min(startIndicesDirtyFrom, indexOfChunk + 2);

            var values = new T[chunkCount - newSize];
            for (int i = chunkCount - 1; i >= newSize; i--)
            {
                var value = chunk[i];
                chunk.RemoveAt(i);
                values[i - newSize] = value;
            }

            foreach (var value in values)
                newChunk.Add(value);

            if (tailChunk == chunk)
                tailChunk = newChunk;
        }

        public void RemoveAt(int index)
        {
            var chunkIndex = GetChunkIndex(index);
            if (chunkIndex.OuterIndex >= 0)
                RemoveAtCore(chunkIndex);
        }

        public int Count
        {
            get { return count; }
        }

        public bool Remove(T item)
        {
            var chunkIndex = IndexOfCore(item);
            if (chunkIndex.OuterIndex >= 0)
            {
                RemoveAtCore(chunkIndex);
                return true;
            }
            return false;
        }

        private void RemoveAtCore(ChunkIndex chunkIndex)
        {
            var chunk = chunkIndex.Chunk;
            chunk.RemoveAt(chunkIndex.IndexInChunk);
            startIndicesDirtyFrom = Math.Min(startIndicesDirtyFrom, chunkIndex.IndexOfChunk + 1);
            count--;

            if (chunk.Count != 0)
                return;

            // Chunk became empty, clean it up
            chunks.RemoveAt(chunkIndex.IndexOfChunk);
            numChunks--;
            startIndices.RemoveAt(chunkIndex.IndexOfChunk);
            startIndicesDirtyFrom = Math.Min(startIndicesDirtyFrom, chunkIndex.IndexOfChunk);

            //our first startIndex must always be valid (and 0):
            if (startIndicesDirtyFrom == 0) 
            {
                startIndicesDirtyFrom = 1;
                if (startIndices.Count > 0)
                    startIndices[0] = 0; // first one should always be 0
            }

            if (tailChunk == chunk)
                tailChunk = chunks.LastOrDefault();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ChunkedListEnumerator(this);
        }
        
        private class ChunkedListEnumerator : IEnumerator<T>
        {
            private ChunkedList<TInnerList, T> chunkedList;
            private readonly int count;
            private int localIndex = -1;
            private int indexOfChunk;
            private int sizeOfChunk;
            private TInnerList currentChunk;

            public ChunkedListEnumerator(ChunkedList<TInnerList, T> chunkedList)
            {
                this.chunkedList = chunkedList;
                count = chunkedList.count;
                Reset();
            }

            public bool MoveNext()
            {
                if (++localIndex >= sizeOfChunk) // go to next chunk
                {
                    if (++indexOfChunk >= chunkedList.chunks.Count)
                        return false;

                    localIndex = 0;
                    currentChunk = chunkedList.chunks[indexOfChunk];
                    sizeOfChunk = currentChunk.Count;
                }

                Current = currentChunk[localIndex];
                return true;
            }

            public void Dispose()
            {
                chunkedList = null; 
                currentChunk = null;
            }

            public void Reset()
            {
                localIndex = -1;
                indexOfChunk = 0;
                sizeOfChunk = 0;
                currentChunk = null;
                if (count > 0)
                {
                    currentChunk = chunkedList.chunks[indexOfChunk];
                    sizeOfChunk = currentChunk.Count;
                }
            }

            public T Current { get; private set; }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

        private ChunkIndex IndexOfCore(T item)
        {
            var outerIndex = 0;
            int indexOfChunk = 0;
            foreach (var chunk in chunks)
            {
                var inChunkIndex = chunk.IndexOf(item);
                if (inChunkIndex >= 0)
                {
                    return NewChunkIndex(chunk, indexOfChunk, inChunkIndex, outerIndex + inChunkIndex);
                }
                outerIndex += chunk.Count;
                indexOfChunk++;
            }
            return EmptyChunkIndex();
        }

        private ChunkIndex GetChunkIndex(int index)
        {
            if (numChunks == 1)
                return NewChunkIndex(tailChunk, 0, index, index);
    
            // quick path:
            var estimateChunkIndex = index/DesiredChunkSize;
            if (estimateChunkIndex < startIndicesDirtyFrom && estimateChunkIndex < numChunks)
            {
                var estimateChunk = chunks[estimateChunkIndex];
                var startIndex = startIndices[estimateChunkIndex];
                if (index >= startIndex && index < startIndex + estimateChunk.Count)
                {
                    return NewChunkIndex(estimateChunk, estimateChunkIndex, index - startIndex, index);
                }
            }
            // complex path:
            return GetChunkIndexSearch(index);
        }

        private ChunkIndex GetChunkIndexSearch(int index)
        {
            // todo: optimize & make more compact
            var lastNonDirtyChunkIndex = startIndicesDirtyFrom - 1;
            var lastNonDirtyStartIndex = startIndices[lastNonDirtyChunkIndex];
            var chunkStartIndex = lastNonDirtyStartIndex + chunks[lastNonDirtyChunkIndex].Count;
            var searchNonDirty = index < chunkStartIndex;

            if (searchNonDirty)
            {
                var estimateChunkIndex = Math.Min(lastNonDirtyChunkIndex, Math.Min(index / DesiredChunkSize, numChunks - 1));
                var estimateChunkStartIndex = startIndices[estimateChunkIndex];
                if (estimateChunkStartIndex > index)
                {
                    chunkStartIndex = estimateChunkStartIndex;
                    for (var chunkIndex = estimateChunkIndex - 1; chunkIndex >= 0; chunkIndex--)
                    {
                        var chunk = chunks[chunkIndex];
                        var chunkCount = chunk.Count;
                        chunkStartIndex -= chunkCount;

                        if (index >= chunkStartIndex)
                            return NewChunkIndex(chunk, chunkIndex, index - chunkStartIndex, index);
                    }
                }
                else
                {
                    chunkStartIndex = estimateChunkStartIndex;
                    for (var chunkIndex = estimateChunkIndex; chunkIndex <= lastNonDirtyChunkIndex; chunkIndex++)
                    {
                        var chunk = chunks[chunkIndex];
                        var chunkCount = chunk.Count;
                        
                        if (index < chunkStartIndex + chunkCount)
                            return NewChunkIndex(chunk, chunkIndex, index - chunkStartIndex, index);
                        chunkStartIndex += chunkCount;
                    }
                }
            }
            else
            {
                // search dirty area (and update start indices while we're at it)
                for (var chunkIndex = startIndicesDirtyFrom; chunkIndex < numChunks; chunkIndex++)
                {
                    var chunk = chunks[chunkIndex];
                    var chunkCount = chunk.Count;
                    startIndices[chunkIndex] = chunkStartIndex;
                    startIndicesDirtyFrom = chunkIndex + 1;
                    if (index < chunkStartIndex + chunkCount)
                    {
                        return NewChunkIndex(chunk, chunkIndex, index - chunkStartIndex, index);
                    }
                    chunkStartIndex += chunkCount;
                }
            }
            return EmptyChunkIndex();
        }

        // TODO: single instance causes threading problems, new instance every time hurts performance, for now reverting (TOOLS-20543)
        private readonly ChunkIndex indexInstance = new ChunkIndex();
        private ChunkIndex NewChunkIndex(TInnerList chunk, int indexOfChunk, int indexInChunk, int outerIndex)
        {
            indexInstance.Chunk = chunk;
            indexInstance.IndexOfChunk = indexOfChunk;
            indexInstance.IndexInChunk = indexInChunk;
            indexInstance.OuterIndex = outerIndex;
            return indexInstance;
        }
        private ChunkIndex EmptyChunkIndex()
        {
            indexInstance.Chunk = null;
            indexInstance.IndexOfChunk = -1;
            indexInstance.IndexInChunk = -1;
            indexInstance.OuterIndex = -1;
            return indexInstance;
        }
        private class ChunkIndex
        {
            public TInnerList Chunk;
            public int OuterIndex;
            public int IndexOfChunk;
            public int IndexInChunk;
        }

        public object SyncRoot { get { return null; } }
        public bool IsSynchronized { get { return false; } }
        public bool IsReadOnly { get { return false; } }
        public bool IsFixedSize { get { return false; } }

        #region IList Impl

        object IList.this[int index]
        {
            get { return this[index]; }
            set
            {
                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(
                        string.Format("The value \"{0}\" is not of type \"{1}\" and cannot be used in this generic collection.{2}Parameter name: value",
                            value, typeof(T), Environment.NewLine));
                }
            }
        }

        public int Add(object value)
        {
            Add((T)value);
            return count;
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>)this).GetEnumerator();
        }

        public bool Contains(object value)
        {
            return Contains((T)value);
        }

        public int IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        public void Remove(object value)
        {
            Remove((T)value);
        }

        #endregion
    }


    /// <summary>
    /// Implements IList but internally splits the list in several chunks. Although this decreases the performance
    /// of the list, it does help for large lists where allocating the entire list as a continuous block of memory 
    /// can be a problem. Note that if the list is fixed size, ChunkedArray provides a more performant alternative.
    /// 
    /// You can supply the type of the inner lists to be used; default is List<T>. You can also supply the desired
    /// chunk size (in elements, not in bytes); larger is generally more efficient in terms of performance.
    /// </summary>
    /// <typeparam name="TInnerList"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class ChunkedList<T> : ChunkedList<List<T>, T>
    {
        public ChunkedList(int desiredChunkSize = 65536) : base(desiredChunkSize)
        {
        }
    }
}