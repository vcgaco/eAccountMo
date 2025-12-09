namespace eAccount.Models.ViewModel
{
    public class TrialBalanceViewModel
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public string DebitDisplay { get; set; }
        public string CreditDisplay { get; set; }
    }
}
