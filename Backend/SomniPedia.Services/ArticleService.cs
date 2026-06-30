using SomniPedia.Core.DTOs;
using SomniPedia.Core.Interfaces;

namespace SomniPedia.Services;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _repository;

    public ArticleService(IArticleRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResponse<ArticleResponseDto>> GetPagedArticlesAsync(int page, int pageSize, string? theme, string? category, string? playlist, string? searchTerm)
    {
        var (articles, totalCount) = await _repository.GetPagedAsync(page, pageSize, theme, category, playlist, searchTerm);
        
        var dtos = articles.Select(a => new ArticleResponseDto
        {
            Id = a.Id,
            ArticleTitle = a.ArticleTitle,
            Language = a.Language,
            License = a.License,
            Theme = a.Theme,
            Category = a.Category,
            Playlist = a.Playlist,
            S3FormattedKey = a.S3FormattedKey,
            CreatedAt = a.CreatedAt
        });

        return new PagedResponse<ArticleResponseDto>(dtos, page, pageSize, totalCount);
    }

    public async Task<IEnumerable<string>> GetThemesAsync() => await _repository.GetDistinctThemesAsync();
    
    public async Task<IEnumerable<string>> GetCategoriesByThemeAsync(string theme) => await _repository.GetDistinctCategoriesAsync(theme);
    
    public async Task<IEnumerable<string>> GetPlaylistsByCategoryAsync(string category) => await _repository.GetDistinctPlaylistsAsync(category);

    public async Task<ArticleResponseDto?> GetArticleByIdAsync(string id)
    {
        var a = await _repository.GetByIdAsync(id);
        if (a == null) return null;

        return new ArticleResponseDto
        {
            Id = a.Id,
            ArticleTitle = a.ArticleTitle,
            Language = a.Language,
            License = a.License,
            Theme = a.Theme,
            Category = a.Category,
            Playlist = a.Playlist,
            S3FormattedKey = a.S3FormattedKey,
            CreatedAt = a.CreatedAt
        };
    }
}
