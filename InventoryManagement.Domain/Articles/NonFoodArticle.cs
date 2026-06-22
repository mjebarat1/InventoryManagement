using InventoryManagement.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Articles
{
    public sealed class NonFoodArticle : Article
    {
        public PackagingLevel PackagingLevel { get; private set; }


        private NonFoodArticle()
        {
            // EF Core
        }

        private NonFoodArticle(
            Ean13Reference reference,
            string name,
            Money priceExcludingTax,
            PackagingLevel packagingLevel)
            : base(reference, name, priceExcludingTax)
        {
            PackagingLevel = packagingLevel;
        }

        public static NonFoodArticle Create(
            Ean13Reference reference,
            string name,
            Money priceExcludingTax,
            PackagingLevel packagingLevel)
        {
            return new NonFoodArticle(
                reference,
                name,
                priceExcludingTax,
                packagingLevel);
        }

        public override VatRate GetVatRate(SaleMode saleMode)
        {
            return VatRate.NonFood();
        }
    }
}
