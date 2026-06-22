using InventoryManagement.Application.Ports.Out;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure
{
    internal class SystemClock : IClock
    {
        public DateTime Today => DateTime.Today;

        public DateTime UtcNow => DateTime.UtcNow;
    }
}
