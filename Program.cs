﻿class Program
{
    static Dictionary<char, decimal> variables = [];

    static void Main(string[] args)
    {
        while (true)
        {
            string? input;
            if (args.Length > 0)
            {
                input = string.Join(" ", args);
                args = new string[0];
            }
            else
            {
                input = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            if (Command(input))
            {
                continue;
            }

            var allTokens = Tokenize(input);

            if (allTokens.Count == 0)
            {
                continue;
            }

            if (!ValidateParens(allTokens.ToArray()))
            {
                Console.WriteLine("Invalid parens.");
                continue;
            }

            List<List<Token>> allTokenLists = [[]];
            int listIndex = 0;
            for (int i = 0; i < allTokens.Count(); i++)
            {
                allTokenLists[listIndex].Add(allTokens[i]);

                if (i + 1 < allTokens.Count() && (allTokens[i].Type == TokenType.Letter || allTokens[i].Type == TokenType.Number)
                    && (allTokens[i + 1].Type == TokenType.Letter || allTokens[i + 1].Type == TokenType.Number))
                {
                    allTokenLists.Add([]);
                    listIndex++;
                }
            }
            allTokens = null;

            foreach (var tokens in allTokenLists)
            {
                char? letter = null;
                if (2 < tokens.Count() && tokens[0].Type == TokenType.Letter && tokens[1].Type == TokenType.Equal)
                {
                    letter = (char)tokens[0].Value;
                    tokens.RemoveRange(0, 2);
                }

                var output = Evaluate(tokens);
                if (!output.HasValue)
                {
                    continue;
                }

                if (letter.HasValue)
                {
                    variables[letter.Value] = output.Value;
                }

                Console.WriteLine(output.Value);
            }
        }
    }

    static bool Command(string input)
    {
        switch (input.Trim().ToLower())
        {
            case "exit":
                Environment.Exit(0);
                return true;
            case "clear":
                Console.Clear();
                return true;
            case "reset":
                variables.Clear();
                return true;
            case "vars":
                foreach (var varibale in variables)
                {
                    Console.WriteLine($"{varibale.Key} = {varibale.Value}");
                }
                return true;
            default:
                return false;
        }
    }

    static decimal? Evaluate(List<Token> tokens)
    {
        try
        {
            if (tokens.Count() == 1)
            {
                if (tokens[0].Type == TokenType.Number)
                {
                    return (decimal)tokens[0].Value;
                }
                else if (tokens[0].Type == TokenType.Letter)
                {
                    return variables[(char)tokens[0].Value];
                }
            }

            var operators = new Stack<Token>();
            var operands = new Stack<decimal>();

            for (int i = 0; i < tokens.Count(); i++)
            {
                var token = tokens[i];

                if (token.Type == TokenType.Number)
                {
                    operands.Push((decimal)token.Value);
                }
                else if (token.Type == TokenType.Letter)
                {
                    operands.Push(variables[(char)token.Value]);
                }
                else if (token.Type == TokenType.LeftParn)
                {
                    operators.Push(token);
                }
                else if (token.Type == TokenType.RightParn)
                {
                    while (operators.Peek().Type != TokenType.LeftParn)
                    {
                        operands.Push(ApplyOperator(operators.Pop(), operands.Pop(), operands.Pop()));
                    }

                    operators.Pop();
                }
                else
                {
                    while (operators.Count > 0 && Precedence(operators.Peek()) >= Precedence(token))
                    {
                        operands.Push(ApplyOperator(operators.Pop(), operands.Pop(), operands.Pop()));
                    }

                    operators.Push(token);
                }
            }

            while (operators.Count > 0)
            {
                operands.Push(ApplyOperator(operators.Pop(), operands.Pop(), operands.Pop()));
            }

            return operands.Pop();

        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to evaluate.");
            Console.WriteLine(e.Message);
            return null;
        }
    }

    static decimal ApplyOperator(Token token, decimal b, decimal a)
    {
        switch (token.Type)
        {
            case TokenType.Plus:
                return a + b;
            case TokenType.Minus:
                return a - b;
            case TokenType.Star:
                return a * b;
            case TokenType.Slash:
                return a / b;
            default:
                throw new Exception("Invalid operator.");
        }
    }

    static int Precedence(Token token)
    {
        switch (token.Type)
        {
            case TokenType.Plus:
            case TokenType.Minus:
                return 1;
            case TokenType.Star:
            case TokenType.Slash:
                return 2;
            default:
                return 0;
        }
    }

    static bool ValidateParens(Token[] tokens)
    {
        int count = 0;

        foreach (var token in tokens)
        {
            if (token.Type == TokenType.LeftParn)
            {
                count++;
            }
            else if (token.Type == TokenType.RightParn)
            {
                if (count == 0)
                {
                    return false;
                }

                count--;
            }
        }

        return count == 0;
    }

    static List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();

        for (int i = 0; i < input.Count(); i++)
        {
            var c = input[i];

            switch (c)
            {
                case ' ':
                    break;
                case '(':
                    tokens.Add(new Token(TokenType.LeftParn));
                    break;
                case ')':
                    tokens.Add(new Token(TokenType.RightParn));
                    break;
                case '+':
                    tokens.Add(new Token(TokenType.Plus));
                    break;
                case '-':
                    tokens.Add(new Token(TokenType.Minus));
                    break;
                case '*':
                    tokens.Add(new Token(TokenType.Star));
                    break;
                case '/':
                    tokens.Add(new Token(TokenType.Slash));
                    break;
                case '=':
                    tokens.Add(new Token(TokenType.Equal));
                    break;
                default:
                    if (char.IsDigit(c) || c == '.')
                    {
                        var number = string.Empty;
                        if (tokens.Count > 1 && tokens[^2].Type != TokenType.Number && tokens[^1].Type == TokenType.Minus)
                        {
                            tokens.RemoveAt(tokens.Count() - 1);
                            number = "-";
                        }
                        number += c.ToString();

                        while (i + 1 < input.Count() && (char.IsDigit(input[i + 1]) || input[i + 1] == '.'))
                        {
                            number += input[i + 1];
                            i++;
                        }

                        tokens.Add(new Token(TokenType.Number, decimal.Parse(number)));
                    }
                    else if (char.IsLetter(c))
                    {
                        tokens.Add(new Token(TokenType.Letter, c));
                    }
                    break;
            }
        }

        return tokens;
    }
}
