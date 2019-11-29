using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.DTO
{
    class ScriptDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; }

        public int ScriptsGroup_Id { get; set; }

        public string Path { get; set; }

        public string[] RelatedTestCases { get; set; }
    }
}
