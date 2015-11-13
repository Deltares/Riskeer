using Core.Common.Controls.Swf.Properties;
using DevExpress.XtraGrid.Localization;

namespace Core.Common.Controls.Swf.Table
{
    public class TableViewExceptionMessageController : GridLocalizer
    {
        public override string GetLocalizedString(GridStringId id)
        {
            return id == GridStringId.ColumnViewExceptionMessage
                       ? Resources.TableViewExceptionMessageController_GetLocalizedString_
                       : base.GetLocalizedString(id);
        }
    }
}