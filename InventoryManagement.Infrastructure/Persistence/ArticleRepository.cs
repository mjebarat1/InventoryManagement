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
    }
}
