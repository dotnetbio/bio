using System;
using System.Text;
using System.Windows;

namespace Tools.VennDiagram
{
    public class PolarCoordinate
    {
        double rho;     // radius
        double theta;   // angle in radians -pi - +pi

        public PolarCoordinate()
        {
            rho = 0.0;
            theta = 0.0;
        }
        public double Rho { get { return rho; } }
        public double Theta { get { return theta; } }
                
        public PolarCoordinate(Point pt)
        {
            ConvertToPolar(pt);
        }
        public void ConvertToPolar(Point pt)
        {
            rho = Math.Sqrt(Math.Pow(pt.X, 2.0) + Math.Pow(pt.Y, 2.0));
            theta = Math.Atan2(pt.Y, pt.X);
        }
        public Point ConvertToCartesian()
        {
            Point pt = new Point();
            pt.X = rho * Math.Cos(theta);
            pt.Y = rho * Math.Sin(theta);
            return pt;
        }
    }

    public class VennCircle
    {
        public Point center;
        public double radius;
        private double m_area;
        public double Area
        {
            set
            {
                if (value <= 0.0)
                    throw new ArgumentOutOfRangeException("Area");
                m_area = value;
                radius = Math.Sqrt(m_area / Math.PI);
            }
            get
            {
                return m_area;
            }
        }
        public double Radius
        {
            get
            {
                return (radius);
            }
        }

        public VennCircle()
        {
            center.X = 0.0;
            center.Y = 0.0;
            radius = 0.0;
            m_area = 0.0;
        }

        public VennCircle(double area)
        {
            // compute the radius give the size of a region (area = pi * (radius^2) =>
            //  radius = sqrt( area )/pi
            center.X = 0.0;
            center.Y = 0.0;
            Area = area;
        }

        public VennCircle(VennCircle vc)
        {
            center.X = vc.center.X;
            center.Y = vc.center.Y;
            radius = vc.radius;
            m_area = vc.m_area;
        }
#if false
        public VennCircle(VennCircle vc, double scaleFactor)
        {
            this.center.X = vc.center.X * scaleFactor;
            this.center.Y = vc.center.Y * scaleFactor;
            this.radius = vc.radius * scaleFactor;
            this.m_area = vc.m_area;
        }
#endif
        public double ComputeDistanceBetweenCenters(VennCircle circle)
        {
            // cartesian distance d = Sqrt( ((x2-x1)^2) + ((y2-y1)^2) )
            double distance = Math.Sqrt(Math.Pow((center.X - circle.center.X), 2.0) + Math.Pow((center.Y - circle.center.Y), 2.0));
            return distance;
        }

        public Point ComputeIntersectionPoint(VennCircle circle)
        {
            // compute a point of intersection for the circles then the area of the overlap
            Point pt = new Point();
            double distance = ComputeDistanceBetweenCenters(circle);
            pt.X = (Math.Pow(radius, 2.0) - Math.Pow(circle.radius, 2.0) + Math.Pow(distance, 2.0)) / (2.0 * distance);
            pt.Y = Math.Sqrt(Math.Pow(radius, 2.0) - Math.Pow(pt.X, 2.0));
            return pt;
        }

        public Point ComputeIntersectionPoint(VennCircle circle, double distance)
        {
            // compute a point of intersection for the circles
            Point pt = new Point();
            pt.X = (Math.Pow(radius, 2.0) - Math.Pow(circle.radius, 2.0) + Math.Pow(distance, 2.0)) / (2.0 * distance);
            pt.Y = Math.Sqrt(Math.Pow(radius, 2.0) - Math.Pow(pt.X, 2.0));
            return pt;
        }

        public double ComputeOverlap(VennCircle circle)
        {
            double distance = ComputeDistanceBetweenCenters(circle);
            return ComputeOverlap(circle, distance);
        }

        public double ComputeOverlap(VennCircle circle, double distance)
        {
            if (distance > radius + circle.radius)
                throw new Exception("No overlap");
            if (distance <= Math.Abs(radius - circle.radius))
                throw new Exception("Completely Enclosed");
            Point pt = ComputeIntersectionPoint(circle, distance);
            double overlap = (Math.Pow(radius, 2.0) * Math.Atan2(pt.Y, pt.X))
                           + (Math.Pow(circle.radius, 2.0) * Math.Atan2(pt.Y, distance - pt.X))
                           - (distance * pt.Y);
            return overlap;
        }

    }

    public class VennDiagramData //: FrameworkElement
    {
        public enum VennTypes { TwoCircle, ThreeCircle };
        public VennTypes vennType;
        public double RegionA { get; set; }
        public double RegionB { get; set; }
        public double RegionC { get; set; }
        public double RegionAB { get; set; }
        public double RegionAC { get; set; }
        public double RegionBC { get; set; }
        public double RegionABC { get; set; }
        public double DistanceAB { get; set; }
        public double DistanceAC { get; set; }
        public double DistanceBC { get; set; }

        public VennCircle CircleA;
        public VennCircle CircleB;
        public VennCircle CircleC;

        private bool fScaled;

        public VennDiagramData()
        {
        }

        public VennDiagramData(VennDiagramData vdd)
        {
            vennType = vdd.vennType;
            RegionA = vdd.RegionA;
            RegionB = vdd.RegionB;
            RegionC = vdd.RegionC;
            RegionAB = vdd.RegionAB;
            RegionAC = vdd.RegionAC;
            RegionBC = vdd.RegionBC;
            RegionABC = vdd.RegionABC;
            DistanceAB = vdd.DistanceAB;
            DistanceAC = vdd.DistanceAC;
            DistanceBC = vdd.DistanceBC;
            CircleA = new VennCircle(vdd.CircleA);
            CircleB = new VennCircle(vdd.CircleB);
            if (vennType == VennTypes.ThreeCircle)
            {
                CircleC = new VennCircle(vdd.CircleC);
            }
            fScaled = vdd.fScaled;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder( "VennDiagramData -- ");
            sb.Append(vennType.ToString());
            sb.Append( "\nx,y,r\n" );
            sb.Append(CircleA.center.X.ToString() + "," + CircleA.center.Y.ToString() + "," + CircleA.radius.ToString() + "\n");
            sb.Append(CircleB.center.X.ToString() + "," + CircleB.center.Y.ToString() + "," + CircleB.radius.ToString() + "\n");
            if (vennType == VennTypes.ThreeCircle)
            {
                sb.Append(CircleC.center.X.ToString() + "," + CircleC.center.Y.ToString() + "," + CircleC.radius.ToString() + "\n");
            }
            return sb.ToString();
        }

        public void WriteVennDiagramData()              // ToString overload w/ format specifier?
        {
            Console.WriteLine("\nCircleID,Center.X,Center.Y,Radius");
            Console.WriteLine("CircleA,{0:F3},{1:F3},{2:F3}", CircleA.center.X, CircleA.center.Y, CircleA.radius);
            Console.WriteLine("CircleB,{0:F3},{1:F3},{2:F3}", CircleB.center.X, CircleB.center.Y, CircleB.radius);
            if (vennType == VennDiagramData.VennTypes.ThreeCircle)
            {
                Console.WriteLine("CircleC,{0:F3},{1:F3},{2:F3}", CircleC.center.X, CircleC.center.Y, CircleC.radius);
            }
        }

        public void WritePolarVennDiagramData()         // ToString overload w/ format specifier?
        {
            PolarCoordinate pcA = new PolarCoordinate(CircleA.center);
            PolarCoordinate pcB = new PolarCoordinate(CircleB.center);

            Console.WriteLine("\nCircleID,Center.theta,Center.rho,Radius");
            Console.WriteLine("CircleA,{0:F3},{1:F3},{2:F3}", pcA.Theta, pcA.Rho, CircleA.radius);
            Console.WriteLine("CircleB,{0:F3},{1:F3},{2:F3}", pcB.Theta, pcB.Rho, CircleB.radius);

            if (vennType == VennDiagramData.VennTypes.ThreeCircle)
            {
                PolarCoordinate pcC = new PolarCoordinate(CircleC.center);
                Console.WriteLine("CircleC,{0:F3},{1:F3},{2:F3}", pcC.Theta, pcC.Rho, CircleC.radius);
            }
        }

        double ComputeVennCircleDistanceForOverlap(VennCircle CircleX, VennCircle CircleY, double OverlapRegionXY, double precision)
        {
            double distanceMax = CircleX.radius + CircleY.radius;
            double distanceMin = Math.Abs(CircleX.radius - CircleY.radius);

            double distanceMid = (distanceMax + distanceMin) / 2.0;
            while (true)
            {
                double overlap = CircleX.ComputeOverlap(CircleY, distanceMid);
                if (overlap > (OverlapRegionXY + precision))
                {
                    // overlap is too large.  Move the centers farther apart
                    distanceMin = distanceMid;
                    distanceMid = (distanceMax + distanceMin) / 2.0;
                }
                else if (overlap < (OverlapRegionXY - precision))
                {
                    // overlap is too small.  Move the centers closer together
                    distanceMax = distanceMid;
                    distanceMid = (distanceMax + distanceMin) / 2.0;
                }
                else
                {
                    break;
                }
            }
            return distanceMid;
        }


        public VennDiagramData(double regionA, double regionB, double regionAB)
        {
            fScaled = false;
            SetTwoCircleVennDiagram(regionA, regionB, regionAB);
        }

        public VennDiagramData(double[] regionArray)
        {
            fScaled = false;
            if ((regionArray.Length == 3)
                && ((regionArray[0]>0.0) && (regionArray[1]>0.0) && (regionArray[2]>0.0)) )
            {
                SetTwoCircleVennDiagram(regionArray[0], regionArray[1], regionArray[2]);
            }
            else if ( (regionArray.Length == 7)
                && ((regionArray[0]>0.0) && (regionArray[1]>0.0) && (regionArray[2]>0.0)
                &&  (regionArray[3]>0.0) && (regionArray[4]>0.0) && (regionArray[5]>0.0) && (regionArray[6]>0.0)) )
            {
                SetThreeCircleVennDiagram(regionArray[0], regionArray[1], regionArray[2], regionArray[3], regionArray[4], regionArray[5], regionArray[6]);
            }
            else
            {
                throw new ArgumentException(Properties.Resources.NotEnoughRegions);
            }
        }

        public VennDiagramData(double regionA, double regionB, double regionC, double regionAB, double regionAC, double regionBC, double regionABC)
        {
            fScaled = false;
            SetThreeCircleVennDiagram(regionA, regionB, regionC, regionAB, regionAC, regionBC, regionABC);
        }

        public void SetTwoCircleVennDiagram(double regionA, double regionB, double regionAB)
        {
            if ((regionA <= 0) || (regionB <= 0) || (regionAB <= 0))
                throw new Exception(Properties.Resources.InvalidOverlappingError);

            vennType = VennTypes.TwoCircle;
            RegionA = regionA;
            RegionB = regionB;
            RegionAB = regionAB;
            ComputeVennDiagramData();
        }

        public void SetThreeCircleVennDiagram(double regionA, double regionB, double regionC, double regionAB, double regionAC, double regionBC, double regionABC)
        {
            if ((regionA <= 0) || (regionB <= 0) || (regionC <= 0)
                || (regionAB <= 0) || (regionAC <= 0) || (regionBC <= 0)
                || (regionABC < 0))
                throw new Exception(Properties.Resources.InvalidOverlappingError);

            vennType = VennTypes.ThreeCircle;
            RegionA = regionA;
            RegionB = regionB;
            RegionC = regionC;
            RegionAB = regionAB;
            RegionAC = regionAC;
            RegionBC = regionBC;
            RegionABC = regionABC;

            ComputeVennDiagramData();
        }


        public double TotalArea()
        {
            double totalArea = RegionA + RegionB + RegionC + RegionAB + RegionAC + RegionBC + RegionABC;
            return (totalArea);
        }

        public VennDiagramData CenterVennDiagramData()
        {
            // Shift the circles to be centered about zero
            VennDiagramData v = new VennDiagramData(this);

            v.Center();
            return v;
        }

        private void Center()
        {
            double MaxX = Math.Max(CircleA.center.X + CircleA.radius, CircleB.center.X + CircleB.radius);
            double MinX = Math.Min(CircleA.center.X - CircleA.radius, CircleB.center.X - CircleB.radius);
            double MaxY = Math.Max(CircleA.center.Y + CircleA.radius, CircleB.center.Y + CircleB.radius);
            double MinY = Math.Min(CircleA.center.Y - CircleA.radius, CircleB.center.Y - CircleB.radius);
            if (vennType == VennTypes.ThreeCircle)
            {
                MaxX = Math.Max(MaxX, CircleC.center.X + CircleC.radius);
                MinX = Math.Min(MinX, CircleC.center.X - CircleC.radius);
                MaxY = Math.Max(MaxY, CircleC.center.Y + CircleC.radius);
                MinY = Math.Min(MinY, CircleC.center.Y - CircleC.radius);
            }

            double drawingWidth = MaxX - MinX;
            double drawingHeight = MaxY - MinY;
            double xMargin = drawingWidth * 0.05;        // 5% margin on each side
            double yMargin = drawingHeight * 0.05;

            double xOriginShift = 0-((MinX + MaxX) / 2.0);
            double yOriginShift = 0-((MinY + MaxY) / 2.0);
            CircleA.center.Offset(xOriginShift, yOriginShift);
            CircleB.center.Offset(xOriginShift, yOriginShift);
            if (vennType == VennTypes.ThreeCircle)
            {
                CircleC.center.Offset(xOriginShift, yOriginShift);
            }
        }

        public double MaxRadius()
        {
            double max = Math.Max(CircleA.radius, CircleB.radius);
            if (vennType == VennTypes.ThreeCircle)
            {
                max = Math.Max(max, CircleC.radius);
            }
            return max;
        }

        public void ScaleTo( double max )
        {
            double sf = ScaleFactor( max );
            Scale( sf );
        }

        public double ScaleFactor( double maxRadius )
        {
            double mr = MaxRadius();
            return( maxRadius / mr );
        }

        private void Scale(double scaleFactor)
        {
            /*
             *  This does not scale the area numbers!!! 
             *  this is data is not consistent, so FAIL
             *  if it has already been scaled.
             */
            if (fScaled)
            {
                throw new ArgumentException(Properties.Resources.AlreadyScaled);
            }
            fScaled = true;

            CircleA.center.X *= scaleFactor;
            CircleA.center.Y *= scaleFactor;
            CircleA.radius *= scaleFactor;
            CircleB.center.X *= scaleFactor;
            CircleB.center.Y *= scaleFactor;
            CircleB.radius *= scaleFactor;
            if (vennType == VennTypes.ThreeCircle)
            {
                CircleC.center.X *= scaleFactor;
                CircleC.center.Y *= scaleFactor;
                CircleC.radius *= scaleFactor;
            }
        }

        public void ComputeVennDiagramData()
        {
            // Create the circles and size them
            CircleA = new VennCircle(RegionA + RegionAB + RegionAC + RegionABC);
            CircleB = new VennCircle(RegionB + RegionAB + RegionBC + RegionABC);

            // TODO:  what is the right precision computation?
            //          fixed? based on smallest region?  largest region?  average of region sizes?
            //      for now, compute our precision to 0.1% of average region size but no less than 1
            double myPrecision = ((RegionA + RegionB + +RegionC + RegionAB + RegionAC + RegionBC + RegionABC) / 7) * 0.001;
            if (myPrecision < 1.0)
            {
                myPrecision = 1.0;
            }
            DistanceAB = ComputeVennCircleDistanceForOverlap(CircleA, CircleB, RegionAB + RegionABC, myPrecision);
#if false
            Console.WriteLine("AB Distance {0}", DistanceAB);
#endif
            // Position CircleA and CircleB along the x axis
            //  since the Y axis is increasing going down, assign a negative value for Y
            CircleA.center.X = 0.0;
            CircleB.center.X = DistanceAB;

            // if this is a three circle Venn Diagram, compute the other circle and placement
            if (vennType == VennTypes.ThreeCircle)
            {
                CircleC = new VennCircle(RegionC + RegionAC + RegionBC + RegionABC);
                DistanceAC = ComputeVennCircleDistanceForOverlap(CircleA, CircleC, RegionAC + RegionABC, myPrecision);
                DistanceBC = ComputeVennCircleDistanceForOverlap(CircleB, CircleC, RegionBC + RegionABC, myPrecision);
#if false
                Console.WriteLine("AC Distance {0}", DistanceAC);
                Console.WriteLine("BC Distance {0}", DistanceBC);
#endif
                if ((DistanceAB + DistanceAC < DistanceBC)
                    || (DistanceAB + DistanceBC < DistanceAC)
                    || (DistanceAC + DistanceBC < DistanceAB))
                    throw new ArgumentOutOfRangeException(Properties.Resources.NoFeasibleSolution);

                // CircleA and CircleB are along the x axis, position CircleC above
                //  since the Y axis is increasing going down, assign a negative value for Y
                CircleC.center.X = (Math.Pow(DistanceAC, 2.0) - Math.Pow(DistanceBC, 2.0) + Math.Pow(DistanceAB, 2.0)) / (2 * DistanceAB);
                CircleC.center.Y = 0 - Math.Sqrt(Math.Pow(DistanceAC, 2) - Math.Pow(CircleC.center.X, 2.0));
            }
        }
    }
}
 
