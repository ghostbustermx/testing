using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;

namespace Locust.Helpers
{
    public static class VerifyToken
    {
        public static bool IsTokenValid(HttpRequestMessage re)
        {
            var headers = re.Headers;

            if (headers.Contains("Token"))
            {
                string token = headers.GetValues("Token").First();
                if (token==null)
                {
                    return false;
                }
                else
                {
                    var secretKey = WebConfigurationManager.AppSettings["SecretKey"];
                    if (token.Equals(secretKey))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            else
            {
                return false;
            }


          

        }
    }
}