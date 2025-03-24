namespace RickAndMorty.DTO;

public sealed class ApiResponse<T>
{
    public ResponseInfo Info { get; set; } = default!;
    public List<T> Results { get; set; } = [];
}
