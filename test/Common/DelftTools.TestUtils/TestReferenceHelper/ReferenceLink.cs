using System.Linq;

namespace DelftTools.TestUtils.TestReferenceHelper
{
    public class ReferenceLink
    {
        public ReferenceLink(ReferenceNode from, ReferenceNode to, string name)
        {
            From = from;
            To = to;
            Name = name;
        }

        public ReferenceNode From { get; private set; }
        public ReferenceNode To { get; private set; }
        public string Name { get; private set; }

        public string ToPathString()
        {
            var lastLink = From.Links.First(link => link.To == To);

            return From.ToPathString() + "." + lastLink.Name;
        }
    }
}