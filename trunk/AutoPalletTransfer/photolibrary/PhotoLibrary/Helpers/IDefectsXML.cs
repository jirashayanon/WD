using System;
using System.Collections.Generic;

namespace PhotoLibrary.Helpers
{
    interface IDefectsXML
    {
        List<string> GetAttach();
        List<string> GetOTH();
        List<string> GetSJB();
        List<string> GetSuspension();
    }
}
