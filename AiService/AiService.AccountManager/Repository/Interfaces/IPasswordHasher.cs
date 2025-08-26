using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiService.AccountManager.Repository.Interfaces
{
    public interface IPasswordHasher
    {
        (string Hash, string Salt) Hash(string password);
        bool Verify(string password, string hash, string salt);
    }
}
