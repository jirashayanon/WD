using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoLibrary;
using PhotoLibrary.Helpers;
using System.Windows;

namespace PhotoLibrary.UnitTest
{
    [TestClass]
    public class UnitTestMathHelper
    {
        [TestMethod]
        public void TestMin()
        {
            double actual = MathHelper.Min(2.5, 4.9);
            Assert.AreEqual(2.5, actual);
        }

        [TestMethod]
        public void TestRadianToDegree()
        {
            double actual = MathHelper.RadianToDegree(Math.PI / 3);
            Assert.AreEqual(60, actual, 0.001);
            actual = MathHelper.RadianToDegree(-Math.PI / 2);
            Assert.AreEqual(-90, actual, 0.001);
        }

        [TestMethod]
        public void TestDegreeToRadian()
        {
            double actual = MathHelper.DegreeToRadian(60);
            Assert.AreEqual(Math.PI / 3, actual);
            actual = MathHelper.DegreeToRadian(-90);
            Assert.AreEqual(-Math.PI / 2, actual);
        }

        [TestMethod]
        public void TestDistance()
        {
            double actual = MathHelper.Distance(new Point(4.2, 10), new Point(-2.1, 5.9));
            Assert.AreEqual(7.5166, actual, 0.0001);
        }

        [TestMethod]
        public void TestFindTopLeftRectangle()
        {
            Point actual = MathHelper.FindTopLeftRectangle(new Point(4.2, 10), new Point(-2.1, 5.9), new Point(-1.9, 4.3), new Point(10, 8));
            Point expected = new Point(-2.1, 4.3);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLineToDegreeAngle()
        {
            double actual = MathHelper.LineToDegreeAngle(new Point(1, 1), new Point(2, 2));
            double expected = 45;
            Assert.AreEqual(expected, actual, 0.0001);

            actual = MathHelper.LineToDegreeAngle(new Point(3, 2), new Point(1, 3));
            expected = 180 - MathHelper.RadianToDegree(Math.Atan(0.5));
            Assert.AreEqual(expected, actual, 0.0001);

            actual = MathHelper.LineToDegreeAngle(new Point(-1, -1), new Point(-3, -3));
            expected = 225;
            Assert.AreEqual(expected, actual, 0.0001);

            actual = MathHelper.LineToDegreeAngle(new Point(1, 3), new Point(3, 2));
            expected = 360 - MathHelper.RadianToDegree(Math.Atan(0.5));
            Assert.AreEqual(expected, actual, 0.0001);

            actual = MathHelper.LineToDegreeAngle(new Point(1, 2), new Point(1, 10));
            expected = 90;
            Assert.AreEqual(expected, actual, 0.0001);

            actual = MathHelper.LineToDegreeAngle(new Point(1, 10), new Point(1, 5.9));
            expected = 270;
            Assert.AreEqual(expected, actual, 0.0001);

            actual = MathHelper.LineToDegreeAngle(new Point(2, 2), new Point(5, 2));
            expected = 0;
            Assert.AreEqual(expected, actual, 0.0001);

            actual = MathHelper.LineToDegreeAngle(new Point(110, 2), new Point(8, 2));
            expected = 180;
            Assert.AreEqual(expected, actual, 0.0001);

            actual = MathHelper.LineToDegreeAngle(new Point(4.2, 10), new Point(-2.1, 5.9));
            expected = 180 + MathHelper.RadianToDegree(Math.Atan((10 - 5.9) / (4.2 - (-2.1))));
            Assert.AreEqual(expected, actual, 0.0001);
        }

        [TestMethod]
        public void TestInterceptofLine()
        {
            // test 1
            LineEquation line1 = new LineEquation(new Point(1, -1), new Point(5, 5));   // 3x-2y=5
            LineEquation line2 = new LineEquation(new Point(1, 2.0 / 3), new Point(2, 0)); // 2x+3y=4

            double x = line1.interceptXofLine(line2); // 69/39
            double y = line1.interceptYofLine(line2); // 2/13
            double expected_x = 69.0 / 39;
            double expected_y = 2.0 / 13;
            Assert.AreEqual(expected_x, x, 1e-6);
            Assert.AreEqual(expected_y, y, 1e-6);

            // test 2
            line1 = new LineEquation(new Point(1, 5), new Point(2, 4));     // x+y=6 
            line2 = new LineEquation(new Point(1, 1), new Point(0, 3));     // 2x+y=3

            x = line1.interceptXofLine(line2);
            y = line1.interceptYofLine(line2);
            expected_x = -3;
            expected_y = 9;
            Assert.AreEqual(expected_x, x, 1e-6);
            Assert.AreEqual(expected_y, y, 1e-6);
        }
    }
}
