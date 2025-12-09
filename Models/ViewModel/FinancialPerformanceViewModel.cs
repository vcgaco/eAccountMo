namespace eAccount.Models.ViewModel
{
    public class FinancialPerformanceViewModel
    {
        public List<FinancialPerformanceRow> Revenues { get; set; } = new();
        public List<FinancialPerformanceRow> Expenses { get; set; } = new();

        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }

        public decimal NetSurplus { get; set; }
    }
}
