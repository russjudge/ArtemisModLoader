using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandboxConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //LeftLineX = WallRatio * Range *
            //   Math.Cos(ArcWidth * Math.PI + Math.Atan(CenterWallY / CenterWallX))
            //   * ((CenterWallX < 0 && Math.Cos(Math.Atan(CenterWallY / CenterWallX)) > 0) ? -1 : 1);
            double CenterWallX = 22;
            double CenterWallY = 13;

            double relativeX = Math.Atan(CenterWallY / CenterWallX);

            double negator = (CenterWallX < 0 && Math.Cos(relativeX) > 0) ? -1 : 1;



            double WallRatio = .07;
            double Range = 1500;
            double ArcWidth = .35;
          
            double LeftLineX =  WallRatio * Range *
               Math.Cos(ArcWidth * Math.PI + relativeX)
               * negator;

            //Now: solve for arcwidth:
            
            


            //ArcWidth = adjuster * (Math.Acos(x / (multPler - relativeX)) / Math.PI);
            ArcWidth = (Math.Acos(LeftLineX / (WallRatio * Range * negator)) - relativeX) / Math.PI;
            for (double i = LeftLineX; i < 200; i++)
            {
                ArcWidth = (Math.Acos(i / (WallRatio * Range * negator)) - relativeX) / Math.PI;
            }

        }
    }
}
