using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Exceptions
{
    public class InvalidIDException(string message)
        : ServiceException(StatusCodes.Status400BadRequest, message)
    {
    }
}

