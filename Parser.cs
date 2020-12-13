﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AssignmentASE
{
    /// <summary>
    /// Responsible for parsing commands from command line and code editor
    /// </summary>
    public class Parser
    {
        // The painter object where apporopriate methods are called
        Painter p;
        String error;
        Lexer lexer;
        // A dictionary for stroring variables
        public Dictionary<string, string> Variables { get; set; }


        /// <summary>
        /// Parameterized Constructor to initialize the painter object
        /// </summary>
        /// <param name="p">The painter object which knows where to draw on</param>
        public Parser(Painter p)
        {
            this.p = p;
            this.lexer = new Lexer();
            error = "";
            Variables = new Dictionary<string, string>();
        }


        /// <summary>
        /// Parses a single string input
        /// </summary>
        /// <param name="input">The string to be parsed</param>
        /// <param name="lineNum">The current line number</param>
        /// <remarks>The line number is 0 when a single command is to be parsed</remarks>
        public void parseCommand(string input, int lineNum)
        {
            // Tidy the input string
            input = input.ToLower().Trim();

            // Split the input on a single space into tokens
            string[] token = input.Split(' ');

            // Extract the command from the tokens
            string command = token[0];

            // Create a new empty string array for parameters
            string[] parameters = new string[0];

            // If parameters are more than one, then add them to the array by splitting the rest of the token on ','
            if (token.Length > 1) { parameters = token[1].Split(','); }

            // If the command is tocenter
            if (command.Equals("tocenter"))
            {
                // Call the center method in painter
                p.Center();
            }

            // If the command is fill
            else if (command.Equals("fill"))
            {
                // Check if paramter count is correct
                if (parameters.Length == 1)
                {   // Try to set the fill
                    try { p.SetFill(parameters[0]); }
                    catch (ArgumentException c)
                    {
                        // If exception caught then display error for wrong parameter
                        error += "[" + DateTime.Now.ToString("T") + "] " + c.Message;
                        error += (lineNum != 0) ? " at line " + lineNum : "";
                        error += ".\r\n";
                    }
                }
                else
                {   // Display error for invalid number of parameters
                    error += "[" + DateTime.Now.ToString("T") + "] " + "Incorrect number of parameters for this command";
                    error += (lineNum != 0) ? " at line " + lineNum : "";
                    error += ".\r\n";
                }
            }

            // If command is pen
            else if (command.Equals("pen"))
            {
                // Check if paramter count is correct
                if (parameters.Length == 1)
                {
                    // Try to set the color
                    try { p.SetColor(parameters[0]); }
                    catch (ArgumentException c)
                    {
                        // If exception caught then display error for wrong parameter
                        error += "[" + DateTime.Now.ToString("T") + "] " + c.Message;
                        error += (lineNum != 0) ? " at line " + lineNum : "";
                        error += ".\r\n";
                    }
                }
                else
                {
                    // Display error for invalid number of parameters
                    error += "[" + DateTime.Now.ToString("T") + "] " + "Incorrect number of parameters for this command";
                    error += (lineNum != 0) ? " at line " + lineNum : "";
                    error += ".\r\n";
                }
            }
            // If command is drawto, moveto, clear, reset
            else if (command.Equals("drawto") || command.Equals("moveto") || command.Equals("clear") || command.Equals("reset"))
            {
                // initialize parameters list
                int[] parametersInt = new int[0];

                // If there are parmaters
                if (parameters.Length > 0)
                {
                    // Set the length of the parameter list
                    parametersInt = new int[parameters.Length];
                    try
                    {
                        // Try to parse all parameter to integer and store in list
                        for (int i = 0; i < parameters.Length; ++i)
                            if (Variables.ContainsKey(parameters[i]))
                                parametersInt[i] = Int32.Parse(Variables[parameters[i]]);
                            else
                                parametersInt[i] = Int32.Parse(parameters[i]);

                    }
                    catch (FormatException)
                    {
                        // Catch Exception and display invalid parameters errpr
                        error += "[" + DateTime.Now.ToString("T") + "] " + "Parameters not valid for this command";
                        error += (lineNum != 0) ? " at line " + lineNum : "";
                        error += ".\r\n";
                    }

                }
                try
                {   // Try to execute the command
                    p.ExecuteCommand(command, parametersInt);
                }
                catch (IndexOutOfRangeException)
                {
                    // Catch Exception and display incorrect number of parameters error
                    error += "[" + DateTime.Now.ToString("T") + "] " + "Incorrect number of parameters for this command.";
                    error += (lineNum != 0) ? " at line " + lineNum : "";
                    error += ".\r\n";
                }
            }

            // if the command is circle, square, rect, triangle
            else if (command.Equals("circle") || command.Equals("square") || command.Equals("rect") || command.Equals("triangle"))
            {

                // Set the length of the parameter list
                int[] parametersInt = new int[parameters.Length];
                try
                {   // Try to parse all parameter to integer and store in list
                    for (int i = 0; i < parameters.Length; ++i)
                    {
                        if (Variables.ContainsKey(parameters[i]))
                            parametersInt[i] = Int32.Parse(Variables[parameters[i]]);
                        else
                            parametersInt[i] = Int32.Parse(parameters[i]);
                    }
                    p.DrawShape(command, parametersInt);
                }
                catch (FormatException)
                {
                    // Catch Exception and display invalid parameters errpr
                    error += "[" + DateTime.Now.ToString("T") + "] " + "Parameters not valid for this command";
                    error += (lineNum != 0) ? " at line " + lineNum : "";
                    error += ".\r\n";
                }
                catch (IndexOutOfRangeException)
                {
                    // Catch Exception and display incorrect number of parameters error
                    error += "[" + DateTime.Now.ToString("T") + "] " + "Incorrect number of parameters for this command.";
                    error += (lineNum != 0) ? " at line " + lineNum : "";
                    error += ".\r\n";
                }
            }
            else
            {
                // If the command is not recognized show error
                error += "[" + DateTime.Now.ToString("T") + "] " + "Command not recognized";
                error += (lineNum != 0) ? " at line " + lineNum : "";
                error += ".\r\n";
            }

        }

        private void parseUsingLexer(string input, int lineNum)
        {
            var tokens = lexer.Advance(input);
            string variable_name = "";
            //Console.WriteLine(tokens.Count);
            for (int i = 0; i < tokens.Count; i++)
            {

                var t = tokens[i];
                var numbersList = new List<int>();
                var operators = new Queue<string>();

                // Check for variable statement
                if (t.getType() == Type.IDENTIFIER && i == 0) { variable_name = t.getValue(); }

                // Check for variable in expression
                if (t.getType() == Type.OPERATOR && t.getValue() == "=" && tokens.Count > 3)
                {
                    foreach (var _token in tokens.GetRange(2, tokens.Count - 2))
                    {
                        if (_token.getType() == Type.IDENTIFIER)
                            numbersList.Add(int.Parse(Variables[_token.getValue()]));
                        if (_token.getType() == Type.NUMBER)
                            numbersList.Add(int.Parse(_token.getValue()));
                        if (_token.getType() == Type.OPERATOR)
                            operators.Enqueue(_token.getValue());
                    }
                    var result = numbersList[0];
                    for (int j = 1; j < numbersList.Count; j++)
                    {

                        switch (operators.Dequeue())
                        {
                            case "%":
                                result %= numbersList[j];
                                break;
                            case "+":
                                result += numbersList[j];
                                break;
                            case "-":
                                result -= numbersList[j];
                                break;
                            case "/":
                                result /= numbersList[j];
                                break;
                            case "*":
                                result *= numbersList[j];
                                break;
                        }
                    }

                    if (!Variables.ContainsKey(variable_name))
                    {
                        Variables.Add(variable_name, result.ToString());
                    }
                    else
                    {
                        Variables[variable_name] = result.ToString();
                    }

                    break;
                }

                // Assign and store number to var

                if (t.getType() == Type.IDENTIFIER && i > 0)
                {
                    if (!Variables.ContainsKey(variable_name))
                    {
                        Variables.Add(variable_name, Variables[t.getValue()]);
                    }
                    else
                    {
                        Variables[variable_name] = Variables[t.getValue()];
                    }
                }
                if (t.getType() == Type.NUMBER && i > 0)
                {
                    if (!Variables.ContainsKey(variable_name))
                    {
                        Variables.Add(variable_name, t.getValue());
                    }
                    else
                    {
                        Variables[variable_name] = t.getValue();
                    }
                }

            }

        }

        public bool parseUsingIf(string input)
        {
            var tokens = lexer.Advance(input);
            bool result = false;
            string op = "";
            var numbersList = new List<int>();
            foreach (var _token in tokens.GetRange(1, 3))
            {

                if (_token.getType() == Type.IDENTIFIER)
                    numbersList.Add(int.Parse(Variables[_token.getValue()]));
                if (_token.getType() == Type.NUMBER)
                    numbersList.Add(int.Parse(_token.getValue()));
                if (_token.getType() == Type.OPERATOR)
                    op = _token.getValue();
            }

            int left = numbersList[0];
            int right = numbersList[1];

            switch (op)
            {
                case "<":
                    result = left < right;
                    break;
                case ">":
                    result = left > right;
                    break;
                case "<=":
                    result = left <= right;
                    break;
                case ">=":
                    result = left >= right;
                    break;
                case "==":
                    result = left == right;
                    break;
                case "!=":
                    result = left != right;
                    break;
            }

            return result;
        }


        /// <summary>
        /// Parses the text form the code editor
        /// </summary>
        /// <param name="input">The text from the code editor</param>
        /// <remarks>Uses <see cref="parseCommand(string, int)"/></remarks>
        public void parseEditor(string input)
        {

            // Split the input on new line into lines
            string[] lines = input.Split('\n');
            string currentFunction = "";
            LinkedList<string> paramters = new LinkedList<string>();

            int cursor = 0;
            // For each line
            for (int lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                // If the line is not blank or null
                if (!String.IsNullOrWhiteSpace(lines[lineNum]))
                {
                    int ifLineNum;
                    if (lines[lineNum].Contains("=") || lines[lineNum].Contains("if") || lines[lineNum].Contains("endif") || lines[lineNum].Contains("while") || lines[lineNum].Contains("method") || Regex.Match(lines[lineNum], @"(\(.*\))").Success)
                    {

                        if (lines[lineNum].Contains("endmethod"))
                        {
                            if (!currentFunction.Equals(""))
                            {
                                var formalParam = Variables[currentFunction].Split(',')[2].Split('|');
                                foreach (var _parameter in formalParam)
                                {
                                    Variables.Remove(_parameter);
                                }
                            }
                            lineNum = cursor;
                        }

                        else if (lines[lineNum].Contains("method"))
                        {
                            var tokens = lexer.Advance(lines[lineNum]);
                            string formalParam = "";

                            foreach (var _token in tokens.GetRange(2, tokens.Count - 2))
                            {
                                if (_token.getType() == Type.IDENTIFIER)
                                {
                                    formalParam += _token.getValue() + "|";
                                }
                            }

                            int functionLineNum = lineNum;
                            for (; functionLineNum < lines.Length; functionLineNum++)
                            {
                                if (lines[functionLineNum].Contains("endmethod"))
                                {
                                    break;
                                }
                            }
                            if (!formalParam.Equals(""))
                                formalParam = formalParam.Remove(formalParam.Length - 1);

                            Variables.Add(tokens[1].getValue(), lineNum + "," + (functionLineNum - 1) + "," + formalParam);

                            lineNum = functionLineNum;
                        }

                        else if (Regex.Match(lines[lineNum], @"(\(.*\))").Success)
                        {
                            paramters.Clear();
                            cursor = lineNum;
                            var tokens = lexer.Advance(lines[lineNum]);
                            currentFunction = tokens[0].getValue();
                            var functionLines = Variables[tokens[0].getValue()].Split(',');
                            var formalParam = functionLines[2].Split('|');

                            foreach (var _token in tokens.GetRange(1, tokens.Count - 1))
                            {
                                if (_token.getType() == Type.NUMBER)
                                    paramters.AddLast(_token.getValue());
                                else if (_token.getType() == Type.IDENTIFIER)
                                    paramters.AddLast(Variables[_token.getValue()]);
                            }

                            if (formalParam.Length == paramters.Count)
                            {
                                for (int i = 0; i < formalParam.Length; i++)
                                {
                                    if (Variables.ContainsKey(formalParam[i]))
                                    {
                                        Variables[formalParam[i]] = paramters.ElementAt(i);
                                    }
                                    else
                                        Variables.Add(formalParam[i], paramters.ElementAt(i));
                                }
                            }

                            lineNum = Int32.Parse(functionLines[0]);
                        }

                        else if (lines[lineNum].Contains("while") && !lines[lineNum].Contains("endwhile"))
                        {
                            int whileNum = lineNum;
                            whileNum++;
                            while (parseUsingIf(lines[lineNum]))
                            {
                                if (lines[whileNum].Contains("endwhile"))
                                {
                                    whileNum = lineNum;
                                }
                                else
                                {
                                    if (lines[whileNum].Contains("endmethod"))
                                    {
                                        if (!currentFunction.Equals(""))
                                        {
                                            var formalParam = Variables[currentFunction].Split(',')[2].Split('|');
                                            foreach (var _parameter in formalParam)
                                            {
                                                Console.WriteLine(_parameter);
                                                Variables.Remove(_parameter);
                                            }
                                        }
                                        whileNum = cursor;
                                    }
                                    else if (Regex.Match(lines[whileNum], @"(\(.*\))").Success && !lines[whileNum].Contains("method"))
                                    {
                                        paramters.Clear();
                                        cursor = whileNum;
                                        var tokens = lexer.Advance(lines[whileNum]);
                                        currentFunction = tokens[0].getValue();
                                        var functionLines = Variables[tokens[0].getValue()].Split(',');
                                        var formalParam = functionLines[2].Split('|');

                                        foreach (var _token in tokens.GetRange(1, tokens.Count - 1))
                                        {
                                            if (_token.getType() == Type.NUMBER)
                                                paramters.AddLast(_token.getValue());
                                            else if (_token.getType() == Type.IDENTIFIER)
                                                paramters.AddLast(Variables[_token.getValue()]);
                                        }

                                        if (formalParam.Length == paramters.Count)
                                        {
                                            for (int i = 0; i < formalParam.Length; i++)
                                            {
                                                if (Variables.ContainsKey(formalParam[i]))
                                                {
                                                    Variables[formalParam[i]] = paramters.ElementAt(i);
                                                }
                                                else
                                                    Variables.Add(formalParam[i], paramters.ElementAt(i));
                                            }
                                        }

                                        whileNum = Int32.Parse(functionLines[0]);
                                    }
                                    else if (lines[whileNum].Contains("endif"))
                                    {
                                        continue;
                                    }

                                    else if (lines[whileNum].Contains("if"))
                                    {
                                        if (!parseUsingIf(lines[whileNum]))

                                        {
                                            bool flag = false;
                                            ifLineNum = whileNum;
                                            for (; ifLineNum < lines.Length; ifLineNum++)
                                            {
                                                if (lines[ifLineNum].Contains("endif"))
                                                {
                                                    flag = true;
                                                    break;
                                                }
                                            }
                                            whileNum = flag ? ifLineNum : whileNum + 1;
                                        }
                                    }

                                    else if (lines[whileNum].Contains("="))
                                    {
                                        parseUsingLexer(lines[whileNum], whileNum);
                                    }

                                    else
                                        parseCommand(lines[whileNum], whileNum);

                                }
                                whileNum++;
                            }
                            lineNum = whileNum;
                        }

                        else if (lines[lineNum].Contains("endif"))
                        {
                            continue;
                        }

                        else if (lines[lineNum].Contains("if"))
                        {
                            if (!parseUsingIf(lines[lineNum]))

                            {
                                bool flag = false;
                                ifLineNum = lineNum;
                                for (; ifLineNum < lines.Length; ifLineNum++)
                                {
                                    if (lines[ifLineNum].Contains("endif"))
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                lineNum = flag ? ifLineNum : lineNum + 1;
                            }
                        }

                        else if (lines[lineNum].Contains("="))
                        {
                            parseUsingLexer(lines[lineNum], lineNum);
                        }

                    }
                    else   // Call the parse command method passing the line , and the line num + 1
                        parseCommand(lines[lineNum], lineNum + 1);
                }
            }
        }


        /// <summary>
        /// Displays the errors encountered
        /// </summary>
        /// /// <remarks>Uses <see cref="Painter.WriteError(string)"/></remarks>
        public void displayError()
        {
            // IF there are errors
            if (error != "")
            {
                // Call the write error method
                p.WriteError(error); error = "";
            }
        }
    }
}