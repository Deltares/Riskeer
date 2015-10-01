using DelftTools.Utils;
using DelftTools.Utils.Aop;
using log4net;

namespace DelftTools.Shell.Gui
{
    ///<summary>
    /// TODO: separate manager from domain object
    ///</summary>
    [Entity(FireOnPropertyChange = false)]
    public class GuiContextManager : IManualCloneable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiContextManager));

        public virtual string Name { get { return "gui context"; } set { } }

        public virtual object DeepClone()
        {
            var clone = new GuiContextManager {Name = Name};

            log.Warn("Cloning view contexts is not supported yet.");

            return clone;
        }

        public virtual object Clone()
        {
            return DeepClone();
        }
    }
}