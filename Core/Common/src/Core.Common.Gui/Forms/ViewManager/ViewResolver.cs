// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Reflection;

using log4net;

namespace Core.Common.Gui.Forms.ViewManager
{
    /// <summary>
    /// Object responsible for finding a view given some data-object.
    /// </summary>
    public class ViewResolver : IViewResolver
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ViewResolver));

        private readonly IDictionary<Type, Type> defaultViewTypes = new Dictionary<Type, Type>();

        private readonly ViewList viewList;
        private readonly ViewInfo[] viewInfos;
        private readonly IWin32Window dialogParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewResolver"/> class.
        /// </summary>
        /// <param name="viewList">The view list.</param>
        /// <param name="viewInfos">The sequence of available view info objects.</param>
        /// <param name="dialogParent">The parent object for which dialogs should be shown on top.</param>
        public ViewResolver(ViewList viewList, IEnumerable<ViewInfo> viewInfos, IWin32Window dialogParent)
        {
            this.viewList = viewList;
            this.viewInfos = viewInfos.ToArray();
            this.dialogParent = dialogParent;
        }

        public IDictionary<Type, Type> DefaultViewTypes
        {
            get
            {
                return defaultViewTypes;
            }
        }

        public bool OpenViewForData(object data, bool alwaysShowDialog = false)
        {
            try
            {
                IsOpeningView = true;

                if (data == null)
                {
                    return false;
                }

                var viewInfoList = FilterOnInheritance(GetViewInfosFor(data)).ToArray();
                if (viewInfoList.Length == 0)
                {
                    log.DebugFormat(Resources.ViewResolver_OpenViewForData_No_view_registered_for_0_, data);
                    return false;
                }

                if (!alwaysShowDialog)
                {
                    if (viewInfoList.Length == 1)
                    {
                        CreateViewFromViewInfo(data, viewInfoList[0]);
                        return true;
                    }

                    // Create default view
                    var defaultType = GetDefaultViewType(data);
                    var defaultViewInfo = viewInfoList.FirstOrDefault(vi => vi.ViewType == defaultType);

                    if (defaultViewInfo != null)
                    {
                        CreateViewFromViewInfo(data, defaultViewInfo);
                        return true;
                    }
                }

                // Create chosen view
                var chosenViewInfo = GetViewInfoUsingDialog(data, viewInfoList);
                if (chosenViewInfo == null)
                {
                    return false;
                }
                else
                {
                    CreateViewFromViewInfo(data, chosenViewInfo);
                    return true;
                }
            }
            finally
            {
                IsOpeningView = false;
            }
        }

        public void CloseAllViewsFor(object data)
        {
            if (data == null)
            {
                return;
            }

            foreach (var view in viewList.ToArray())
            {
                if (ShouldRemoveViewForData(view, data))
                {
                    viewList.Remove(view);
                }
            }
        }

        public IEnumerable<ViewInfo> GetViewInfosFor(object data)
        {
            return viewInfos.Where(vi => data.GetType().Implements(vi.DataType) && vi.AdditionalDataCheck(data));
        }

        public string GetViewName(IView view)
        {
            var viewInfo = GetViewInfoForView(view);
            if (viewInfo != null)
            {
                return viewInfo.GetViewName(view, view.Data);
            }

            return "";
        }

        /// <summary>
        /// Indicates if the view resolver is opening a view. Can be used to ensure consistency 
        /// of <see cref="ViewList"/> / <see cref="ViewResolver"/> logic. Sometimes views 
        /// are closed while being opened.
        /// </summary>
        internal static bool IsOpeningView { get; private set; }

        private bool ShouldRemoveViewForData(IView view, object data)
        {
            if (IsViewData(view, data))
            {
                return true;
            }

            var viewInfo = GetViewInfoForView(view);
            return viewInfo != null && viewInfo.CloseForData(view, data);
        }

        private Type GetDefaultViewType(object dataObject)
        {
            var selectionType = dataObject.GetType();

            return defaultViewTypes.ContainsKey(selectionType)
                       ? defaultViewTypes[selectionType]
                       : null;
        }

        private static IEnumerable<ViewInfo> FilterOnInheritance(IEnumerable<ViewInfo> compatibleStandaloneViewInfos)
        {
            var viewInfos = compatibleStandaloneViewInfos.ToArray();

            // filter on inheritance
            var dataTypes = viewInfos.Select(i => i.DataType).ToArray();
            return viewInfos.Where(i => !dataTypes.Any(t => t != i.DataType && t.Implements(i.DataType)));
        }

        private void CreateViewFromViewInfo(object data, ViewInfo viewInfo)
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

                return;
            }

            var newView = CreateViewForData(data, viewInfo);

            viewList.Add(newView);
            viewList.SetImage(newView, viewInfo.Image);
        }

        private static IView CreateViewForData(object data, ViewInfo viewInfo)
        {
            var view = (IView)Activator.CreateInstance(viewInfo.ViewType);

            view.Data = viewInfo.GetViewData(data);

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
            using (var viewSelector = new SelectViewDialog(dialogParent)
            {
                DefaultViewName = defaultViewName,
                Items = viewTypeDictionary.Keys.ToList()
            })
            {
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
        }

        private IEnumerable<IView> GetOpenViewsFor(IEnumerable<IView> viewsToCheck, object data)
        {
            return viewsToCheck.Where(view => IsViewData(view, data));
        }

        private bool IsViewData(IView view, object data)
        {
            var viewInfo = GetViewInfoForView(view);
            return data.Equals(view.Data) || (IsDataForView(data, GetViewInfoForView(view)) && Equals(viewInfo.GetViewData(data), view.Data));
        }

        private bool IsDataForView(object data, ViewInfo info)
        {
            return info != null && data.GetType().Implements(info.DataType) && info.AdditionalDataCheck(data);
        }

        private ViewInfo GetViewInfoForView(IView view)
        {
            return viewInfos.FirstOrDefault(vi => vi.ViewType == view.GetType());
        }

        private void ClearDefaultView(object data)
        {
            var selectedItemType = data.GetType();

            if (defaultViewTypes.ContainsKey(selectedItemType))
            {
                defaultViewTypes.Remove(selectedItemType);
            }
        }

        private void SetDefaultView(Type selectedViewType, object data)
        {
            var selectedItemType = data.GetType();

            if (defaultViewTypes.ContainsKey(selectedItemType))
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

            return defaultViewTypes.ContainsKey(selectionType) ? defaultViewTypes[selectionType] : null;
        }
    }
}