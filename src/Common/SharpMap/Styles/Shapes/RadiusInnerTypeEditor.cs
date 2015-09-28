// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Design;

namespace SharpMap.Styles.Shapes
{
    public class RadiusInnerTypeEditor : UITypeEditor
    {
        // Methods
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) & (provider != null))
            {
                IWindowsFormsEditorService editorService = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
                if (editorService != null)
                {
                    Shape Instance;
                    float ratio;
                    DropdownRadiusInner dropDownEditor = new DropdownRadiusInner(editorService);
                    if (context.Instance.GetType() == typeof(Shape))
                    {
                        Instance = (Shape) context.Instance;
                    }
                    else
                    {
                        throw new NotImplementedException();
                        //Instance = (Shape) NewLateBinding.LateGet(context.Instance, null, "CurrShape", new object[0], null, null, null);
                    }
                    DropdownRadiusInner L0 = dropDownEditor;
                    if (Instance.Width > Instance.Height)
                    {
                        L0.TheShape.Height = (int) Math.Round((double) (L0.TheShape.Width * (((double) Instance.Height) / ((double) Instance.Width))));
                        L0.TheShape.Top = (int) Math.Round((double) (((double) (L0.panShapeHolder.Height - L0.TheShape.Height)) / 2.0));
                        ratio = (float) (((double) L0.TheShape.Height) / ((double) Instance.Height));
                    }
                    else
                    {
                        L0.TheShape.Width = (int) Math.Round((double) (L0.TheShape.Height * (((double) Instance.Width) / ((double) Instance.Height))));
                        L0.TheShape.Left = (int) Math.Round((double) (((double) (L0.panShapeHolder.Width - L0.TheShape.Width)) / 2.0));
                        ratio = (float) (((double) L0.TheShape.Width) / ((double) Instance.Width));
                    }
                    L0.lblValue.Text = Convert.ToString(Convert.ToSingle(value));
                    L0.TheShape.RadiusInner = Convert.ToSingle(value);
                    L0.tbarRadiusInner.Value = (int) Math.Round((double) (Convert.ToSingle(value) * 100f));
                    L0.TheShape.ShapeType = Instance.ShapeType;
                    L0.TheShape.FillType = Instance.FillType;
                    L0.TheShape.FillTypeLinear = Instance.FillTypeLinear;
                    L0.TheShape.ColorFillSolid = Instance.ColorFillSolid;
                    L0.TheShape.BorderWidth = Instance.BorderWidth;
                    L0.TheShape.BorderColor = Instance.BorderColor;
                    L0.TheShape.BorderStyle = Instance.BorderStyle;
                    L0.TheShape.FocalPoints = new cFocalPoints(Instance.FocalPoints.CenterPoint, Instance.FocalPoints.FocusScales);
                    L0.TheShape.ColorFillBlend = Instance.ColorFillBlend;
                    L0.TheShape.Corners = new CornersProperty((short) Math.Round((double) (Instance.Corners.LowerLeft * ratio)), (short) Math.Round((double) (Instance.Corners.LowerRight * ratio)), (short) Math.Round((double) (Instance.Corners.UpperLeft * ratio)), (short) Math.Round((double) (Instance.Corners.UpperRight * ratio)));
                    L0 = null;
                    editorService.DropDownControl(dropDownEditor);
                    return dropDownEditor.TheShape.RadiusInner;
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