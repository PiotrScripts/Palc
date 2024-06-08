class Program
{
    static Dictionary<char, decimal> variables = [];

    static void Main(string[] args)
    {
        while (true)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            var tokens = Tokenize(input);

            if (tokens.Count == 0)
            {
                Console.WriteLine("Invalid input");
                continue;
            }

            if (!ValidateParens(tokens.ToArray()))
            {
                Console.WriteLine("Invalid parens");
                continue;
            }

            bool print = false;
            if (tokens[0].Type == TokenType.Bang)
            {
                print = true;
                tokens.RemoveAt(0);
            }

            if (tokens.Count == 0)
            {
                Console.WriteLine("Invalid input");
                continue;
            }

            if (2 < tokens.Count() && tokens[0].Type == TokenType.Letter && tokens[1].Type == TokenType.Equal)
            {
                char letter = (char)tokens[0].Value;
                tokens.RemoveRange(0, 2);
                SetVariable(letter, Evaluate(tokens));
                if (print)
                {
                    Console.WriteLine(variables[letter]);
                }
            }
            else
            {
                Console.WriteLine(Evaluate(tokens));
            }
        }
    }

    static void SetVariable(char name, decimal value)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
        }
        else
        {
            variables.Add(name, value);
        }
    }

    static decimal Evaluate(List<Token> tokens)
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
                throw new Exception("Invalid operator");
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
                case '!':
                    tokens.Add(new Token(TokenType.Bang));
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

enum TokenType
{
    LeftParn, RightParn,
    Plus, Minus,
    Star, Slash,
    Number,
    Letter,
    Equal,
    Bang
}

class Token
{
    public TokenType Type { get; set; }
    public object? Value { get; set; }

    public Token(TokenType type, object? value = null)
    {
        Type = type;
        Value = value;
    }
}
