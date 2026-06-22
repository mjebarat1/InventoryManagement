using InventoryManagement.Domain.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Application.Articles.CreateFoodArticle
{
    public sealed record CreateFoodArticleCommand(
        string Reference,
        string Name,
        decimal PriceExcludingTax,
        IEnumerable<SaleMode> SaleModes);
}
