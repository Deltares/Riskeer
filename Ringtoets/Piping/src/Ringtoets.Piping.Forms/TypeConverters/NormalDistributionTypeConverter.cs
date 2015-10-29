using System;
using System.ComponentModel;
using System.Globalization;

using Core.Common.BaseDelftTools;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="NormalDistribution"/>
    /// properties.
    /// </summary>
    /// <remarks>This class has been designed to be used in <see cref="PipingCalculationInputsProperties"/>.
    /// If its reused somewhere else, change notification might not work properly.</remarks>
    public class NormalDistributionTypeConverter : ProbabilisticDistributionTypeConverter<NormalDistribution>
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var distribution = (NormalDistribution)value;
                return String.Format("Normale verdeling (\u03BC = {0}, \u03C3 = {1})",
                                     distribution.Mean.ToString(culture),
                                     distribution.StandardDeviation.ToString(culture));
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            IObservable observableParent = GetObservableOwnerOfDistribution(context);

            var propertyDescriptorCollection = TypeDescriptor.GetProperties(value);
            var properties = new PropertyDescriptor[2];
            properties[0] = CreateMeanPropertyDescriptor(propertyDescriptorCollection, observableParent);
            properties[1] = CreateStandardDeviationPropertyDescriptor(propertyDescriptorCollection, observableParent);

            return new PropertyDescriptorCollection(properties);
        }

        private PropertyDescriptor CreateMeanPropertyDescriptor(PropertyDescriptorCollection originalProperties, IObservable observableParent)
        {
            return CreatePropertyDescriptor(originalProperties, nd => nd.Mean, "\u03BC", "De gemiddelde waarde van de normale verdeling.", observableParent);
        }

        private PropertyDescriptor CreateStandardDeviationPropertyDescriptor(PropertyDescriptorCollection originalProperties, IObservable observableParent)
        {
            return CreatePropertyDescriptor(originalProperties, nd => nd.StandardDeviation, "\u03C3", "De standaardafwijking van de normale verdeling.", observableParent);
        }
    }
}