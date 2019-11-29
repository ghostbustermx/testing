using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Locust.Helpers
{
    public static class SplitterHelper
    {
        public static string[] splitStringFromKey(string toBeSplitted)
        {

            String types = ConfigurationManager.AppSettings[toBeSplitted];

            string[] type = types.Split(',');

            return type;

        }
    }
}