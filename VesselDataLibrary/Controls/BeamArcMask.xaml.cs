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
using log4net;
using System.Reflection;
using RussLibrary.Helpers;

namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for BeamArcMask.xaml
    /// </summary>
    public partial class BeamArcMask : UserControl
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(BeamArcMask));
        public BeamArcMask()
        {
            InitializeComponent();
        }
      


        public static readonly DependencyProperty HighlighterProperty =
          DependencyProperty.Register("Highlighter", typeof(bool),
          typeof(BeamArcMask));

        public bool Highlighter
        {
            get
            {
                return (bool)this.UIThreadGetValue(HighlighterProperty);

            }
            set
            {
                this.UIThreadSetValue(HighlighterProperty, value);

            }
        }


        public static readonly DependencyProperty DraggerProperty =
          DependencyProperty.Register("Dragger", typeof(bool),
          typeof(BeamArcMask));

        public bool Dragger
        {
            get
            {
                return (bool)this.UIThreadGetValue(DraggerProperty);

            }
            set
            {
                this.UIThreadSetValue(DraggerProperty, value);

            }
        }







        public static readonly DependencyProperty LeftPointXProperty =
          DependencyProperty.Register("LeftPointX", typeof(double),
          typeof(BeamArcMask), new PropertyMetadata(OnLeftPointChange));

        public double LeftPointX
        {
            get
            {
                return (double)this.UIThreadGetValue(LeftPointXProperty);

            }
            set
            {
                this.UIThreadSetValue(LeftPointXProperty, value);

            }
        }



        public static readonly DependencyProperty LeftPointYProperty =
          DependencyProperty.Register("LeftPointY", typeof(double),
          typeof(BeamArcMask), new PropertyMetadata(OnLeftPointChange));

        public double LeftPointY
        {
            get
            {
                return (double)this.UIThreadGetValue(LeftPointYProperty);

            }
            set
            {
                this.UIThreadSetValue(LeftPointYProperty, value);

            }
        }




        public static readonly DependencyProperty RightPointXProperty =
          DependencyProperty.Register("RightPointX", typeof(double),
          typeof(BeamArcMask), new PropertyMetadata(OnRightPointChange));

        public double RightPointX
        {
            get
            {
                return (double)this.UIThreadGetValue(RightPointXProperty);

            }
            set
            {
                this.UIThreadSetValue(RightPointXProperty, value);

            }
        }



        public static readonly DependencyProperty RightPointYProperty =
          DependencyProperty.Register("RightPointY", typeof(double),
          typeof(BeamArcMask), new PropertyMetadata(OnRightPointChange));

        public double RightPointY
        {
            get
            {
                return (double)this.UIThreadGetValue(RightPointYProperty);

            }
            set
            {
                this.UIThreadSetValue(RightPointYProperty, value);

            }
        }

        bool MouseIsDown = false;

        bool LeftLineMoving = false;
        bool RightLineMoving = false;
        static void OnLeftPointChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamArcMask me = sender as BeamArcMask;
            if (me != null)
            {
                if (me.LeftLineMoving)
                {
                    //Set LeftLine



                    me.RecalculateRange(me.LeftPointX, me.LeftPointY);
                    me.RecalculateArc(me.LeftPointX, true);
                    
                }
            }
        }
        static void OnLeftLineChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamArcMask me = sender as BeamArcMask;
            if (me != null)
            {
                if (!me.LeftLineMoving)
                {
                    //Set LeftPoint
                    me.LeftPointX = me.LeftLineX + me.CenterX;
                    me.LeftPointY = me.LeftLineY + me.CenterY;
                }
            }
        }

        static void OnRightPointChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamArcMask me = sender as BeamArcMask;
            if (me != null)
            {
                if (me.RightLineMoving)
                {
                    //Set RightLine
                    me.RecalculateRange(me.RightPointX, me.RightPointY);
                    me.RecalculateArc(me.RightPointX, false);
                    
                }
            }
        }
        static void OnRightLineChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamArcMask me = sender as BeamArcMask;
            if (me != null)
            {
                if (!me.RightLineMoving)
                {
                    //Set RightPoint
                    me.RightPointX = me.RightLineX + me.CenterX;
                    me.RightPointY = me.RightLineY + me.CenterY;
                }
            }
        }
        

        void RecalculateRange(double LineX, double LineY)
        {
            double x = LineX- CenterX;

            double y = LineY - CenterY;

            double u2 = Math.Sqrt(Math.Pow(x, 2)
                + Math.Pow(y, 2));
            //Range = LineX;

            //double ToWall = Math.Sqrt(Math.Pow(CenterWallX, 2) + Math.Pow(CenterWallY, 2));
            if (!double.IsNaN(u2) && !double.IsNegativeInfinity(u2) && !double.IsPositiveInfinity(u2))
            {
                double r = Convert.ToInt32(u2) / WallRatio;
                if (r < 5000)
                {
                    Range = r;
                }
            }
        }
        void RecalculateArc(double LineX, bool IsLeftSide)
        {


            double adjuster = IsLeftSide ? 1 : -1;

            double relativeX = Math.Atan(CenterWallY / CenterWallX);

            double negator = (CenterWallX < 0 && Math.Cos(relativeX) > 0) ? -1 : 1;

            //double LeftX = LeftLineX;
            double x = LineX - CenterX;
            //ArcWidth = adjuster * (Math.Acos(x / (multPler - relativeX)) / Math.PI);


            //TODO: Get this working!!!
            //  test: using LeftLineX and LeftLineY calculations, iterate through ArcWidth changes from 0 to 1
            //      and look for patterns, then do reverse and find what is wrong.
            //
            //      markers are currently not visible.
            ArcWidth = adjuster * (Math.Acos(x / (WallRatio * Range * negator)) - relativeX) / Math.PI;
            if (ArcWidth < 0)
            {
                ArcWidth = 0;
            }
            if (ArcWidth > 1)
            {
                ArcWidth = 1;
            }

        }
        const double RelativeWall = 1500;
        void CalculateRightLine()
        {

            double relativeX = Math.Atan(CenterWallY / CenterWallX);


            RightLineX = WallRatio * Range *
                Math.Cos(-ArcWidth * Math.PI + relativeX)
                * ((CenterWallX < 0 && Math.Cos(relativeX) > 0) ? -1 : 1);

            RightLineY = WallRatio * Range *
              Math.Sin(-ArcWidth * Math.PI + relativeX)
              * (((CenterWallY > 0 && Math.Sin(relativeX) < 0) || (CenterWallY < 0 && Math.Sin(relativeX) > 0)) ? -1 : 1);



        }


        void CalculateLeftLine()
        {


            double relativeX = Math.Atan(CenterWallY / CenterWallX);

        
            LeftLineX = WallRatio * Range *
                Math.Cos(ArcWidth * Math.PI + relativeX)
                * ((CenterWallX < 0 && Math.Cos(relativeX) > 0) ? -1 : 1);

            LeftLineY = WallRatio * Range *
              Math.Sin(ArcWidth * Math.PI + relativeX)
              * (((CenterWallY > 0 && Math.Sin(relativeX) < 0) || (CenterWallY < 0 && Math.Sin(relativeX) > 0)) ? -1 : 1);


        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Double.ToString")]
        void DrawArc()
        {


            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }


            //
            //  Odd behavior when z is zero and x < zero: points get switched. 
            //  this code switches them back.
            if (Z == 0 && X < 0)
            {
                double wrk = RightLineY;
                RightLineY = LeftLineY;
                LeftLineY = wrk;

            }

            PathGeometry pathGeometry = new PathGeometry();

            PathFigure figure = new PathFigure();
           
            figure.StartPoint = new Point(RightLineX + CenterX, RightLineY + CenterY);

           
           
            Point EndPoint = new Point(LeftLineX + CenterX, LeftLineY + CenterY);
            if (ArcWidth == 1)
            {
              //@@@@

                double relativeX = Math.Atan(CenterWallY / CenterWallX);
                double wrkArc = 0.999999D;

                double wrkX = WallRatio * Range *
                    Math.Cos(wrkArc * Math.PI + relativeX)
                    * ((CenterWallX < 0 && Math.Cos(relativeX) > 0) ? -1 : 1);

                double wrkY = WallRatio * Range *
                  Math.Sin(wrkArc * Math.PI + relativeX)
                  * (((CenterWallY > 0 && Math.Sin(relativeX) < 0) || (CenterWallY < 0 && Math.Sin(relativeX) > 0)) ? -1 : 1);

                EndPoint = new Point(wrkX + CenterX, wrkY + CenterY);






                wrkX = WallRatio * Range *
                    Math.Cos(-wrkArc * Math.PI + relativeX)
                    * ((CenterWallX < 0 && Math.Cos(relativeX) > 0) ? -1 : 1);

                wrkY = WallRatio * Range *
                  Math.Sin(-wrkArc * Math.PI + relativeX)
                  * (((CenterWallY > 0 && Math.Sin(relativeX) < 0) || (CenterWallY < 0 && Math.Sin(relativeX) > 0)) ? -1 : 1);

                figure.StartPoint = new Point(wrkX + CenterX, wrkY + CenterY);

            }
            Size arcSize = new Size(Range * WallRatio, Range * WallRatio);
            bool largeArc = (ArcWidth > 0.5);


            if (_log.IsInfoEnabled)
            {
                _log.InfoFormat("~~~~~~~~~~~~~~~~WallRatio={0}, X={1}, Z={2}", WallRatio.ToString(), X.ToString(), Z.ToString());
                _log.InfoFormat("Rendering arc.  RightLineX={0}, RightLineY={1}, LeftLineX={2}, LeftLineY={3}, Range={4}, largeArc={5}, arcWidth={6}",
                    RightLineX.ToString(), RightLineY.ToString(), LeftLineX.ToString(),
                    LeftLineY.ToString(), Range.ToString(), largeArc.ToString(),
                    ArcWidth.ToString());
                _log.InfoFormat("Start Point=({0},{1}), End Point=({2},{3})",
                    figure.StartPoint.X.ToString(),
                    figure.StartPoint.Y.ToString(),
                    EndPoint.X.ToString(),
                    EndPoint.Y.ToString());

            }
            
            figure.Segments.Add(
                new ArcSegment(
                    EndPoint,
                    arcSize, 0,
                    largeArc,
                    SweepDirection.Clockwise,
                    true));


            Brush FillBrush = null;
            double strokeThickness = 1;
            if (Highlighter)
            {
                strokeThickness = 3;
                BrushConverter cnv = new BrushConverter();
                string wrk = cnv.ConvertToString(this.SideLinesBrush);
                FillBrush = new BrushConverter().ConvertFromString("#70" + wrk.Substring(3)) as Brush;


                List<Point> points = new List<Point>();
                points.Add(new Point(CenterX, CenterY));
                points.Add(new Point(LeftLineX + CenterX, LeftLineY + CenterY));
                points.Add(new Point(RightLineX + CenterX, RightLineY + CenterY));
                myTriangle.Points = new PointCollection(points);
                
                myTriangle.Fill = FillBrush;
                myTriangle.Visibility = Visibility.Visible;

                

            }
            else
            {
                myTriangle.Visibility = Visibility.Collapsed;
            }


            LeftLine.StrokeThickness = strokeThickness;
            CenterLine.StrokeThickness = strokeThickness;
            RightLine.StrokeThickness = strokeThickness;
            pathGeometry.Figures.Add(figure);

            myArc.Data = pathGeometry;
            
            myArc.Fill = FillBrush;
            myArc.Stroke = this.SideLinesBrush;
            myArc.StrokeThickness = strokeThickness;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        }

        static void OnArcWidthChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamArcMask me = sender as BeamArcMask;
            if (me != null)
            {

                me.CalculateLines();

                me.DrawArc();
            }
        }

        static void OnRangeChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamArcMask me = sender as BeamArcMask;
            if (me != null)
            {

                me.CalculateLines();


                me.DrawArc();
            }
        }
        static void OnDataChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamArcMask me = sender as BeamArcMask;
            if (me != null)
            {

                me.SetCenterImagePosition();
                me.DrawCenterLine();
                me.CalculateLines();
                me.DrawArc();
            }
        }

        void DrawCenterLine()
        {

            double baseX = X;
            double baseZ = Z;


            if ((double.IsNaN(baseZ) && double.IsNaN(baseX)) || (baseZ == 0 && baseX == 0))
            {
                baseZ = 1;
            }



            double RatioX = 0;
            
            RatioX = CenterX / baseX;
           

            double RatioY = 0;
           
            RatioY = CenterY / baseZ;
           

            double RatioToUse = 0;
            if (Math.Abs(RatioX) > Math.Abs(RatioY))
            {
                RatioToUse = Math.Abs(RatioY);
            }
            else
            {
                RatioToUse = Math.Abs(RatioX);
            }


            if (double.IsPositiveInfinity(RatioToUse))
            {
                CenterWallX = CenterX;
                CenterWallY = CenterY;

            }
            else
            {



                double adjY = 1;
                if (RatioY < 0)
                {
                    RatioY = -1;
                }

                double x = baseX * RatioToUse; // +zeroPointX;
                double y = -baseZ * RatioToUse * adjY; // +zeroPointY;


                //how to draw to control edge?
                if (!double.IsNaN(x) && !double.IsNegativeInfinity(x) && !double.IsPositiveInfinity(x))
                {
                    CenterWallX = x;
                }
                if (!double.IsNaN(y) && !double.IsNegativeInfinity(y) && !double.IsPositiveInfinity(y))
                {
                    CenterWallY = y;
                }
            }

        }
        void SetCenterImagePosition()
        {
           
            CenterY = this.ActualHeight / 2;
            CenterX = this.ActualWidth / 2;

        }

        public static readonly DependencyProperty XProperty =
           DependencyProperty.Register("X", typeof(double),
           typeof(BeamArcMask), new PropertyMetadata(OnDataChange));

        public double X
        {
            get
            {
                return (double)this.UIThreadGetValue(XProperty);

            }
            set
            {
                this.UIThreadSetValue(XProperty, value);

            }
        }

        public static readonly DependencyProperty ZProperty =
          DependencyProperty.Register("Z", typeof(double),
          typeof(BeamArcMask), new PropertyMetadata(OnDataChange));

        public double Z
        {
            get
            {
                return (double)this.UIThreadGetValue(ZProperty);

            }
            set
            {
                this.UIThreadSetValue(ZProperty, value);

            }
        }




        public static readonly DependencyProperty CenterXProperty =
          DependencyProperty.Register("CenterX", typeof(double),
          typeof(BeamArcMask));

        public double CenterX
        {
            get
            {
                return (double)this.UIThreadGetValue(CenterXProperty);

            }
            set
            {
                this.UIThreadSetValue(CenterXProperty, value);

            }
        }


        public static readonly DependencyProperty CenterYProperty =
          DependencyProperty.Register("CenterY", typeof(double),
          typeof(BeamArcMask));

        public double CenterY
        {
            get
            {
                return (double)this.UIThreadGetValue(CenterYProperty);

            }
            set
            {
                this.UIThreadSetValue(CenterYProperty, value);

            }
        }




        public static readonly DependencyProperty CenterWallXProperty =
          DependencyProperty.Register("CenterWallX", typeof(double),
          typeof(BeamArcMask));

        public double CenterWallX
        {
            get
            {
                return (double)this.UIThreadGetValue(CenterWallXProperty);

            }
            set
            {
                this.UIThreadSetValue(CenterWallXProperty, value);

            }
        }



        public static readonly DependencyProperty CenterWallYProperty =
          DependencyProperty.Register("CenterWallY", typeof(double),
          typeof(BeamArcMask));

        public double CenterWallY
        {
            get
            {
                return (double)this.UIThreadGetValue(CenterWallYProperty);

            }
            set
            {
                this.UIThreadSetValue(CenterWallYProperty, value);

            }
        }





        public static readonly DependencyProperty ArcWidthProperty =
          DependencyProperty.Register("ArcWidth", typeof(double),
          typeof(BeamArcMask), new PropertyMetadata(OnArcWidthChange));

        public double ArcWidth
        {
            get
            {
                return (double)this.UIThreadGetValue(ArcWidthProperty);

            }
            set
            {
                this.UIThreadSetValue(ArcWidthProperty, value);

            }
        }

        static void OnWallRatioChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamArcMask me = sender as BeamArcMask;
            if (me != null)
            {
                me.CalculateLines();
                me.DrawArc();
            }
        }

        public static readonly DependencyProperty WallRatioProperty =
          DependencyProperty.Register("WallRatio", typeof(double),
          typeof(BeamArcMask), new PropertyMetadata(OnWallRatioChanged));

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



        public static readonly DependencyProperty RangeProperty =
          DependencyProperty.Register("Range", typeof(double),
          typeof(BeamArcMask), new PropertyMetadata(OnRangeChange));

        public double Range
        {
            get
            {
                return (double)this.UIThreadGetValue(RangeProperty);

            }
            set
            {
                this.UIThreadSetValue(RangeProperty, value);

            }
        }


        public static readonly DependencyProperty LeftLineXProperty =
          DependencyProperty.Register("LeftLineX", typeof(double),
          typeof(BeamArcMask), new PropertyMetadata(OnLeftLineChange));

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
          typeof(BeamArcMask), new PropertyMetadata(OnLeftLineChange));

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
          typeof(BeamArcMask), new PropertyMetadata(OnRightLineChange));

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
          typeof(BeamArcMask), new PropertyMetadata(OnRightLineChange));

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
        static void SetDragging(UIElement elem)
        {
            //if (elem != null)
            //{
            //    elem.InitializeDragging(this);
            //    elem.SetDragTypes(typeof(Image));


            //}
        }
        private void uc_Unloaded(object sender, RoutedEventArgs e)
        {
            //RightPointer.DisposeDragging();
            //LeftPointer.DisposeDragging();
            //DragHelper.PreviewDragStarted -= new EventHandler<DragStartedEventArgs>(DragHelper_PreviewDragStarted);
        }
        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            SetDragging(RightPointer);
            SetDragging(LeftPointer);
            SetCenterImagePosition();
            DrawCenterLine();

        }
        void CalculateLines()
        {

            CalculateLeftLine();
            CalculateRightLine(); 
        }
        private void uc_SizeChanged(object sender, SizeChangedEventArgs e)
        {
          
            SetCenterImagePosition();
            DrawCenterLine();
            CalculateLines();
            DrawArc();
           

        }
        static void OnSideLinesBrushChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamArcMask me = sender as BeamArcMask;
            if (me != null)
            {
                me.DrawArc();
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SideLines")]
        public static readonly DependencyProperty SideLinesBrushProperty =
          DependencyProperty.Register("SideLinesBrush", typeof(Brush),
          typeof(BeamArcMask), new PropertyMetadata(Brushes.Cyan, OnSideLinesBrushChanged));

        public Brush SideLinesBrush
        {
            get
            {
                return (Brush)this.UIThreadGetValue(SideLinesBrushProperty);

            }
            set
            {
                this.UIThreadSetValue(SideLinesBrushProperty, value);

            }
        }



        public static readonly DependencyProperty CenterLineBrushProperty =
          DependencyProperty.Register("CenterLineBrush", typeof(Brush),
          typeof(BeamArcMask), new PropertyMetadata(Brushes.Yellow));

        public Brush CenterLineBrush
        {
            get
            {
                return (Brush)this.UIThreadGetValue(CenterLineBrushProperty);

            }
            set
            {
                this.UIThreadSetValue(CenterLineBrushProperty, value);

            }
        }
        Point startPoint = new Point();
        private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);

            Vector diff = startPoint - mousePos;


            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.MouseIsDown = true;
                FrameworkElement elem = sender as FrameworkElement;

                if (elem.Tag.ToString() == "Right")
                {
                    RightLineMoving = true;
                    Point workPoint = e.GetPosition(this.RightLine);
                    RightPointX = workPoint.X + CenterX;
                    RightPointY = workPoint.Y + CenterY;
                    RightLineMoving = false;
                }
                else
                {
                    LeftLineMoving = true;
                    Point workPoint = e.GetPosition(this.LeftLine);
                    LeftPointX = workPoint.X + CenterX;
                    LeftPointY = workPoint.Y + CenterY;
                    LeftLineMoving = false;
                }



            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseIsDown = false;
        }

        private void RightPointer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (MouseIsDown)
            {
                Image_MouseMove(sender, e);
            }
        }

    }
}
