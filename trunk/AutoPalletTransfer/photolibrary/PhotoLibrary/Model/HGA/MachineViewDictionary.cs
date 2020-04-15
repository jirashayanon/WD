using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotoLibrary.Repository;
using Microsoft.Practices.ServiceLocation;

namespace PhotoLibrary.Model.HGA
{
    public class MachineViewDictionary
    {
        private static readonly object padlock = new object();

        public static IRepository dataAccess;

        /// <summary>
        /// A static constructor is used to initialize any static data, or to perform a particular action that needs to be performed once only. 
        /// It is called automatically before the first instance is created or any static members are referenced.
        /// See also: https://msdn.microsoft.com/en-us/library/k9x6w0hc.aspx
        /// </summary>
        static MachineViewDictionary()
        {
            dataAccess = ServiceLocator.Current.GetInstance<IRepository>();
        }

        private static Dictionary<int, List<MachineView>> instance;
        public static Dictionary<int, List<MachineView>> Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = dataAccess.InspectionView();
                    }
                    return instance;
                }
            }
        }
    }
}
