namespace RickAndMorty.Contracts;

public sealed class CharacterFilter
{
    public string? Name { get; set; }
    public string? Planet { get; set; }
    public string? Status { get; set; }
    public string? Gender { get; set; }

    public string ToQueryString()
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrWhiteSpace(Name))
            queryParams.Add($"name={Uri.EscapeDataString(Name)}");

        if (!string.IsNullOrWhiteSpace(Planet))
            queryParams.Add($"planet={Uri.EscapeDataString(Planet)}");

        if (!string.IsNullOrWhiteSpace(Status))
            queryParams.Add($"status={Uri.EscapeDataString(Status)}");

        if (!string.IsNullOrWhiteSpace(Gender))
            queryParams.Add($"gender={Uri.EscapeDataString(Gender)}");

        return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
    }
}
