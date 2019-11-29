using Locus.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locus.Core.Helpers
{
    public static class TextExtractorHelper
    {

        public static string[] ExtractFromScript(Scripts script)
        {

            string FileFullText = File.ReadAllText(script.Path);
            string[] CroppedText = FileFullText.Split('\n');
            List<string> TestCasesNumberList = new List<string>();
            foreach (var textLine in CroppedText)
            {
                if (textLine.Contains("TC_"))
                {
                    var text = textLine.Substring(textLine.IndexOf("TC_"), textLine.Length - 1);
                    text = text.Trim();
                    TestCasesNumberList.Add(text);
                }
            }

            return TestCasesNumberList.ToArray();

        }

    }
}
