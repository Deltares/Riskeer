namespace SharpMap.Data.Providers.EGIS.ShapeFileLib
{
    internal class EndianUtils
    {

        private EndianUtils()
        {
        }

        public static int ReadIntBE(byte[] data, int offset)
        {
            int result = data[offset];
            result= (result<<8)|data[offset+1];
            result= (result<<8)|data[offset+2];
            result= (result<<8)|data[offset+3];            
            return result;
        }

        /// <summary>
        /// swaps the bytes ordering of the data at offset
        /// ie. bytes offset, ofset+1, offset+2, offset+3 become offset+3, offset+2, offset+1, offset
        /// This can be used to convert a BE int to a LE int and vice-versa
        /// Note that no bounds checks are performed so offset must be &lt;= data.length-4
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        public static void SwapIntBytes(byte[] data, int offset)
        {
            byte temp = data[offset];
            data[offset] = data[offset+3];
            data[offset+3]=temp;

            temp = data[offset+1];
            data[offset+1] = data[offset+2];
            data[offset+2] = temp;
        }

    }
}