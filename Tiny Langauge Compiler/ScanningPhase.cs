using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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

        public Regex regex() { return new Regex(regularExpression); }
        public Match Match(String s)
        {
            return new Regex(regularExpression).Match(s);
        }

        public MatchCollection Matches(String s)
        {
            return new Regex(regularExpression).Matches(s);
        }

        public MatchCollection MatchAllExceptThis(String s)
        {
            return new Regex(String.Format("{0}", regularExpression)).Matches(s);
        }
        
            
        public bool isMatch(String s)
        {
            return new Regex(regularExpression).IsMatch(s);
        }


    }
}
