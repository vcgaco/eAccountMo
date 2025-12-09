namespace eAccount.Models.ViewModel
{
    public class AccountLedgerEntry
    {
        public string AccountCode { get; set; }         // Account code
        public string AccountName { get; set; }         // Account name
        public string SubsidiaryName { get; set; }      // Subsidiary (optional)
        public string SubsidiaryCode { get; set; }      // Subsidiary code (optional)
        public string JevNo { get; set; }               // JEV number
        public DateTime EncodedDate { get; set; }       // Date of JEV
        public string Particular { get; set; }          // Description/particulars
        public decimal Debit { get; set; }              // Debit amount
        public decimal Credit { get; set; }             // Credit amount
    }
}
