using Core.Common.Gui.PropertyBag;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Interface defining the operations of handling a change of <see cref="ObjectProperties{T}"/>.
    /// </summary>
    public interface IPropertyChangeHandler
    {
        /// <summary>
        /// Defines the action that is executed after a property of <see cref="ObjectProperties{T}"/>
        /// has been changed.
        /// </summary>
        void PropertyChanged();
    }
}