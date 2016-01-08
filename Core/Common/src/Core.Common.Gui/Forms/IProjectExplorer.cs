﻿using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;

namespace Core.Common.Gui.Forms
{
    public interface IProjectExplorer : IView
    {
        TreeView TreeView { get; }

        void ScrollTo(object o);
    }
}