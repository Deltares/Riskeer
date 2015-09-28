using DelftTools.Utils;

namespace Wti.Data
{
    public class WtiProject : INameable
    {
        public WtiProject()
        {
            Name = "WTI 2017 project";
        }

        public string Name { get; set; }
    }
}