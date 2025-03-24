namespace RickAndMorty.DB.Models;

public class Location
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Dimension { get; set; }
    public required string Url { get; set; }
    public DateTime Created { get; set; }

}
