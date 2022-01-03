using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data;
using System.Diagnostics;
public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer, Float, String,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant, Repeat, ElseIf, Return, Endl, GreaterThanEqOp, LessThanEqOp, ANDOp, OROp,
    LCurlyBraces, RCurlyBraces, AssignOperator,Main , FunctionIdentifier
}

namespace Tiny_Langauge_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;

        public Token() { }
        public Token(string lex,Token_Class token)
        {
            this.lex = lex;
            this.token_type = token;
        }
    }
    class Scanner
    {
        ScanningPhase Number, StringInQuotes, ReservedWords, DataTypes, CommentStatement, Identifier,
            Function, Semicolon, Comma, LParentheses, RParentheses, LBraces, RBraces, Assign, Equation, ArithmeticOperators, ConditionalOperators, BooleanOperators;

        public DataTable tokensDataTable;
        public List<Token> Tokens = new List<Token>();
        char[] operators = { '+', '-', '/', '*'};
        string[] conditionalOperators = { "=", "<", "<=", ">=", "<>" , ">"};
        string assignOperator = ":="; 
        public List<string>errorList;
        public Scanner()
        {
            tokensDataTable = new DataTable();
            tokensDataTable.Columns.Add("Lexeme");
            tokensDataTable.Columns.Add("Token");

            Number = new ScanningPhase(@"^\d+(\.\d*)?$", "Number");
            StringInQuotes = new ScanningPhase(@"^""[^""]*""$", "String");
            ReservedWords = new ScanningPhase(@"^(read|write|repeat|until|if|elseif|else|return|then|endl)$", "Reserved Word");
            DataTypes = new ScanningPhase(@"^(int|float|string|char)$", "DataType");
            CommentStatement = new ScanningPhase(@"/\*(.|\s)*\*/", "Comment");
            Identifier = new ScanningPhase(@"^[a-zA-Z_][a-zA-Z0-9_]*$", "Identifier");
            Function = new ScanningPhase(String.Format(@"^{0}\(({0}\s*(,\s* {0})*)?\)$", Identifier.getRegularExpression()), "Function");
            Semicolon = new ScanningPhase(@"^;$", "Semicolon");
            Comma = new ScanningPhase(@"^,$", "Comma");
            LParentheses = new ScanningPhase(@"^\($", "Left Parentheses");
            RParentheses = new ScanningPhase(@"^\)$", "Right Parentheses");
            LBraces = new ScanningPhase(@"^{$", "Left Braces");
            RBraces = new ScanningPhase(@"^}$", "Right Braces");
            Assign = new ScanningPhase("^:=$", "Assign Operator");
            ArithmeticOperators = new ScanningPhase(@"^(\+|\-|\*|/)$", "Arithmentic Operator");
            Equation = new ScanningPhase(String.Format(@"^\(*{0}\)*(\s*{1}\s*{0}\)*)+$", Number.getRegularExpression(), ArithmeticOperators.getRegularExpression()), "Equation");
            ConditionalOperators = new ScanningPhase(@"^(<|>|=|(<>)|(<=)|(>=))$", "Conditional Operator");
            BooleanOperators = new ScanningPhase(@"^(&&|\|\|)$", "Boolean Operator");
            errorList = new List<string>();
        }

        public void StartScan(String code)
        {
            int openParentheses = 0 , openBraces = 0;
            int line = 1;
            bool isFunction = false;
            errorList.Clear();
            for (int i = 0; i < code.Length; i++)
            {
                String lexeme = "";
                char currentChar = code[i];
                if (currentChar == ' ' || currentChar == '\r' || currentChar == '\t')
                    continue;
                if(currentChar == '\n')
                {
                    line++;
                    continue;
                }

                if (i < code.Length - 1 && currentChar == '/' && code[i + 1] == '*')
                {
                    bool commentFinished = false;
                    lexeme += "/*";
                    i += 2;
                    while (i < code.Length)
                    {
                        if (i < code.Length - 1 && code[i] == '*' && code[i + 1] == '/')
                        {
                            lexeme += "*/";
                            i++;
                            commentFinished = true;
                            break;
                        }
                        currentChar = code[i];
                        lexeme += currentChar;
                        i++;
                    }
                    if(!commentFinished)
                    {
                        errorList.Add("Line " + line + " : Unrecognized token : " + lexeme);
                        continue;
                    }
                   

                }
                else if (i < code.Length - 1 && currentChar == '*' && code[i + 1] == '/')
                {
                    i++;
                    lexeme += "*/";
                }
                else if (currentChar == '(')
                {
                    openParentheses++;
                    lexeme += currentChar;
                }
                else if (currentChar == ')')
                {
                    lexeme += currentChar;
                    if (openParentheses == 0)
                    {
                        errorList.Add("Line " + line + " : Close Parenthese without opening");
                    }
                    else openParentheses--;

                }
                else if (currentChar == '{')
                {
                    openBraces++;
                    lexeme += currentChar;
                }
                else if (currentChar == '}')
                {
                    lexeme += currentChar;
                    if (openBraces == 0)
                    {
                        //error
                        errorList.Add("Line " + line + " : Close braces without opening");
                    }
                    else openBraces--;

                }
                else if (char.IsLetter(currentChar) || currentChar == '_')
                {
                    while ((char.IsLetter(currentChar) || currentChar == '_') && i < code.Length)
                    {
                        currentChar = code[i];
                        lexeme += currentChar;
                        i++;
                        if (i < code.Length)
                            currentChar = code[i];
                        
                    }
                    i--;
                }
                else if (char.IsDigit(currentChar))
                {
                    while ((char.IsDigit(currentChar) || currentChar == '.') && i < code.Length)
                    {
                        currentChar = code[i];
                        lexeme += currentChar;
                        i++;
                        if (i < code.Length)
                            currentChar = code[i];
                    }
                    i--;
                }
                else
                {

                    if (operators.Contains<char>(currentChar))
                    {
                        lexeme += currentChar;
                    }
                    else if (conditionalOperators.Contains(currentChar.ToString()))
                    {
                        lexeme += currentChar;
                        if (i < code.Length - 1)
                            if (conditionalOperators.Contains(lexeme + code[i + 1]))
                            {
                                i++;
                                lexeme += code[i];
                            }
                    }
                    else if (currentChar == ':')
                    {
                        if (i < code.Length - 1)
                        { 
                            if (code[i+1] == '=')
                            {
                                lexeme += assignOperator;
                                i++;
                            }
                            else
                            {
                                lexeme += currentChar;
                            }
                        }
                        else
                        {
                            lexeme += currentChar;
                            
                        }
                    }
                    else if (currentChar == '&')
                    {
                        if (i < code.Length - 1)
                        {
                            if (code[i + 1] == '&')
                            {
                                lexeme += "&&";
                                i++;
                            }
                            else
                            {
                                lexeme += currentChar;
                            }
                        }
                        else
                        {
                            lexeme += currentChar;

                        }
                    }
                    else if (currentChar == '|')
                    {
                        if (i < code.Length - 1)
                        {
                            if (code[i + 1] == '|')
                            {
                                lexeme += "||";
                                i++;
                            }
                            else
                            {
                                lexeme += currentChar;
                            }
                        }
                        else
                        {
                            lexeme += currentChar;

                        }
                    }
                    else if (currentChar == '"')
                    {
                        lexeme += code[i];
                        i++;
                        while (i < code.Length && code[i] != '\n')
                        {
                            if (code[i] == '"')
                            {
                                lexeme += code[i];
                                i++;
                                break;
                            }
                            lexeme += code[i];
                            i++;
                        }
                        if (i < code.Length && code[i] == ';')
                            i--;
                        if (i < code.Length && code[i] == '\n') line++;
                    }

                    else
                    {
                        lexeme += currentChar;
                    }

                }
                if (i+1 < code.Length&& code[i+1] == '(')
                    isFunction = true;
                if(lexeme.Length > 0)
                    ScanWord(lexeme, line, isFunction);
                isFunction = false;
            }
            if (openBraces > 0)
            {
                errorList.Add("There are open braces that haven't been closed");
            }
            if (openParentheses > 0)
            {
                errorList.Add("There are open Parentheses that haven't been closed");
            }
        }
        public void ScanWord(String word, int line, bool isFunction = false)
        {
            //Comment
            Match match = CommentStatement.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, CommentStatement.getType());
                return;
            }
            
            //Identifier Phase
            match = Identifier.Match(word);
            if (match.Success && !DataTypes.isMatch(match.Value) && !ReservedWords.isMatch(match.Value))
            {
                tokensDataTable.Rows.Add(match.Value, isFunction?"FunctionIdentifier":Identifier.getType());
                if (match.Value == "main")
                    Tokens.Add(new Token(match.Value, Token_Class.Main));
                else
                {
                    if (!isFunction)
                        Tokens.Add(new Token(match.Value, Token_Class.Idenifier));
                    else
                        Tokens.Add(new Token(match.Value, Token_Class.FunctionIdentifier));
                }
                return;
            }
            //DataTypes Phase
            match = DataTypes.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, DataTypes.getType() + "(" + match.Value.ToUpper() + ")");
                string Dtype = match.Value.ToLower();
                Token_Class token = new Token_Class();

                if (Dtype == "int")
                    token = Token_Class.Integer;
                else if (Dtype == "float")
                    token = Token_Class.Float;
                else if (Dtype == "string")
                    token = Token_Class.String;
                    
                Tokens.Add(new Token(Dtype, token));
                return;
            }
            //Reserved Words Phase
            match = ReservedWords.Match(word);
            if (match.Success)
            { 
                tokensDataTable.Rows.Add(match.Value, word.ToUpper());
                string Dtype = match.Value.ToLower();
                Token_Class token = new Token_Class();

                switch (Dtype)
                {
                   
                    case "read":
                        token = Token_Class.Read;
                        break;
                    case "write":
                        token = Token_Class.Write;
                        break;
                    case "repeat":
                        token = Token_Class.Repeat;
                        break;
                    case "until":
                        token = Token_Class.Until;
                        break;
                    case "if":
                        token = Token_Class.If;
                        break;
                    case "elseif":
                        token = Token_Class.ElseIf;
                        break;
                    case "else":
                        token = Token_Class.Else;
                        break;
                    case "return":
                        token = Token_Class.Return;
                        break;
                    case "then":
                        token = Token_Class.Then;
                        break;
                    case "endl":
                        token = Token_Class.Endl;
                        break;
            }

                Tokens.Add(new Token(Dtype, token));
                return;
            }
            //Number Phase
            match = Number.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, Number.getType());
                Tokens.Add(new Token(match.Value, Token_Class.Constant));
                return;
            }
           
            /* Operators */

            //Arithmetic Operators
            match = ArithmeticOperators.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, ArithmeticOperators.getType());
                string Dtype = match.Value;
                Token_Class token = new Token_Class();

                switch (Dtype)
                {
                    case "+":
                        token = Token_Class.PlusOp;
                        break;
                    case "-":
                        token = Token_Class.MinusOp;
                        break;
                    case "*":
                        token = Token_Class.MultiplyOp;
                        break;
                    case "/":
                        token = Token_Class.DivideOp;
                        break;
                }

                Tokens.Add(new Token(Dtype, token));
                return;
            }

            //Conditional Operators
            match = ConditionalOperators.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, ConditionalOperators.getType());
                string Dtype = match.Value;
                Token_Class token = new Token_Class();

                switch (Dtype)
                {
                    case ">":
                        token = Token_Class.GreaterThanOp;
                        break;
                    case "<":
                        token = Token_Class.LessThanOp;
                        break;
                    case "<>":
                        token = Token_Class.NotEqualOp;
                        break;
                    case "<=":
                        token = Token_Class.LessThanEqOp;
                        break;
                    case ">=":
                        token = Token_Class.GreaterThanEqOp;
                        break;
                    case "=":
                        token = Token_Class.EqualOp;
                        break;
                }

                Tokens.Add(new Token(Dtype, token));
                return;
                
            }
            //Boolean Operators
            match = BooleanOperators.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, (word == "&&" ? "AND":"OR"));
                string Dtype = match.Value;
                Token_Class token = new Token_Class();

                switch (Dtype)
                {
                    case "&&":
                        token = Token_Class.ANDOp;
                        break;
                    case "||":
                        token = Token_Class.OROp;
                        break;
                    
                }

                Tokens.Add(new Token(Dtype, token));
                return;
            }
            //semicolon
            match = Semicolon.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, Semicolon.getType());
                Tokens.Add(new Token(match.Value, Token_Class.Semicolon));
                return;
            }
            //comma
            match = Comma.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, Comma.getType());
                Tokens.Add(new Token(match.Value, Token_Class.Comma));
                return;
            }
            //openparenthese
            match = LParentheses.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, LParentheses.getType());
                Tokens.Add(new Token(match.Value, Token_Class.LParanthesis));
                return;
            }
            //closeparenthese
            match = RParentheses.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, RParentheses.getType());
                Tokens.Add(new Token(match.Value, Token_Class.RParanthesis));
                return;
            }
            //openBraces
            match = LBraces.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, LBraces.getType());
                Tokens.Add(new Token(match.Value, Token_Class.LCurlyBraces));
                return;
            }
            //close braces
            match = RBraces.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, RBraces.getType());
                Tokens.Add(new Token(match.Value, Token_Class.RCurlyBraces));
                return;
            }

            //Assign Operator
            match = Assign.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, Assign.getType());
                Tokens.Add(new Token(match.Value, Token_Class.AssignOperator));
                return;
            }
            
             //String
            match = StringInQuotes.Match(word);
            if (match.Success)
            {
                tokensDataTable.Rows.Add(match.Value, StringInQuotes.getType());
                Tokens.Add(new Token(match.Value, Token_Class.Constant));
                return;
            }
            errorList.Add("Line "+ line + " : Unrecognized token : " +  word);
            
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
