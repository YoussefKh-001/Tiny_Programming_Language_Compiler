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
        
        public Form1()
        {
            
            InitializeComponent();
        }

        private void CompileBtn_Click(object sender, EventArgs e)
        {
           
            String Code = String.Join(" ", CodeTextBox.Text);

            MatchCollection matches;
            Scanner scanner = new Scanner();
            scanner.StartScan(Code);
            /*for(int i = 0;i<Code.Length;i++)
            {
                String CurrentLine = Code[i];
                
                Match CommentMatch = CommentStatement.Match(CurrentLine);
                if (CommentMatch.Success)
                    tokensDataTable.Rows.Add(CommentMatch.Value, CommentStatement.getType());

                CurrentLine = Scanner.DeleteComments(CurrentLine);
                String CurrentLineWithStrings = CurrentLine;
                CurrentLine = Scanner.DeleteStrings(CurrentLine);

                String[] splittedWords = CurrentLine.Split(' ');
                foreach (String word in splittedWords)
                {
                    //Left Braces
                    Match match = LBraces.Match(word);
                    if (match.Success)
                        tokensDataTable.Rows.Add(match.Value, LBraces.getType());
                    //Identifier Phase
                    match = Identifier.Match(word);
                    if (match.Success && !DataTypes.isMatch(match.Value) && !ReservedWords.isMatch(match.Value))
                        tokensDataTable.Rows.Add(match.Value, Identifier.getType());
                    //Left Parentheses
                    match = LParentheses.Match(word);
                    if (match.Success)
                        tokensDataTable.Rows.Add(match.Value, LParentheses.getType());
                    //DataTypes Phase
                    match = DataTypes.Match(word);
                    if(match.Success)
                        tokensDataTable.Rows.Add(match.Value, DataTypes.getType() + "(" + match.Value.ToUpper() + ")");
                    //Reserved Words Phase
                    match = ReservedWords.Match(word);
                    if (match.Success)
                        tokensDataTable.Rows.Add(match.Value, ReservedWords.getType());
                    
                    //Comma Phase
                    match = Comma.Match(word);
                    if (match.Success)
                        tokensDataTable.Rows.Add(match.Value, Comma.getType());
                    //Right Parentheses
                    match = RParentheses.Match(word);
                    if (match.Success)
                        tokensDataTable.Rows.Add(match.Value, RParentheses.getType());
                    //Assign Operator
                    match = Assign.Match(word);
                    if (match.Success)
                        tokensDataTable.Rows.Add(match.Value, Assign.getType());
                    
                }

                Match StringMatch = StringInQuotes.Match(CurrentLineWithStrings);
                if (StringMatch.Success)
                    tokensDataTable.Rows.Add(StringMatch.Value, StringInQuotes.getType());

                //SemiColon Phase
                Match EndingMatch = Semicolon.Match(CurrentLine);
                if (EndingMatch.Success)
                    tokensDataTable.Rows.Add(EndingMatch.Value, Semicolon.getType());
                //Right Braces
                EndingMatch = RBraces.Match(CurrentLine);
                if (EndingMatch.Success)
                    tokensDataTable.Rows.Add(EndingMatch.Value, RBraces.getType());
            }*/
            
            
            


            TokensGridView.DataSource = scanner.tokensDataTable;
        }
    }
}
