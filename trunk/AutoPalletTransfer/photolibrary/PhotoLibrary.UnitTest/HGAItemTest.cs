using PhotoLibrary.Model.HGA;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Moq;
using GalaSoft.MvvmLight.Ioc;
using PhotoLibrary.Repository;
using Microsoft.Practices.ServiceLocation;

namespace PhotoLibrary.UnitTest
{
    /// <summary>
    ///This is a test class for HGAItemTest and is intended
    ///to contain all HGAItemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HGAItemTest
    {
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


        /// <summary>
        ///A test for GetPathHGA
        ///</summary>
        //[TestMethod()]
        public void GetPathHGATest()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            Dictionary<int, List<MachineView>> machineview = new Dictionary<int, List<MachineView>>();
            List<MachineView> viewOne = new List<MachineView> { new MachineView() {
                ViewId = 0,
                View = "TopView",
                FileType = "bmp"
            }};
            List<MachineView> viewTwo = new List<MachineView> { new MachineView() {
                ViewId = 0,
                View = "FrontView",
                FileType = "jpg"
            }};
            machineview.Add(3, viewOne);
            machineview.Add(4, viewTwo);

            var mockDB = new Mock<IRepository>();
            mockDB.Setup(t => t.InspectionView()).Returns(machineview);
            IRepository aaa = mockDB.Object;
            SimpleIoc.Default.Register<IRepository>(() => aaa);

            //PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            HGAItem target = new HGAItem(1, "tray2", 3, "serial4", "slider5", "pack6", 0); // TODO: Initialize to an appropriate value

            // TODO: Now, it fails.
            var mock = new Mock<IHGAItem>();

            HGAInspection hinspect = new HGAInspection()
            {
                InspectionDataId = 5,
                InspectionMachineId = 3,
                Machine = "machine1",
                Module = "module1",
                Datetime = new DateTime(2015, 9, 9),
                StatusFromMachine = "Good",
                StatusFromOQA = "None"
            };
            HGAInspection hinspect2 = new HGAInspection()
            {
                InspectionDataId = 6,
                InspectionMachineId = 4,
                Machine = "machine2",
                Module = "module2",
                Datetime = new DateTime(2015, 9, 10),
                StatusFromMachine = "Good",
                StatusFromOQA = "None"
            };
            target.HGAInspections.Add(hinspect);
            target.HGAInspections.Add(hinspect2);

            List<MachineView> listView = null;
            List<MachineView> listViewExpected = new List<MachineView>();
            listViewExpected.Add(viewOne[0]);
            listViewExpected.Add(viewTwo[0]);

            List<string> expected = new List<string>();
            expected.Add("20150909/machine1/slider5/module1/TopView/serial4__tray2_3_TopView.bmp");
            expected.Add("20150910/machine2/slider5/module2/FrontView/serial4__tray2_3_FrontView.jpg");

            List<string> actual;
            actual = target.GetPathHGA(out listView);

            CollectionAssert.AreEqual(listViewExpected, listView);
            CollectionAssert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetInspectionDataIdFromHGA
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("PhotoLibrary.exe")]
        //public void GetInspectionDataIdFromHGATest()
        //{
        //    PrivateObject param0 = null; // TODO: Initialize to an appropriate value
        //    HGAItem_Accessor target = new HGAItem_Accessor(param0); // TODO: Initialize to an appropriate value
        //    List<long> expected = null; // TODO: Initialize to an appropriate value
        //    List<long> actual;
        //    actual = target.GetInspectionDataIdFromHGA();
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Verify the correctness of this test method.");
        //}
    }
}
