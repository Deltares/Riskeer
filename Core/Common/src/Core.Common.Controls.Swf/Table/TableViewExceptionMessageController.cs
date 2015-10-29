using DevExpress.XtraGrid.Localization;

namespace Core.Common.Controls.Swf.Table
{
    internal class TableViewExceptionMessageController : GridLocalizer
    {
        public override string GetLocalizedString(GridStringId id)
        {
            return id == GridStringId.ColumnViewExceptionMessage
                       ? " Do you want to correct the value?\n\n" +
                         "Choose Yes to correct the value yourself. Choose No to revert to the original value."
                       : base.GetLocalizedString(id);
        }
    }
}