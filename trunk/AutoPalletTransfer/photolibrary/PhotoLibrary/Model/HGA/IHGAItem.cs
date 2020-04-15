using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoLibrary.Model.HGA
{
    public interface IHGAItem
    {
        List<string> GetPathHGA(out List<MachineView> listView);
        List<long> GetInspectionDataIdFromHGA();
    }
}
