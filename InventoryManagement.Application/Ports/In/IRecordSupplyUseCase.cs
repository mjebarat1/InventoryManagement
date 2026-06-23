using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.RecordSupply;

namespace InventoryManagement.Application.Ports.In;

public interface IRecordSupplyUseCase
    : IUseCase<RecordSupplyCommand, RecordSupplyResult?>
{
}
