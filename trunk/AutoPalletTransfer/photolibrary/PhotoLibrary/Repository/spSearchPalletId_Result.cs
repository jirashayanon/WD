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
    
    public partial class spSearchPalletId_Result
    {
        public int Id { get; set; }
        public string LineNo { get; set; }
        public Nullable<int> PalletId { get; set; }
        public int Position { get; set; }
        public System.DateTime Processdatetime { get; set; }
        public string IaviResult { get; set; }
        public int VmiResult { get; set; }
        public string SerialNumber { get; set; }
        public string IaviSerialNumber { get; set; }
        public Nullable<int> TrayId { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public Nullable<System.DateTime> UpdatedAt { get; set; }
        public double A_Location { get; set; }
        public double B_Location { get; set; }
        public double Skew { get; set; }
        public double PitchOffset { get; set; }
        public double RollOffset { get; set; }
        public Nullable<int> viewId { get; set; }
        public string imagePath { get; set; }
        public Nullable<int> result { get; set; }
        public string palletSN { get; set; }
        public int isCompleted { get; set; }
    }
}