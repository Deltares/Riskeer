﻿using Core.Common.Controls.TreeView;
using Core.Common.Forms.Views;

namespace Core.Common.Gui.Forms
{
    public interface IProjectExplorer : IView
    {
        ITreeView TreeView { get; }

        void ScrollTo(object o);
    }
}