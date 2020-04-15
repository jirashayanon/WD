using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoLibrary.Model
{
    public class DefectCollection
    {
        public const string SKIP_DEFECT = "A1_Autowipe";

        public static bool CheckSkipDefect(string defectName)
        {
            return defectName == SKIP_DEFECT;
        }
    }
}
