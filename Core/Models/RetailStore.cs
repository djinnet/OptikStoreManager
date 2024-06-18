using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Models;

/// <summary>
/// This is the Retail store - in database it will be named as Store.
/// The class should contained at least basic info of the store.
/// The number should be unique.
/// The store can be hooked to an chain or be an independent store
/// </summary>
[Table("Store")]
public class RetailStore
{
    //automated generated guid ID
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid Id { get; set; }

    //the property Is required to filled out
    public required int Number {  get; set; }

    //the property Is required to filled out
    public required string StoreName { get; set; }

    //rest is optional properties

    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; } //added since it makes sense to included
    public string? PostalCode { get; set; }
    public string? Country { get; set; } //added since it makes sense to included
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? StoreOwner { get; set; } //Question; Can a store have without a storeowner?

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }

    //relationship with the chain
    public Guid? ChainId { get; set; }
    public RetailChain? Chain { get; set; } = null;

    [NotMapped]
    [JsonIgnore]
    public string FullAddress => $"{City} {Region} {PostalCode} {Country}";

    [NotMapped]
    [JsonIgnore]
    public string IsPartOfChain => ChainId == null || ChainId == Guid.Empty ? "No" : "Yes";
}
