namespace eAccount.Models.ViewModel
{
    public class EditJevEntryVM
    {
        public int Id { get; set; }
        public int JevId { get; set; }

        public int AccountId { get; set; }
        public int? SubsidiaryId { get; set; }
        public int? FixedAssetId { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
