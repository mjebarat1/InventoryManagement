using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Application.Ports.Out
{
    public interface IClock
    {
        DateTime Today { get; }
        DateTime UtcNow { get; }
    }
}
