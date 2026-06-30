using SomniPedia.Core.Entities;

namespace SomniPedia.Core.Interfaces;

public interface IArticleRepository
{
    Task<Article?> GetByIdAsync(string id);
    Task<(IEnumerable<Article> Articles, long TotalCount)> GetPagedAsync(int page, int pageSize, string? theme, string? category, string? playlist, string? searchTerm);
    
    Task<IEnumerable<string>> GetDistinctThemesAsync();
    Task<IEnumerable<string>> GetDistinctCategoriesAsync(string theme);
    Task<IEnumerable<string>> GetDistinctPlaylistsAsync(string category);

    Task<Article> CreateAsync(Article article);
    Task UpdateAsync(string id, Article article);
}
