using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tiny_Langauge_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;
        public List<string> Error_List;
        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        
        Node Program()
        {
           

            return null;
        }
        //Function Call
        Node FunctionCall()
        {
            //Done
            Node functionCall = new Node("Function Call");
            functionCall.Children.Add(match(Token_Class.Idenifier));
            functionCall.Children.Add(match(Token_Class.LParanthesis));
            functionCall.Children.Add(IdentifiersList());
            functionCall.Children.Add(match(Token_Class.RParanthesis));

            return functionCall;
        }

        Node IdentifiersList()
        {
            //Done
            Node identifierList = new Node("Identifier List");
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                identifierList.Children.Add(match(Token_Class.Idenifier));
                identifierList.Children.Add(Parameters());
            }else
            {
                return null;
            }
            return identifierList;
        }

        Node Parameters()
        {
            //Done
            Node parameters = new Node("Parameters");

            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                parameters.Children.Add(match(Token_Class.Comma));
                parameters.Children.Add(match(Token_Class.Idenifier));
                parameters.Children.Add(Parameters());
            }
            else
            {
                return null;
            }
            
            return parameters;
        }
        //TERM 
        Node Term()
        {
            //Done
            //identifier | Number | Function Call
            Node term = new Node("Term");
            if (Token[InputPointer].token_type == Token_Class.Idenifier && Token[InputPointer+1].token_type == Token_Class.LParanthesis)
            {
                 //function call
                term.Children.Add(FunctionCall());
            }
            if (Token[InputPointer].token_type == Token_Class.Idenifier)
            {
                //Identfier
                term.Children.Add(match(Token_Class.Idenifier));
            }
            else if (Token[InputPointer].token_type == Token_Class.Constant)
            {
                //Number
                term.Children.Add(match(Token_Class.Constant));
            }
            return term;
        }
        //Equation
        Node Equation()
        {
            //Done
            //MathExpression Equation’
            Node equation = new Node("Equation");
            //MathExpression
            equation.Children.Add(MathExpression());
            //EquationDash
            equation.Children.Add(EquationDash());
            return equation;
        }
        Node MathExpression()
        {
            //Done
            //Equation MathExpression' || MathTerm MathExpression’
            Node mathExpression = new Node("Math Expression");
            if (Token[InputPointer].token_type == Token_Class.Idenifier || Token[InputPointer].token_type == Token_Class.Constant ||Token[InputPointer].token_type == Token_Class.LParanthesis )
            {
                //Math Term
                mathExpression.Children.Add(MathTerm());
                mathExpression.Children.Add(MathExpressionDash());
            }
            else
            {
                //Equation
                mathExpression.Children.Add(Equation());
                mathExpression.Children.Add(MathExpressionDash());
            }
            return mathExpression;
        }
        Node MathExpressionDash()
        {
            //Done
            // e | MultOp MathExpression MathExpression’
            Node mathExpressionDash = new Node("Math Expression Dash");
            if (Token[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                //multiplyOp
                mathExpressionDash.Children.Add(match(Token_Class.MultiplyOp);
                mathExpressionDash.Children.Add(MathExpression());
                mathExpressionDash.Children.Add(MathExpressionDash());
            }
            else if (Token[InputPointer].token_type == Token_Class.DivideOp)
            {
                //Divide Op
                mathExpressionDash.Children.Add(match(Token_Class.DivideOp);
                mathExpressionDash.Children.Add(MathExpression());
                mathExpressionDash.Children.Add(MathExpressionDash());
            }
            else
            {
                //epsilon
                return null;
            }
            return mathExpressionDash;

        }
        Node MathTerm()
        {
            //Done
            // Term | (Equation)
            Node mathTerm = new Node("Math Term");
            if (Token[InputPointer].token_type == Token_Class.LParanthesis)
            {
                // (Equation)
                mathTerm.Children.Add(match(Token_Class.LParanthesis));
                mathTerm.Children.Add(Equation());
                mathTerm.Children.Add(match(Token_Class.RParanthesis));
            }
            else
            {
                //Term
                mathTerm.Children.Add(Term());
            }
            return mathTerm;
        }
        Node EquationDash()
        {
            //Done
            //ADD OP mathexpression || e
            Node equationDash = new Node("Equation Dash");
            if (Token[InputPointer].token_type == Token_Class.PlusOp)
            {
                //plus Op
                equationDash.Children.Add(match(Token_Class.PlusOp));
                equationDash.Children.Add(MathExpression());
            }
            else if (Token[InputPointer].token_type == Token_Class.MinusOp)
            {
                //minus OP
                equationDash.Children.Add(match(Token_Class.MinusOp));
                equationDash.Children.Add(MathExpression());
            }
            else
            {
                //epsilon
                return null;
            }
            return equationDash;
   
        }
        //Assignment Statment
        Node AssignmentStatment()
        {
            //Done
            //identifier :=  Expression
            Node assignmentStatment = new Node("Assignment Statment");
            assignmentStatment.Children.Add(match(Token_Class.Idenifier));
            assignmentStatment.Children.Add(match(Token_Class.AssignOperator));
            assignmentStatment.Children.Add(Expression());
            return assignmentStatment;
        }
       Node Expression()
        {
            //Done
            //Term | string | Equation
            Node expression = new Node ("Expression");
            if (Token[InputPointer].token_type == Token_Class.String)
            {
                //string
                expression.Children.Add(match(Token_Class.String));
            }
            else if (Token[InputPointer].token_type == Token_Class.Idenifier || Token[InputPointer].token_type == Token_Class.Constant)
            {
                //Term
                expression.Children.Add(Term);
            }
            else
            {
                //Equation
                expression.Children.Add(Equation());
            }

            return expression;
        }
        // Implement your logic here

        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}
