//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PhotoLibrary.Repository
{
    using System;
    using System.Collections.Generic;
    
    public partial class hgadefect
    {
        public int Id { get; set; }
        public int HgainfoViewId { get; set; }
        public int DefectId { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
    
        public virtual defect defect { get; set; }
        public virtual hgainfoView hgainfoView { get; set; }
    }
}
