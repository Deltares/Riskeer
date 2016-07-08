using System;
using System.Windows.Forms;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// This class makes it easier to temporarily disable automatic resizing of a column,
    /// for example when its data is being changed or you are replacing the list items
    /// available in a combo-box for that column.
    /// 
    /// This resolves the "DataGridViewComboBoxCell value is not valid" error when updating the <see cref="DataGridViewComboBoxColumn"/>.
    /// That error turns out not to refer to the content of the value, 
    /// but to the string representation of the value not fitting inside the cell current width.
    /// </summary>
    public class SuspendDataGridViewColumnResizes : IDisposable
    {
        private readonly DataGridViewColumn column;
        private readonly DataGridViewAutoSizeColumnMode originalValue;

        public SuspendDataGridViewColumnResizes(DataGridViewColumn columnToSuspend)
        {
            column = columnToSuspend;
            originalValue = columnToSuspend.AutoSizeMode;
            columnToSuspend.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }

        public void Dispose()
        {
            column.AutoSizeMode = originalValue;
        }
    }
}