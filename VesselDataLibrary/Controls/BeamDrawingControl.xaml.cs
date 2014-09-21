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
using VesselDataLibrary.Xml;
using RussLibrary;
using RussLibrary.Helpers;
namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for BeamDrawingControl.xaml
    /// </summary>
    public partial class BeamDrawingControl : UserControl
    {
        public BeamDrawingControl()
        {
            InitializeComponent();

            
        }


        public static readonly DependencyProperty WallRatioProperty =
          DependencyProperty.Register("WallRatio", typeof(double),
          typeof(BeamDrawingControl), new PropertyMetadata(0.25D));

        public double WallRatio
        {
            get
            {
                return (double)this.UIThreadGetValue(WallRatioProperty);

            }
            set
            {
                this.UIThreadSetValue(WallRatioProperty, value);

            }
        }

        void Beam_VectorItemChanged(object sender, EventArgs e)
        {
            
        }
       
        static void OnDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamDrawingControl me = sender as BeamDrawingControl;
            if (me != null)
            {
                
            }
        }
        static void OnBeamChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamDrawingControl me = sender as BeamDrawingControl;
            if (me != null)
            {
                BeamPort oldValue = e.OldValue as BeamPort;
                BeamPort newValue = e.NewValue as BeamPort;
                if (oldValue != null)
                {
                    oldValue.VectorItemChanged -= new EventHandler(me.Beam_VectorItemChanged);
                }
                if (newValue != null)
                {
                    newValue.VectorItemChanged += new EventHandler(me.Beam_VectorItemChanged);
                }
            }
        }
        public static readonly DependencyProperty BeamProperty =
            DependencyProperty.Register("Beam", typeof(BeamPort),
            typeof(BeamDrawingControl), new PropertyMetadata(OnBeamChanged));

        public BeamPort Beam
        {
            get
            {
                return (BeamPort)this.UIThreadGetValue(BeamProperty);

            }
            set
            {
                this.UIThreadSetValue(BeamProperty, value);

            }
        }
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
       
        }

        public static readonly DependencyProperty ArcSweepDirectionProperty =
        DependencyProperty.Register("ArcSweepDirection", typeof(SweepDirection),
        typeof(BeamDrawingControl), new PropertyMetadata(SweepDirection.Clockwise));

        public SweepDirection ArcSweepDirection
        {
            get
            {
                return (SweepDirection)this.UIThreadGetValue(ArcSweepDirectionProperty);

            }
            set
            {
                this.UIThreadSetValue(ArcSweepDirectionProperty, value);

            }
        }


        public static readonly DependencyProperty LeftLineXProperty =
          DependencyProperty.Register("LeftLineX", typeof(double),
          typeof(BeamDrawingControl), new PropertyMetadata(OnLinesChanged));

        public double LeftLineX
        {
            get
            {
                return (double)this.UIThreadGetValue(LeftLineXProperty);

            }
            set
            {
                this.UIThreadSetValue(LeftLineXProperty, value);

            }
        }



        public static readonly DependencyProperty LeftLineYProperty =
          DependencyProperty.Register("LeftLineY", typeof(double),
          typeof(BeamDrawingControl), new PropertyMetadata(OnLinesChanged));

        public double LeftLineY
        {
            get
            {
                return (double)this.UIThreadGetValue(LeftLineYProperty);

            }
            set
            {
                this.UIThreadSetValue(LeftLineYProperty, value);

            }
        }




        public static readonly DependencyProperty RightLineXProperty =
          DependencyProperty.Register("RightLineX", typeof(double),
          typeof(BeamDrawingControl), new PropertyMetadata(OnLinesChanged));

        public double RightLineX
        {
            get
            {
                return (double)this.UIThreadGetValue(RightLineXProperty);

            }
            set
            {
                this.UIThreadSetValue(RightLineXProperty, value);

            }
        }



        public static readonly DependencyProperty RightLineYProperty =
          DependencyProperty.Register("RightLineY", typeof(double),
          typeof(BeamDrawingControl), new PropertyMetadata(OnLinesChanged));

        public double RightLineY
        {
            get
            {
                return (double)this.UIThreadGetValue(RightLineYProperty);

            }
            set
            {
                this.UIThreadSetValue(RightLineYProperty, value);

            }
        }




        public static readonly DependencyProperty CenterLineXProperty =
          DependencyProperty.Register("CenterLineX", typeof(double),
          typeof(BeamDrawingControl), new PropertyMetadata(OnLinesChanged));

        public double CenterLineX
        {
            get
            {
                return (double)this.UIThreadGetValue(CenterLineXProperty);

            }
            set
            {
                this.UIThreadSetValue(CenterLineXProperty, value);

            }
        }

        static void OnLinesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamDrawingControl me = sender as BeamDrawingControl;
            if (me != null)
            {
                if (!me.IsUpdating)
                {
                    me.IsUpdating = true;
                    me.LeftBallX = me.LeftLineX + me.CenterLineX;
                    me.LeftBallY = me.LeftLineY + me.CenterLineY;

                    me.RightBallX = me.RightLineX + me.CenterLineX;
                    me.RightBallY = me.RightLineY + me.CenterLineY;
                    me.IsUpdating = false;
                }
            }
        }

        public static readonly DependencyProperty CenterLineYProperty =
          DependencyProperty.Register("CenterLineY", typeof(double),
          typeof(BeamDrawingControl), new PropertyMetadata(OnLinesChanged));

        public double CenterLineY
        {
            get
            {
                return (double)this.UIThreadGetValue(CenterLineYProperty);

            }
            set
            {
                this.UIThreadSetValue(CenterLineYProperty, value);

            }
        }



        public static readonly DependencyProperty LeftBallXProperty =
          DependencyProperty.Register("LeftBallX", typeof(double),
          typeof(BeamDrawingControl), new PropertyMetadata(OnLeftBallChanged));

        public double LeftBallX
        {
            get
            {
                return (double)this.UIThreadGetValue(LeftBallXProperty);

            }
            set
            {
                this.UIThreadSetValue(LeftBallXProperty, value);

            }
        }



        public static readonly DependencyProperty LeftBallYProperty =
          DependencyProperty.Register("LeftBallY", typeof(double),
          typeof(BeamDrawingControl), new PropertyMetadata(OnLeftBallChanged));

        public double LeftBallY
        {
            get
            {
                return (double)this.UIThreadGetValue(LeftBallYProperty);

            }
            set
            {
                this.UIThreadSetValue(LeftBallYProperty, value);

            }
        }



        static void OnLeftBallChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamDrawingControl me = sender as BeamDrawingControl;
            if (me != null)
            {
                if (!me.IsUpdating)
                {
                    me.IsUpdating = true;
                    me.LeftLineX = me.CenterLineX - me.LeftBallX;
                    me.LeftLineY = me.CenterLineY - me.LeftBallY;
                    me.IsUpdating = false;
                }
            }
        }
        bool IsUpdating = false;
        static void OnRightBallChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamDrawingControl me = sender as BeamDrawingControl;
            if (me != null)
            {
                if (!me.IsUpdating)
                {
                    me.IsUpdating = true;
                    me.RightLineX = me.RightBallX - me.CenterLineX;
                    me.RightLineY = me.RightBallY - me.CenterLineY;
                    me.IsUpdating = false;
                }
            }
        }

        public static readonly DependencyProperty RightBallXProperty =
          DependencyProperty.Register("RightBallX", typeof(double),
          typeof(BeamDrawingControl), new PropertyMetadata(OnRightBallChanged));

        public double RightBallX
        {
            get
            {
                return (double)this.UIThreadGetValue(RightBallXProperty);

            }
            set
            {
                this.UIThreadSetValue(RightBallXProperty, value);

            }
        }



        public static readonly DependencyProperty RightBallYProperty =
          DependencyProperty.Register("RightBallY", typeof(double),
          typeof(BeamDrawingControl), new PropertyMetadata(OnRightBallChanged));

        public double RightBallY
        {
            get
            {
                return (double)this.UIThreadGetValue(RightBallYProperty);

            }
            set
            {
                this.UIThreadSetValue(RightBallYProperty, value);

            }
        }






        public static readonly DependencyProperty ArcSizeProperty =
         DependencyProperty.Register("ArcSize", typeof(Size),
         typeof(BeamDrawingControl));

        public Size ArcSize
        {
            get
            {
                return (Size)this.UIThreadGetValue(ArcSizeProperty);

            }
            set
            {
                this.UIThreadSetValue(ArcSizeProperty, value);

            }
        }


        public static readonly DependencyProperty LargeArcProperty =
          DependencyProperty.Register("LargeArc", typeof(bool),
          typeof(BeamDrawingControl));

        public bool LargeArc
        {
            get
            {
                return (bool)this.UIThreadGetValue(LargeArcProperty);

            }
            set
            {
                this.UIThreadSetValue(LargeArcProperty, value);

            }
        }

        
        const double adjustment = 12;



        //void SetDragging(UIElement elem)
        //{
        //    if (elem != null)
        //    {
        //        elem.InitializeDragging(this);
        //        elem.SetDragTypes(typeof(Image));


        //    }
        //}
        private void uc_Loaded(object sender, RoutedEventArgs e)
        {


            //SetDragging(pointer1);
            //SetDragging(pointer2);
            //DragHelper.PreviewDragStarted += new EventHandler<DragStartedEventArgs>(DragHelper_PreviewDragStarted);
        }

        void DragHelper_PreviewDragStarted(object sender, DragStartedEventArgs e)
        {
            //FrameworkElement item = e.DragObject as FrameworkElement;
            //if (item != null)
            //{



            //    canvas.Children.Remove(item);
             
            //}
        }
       
        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(typeof(Image)))
            //{
                
            //    Image fmc = (Image)e.Data.GetData(typeof(Image));
            //    Point pnt = e.GetPosition(canvas);
            //    Point pnt2 = DragHelper.GetRelativeMousePoint(fmc);
            //    Canvas.SetTop(fmc, pnt.Y - pnt2.Y);
            //    Canvas.SetLeft(fmc, pnt.X - pnt2.X);
            //    if (fmc.Parent != canvas)
            //    {
            //        canvas.Children.Add(fmc);
            //    }

                
             

            //}
        }

      
        private void uc_Unloaded(object sender, RoutedEventArgs e)
        {
            //pointer1.DisposeDragging();
            //pointer2.DisposeDragging();
            //DragHelper.PreviewDragStarted -= new EventHandler<DragStartedEventArgs>(DragHelper_PreviewDragStarted);
        }
    }
}
