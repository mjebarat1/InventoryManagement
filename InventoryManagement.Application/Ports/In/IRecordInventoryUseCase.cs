using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.RecordInventory;

namespace InventoryManagement.Application.Ports.In;

public interface IRecordInventoryUseCase
    : IUseCase<RecordInventoryCommand, RecordInventoryResult?>
{
}
