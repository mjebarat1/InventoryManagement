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
    }
}
