using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;
using PhotoLibrary.Repository;
using Moq;
using PhotoLibrary.Model;

namespace PhotoLibrary.UnitTest
{
    /// <summary>
    /// Summary description for DatabaseTest
    /// </summary>
    [TestClass]
    public class DatabaseTest
    {
        public DatabaseTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        ///A test for GetPathHGA
        ///</summary>
        //[TestMethod()]
        public void TestGetDefectFromDB()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IRepository, SQLServer>();
            IRepository dataAccess = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IRepository>();
            Dictionary<int, Defect> defects = dataAccess.getDefectFromDB();
            Assert.IsTrue(defects.Count > 50);
            Assert.AreEqual(defects[1].Name, "None");
            Assert.AreEqual(defects[2].Name, "Good");


            //var mockDB = new Mock<IRepository>();
            //mockDB.Setup(t => t.InspectionView()).Returns(machineview);
            //IRepository aaa = mockDB.Object;
            //SimpleIoc.Default.Register<IRepository>(() => aaa);


            //CollectionAssert.AreEqual(listViewExpected, listView);
            //CollectionAssert.AreEqual(expected, actual);
        }
    }
}
