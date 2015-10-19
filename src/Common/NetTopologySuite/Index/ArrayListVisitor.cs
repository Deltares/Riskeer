using System.Collections;

namespace GisSharpBlog.NetTopologySuite.Index
{
    /// <summary>
    /// 
    /// </summary>
    public class ArrayListVisitor : IItemVisitor
    {
        private readonly ArrayList items = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        public ArrayList Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void VisitItem(object item)
        {
            items.Add(item);
        }
    }
}