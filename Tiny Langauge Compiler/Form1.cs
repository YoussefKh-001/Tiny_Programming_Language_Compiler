using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace Tiny_Langauge_Compiler
{
    public partial class Form1 : Form
    {
        ScanningPhase Number,StringInQuotes,ReservedWords,CommentStatement,Identifier,FunctionCall;
        public Form1()
        {
            Number = new ScanningPhase(@"(\d+(\.\d*)?)", "Number");
            StringInQuotes = new ScanningPhase(@"(""[^""]*"")", "String");
            ReservedWords = new ScanningPhase(@"(int|float|string|read|write|repeat|until|if|elseif|else|return|then|endl)", "Reserved Word");
            CommentStatement = new ScanningPhase(@"(/\*([^(/\*)^(\*/)]|\s*)*\*/)", "Comment");
            Identifier = new ScanningPhase(@"([a-zA-Z_][a-zA-Z0-9_]*)", "Identifier");
            FunctionCall = new ScanningPhase(String.Format(@"{0}\(({0}\s*(,\s* {0})*)?\)", Identifier.getRegularExpression()), "FunctionCall");

            InitializeComponent();
        }

        private void CompileBtn_Click(object sender, EventArgs e)
        {
            DataTable tokensDataTable = new DataTable();
            tokensDataTable.Columns.Add("Token");
            tokensDataTable.Columns.Add("Type");
            foreach(String line in CodeTextBox.Lines)
            {
                //Numbers Phase
                MatchCollection matchCollection = new Regex(Number.getRegularExpression()).Matches(line);
                foreach(Match match in matchCollection)
                {
                    tokensDataTable.Rows.Add(match.Value, Number.getType());
                }

                //String Phase
                matchCollection = new Regex(StringInQuotes.getRegularExpression()).Matches(line);
                foreach (Match match in matchCollection)
                {
                    tokensDataTable.Rows.Add(match.Value, StringInQuotes.getType());
                }

                //Reserved Words Phase
                matchCollection = new Regex(ReservedWords.getRegularExpression()).Matches(line);
                foreach (Match match in matchCollection)
                {
                    tokensDataTable.Rows.Add(match.Value, ReservedWords.getType());
                }

                //Comment Phase
                matchCollection = new Regex(CommentStatement.getRegularExpression()).Matches(line);
                foreach (Match match in matchCollection)
                {
                    tokensDataTable.Rows.Add(match.Value, CommentStatement.getType());
                }

                //Identifier Phase
                matchCollection = new Regex(Identifier.getRegularExpression()).Matches(line);
                foreach (Match match in matchCollection)
                {
                    tokensDataTable.Rows.Add(match.Value, Identifier.getType());
                }

                //Function Call Phase
                matchCollection = new Regex(FunctionCall.getRegularExpression()).Matches(line);
                foreach (Match match in matchCollection)
                {
                    tokensDataTable.Rows.Add(match.Value, FunctionCall.getType());
                }
            }
            TokensGridView.DataSource = tokensDataTable;
        }
    }
}
