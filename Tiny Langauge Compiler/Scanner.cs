using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
namespace Tiny_Langauge_Compiler
{
    class Scanner
    {
        ScanningPhase Number, StringInQuotes, ReservedWords, DataTypes, CommentStatement, Identifier,
            Function, Semicolon, Comma, LParentheses, RParentheses, LBraces, RBraces, Assign, Equation, ArithmeticOperators, ConditionalOperators;

        public DataTable tokensDataTable;

        char[] operators = { '+', '-', '/', '*'};
        string[] conditionalOperators = { "=", "<", "<=", ">=", "<>" , ">"};
        string assignOperator = ":="; 
        public Scanner()
        {
            tokensDataTable = new DataTable();
            tokensDataTable.Columns.Add("Lexeme");
            tokensDataTable.Columns.Add("Token");

            Number = new ScanningPhase(@"^\d+(\.\d*)?$", "Number");
            StringInQuotes = new ScanningPhase(@"^""[^""]*""$", "String");
            ReservedWords = new ScanningPhase(@"^(read|write|repeat|until|if|elseif|else|return|then|endl)$", "Reserved Word");
            DataTypes = new ScanningPhase(@"^(int|float|string|char)$", "DataType");
            CommentStatement = new ScanningPhase(@"^\/\*([^\*][^\/]|\s*)*\*\/$", "Comment");
            Identifier = new ScanningPhase(@"^[a-zA-Z_][a-zA-Z0-9_]*$", "Identifier");
            Function = new ScanningPhase(String.Format(@"^{0}\(({0}\s*(,\s* {0})*)?\)$", Identifier.getRegularExpression()), "Function");
            Semicolon = new ScanningPhase(";", "Semicolon");
            Comma = new ScanningPhase(",", "Comma");
            LParentheses = new ScanningPhase(@"\(", "Left Parentheses");
            RParentheses = new ScanningPhase(@"\)", "Right Parentheses");
            LBraces = new ScanningPhase(@"{", "Left Braces");
            RBraces = new ScanningPhase(@"}", "Right Braces");
            Assign = new ScanningPhase("^:=$", "Assign Operator");
            ArithmeticOperators = new ScanningPhase(@"^\+|\-|\*|/$", "Arithmentic Operator");
            Equation = new ScanningPhase(String.Format(@"^\(*{0}\)*(\s*{1}\s*{0}\)*)+$", Number.getRegularExpression(), ArithmeticOperators.getRegularExpression()), "Equation");
            ConditionalOperators = new ScanningPhase(@"^(<|>|=|(<>)|(<=)|(>=))$", "Conditional Operator");
        }

        public void StartScan(String code)
        {
            for (int i = 0; i < code.Length; i++)
            {
                String lexeme = "";
                char currentChar = code[i];
                if (currentChar == ' ' || currentChar == '\n' || currentChar == '\r' || currentChar == '\t')
                    continue;

                if (currentChar == '(' || currentChar == ')' || currentChar == '{' || currentChar == '}' || currentChar == ',' || currentChar == ';')
                    continue;

                if (char.IsLetter(currentChar) || currentChar == '_')
                {
                    while ((char.IsLetter(currentChar) || currentChar == '_') && i < code.Length)
                    {
                        lexeme += currentChar;
                        currentChar = code[++i];
                    }
                    i--;
                }
                else if(char.IsDigit(currentChar))
                {
                    while(char.IsDigit(currentChar) || currentChar == '.')
                    {
                        lexeme += currentChar;
                        currentChar = code[++i];
                    }
                    i--;
                }
                else
                {
                    if(operators.Contains<char>(currentChar))
                    {
                        lexeme += currentChar;
                    }else if(conditionalOperators.Contains(currentChar.ToString()))
                    {
                        lexeme += currentChar;
                        if (i < code.Length - 1)
                            if (conditionalOperators.Contains(lexeme + code[++i]))
                                lexeme += code[i];
                    }else if(currentChar == ':')
                    {
                        if (i < code.Length - 1 && code[++i] == '=')
                            lexeme += assignOperator;
                        else
                        {
                            lexeme += currentChar;
                            i--;
                        }
                    }
                    else
                    {
                        lexeme += currentChar;
                    }

                }
                if(lexeme.Length > 0)
                    ScanWord(lexeme);
            }
        }
        public void ScanWord(String word)
        {
            //Identifier Phase
            Match match = Identifier.Match(word);
            if (match.Success && !DataTypes.isMatch(match.Value) && !ReservedWords.isMatch(match.Value))
            {
                tokensDataTable.Rows.Add(match.Value, Identifier.getType());
                return;
            }
            //DataTypes Phase
            match = DataTypes.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, DataTypes.getType() + "(" + match.Value.ToUpper() + ")");
                return;
            }
            //Reserved Words Phase
            match = ReservedWords.Match(word);
            if (match.Success)
            { 
                tokensDataTable.Rows.Add(match.Value, ReservedWords.getType());
                return;
            }
            //Number Phase
            match = Number.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, Number.getType());
                return;
            }
           
            /* Operators */

            //Arithmetic Operators
            match = ArithmeticOperators.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, ArithmeticOperators.getType());
                return;
            }

            //Conditional Operators
            match = ConditionalOperators.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, ConditionalOperators.getType());
                return;
            }

            //Assign Operator
            match = Assign.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, Assign.getType());
                return;
            }
            tokensDataTable.Rows.Add(word, "Unrecognized");
        }

        public void ScanSpecial(char c)
        {
            
        }






        /*public static String DeleteComments(String text)
        {
            int started = 0;  
            for(int i = 0;i<text.Length-1;i++)
            {
                if(text[i] == '/' && text[i+1] == '*')
                {
                    started++;
                    text = text.Remove(i--,2);
                    
                }
                else if(text[i] == '*' && text[i+1] == '/')
                {
                    started--;
                    text = text.Remove(i--,2);
                }
                else if (started>0)
                    text = text.Remove(i--, 1);
            }

            if (started == 0)
                return text;
            else
            {
                Console.WriteLine("ERROR");
                return "";
            }
        }

        public static String DeleteStrings(String text)
        {
            int started = 0;
            for (int i = 0; i < text.Length - 1; i++)
            {
                if (text[i] == '"')
                {
                    if (started == 0)
                        started++;
                    else started--;
                    text = text.Remove(i--, 1);

                }
                else if (started > 0)
                    text = text.Remove(i--, 1);
            }
           
            if (started == 0)
                return text;
            else
                return "";
        }*/
    }
}
