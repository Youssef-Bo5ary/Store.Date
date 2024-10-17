using Store.Date.Entities.IdentityEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.TokenServices
{
    public interface ITokenService
    {
        string GenerateToken(AppUser appUser);
    }
}
