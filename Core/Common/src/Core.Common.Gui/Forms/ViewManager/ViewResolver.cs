using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Views;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Reflection;
using log4net;

namespace Core.Common.Gui.Forms.ViewManager
{
    public class ViewResolver : IViewResolver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ViewResolver));
        private readonly IDictionary<Type, Type> defaultViewTypes = new Dictionary<Type, Type>();

        private readonly ViewList viewList;
        private readonly IList<ViewInfo> viewInfos;
        private readonly IWin32Window owner;

        public ViewResolver(ViewList viewList, IEnumerable<ViewInfo> viewInfos, IWin32Window owner)
        {
            this.viewList = viewList;
            this.viewInfos = viewInfos.ToList();
            this.owner = owner;
        }

        public IDictionary<Type, Type> DefaultViewTypes
        {
            get
            {
                return defaultViewTypes;
            }
        }

        public IList<ViewInfo> ViewInfos
        {
            get
            {
                return viewInfos;
            }
        }

        public bool OpenViewForData(object data, Type viewType = null, bool alwaysShowDialog = false)
        {
            try
            {
                IsViewOpening = true;

                if (data == null)
                {
                    return false;
                }

                var viewInfoList = FilterOnInheritance(GetViewInfosFor(data, viewType)).ToList();

                if (viewInfoList.Count == 0)
                {
                    Log.DebugFormat(Resources.ViewResolver_OpenViewForData_No_view_registered_for_0_, data);
                    return false;
                }

                if (!alwaysShowDialog)
                {
                    if (viewInfoList.Count == 1)
                    {
                        return CreateViewFromViewInfo(data, viewInfoList[0]);
                    }

                    // Create default view
                    var defaultType = GetDefaultViewType(data);
                    var defaultViewInfo = viewInfoList.FirstOrDefault(vi => vi.ViewType == defaultType);

                    if (defaultViewInfo != null)
                    {
                        if (CreateViewFromViewInfo(data, defaultViewInfo))
                        {
                            return true;
                        }

                        viewInfoList.Remove(defaultViewInfo);
                    }
                }

                if (viewInfoList.Count == 0)
                {
                    return false;
                }

                // Create chosen view
                var chosenViewInfo = GetViewInfoUsingDialog(data, viewInfoList);

                return chosenViewInfo != null && CreateViewFromViewInfo(data, chosenViewInfo);
            }
            finally
            {
                IsViewOpening = false;
            }
        }

        public IView CreateViewForData(object data, Func<ViewInfo, bool> selectViewInfo = null)
        {
            var viewInfoList = ((selectViewInfo == null)
                                    ? GetViewInfosFor(data)
                                    : GetViewInfosFor(data).Where(selectViewInfo))
                .ToList();

            if (viewInfoList.Count == 0)
            {
                return null;
            }

            if (viewInfoList.Count > 1)
            {
                throw new Exception(Resources.ViewResolver_CreateViewForData_More_than_one_view_for_data);
            }

            return CreateViewForData(data, viewInfoList[0]);
        }

        public bool CanOpenViewFor(object data, Type viewType = null)
        {
            return data != null && GetViewInfosFor(data, viewType).Any();
        }

        public IList<IView> GetViewsForData(object data)
        {
            return GetViewsForData(viewList, data).ToList();
        }

        public void CloseAllViewsFor(object data)
        {
            DoWithMatchingViews(data, viewList,
                                v => viewList.Remove(v),
                                (v, o) => v.ViewInfo != null && v.ViewInfo.CloseForData(v, o));
        }

        public Type GetDefaultViewType(object dataObject)
        {
            if (dataObject == null)
            {
                return null;
            }

            var selectionType = dataObject.GetType();

            return defaultViewTypes.Keys.Contains(selectionType)
                       ? defaultViewTypes[selectionType]
                       : null;
        }

        public IEnumerable<ViewInfo> GetViewInfosFor(object data, Type viewType = null)
        {
            var infos = ViewInfos.Where(vi => data.GetType().Implements(vi.DataType) && vi.AdditionalDataCheck(data));

            return viewType != null
                       ? infos.Where(vi => viewType.IsAssignableFrom(vi.ViewType))
                       : infos;
        }

        /// <summary>
        /// Checks consistency of ViewList / ViewResolver logic. Sometimes views are closed while being opened.
        /// </summary>
        internal static bool IsViewOpening { get; private set; }

        private void DoWithMatchingViews(object data, IEnumerable<IView> views, Action<IView> viewAction, Func<IView, object, bool> extraCheck = null)
        {
            var viewsToCheck = views.ToList();
            foreach (var view in viewsToCheck)
            {
                if (IsViewData(view, data) || (extraCheck != null && extraCheck(view, data)))
                {
                    viewAction(view);
                }
            }
        }

        private IEnumerable<IView> GetViewsForData(IEnumerable<IView> viewsToCheck, object data)
        {
            return data != null
                       ? GetOpenViewsFor(viewsToCheck, data, (v, vi) =>
                                                             !(v is IReusableView && !((IReusableView) v).Locked &&
                                                               data.GetType().Implements(vi.DataType)))
                       : Enumerable.Empty<IView>();
        }

        private static IEnumerable<ViewInfo> FilterOnInheritance(IEnumerable<ViewInfo> compatibleStandaloneViewInfos)
        {
            var viewInfos = compatibleStandaloneViewInfos.ToList();

            // filter on inheritance
            var dataTypes = viewInfos.Select(i => i.DataType).ToList();
            return viewInfos.Where(i => !dataTypes.Any(t => t != i.DataType && t.Implements(i.DataType)));
        }

        private bool CreateViewFromViewInfo(object data, ViewInfo viewInfo)
        {
            var viewData = viewInfo.GetViewData(data);
            var view = (viewInfo.DataType == viewInfo.ViewDataType
                            ? GetOpenViewsFor(viewList, data)
                            : GetOpenViewsFor(viewList, data).Concat(GetOpenViewsFor(viewList, viewData)))
                .FirstOrDefault(v => v.GetType() == viewInfo.ViewType);

            if (view != null)
            {
                viewInfo.OnActivateView(view, data);
                viewList.ActiveView = view;
                return true;
            }

            var reusableView = viewList.OfType<IReusableView>()
                                       .FirstOrDefault(rv =>
                                                       !rv.Locked &&
                                                       IsDataForView(rv, viewData) &&
                                                       viewInfo.ViewDataType == rv.ViewInfo.ViewDataType);

            if (reusableView != null)
            {
                // Set reusable view data
                reusableView.Data = viewData;
                viewInfo.AfterCreate(reusableView, data);
                viewInfo.OnActivateView(reusableView, data);
                viewList.ActiveView = reusableView;

                if (viewList.UpdateViewNameAction != null)
                {
                    viewList.UpdateViewNameAction(reusableView);
                }
                return true;
            }

            viewList.Add(CreateViewForData(data, viewInfo));
            return true;
        }

        private static IView CreateViewForData(object data, ViewInfo viewInfo)
        {
            var view = (IView) Activator.CreateInstance(viewInfo.ViewType);

            view.Data = viewInfo.GetViewData(data);
            view.ViewInfo = viewInfo;

            viewInfo.AfterCreate(view, data);
            view.Text = viewInfo.GetViewName(view, view.Data);

            viewInfo.OnActivateView(view, data);

            return view;
        }

        private ViewInfo GetViewInfoUsingDialog(object data, IList<ViewInfo> viewInfoList)
        {
            var defaultViewTypeForData = GetDefaultViewTypeForData(data);
            var defaultViewName = (defaultViewTypeForData != null)
                                      ? viewInfoList.First(vi => vi.ViewType == defaultViewTypeForData).Description
                                      : null;

            var viewTypeDictionary = viewInfoList.ToDictionary(vi => vi.Description ?? vi.ViewType.Name);
            var viewSelector = new SelectViewDialog(owner)
            {
                DefaultViewName = defaultViewName,
                Items = viewTypeDictionary.Keys.ToList()
            };

            if (viewSelector.ShowDialog() != DialogResult.OK)
            {
                return null;
            }
            var selectedViewInfo = viewTypeDictionary[viewSelector.SelectedItem];

            if (viewSelector.DefaultViewName == null)
            {
                ClearDefaultView(data);
            }
            else
            {
                var defaultViewInfo = viewTypeDictionary[viewSelector.DefaultViewName];
                SetDefaultView(defaultViewInfo.ViewType, data);
            }

            return selectedViewInfo;
        }

        private IEnumerable<IView> GetOpenViewsFor(IEnumerable<IView> viewsToCheck, object data, Func<IView, ViewInfo, bool> extraCheck = null)
        {
            if (data == null)
            {
                yield break;
            }

            foreach (var view in viewsToCheck)
            {
                var viewInfo = GetViewInfoForView(data, view);

                if (IsViewData(view, data) && (extraCheck == null || extraCheck(view, viewInfo)))
                {
                    yield return view;
                }
            }
        }

        private bool IsViewData(IView view, object data)
        {
            var viewInfo = GetViewInfoForView(data, view);
            return data.Equals(view.Data) || (IsDataForView(view, data) && Equals(viewInfo.GetViewData(data), view.Data));
        }

        private bool IsDataForView(IView view, object data)
        {
            if (data == null)
            {
                return false;
            }

            var viewInfo = GetViewInfoForView(data, view);
            return viewInfo != null && data.GetType().Implements(viewInfo.DataType) && viewInfo.AdditionalDataCheck(data);
        }

        private ViewInfo GetViewInfoForView(object data, IView view)
        {
            return view.ViewInfo ?? (view.ViewInfo = GetViewInfosFor(data, view.GetType()).FirstOrDefault());
        }

        private void ClearDefaultView(object data)
        {
            var selectedItemType = data.GetType();

            if (defaultViewTypes.Keys.Contains(selectedItemType))
            {
                defaultViewTypes.Remove(selectedItemType);
            }
        }

        private void SetDefaultView(Type selectedViewType, object data)
        {
            var selectedItemType = data.GetType();

            if (defaultViewTypes.Keys.Contains(selectedItemType))
            {
                defaultViewTypes[selectedItemType] = selectedViewType;
            }
            else
            {
                defaultViewTypes.Add(selectedItemType, selectedViewType);
            }
        }

        private Type GetDefaultViewTypeForData(object dataObject)
        {
            var selectionType = dataObject.GetType();

            return defaultViewTypes.Keys.Contains(selectionType) ? defaultViewTypes[selectionType] : null;
        }
    }
}