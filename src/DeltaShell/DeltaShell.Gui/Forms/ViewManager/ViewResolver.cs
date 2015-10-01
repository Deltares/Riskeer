using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Utils.Reflection;
using DeltaShell.Gui.Properties;
using log4net;

namespace DeltaShell.Gui.Forms.ViewManager
{
    public class ViewResolver : IViewResolver
    {
        private readonly IDictionary<Type, Type> defaultViewTypes = new Dictionary<Type, Type>();
        private static readonly ILog Log = LogManager.GetLogger(typeof(ViewResolver));

        private readonly ViewList viewList;
        private readonly IList<ViewInfo> viewInfos;

        public ViewResolver(ViewList viewList, IEnumerable<ViewInfo> viewInfos)
        {
            this.viewList = viewList;
            this.viewInfos = viewInfos.ToList();
        }
        
        public IDictionary<Type, Type> DefaultViewTypes
        {
            get { return defaultViewTypes; }
        }

        public IList<ViewInfo> ViewInfos
        {
            get { return viewInfos; }
        }

        /// <summary>
        /// Checks consistency of ViewList / ViewResolver logic. Sometimes views are closed while being opened.
        /// </summary>
        internal static bool IsViewOpening { get; private set; }

        public bool OpenViewForData(object data, Type viewType = null, bool alwaysShowDialog = false)
        {
            try
            {
                IsViewOpening = true;

                if (data == null) return false;

                var viewInfoList = FilterOnInheritance(GetViewInfosFor(data, viewType)).ToList();

                if (viewInfoList.Count == 0)
                {
                    Log.DebugFormat(Resources.ViewResolver_OpenViewForData_No_view_registered_for____0_, data);
                    return false;
                }

                // Filter on composite view infos
                viewInfoList = FilterCompositeViewsForChildViews(viewInfoList).ToList();

                if (!alwaysShowDialog)
                {
                    if (viewInfoList.Count == 1)
                    {
                        return CreateViewFromViewInfo(data, viewInfoList[0]);
                    }

                    var principalViewInfoList =
                        viewInfoList.Where(vi => !vi.ViewType.Implements<IAdditionalView>()).ToList();

                    if (principalViewInfoList.Count == 1)
                    {
                        return CreateViewFromViewInfo(data, principalViewInfoList[0]);
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

                if (viewInfoList.Count == 0) return false;

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

        public IList<IView> GetViewsForData(object data, bool includeChildViews = false)
        {
            return GetViewsForData(viewList.Where(v => !(v is IAdditionalView)), data, includeChildViews).ToList();
        }

        public void CloseAllViewsFor(object data)
        {
            DoWithMatchingViews(data, viewList,
                v => viewList.Remove(v),
                (compV, view) => compV.ChildViews.Remove(view),
                (v, o) => v.ViewInfo != null && v.ViewInfo.CloseForData(v, o));
        }

        private void DoWithMatchingViews(object data, IEnumerable<IView> views, Action<IView> viewAction, Action<ICompositeView, IView> compositeViewAction, Func<IView,object,bool> extraCheck = null)
        {
            var viewsToCheck = views.ToList();
            foreach (var view in viewsToCheck)
            {
                if (IsViewData(view, data) && (extraCheck == null || extraCheck(view,data)))
                {
                    viewAction(view);
                }

                var compositeView = view as ICompositeView;
                if (compositeView == null || compositeView.HandlesChildViews) continue;

                DoWithMatchingViews(data, compositeView.ChildViews, v => compositeViewAction(compositeView, v), compositeViewAction);
            }
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

        private IEnumerable<IView> GetViewsForData(IEnumerable<IView> viewsToCheck, object data, bool includeChildViews = true)
        {
            return data != null
                       ? GetOpenViewsFor(viewsToCheck, data, includeChildViews, (v, vi) =>
                                                                                !(v is IReusableView && !((IReusableView) v).Locked &&
                                                                                  data.GetType().Implements(vi.DataType)))
                       : Enumerable.Empty<IView>();
        }

        private static IEnumerable<ViewInfo> FilterOnInheritance(IEnumerable<ViewInfo> compatibleStandaloneViewInfos)
        {
            var viewInfos = compatibleStandaloneViewInfos.ToList();

            // filter on inheritance
            var dataTypes = viewInfos.Select(i => i.DataType).ToList();
            return viewInfos.Where(i => i.DataType == typeof(IProjectItem) || !dataTypes.Any(t => t != i.DataType && t.Implements(i.DataType)));
        }

        private static IEnumerable<ViewInfo> FilterCompositeViewsForChildViews(IList<ViewInfo> viewInfoList)
        {
            var compositeViewTypes = viewInfoList.Select(vi => vi.CompositeViewType).Where(t => t != null).ToList();
            return viewInfoList.Where(vi => !compositeViewTypes.Contains(vi.ViewType));
        }

        private bool CreateViewFromViewInfo(object data, ViewInfo viewInfo)
        {
            var viewData = viewInfo.GetViewData(data);
            var view = (viewInfo.DataType == viewInfo.ViewDataType
                            ? GetOpenViewsFor(viewList, data)
                            : GetOpenViewsFor(viewList, data).Concat(GetOpenViewsFor(viewList, viewData)))
                            .FirstOrDefault(v => v.GetType() == viewInfo.ViewType);

            if (viewInfo.CompositeViewType == null)
            {
                if (view != null)
                {
                    viewInfo.OnActivateView(view, data);
                    viewList.ActiveView = view;
                    return true;
                }

                var reusableView = FindViewsRecursive<IReusableView>(viewList).FirstOrDefault(rv => 
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

            var compositeView = GetCompositeView(data, viewInfo);
            if (compositeView == null) return false;

            viewList.ActiveView = compositeView;

            var childView = GetChildView(data, viewInfo, view, compositeView);
            if (childView == null) return false;

            compositeView.ActivateChildView(childView);
            return true;
        }

        private IView GetChildView(object data, ViewInfo viewInfo, IView view, ICompositeView compositeView)
        {
            var childView = view ?? compositeView.ChildViews.FirstOrDefault(v => v.Data == data);

            if (childView == null && !compositeView.HandlesChildViews)
            {
                // Add child view to composite view
                childView = CreateViewForData(data, viewInfo);
                compositeView.ChildViews.Add(childView);
                if (!compositeView.HandlesChildViews && viewList.UpdateViewNameAction != null)
                {
                    viewList.UpdateViewNameAction(childView);
                }
            }
            else
            {
                viewInfo.OnActivateView(childView, data);
            }
            return childView;
        }

        private ICompositeView GetCompositeView(object data, ViewInfo viewInfo)
        {
            var compositeViewInfo = ViewInfos.FirstOrDefault(vi => vi.ViewType == viewInfo.CompositeViewType);
            if (compositeViewInfo == null) return null;

            var compositeViewData = compositeViewInfo.GetViewData(viewInfo.GetCompositeViewData(data));
            if (compositeViewData == null) return null;

            var compositeView = ((IEnumerable<ICompositeView>) TypeUtils.CallGenericMethod(GetType(), "FindViewsRecursive", viewInfo.CompositeViewType, this, viewList))
                .FirstOrDefault(v => v.Data.Equals(compositeViewData) && (!v.HandlesChildViews || v.ChildViews.Any(cv => cv.Data == data)));

            if (compositeView == null)
            {
                compositeView = (ICompositeView) CreateViewForData(compositeViewData, compositeViewInfo);
                viewList.Add(compositeView);
            }

            compositeViewInfo.OnActivateView(compositeView, data);
            
            return compositeView;
        }

        private static IView CreateViewForData(object data, ViewInfo viewInfo)
        {
            var view = (IView)Activator.CreateInstance(viewInfo.ViewType);

            view.Data = viewInfo.GetViewData(data);
            view.Image = viewInfo.Image;
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
            var viewSelector = new SelectViewDialog
            {
                DefaultViewName = defaultViewName,
                Items = viewTypeDictionary.Keys.ToList()
            };

            // TODO : get MainWindow for ShowDialog
            if (viewSelector.ShowDialog(/*gui.MainWindow as Form*/) != DialogResult.OK) return null;
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

        private IEnumerable<IView> GetOpenViewsFor(IEnumerable<IView> viewsToCheck, object data, bool includeChildViews = true, Func<IView, ViewInfo, bool> extraCheck = null)
        {
            if(data == null) yield break;

            foreach (var view in viewsToCheck)
            {
                var viewInfo = GetViewInfoForView(data, view);

                if (IsViewData(view, data) && (extraCheck == null || extraCheck(view, viewInfo)))
                {
                    yield return view;
                }

                if (!includeChildViews) continue;

                var compositeView = view as ICompositeView;
                if (compositeView == null || compositeView.HandlesChildViews) continue;

                var childViews = GetOpenViewsFor(compositeView.ChildViews, data).ToList();
                if (!childViews.Any()) continue;

                foreach (var childView in childViews)
                {
                    yield return childView;
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
            if (data == null) return false;

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

        private IEnumerable<T> FindViewsRecursive<T>(IEnumerable<IView> views)
        {
            foreach (var view in views)
            {
                if (view is T)
                {
                    yield return (T)view;
                }

                var compositeView = view as ICompositeView;
                if (compositeView == null) continue;

                foreach (var childView in FindViewsRecursive<T>(compositeView.ChildViews))
                {
                    yield return childView;
                }
            }
        }
    }
}