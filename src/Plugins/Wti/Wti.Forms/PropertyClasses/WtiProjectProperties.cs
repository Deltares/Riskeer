using DelftTools.Shell.Gui;

using Wti.Data;

namespace Wti.Forms.PropertyClasses
{
    public class WtiProjectProperties : ObjectProperties<WtiProject>
    {
        public string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
                data.NotifyObservers();
            }
        }
    }
}