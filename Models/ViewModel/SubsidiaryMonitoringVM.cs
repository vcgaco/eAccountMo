namespace eAccount.Models.ViewModel
{
    public class SubsidiaryMonitoringVM
    {
        public DateTime Date { get; set; }
        public string JevNo { get; set; }
        public string Particular { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
