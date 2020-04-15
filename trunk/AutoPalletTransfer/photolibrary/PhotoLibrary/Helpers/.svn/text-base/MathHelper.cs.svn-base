using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace PhotoLibrary.Helpers
{
    public class MathHelper
    {
        public static double Min(double a1, double a2)
        {
            double min_value = a1;
            if (min_value < a2) return min_value;
            else return a2;
        }

        public static double RadianToDegree(double radian)
        {
            return radian * 180 / Math.PI;
        }

        public static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }

        public static double Distance(Point startPoint, Point endPoint)
        {
            return Math.Sqrt((startPoint.X - endPoint.X) * (startPoint.X - endPoint.X) + (startPoint.Y - endPoint.Y) * (startPoint.Y - endPoint.Y));
        }
        public static Vector VectorBetweenPoint(Point startPoint, Point endPoint)
        {
            Vector v = new Vector(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
            return v;
        }
        public static double Distance_two(Point startPoint, Point endPoint)
        {
            if (endPoint.Y - startPoint.Y < 0 || endPoint.X - startPoint.X < 0)
            {
                return -1 * (Math.Sqrt((startPoint.X - endPoint.X) * (startPoint.X - endPoint.X) + (startPoint.Y - endPoint.Y) * (startPoint.Y - endPoint.Y)));
            }
            return Math.Sqrt((startPoint.X - endPoint.X) * (startPoint.X - endPoint.X) + (startPoint.Y - endPoint.Y) * (startPoint.Y - endPoint.Y));
        }

        /// <summary>
        /// Find top left of rectangle wrapping all 4 points.
        /// Compute on Computer Coordinate (0, 0) to (n, m) - not Cartesian coordinate system.
        /// </summary>
        public static Point FindTopLeftRectangle(Point p1, Point p2, Point p3, Point p4)
        {
            double pos_x = MathHelper.Min(MathHelper.Min(p1.X, p2.X), MathHelper.Min(p3.X, p4.X));
            double pos_y = MathHelper.Min(MathHelper.Min(p1.Y, p2.Y), MathHelper.Min(p3.Y, p4.Y));

            Point topleft = new Point(pos_x, pos_y);
            return topleft;
        }

        /// <summary>
        /// Compute on Cartesian Coordinate System (Still be applied to Computer Coordinate - (0, 0) to (n, m)).
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static double LineToDegreeAngle(Point startPoint, Point endPoint)
        {
            double lineAngle;
            if (endPoint.X == startPoint.X)
            {
                lineAngle = Math.PI / 2;
            }
            else
            {
                lineAngle = Math.Atan((endPoint.Y - startPoint.Y) / (endPoint.X - startPoint.X));
            }
            lineAngle = RadianToDegree(lineAngle);

            // Convert (-90, 90) to [0,360)
            if (startPoint.X <= endPoint.X && startPoint.Y <= endPoint.Y) { }
            else if (startPoint.X > endPoint.X && startPoint.Y <= endPoint.Y)
            {
                lineAngle += 180;
            }
            else if (startPoint.X >= endPoint.X && startPoint.Y > endPoint.Y)
            {
                lineAngle += 180;
            }
            else if (startPoint.X < endPoint.X && startPoint.Y > endPoint.Y)
            {
                lineAngle += 360;
            }
            return lineAngle;
        }

        /// <summary>
        /// Distance between 2 parallel lines.
        /// ax+by=c => distance = abs(c2-c1) / sqrt(a^2+b^2)
        /// y=mx+b => distance = abs(b2-b1) / sqrt(m^2+1)
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static double DistanceBetweenTwoLine(LineEquation line1, LineEquation line2)
        {
            return Math.Abs(line2.c - line1.c) / Math.Sqrt(line1.slope * line1.slope + 1);
        }
        public static double AngleBetweenTwoLines(Point p1, Point p1_a, Point p2, Point p2_a)
        {
            double theta1 = Math.Atan2(p1.Y - p1_a.Y, p1.X - p1_a.X);
            double theta2 = Math.Atan2(p2.Y - p2_a.Y, p2.X - p2_a.X);
            double diff = Math.Abs(theta1 - theta2);
            double angle = Min(diff, Math.Abs(180 - diff));
            return RadianToDegree(Math.Abs(angle));

        }
        public static Point MidPointBetweenPoint(Point p1, Point p2)
        {
            double x = 0;
            x += p1.X;
            x += p2.X;

            double y = 0;
            y += p1.Y;
            y += p2.Y;

            return new Point(x / 2, y / 2);
        }
        public static bool ZoomOut(Double width, double height, Point p, Point p_a)
        {
            double dis1 = Distance(p, new Point(width / 2, height / 2));
            double dis2 = Distance(p_a, new Point(width / 2, height / 2));
            if (dis2 > dis1) return true;
            else return false;
        }
        public static double getDelta(double width, double height, Point p1, Point p1_a, Point p2, Point p2_a)
        {

            double diff1 = Distance(p1, p2);
            double diff2 = Distance(p1_a, p2_a);
            return diff2 - diff1;
        }


        public static Vector FindPerpendicularVector(Vector v)
        {
            return new Vector(-v.Y, v.X);
        }

        public static double CosDegree(double angle)
        {
            return Math.Cos(MathHelper.DegreeToRadian(angle));
        }

        public static double SinDegree(double angle)
        {
            return Math.Sin(MathHelper.DegreeToRadian(angle));
        }
    }

    public class LineEquation
    {
        public double slope;
        public double c;

        public LineEquation(Point start, Point end)
        {
            slope = (end.Y - start.Y) / (end.X - start.X);
            c = start.Y - slope * start.X;
        }

        public LineEquation(Line line)
        {
            slope = (line.Y2 - line.Y1) / (line.X2 - line.X1);
            c = line.Y1 - slope * line.X1;
        }

        public double interceptXofLine(LineEquation other)
        {
            return -(this.c - other.c) / (this.slope - other.slope);
        }

        public double interceptYofLine(LineEquation other)
        {
            return (other.slope * this.c - this.slope * other.c) / (other.slope - this.slope);
        }
    }
}
