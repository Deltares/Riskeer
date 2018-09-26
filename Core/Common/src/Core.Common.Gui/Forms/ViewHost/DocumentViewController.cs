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
using Core.Common.Util.Reflection;

namespace Core.Common.Gui.Forms.ViewHost
{
    /// <summary>
    /// Class responsible for finding a view given some data object.
    /// </summary>
    public class DocumentViewController : IDocumentViewController
    {
        private readonly IViewHost viewHost;
        private readonly ViewInfo[] viewInfos;
        private readonly IWin32Window dialogParent;

        private readonly IDictionary<object, Tuple<IView, ViewInfo>> openedViewLookup = new Dictionary<object, Tuple<IView, ViewInfo>>();

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

            DefaultViewTypes = new Dictionary<Type, Type>();

            viewHost.ViewClosed += ViewHostOnViewClosed;
        }

        /// <summary>
        /// Gets the default view types registered for data object types, that can be used to
        /// automatically resolve a particular view when multiple candidates are available.
        /// </summary>
        /// <remarks>The keys in this dictionary are the object types and the values the 
        /// corresponding view types.</remarks>
        public IDictionary<Type, Type> DefaultViewTypes { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool OpenViewForData(object data, bool alwaysShowDialog = false)
        {
            if (data == null)
            {
                return false;
            }

            ViewInfo[] viewInfoList = FilterOnInheritance(GetViewInfosFor(data)).ToArray();
            if (viewInfoList.Length == 0)
            {
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                viewHost.ViewClosed -= ViewHostOnViewClosed;
            }
        }

        private static IEnumerable<ViewInfo> FilterOnInheritance(IEnumerable<ViewInfo> compatibleStandaloneViewInfos)
        {
            ViewInfo[] viewInfos = compatibleStandaloneViewInfos.ToArray();

            // filter on inheritance
            IEnumerable<Type> dataTypes = viewInfos.Select(i => i.DataType);
            return viewInfos.Where(i => !dataTypes.Any(t => t != i.DataType && t.Implements(i.DataType)));
        }

        private void CreateViewFromViewInfo(object data, ViewInfo viewInfo)
        {
            Tuple<IView, ViewInfo> view;
            openedViewLookup.TryGetValue(data, out view);

            if (view != null)
            {
                viewHost.BringToFront(view.Item1);
                return;
            }

            view = new Tuple<IView, ViewInfo>(CreateViewForData(data, viewInfo), viewInfo);

            openedViewLookup.Add(data, view);

            viewHost.AddDocumentView(view.Item1);
            viewHost.SetImage(view.Item1, viewInfo.Image);
        }

        private static IView CreateViewForData(object data, ViewInfo viewInfo)
        {
            IView view = viewInfo.CreateInstance(data);

            view.Data = viewInfo.GetViewData(data);

            viewInfo.AfterCreate(view, data);

            view.Text = viewInfo.GetViewName(view, data);

            return view;
        }

        private bool ShouldRemoveViewForData(IView view, object data)
        {
            ViewInfo viewInfo = openedViewLookup.Single(openedView => ReferenceEquals(view, openedView.Value.Item1)).Value.Item2;

            if (viewInfo == null)
            {
                return false;
            }

            bool isViewData = data.GetType().Implements(viewInfo.DataType)
                              && Equals(viewInfo.GetViewData(data), view.Data);

            return isViewData || viewInfo.CloseForData(view, data);
        }

        private void ViewHostOnViewClosed(object sender, ViewChangeEventArgs viewChangeEventArgs)
        {
            object data = openedViewLookup.Where(kv => ReferenceEquals(kv.Value.Item1, viewChangeEventArgs.View))
                                          .Select(kv => kv.Key)
                                          .FirstOrDefault();

            if (data != null)
            {
                openedViewLookup.Remove(data);
            }
        }

        private Type GetDefaultViewType(object dataObject)
        {
            Type selectionType = dataObject.GetType();

            return DefaultViewTypes.ContainsKey(selectionType)
                       ? DefaultViewTypes[selectionType]
                       : null;
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

        private ViewInfo GetViewInfoUsingDialog(object data, IEnumerable<ViewInfo> viewInfoList)
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
    }
}