using System.Text.Json.Serialization;

namespace RickAndMorty.DTO.Episode;

public class NewEpisodeDto
{
    public int Id { get; set; }
    public required string Name { get; set; }

    [JsonPropertyName("air_date")]
    public required string AirDate { get; set; }

    [JsonPropertyName("episode")]
    public required string Code { get; set; }
    public required string Url { get; set; }
    public DateTime Created { get; set; }
}
