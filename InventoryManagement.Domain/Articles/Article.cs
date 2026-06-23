using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;

namespace InventoryManagement.Domain.Articles
{
    public abstract class Article
    {
        public Guid Id { get; private set; }
        public Ean13Reference Reference { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public Money PriceExcludingTax { get; private set; } = null!;
        public bool IsActive { get; private set; }

        public ICollection<StockMovement.StockMovement> StockMovements { get; private set; } = new List<StockMovement.StockMovement>();

        protected Article()
        {
            // EF Core
        }

        protected Article(Ean13Reference reference, string name, Money priceExcludingTax)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleException(DomainErrorCodes.ArticleNameRequired);

            Id = Guid.NewGuid();
            Reference = reference;
            Name = name.Trim();
            PriceExcludingTax = priceExcludingTax;
            IsActive = true;
            StockMovements = new List<StockMovement.StockMovement>();
        }

        public abstract VatRate GetVatRate(SaleMode saleMode);

        public Money GetPriceIncludingTax(SaleMode saleMode)
        {
            return PriceExcludingTax.AddVat(GetVatRate(saleMode));
        }

        public void EnsureActive()
        {
            if (!IsActive)
                throw new BusinessRuleException(DomainErrorCodes.ArticleInactive);
        }

        public void Deactivate()
        {
            EnsureActive();
            IsActive = false;
        }

        protected void UpdateDetails(string name, Money priceExcludingTax)
        {
            EnsureActive();
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleException(DomainErrorCodes.ArticleNameRequired);

            Name = name.Trim();
            PriceExcludingTax = priceExcludingTax;
        }
    }
}
