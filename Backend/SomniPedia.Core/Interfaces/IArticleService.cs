using SomniPedia.Core.DTOs;
using SomniPedia.Core.Entities;

namespace SomniPedia.Core.Interfaces;

public interface IArticleService
{
    Task<PagedResponse<ArticleResponseDto>> GetPagedArticlesAsync(int page, int pageSize, string? theme, string? category, string? playlist, string? searchTerm);
    
    Task<IEnumerable<string>> GetThemesAsync();
    Task<IEnumerable<string>> GetCategoriesByThemeAsync(string theme);
    Task<IEnumerable<string>> GetPlaylistsByCategoryAsync(string category);
    
    Task<ArticleResponseDto?> GetArticleByIdAsync(string id);
}
