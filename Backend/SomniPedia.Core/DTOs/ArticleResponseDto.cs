namespace SomniPedia.Core.DTOs;

public class ArticleResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string ArticleTitle { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Playlist { get; set; } = string.Empty;
    public string S3FormattedKey { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
