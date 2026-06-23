using InventoryManagement.Api.DTO;
using InventoryManagement.Application.Articles.CreateFoodArticle;
using InventoryManagement.Application.Articles.CreateNonFoodArticle;
using InventoryManagement.Application.Articles.GetArticleById;
using InventoryManagement.Application.Articles.SearchArticles;
using InventoryManagement.Application.Articles.RecordSupply;
using InventoryManagement.Application.Articles.RecordSale;
using InventoryManagement.Application.Articles.RecordInventory;
using InventoryManagement.Application.Articles.SearchStockBuckets;
using InventoryManagement.Application.Articles.UpdateArticle;
using InventoryManagement.Application.Articles.DeactivateArticle;
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
        private readonly IRecordSupplyUseCase _recordSupplyUseCase;
        private readonly IRecordSaleUseCase _recordSaleUseCase;
        private readonly IRecordInventoryUseCase _recordInventoryUseCase;
        private readonly ISearchStockBucketsUseCase _searchStockBucketsUseCase;
        private readonly IUpdateArticleUseCase _updateArticleUseCase;
        private readonly IDeactivateArticleUseCase _deactivateArticleUseCase;

        public ArticlesController(
            ICreateFoodArticleUseCase createFoodArticleUseCase,
            ICreateNonFoodArticleUseCase createNonFoodArticleUseCase,
            ISearchArticlesUseCase searchArticlesUseCase,
            IGetArticleByIdUseCase getArticleByIdUseCase,
            IRecordSupplyUseCase recordSupplyUseCase,
            IRecordSaleUseCase recordSaleUseCase,
            IRecordInventoryUseCase recordInventoryUseCase,
            ISearchStockBucketsUseCase searchStockBucketsUseCase,
            IUpdateArticleUseCase updateArticleUseCase,
            IDeactivateArticleUseCase deactivateArticleUseCase)
        {
            _createFoodArticleUseCase = createFoodArticleUseCase;
            _createNonFoodArticleUseCase = createNonFoodArticleUseCase;
            _searchArticlesUseCase = searchArticlesUseCase;
            _getArticleByIdUseCase = getArticleByIdUseCase;
            _recordSupplyUseCase = recordSupplyUseCase;
            _recordSaleUseCase = recordSaleUseCase;
            _recordInventoryUseCase = recordInventoryUseCase;
            _searchStockBucketsUseCase = searchStockBucketsUseCase;
            _updateArticleUseCase = updateArticleUseCase;
            _deactivateArticleUseCase = deactivateArticleUseCase;
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
                    request.SearchTerm,
                    request.ActivityFilter),
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

        [HttpPost("{id:guid}/supplies")]
        public async Task<IActionResult> RecordSupply(
            Guid id,
            [FromBody] RecordSupplyRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _recordSupplyUseCase.ExecuteAsync(
                new RecordSupplyCommand(
                    id,
                    request.StockBucketReference,
                    request.Quantity,
                    request.ExpirationDate,
                    request.PackagingLevel),
                cancellationToken);

            return result is null
                ? NotFound()
                : StatusCode(StatusCodes.Status201Created, new
                {
                    movementId = result.MovementId,
                    bucketId = result.BucketId
                });
        }

        [HttpPost("{id:guid}/sales")]
        public async Task<IActionResult> RecordSale(
            Guid id,
            [FromBody] RecordSaleRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _recordSaleUseCase.ExecuteAsync(
                new RecordSaleCommand(id, request.Quantity, request.SaleMode),
                cancellationToken);

            return result is null
                ? NotFound()
                : StatusCode(StatusCodes.Status201Created, new
                {
                    movementId = result.MovementId,
                    soldQuantity = result.SoldQuantity
                });
        }

        [HttpPost("{id:guid}/inventories")]
        public async Task<IActionResult> RecordInventory(
            Guid id,
            [FromBody] RecordInventoryRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _recordInventoryUseCase.ExecuteAsync(
                new RecordInventoryCommand(
                    id,
                    request.Comment,
                    (request.ExistingBuckets ?? []).Select(item => new RecordInventoryExistingBucketCommand(
                        item.StockBucketId,
                        item.CountedQuantity)).ToArray(),
                    (request.NewBuckets ?? []).Select(item => new RecordInventoryNewBucketCommand(
                        item.Reference,
                        item.CountedQuantity,
                        item.ExpirationDate,
                        item.PackagingLevel)).ToArray()),
                cancellationToken);

            return result is null
                ? NotFound()
                : StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("{id:guid}/stock-buckets/search")]
        public async Task<IActionResult> SearchStockBuckets(
            Guid id,
            [FromBody] SearchStockBucketsRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _searchStockBucketsUseCase.ExecuteAsync(
                new SearchStockBucketsQuery(id, request.ReferenceDigits),
                cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateArticle(
            Guid id,
            [FromBody] UpdateArticleRequest request,
            CancellationToken cancellationToken)
        {
            var updated = await _updateArticleUseCase.ExecuteAsync(
                new UpdateArticleCommand(
                    id,
                    request.Name,
                    request.PriceExcludingTax,
                    request.AllowedSaleModes),
                cancellationToken);

            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeactivateArticle(
            Guid id,
            CancellationToken cancellationToken)
        {
            var deactivated = await _deactivateArticleUseCase.ExecuteAsync(
                new DeactivateArticleCommand(id),
                cancellationToken);

            return deactivated ? NoContent() : NotFound();
        }
    }
}
