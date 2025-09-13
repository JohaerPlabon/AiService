using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiService.Domains.Entities
{
    public class PaymentInfo
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty; // Bkash, Rocket, Nagad, Visa
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
        public string TransactionId { get; set; } = string.Empty;
        public string CreatedAtTimeStamp { get; set; } = string.Empty;
    }
}
