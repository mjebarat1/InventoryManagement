using InventoryManagement.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Shared
{
    public sealed record StockState(
      Quantity Total,
      Quantity Sellable,
      Quantity Unsellable
    );

    public sealed record StockCalculationContext(DateOnly Today);

    public sealed record StockConsumption(
        Guid StockBucketId,
        Quantity CurrentQuantity,
        Quantity ConsumedQuantity
    );

    public sealed record StockBucketAvailability(
        StockBucket.StockBucket Bucket,
        Quantity CurrentQuantity
    );

    public sealed record StockInventoryAdjustment(
        Guid StockBucketId,
        Quantity CurrentQuantity,
        Quantity CountedQuantity
    );

}
