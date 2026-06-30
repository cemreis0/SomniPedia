using Microsoft.EntityFrameworkCore;
using SomniPedia.Core.Entities;
using SomniPedia.Core.Interfaces;

namespace SomniPedia.Repository.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly SomniPediaDbContext _context;

    public ArticleRepository(SomniPediaDbContext context)
    {
        _context = context;
    }

    public async Task<Article?> GetByIdAsync(string id)
    {
        return await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<(IEnumerable<Article> Articles, long TotalCount)> GetPagedAsync(int page, int pageSize, string? theme, string? category, string? playlist, string? searchTerm)
    {
        var query = _context.Articles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(theme))
            query = query.Where(a => a.Theme == theme);
            
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(a => a.Category == category);
            
        if (!string.IsNullOrWhiteSpace(playlist))
            query = query.Where(a => a.Playlist == playlist);
            
        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(a => a.ArticleTitle.ToLower().Contains(searchTerm.ToLower()));

        var totalCount = await query.CountAsync();
        var articles = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            
        return (articles, totalCount);
    }

    public async Task<IEnumerable<string>> GetDistinctThemesAsync()
    {
        return await _context.Articles
            .Where(a => !string.IsNullOrEmpty(a.Theme))
            .Select(a => a.Theme)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetDistinctCategoriesAsync(string theme)
    {
        return await _context.Articles
            .Where(a => a.Theme == theme && !string.IsNullOrEmpty(a.Category))
            .Select(a => a.Category)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetDistinctPlaylistsAsync(string category)
    {
        return await _context.Articles
            .Where(a => a.Category == category && !string.IsNullOrEmpty(a.Playlist))
            .Select(a => a.Playlist)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Article> CreateAsync(Article article)
    {
        article.CreatedAt = DateTime.UtcNow;
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();
        return article;
    }

    public async Task UpdateAsync(string id, Article article)
    {
        var existing = await GetByIdAsync(id);
        if (existing != null)
        {
            article.Id = id;
            article.UpdatedAt = DateTime.UtcNow;
            _context.Entry(existing).CurrentValues.SetValues(article);
            await _context.SaveChangesAsync();
        }
    }
}
