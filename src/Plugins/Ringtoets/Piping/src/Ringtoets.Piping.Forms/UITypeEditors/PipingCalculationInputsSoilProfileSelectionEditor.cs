using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DelftTools.Utils.PropertyBag.Dynamic;
using DelftTools.Utils.Reflection;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.UITypeEditors
{
    public class PipingCalculationInputsSoilProfileSelectionEditor : UITypeEditor
    {
        private IWindowsFormsEditorService editorService;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            }

            if (editorService != null)
            {
                // Create editor:
                var listBox = CreateSoilProfileSelectionControl(context);

                // Open editor for user to select an item:
                editorService.DropDownControl(listBox);

                // Return user selected object, or original value if user did not select anything:
                return listBox.SelectedItem ?? value;
            }
            return base.EditValue(context, provider, value);
        }

        private ListBox CreateSoilProfileSelectionControl(ITypeDescriptorContext context)
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.One,
                DisplayMember = TypeUtils.GetMemberName<PipingSoilProfile>(sl => sl.Name)
            };
            listBox.SelectedValueChanged += (sender, eventArgs) => editorService.CloseDropDown();

            var properties = (PipingCalculationInputsProperties)((DynamicPropertyBag)context.Instance).WrappedObject;
            foreach (var soilProfile in properties.GetAvailableSoilProfiles())
            {
                int index = listBox.Items.Add(soilProfile);
                if (ReferenceEquals(properties.SoilProfile, soilProfile))
                {
                    listBox.SelectedIndex = index;
                }
            }
            return listBox;
        } 
    }
}