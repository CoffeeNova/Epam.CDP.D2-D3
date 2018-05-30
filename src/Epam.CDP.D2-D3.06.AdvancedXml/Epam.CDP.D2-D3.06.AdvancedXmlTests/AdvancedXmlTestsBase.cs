using System;
using System.Collections.Generic;
using System.IO;

namespace Epam.CDP.D2_D3._06.AdvancedXmlTests
{
    public class AdvancedXmlTestsBase
    {
        protected static string AppDirectory => AppDomain.CurrentDomain.BaseDirectory;

        protected string ReturnXmlFullPath(string xmlName, params string[] subDirs)
        {
            var path = new List<string> { AppDirectory };
            path.AddRange(subDirs);
            path.Add(xmlName);

            return Path.Combine(path.ToArray());
        }
    }
}
