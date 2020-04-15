using System;
using System.Collections.Generic;

namespace PhotoLibrary.Helpers
{
    interface IConfigXML
    {
        int GetImageWidth();
        List<string> GetListView();
        int GetMaxPictureOrder();
        string GetPath();
        string GetPathEvaluation();
        int GetStartPictureId();
        int GetThumbnailWidth();
        int GetTimeout();
    }
}
