namespace eAccount.Models.ViewModel
{
    public class CashFlowViewModel
    {
        public List<CashFlowItem> OperatingActivities { get; set; } = new();
        public List<CashFlowItem> InvestingActivities { get; set; } = new();
        public List<CashFlowItem> FinancingActivities { get; set; } = new();

        public decimal NetOperating { get; set; }
        public decimal NetInvesting { get; set; }
        public decimal NetFinancing { get; set; }

        public decimal NetIncreaseInCash { get; set; }
    }
    public class CashFlowItem
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
    }
}
