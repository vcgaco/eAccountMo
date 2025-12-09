using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Fund
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string FundName { get; set; }

    public string FundCode { get; set; }
}
