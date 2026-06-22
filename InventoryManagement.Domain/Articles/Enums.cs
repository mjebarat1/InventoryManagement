using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Articles
{
    public enum SaleMode
    {
        TakeAway = 1,
        OnSite = 2
    }
    public enum PackagingLevel
    {
        New = 1,
        Refurbished = 2,
        Unsellable = 3
    }

}
