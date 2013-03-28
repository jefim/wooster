using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wooster.Utils
{
    public static class AssemblyHelper
    {
        public static Stream GetEmbeddedResource(Assembly asm, string endsWith)
        {
            var resources = asm.GetManifestResourceNames();
            var resourceFullName = resources.FirstOrDefault(o => o.EndsWith(endsWith));
            return asm.GetManifestResourceStream(resourceFullName);
        }
    }
}
