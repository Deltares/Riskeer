using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    public enum ColorSet
    {
        SystemColors,
        WebColors
    }

    public class ColorPicker : ComboBox
    {
        public ColorPicker()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            
            SetColorSet();
            SelectedIndex = 0;
            ItemHeight = 16;
        }

        private ColorSet colorSet;

        public ColorSet ColorSet
        {
            get { return colorSet; }
            set
            {
                colorSet = value;
                SetColorSet();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ObjectCollection Items
        {
            get { return base.Items; } 
        }

        private void SetColorSet()
        {
            Items.Clear();

            if (colorSet == ColorSet.SystemColors)
            {
                var systemColors = GetConstants(typeof (SystemColors)).OfType<object>().ToArray();
                Items.AddRange(systemColors);
            }

            if (colorSet == ColorSet.WebColors)
            {
                var webColors = GetConstants(typeof (Color)).OfType<object>().ToArray();
                Items.AddRange(webColors);
            }
        }
        


        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= Items.Count) return;
            
            var g = e.Graphics;
            e.DrawBackground();
            e.DrawFocusRectangle();

            try
            {
                var color = (Color)Items[e.Index];

                using (var colorBrush = new SolidBrush(color))
                {
                    g.FillRectangle(colorBrush, 2, e.Bounds.Y + 2, 16, ItemHeight - 4);
                }
                using (var blackBrush = new SolidBrush(Color.Black))
                {
                    g.DrawString(color.Name, Font, blackBrush, 20, e.Bounds.Y + 2);
                }
            }
            catch (Exception)
            {
            }
        }

        public static IEnumerable<Color> GetConstants(Type enumType)
        {
            var attributes = MethodAttributes.Static | MethodAttributes.Public;
            var propertyInfos = enumType.GetProperties()
                    .Where(p =>
                            p.PropertyType == typeof (Color) && p.GetGetMethod() != null &&
                            (p.GetGetMethod().Attributes & attributes) == attributes);

            return propertyInfos.Select(pi => pi.GetValue(null, null)).OfType<Color>().ToList();
        }
    }
}
