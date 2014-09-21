using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RussLibrary;
using System.Collections.ObjectModel;
using RussLibrary.Helpers;
namespace MissionStudio.Spacemap
{
    /// <summary>
    /// Interaction logic for Designer.xaml
    /// </summary>
    public partial class Designer : UserControl
    {
        Brush Gridline = new BrushConverter().ConvertFromString("#FF0000C0") as Brush;
        List<Line> VerticalLines = new List<Line>();
        List<Line> HorizontalLines = new List<Line>();
        Dictionary<KeyValuePair<byte, int>, TextBlock> Coords = new Dictionary<KeyValuePair<byte, int>, TextBlock>();
        public Designer()
        {
            CommandsAndConditions = new ObservableCollection<UnmappableObject>();
            LoadSpaceObjectTypes();
            LoadUnmappables();
            InitializeComponent();
            InitializeContextMenu();
            InitializeMap();
            DragHelper.PreviewDragStarted += new EventHandler<DragStartedEventArgs>(DragHelper_PreviewDragStarted);
        }
        ContextMenu ObjectMenu;
        void InitializeContextMenu()
        {
            ObjectMenu = new ContextMenu();
            MenuItem mnu = new MenuItem();
            mnu.Header = "Delete";
            mnu.Click += new RoutedEventHandler(mnuDelete_Click);
            ObjectMenu.Items.Add(mnu);
        }

        void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSpaceObject != null)
            {
                RemoveSpaceObjectEvents(SelectedSpaceObject);
                if (canvas.Children.Contains(SelectedSpaceObject))
                {
                    canvas.Children.Remove(SelectedSpaceObject);
                }

                UnmappableObject obj = SelectedSpaceObject as UnmappableObject;
                if (obj == null)
                {
                    SpaceObject sp = SelectedSpaceObject as SpaceObject;
                    if (sp != null)
                    {
                        obj = sp.Tag as UnmappableObject;
                    }
                }
                if (obj != null)
                {
                    if (CommandsAndConditions.Contains(obj))
                    {
                        CommandsAndConditions.Remove(obj);
                    }
                }
                SelectedSpaceObject = null;

            }
        }
        void LoadSpaceObjectTypes()
        {
            ObjectList = new ObservableCollection<SpaceObjectType>();
            foreach (SpaceObjectType v in Enum.GetValues(typeof(SpaceObjectType)))
            {
                if (v != SpaceObjectType.Other)
                {
                    ObjectList.Add(v);
                }
            }
        }
        void LoadUnmappables()
        {
            
            List<string> wrk = new List<string>();
            foreach (string cmd in Commands.Current.CommandDictionary.Keys)
            {
                if (cmd != "create" && cmd != "destroy_near")
                {
                    wrk.Add(cmd);
                }
            }
            wrk.Sort();
            Unmappables = new ObservableCollection<string>(wrk);
        }
        public void LoadScript(string script, int caretPosition)
        {
        }
        void InitializeMap()
        {
            for (int i = 0; i < 6; i++)
            {
                VerticalLines.Add(new Line());
                HorizontalLines.Add(new Line());
                VerticalLines[i].Stroke = Gridline;
                HorizontalLines[i].Stroke = Gridline;
                VerticalLines[i].StrokeThickness = 1;
                HorizontalLines[i].StrokeThickness = 1;

                canvas.Children.Add(VerticalLines[i]);
                canvas.Children.Add(HorizontalLines[i]);
            }
            for (byte i = 1; i < 6; i++)
            {
                for (int j = 1; j < 6; j++)
                {
                    string text = ((char)(i + 64)).ToString() + j.ToString();
                    TextBlock t = new TextBlock();
                    t.Text = text;
                    t.Foreground = Gridline;
                    t.FontFamily = new System.Windows.Media.FontFamily("GenericSansSerif");
                    Coords.Add(new KeyValuePair<byte, int>(i, j), t);
                    canvas.Children.Add(t);
                }

            }
        }
        

        public static readonly RoutedEvent ObjectAddedEvent =
            EventManager.RegisterRoutedEvent(
            "ObjectAdded", RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(Designer));

        public event RoutedEventHandler ObjectAdded
        {
            add { AddHandler(ObjectAddedEvent, value); }
            remove { RemoveHandler(ObjectAddedEvent, value); }
        }

        public static readonly RoutedEvent ObjectMovedEvent =
            EventManager.RegisterRoutedEvent(
            "ObjectMoved", RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(Designer));

        public event RoutedEventHandler ObjectMoved
        {
            add { AddHandler(ObjectMovedEvent, value); }
            remove { RemoveHandler(ObjectMovedEvent, value); }
        }

        
        void AddSpaceObjectEvents(Control so2)
        {
            so2.Loaded += new RoutedEventHandler(SpaceObject_loaded);
            so2.Unloaded += new RoutedEventHandler(SpaceObject_unloaded);
            so2.MouseDown += new MouseButtonEventHandler(SpaceObject_MouseDown);
            so2.MouseUp += new MouseButtonEventHandler(SpaceObject_MouseUp);
            so2.KeyDown+=new KeyEventHandler(Canvas_KeyDown);
            so2.KeyUp+=new KeyEventHandler(Canvas_KeyUp);
            Keyboard.AddKeyDownHandler(so2, Canvas_KeyDown);
            Keyboard.AddKeyUpHandler(so2, Canvas_KeyUp);
            SpaceObject so = so2 as SpaceObject;
            if (so != null)
            {
                so.LocationChanged += new EventHandler(SpaceObject_LocationChanged);
            }
            so2.ContextMenu = ObjectMenu;
        }

        void SpaceObject_LocationChanged(object sender, EventArgs e)
        {
            SpaceObject so = sender as SpaceObject;
            if (so != null)
            {

                if (!double.IsNaN(so.X) && !double.IsNaN(so.Z))
                {
                    double wrkX = so.X * zoomFactor;
                    double wrkY = so.Z * zoomFactor;
                    UnmappableObject removeObject = null;
                    foreach (UnmappableObject ob in CommandsAndConditions)
                    {

                        if (ob.MappableObject == so)
                        {
                            removeObject = ob;
                            break;
                        }
                    }
                    if (removeObject != null)
                    {
                        CommandsAndConditions.Remove(removeObject);
                    }
                    if (so.Parent != null)
                    {
                        ContentControl parnt = so.Parent as ContentControl;
                        if (parnt != null)
                        {
                            parnt.Content = null;

                        }

                    }
                    if (!canvas.Children.Contains(so))
                    {
                        canvas.Children.Add(so);
                    }
                    Canvas.SetTop(so, wrkY + canvasScroll.VerticalOffset);
                    Canvas.SetLeft(so, wrkX + canvasScroll.HorizontalOffset);
                }
                else
                {
                    //drop into unmappable canvas, if not already there.
                    if (canvas.Children.Contains(so))
                    {
                        canvas.Children.Remove(so);
                    }
                    UnmappableObject removeObject = null;
                    foreach (UnmappableObject ob in CommandsAndConditions)
                    {

                        if (ob.MappableObject == so)
                        {
                            removeObject = ob;
                            break;
                        }
                    }
                    if (removeObject == null)
                    {
                        removeObject = new UnmappableObject();
                        removeObject.CommandName = "create";
                        removeObject.Attributes = so.Attributes;
                        removeObject.MappableObject = so;
                        so.Tag = removeObject;
                        //SelectedSpaceObject = removeObject;
                        CommandsAndConditions.Add(removeObject);
                    }

                }
            }
        }

        public static readonly RoutedEvent ObjectDeletedEvent =
            EventManager.RegisterRoutedEvent(
            "ObjectDeleted", RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(Designer));

        public event RoutedEventHandler ObjectDeleted
        {
            add { AddHandler(ObjectDeletedEvent, value); }
            remove { RemoveHandler(ObjectDeletedEvent, value); }
        }

        void RemoveSpaceObjectEvents(Control so2)
        {
            so2.Loaded -= new RoutedEventHandler(SpaceObject_loaded);
            so2.Unloaded -= new RoutedEventHandler(SpaceObject_unloaded);
            so2.MouseDown -= new MouseButtonEventHandler(SpaceObject_MouseDown);
            so2.MouseUp -= new MouseButtonEventHandler(SpaceObject_MouseUp);
            Keyboard.RemoveKeyDownHandler(so2, Canvas_KeyDown);
            Keyboard.RemoveKeyUpHandler(so2, Canvas_KeyUp);
            SpaceObject spOb = so2 as SpaceObject;
            if (spOb != null)
            {
                spOb.LocationChanged -= new EventHandler(SpaceObject_LocationChanged);
            }

            so2.DisposeDragging();
            this.RaiseEvent(new RoutedEventArgs(ObjectDeletedEvent, so2));
        }
      


        public static readonly DependencyProperty SelectedSpaceObjectProperty =
        DependencyProperty.Register("SelectedSpaceObject", typeof(Control),
        typeof(Designer));
        public Control SelectedSpaceObject
        {
            get
            {
                return (Control)this.UIThreadGetValue(SelectedSpaceObjectProperty);

            }
            set
            {
                this.UIThreadSetValue(SelectedSpaceObjectProperty, value);

            }
        }


        public static readonly DependencyProperty CommandsAndConditionsProperty =
        DependencyProperty.Register("CommandsAndConditions", typeof(ObservableCollection<UnmappableObject>),
        typeof(Designer));
        public ObservableCollection<UnmappableObject> CommandsAndConditions
        {
            get
            {
                return (ObservableCollection<UnmappableObject>)this.UIThreadGetValue(CommandsAndConditionsProperty);

            }
            set
            {
                this.UIThreadSetValue(CommandsAndConditionsProperty, value);

            }
        }
        void AddBorderAdorner(FrameworkElement adornedElement)
        {
            if (adornedElement != null)
            {
                AdornerLayer layr = AdornerLayer.GetAdornerLayer(adornedElement);
                if (layr != null)
                {
                    BorderAdorner adr = new BorderAdorner(adornedElement);
                    adr.ToolTip = adornedElement.ToolTip;
                    adr.ContextMenu = adornedElement.ContextMenu;

                    layr.Add(adr);
                    SetDragging(adr);
                }
            }
        }
        void SpaceObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Control so = sender as Control;
            if (so != null)
            {
                SelectedSpaceObject = so;
                AddBorderAdorner(so);
            }
        }
        private void canvas_Drop(object sender, DragEventArgs e)
        {
      
            SpaceObject so = null;
            if (e.Data.GetDataPresent(typeof(SpaceObject)))
            {

                so = e.Data.GetData(typeof(SpaceObject)) as SpaceObject;
               
                

            }
            else if (e.Data.GetDataPresent(typeof(BorderAdorner)))
            {
                so = SelectedSpaceObject as SpaceObject;
            }
            else if (e.Data.GetDataPresent(typeof(GroupBox)))
            {
                GroupBox obj = e.Data.GetData(typeof(GroupBox)) as GroupBox;
                if (obj != null)
                {
                    UnmappableObject newObj = new UnmappableObject();
                    newObj.CommandName = obj.Tag as string;
                   
                    AddSpaceObjectEvents(newObj);
                 //Need only to add delete support at this point.
                    CommandsAndConditions.Add(newObj);
                }
            }

            if (so != null)
            {
                FrameworkElement dscope = DragHelper.GetDragScope(so);
                DragAdorner adorner = null;
                if (dscope != null)
                {
                    adorner = DragHelper.GetDragAdorner(dscope);
                }
                bool DoAdd = false;

                if (so.Tag == null)
                {

                    SpaceObject so2 = new SpaceObject();
                    so2.ObjectType = so.ObjectType;
                    so2.Tag = "xxx";
                    SetSpaceObjectSize(so2);
                    AddSpaceObjectEvents(so2);
                    //so2.ObjectName = "test";
                    so = so2;
                    //so.X = Canvas.GetLeft(so);
                    //so.Z = Canvas.GetTop(so);
                    //Raise added (needs to get to Xml))

                    DoAdd = true;
                }


                if (!canvas.Children.Contains(so))
                {
                    canvas.Children.Add(so);
                }
                //Point wrkP = DragHelper.GetRelativeMousePoint(so); no
                Point wrkP = e.GetPosition(canvas);
                if (adorner != null)
                {

                    //wrkP = new Point(
                    //    adorner.LeftOffset - (ItemPool.ActualWidth + gridSplit.ActualWidth) + canvasScroll.HorizontalOffset,
                    //    adorner.TopOffset + canvasScroll.VerticalOffset);


                    wrkP = new Point(
                        adorner.LeftOffset + canvasScroll.HorizontalOffset,
                        adorner.TopOffset + canvasScroll.VerticalOffset);
                }


                
                so.X = wrkP.X / zoomFactor;
                so.Z = wrkP.Y / zoomFactor;
                

                Canvas.SetTop(so, wrkP.Y);
                Canvas.SetLeft(so, wrkP.X);

                if (DoAdd)
                {
                    this.RaiseEvent(new RoutedEventArgs(ObjectAddedEvent, so));
                }
                else
                {
                    this.RaiseEvent(new RoutedEventArgs(ObjectMovedEvent, so));
                }
                //this.Dispatcher.BeginInvoke(new Action<SpaceObject>(SetSpaceObjectSize), System.Windows.Threading.DispatcherPriority.Loaded, so);


            }

        }
        void RemoveBorderAdorner(UIElement adornedElement)
        {
            if (adornedElement != null)
            {
                AdornerLayer layr = AdornerLayer.GetAdornerLayer(adornedElement);
                if (layr != null)
                {
                    Adorner[] adorners = layr.GetAdorners(adornedElement);
                    if (adorners != null)
                    {
                        foreach (Adorner adr in adorners)
                        {

                            BorderAdorner badr = adr as BorderAdorner;

                            if (badr != null)
                            {

                                layr.Remove(badr);
                                badr.DisposeDragging();
                            }
                        }
                    }
                }
            }
        }
        void SpaceObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedSpaceObject != null)
            {
                RemoveBorderAdorner(SelectedSpaceObject);
            }
        }
      
        void DragHelper_PreviewDragStarted(object sender, DragStartedEventArgs e)
        {
            
            SpaceObject s = e.DragObject as SpaceObject;

            if (s != null && s.Tag != null)
            {

                canvas.Children.Remove(s);
            }
            else
            {
                BorderAdorner badr = e.DragObject as BorderAdorner;
                if (badr != null)
                {
                    canvas.Children.Remove(SelectedSpaceObject);
                }
            }
        }
        void SetSpaceObjectSize(SpaceObject so)
        {
            double wrk = 12;
            if (so.ObjectType == SpaceObjectType.BlackHole || so.ObjectType == SpaceObjectType.Nebulas)
            {
                wrk = 48; // zoomFactor * 256 *24;
                
            }
            else
            {
              wrk = 24; // zoomFactor * 128 * 24;
                
            }
            //if (wrk <12)
            //{
            //    wrk = 12;

            //}
            so.ImageWidth = wrk;
       
        }

        public static readonly DependencyProperty UnmappablesListProperty =
            DependencyProperty.Register("Unmappables", typeof(ObservableCollection<string>),
            typeof(Designer));
        public ObservableCollection<string> Unmappables
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(UnmappablesListProperty);

            }
            set
            {
                this.UIThreadSetValue(UnmappablesListProperty, value);

            }
        }

        public static readonly DependencyProperty ObjectListProperty =
         DependencyProperty.Register("ObjectList", typeof(ObservableCollection<SpaceObjectType>),
         typeof(Designer));
        public ObservableCollection<SpaceObjectType> ObjectList
        {
            get
            {
                return (ObservableCollection<SpaceObjectType>)this.UIThreadGetValue(ObjectListProperty);

            }
            set
            {
                this.UIThreadSetValue(ObjectListProperty, value);

            }
        }
        void SetDragging(UIElement elem)
        {
            if (elem != null)
            {
                elem.InitializeDragging(canvas);
                elem.SetDragTypes(typeof(SpaceObject), typeof(BorderAdorner), typeof(UnmappableObject), typeof(GroupBox));


            }
        }
        private void uc_Unloaded(object sender, RoutedEventArgs e)
        {
            //RightPointer.DisposeDragging();
            //LeftPointer.DisposeDragging();
            //DragHelper.PreviewDragStarted -= new EventHandler<DragStartedEventArgs>(DragHelper_PreviewDragStarted);
        }
       
        private void SpaceObject_loaded(object sender, RoutedEventArgs e)
        {
            SetDragging(sender as UIElement);
        }

        private void SpaceObject_unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomOut();

        }
        void ZoomOut()
        {
            double divisor = 1.2;
            double w = canvas.ActualWidth / divisor;

            if (w >= canvas.MinWidth)
            {


                canvas.Width = w;
                canvas.Height = w;
                
                
            }
        }
        void ZoomIn()
        {
            double divisor = 1.2;
            canvas.Width = canvas.ActualWidth * divisor;
            canvas.Height = canvas.ActualHeight * divisor;
            
        }
       
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomIn();
         
        }
        double zoomFactor = 500 / MaxMapSize;
        const double MaxMapSize = 100000;
        private void uc_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double widthZoom = e.NewSize.Width / MaxMapSize;
            double heightZoom = e.NewSize.Height / MaxMapSize;
            double sizeChange = (e.NewSize.Width + e.PreviousSize.Width) / e.PreviousSize.Width;
            double baseFontSize = MaxMapSize / 25;
            zoomFactor = (widthZoom > heightZoom) ? heightZoom : widthZoom;

            double Limit = (canvas.ActualWidth < canvas.ActualHeight) ? canvas.ActualWidth : canvas.ActualHeight;

            //LInes
            for (int i = 0; i < 6; i++)
            {
                double dI = (double)i;
                Line l = VerticalLines[i];
                //Canvas.SetTop(l, i * 20000 * zoomFactor);


                l.X1 = dI * 20000 * zoomFactor;
                l.Y1 = 0;
                l.X2 = l.X1;

                l.Y2 = Limit;

                l = HorizontalLines[i];
                l.X1 = 0;
                l.Y1 = dI * 20000 * zoomFactor;
                l.X2 = Limit;
                l.Y2 = l.Y1;


            }

            //Letter NUmbers
            for (int i = 1; i < 6; i++)
            {
                for (byte j = 1; j < 6; j++)
                {
                    KeyValuePair<byte, int> key = new KeyValuePair<byte, int>(j, i);

                    TextBlock t = Coords[key];
                    double dI = (double)(i - 1) * 20000 * zoomFactor;
                    double dJ = (double)(j - 1) * 20000 * zoomFactor;


                    Canvas.SetLeft(t, dI);
                    Canvas.SetTop(t, dJ);
                    try
                    {
                        t.FontSize = baseFontSize * zoomFactor;
                    }
                    catch (ArgumentException)
                    { }
                }

            }
            //Now go through each child, which must use an interface with x, z to scale.
            foreach (object obj in canvas.Children)
            {
                SpaceObject mapItem = obj as SpaceObject;
                if (mapItem != null)
                {

                    double oldX = Canvas.GetLeft(mapItem);
                    double oldY = Canvas.GetTop(mapItem);//sizeChange
                    double newX = mapItem.X * zoomFactor; // (mapItem.X * LocateMultiplier) / (MaxMapSize * zoomFactor);
                    double newY = mapItem.Z * zoomFactor;// (mapItem.Z * LocateMultiplier) / (MaxMapSize * zoomFactor);
                    Canvas.SetLeft(mapItem, newX);
                    Canvas.SetTop(mapItem, newY);
                }
            }

        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (CtlIsDown)
            {
                if (e.Delta < 0)
                {
                    ZoomIn();
                }
                else
                {
                    ZoomOut();
                }
                e.Handled = true;
            }
        }
        bool CtlIsDown = false;
        bool ShiftIsDown = false;
        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            //{
            //    CtlIsDown = true;
            //}
            //if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            //{
            //    ShiftIsDown = true;
            //}
            
        }

        private void Canvas_KeyUp(object sender, KeyEventArgs e)
        {
           
            //if (e.Key == Key.Delete)
            //{
            //    if (SelectedSpaceObject != null)
            //    {
            //        RemoveSpaceObjectEvents(SelectedSpaceObject);
            //        canvas.Children.Remove(SelectedSpaceObject);
            //        SelectedSpaceObject = null;
               
            //    }
            //}
            //if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            //{
            //    CtlIsDown = false;
            //}
            //if (e.Key == Key.RightShift || e.Key == Key.LeftShift)
            //{
            //    ShiftIsDown = false;
            //}
        }
    }
}
