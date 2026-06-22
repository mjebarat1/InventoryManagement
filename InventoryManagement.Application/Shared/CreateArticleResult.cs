using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Application.Shared
{
    public sealed record CreateArticleResult(
        Guid ArticleId,
        string Reference,
        string Name);
}
