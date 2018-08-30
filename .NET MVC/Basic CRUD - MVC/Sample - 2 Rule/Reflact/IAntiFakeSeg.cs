using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.CodeBuild
{
    public interface IAntiFakeSeg : ICodeSeg
    {
        string Generate(string text);
    }
}
