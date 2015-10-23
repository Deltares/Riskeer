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
    /// <summary>
    /// This class defines a drop down list edit-control from which the user can select a
    /// <see cref="RingtoetsPipingSurfaceLine"/> from a collection.
    /// </summary>
    public class PipingCalculationInputsSurfaceLineSelectionEditor : UITypeEditor
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
                var listBox = CreateSurfaceLinesSelectionControl(context);

                // Open editor for user to select an item:
                editorService.DropDownControl(listBox);

                // Return user selected object, or original value if user did not select anything:
                return listBox.SelectedItem ?? value;
            }
            return base.EditValue(context, provider, value);
        }

        private ListBox CreateSurfaceLinesSelectionControl(ITypeDescriptorContext context)
        {
            var listBox = new ListBox
            {
                SelectionMode = SelectionMode.One,
                DisplayMember = TypeUtils.GetMemberName<RingtoetsPipingSurfaceLine>(sl => sl.Name)
            };
            listBox.SelectedValueChanged += (sender, eventArgs) => editorService.CloseDropDown();

            var properties = (PipingCalculationInputsProperties)((DynamicPropertyBag)context.Instance).WrappedObject;
            foreach (var surfaceLine in properties.GetAvailableSurfaceLines())
            {
                int index = listBox.Items.Add(surfaceLine);
                if (ReferenceEquals(properties.SurfaceLine, surfaceLine))
                {
                    listBox.SelectedIndex = index;
                }
            }
            return listBox;
        }
    }
}