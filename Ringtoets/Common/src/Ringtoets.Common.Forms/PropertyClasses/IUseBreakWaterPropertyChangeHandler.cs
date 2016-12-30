namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Interface defining the operations of handling a change of <see cref="UseBreakWaterProperties"/>.
    /// </summary>
    public interface IUseBreakWaterPropertyChangeHandler
    {
        /// <summary>
        /// Defines the action that is executed after a property of <see cref="UseBreakWaterProperties"/>
        /// has been changed.
        /// </summary>
        void PropertyChanged();
    }
}