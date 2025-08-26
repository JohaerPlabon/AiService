using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiService.AccountManager.Repository.DTO
{
    public class AuthResult
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public Guid? UserId { get; private set; }
        public string? UserName { get; private set; }
        public bool IsGuest { get; private set; }

        public static AuthResult Success(Guid id, string userName, bool isGuest)
            => new AuthResult { Succeeded = true, UserId = id, UserName = userName, IsGuest = isGuest };

        public static AuthResult Fail(string error)
            => new AuthResult { Succeeded = false, Error = error };
    }
}
