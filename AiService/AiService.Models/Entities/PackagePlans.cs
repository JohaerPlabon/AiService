using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiService.Domains.Entities
{
    public class PackagePlans
    {
        public string Title { get; set; }
        public string Data { get; set; }
        public decimal Price { get; set; }
        public int ValidityDays { get; set; }
        public string Tag { get; set; }
        public bool ShowTimer { get; set; }
        public TimeSpan RemainingTime { get; set; }
    }
}
