namespace BoticaMVC.ViewModels
{
    public class DashboardVM
    {
        public int TotalMedicamentos { get; set; }
        public int StockBajo { get; set; }
        public int Vencidos { get; set; }
        public int PorVencer30Dias { get; set; }
    }
}
