using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.ServiceContracts
{
    public interface IProfileService
    {
       public Task<ApplicationUser> GetCurrentUserAsync();
    }
}