using System.ComponentModel;

namespace DelftTools.Utils.PropertyBag.Dynamic
{
    public class DynamicDataItemPropertyBag : DynamicPropertyBag
    {
        private readonly object dataItemProperties;
        private readonly DynamicPropertyBag dataItemPropertyBag;

        public DynamicDataItemPropertyBag(object dataItemProperties, object propertyObject)
            : base(propertyObject)
        {
            this.dataItemProperties = dataItemProperties;
            dataItemPropertyBag = new DynamicPropertyBag(dataItemProperties);

            FillProperties();
        }

        protected override void OnGetValue(PropertySpecEventArgs e)
        {
            if (dataItemPropertyBag.Properties.Contains(e.Property))
            {
                var descriptor = (dataItemPropertyBag as ICustomTypeDescriptor).GetProperties().Find(e.Property.Name, false);
                e.Value = descriptor.GetValue(dataItemProperties);
            }
            else
            {
                base.OnGetValue(e);
            }
        }

        protected override void OnSetValue(PropertySpecEventArgs e)
        {
            if (dataItemPropertyBag.Properties.Contains(e.Property))
            {
                var descriptor = (dataItemPropertyBag as ICustomTypeDescriptor).GetProperties().Find(e.Property.Name, false);
                descriptor.SetValue(dataItemProperties, e.Value);
            }
            else
            {
                base.OnSetValue(e);
            }
        }

        private void FillProperties()
        {
            foreach (PropertySpec property in dataItemPropertyBag.Properties)
            {
                if (!Properties.Contains(property.Name))
                {
                    Properties.Add(property);
                }
            }
        }
    }
}