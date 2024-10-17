using Microsoft.AspNetCore.Identity;
using Store.Date.Entities.IdentityEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository
{
    public class StoreIdentityContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any()) 
            {
                var user = new AppUser
                {
                    DisplayName="Mohamed Saaied",
                    Email= "Mohamed Saaied@gmail.com",
                    UserName="MohamedSaaied",
                    Address=new Address
                    {
                        FirstName="Mohamed",
                        LastName="Saaied",
                        City="Alexandria",
                        State= "Alexandria",
                        Street="5",
                        PostalCode= "123425"

                    }
                };
            await userManager.CreateAsync(user,"Password123$");
            }
        }
    }
}
