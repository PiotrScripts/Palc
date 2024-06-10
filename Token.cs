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
