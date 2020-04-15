using Microsoft.Practices.ServiceLocation;
using PhotoLibrary.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoLibrary.Model.HGA
{
    public class HGAViewData
    {
        //public long Id { get; set; }
        public long HgainfoId { get; set; }
        public int ViewId { get; set; }
        public string ImagePath { get; set; }
        public string Result { get; set; }
    }

    public class View
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static IRepository dataAccess;

        /// <summary>
        /// A static constructor is used to initialize any static data, or to perform a particular action that needs to be performed once only. 
        /// It is called automatically before the first instance is created or any static members are referenced.
        /// See also: https://msdn.microsoft.com/en-us/library/k9x6w0hc.aspx
        /// </summary>
        static View()
        {
            dataAccess = ServiceLocator.Current.GetInstance<IRepository>();
        }

        private static readonly object padlock = new object();

        private static List<View> _instance;
        public static List<View> Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = dataAccess.GetView();
                    }
                    return _instance;
                }
            }
        }
    }
}
