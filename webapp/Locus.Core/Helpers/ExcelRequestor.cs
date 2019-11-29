using Locus.Core.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Helpers
{
    public static class ExcelRequestor
    {

        public static AssignedStatusDTO Requestor()
        {

            string path = System.Web.HttpContext.Current.Server.MapPath("~\\Files\\IsAvailable.txt");
            string readMeText = null;
            AssignedStatusDTO assignedStatusDTO = new AssignedStatusDTO();

            using (StreamReader readtext = new StreamReader(path))
            {
                readMeText = readtext.ReadLine();
                readtext.Close();
            }
            int aux = Int32.Parse(readMeText);
            if (aux == 1)
            {
                using (StreamWriter writetext = new StreamWriter(path))
                {
                    writetext.WriteLine("0");
                    writetext.Close();
                }
                assignedStatusDTO.assigned = true;
                assignedStatusDTO.message = "CurrentOwner";
                return assignedStatusDTO;

            }

            assignedStatusDTO.assigned = false;
            assignedStatusDTO.message = "NotOwner";
            return assignedStatusDTO;
        }
    }

}
