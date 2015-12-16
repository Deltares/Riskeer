using Core.Common.Utils.Collections.Generic;

namespace Core.Common.Controls.Views
{
    public interface ICompositeView : IView
    {
        IEventedList<IView> ChildViews { get; }

        bool HandlesChildViews { get; }

        void ActivateChildView(IView childView);
    }
}