﻿using System;
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
        
        Node FunctionCall()
        {
            // identifier (IdentifiersList)
            Node functionCall = new Node("Function Call");

            functionCall.Children.Add(match(Token_Class.Idenifier));
            functionCall.Children.Add(match(Token_Class.LParanthesis));
            functionCall.Children.Add(IdentifiersList());
            functionCall.Children.Add(match(Token_Class.RParanthesis));

            return functionCall;
        }

        Node IdentifiersList()
        {
            // identifier Parameters | epsilon
            Node identifiersList = new Node("Identifiers List");
            
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                identifiersList.Children.Add(match(Token_Class.Idenifier));
                identifiersList.Children.Add(Parameters());
            }else
            {
                return null;
            }
            
            return identifiersList;
        }

        Node Parameters()
        {
            // ,identifier Parameters | epsilon
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
        
        Node Term()
        {
            // identifier | number | FunctionCall
            Node term = new Node("Term");

            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
            {
                 // Functioncall
                term.Children.Add(FunctionCall());
            }
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                term.Children.Add(match(Token_Class.Idenifier));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Constant)
            {
                // Number
                term.Children.Add(match(Token_Class.Constant));
            }

            return term;
        }
        
        Node Equation()
        {
            //MathExpression Equation’
            Node equation = new Node("Equation");
         
            equation.Children.Add(MathExpression());
            equation.Children.Add(EquationDash());
            
            return equation;
        }
        
        Node MathExpression()
        {
            // Equation MathExpression' || MathTerm MathExpression’
            Node mathExpression = new Node("Math Expression");
        
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Constant || TokenStream[InputPointer].token_type == Token_Class.LParanthesis || TokenStream[InputPointer].token_type == Token_Class.Idenifier && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
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
            // epsilon | MultOp MathExpression MathExpression’
            Node mathExpressionDash = new Node("Math Expression Dash");
            
            if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
            {
                mathExpressionDash.Children.Add(match(Token_Class.MultiplyOp));
                mathExpressionDash.Children.Add(MathExpression());
                mathExpressionDash.Children.Add(MathExpressionDash());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
            {
                mathExpressionDash.Children.Add(match(Token_Class.DivideOp));
                mathExpressionDash.Children.Add(MathExpression());
                mathExpressionDash.Children.Add(MathExpressionDash());
            }
            else
            {
                return null;
            }

            return mathExpressionDash;
        }

        Node MathTerm()
        {
            // Term | (Equation)
            Node mathTerm = new Node("Math Term");
            
            if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                // (Equation)
                mathTerm.Children.Add(match(Token_Class.LParanthesis));
                mathTerm.Children.Add(Equation());
                mathTerm.Children.Add(match(Token_Class.RParanthesis));
            }
            else
            {
                // Term
                mathTerm.Children.Add(Term());
            }
            return mathTerm;
        }
        
        Node EquationDash()
        {
            // AddOp MathExpression | epsilon
            Node equationDash = new Node("Equation Dash");
            
            if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
            {
                equationDash.Children.Add(match(Token_Class.PlusOp));
                equationDash.Children.Add(MathExpression());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
            {
                equationDash.Children.Add(match(Token_Class.MinusOp));
                equationDash.Children.Add(MathExpression());
            }
            else
            {
                return null;
            }

            return equationDash;
        }
        //Assignment Statment
        Node AssignmentStatment()
        {
            //identifier :=  Expression
            Node assignmentStatment = new Node("Assignment Statement");
           
            assignmentStatment.Children.Add(match(Token_Class.Idenifier));
            assignmentStatment.Children.Add(match(Token_Class.AssignOperator));
            assignmentStatment.Children.Add(Expression());
            
            return assignmentStatment;
        }

       Node Expression()
       {
            // Term | string | Equation
            Node expression = new Node ("Expression");
            
            if (TokenStream[InputPointer].token_type == Token_Class.String)
            {
                expression.Children.Add(match(Token_Class.String));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Constant)
            {
                expression.Children.Add(Term());
            }
            else
            {
                expression.Children.Add(Equation());
            }

            return expression;
       }
       
        Node DeclarationStatement()
        {
            // DataType IdentifiersList
            Node declarationStatement = new Node("Declaration Statement");

            declarationStatement.Children.Add(DataType());
            declarationStatement.Children.Add(IdentifiersList());

            return declarationStatement;
        }

        Node DataType()
        {
            // int | float | string
            Node dataType = new Node("DataType");

            if (TokenStream[InputPointer].token_type == Token_Class.Integer)
            {
                dataType.Children.Add(match(Token_Class.Integer));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Float)
            {
                dataType.Children.Add(match(Token_Class.Float));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.String)
            {
                dataType.Children.Add(match(Token_Class.String));
            }

            return dataType;

        }

        Node WriteStatement()
        {
            // write WriteTerminal;
            Node writeStatement = new Node("Write Statement");

            writeStatement.Children.Add(match(Token_Class.Write));
            writeStatement.Children.Add(WriteTerminal());
            writeStatement.Children.Add(match(Token_Class.Semicolon));

            return writeStatement;
        }

        Node WriteTerminal()
        {
            // Expression | endl
            Node writeTeminal = new Node("Write Terminal");

            if (TokenStream[InputPointer].token_type == Token_Class.Endl)
                writeTeminal.Children.Add(match(Token_Class.Endl));
            else
                writeTeminal.Children.Add(Expression());

            return writeTeminal;
        }

        Node ReadStatement()
        {
            // read identifier;
            Node readStatement = new Node("Read Statement");

            readStatement.Children.Add(match(Token_Class.Read));
            readStatement.Children.Add(match(Token_Class.Idenifier));
            readStatement.Children.Add(match(Token_Class.Semicolon));

            return readStatement;
        }

        Node ConditionStatement()
        {
            // Condition ConditionStatement’
            Node conditionStatement = new Node("Condition Statement");

            conditionStatement.Children.Add(Condition());
            conditionStatement.Children.Add(ConditionStatementDash());

            return conditionStatement;
        }

        Node ConditionStatementDash()
        {
            // BoolOp Condition | epsilon
            Node conditionStatementDash = new Node("Conidtion Statement Dash");

            if (TokenStream[InputPointer].token_type == Token_Class.ANDOp && TokenStream[InputPointer].token_type == Token_Class.OROp)
            {
                conditionStatementDash.Children.Add(BoolOeprator());
                conditionStatementDash.Children.Add(Condition());
            }else
            {
                return null;
            }

            return conditionStatementDash;

        }
        Node Condition()
        {
            // Term ConditionalOperator Term | (ConditionStatement) | ConditionStatement
            Node condition = new Node("Condition");

            if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
            {
                condition.Children.Add(match(Token_Class.LParanthesis));
                condition.Children.Add(ConditionStatement());
                condition.Children.Add(match(Token_Class.RParanthesis));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Constant || TokenStream[InputPointer].token_type == Token_Class.Idenifier && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
            {
                condition.Children.Add(Term());
                condition.Children.Add(ConditionalOperator());
                condition.Children.Add(Term());
            }
            else
            {
                condition.Children.Add(ConditionStatement());
            }

            return condition;
        }

        Node ConditionalOperator()
        {
            // > | < | <> | =
            Node conditionalOperator = new Node("Conditional Operator");

            if (TokenStream[InputPointer].token_type == Token_Class.EqualOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.EqualOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThanOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.GreaterThanOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.LessThanOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.LessThanOp));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.NotEqualOp)
            {
                conditionalOperator.Children.Add(match(Token_Class.NotEqualOp));
            }

            return conditionalOperator;
        }

        Node BoolOeprator()
        {
            // && | ||
            Node boolOperator = new Node("Boolean Operator");

            if (TokenStream[InputPointer].token_type == Token_Class.ANDOp && TokenStream[InputPointer + 1].token_type == Token_Class.ANDOp)
            {
                boolOperator.Children.Add(match(Token_Class.ANDOp));
                boolOperator.Children.Add(match(Token_Class.ANDOp));
            }else if (TokenStream[InputPointer].token_type == Token_Class.OROp && TokenStream[InputPointer + 1].token_type == Token_Class.OROp)
            {
                boolOperator.Children.Add(match(Token_Class.OROp));
                boolOperator.Children.Add(match(Token_Class.OROp));
            }

            return boolOperator;
        }

        Node ReturnStatement()
        {
            // return Expression;
            Node returnStatement = new Node("Return Statement");

            returnStatement.Children.Add(match(Token_Class.Return));
            returnStatement.Children.Add(Expression());
            returnStatement.Children.Add(match(Token_Class.Semicolon));

            return returnStatement;
        }

        Node Statements()
        {
            // WriteStatement| ReadStatement| AssignmentStatement | DeclarationStatement | ReturnStatement | IfStatement | RepeatStatement
            Node statements = new Node("Statements");

            if (TokenStream[InputPointer].token_type == Token_Class.Write)
            {
                statements.Children.Add(WriteStatement());
            }else if (TokenStream[InputPointer].token_type == Token_Class.Read)
            {
                statements.Children.Add(ReadStatement());
            }else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
            {
                statements.Children.Add(AssignmentStatement());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                statements.Children.Add(ReturnStatement());
            }
            

        }
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
