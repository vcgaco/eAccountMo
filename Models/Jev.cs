using eAccount.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Jev
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string JevNo { get; set; }
    [DisplayName("Jev Amount")]
    public decimal JevAmount { get; set; }
    public string Particular { get; set; }
    public DateTime EncodedDate { get; set; } = DateTime.Now;
    [DisplayName("Accounting Period")]
    public DateTime AccountingPeriod { get; set; } = DateTime.Now;

    public int? FundId { get; set; }
    public int? SpecialAccountId { get; set; }
    public int? TransId { get; set; }
    public string AccountingYear { get; set; }
    [ValidateNever]
    public virtual ICollection<JevEntry> Entries { get; set; }

}
