using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Articles
{
    public  class FoodArticleSaleMode
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
