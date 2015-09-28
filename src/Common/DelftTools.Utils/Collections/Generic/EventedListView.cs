using System;
using System.Collections.Generic;
using DelftTools.Utils.Aop;

namespace DelftTools.Utils.Collections.Generic
{
    /// <summary>
    /// TODO: extend with logic when item in the parent changes and suddenly becomes part of the view
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventedListView<T> : EventedList<T>
    {
        private readonly List<int> indices = new List<int>();

        private EventedListView(){}

        public EventedListView(IEventedList<T> parent, Func<T, bool> filter)
        {
            Parent = parent;
            Filter = filter;

            if (Parent == null) return;

            Parent.CollectionChanged += Parent_CollectionChanged;

            for (var i = 0; i < parent.Count; i++)
            {
                var item = parent[i];
                
                if (!Filter(item))
                {
                    continue;
                }

                indices.Add(i);
                Add(item);
            }
        }

        [EditAction]
        void Parent_CollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if(sender != Parent)
            {
                return; // no handling for internal collection changed
            }

            switch (e.Action)
            {
                case NotifyCollectionChangeAction.Add:
                    if(Filter((T) e.Item))
                    {
                        Add((T) e.Item);
                        indices.Add(e.Index);
                    }
                    break;
                case NotifyCollectionChangeAction.Remove:
                    if (Filter((T) e.Item))
                    {
                        Remove(e.Item);
                        indices.Remove(e.Index);
                    }
                    break;
                case NotifyCollectionChangeAction.Replace:
                    if (Filter((T)e.Item))
                    {
                        var index = indices.IndexOf(e.Index);
                        this[index] = (T)e.Item;
                    }
                    break;
            }
        }

        public Func<T, bool> Filter { get; private set; }

        public IEventedList<T> Parent { get; private set; }
    }
}