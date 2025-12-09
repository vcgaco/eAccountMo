namespace eAccount.Models.ViewModel
{
    public class FinancialPositionViewModel
    {
        public string Section { get; set; }
        public string SubSection { get; set; }
        public string Category { get; set; }

        public string AccountCode { get; set; }
        public string AccountName { get; set; }

        public decimal Amount { get; set; }
        public int DisplayOrder { get; set; }
    }
}
