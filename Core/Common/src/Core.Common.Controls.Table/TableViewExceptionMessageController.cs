using Core.Common.Controls.Table.Properties;
using DevExpress.XtraGrid.Localization;

namespace Core.Common.Controls.Table
{
    public class TableViewExceptionMessageController : GridLocalizer
    {
        public override string GetLocalizedString(GridStringId id)
        {
            return id == GridStringId.ColumnViewExceptionMessage
                       ? Resources.TableViewExceptionMessageController_GetLocalizedString
                       : base.GetLocalizedString(id);
        }
    }
}