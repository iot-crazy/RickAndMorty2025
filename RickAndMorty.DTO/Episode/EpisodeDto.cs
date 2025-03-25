namespace RickAndMorty.DTO.Episode;

public class EpisodeDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime AirDate { get; set; }
    public required string Code { get; set; }
    public required string Url { get; set; }
    public DateTime Created { get; set; }
}
