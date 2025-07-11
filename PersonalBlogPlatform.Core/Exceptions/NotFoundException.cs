using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Exceptions
{
   public class NotFoundException(string message)
       : ServiceException(StatusCodes.Status404NotFound, message)
   {
   }
}