namespace RickAndMorty.DTO;

public sealed class ResponseInfo
{
    public int Count { get; set; }
    public int Pages { get; set; }
    public required string Next { get; set; }
    public required string Prev { get; set; }

}