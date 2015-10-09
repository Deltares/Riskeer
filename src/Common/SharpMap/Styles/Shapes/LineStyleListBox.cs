// License: The Code Project Open License (CPOL) 1.02
// Original author is: http://www.codeproject.com/KB/buttons/CButton.aspx?msg=3060090

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SharpMap.Styles.Shapes
{
    [ToolboxItem(false)]
    public class LineStyleListBox : ListBox
    {
        // Fields
        private readonly IWindowsFormsEditorService m_EditorService;

        // Methods
        public LineStyleListBox(DashStyle line_style, IWindowsFormsEditorService editor_service)
        {
            DrawItem += new DrawItemEventHandler(LineStyleListBox_DrawItem);
            Click += new EventHandler(LineStyleListBox_Click);
            m_EditorService = editor_service;
            int i = 0;
            do
            {
                Items.Add(i);
                i++;
            } while (i <= 4);
            SelectedIndex = (int) line_style;
            DrawMode = DrawMode.OwnerDrawFixed;
            ItemHeight = 0x12;
        }

        private void LineStyleListBox_Click(object sender, EventArgs e)
        {
            if (m_EditorService != null)
            {
                m_EditorService.CloseDropDown();
            }
        }

        private void LineStyleListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            LineStyleEditorStuff.DrawSamplePen(e.Graphics, e.Bounds, Color.Black, (DashStyle) e.Index);
        }
    }
}