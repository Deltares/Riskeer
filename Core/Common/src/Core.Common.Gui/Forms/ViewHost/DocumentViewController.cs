// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Core.Common.Gui.Forms.ViewHost
{
    /// <summary>
    /// Class responsible for finding a view given some data-object.
    /// </summary>
    public class DocumentViewController : IDocumentViewController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentViewController));

        private readonly IViewHost viewHost;
        private readonly ViewInfo[] viewInfos;
        private readonly IWin32Window dialogParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentViewController"/> class.
        /// </summary>
        /// <param name="viewHost">The view host.</param>
        /// <param name="viewInfos">The sequence of available view info objects.</param>
        /// <param name="dialogParent">The parent object for which dialogs should be shown on top.</param>
        public DocumentViewController(IViewHost viewHost, IEnumerable<ViewInfo> viewInfos, IWin32Window dialogParent)
        {
            this.viewHost = viewHost;
            this.viewInfos = viewInfos.ToArray();
            this.dialogParent = dialogParent;
        }

        public IDictionary<Type, Type> DefaultViewTypes { get; } = new Dictionary<Type, Type>();

        public bool OpenViewForData(object data, bool alwaysShowDialog = false)
        {
            if (data == null)
            {
                return false;
            }

            ViewInfo[] viewInfoList = FilterOnInheritance(GetViewInfosFor(data)).ToArray();
            if (viewInfoList.Length == 0)
            {
                log.DebugFormat(Resources.DocumentViewController_OpenViewForData_No_view_registered_for_0_, data);
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
                Type defaultType = GetDefaultViewType(data);
                ViewInfo defaultViewInfo = viewInfoList.FirstOrDefault(vi => vi.ViewType == defaultType);

                if (defaultViewInfo != null)
                {
                    CreateViewFromViewInfo(data, defaultViewInfo);
                    return true;
                }
            }

            // Create chosen view
            ViewInfo chosenViewInfo = GetViewInfoUsingDialog(data, viewInfoList);
            if (chosenViewInfo == null)
            {
                return false;
            }

            CreateViewFromViewInfo(data, chosenViewInfo);

            return true;
        }

        public void CloseAllViewsFor(object data)
        {
            if (data == null)
            {
                return;
            }

            foreach (IView view in viewHost.DocumentViews.Where(view => ShouldRemoveViewForData(view, data)).ToArray())
            {
                viewHost.Remove(view);
            }
        }

        public IEnumerable<ViewInfo> GetViewInfosFor(object data)
        {
            return viewInfos.Where(vi => data.GetType().Implements(vi.DataType) && vi.AdditionalDataCheck(data));
        }

        private bool ShouldRemoveViewForData(IView view, object data)
        {
            if (IsViewData(view, data))
            {
                return true;
            }

            ViewInfo viewInfo = GetViewInfoForView(view);
            return viewInfo != null && viewInfo.CloseForData(view, data);
        }

        private Type GetDefaultViewType(object dataObject)
        {
            Type selectionType = dataObject.GetType();

            return DefaultViewTypes.ContainsKey(selectionType)
                       ? DefaultViewTypes[selectionType]
                       : null;
        }

        private static IEnumerable<ViewInfo> FilterOnInheritance(IEnumerable<ViewInfo> compatibleStandaloneViewInfos)
        {
            ViewInfo[] viewInfos = compatibleStandaloneViewInfos.ToArray();

            // filter on inheritance
            Type[] dataTypes = viewInfos.Select(i => i.DataType).ToArray();
            return viewInfos.Where(i => !dataTypes.Any(t => t != i.DataType && t.Implements(i.DataType)));
        }

        private void CreateViewFromViewInfo(object data, ViewInfo viewInfo)
        {
            object viewData = viewInfo.GetViewData(data);
            IView view = (viewInfo.DataType == viewInfo.ViewDataType
                              ? GetOpenViewsFor(viewHost.DocumentViews, data)
                              : GetOpenViewsFor(viewHost.DocumentViews, data).Concat(GetOpenViewsFor(viewHost.DocumentViews, viewData)))
                .FirstOrDefault(v => v.GetType() == viewInfo.ViewType);

            if (view != null)
            {
                viewHost.SetFocusToView(view);
                return;
            }

            IView newView = CreateViewForData(data, viewInfo);

            viewHost.AddDocumentView(newView);
            viewHost.SetImage(newView, viewInfo.Image);
        }

        private static IView CreateViewForData(object data, ViewInfo viewInfo)
        {
            IView view = viewInfo.CreateInstance();

            view.Data = viewInfo.GetViewData(data);

            viewInfo.AfterCreate(view, data);

            view.Text = viewInfo.GetViewName(view, view.Data);

            return view;
        }

        private ViewInfo GetViewInfoUsingDialog(object data, IList<ViewInfo> viewInfoList)
        {
            Type defaultViewTypeForData = GetDefaultViewTypeForData(data);
            string defaultViewName = defaultViewTypeForData != null
                                         ? viewInfoList.First(vi => vi.ViewType == defaultViewTypeForData).Description
                                         : null;

            Dictionary<string, ViewInfo> viewTypeDictionary = viewInfoList.ToDictionary(vi => vi.Description ?? vi.ViewType.Name);
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
                ViewInfo selectedViewInfo = viewTypeDictionary[viewSelector.SelectedItem];

                if (viewSelector.DefaultViewName == null)
                {
                    ClearDefaultView(data);
                }
                else
                {
                    ViewInfo defaultViewInfo = viewTypeDictionary[viewSelector.DefaultViewName];
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
            ViewInfo viewInfo = GetViewInfoForView(view);
            return data.Equals(view.Data) || IsDataForView(data, viewInfo) && Equals(viewInfo.GetViewData(data), view.Data);
        }

        private static bool IsDataForView(object data, ViewInfo info)
        {
            return info != null && data.GetType().Implements(info.DataType) && info.AdditionalDataCheck(data);
        }

        private ViewInfo GetViewInfoForView(IView view)
        {
            return viewInfos.FirstOrDefault(vi => vi.ViewType == view.GetType());
        }

        private void ClearDefaultView(object data)
        {
            Type selectedItemType = data.GetType();

            if (DefaultViewTypes.ContainsKey(selectedItemType))
            {
                DefaultViewTypes.Remove(selectedItemType);
            }
        }

        private void SetDefaultView(Type selectedViewType, object data)
        {
            Type selectedItemType = data.GetType();

            if (DefaultViewTypes.ContainsKey(selectedItemType))
            {
                DefaultViewTypes[selectedItemType] = selectedViewType;
            }
            else
            {
                DefaultViewTypes.Add(selectedItemType, selectedViewType);
            }
        }

        private Type GetDefaultViewTypeForData(object dataObject)
        {
            Type selectionType = dataObject.GetType();

            return DefaultViewTypes.ContainsKey(selectionType) ? DefaultViewTypes[selectionType] : null;
        }
    }
}