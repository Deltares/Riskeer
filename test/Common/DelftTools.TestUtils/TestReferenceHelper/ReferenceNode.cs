using System;
using System.Collections.Generic;
using System.Text;

namespace DelftTools.TestUtils.TestReferenceHelper
{
    public class ReferenceNode
    {
        public ReferenceNode(object o)
        {
            Object = o;

            try
            {
                Name = Object.ToString();
            }
            catch (Exception)
            {
                Name = "<exception>";
            }
            Links = new List<ReferenceLink>();
            Path = new List<ReferenceLink>();
        }

        public object Object { get; private set; }
        
        public string Name { get; private set; }
        
        public IList<ReferenceLink> Links { get; private set; }
        
        public IList<ReferenceLink> Path { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] - {1}", ToPathString(), Object);
        }

        public string ToPathString()
        {
            StringBuilder pathString = new StringBuilder();

            if (Path.Count > 0)
            {
                pathString.Append(String.Format("{0}", Path[0].From.Name));
            }
            foreach(var link in Path)
            {
                pathString.Append(String.Format(".{0}", link.Name));
            }

            return pathString.ToString();
        }

        protected bool Equals(ReferenceNode other)
        {
            return Equals(Object, other.Object);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReferenceNode)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Object != null ? Object.GetHashCode() : 0);
            }
        }
    }
}