using System;
using System.Drawing;

namespace Core.Common.Controls
{
    public class ViewInfo : ICloneable
    {
        /// <summary>
        /// Type of the data for this viewInfo
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Type of the data of the view
        /// </summary>
        public Type ViewDataType { get; set; }

        /// <summary>
        /// Type of the view
        /// </summary>
        public Type ViewType { get; set; }

        /// <summary>
        /// Type of the composite view to which this view belongs
        /// </summary>
        public Type CompositeViewType { get; set; }

        /// <summary>
        /// Description of the view (shown to the user when there is more then one view for an item) 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Name the view should have
        /// <list type="number">
        ///     <item>The view to get a name for</item>
        ///     <item>The data of the view</item>
        ///     <item>out - the view name</item>
        /// </list>
        /// </summary>
        public Func<IView, object, string> GetViewName { get; set; }

        /// <summary>
        /// Icon of the view (shown top left)
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Additional data checking for matching the ViewInfo
        /// <list type="number">
        ///     <item>Data as provided by the ViewProvider</item>
        ///     <item>out - Check succeeded</item>
        /// </list>
        /// </summary>
        public Func<object, bool> AdditionalDataCheck { get; set; }

        /// <summary>
        /// Function that returns the data for the view (when not set it returns T in <see cref="System.Func{T,TResult}"/>)
        /// <list type="number">
        ///     <item>object - Original data for the view</item>
        ///     <item>out object - data for the view</item>
        /// </list>
        /// </summary>
        public Func<object, object> GetViewData { get; set; }

        /// <summary>
        /// Extra actions that can be performed on the view after creation
        /// <list type="number">
        ///     <item>View to modify</item>
        ///     <item>Data for this viewinfo</item>
        /// </list>
        /// </summary>
        public Action<IView, object> AfterCreate { get; set; }

        /// <summary>
        /// Extra actions that can be performed on the view after the focus has been set on the view.
        /// (Will be called after creation and when the user tries to open a view for data while there is an existing view
        /// (and only the focus will be set to the existing view))
        /// <list type="number">
        ///     <item>View to modify</item>
        ///     <item>Data for this viewinfo</item>
        /// </list>
        /// </summary>
        public Action<IView, object> OnActivateView { get; set; }

        /// <summary>
        /// Gets the data for the composite view of which this is the child view info
        /// </summary>
        public Func<object, object> GetCompositeViewData { get; set; }

        /// <summary>
        /// Override the default closing of the view constructed with this info
        /// <list type="number">
        ///     <item>View to close</item>
        ///     <item></item>
        ///     <item>out - Close succeeded</item>
        /// </list>
        /// </summary>
        public Func<IView, object, bool> CloseForData { get; set; }

        public override string ToString()
        {
            return DataType + " : " + ViewDataType + " : " + ViewType;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class ViewInfo<TData, TViewData, TView> where TView : IView
    {
        public Type DataType
        {
            get
            {
                return typeof(TData);
            }
        }

        public Type ViewDataType
        {
            get
            {
                return typeof(TViewData);
            }
        }

        public Type ViewType
        {
            get
            {
                return typeof(TView);
            }
        }

        public Type CompositeViewType { get; set; }

        public string Description { get; set; }

        public Func<TView, TViewData, string> GetViewName { get; set; }

        public Image Image { get; set; }

        public Func<TData, bool> AdditionalDataCheck { get; set; }

        public Func<TData, TViewData> GetViewData { get; set; }

        public Action<TView, TData> AfterCreate { get; set; }

        public Action<TView, object> OnActivateView { get; set; }

        public Func<TData, object> GetCompositeViewData { get; set; }

        public Func<TView, object, bool> CloseForData { get; set; }

        public static implicit operator ViewInfo(ViewInfo<TData, TViewData, TView> viewInfo)
        {
            return new ViewInfo
            {
                DataType = viewInfo.DataType,
                ViewDataType = viewInfo.ViewDataType,
                ViewType = viewInfo.ViewType,
                CompositeViewType = viewInfo.CompositeViewType,
                Description = viewInfo.Description,
                Image = viewInfo.Image,
                AdditionalDataCheck = o => viewInfo.AdditionalDataCheck == null || viewInfo.AdditionalDataCheck((TData) o),
                GetViewData = o => viewInfo.GetViewData != null ? viewInfo.GetViewData((TData) o) : o,
                GetCompositeViewData = o => viewInfo.GetCompositeViewData != null ? viewInfo.GetCompositeViewData((TData) o) : null,
                CloseForData = (v, o) => viewInfo.CloseForData != null && viewInfo.CloseForData((TView) v, o),
                AfterCreate = (v, o) =>
                {
                    if (viewInfo.AfterCreate != null)
                    {
                        viewInfo.AfterCreate((TView) v, (TData) o);
                    }
                },
                OnActivateView = (v, o) =>
                {
                    if (viewInfo.OnActivateView != null)
                    {
                        viewInfo.OnActivateView((TView) v, o);
                    }
                },
                GetViewName = (v, o) => viewInfo.GetViewName != null ? viewInfo.GetViewName((TView) v, (TViewData) o) : null
            };
        }

        public override string ToString()
        {
            return DataType + " : " + ViewDataType + " : " + ViewType;
        }
    }

    public class ViewInfo<TData, TView> : ViewInfo<TData, TData, TView> where TView : IView {}
}