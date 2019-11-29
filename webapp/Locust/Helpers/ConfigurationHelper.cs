using Locus.Core.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Locust.Helpers
{
    public static class ConfigurationHelper
    {
        public static ValidationDTO GetFilesValidation()
        {
            String types = ConfigurationManager.AppSettings["FilesAllowed"];

            String MaximunNumberOfFiles = ConfigurationManager.AppSettings["NumberOfAttachments"];

            int NumberOfFiles = int.Parse(MaximunNumberOfFiles);

            String MaxSize = ConfigurationManager.AppSettings["TotalMb"];

            double FileSize = Double.Parse(MaxSize);

            ValidationDTO validationDTO = new ValidationDTO();

            validationDTO.FilesAllowed = types;
            validationDTO.NumberOfAttachments = NumberOfFiles;
            validationDTO.TotalMb = FileSize;

            return validationDTO;
        }

        public static ValidationDTO GetScriptsValidation()
        {
            String types = ConfigurationManager.AppSettings["ScriptsAllowed"];

            String MaxSize = ConfigurationManager.AppSettings["TotalMb"];

            double FileSize = Double.Parse(MaxSize);

            ValidationDTO validationDTO = new ValidationDTO();

            validationDTO.FilesAllowed = types;
            validationDTO.TotalMb = FileSize;

            return validationDTO;
        }



    }
}