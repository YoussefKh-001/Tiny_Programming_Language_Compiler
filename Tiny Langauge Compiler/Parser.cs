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

        public Parser()
        {
            Error_List = new List<string>();
        }
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
            // FunctionStatements MainFunction
            Node program = new Node("Program");

            program.Children.Add(FunctionStatements());
            program.Children.Add(MainFunction());

            return program;
        }
        
        Node FunctionCallAsTerm()
        {
            //Identifier (IdentifiersList)
            Node functionCallAsTerm = new Node("Function Call As Term");

            functionCallAsTerm.Children.Add(match(Token_Class.FunctionIdentifier));
            functionCallAsTerm.Children.Add(match(Token_Class.LParanthesis));
            functionCallAsTerm.Children.Add(IdentifiersList());
            functionCallAsTerm.Children.Add(match(Token_Class.RParanthesis));

            return functionCallAsTerm;
        }
        Node FunctionCall()
        {
            // FunctionCallAsTerm ;
            Node functionCall = new Node("Function Call");

            
            functionCall.Children.Add(FunctionCallAsTerm());
            functionCall.Children.Add(match(Token_Class.Semicolon));
            if (TokenStream[InputPointer - 1].token_type != Token_Class.Semicolon)
            {
                InputPointer--;
                return null;
            }
            return functionCall;
        }
  
        Node IdentifiersList()
        {
            // Expression Parameters | epsilon
            Node identifiersList = new Node("Identifiers List");
            try
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Constant|| TokenStream[InputPointer].token_type == Token_Class.StringConstant)
                {
                    identifiersList.Children.Add(Expression());
                    identifiersList.Children.Add(Parameters());
                }
                else
                {
                    return null;
                }
            }catch(Exception x)
            {
                return null;
            }
            
            return identifiersList;
        }

        Node Parameters()
        {
            // ,Expression Parameters | epsilon
            Node parameters = new Node("Parameters");
            try
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    parameters.Children.Add(match(Token_Class.Comma));
                    parameters.Children.Add(Expression());
                    parameters.Children.Add(Parameters());
                }
                else
                {
                    return null;
                }
            }
            catch (Exception x) { return null; }
            
            return parameters;
        }
        
        Node Term()
        {
            // identifier | number | FunctionCallAsParameter
            Node term = new Node("Term");

            if (TokenStream[InputPointer].token_type == Token_Class.FunctionIdentifier)
            {
                 // Functioncall
                term.Children.Add(FunctionCallAsTerm());
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
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
            // Equation MathExpression' | MathTerm MathExpression’
            Node mathExpression = new Node("Math Expression");
        
            if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Constant || TokenStream[InputPointer].token_type == Token_Class.LParanthesis || TokenStream[InputPointer].token_type == Token_Class.FunctionIdentifier && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
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

            try
            {
                if (TokenStream[InputPointer].token_type == Token_Class.MultiplyOp)
                {
                    mathExpressionDash.Children.Add(match(Token_Class.MultiplyOp));
                    if (TokenStream[InputPointer].token_type == Token_Class.Semicolon || TokenStream[InputPointer].token_type == Token_Class.Comma)
                    {
                        Error_List.Add("Parsing Error: Expected "
                       + "Math Term" + " and " +
                       TokenStream[InputPointer].token_type.ToString() +
                       " found\r\n");
                        return null; 
                    }
                    mathExpressionDash.Children.Add(MathExpression());
                    mathExpressionDash.Children.Add(MathExpressionDash());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.DivideOp)
                {
                    mathExpressionDash.Children.Add(match(Token_Class.DivideOp));
                    if (TokenStream[InputPointer].token_type == Token_Class.Semicolon || TokenStream[InputPointer].token_type == Token_Class.Comma)
                    {
                        Error_List.Add("Parsing Error: Expected "
                       + "Math Term" + " and " +
                       TokenStream[InputPointer].token_type.ToString() +
                       " found\r\n");
                        return null;
                    }
                    mathExpressionDash.Children.Add(MathExpression());
                    mathExpressionDash.Children.Add(MathExpressionDash());
                }
                else
                {
                    return null;
                }
            }catch(Exception x)
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
            try
            {
                if (TokenStream[InputPointer].token_type == Token_Class.PlusOp)
                {
                    equationDash.Children.Add(match(Token_Class.PlusOp));
                    if (TokenStream[InputPointer].token_type == Token_Class.Semicolon || TokenStream[InputPointer].token_type == Token_Class.Comma)
                    {
                        Error_List.Add("Parsing Error: Expected "
                       + "Math Term" + " and " +
                       TokenStream[InputPointer].token_type.ToString() +
                       " found\r\n");
                        return null;
                    }
                    equationDash.Children.Add(MathExpression());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.MinusOp)
                {
                    equationDash.Children.Add(match(Token_Class.MinusOp));
                    if (TokenStream[InputPointer].token_type == Token_Class.Semicolon || TokenStream[InputPointer].token_type == Token_Class.Comma)
                    {
                        Error_List.Add("Parsing Error: Expected "
                       + "Math Term" + " and " +
                       TokenStream[InputPointer].token_type.ToString() +
                       " found\r\n");
                        return null;
                    }
                    equationDash.Children.Add(MathExpression());
                }
                else
                {
                    return null;
                }
            }
            catch(Exception x)
            {
                return null;
            }

            return equationDash;
        }
        //Assignment Statment
        Node AssignmentStatement()
        {
            //identifier :=  Expression;
            Node assignmentStatment = new Node("Assignment Statement");
           
            assignmentStatment.Children.Add(match(Token_Class.Idenifier));
            assignmentStatment.Children.Add(match(Token_Class.AssignOperator));
            if (TokenStream[InputPointer - 1].token_type != Token_Class.AssignOperator)
            {
                InputPointer--;
                return null;
            }
            else
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Semicolon)
                {
                    Error_List.Add("Parsing Error: Expected "
                    + Token_Class.Constant.ToString() + " and " +
                    " Semicolon" +
                    " found\r\n");
                    return null;
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    Error_List.Add("Parsing Error: Expected "
                    + Token_Class.Constant.ToString() + " and " +
                    " Comma" +
                    " found\r\n");
                    return null;
                }
            }
            assignmentStatment.Children.Add(Expression());
            assignmentStatment.Children.Add(match(Token_Class.Semicolon));
            if (TokenStream[InputPointer - 1].token_type != Token_Class.Semicolon)
            {
                InputPointer--;
                return null;
            }
            return assignmentStatment;
        }

       Node Expression()
       {
            // Term | string | Equation
            Node expression = new Node ("Expression");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.StringConstant)
                {
                    expression.Children.Add(match(Token_Class.StringConstant));
                }
                else
                {
                    expression.Children.Add(Equation());
                }
            }else
            {
                return null;
            }

            return expression;
       }
       
        Node DeclarationStatement()
        {
            // DataType IdentifiersList;
            Node declarationStatement = new Node("Declaration Statement");

            declarationStatement.Children.Add(DataType());
            declarationStatement.Children.Add(IdentifiersDeclarationList());
            declarationStatement.Children.Add(match(Token_Class.Semicolon));
            if (TokenStream[InputPointer - 1].token_type != Token_Class.Semicolon)
            {
                InputPointer--;
                return null;
            }
            return declarationStatement;
        }

        Node IdentifiersDeclarationList()
        {
            //
            Node identifiersDeclarationList = new Node("Identifiers Declaration List");

            identifiersDeclarationList.Children.Add(match(Token_Class.Idenifier));
            if (TokenStream[InputPointer - 1].token_type != Token_Class.Idenifier)
            {
                if (TokenStream[InputPointer].token_type != Token_Class.Semicolon)
                    InputPointer--;
                
                return null;
            }
            identifiersDeclarationList.Children.Add(AssignOrNot());
            identifiersDeclarationList.Children.Add(DeclarationParameters());

            return identifiersDeclarationList;
        }

        Node AssignOrNot()
        {
            //
            Node assignOrNot = new Node("Assign Or Not");

            try
            {
                if (TokenStream[InputPointer].token_type == Token_Class.AssignOperator)
                {
                    assignOrNot.Children.Add(match(Token_Class.AssignOperator));
                    if(TokenStream[InputPointer].token_type == Token_Class.Semicolon)
                    {
                        Error_List.Add("Parsing Error: Expected "
                        + Token_Class.Constant.ToString() + " and " +
                        " Semicolon" +
                        " found\r\n");
                        return null;
                    }
                    else if(TokenStream[InputPointer].token_type == Token_Class.Comma)
                    {
                        Error_List.Add("Parsing Error: Expected "
                        + Token_Class.Constant.ToString() + " and " +
                        " Comma" +
                        " found\r\n");
                        return null;
                    }
                    assignOrNot.Children.Add(Expression());
                } else return null;
            }catch(Exception e)
            {
                return null;
            }

            return assignOrNot;
        }

        Node DeclarationParameters()
        {
            // ,identifier Parameters | epsilon
            Node declarationParameters = new Node("Declaration Parameters");
            try
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    declarationParameters.Children.Add(match(Token_Class.Comma));
                    declarationParameters.Children.Add(match(Token_Class.Idenifier));
                    declarationParameters.Children.Add(AssignOrNot());
                    declarationParameters.Children.Add(DeclarationParameters());
                }
                else
                {
                    return null;
                }
            }
            catch (Exception x) { return null; }

            return declarationParameters;
        }
        Node AssignOrCommaOrNothing()
        {
            //:= Expression AssignOrCommaOrNothing | , identifier AssignOrCommaOrNothing | ε
            Node assignOrCommaOrNothing = new Node("Assign or comma or nothing");
            try {
                
                if (TokenStream[InputPointer].token_type == Token_Class.AssignOperator)
                {
                     assignOrCommaOrNothing.Children.Add(match(Token_Class.AssignOperator));
                     assignOrCommaOrNothing.Children.Add(Expression());
                     assignOrCommaOrNothing.Children.Add(AssignOrCommaOrNothing());
                }
            else if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                assignOrCommaOrNothing.Children.Add(match(Token_Class.Comma));
                assignOrCommaOrNothing.Children.Add(match(Token_Class.Idenifier));
                assignOrCommaOrNothing.Children.Add(AssignOrCommaOrNothing());
            }
            else
            {
                return null;
            }
           
            }catch(Exception e)
            {
                return null;
            }
            return assignOrCommaOrNothing;
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
            Node Write = WriteTerminal();
            if (Write == null)return null;
            writeStatement.Children.Add(Write);
            writeStatement.Children.Add(match(Token_Class.Semicolon));
            if (TokenStream[InputPointer - 1].token_type != Token_Class.Semicolon)
            {
                InputPointer--;
                return null;
            }
            return writeStatement;
        }

        Node WriteTerminal()
        {
            // Expression | endl
            Node writeTeminal = new Node("Write Terminal");

            if (TokenStream[InputPointer].token_type == Token_Class.Endl)
                writeTeminal.Children.Add(match(Token_Class.Endl));
            else if (TokenStream[InputPointer].token_type == Token_Class.Constant)
            {
                writeTeminal.Children.Add(match(Token_Class.Constant));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.StringConstant)
            {
                writeTeminal.Children.Add(match(Token_Class.StringConstant));
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Semicolon)
            {
                Error_List.Add("Parsing Error: Expected "
                        + Token_Class.Idenifier.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                match(Token_Class.Semicolon);
                return null;
            }
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier && TokenStream[InputPointer+1].token_type == Token_Class.AssignOperator)
            {
                Error_List.Add("Parsing Error: Expected "
                        + Token_Class.Idenifier.ToString() + " and " +
                        "Assignment Statment" +
                        "  found\r\n");
                return null;
            }
            //WriteStatement Statements| ReadStatement Statements | AssignmentStatement Statements | DeclarationStatement Statements | ReturnStatement Statements | IfStatement Statements | RepeatStatement Statements | FunctionCall Statements| ε
            else if (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.String || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.Write || TokenStream[InputPointer].token_type == Token_Class.Read || TokenStream[InputPointer].token_type == Token_Class.Repeat || TokenStream[InputPointer].token_type == Token_Class.If || TokenStream[InputPointer].token_type == Token_Class.ElseIf || TokenStream[InputPointer].token_type == Token_Class.Else || TokenStream[InputPointer].token_type == Token_Class.Return)
            {
                Error_List.Add("Parsing Error: Expected "
                        + Token_Class.Idenifier.ToString() + " and " +
                        " Other Statments" +
                        " found\r\n");
                return null;
            }
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
            if (TokenStream[InputPointer - 1].token_type != Token_Class.Idenifier)
            {
                if (TokenStream[InputPointer - 1].token_type != Token_Class.Semicolon)
                {
                    InputPointer--;
                    return null;
                }
                else
                {
                    
                    return null;
                }

            }
            readStatement.Children.Add(match(Token_Class.Semicolon));
            if (TokenStream[InputPointer - 1].token_type != Token_Class.Semicolon)
            {
                InputPointer--;
                if (TokenStream[InputPointer].token_type == Token_Class.AssignOperator)
                    InputPointer--;
                return null;
            }
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
            try
            {
                if (TokenStream[InputPointer].token_type == Token_Class.ANDOp || TokenStream[InputPointer].token_type == Token_Class.OROp)
                {
                    conditionStatementDash.Children.Add(BoolOeprator());
                    conditionStatementDash.Children.Add(Condition());
                    conditionStatementDash.Children.Add(ConditionStatementDash());
                }
                else
                {
                    return null;
                }
            }catch(Exception x)
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
            else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier || TokenStream[InputPointer].token_type == Token_Class.Constant || TokenStream[InputPointer].token_type == Token_Class.FunctionIdentifier && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
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

            if (TokenStream[InputPointer].token_type == Token_Class.ANDOp)
            {
                boolOperator.Children.Add(match(Token_Class.ANDOp));
            }else if (TokenStream[InputPointer].token_type == Token_Class.OROp)
            {
                boolOperator.Children.Add(match(Token_Class.OROp));
            }

            return boolOperator;
        }

        Node ReturnStatement()
        {
            // return Expression;
            Node returnStatement = new Node("Return Statement");

            Node ret = match(Token_Class.Return);
            if (ret != null)
                returnStatement.Children.Add(ret);
            else
            {
                InputPointer--;
                return null;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.RCurlyBraces)
            { 
                //put it in the error list
                Error_List.Add("Parsing Error: Expected "
                        + Token_Class.Constant + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                return null;
            }
            if (TokenStream[InputPointer].token_type == Token_Class.Semicolon)
            {
                match(Token_Class.Constant);
                return null;
            }
            returnStatement.Children.Add(Expression());
            returnStatement.Children.Add(match(Token_Class.Semicolon));
            if (TokenStream[InputPointer - 1].token_type != Token_Class.Semicolon)
            {
                InputPointer--;
                return null;
            }
            return returnStatement;
        }

        Node IfStatement()
        {
            // if ConditionStatement then Statements ElseIfStatement
            Node ifStatement = new Node("If Statement");

          
            ifStatement.Children.Add(match(Token_Class.If));
            ifStatement.Children.Add(ConditionStatement());
            ifStatement.Children.Add(match(Token_Class.Then));
            ifStatement.Children.Add(Statements());
            ifStatement.Children.Add(ElseIfStatement());

            return ifStatement;
        }

        Node ElseIfStatement()
        {
            // elseif ConditionStatement then Statements ElseIfStatement | ElseStatement | end
            Node elseIfStatement = new Node("Else If Statement");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.ElseIf)
                {
                    elseIfStatement.Children.Add(match(Token_Class.ElseIf));
                    elseIfStatement.Children.Add(ConditionStatement());
                    elseIfStatement.Children.Add(match(Token_Class.Then));
                    elseIfStatement.Children.Add(Statements());
                    elseIfStatement.Children.Add(ElseIfStatement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Else)
                {
                    elseIfStatement.Children.Add(ElseStatement());
                }
                else
                {
                    elseIfStatement.Children.Add(match(Token_Class.End));
                }
            }
            return elseIfStatement;
        }

        Node ElseStatement()
        {
            // else Statements end 
            Node elseStatement = new Node("Else Statement");

            elseStatement.Children.Add(match(Token_Class.Else));
            elseStatement.Children.Add(Statements());
            elseStatement.Children.Add(match(Token_Class.End));

            return elseStatement;
        }

        Node RepeatStatement()
        {
            // repeat Statements until ConditionStatement
            Node repeatStatement = new Node("Repeat Statement");

            repeatStatement.Children.Add(match(Token_Class.Repeat));
            repeatStatement.Children.Add(Statements());
            repeatStatement.Children.Add(match(Token_Class.Until));
            if (TokenStream[InputPointer-1].token_type != Token_Class.Until)
            {
                InputPointer--;
                return null;
            }
            repeatStatement.Children.Add(ConditionStatement());

            return repeatStatement;
        }

        Node FunctionDeclaration()
        {
            // Dataype identifier(ArgumentsList)
            Node functionDeclaration = new Node("Function Declaration");

            functionDeclaration.Children.Add(DataType());
            functionDeclaration.Children.Add(match(Token_Class.FunctionIdentifier));
            functionDeclaration.Children.Add(match(Token_Class.LParanthesis));
            functionDeclaration.Children.Add(ArgumentsList());
            functionDeclaration.Children.Add(match(Token_Class.RParanthesis));

            return functionDeclaration;
        }

        Node ArgumentsList()
        {
            // Argument Arguments | epsilon
            Node argumentsList = new Node("Arguments List");
            try
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    argumentsList.Children.Add(Argument());
                    argumentsList.Children.Add(Arguments());
                }
                else
                {
                    return null;
                }
            }catch(Exception x)
            {
                return null;
            }

            return argumentsList;
        }

        Node Arguments()
        {
            // ,Argument Arguments | epsilon 
            Node arguments = new Node("Arguments");
            try
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    arguments.Children.Add(match(Token_Class.Comma));
                    arguments.Children.Add(Argument());
                    arguments.Children.Add(Arguments());
                }
                else
                {
                    return null;
                }
            }catch(Exception x)
            {
                return null;
            }

            return arguments;
        }

        Node Argument()
        {
            // DataType identifier
            Node argument = new Node("Argument");

            argument.Children.Add(DataType());
            argument.Children.Add(match(Token_Class.Idenifier));

            return argument;
        }
       
        Node FunctionBody()
        {
            // {Statements ReturnStatements}
            Node functionBody = new Node("Function Body");

            functionBody.Children.Add(match(Token_Class.LCurlyBraces));
            functionBody.Children.Add(Statements());
            functionBody.Children.Add(ReturnStatement());
            functionBody.Children.Add(match(Token_Class.RCurlyBraces));

            return functionBody;
        }

        Node FunctionStatement()
        {
            // FunctionDeclaration FunctionBody
            Node functionStatement = new Node("Function Statement");

            functionStatement.Children.Add(FunctionDeclaration());
            functionStatement.Children.Add(FunctionBody());
      

            return functionStatement;
        }
        Node Statements()
        {
            /* WriteStatement Statements| ReadStatement Statements | AssignmentStatement Statements 
                | DeclarationStatement Statements | ReturnStatement Statements 
                | IfStatement Statements | RepeatStatement Statements | FunctionCall Statements 
                | epsilon */
            Node statements = new Node("Statements");
            if (InputPointer < TokenStream.Count)
            {
                try
                {
                    if (TokenStream[InputPointer].token_type == Token_Class.Write)
                    {
                        statements.Children.Add(WriteStatement());
                        statements.Children.Add(Statements());
                    }
                    else if (TokenStream[InputPointer].token_type == Token_Class.Read)
                    {
                        statements.Children.Add(ReadStatement());
                        statements.Children.Add(Statements());
                    }
                    else if (TokenStream[InputPointer].token_type == Token_Class.Idenifier)
                    {
                        statements.Children.Add(AssignmentStatement());
                        statements.Children.Add(Statements());
                    }
                    else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
                    {
                        statements.Children.Add(RepeatStatement());
                        statements.Children.Add(Statements());
                    }
                    else if (TokenStream[InputPointer].token_type == Token_Class.If)
                    {
                        statements.Children.Add(IfStatement());
                        statements.Children.Add(Statements());
                    }
                    else if (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String)
                    {
                        statements.Children.Add(DeclarationStatement());
                        statements.Children.Add(Statements());
                    }
                    else if (TokenStream[InputPointer].token_type == Token_Class.FunctionIdentifier)
                    {
                        statements.Children.Add(FunctionCall());
                        
                    }
                    else if (TokenStream[InputPointer].token_type == Token_Class.Return || TokenStream[InputPointer].token_type == Token_Class.Until || TokenStream[InputPointer].token_type == Token_Class.End || TokenStream[InputPointer].token_type == Token_Class.Else || TokenStream[InputPointer].token_type == Token_Class.ElseIf)
                    {
                        return null;
                    }
                    else
                    {
                        Error_List.Add("Parsing Error: Expected "
                        + "Statement" + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                        InputPointer++;
                        statements.Children.Add(Statements());
                    }
                   
                }
                catch (Exception x)
                {
                    return null;
                }
            }
            return statements;
        }

        Node MainFunction()
        {
            // DataType main() FunctionBody
            Node mainFunction = new Node("Main Function");

            mainFunction.Children.Add(DataType());
            mainFunction.Children.Add(match(Token_Class.Main));
            mainFunction.Children.Add(match(Token_Class.LParanthesis));
            mainFunction.Children.Add(match(Token_Class.RParanthesis));
            mainFunction.Children.Add(FunctionBody());

            return mainFunction;
        }

        Node FunctionStatements()
        {
            // FunctionStatment FunctionStatements| epsilon
            Node functionStatements = new Node("Function Statements");
            try
            {

                if (TokenStream[InputPointer].token_type == Token_Class.Integer || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    if (TokenStream[InputPointer + 1].token_type == Token_Class.FunctionIdentifier)
                    {
                        functionStatements.Children.Add(FunctionStatement());
                        functionStatements.Children.Add(FunctionStatements());
                    }
                    else
                        return null;

                }
                else
                {
                    return null;
                }
            }catch(Exception x) { return null; }

            return functionStatements;
        }
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    if (InputPointer < TokenStream.Count - 1)
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
