using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Documents;

namespace RussLibrary.Helpers
{

    public class ReverseMultipleColumnSorter : IComparer
    {

        public ReverseMultipleColumnSorter(string[] propertyNames)
        {
            PropertyNames = propertyNames;
        }
        private string[] PropertyNames { get; set; }

        #region IComparer Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public int Compare(object x, object y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            else if (x == null && y != null)
            {
                return 1;
            }
            else if (x != null && y == null)
            {
                return -1;
            }
            else
            {
                int retVal = 0;
                List<PropertyInfo> propertiesObjectX = new List<PropertyInfo>();
                List<PropertyInfo> propertiesObjectY = new List<PropertyInfo>();
                foreach (string name in PropertyNames)
                {
                    propertiesObjectX.Add(x.GetType().GetProperty(name));
                    propertiesObjectY.Add(y.GetType().GetProperty(name));

                }
                for (int i = 0; i < PropertyNames.Length; i++)
                {
                    if (propertiesObjectX[i] != null && propertiesObjectX[i].CanRead && propertiesObjectY[i] != null && propertiesObjectY[i].CanRead)
                    {
                        if (propertiesObjectX[i].GetType() == typeof(IComparable) && propertiesObjectY[i].GetType() == typeof(IComparable))
                        {
                            retVal = ((IComparable)propertiesObjectX[i].GetValue(x, null)).CompareTo((IComparable)propertiesObjectY[i].GetValue(y, null));
                        }
                        else
                        {
                            retVal = 0;
                        }
                    }
                    if (retVal != 0)
                    {
                        break;
                    }
                }
                return retVal;

            }
        }

        #endregion
    }
    public class MultipleColumnSorter : IComparer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Properties"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyNames"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "Properties")]
        public static bool IsCandidate(ReadOnlyCollection<ItemPropertyInfo> Properties, string[] propertyNames)
        {
            return false;

            //below not supported-not sure how to get this to work.
            //bool retVal = true;
            //foreach (string name in propertyNames)
            //{
            //    foreach (ItemPropertyInfo item in Properties)
            //    {
            //        if (item.Name.ToUpper() == name.ToUpper() && item.GetType() != typeof(IComparable))
            //        {
            //            retVal = false;
            //            break;
            //        }
            //    }

            //}
            //return retVal;
        }
        public MultipleColumnSorter(string[] propertyNames)
        {
            PropertyNames = propertyNames;
        }
        private string[] PropertyNames { get; set; }

        #region IComparer Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public int Compare(object x, object y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            else if (x == null && y != null)
            {
                return -1;
            }
            else if (x != null && y == null)
            {
                return 1;
            }
            else
            {
                int retVal = 0;
                List<PropertyInfo> propertiesObjectX = new List<PropertyInfo>();
                List<PropertyInfo> propertiesObjectY = new List<PropertyInfo>();
                foreach (string name in PropertyNames)
                {
                    propertiesObjectX.Add(x.GetType().GetProperty(name));
                    propertiesObjectY.Add(y.GetType().GetProperty(name));

                }
                for (int i = 0; i < PropertyNames.Length; i++)
                {
                    if (propertiesObjectX[i] != null && propertiesObjectX[i].CanRead && propertiesObjectY[i] != null && propertiesObjectY[i].CanRead)
                    {
                        if (propertiesObjectX[i].GetType() == typeof(IComparable) && propertiesObjectY[i].GetType() == typeof(IComparable))
                        {
                            retVal = ((IComparable)propertiesObjectY[i].GetValue(y, null)).CompareTo((IComparable)propertiesObjectX[i].GetValue(x, null));
                        }
                        else
                        {
                            retVal = 0;
                        }
                    }
                    if (retVal != 0)
                    {
                        break;
                    }
                }
                return retVal;

            }
        }

        #endregion
    }

    public class ReverseComparableSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            IComparable dtx = x as IComparable;
            IComparable dty = y as IComparable;
            if (dtx == null && dty == null)
            {
                return 0;
            }
            else if (dtx == null && dty != null)
            {
                return -1;
            }
            else if (dtx != null && dty == null)
            {
                return 1;
            }
            else
            {

                return dty.CompareTo(dtx);
            }
        }
    }

    public class ComparableSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            IComparable dtx = x as IComparable;
            IComparable dty = y as IComparable;
            if (dtx == null && dty == null)
            {
                return 0;
            }
            else if (dtx == null && dty != null)
            {
                return 1;
            }
            else if (dtx != null && dty == null)
            {
                return -1;
            }
            else
            {

                return dtx.CompareTo(dty);
            }
        }
    }

    public class ReverseDateSorter : IComparer
    {

        public int Compare(object x, object y)
        {
            DateTime? dtx = x as DateTime?;
            DateTime? dty = y as DateTime?;
            if (dtx == null && dty == null)
            {
                return 0;
            }
            else if (dtx == null && dty != null)
            {
                return -1;
            }
            else if (dtx != null && dty == null)
            {
                return 1;
            }
            else
            {

                return dty.Value.CompareTo(dtx.Value);
            }
        }
    }

    public class DateSorter : IComparer
    {

        public int Compare(object x, object y)
        {
            DateTime? dtx = x as DateTime?;
            DateTime? dty = y as DateTime?;
            if (dtx == null && dty == null)
            {
                return 0;
            }
            else if (dtx == null && dty != null)
            {
                return 1;
            }
            else if (dtx != null && dty == null)
            {
                return -1;
            }
            else
            {

                return dtx.Value.CompareTo(dty.Value);
            }
        }
    }
    public static class GridViewColumnHeaderSorter
    {

        #region GridColumnHeader Sorting in ListView
        //To use: Binding for ItemsSource should be ObservableCollection<T>.
        //Currently only works for ListViews.
        // Add <GridColumnHeader Extensions.SortColumnID="xxx"> where "xxx" is the field name in the binding
        //  to sort on.  only works with simple (one field) sorting.


        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.RegisterAttached("SortDirection",
            typeof(ListSortDirection), typeof(GridViewColumnHeaderSorter),
            new FrameworkPropertyMetadata(ListSortDirection.Ascending));


        public static void SetSortDirection(DependencyObject element, ListSortDirection value)
        {
            if (element != null)
            {
                element.UIThreadSetValue(SortDirectionProperty, value);
            }
        }
        public static ListSortDirection GetSortDirection(DependencyObject element)
        {

            ListSortDirection value = ListSortDirection.Ascending;
            if (element != null)
            {
                value = (ListSortDirection)element.UIThreadGetValue(SortDirectionProperty);

            }
            return value;
        }



        public static readonly DependencyProperty IsDefaultProperty =
           DependencyProperty.RegisterAttached("IsDefault",
           typeof(bool), typeof(GridViewColumnHeaderSorter),
           new FrameworkPropertyMetadata(true));


        public static void SetIsDefault(DependencyObject element, bool value)
        {
            if (element != null)
            {
                element.UIThreadSetValue(IsDefaultProperty, value);
            }
        }
        public static bool GetIsDefault(DependencyObject element)
        {

            bool value = true;
            if (element != null)
            {
                value = (bool)element.UIThreadGetValue(IsDefaultProperty);

            }
            return value;
        }




        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static readonly DependencyProperty SortColumnIDProperty =
            DependencyProperty.RegisterAttached("SortColumnID",
            typeof(string), typeof(GridViewColumnHeaderSorter), new FrameworkPropertyMetadata(OnSortColumnIDChanged));


        static void OnParentListViewChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListView oldparent = e.OldValue as ListView;
            ListView newparent = e.NewValue as ListView;

            if (newparent != null)
            {
                newparent.ItemContainerGenerator.ItemsChanged += (o, err) => ItemContainerGenerator_ItemsChanged(o, err, newparent);

            }
            if (oldparent != null)
            {
                oldparent.ItemContainerGenerator.ItemsChanged -= (o, err) => ItemContainerGenerator_ItemsChanged(o, err, oldparent);
            }
        }
        public static readonly DependencyProperty ParentListViewProperty =
            DependencyProperty.RegisterAttached("ParentListView",
            typeof(ListView), typeof(GridViewColumnHeaderSorter), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnParentListViewChanged)));




        private static readonly DependencyProperty CurrentSortColumnProperty =
            DependencyProperty.RegisterAttached("CurrentSortColumn",
            typeof(GridViewColumnHeader), typeof(GridViewColumnHeaderSorter), new FrameworkPropertyMetadata());


        static void OnCurrentSortAdornerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListView me = sender as ListView;
            if (me != null)
            {
                //Sender will be a listview?
                //remove all sortadorners from all columns but one for e.newvalue.

              
            }
        }

        private static readonly DependencyProperty CurrentSortAdornerProperty =
            DependencyProperty.RegisterAttached("CurrentSortAdorner",
            typeof(SortAdorner), typeof(GridViewColumnHeaderSorter), new FrameworkPropertyMetadata(OnCurrentSortAdornerChanged));

        private static readonly DependencyProperty IsSortingProperty =
          DependencyProperty.RegisterAttached("IsSorting",
          typeof(bool), typeof(GridViewColumnHeaderSorter), new FrameworkPropertyMetadata());

        private static void OnSortColumnIDChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            GridViewColumnHeader column = sender as GridViewColumnHeader;

            if (column != null)
            {
                if (e.NewValue != null && e.OldValue == null)
                {
                    column.Click += new RoutedEventHandler(GridColumnHeader_Click);
                    column.Loaded += new RoutedEventHandler(column_Loaded);

                }
                if (e.NewValue == null && e.OldValue != null)
                {
                    column.Click -= new RoutedEventHandler(GridColumnHeader_Click);
                    column.Loaded -= new RoutedEventHandler(column_Loaded);
                }


                //ListView lv = GetAncestor<ListView>(column);
                //if (lv != null)
                //{
                //    SetParentListView(column, lv);
                //    if (e.NewValue != null && e.OldValue == null)
                //    {
                //        lv.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridColumnHeader_Click));
                //    }
                //    if (e.NewValue == null && e.OldValue != null)
                //    {
                //        lv.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridColumnHeader_Click));
                //    }
                //}
            }

        }

        static void column_Loaded(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (GridViewColumnHeader)sender;
            if (GetIsDefault(column))
            {
                column.Sort();
            }

        }


        private static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
        {
            if (reference != null)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(reference);
                if (parent != null)
                {
                    while (!(parent is T))
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }
                }
                if (parent != null)
                {
                    return (T)parent;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static void SetSortColumnID(DependencyObject element, string value)
        {
            if (element != null)
            {
                element.UIThreadSetValue(SortColumnIDProperty, value);
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static string GetSortColumnID(DependencyObject element)
        {

            string value = null;
            if (element != null)
            {
                value = (string)element.UIThreadGetValue(SortColumnIDProperty);

            }
            return value;
        }
        private static void SetIsSorting(DependencyObject element, bool value)
        {
            if (element != null)
            {
                element.UIThreadSetValue(IsSortingProperty, value);
            }
        }
        private static bool GetIsSorting(DependencyObject element)
        {

            bool value = false;
            if (element != null)
            {
                bool? val = element.UIThreadGetValue(IsSortingProperty) as bool?;
                if (val == null)
                {
                    value = false;
                }
                else
                {
                    value = val.Value;
                }

            }
            return value;
        }

        private static void SetParentListView(DependencyObject element, ListView value)
        {
            if (element != null)
            {
                element.UIThreadSetValue(ParentListViewProperty, value);
            }
        }
        private static ListView GetParentListView(DependencyObject element)
        {

            ListView value = null;
            if (element != null)
            {
                value = (ListView)element.UIThreadGetValue(ParentListViewProperty);

            }
            return value;
        }

        private static void SetCurrentSortColumn(ListView element, GridViewColumnHeader value)
        {
            if (element != null)
            {
               // object x = value.Tag;
                element.UIThreadSetValue(CurrentSortColumnProperty, value);
            }
        }
        private static GridViewColumnHeader GetCurrentSortColumn(ListView element)
        {

            GridViewColumnHeader value = null;
            if (element != null)
            {
                value = (GridViewColumnHeader)element.UIThreadGetValue(CurrentSortColumnProperty);
                //if (value != null)
                //{
                    //object x = value.Tag;
                //}

            }
            return value;
        }
        private static void SetCurrentSortAdorner(ListView element, SortAdorner value)
        {
            if (element != null)
            {
                element.UIThreadSetValue(CurrentSortAdornerProperty, value);
            }
        }
        private static SortAdorner GetCurrentSortAdorner(ListView element)
        {

            SortAdorner value = null;
            if (element != null)
            {
                value = (SortAdorner)element.UIThreadGetValue(CurrentSortAdornerProperty);

            }
            return value;
        }
        private static void GridColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked != null)
            {
                SetSortDirection(headerClicked, (GetSortDirection(headerClicked) == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending);

                headerClicked.Sort();
            }
        }
        static ListView ParentListView(this GridViewColumnHeader me)
        {
            ListView parent = GetParentListView(me);
            if (parent == null)
            {
                parent = GetAncestor<ListView>(me);
                if (parent != null)
                {


                    SetParentListView(me, parent);
                }
            }
            return parent;
        }
        public static void Sort(this GridViewColumnHeader column)
        {
            if (column != null)
            {
                ListView parent = column.ParentListView();
                if (parent != null)
                {
                    if (!GetIsSorting(parent))
                    {
                        SetIsSorting(parent, true);
                        string fieldxxx = GetSortColumnID(column);
                        string[] fieldList = null;
                        string field = null;
                        if (!string.IsNullOrEmpty(fieldxxx))
                        {
                            fieldList = fieldxxx.Split('|');
                            if (fieldList.Length == 1)
                            {
                                field = fieldList[0];
                            }
                        }


                        //TODO: Allow compound column fields.


                        GridViewColumnHeader _CurSortCol = GetCurrentSortColumn(parent);


                        SortAdorner _CurAdorner = GetCurrentSortAdorner(parent);


                        if (fieldList != null)
                        {
                            ListCollectionView dataView = CollectionViewSource.GetDefaultView(parent.ItemsSource) as ListCollectionView;
                            if (dataView != null)
                            {
                                if (_CurSortCol != null)
                                {
                                    AdornerLayer.GetAdornerLayer(_CurSortCol).Remove(_CurAdorner);
                                    dataView.SortDescriptions.Clear();
                                    parent.Items.SortDescriptions.Clear();
                                }
                                else
                                {
                                    if (_CurAdorner != null)
                                    {
                                        AdornerLayer.GetAdornerLayer(parent).Remove(_CurAdorner);
                                        dataView.SortDescriptions.Clear();
                                        parent.Items.SortDescriptions.Clear();
                                    }
                                }
                                ListSortDirection newDir = GetSortDirection(column);


                                _CurAdorner = new SortAdorner(column, newDir);



                                AdornerLayer.GetAdornerLayer(column).Add(_CurAdorner);


                                SetCurrentSortColumn(parent, column);

                                SetCurrentSortAdorner(parent, _CurAdorner);

                                IComparer sorter = null;

                                if (fieldList.Length > 1)
                                {

                                    if (MultipleColumnSorter.IsCandidate(dataView.ItemProperties, fieldList))
                                    {
                                        if (newDir == ListSortDirection.Ascending)
                                        {
                                            sorter = new MultipleColumnSorter(fieldList);
                                        }
                                        else
                                        {
                                            sorter = new ReverseMultipleColumnSorter(fieldList);
                                        }
                                    }
                                    else
                                    {
                                        foreach (string f in fieldList)
                                        {
                                            dataView.SortDescriptions.Add(new SortDescription(f, newDir));
                                        }
                                    }

                                }
                                else
                                {
                                    foreach (ItemPropertyInfo p in dataView.ItemProperties)
                                    {
                                        if (p.Name.ToUpperInvariant() == field.ToUpperInvariant())
                                        {
                                            Type ptype = p.GetType();
                                            if (ptype == typeof(DateTime))
                                            {
                                                if (newDir == ListSortDirection.Ascending)
                                                {
                                                    sorter = new DateSorter();
                                                }
                                                else
                                                {
                                                    sorter = new ReverseDateSorter();
                                                }
                                            }
                                            else if (ptype == typeof(IComparable))
                                            {
                                                if (newDir == ListSortDirection.Ascending)
                                                {
                                                    sorter = new ComparableSorter();
                                                }
                                                else
                                                {
                                                    sorter = new ReverseComparableSorter();
                                                }
                                            }
                                            break;
                                        }
                                    }

                                    if (sorter == null)
                                    {
                                        dataView.SortDescriptions.Add(new SortDescription(field, newDir));
                                    }
                                    else
                                    {

                                        dataView.CustomSort = sorter;
                                    }
                                }
                                dataView.Refresh();
                            }
                        }
                        SetIsSorting(parent, false);
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "sender"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "e")]
        static void ItemContainerGenerator_ItemsChanged(object sender, System.Windows.Controls.Primitives.ItemsChangedEventArgs e, ListView parent)
        {

            if (parent != null)
            {
                GetCurrentSortColumn(parent).Sort();
            }

        }
        #endregion
    }
}

