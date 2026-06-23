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

        private NonFoodArticle()
        {
            // EF Core
        }

        private NonFoodArticle(
            Ean13Reference reference,
            string name,
            Money priceExcludingTax)
            : base(reference, name, priceExcludingTax)
        {
        }

        public static NonFoodArticle Create(
            Ean13Reference reference,
            string name,
            Money priceExcludingTax
        )
        {
            return new NonFoodArticle(
                reference,
                name,
                priceExcludingTax);
        }

        public override VatRate GetVatRate(SaleMode saleMode)
        {
            return VatRate.NonFood();
        }

        public void Update(string name, Money priceExcludingTax)
        {
            UpdateDetails(name, priceExcludingTax);
        }
    }
}
