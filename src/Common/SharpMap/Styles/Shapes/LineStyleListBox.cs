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
        private IWindowsFormsEditorService m_EditorService;

        // Methods
        public LineStyleListBox(DashStyle line_style, IWindowsFormsEditorService editor_service)
        {
            base.DrawItem += new DrawItemEventHandler(this.LineStyleListBox_DrawItem);
            base.Click += new EventHandler(this.LineStyleListBox_Click);
            this.m_EditorService = editor_service;
            int i = 0;
            do
            {
                this.Items.Add(i);
                i++;
            }
            while (i <= 4);
            this.SelectedIndex = (int)line_style;
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = 0x12;
        }

        private void LineStyleListBox_Click(object sender, EventArgs e)
        {
            if (this.m_EditorService != null)
            {
                this.m_EditorService.CloseDropDown();
            }
        }

        private void LineStyleListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            LineStyleEditorStuff.DrawSamplePen(e.Graphics, e.Bounds, Color.Black, (DashStyle)e.Index);
        }
    }
}