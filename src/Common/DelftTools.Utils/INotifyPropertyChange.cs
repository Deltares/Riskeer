using System.ComponentModel;

namespace DelftTools.Utils
{
    public interface INotifyPropertyChange : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// Indicates that object already has parent defined. This property is set to true after instance is added to the object tree where
        /// consistency checks are enabled (see also <see cref="DelftTools.Utils.Aop.AggregationAttribute"/>). 
        /// 
        /// TODO: this property must be moved to IEntity.
        /// </summary>
        bool HasParent { get; set; }
    }
}