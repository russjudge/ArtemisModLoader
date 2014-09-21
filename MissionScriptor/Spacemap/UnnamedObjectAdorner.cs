using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using RussLibrary;
using System.Windows.Media;
namespace MissionStudio.Spacemap
{
    public class UnnamedObjectAdorner : Adorner
    {
        public UnnamedObjectAdorner(UIElement adornedElement)
            : base(adornedElement)
        { }
        static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DependencyObject me = sender as UnnamedObjectAdorner;
            if (me != null)
            {

            }
        }
        protected override void OnRender(
             System.Windows.Media.DrawingContext drawingContext)
        {
            if (drawingContext != null)
            {

                //Only use either:
                // End coordinates
                //or 
                // use start angle, end angle, radius

                //random range used in either case.

                // TODO: auto clear of contrary settings needed.

                if (radius == 0)
                {
                    //draw two half-circles, with a rectangle in the middle.

                    //drawingContext.DrawRectangle(null, new Pen(Brushes.AliceBlue, 1),
                    //        new Rect(new Point(0, 0), DesiredSize));

                    PathGeometry pathGeometry = new PathGeometry();

                    //TODO: Adjust angle based on destination.  below assumes 45 degree angle.

                    //End point will be at exactly 180 degrees from start point (.5 radians).
                    //point 0,0 == upper left corner of object?


                    double x1 = 0; //startX
                    double y1 = 0;  //startY
                    double x2 = endX - startX;
                    double y2 = endY - startY;
                    double x1a = x1 + Math.Cos(0.5 * Math.PI + x2) * randomRange;

                    double x2a = x1a + x2;


                    double y1a = y1 + Math.Sin(0.5 * Math.PI + y2) * randomRange;
                    //double y1a = ??2 * randomRange;
                    double y2a = y1a + y2;


                    double x1b = -x1a;
                    double y1b = -y1a;

                    double x2b = -x2a;
                    double y2b = -y2a;



                    // Center point found--now adjust for range and angle.
                    //rectangle does not work--due to strange angles.
                    LineSegment ls = new LineSegment(new Point(x2a, y2a), true);

                    PathFigure figure = new PathFigure();
                    figure.StartPoint = new Point(x1a, y1a);

                    figure.Segments.Add(ls);
                    pathGeometry.Figures.Add(figure);


                    ls = new LineSegment(new Point(x2b, y2b), true);
                    figure = new PathFigure();
                    figure.StartPoint = new Point(x1b, y1b);
                    figure.Segments.Add(ls);
                    pathGeometry.Figures.Add(figure);

                    //figure.Segments.Add(
                    ArcSegment arc = new ArcSegment(new Point(x1b, y1b),
                        new Size(randomRange, randomRange),
                        Math.Acos(y1a / randomRange), false, SweepDirection.Clockwise, true);


                    figure = new PathFigure();

                    figure.StartPoint = new Point(x1a, y1a);
                    figure.Segments.Add(arc);
                    pathGeometry.Figures.Add(figure);

                    arc = new ArcSegment(new Point(x2b, y2b),
                       new Size(randomRange, randomRange),
                       Math.Acos(y1a / randomRange), false, SweepDirection.Counterclockwise, true);

                    figure = new PathFigure();

                    figure.StartPoint = new Point(x2a, y2a);
                    figure.Segments.Add(arc);


                    pathGeometry.Figures.Add(figure);


                    drawingContext.DrawGeometry(null, new Pen(Brushes.AliceBlue, 1), pathGeometry);


                }
                else
                {
                }

                base.OnRender(drawingContext);
            }
        }
        public static readonly DependencyProperty typeProperty =
           DependencyProperty.Register("type", typeof(string),
           typeof(UnnamedObjectAdorner));
        public string type
        {
            get
            {
                return (string)this.UIThreadGetValue(typeProperty);

            }
            set
            {
                this.UIThreadSetValue(typeProperty, value);

            }
        }
        public static readonly DependencyProperty countProperty =
          DependencyProperty.Register("count", typeof(int),
          typeof(UnnamedObjectAdorner));
        public int count
        {
            get
            {
                return (int)this.UIThreadGetValue(countProperty);

            }
            set
            {
                this.UIThreadSetValue(countProperty, value);

            }
        }
        public static readonly DependencyProperty radiusProperty =
        DependencyProperty.Register("radius", typeof(int),
        typeof(UnnamedObjectAdorner));
        public int radius
        {
            get
            {
                return (int)this.UIThreadGetValue(radiusProperty);

            }
            set
            {
                this.UIThreadSetValue(radiusProperty, value);

            }
        }
        public static readonly DependencyProperty randomRangeProperty =
       DependencyProperty.Register("randomRange", typeof(int),
       typeof(UnnamedObjectAdorner));
        public int randomRange
        {
            get
            {
                return (int)this.UIThreadGetValue(randomRangeProperty);

            }
            set
            {
                this.UIThreadSetValue(randomRangeProperty, value);

            }
        }
        public static readonly DependencyProperty startXProperty =
            DependencyProperty.Register("startX", typeof(int),
            typeof(UnnamedObjectAdorner));
        public int startX
        {
            get
            {
                return (int)this.UIThreadGetValue(startXProperty);

            }
            set
            {
                this.UIThreadSetValue(startXProperty, value);

            }
        }
        public static readonly DependencyProperty startYProperty =
            DependencyProperty.Register("startY", typeof(int),
            typeof(UnnamedObjectAdorner));
        public int startY
        {
            get
            {
                return (int)this.UIThreadGetValue(startYProperty);

            }
            set
            {
                this.UIThreadSetValue(startYProperty, value);

            }
        }
        public static readonly DependencyProperty startZProperty =
            DependencyProperty.Register("startZ", typeof(int),
            typeof(UnnamedObjectAdorner));
        public int startZ
        {
            get
            {
                return (int)this.UIThreadGetValue(startZProperty);

            }
            set
            {
                this.UIThreadSetValue(startZProperty, value);

            }
        }
        public static readonly DependencyProperty endXProperty =
            DependencyProperty.Register("endX", typeof(int),
            typeof(UnnamedObjectAdorner));
        public int endX
        {
            get
            {
                return (int)this.UIThreadGetValue(endXProperty);

            }
            set
            {
                this.UIThreadSetValue(endXProperty, value);

            }
        }
        public static readonly DependencyProperty endYProperty =
            DependencyProperty.Register("endY", typeof(int),
            typeof(UnnamedObjectAdorner));
        public int endY
        {
            get
            {
                return (int)this.UIThreadGetValue(endYProperty);

            }
            set
            {
                this.UIThreadSetValue(endYProperty, value);

            }
        }
        public static readonly DependencyProperty endZProperty =
            DependencyProperty.Register("endZ", typeof(int),
            typeof(UnnamedObjectAdorner));
        public int endZ
        {
            get
            {
                return (int)this.UIThreadGetValue(endZProperty);

            }
            set
            {
                this.UIThreadSetValue(endZProperty, value);

            }
        }
        public static readonly DependencyProperty startAngleProperty =
           DependencyProperty.Register("startAngle", typeof(int),
           typeof(UnnamedObjectAdorner));
        public int startAngle
        {
            get
            {
                return (int)this.UIThreadGetValue(startAngleProperty);

            }
            set
            {
                this.UIThreadSetValue(startAngleProperty, value);

            }
        }

        public static readonly DependencyProperty endAngleProperty =
            DependencyProperty.Register("endAngle", typeof(int),
            typeof(UnnamedObjectAdorner));
        public int endAngle
        {
            get
            {
                return (int)this.UIThreadGetValue(endAngleProperty);

            }
            set
            {
                this.UIThreadSetValue(endAngleProperty, value);

            }
        }



        //    COMMAND: create (the command that creates UNnamed objects in the game)
    //ATTRIBUTE: type
    //        VALID: nebulas, asteroids, mines
			
    //ATTRIBUTE: count
    //        VALID: 	0 to 500
    //ATTRIBUTE: radius
    //        VALID: 	0 to 100000
    //ATTRIBUTE: randomRange
    //        VALID: 	0 to 100000
    //ATTRIBUTE: startX
    //        VALID: 	 0 to 100000
    //ATTRIBUTE: startY
    //        VALID:  -100000 to 100000	
    //ATTRIBUTE: startZ
    //        VALID: 	 0 to 100000
    //ATTRIBUTE: use_gm_position
    //        VALID: anything, just use this attribute to cause the startX,startY,startZ to be at the game master's selected position
			
    //ATTRIBUTE: endX
    //        VALID: 	 0 to 100000
    //ATTRIBUTE: endY
    //        VALID: 	-100000 to 100000
    //ATTRIBUTE: endZ
    //        VALID: 	 0 to 100000
    //ATTRIBUTE: randomSeed
    //        VALID: 	0 to big number
    //ATTRIBUTE: startAngle
    //        VALID: 	0 to 360
    //ATTRIBUTE: endAngle
    //        VALID: 	0 to 360
    }
}
