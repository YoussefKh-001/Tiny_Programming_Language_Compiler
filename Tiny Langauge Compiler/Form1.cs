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

            Scanner scanner = new Scanner();
            scanner.StartScan(Code);
            Parser parser = new Parser();
            parser.StartParsing(scanner.Tokens);
            ErrorListBox.Items.Clear();
            foreach(string error in scanner.errorList)
            {
                ErrorListBox.Items.Add(error);
            }
            foreach (string error in parser.Error_List)
            {
                ErrorListBox.Items.Add(error);
            }

            TokensGridView.DataSource = scanner.tokensDataTable;
            ParseTree.Nodes.Clear();
            ParseTree.Nodes.Add(Parser.PrintParseTree(parser.root));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
