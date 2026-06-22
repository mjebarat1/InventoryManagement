using InventoryManagement.Api.DTO;
using InventoryManagement.Application.Articles.CreateFoodArticle;
using InventoryManagement.Application.Articles.CreateNonFoodArticle;
using InventoryManagement.Application.Articles.GetArticleById;
using InventoryManagement.Application.Articles.SearchArticles;
using InventoryManagement.Application.Ports.In;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ICreateFoodArticleUseCase _createFoodArticleUseCase;
        private readonly ICreateNonFoodArticleUseCase _createNonFoodArticleUseCase;
        private readonly ISearchArticlesUseCase _searchArticlesUseCase;
        private readonly IGetArticleByIdUseCase _getArticleByIdUseCase;

        public ArticlesController(
            ICreateFoodArticleUseCase createFoodArticleUseCase,
            ICreateNonFoodArticleUseCase createNonFoodArticleUseCase,
            ISearchArticlesUseCase searchArticlesUseCase,
            IGetArticleByIdUseCase getArticleByIdUseCase)
        {
            _createFoodArticleUseCase = createFoodArticleUseCase;
            _createNonFoodArticleUseCase = createNonFoodArticleUseCase;
            _searchArticlesUseCase = searchArticlesUseCase;
            _getArticleByIdUseCase = getArticleByIdUseCase;
        }

        [HttpPost("food")]
        public async Task<IActionResult> CreateFoodArticle([FromBody] CreateFoodArticleRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateFoodArticleCommand(
                request.Reference,
                request.Name,
                request.PriceExcludingTax,
                request.SaleModes);

            var result = await _createFoodArticleUseCase.ExecuteAsync(
                command,
                cancellationToken);

            //return CreatedAtAction(
            //    nameof(CreateFoodArticle),
            //    new { id = result.ArticleId },
            //    result);

            return StatusCode(StatusCodes.Status201Created, new
            {
                id = result.ArticleId
            });
        }

        [HttpPost("non-food")]
        public async Task<IActionResult> CreateNonFoodArticle(
            [FromBody] CreateNonFoodArticleRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _createNonFoodArticleUseCase.ExecuteAsync(
                new CreateNonFoodArticleCommand(request.Reference, request.Name, request.PriceExcludingTax),
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, new { id = result.ArticleId });
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchArticles(
            [FromBody] SearchArticlesRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _searchArticlesUseCase.ExecuteAsync(
                new SearchArticlesQuery(
                    request.PageNumber,
                    request.PageSize,
                    request.SortBy,
                    request.SortDirection,
                    request.Type,
                    request.Reference,
                    request.Name),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetArticleById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _getArticleByIdUseCase.ExecuteAsync(
                new GetArticleByIdQuery(id),
                cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
    }
}
