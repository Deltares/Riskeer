using System;
using System.ComponentModel;
using Core.Common.Base;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// This class allows for overriding <see cref="DisplayName"/> and <see cref="Description"/>
    /// of a given <see cref="PropertyDescriptor"/>.
    /// </summary>
    public class TextPropertyDescriptorDecorator : PropertyDescriptor
    {
        private readonly string displayNameOverride;
        private readonly string descriptionOverride;
        private readonly PropertyDescriptor wrappedDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextPropertyDescriptorDecorator"/> class.
        /// </summary>
        /// <param name="descr">The property descriptor being decorated.</param>
        /// <param name="customDisplayName">The custom display name.</param>
        /// <param name="customDescription">The custom description.</param>
        public TextPropertyDescriptorDecorator(PropertyDescriptor descr, string customDisplayName, string customDescription)
            : base(descr)
        {
            displayNameOverride = customDisplayName;
            descriptionOverride = customDescription;
            wrappedDescriptor = descr;
        }

        public override string DisplayName
        {
            get
            {
                return displayNameOverride;
            }
        }

        public override string Description
        {
            get
            {
                return descriptionOverride;
            }
        }

        /// <summary>
        /// Gets or sets the observable object whose should notify all observers when 
        /// <see cref="SetValue"/> is called.
        /// </summary>
        public IObservable ObservableParent { get; set; }

        #region Members delegated to wrapped descriptor

        public override bool CanResetValue(object component)
        {
            return wrappedDescriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            return wrappedDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            wrappedDescriptor.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            wrappedDescriptor.SetValue(component, value);
            if (ObservableParent != null)
            {
                ObservableParent.NotifyObservers();
            }
        }

        public override bool ShouldSerializeValue(object component)
        {
            return wrappedDescriptor.ShouldSerializeValue(component);
        }

        public override Type ComponentType
        {
            get
            {
                return wrappedDescriptor.ComponentType;
            }
        }

        public override bool IsReadOnly
        {
            get
            {
                return wrappedDescriptor.IsReadOnly;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return wrappedDescriptor.PropertyType;
            }
        }

        #endregion
    }
}