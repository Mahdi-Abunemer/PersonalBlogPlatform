using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Exceptions
{
    public class ServiceException :Exception
    {
        public int StatusCode { get; }
        public string ErrorMessage { get; }
        public ServiceException(int statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
