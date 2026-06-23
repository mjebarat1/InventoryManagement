using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.RecordSale;

namespace InventoryManagement.Application.Ports.In;

public interface IRecordSaleUseCase
    : IUseCase<RecordSaleCommand, RecordSaleResult?>
{
}
