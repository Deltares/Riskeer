using System;

namespace DelftTools.Controls
{
    /// <summary>
    /// This helper can be used to reuse another ViewInfo object, when its data is (sometimes) wrapped by 
    /// another object. By supplying an unwrap method, the resulting view info is deduced.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public static class ViewInfoWrapper<TData>
    {
        public static ViewInfo Create<TTargetData, TViewData, TView>(ViewInfo<TTargetData, TViewData, TView> originalViewInfo,
                                                        Func<TData, object> getTargetData, 
                                                        Func<TData, bool> additionalDataCheck = null, 
                                                        Action<TView, TData> afterCreate = null) 
                                                        where TView : IView
        {
            if (getTargetData == null)
                throw new ArgumentNullException("getTargetData", "You must supply a getTargetData method");

            additionalDataCheck = additionalDataCheck ?? (d => true);

            return new ViewInfo
            {
                AdditionalDataCheck =
                    (d) =>
                    {
                        if (!additionalDataCheck((TData) d)) return false;
                        var targetData = getTargetData((TData) d);
                        if (!(targetData is TTargetData)) return false;
                        return (originalViewInfo.AdditionalDataCheck == null ||
                                originalViewInfo.AdditionalDataCheck((TTargetData) targetData));
                    },
                AfterCreate = (v, d) =>
                {
                    if (originalViewInfo.AfterCreate != null)
                        originalViewInfo.AfterCreate((TView) v, (TTargetData) getTargetData((TData) d));
                    if (afterCreate != null)
                        afterCreate((TView) v, (TData) d);
                },
                GetViewName =
                    (v, o) =>
                        originalViewInfo.GetViewName != null
                            ? originalViewInfo.GetViewName((TView) v, (TViewData) o)
                            : null,
                CloseForData =
                    (v, o) =>
                        originalViewInfo.CloseForData != null &&
                        originalViewInfo.CloseForData((TView) v,
                            o is TTargetData ? (TTargetData) o : (TTargetData) getTargetData((TData)o)),
                CompositeViewType = originalViewInfo.CompositeViewType,
                DataType = typeof (TData),
                ViewDataType = typeof (TViewData),
                GetCompositeViewData = wrappedData =>
                {
                    if (originalViewInfo.GetCompositeViewData != null)
                        return originalViewInfo.GetCompositeViewData((TTargetData) getTargetData((TData) wrappedData));
                    return null;
                },
                GetViewData = d =>
                {
                    var targetData = (TTargetData) getTargetData((TData) d);
                    if (originalViewInfo.GetViewData == null)
                        return targetData;
                    return originalViewInfo.GetViewData(targetData);
                },
                OnActivateView = (v, d) =>
                {
                    if (originalViewInfo.OnActivateView != null)
                    {
                        var targetData = (TTargetData) getTargetData((TData) d);
                        originalViewInfo.OnActivateView((TView) v, targetData);
                    }
                },
                Description = originalViewInfo.Description,
                Image = originalViewInfo.Image,
                ViewType = originalViewInfo.ViewType
            };
        }
    }
}