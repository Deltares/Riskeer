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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Drawing;
using Core.Common.Controls.Views;

namespace Core.Common.Gui.Plugin
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

        public string Description { get; set; }

        public Func<TView, TViewData, string> GetViewName { get; set; }

        public Image Image { get; set; }

        public Func<TData, bool> AdditionalDataCheck { get; set; }

        public Func<TData, TViewData> GetViewData { get; set; }

        public Action<TView, TData> AfterCreate { get; set; }

        public Action<TView, object> OnActivateView { get; set; }

        public Func<TView, object, bool> CloseForData { get; set; }

        public static implicit operator ViewInfo(ViewInfo<TData, TViewData, TView> viewInfo)
        {
            return new ViewInfo
            {
                DataType = viewInfo.DataType,
                ViewDataType = viewInfo.ViewDataType,
                ViewType = viewInfo.ViewType,
                Description = viewInfo.Description,
                Image = viewInfo.Image,
                AdditionalDataCheck = o => viewInfo.AdditionalDataCheck == null || viewInfo.AdditionalDataCheck((TData) o),
                GetViewData = o => viewInfo.GetViewData != null ? viewInfo.GetViewData((TData) o) : o,
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