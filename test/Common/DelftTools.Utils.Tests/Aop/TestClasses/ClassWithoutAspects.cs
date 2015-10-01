using System.ComponentModel;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.Data;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    /// <summary>
    /// See also <see cref="ClassWithAspects"/> to compare amount of code when aspects are used.
    /// </summary>
    public class ClassWithoutAspects : EditableObjectUnique<long>, INotifyPropertyChange, INotifyCollectionChange
    {
        private string name;

        private ClassWithoutAspects child;

        private IEventedList<ClassWithoutAspects> children;

        public ClassWithoutAspects()
        {
            Children = new EventedList<ClassWithoutAspects>();
        }

        public IEventedList<ClassWithoutAspects> Children
        {
            get
            {
                return children;
            }
            set
            {
                FirePropertyChanging("Children");
                UnsubscribeEvents(children);
                children = value;
                SubscribeEvents(children);
                FirePropertyChanged("Children");
            }
        }

        public ClassWithoutAspects Child
        {
            get
            {
                return child;
            }
            set
            {
                FirePropertyChanging("Child");
                UnsubscribeEvents(child);
                child = value;
                SubscribeEvents(child);
                FirePropertyChanged("Child");
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                FirePropertyChanging("Name");
                name = value;
                FirePropertyChanged("Name");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangingEventHandler PropertyChanging;

        public event NotifyCollectionChangingEventHandler CollectionChanging;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        bool INotifyCollectionChange.HasParentIsCheckedInItems { get; set; }

        public bool SkipChildItemEventBubbling
        {
            get;
            set;
        }

        private void FirePropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        private void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void UnsubscribeEvents(object o)
        {
            var propertyChange = o as INotifyPropertyChange;

            if (propertyChange != null)
            {
                propertyChange.PropertyChanging -= OnPropertyChanging;
                propertyChange.PropertyChanged -= OnPropertyChanged;
            }

            var collectionChange = o as INotifyCollectionChange;

            if (collectionChange != null)
            {
                collectionChange.CollectionChanging -= OnCollectionChanging;
                collectionChange.CollectionChanged -= OnCollectionChanged;
            }
        }

        private void SubscribeEvents(object o)
        {
            var propertyChange = o as INotifyPropertyChange;

            if (propertyChange != null)
            {
                propertyChange.PropertyChanging += OnPropertyChanging;
                propertyChange.PropertyChanged += OnPropertyChanged;
            }
        }

        private void OnPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(sender, e);
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(sender, e);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (CollectionChanging != null)
            {
                CollectionChanging(sender, e);
            }
        }

        private void OnCollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(sender, e);
            }
        }

        bool INotifyPropertyChange.HasParent { get; set; }
    }
}