using System;
using System.ComponentModel;

using Core.Common.BaseDelftTools;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.TypeConverters
{
    /// <summary>
    /// A <see cref="TypeConverter"/> implementation for <see cref="LognormalDistribution"/>
    /// properties.
    /// </summary>
    /// <remarks>This class has been designed to be used in <see cref="PipingCalculationInputsProperties"/>.
    /// If its reused somewhere else, change notification might not work properly.</remarks>
    public class LognormalDistributionTypeConverter : ProbabilisticDistributionTypeConverter<LognormalDistribution>
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            IObservable observableParent = GetObservableOwnerOfDistribution(context);

            var propertyDescriptorCollection = TypeDescriptor.GetProperties(value);
            var properties = new PropertyDescriptor[2];
            properties[0] = CreateMeanPropertyDescriptor(propertyDescriptorCollection, observableParent);
            properties[1] = CreateStandardDeviationPropertyDescriptor(propertyDescriptorCollection, observableParent);

            return new PropertyDescriptorCollection(properties);
        }

        protected override ParameterDefinition<LognormalDistribution>[] Parameters
        {
            get
            {
                return new[]
                {
                    new ParameterDefinition<LognormalDistribution>
                    {
                        Symbol = "\u03BC", GetValue = distribution => distribution.Mean
                    },
                    new ParameterDefinition<LognormalDistribution>
                    {
                        Symbol = "\u03C3", GetValue = distribution => distribution.StandardDeviation
                    }
                };
            }
        }

        protected override string DistributionName
        {
            get
            {
                return "Lognormale verdeling";
            }
        }

        private PropertyDescriptor CreateMeanPropertyDescriptor(PropertyDescriptorCollection originalProperties, IObservable observableParent)
        {
            return CreatePropertyDescriptor(originalProperties, nd => nd.Mean, "\u03BC", "De gemiddelde waarde van de lognormale verdeling.", observableParent);
        }

        private PropertyDescriptor CreateStandardDeviationPropertyDescriptor(PropertyDescriptorCollection originalProperties, IObservable observableParent)
        {
            return CreatePropertyDescriptor(originalProperties, nd => nd.StandardDeviation, "\u03C3", "De standaardafwijking van de lognormale verdeling.", observableParent);
        }
    }
}