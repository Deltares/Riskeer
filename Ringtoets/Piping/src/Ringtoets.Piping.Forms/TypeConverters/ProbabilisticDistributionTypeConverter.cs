using System;
using System.ComponentModel;
using System.Linq.Expressions;

using Core.Common.BaseDelftTools;
using Core.Common.Utils.PropertyBag.Dynamic;
using Core.Common.Utils.Reflection;

using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// Base class for shared implementation of <see cref="TypeConverter"/> to provide probabilistic
    /// distributions to the property editor.
    /// </summary>
    /// <typeparam name="T">Type of distribution</typeparam>
    public abstract class ProbabilisticDistributionTypeConverter<T> : TypeConverter
    {
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        protected static IObservable GetObservableOwnerOfDistribution(ITypeDescriptorContext context)
        {
            if (context == null)
            {
                return null;
            }
            // Sadly, we need this hack in order to update the correct class
            var dynamicPropertyBag = context.Instance as DynamicPropertyBag;
            if (dynamicPropertyBag == null)
            {
                return null;
            }

            // Note: If this type converter is going to be reused for other classes, we 
            //       might want to reconsider how we want to propagate IObservable updates!
            var pipingCalculationInputProperties = dynamicPropertyBag.WrappedObject as PipingCalculationInputsProperties;
            return pipingCalculationInputProperties != null ?
                       ((PipingCalculationInputs)pipingCalculationInputProperties.Data).PipingData :
                       null;
        }

        protected static PropertyDescriptor CreatePropertyDescriptor(PropertyDescriptorCollection originalProperties, Expression<Func<T, object>> propertyExpression, string customDisplayName, string customDescription, IObservable observableParent)
        {
            string propertyName = TypeUtils.GetMemberName(propertyExpression);
            PropertyDescriptor originalMeanPropertyDescriptor = originalProperties.Find(propertyName, false);
            return new TextPropertyDescriptorDecorator(originalMeanPropertyDescriptor,
                                                       customDisplayName,
                                                       customDescription)
            {
                ObservableParent = observableParent
            };
        }
    }
}