using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace RussLibrary.WPF
{

    public abstract class ChangeDependentList<T> :
         ChangeDependencyObject, IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        
        #region INotifyCollectionChanged Members
        List<T> items = new List<T>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        void ChangeCollection(NotifyCollectionChangedAction action, object changedItem)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
            }
        }
        void ChangeCollection(NotifyCollectionChangedAction action)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void ChangeItem(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            int retVal = -1;
            if (value is T)
            {
                T item = (T)value;
                Add(item);
                retVal = IndexOf(item);
            }
            
            return retVal;

        }

        public void Clear()
        {

            items.Clear();
            ValidationCollection.Clear();
            ChangeCollection(NotifyCollectionChangedAction.Reset);
            ChangeItem("Count");
        }

        public bool Contains(object value)
        {
            bool retVal = false;
            if (value is T)
            {
                retVal = Contains((T)value);
            }
            return retVal;
        }

        public int IndexOf(object value)
        {

            int retVal = -1;
            if (value is T)
            {
                retVal = IndexOf((T)value);

            }
            return retVal;
        }

        public void Insert(int index, object value)
        {
            if (value is T)
            {
                Insert(index, (T)value);
            }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            if (value is T)
            {
                T item = (T)value;
                Remove(item);

            }
        }

        public void RemoveAt(int index)
        {
            object item = items[index];
            items.RemoveAt(index);
            DoUpdate();
            ChangeCollection(NotifyCollectionChangedAction.Remove, item);
            ChangeItem("Count");
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {


                if (value is T)
                {
                    this[index] = (T)value;
                }
                else
                {
                    this[index] = default(T);
                }

            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            T[] newArray = array as T[];
            if (newArray != null)
            {
                CopyTo(newArray, index);
            }
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }
        object lockingObject = new object();
        public object SyncRoot
        {
            get { return lockingObject; }
        }

        #endregion




        #region IList<T> Members

        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            items.Insert(index, item);
            DoUpdate();
            ChangeCollection(NotifyCollectionChangedAction.Add);
            ChangeItem("Count");
        }

        public T this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
                DoUpdate();
                ChangeCollection(NotifyCollectionChangedAction.Replace, value);
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            items.Add(item);
            DoUpdate();
            ChangeCollection(NotifyCollectionChangedAction.Add, item);
            ChangeItem("Count");
        }

        public bool Contains(T item)
        {
            return items.Contains((T)item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array != null)
            {
                int j = -1;
                for (int i = arrayIndex; (i < array.Length && ++j < items.Count); i++)
                {
                    array[i] = items[j];
                }
            }
        }
        private void DoUpdate()
        {
            ValidationCollection.Clear();
            ProcessValidation();
            ValidationObject v = ChangeDependencyObject.GetOverallValidation(this);
            this.ValidationCollection.AddValidation(null, v.Code, v.Message);
            Changed = true;
        }

        public bool Remove(T item)
        {
            bool retVal = false;
            if (items.Contains(item))
            {

                retVal = items.Remove(item);

                DoUpdate();
               
                ChangeCollection(NotifyCollectionChangedAction.Remove, item);
                ChangeItem("Count");
            }
            return retVal;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }
        
        #endregion



        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion
    }
}
