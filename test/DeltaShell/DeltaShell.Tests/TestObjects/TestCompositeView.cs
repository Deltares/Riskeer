using System;
using System.Drawing;
using DelftTools.Controls;
using DelftTools.Utils.Collections.Generic;

namespace DeltaShell.Tests.TestObjects
{
    public class TestCompositeView: ICompositeView
    {
        private readonly IEventedList<IView> childViews = new EventedList<IView>();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public object Data { get; set; }
        public string Text { get; set; }
        public Image Image { get; set; }
        public void EnsureVisible(object item) { }
        public bool Visible { get; private set; }
        public ViewInfo ViewInfo { get; set; }

        public IEventedList<IView> ChildViews
        {
            get { return childViews; }
        }

        public bool HandlesChildViews { get { return true; } }
        
        public void ActivateChildView(IView childView)
        {
            
        }

        public void SetChildViews(params TestView[] views)
        {
            childViews.AddRange(views);
        }
    }
}