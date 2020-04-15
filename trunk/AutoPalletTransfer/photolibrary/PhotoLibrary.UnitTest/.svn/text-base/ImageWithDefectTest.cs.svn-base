using PhotoLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Moq;
using System.Collections.ObjectModel;

namespace PhotoLibrary.UnitTest
{
    /// <summary>
    ///This is a test class for ImageWithDefectTest and is intended
    ///to contain all ImageWithDefectTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ImageWithDefectTest
    {
        private TestContext testContextInstance;

        ///<summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        ImageWithDefect targetGlobal;
        /// <summary>
        ///Set up data
        ///</summary>
        [TestInitialize()]
        public void Setup()
        {
            targetGlobal = new ImageWithDefect();
            Defect j4p = new Defect("J4P");
            Defect a1 = new Defect("A1");
            Defect a2 = new Defect("A2");
            DefectsArea da = new DefectsArea(new System.Windows.Point(0.512341, 0.7), new List<Defect>() { j4p, a1, a2 });

            DefectsArea da2 = new DefectsArea(new System.Windows.Point(0.2, 0.71616), new List<Defect>() { a1 });

            targetGlobal.AddToItemsDefects(da);
            targetGlobal.AddToItemsDefects(da2);
        }

        /// <summary>
        ///A test for AddToItemsDefects
        ///</summary>
        [TestMethod()]
        public void AddToItemsDefectsTest()
        {
            Assert.AreEqual(2, targetGlobal.ItemsDefects.Count);
            Assert.AreEqual(3, targetGlobal.ItemsDefectsStringUnique.Count);
        }

        /// <summary>
        ///A test for ClearItemsDefects
        ///</summary>
        [TestMethod()]
        public void ClearItemsDefectsTest()
        {
            targetGlobal.ClearItemsDefects();

            Assert.AreEqual(0, targetGlobal.ItemsDefects.Count);
            Assert.AreEqual(0, targetGlobal.ItemsDefectsStringUnique.Count);
        }

        /// <summary>
        ///A test for GetListDefectData
        ///</summary>
        [TestMethod()]
        public void GetListDefectDataTest()
        {
            List<string> expected = new List<string>()
            {
                "J4P-A1-A2+0.5123:0.7",
                "A1+0.2:0.7162"
            };
            List<string> actual;
            actual = targetGlobal.GetListDefectData();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetListDefectEachArea
        ///</summary>
        [TestMethod()]
        public void GetListDefectEachAreaTest()
        {
            List<string> expected = new List<string>() { "J4P", "A1", "A2", "A1" };
            
            List<string> actual;
            actual = targetGlobal.GetListDefectEachArea();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
