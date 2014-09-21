using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Documents;

namespace RussLibrary.Helpers
{

    public static class DragHelper
    {
        public static readonly DependencyProperty IsDraggingProperty =
          DependencyProperty.RegisterAttached("IsDragging",
          typeof(bool), typeof(DragHelper), new FrameworkPropertyMetadata());

        public static void SetIsDragging(DependencyObject element, bool value)
        {
            if (element != null)
            {
                element.SetValue(IsDraggingProperty, value);
            }
        }
        public static bool GetIsDragging(DependencyObject element)
        {

            bool retVal = false;
            if (element != null)
            {
                retVal = (bool)element.GetValue(IsDraggingProperty);

            }
            return retVal;
        }

        public static readonly DependencyProperty DragHasLeftScopeProperty =
          DependencyProperty.RegisterAttached("DragHasLeftScope",
          typeof(bool), typeof(DragHelper), new FrameworkPropertyMetadata());

        public static void SetDragHasLeftScope(DependencyObject element, bool value)
        {
            if (element != null)
            {
                element.SetValue(DragHasLeftScopeProperty, value);
            }
        }
        public static bool GetDragHasLeftScope(DependencyObject element)
        {

            bool retVal = false;
            if (element != null)
            {
                retVal = (bool)element.GetValue(DragHasLeftScopeProperty);

            }
            return retVal;
        }


        public static readonly DependencyProperty StartPointProperty =
           DependencyProperty.RegisterAttached("StartPoint",
           typeof(Point), typeof(DragHelper), new FrameworkPropertyMetadata());

        public static void SetStartPoint(DependencyObject element, Point value)
        {
            if (element != null)
            {
                element.SetValue(StartPointProperty, value);
            }
        }
        public static Point GetStartPoint(DependencyObject element)
        {

            Point retVal;
            if (element != null)
            {
                retVal = (Point)element.GetValue(StartPointProperty);

            }
            else
            {
                retVal = new Point();
            }

            return retVal;
        }



        public static readonly DependencyProperty DragAdornerProperty =
           DependencyProperty.RegisterAttached("DragAdorner",
           typeof(DragAdorner), typeof(DragHelper), new FrameworkPropertyMetadata());

        public static void SetDragAdorner(DependencyObject element, DragAdorner value)
        {
            if (element != null)
            {
                element.SetValue(DragAdornerProperty, value);
            }
        }
        public static DragAdorner GetDragAdorner(DependencyObject element)
        {

            DragAdorner retVal = null;
            if (element != null)
            {
                retVal = (DragAdorner)element.GetValue(DragAdornerProperty);

            }
            return retVal;
        }




        public static readonly DependencyProperty RelativeMousePointProperty =
           DependencyProperty.RegisterAttached("RelativeMousePoint",
           typeof(Point), typeof(DragHelper), new FrameworkPropertyMetadata());

        public static void SetRelativeMousePoint(DependencyObject element, Point value)
        {
            if (element != null)
            {
                element.SetValue(RelativeMousePointProperty, value);
            }
        }
        public static Point GetRelativeMousePoint(DependencyObject element)
        {

            Point retVal;
            if (element != null)
            {
                retVal = (Point)element.GetValue(RelativeMousePointProperty);

            }
            else
            {
                retVal = new Point();
            }
            return retVal;
        }




        public static readonly DependencyProperty HasDroppedProperty =
         DependencyProperty.RegisterAttached("HasDropped",
         typeof(bool), typeof(DragHelper), new FrameworkPropertyMetadata());

        /// <summary>
        /// Sets the has dropped.  Useful if multiple Drop handlers are tripped.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetHasDropped(DependencyObject element, bool value)
        {
            if (element != null)
            {
                element.UIThreadSetValue(HasDroppedProperty, value);
            }
        }
        public static bool GetHasDropped(DependencyObject element)
        {

            bool retVal = false;
            if (element != null)
            {
                retVal = (bool)element.UIThreadGetValue(HasDroppedProperty);

            }
            return retVal;
        }



        public static readonly DependencyProperty DragScopeProperty =
         DependencyProperty.RegisterAttached("DragScope",
         typeof(FrameworkElement), typeof(DragHelper), new FrameworkPropertyMetadata());

        public static void SetDragScope(DependencyObject element, FrameworkElement value)
        {
            if (element != null)
            {
                element.UIThreadSetValue(DragScopeProperty, value);
            }
        }
        public static FrameworkElement GetDragScope(DependencyObject element)
        {

            FrameworkElement retVal = null;
            if (element != null)
            {
                retVal = (FrameworkElement)element.UIThreadGetValue(DragScopeProperty);

            }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetDragTypes(this UIElement source, params Type[] types)
        {
            List<Type> ValidTypes = source.UIThreadGetValue(ValidDragTypesProperty) as List<Type>;
            if (ValidTypes != null)
            {
                if (types != null)
                {
                    foreach (Type t in types)
                    {
                        ValidTypes.Add(t);
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void SetInvalidDragTypes(this UIElement source, params Type[] types)
        {
            List<Type> InValidDragTypes = source.UIThreadGetValue(InValidDragTypesProperty) as List<Type>;
            if (InValidDragTypes != null)
            {
                if (types != null)
                {
                    foreach (Type t in types)
                    {
                        InValidDragTypes.Add(t);
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "InValid")]
        public static readonly DependencyProperty InValidDragTypesProperty =
            DependencyProperty.RegisterAttached("InValidDragTypes",
            typeof(List<Type>), typeof(DragHelper), new FrameworkPropertyMetadata());


        public static readonly DependencyProperty ValidDragTypesProperty =
         DependencyProperty.RegisterAttached("ValidDragTypes",
         typeof(List<Type>), typeof(DragHelper), new FrameworkPropertyMetadata());

        public static void InitializeDragging(this UIElement source, FrameworkElement scope)
        {
            if (source != null)
            {
                source.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(source_PreviewMouseLeftButtonDown);
                source.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(source_PreviewMouseMove);

                source.UIThreadSetValue(ValidDragTypesProperty, new List<Type>());
                source.UIThreadSetValue(InValidDragTypesProperty, new List<Type>());
                SetDragScope(source, scope);
            }
        }

        public static void InitializeDragging(this UIElement source)
        {
            source.InitializeDragging(source as FrameworkElement);
        }

        static void source_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            
            e.UseDefaultCursors = false;
            e.Handled = true;
        }
        public static event EventHandler<DragStartedEventArgs> PreviewDragStarted;


        static void source_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DependencyObject obj = sender as DependencyObject;
            if (e.LeftButton == MouseButtonState.Pressed && !GetIsDragging(obj))
            {

                Point position = e.GetPosition(null);
                Point startPoint = GetStartPoint(obj);

                if (Math.Abs(position.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    DependencyObject dragElement = FindAncestor<UIElement>((DependencyObject)e.OriginalSource, obj);
                    if (dragElement != null)
                    {
                        StartDragInProcAdorner(obj, dragElement);
                    }

                }
            }
        }
        private static DependencyObject FindAncestor<T>(DependencyObject current, DependencyObject source)
            where T : DependencyObject
        {
            do
            {
                List<Type> validTypes = (List<Type>)source.UIThreadGetValue(ValidDragTypesProperty);

                List<Type> invalidTypes = (List<Type>)source.UIThreadGetValue(InValidDragTypesProperty);
                T wrk = current as T;

                if (wrk != null && invalidTypes.Count > 0 && invalidTypes.Contains(current.GetType()))
                {
                    return null;
                }

                if ((wrk != null && validTypes.Count == 0) || (validTypes.Count > 0 && validTypes.Contains(current.GetType())))
                {
                    return current;
                }
                if (current is Visual || current is Visual3D)
                {
                    current = VisualTreeHelper.GetParent(current);
                }
                else
                {
                    current = null;
                }
            } while (current != null);
            return null;
        }


        private static void StartDragInProcAdorner(DependencyObject source, DependencyObject dragElement)
        {

            // Let's define our DragScope .. In this case it is every thing inside our main window .. 
            //FrameworkElement DragScope = (source as FrameworkElement).Parent as FrameworkElement;
            FrameworkElement DragScope = GetDragScope(source); // source as FrameworkElement;// Application.Current.MainWindow.Content as FrameworkElement;

            SetHasDropped(dragElement, false);

            // We enable Drag & Drop in our scope ...  We are not implementing Drop, so it is OK, but this allows us to get DragOver 
            bool previousDrop = DragScope.AllowDrop;
            DragScope.AllowDrop = true;

            // Let's wire our usual events.. 
            // GiveFeedback just tells it to use no standard cursors..  

            GiveFeedbackEventHandler feedbackhandler = new GiveFeedbackEventHandler(source_GiveFeedback);
            DragScope.GiveFeedback += feedbackhandler;

            // The DragOver event ... 
            DragEventHandler draghandler = new DragEventHandler(DragOver);
            DragScope.PreviewDragOver += draghandler;

            // Drag Leave is optional, but write up explains why I like it .. 
            DragEventHandler dragleavehandler = new DragEventHandler(DragScope_DragLeave);
            DragScope.DragLeave += dragleavehandler;

            // QueryContinue Drag goes with drag leave... 
            //QueryContinueDragEventHandler queryhandler = new QueryContinueDragEventHandler(DragScope_QueryContinueDrag);
            //DragScope.QueryContinueDrag += queryhandler;

            //Here we create our adorner.. 
            DragAdorner adorn = new DragAdorner(DragScope, (UIElement)dragElement, true, 1);
            //DragAdorner adorn = new DragAdorner((FrameworkElement)source, (UIElement)dragElement, true, 0.5);

            SetDragAdorner(DragScope, adorn);
            AdornerLayer layr = AdornerLayer.GetAdornerLayer(DragScope as Visual);

            layr.Add(adorn);
            adorn.BringIntoView();


            SetIsDragging(source, true);


            SetDragHasLeftScope(source, false);
            //Finally lets drag drop 
            //DataObject data = new DataObject(System.Windows.DataFormats.Text.ToString(), "abcd");
            DataObject data = new DataObject(dragElement);

            if (PreviewDragStarted != null)
            {
                DragStartedEventArgs eArg = new DragStartedEventArgs();
                eArg.DragObject = dragElement;
                PreviewDragStarted(DragScope, eArg);
            }
            //DragDropEffects de =
            DragDrop.DoDragDrop(DragScope, data, DragDropEffects.Move);

            // Clean up our mess :) 
            DragScope.AllowDrop = previousDrop;

            //AdornerLayer.GetAdornerLayer(DragScope).Remove(GetDragAdorner(DragScope));
            layr.Remove(adorn);

            SetDragAdorner(source, null);

            DragScope.GiveFeedback -= feedbackhandler;
            DragScope.DragLeave -= dragleavehandler;
            //DragScope.QueryContinueDrag -= queryhandler;
            DragScope.PreviewDragOver -= draghandler;

            SetIsDragging(source, false);
        }


        static void DragScope_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (GetDragHasLeftScope((DependencyObject)sender))
            {
                e.Action = DragAction.Cancel;
                e.Handled = true;
            }

        }

        static void DragScope_DragLeave(object sender, DragEventArgs e)
        {
            DependencyObject me = (DependencyObject)sender;
            FrameworkElement drgScope = GetDragScope(me);
            if (e.OriginalSource == drgScope)
            {
                Point p = e.GetPosition(drgScope);
                Rect r = VisualTreeHelper.GetContentBounds(drgScope);
                if (!r.Contains(p))
                {
                    SetDragHasLeftScope(me, true);
                    e.Handled = true;
                }
            }

        }
        static void DragOver(object sender, DragEventArgs args)
        {
            DependencyObject me = (DependencyObject)sender;
            DragAdorner adorn = GetDragAdorner(me);
            if (adorn != null)
            {

                //FrameworkElement dragscope = GetDragScope(me);
                Point pos = args.GetPosition((IInputElement)me);
                //Point pos2 = DragHelper.GetRelativeMousePoint(me);
                adorn.LeftOffset = pos.X;

                adorn.TopOffset = pos.Y;
                //                GetDragAdorner(me).TopOffset = args.GetPosition(GetDragScope(me)).Y /* - _startPoint.Y */ ;
            }
        }


        static void source_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            SetStartPoint((DependencyObject)sender, e.GetPosition(null));
        }


        public static void DisposeDragging(this UIElement source)
        {
            if (source != null)
            {
                source.PreviewMouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(source_PreviewMouseLeftButtonDown);
                source.PreviewMouseMove -= new System.Windows.Input.MouseEventHandler(source_PreviewMouseMove);
            }
        }

    }
}
