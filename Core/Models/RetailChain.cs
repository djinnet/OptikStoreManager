using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.Models;

[Table("Chain")]
public class RetailChain
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set;}

    //relationship with the stores
    public ICollection<RetailStore> Stores { get; set; } = [];
}
