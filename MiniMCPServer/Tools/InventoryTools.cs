using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMCPServer.Tools
{
    [McpServerToolType]
    public class InventoryTools
    {
        [McpServerTool]
        [Description("Returns current inventory for all products.")]
        public Dictionary<string, int> get_inventory_levels()
        {
            return new Dictionary<string, int>
            {
                { "Moisturizer", 6 },
                { "Shampoo", 8 },
                { "Body Spray", 28 },
                { "Hair Gel", 5 },
                { "Lip Balm", 12 },
                { "Skin Serum", 9 },
                { "Cleanser", 30 },
                { "Conditioner", 3 },
                { "Setting Powder", 17 },
                { "Dry Shampoo", 45 }
            };
        }

        [McpServerTool]
        [Description("Returns number of units sold last week.")]
        public Dictionary<string, int> get_weekly_sales()
        {
            return new Dictionary<string, int>
            {
                { "Moisturizer", 22 },
                { "Shampoo", 18 },
                { "Body Spray", 3 },
                { "Hair Gel", 2 },
                { "Lip Balm", 14 },
                { "Skin Serum", 19 },
                { "Cleanser", 4 },
                { "Conditioner", 1 },
                { "Setting Powder", 13 },
                { "Dry Shampoo", 17 }
            };
        }
    }
}
