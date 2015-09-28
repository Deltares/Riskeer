// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Design;

namespace SharpMap.Styles.Shapes
{
    public class ShapeTypeEditor : UITypeEditor
    {
        // Methods
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) & (provider != null))
            {
                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    Shape Instance;
                    DropdownShapeEditor dropDownEditor = new DropdownShapeEditor(editorService);
                    if (context.Instance.GetType() == typeof(Shape))
                    {
                        Instance = (Shape)context.Instance;
                    }
                    else
                    {
                        Shape shape = new Shape();
                        shape.ShapeType = (Shape.eShape) value;
                        Instance = shape;
                    }
                    dropDownEditor.TheShape.ShapeType = Instance.ShapeType;
                    dropDownEditor.TheShape.FillType = Instance.FillType;
                    dropDownEditor.TheShape.FillTypeLinear = Instance.FillTypeLinear;
                    dropDownEditor.TheShape.ColorFillSolid = Instance.ColorFillSolid;
                    dropDownEditor.TheShape.BorderWidth = Instance.BorderWidth;
                    dropDownEditor.TheShape.BorderColor = Instance.BorderColor;
                    dropDownEditor.TheShape.BorderStyle = Instance.BorderStyle;
                    dropDownEditor.TheShape.RadiusInner = Instance.RadiusInner;
                    dropDownEditor.TheShape.FocalPoints = new cFocalPoints(Instance.FocalPoints.CenterPoint, Instance.FocalPoints.FocusScales);
                    dropDownEditor.TheShape.ColorFillBlend = Instance.ColorFillBlend;
                    dropDownEditor.TheShape.Corners = Instance.Corners;
                    editorService.DropDownControl(dropDownEditor);
                    return dropDownEditor.TheShape.ShapeType;
                }
            }
            return base.EditValue(context, provider, RuntimeHelpers.GetObjectValue(value));
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                return UITypeEditorEditStyle.DropDown;
            }
            return base.GetEditStyle(context);
        }
    }
}