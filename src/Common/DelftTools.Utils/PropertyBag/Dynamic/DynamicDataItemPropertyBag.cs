using System.ComponentModel;

namespace DelftTools.Utils.PropertyBag.Dynamic
{
    public class DynamicDataItemPropertyBag : DynamicPropertyBag
    {
        private object dataItemProperties;
        private DynamicPropertyBag dataItemPropertyBag;

        protected override void OnGetValue(PropertySpecEventArgs e)
        {
            if (dataItemPropertyBag.Properties.Contains(e.Property))
            {
                var descriptor = (dataItemPropertyBag as ICustomTypeDescriptor).GetProperties().Find(e.Property.Name,false);
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

        public DynamicDataItemPropertyBag(object dataItemProperties, object propertyObject)
            : base(propertyObject)
        {
            this.dataItemProperties = dataItemProperties;
            this.dataItemPropertyBag = new DynamicPropertyBag(dataItemProperties);

            FillProperties();
        }

        private void FillProperties()
        {
            foreach(PropertySpec property in dataItemPropertyBag.Properties)
            {
                if (!Properties.Contains(property.Name))
                {
                    Properties.Add(property);
                }
            }
        }
    }
}
