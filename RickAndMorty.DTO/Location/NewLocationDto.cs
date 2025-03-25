namespace RickAndMorty.DTO.Location;

public class NewLocationDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Dimension { get; set; }
    public required string Url { get; set; }
    public DateTime Created { get; set; }
}
