using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Shared
{
    public sealed record CreateArticleResult(
        Guid ArticleId,
        string Reference,
        string Name);
}
