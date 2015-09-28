using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Shell.Gui;
using DelftTools.Utils.Collections;

namespace DeltaShell.Plugins.CommonTools.Tests
{
    public class TestViewList : IViewList
    {
        private IList<IView> views = new List<IView>();

        public void AddRange(IEnumerable<IView> enumerable)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IView> GetEnumerator()
        {
            return views.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IView item)
        {
            views.Add(item);   
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(IView item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(IView[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IView item)
        {
            return views.Remove(item);
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }
        public int IndexOf(IView item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, IView item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public IView this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public event NotifyCollectionChangingEventHandler CollectionChanging;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        
        bool INotifyCollectionChange.HasParentIsCheckedInItems { get; set; }

        public bool SkipChildItemEventBubbling
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool IgnoreActivation { get; set; }
        private IView activeView;
        public IView ActiveView
        {
            get { return activeView; }
            set
            {
                activeView = value;
                OnActiveViewChanged(activeView);
            }
        }

        public event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanging;

        public event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanged;

        public event NotifyCollectionChangedEventHandler ChildViewChanged;

        public IList<IView> GetViewsForData(object data, bool includeChildViews)
        {
            throw new NotImplementedException();
        }

        public IList<IView> GetViewsToRemoveOnDataRemove(object data)
        {
            throw new NotImplementedException();
        }

        public IDictionary<Type, Type> DefaultViewTypes { get; private set; }
        public Type GetDefaultViewTypeForData(object dataObject)
        {
            throw new NotImplementedException();
        }

        public void Add(IView view, ViewLocation viewLocation)
        {
            Add(view);
        }

        public void Clear(IView view)
        {
            throw new NotImplementedException();
        }

        public IView CreateViewForData(object data, Func<ViewInfo, bool> selectViewInfo = null)
        {
            throw new NotImplementedException();
        }

        public IView CreateViewForData(object data, ViewInfo viewInfo)
        {
            throw new NotImplementedException();
        }

        public void TryToOpenViewFor(object data, Type viewType = null, bool alwaysShowDialog = false)
        {
            throw new NotImplementedException();
        }

        public Type GetDefaultViewType(object dataObject)
        {
            throw new NotImplementedException();
        }
        
        public void SetTooltip(IView view, string tooltip)
        {
            throw new NotImplementedException();
        }

        public void UpdateViewName(IView view)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ViewInfo> GetViewInfosFor(object data, Type viewType = null)
        {
            throw new NotImplementedException();
        }

        public void OpenViewForData(object data, Type viewType = null, bool alwaysShowDialog = false)
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<T> GetActiveViews<T>() where T : class, IView
        {
            return views.OfType<T>();
        }

        public IEnumerable<T> FindViewsRecursive<T>(IEnumerable<IView> views) where T : class, IView
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IView> AllViews { get; private set; }

        public void OnActiveViewChanged(IView activeView)
        {
            if (ActiveViewChanged != null)
            {
                ActiveViewChanged(this, new ActiveViewChangeEventArgs {View = activeView});
            }
        }

        public void Dispose()
        {
        }
    }
}