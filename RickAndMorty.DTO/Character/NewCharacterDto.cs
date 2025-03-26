using System.ComponentModel.DataAnnotations;

namespace RickAndMorty.DTO.Character;

public class NewCharacterDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = string.Empty;

    public int OriginLocationId { get; set; } = default!;

    public int LocationId { get; set; } = default!;

    public string Image { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public List<int> Episodes { get; set; } = [];
}
