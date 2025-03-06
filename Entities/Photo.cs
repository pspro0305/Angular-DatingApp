using System.ComponentModel.DataAnnotations.Schema;

namespace DatingAppAPI.Entities;

[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsMain { get; set; }
    public string? PublicId { get; set; }
    //Navigation property
    public AppUser AppUser { get; set; } = null!; // Added null forgiving operator
    public int AppUserId { get; set; }
}