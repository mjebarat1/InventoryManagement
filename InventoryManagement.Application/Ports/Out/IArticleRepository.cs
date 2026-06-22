using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Application.Ports.Out
{
    public  interface IArticleRepository
    {
        /// <summary>
        /// Vérifie l'existence d'une réference
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ExistsByReferenceAsync(Ean13Reference reference, CancellationToken cancellationToken = default);

        /// <summary>
        /// Ajout d'un nouvel article
        /// </summary>
        /// <param name="article"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task AddAsync(Article article,  CancellationToken cancellationToken = default);

        Task<Article?> GetByIdAsync(Guid articleId, CancellationToken cancellationToken = default);

        Task<ArticleSearchPage> SearchAsync(
            ArticleSearchCriteria criteria,
            CancellationToken cancellationToken = default);
    }

    public sealed record ArticleSearchCriteria(
        int PageNumber,
        int PageSize,
        ArticleSortField SortBy,
        SortDirection SortDirection,
        ArticleKind? Type,
        string? Reference,
        string? Name);

    public sealed record ArticleSearchPage(
        IReadOnlyCollection<Article> Items,
        int TotalItems);

    public enum ArticleKind { Food, NonFood }
    public enum ArticleSortField { Reference, Name, Type, PriceExcludingTax }
    public enum SortDirection { Asc, Desc }
}
