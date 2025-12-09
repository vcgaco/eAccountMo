using eAccount.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SpecialAccount
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string AccountName { get; set; }

    public int FundId { get; set; }
    public Fund Fund { get; set; }
}
