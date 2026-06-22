using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Articles
{
    public sealed class FoodArticle : Article
    {
        // Mode de vente autorisé pour cet aritcle
        private readonly List<FoodArticleSaleMode> _saleModes = new();
        public IReadOnlyCollection<SaleMode> SaleModes
            => _saleModes.Select(x => x.Value).ToList().AsReadOnly();

        private FoodArticle()
        {
            // EF Core
        }
        private FoodArticle(
            Ean13Reference reference,
            string name,
            Money priceExcludingTax,
            IEnumerable<SaleMode> saleModes)
            : base(reference, name, priceExcludingTax)
        {
            SetSaleModes(saleModes);
        }

        public static FoodArticle Create(
            Ean13Reference reference,
            string name,
            Money priceExcludingTax,
            IEnumerable<SaleMode> saleModes)
        {
            return new FoodArticle(
                reference,
                name,
                priceExcludingTax,
                saleModes);
        }

        public override VatRate GetVatRate(SaleMode saleMode)
        {
            if (!CanBeSoldAs(saleMode))
                throw new BusinessRuleException("Ce mode de vente n'est pas autorisé pour cet article.");
            

            return saleMode switch
            {
                SaleMode.TakeAway => VatRate.FoodTakeAway(),
                SaleMode.OnSite => VatRate.FoodOnSite(),
                _ => throw new BusinessRuleException("Mode de vente inconnu.")
            };
        }

        private bool CanBeSoldAs(SaleMode saleMode)
        {
            return _saleModes.Any(x => x.Value == saleMode);
        }


        private void SetSaleModes(IEnumerable<SaleMode> saleModes)
        {
            if (saleModes is null)
            {
                throw new BusinessRuleException("Au moins un mode de vente est obligatoire.");
            }

            var distinctSaleModes = saleModes
                .Distinct()
                .ToList();

            if (distinctSaleModes.Count == 0)
            {
                throw new BusinessRuleException("Au moins un mode de vente est obligatoire.");
            }

            _saleModes.Clear();
            foreach (var saleMode in distinctSaleModes)
                _saleModes.Add(FoodArticleSaleMode.Create(saleMode));

        }

    }
}
