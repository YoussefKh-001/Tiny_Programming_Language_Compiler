using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiny_Langauge_Compiler
{
    class ScanningPhase
    {
        private String regularExpression;
        private String type;

        public ScanningPhase(String regularExpression, String type)
        {
            this.regularExpression = regularExpression;
            this.type = type;
        }

        public String getRegularExpression() { return this.regularExpression; }
        public String getType() { return this.type; }

    }
}
