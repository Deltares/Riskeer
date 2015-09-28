namespace DelftTools.Utils
{
    // source: http://blogs.msdn.com/b/joshwil/archive/2005/08/10/450202.aspx
    // adjusted to use as ChunkedArray to deal with fragmentation (not as >2gb array)
    public class ChunkedArray<T>
    {
        // These need to be const so that the getter/setter get inlined by the JIT into 
        // calling methods just like with a real array to have any chance of meeting our 
        // performance goals.
        //
        // BLOCK_SIZE must be a power of 2, and we want it to be big enough that we allocate
        // blocks in the large object heap so that they don't move.
        public const int BLOCK_SIZE = 32768;
        private const int BLOCK_SIZE_LOG2 = 15;

        // Don't use a multi-dimensional array here because then we can't right size the last
        // block and we have to do range checking on our own and since there will then be 
        // exception throwing in our code there is a good chance that the JIT won't inline.
        private readonly T[][] _elements;
        private readonly int _length;

        // maximum ChunkedArray size = BLOCK_SIZE * Int.MaxValue
        public ChunkedArray(int size)
        {
            int numBlocks = size/BLOCK_SIZE;
            if ((numBlocks * BLOCK_SIZE) < size)
            {
                numBlocks += 1;
            }

            _length = size;
            _elements = new T[numBlocks][];
            for (int i = 0; i < (numBlocks - 1); i++)
            {
                _elements[i] = new T[BLOCK_SIZE];
            }
            // by making sure to make the last block right sized then we get the range checks 
            // for free with the normal array range checks and don't have to add our own
            var numElementsInLastBlock = size%BLOCK_SIZE;
            if (numElementsInLastBlock == 0)
                numElementsInLastBlock = BLOCK_SIZE;
            _elements[numBlocks - 1] = new T[numElementsInLastBlock];
        }

        public int Length
        {
            get { return _length; }
        }

        public T this[int elementNumber]
        {
            // these must be _very_ simple in order to ensure that they get inlined into
            // their caller 
            get
            {
                int blockNum = elementNumber >> BLOCK_SIZE_LOG2;
                int elementNumberInBlock = elementNumber & (BLOCK_SIZE-1);
                return _elements[blockNum][elementNumberInBlock];
            }
            set
            {
                int blockNum = elementNumber >> BLOCK_SIZE_LOG2;
                int elementNumberInBlock = elementNumber & (BLOCK_SIZE-1);
                _elements[blockNum][elementNumberInBlock] = value;
            }
        }
    }
}