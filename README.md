# Palc
C# console based calculator.

# Documentation
Special operators:
- "!" : Prints the output. Must be the first character in the line.

      !-25.6 + 381.02 * 2
      736.44

- "=" : Assaigns a letter the output. Requires a letter before it.

      !a = 10 * (20 + 30)
      500

Operators:
- "+"
- "-"
- "*"
- "/"

Commands:
- Exit : Exits Palc.
- Clear : Clears the console.
- Reset : Clears all variables.
- Vars : Prints all variables.

# Installation
    git clone https://github.com/PiotrScripts/Palc.git

# Run
    dotnet build -c Release

    ./bin/Release/net8.0/Palc
Or

    dotnet run -c Release
