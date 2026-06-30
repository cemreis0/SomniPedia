using System.ComponentModel.DataAnnotations;

namespace SomniPedia.Core.Entities;

public class Article : BaseEntity
{
    [Required]
    public string SourceUrl { get; set; } = string.Empty;

    [Required]
    public string ArticleTitle { get; set; } = string.Empty;

    public string Language { get; set; } = "en";

    [Required]
    public string RevisionId { get; set; } = string.Empty;

    [Required]
    public string License { get; set; } = "CC BY-SA 4.0";

    [Required]
    public string LicenseUrl { get; set; } = "https://creativecommons.org/licenses/by-sa/4.0/";

    public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;

    public bool Modified { get; set; } = true;

    [Required]
    public string S3RawKey { get; set; } = string.Empty;

    [Required]
    public string S3FormattedKey { get; set; } = string.Empty;

    public string Theme { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Playlist { get; set; } = string.Empty;
}
