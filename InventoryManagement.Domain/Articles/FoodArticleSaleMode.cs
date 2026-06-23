namespace InventoryManagement.Domain.Articles
{
    public sealed class FoodArticleSaleMode
    {
        public SaleMode Value { get; private set; }

        private FoodArticleSaleMode()
        {
            // EF Core
        }

        private FoodArticleSaleMode(SaleMode value)
        {
            Value = value;
        }

        public static FoodArticleSaleMode Create(SaleMode value)
        {
            return new FoodArticleSaleMode(value);
        }

    }
}
