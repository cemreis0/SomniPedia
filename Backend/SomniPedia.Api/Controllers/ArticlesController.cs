using Microsoft.AspNetCore.Mvc;
using SomniPedia.Core.DTOs;
using SomniPedia.Core.Interfaces;

namespace SomniPedia.Api.Controllers;

/// <summary>
/// Handles all HTTP requests for interacting with Wikipedia Articles stored in MongoDB.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _service;

    public ArticlesController(IArticleService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves a paginated and filtered list of Wikipedia articles.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<ArticleResponseDto>>> GetPaged(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20,
        [FromQuery] string? theme = null,
        [FromQuery] string? category = null,
        [FromQuery] string? playlist = null,
        [FromQuery] string? search = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 100) pageSize = 100;

        var result = await _service.GetPagedArticlesAsync(page, pageSize, theme, category, playlist, search);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all distinct Themes available in the database.
    /// </summary>
    [HttpGet("themes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetThemes()
    {
        var themes = await _service.GetThemesAsync();
        return Ok(themes);
    }

    /// <summary>
    /// Retrieves all distinct Categories belonging to a specific Theme.
    /// </summary>
    [HttpGet("themes/{theme}/categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories(string theme)
    {
        var categories = await _service.GetCategoriesByThemeAsync(theme);
        return Ok(categories);
    }

    /// <summary>
    /// Retrieves all distinct Playlists belonging to a specific Category.
    /// </summary>
    [HttpGet("categories/{category}/playlists")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetPlaylists(string category)
    {
        var playlists = await _service.GetPlaylistsByCategoryAsync(category);
        return Ok(playlists);
    }

    /// <summary>
    /// Retrieves a specific Wikipedia article by its ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleResponseDto>> GetById(string id)
    {
        var article = await _service.GetArticleByIdAsync(id);
        if (article == null) return NotFound();
        return Ok(article);
    }
}
