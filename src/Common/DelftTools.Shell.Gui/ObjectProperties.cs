using System.ComponentModel;

namespace DelftTools.Shell.Gui
{
    /// <summary>
    /// Base class for object properties with data of type <typeparam name="T"/>
    /// </summary>
    public class ObjectProperties<T> : IObjectProperties
    {
        protected T data;

        [Browsable(false)]
        public virtual object Data
        {
            get
            {
                return data;
            }
            set
            {
                data = (T) value;
            }
        }
    }
}