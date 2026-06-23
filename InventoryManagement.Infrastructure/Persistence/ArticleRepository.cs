using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure.Persistence
{
    internal class ArticleRepository : IArticleRepository
    {
        private readonly StockDbContext _context; 

        public ArticleRepository(StockDbContext context) 
        { 
            _context = context;
        }
        public async Task AddAsync(Domain.Articles.Article article, CancellationToken cancellationToken = default)
        {
            await _context.Articles.AddAsync(article, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsByReferenceAsync(Ean13Reference reference, CancellationToken cancellationToken = default)
        {
           return await _context.Articles.AnyAsync( x=> x.Reference.Value == reference.Value, cancellationToken);
        }

        public async Task<Domain.Articles.Article?> GetByIdAsync(
            Guid articleId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Articles
                .AsNoTracking()
                .Include(article => article.StockMovements)
                    .ThenInclude(movement => movement.Lines)
                .AsSplitQuery()
                .SingleOrDefaultAsync(article => article.Id == articleId, cancellationToken);
        }

        public async Task<ArticleSearchPage> SearchAsync(
            ArticleSearchCriteria criteria,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Domain.Articles.Article> query = _context.Articles
                .AsNoTracking()
                .Include(article => article.StockMovements)
                    .ThenInclude(movement => movement.Lines)
                .AsSplitQuery();

            if (criteria.Type == ArticleKind.Food)
                query = query.Where(article => article is Domain.Articles.FoodArticle);
            else if (criteria.Type == ArticleKind.NonFood)
                query = query.Where(article => article is Domain.Articles.NonFoodArticle);

            if (!string.IsNullOrWhiteSpace(criteria.Reference))
            {
                var reference = criteria.Reference.Trim();
                query = query.Where(article => article.Reference.Value.Contains(reference));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Name))
            {
                var name = criteria.Name.Trim();
                query = query.Where(article => article.Name.Contains(name));
            }

            var totalItems = await query.CountAsync(cancellationToken);
            query = ApplySorting(query, criteria.SortBy, criteria.SortDirection);

            var items = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync(cancellationToken);

            return new ArticleSearchPage(items, totalItems);
        }

        private static IQueryable<Domain.Articles.Article> ApplySorting(
            IQueryable<Domain.Articles.Article> query,
            ArticleSortField sortBy,
            SortDirection direction)
        {
            return (sortBy, direction) switch
            {
                (ArticleSortField.Name, SortDirection.Asc) => query.OrderBy(article => article.Name),
                (ArticleSortField.Name, SortDirection.Desc) => query.OrderByDescending(article => article.Name),
                (ArticleSortField.Type, SortDirection.Asc) => query.OrderBy(article => article is Domain.Articles.FoodArticle ? 0 : 1),
                (ArticleSortField.Type, SortDirection.Desc) => query.OrderByDescending(article => article is Domain.Articles.FoodArticle ? 0 : 1),
                (ArticleSortField.PriceExcludingTax, SortDirection.Asc) => query.OrderBy(article => article.PriceExcludingTax.Amount),
                (ArticleSortField.PriceExcludingTax, SortDirection.Desc) => query.OrderByDescending(article => article.PriceExcludingTax.Amount),
                (ArticleSortField.Reference, SortDirection.Desc) => query.OrderByDescending(article => article.Reference.Value),
                _ => query.OrderBy(article => article.Reference.Value)
            };
        }
    }
}
