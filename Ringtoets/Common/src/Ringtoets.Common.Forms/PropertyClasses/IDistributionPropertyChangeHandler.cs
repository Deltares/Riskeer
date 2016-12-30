namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Interface defining the operations of handling a change of <see cref="DistributionPropertiesBase{T}"/>.
    /// </summary>
    public interface IDistributionPropertyChangeHandler
    {
        /// <summary>
        /// Defines the action that is executed after a property of <see cref="DistributionPropertiesBase{T}"/>
        /// has been changed.
        /// </summary>
        void PropertyChanged();
    }
}