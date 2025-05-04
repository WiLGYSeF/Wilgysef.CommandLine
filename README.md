# Wilgysef.CommandLine

A C# NuGet package for parsing command-line options.

# Configuring Options

## Class Configurations

```csharp
string[] args = ["--name", "John"];

var parser = new ArgumentParser();
parser.WithOptionsFrom<OptionConfigClassSimpleTest>();

var options = parser.ParseTo<OptionConfigClassSimpleTest>(args);

private class OptionConfigClassSimpleTest
{
    [Option(Description = "Name to use")]
    [LongName("name")]
    public string? Name { get; set; }
}
```

## Code Configurations

```csharp
string[] args = ["--name", "John"];

var parser = new ArgumentParser();

var inputOption = Option.LongOptionWithValue("Name", "name");
inputOption.Description = "Name to use";
parser.Options.Add(inputOption);

var options = parser.ParseTo<OptionCodeClassSimpleTest>(args);

private class OptionCodeClassSimpleTest
{
    public string? Name { get; set; }
}
```

or

```csharp
string[] args = ["--name", "John"];

var parser = new ArgumentParser();
parser.Options.Add(new Option("Name")
{
    LongNames = ["name"],
    Description = "Name to use",
    ValueCountRange = ValueRange.Exactly(1),
});

var options = parser.ParseTo<OptionCodeClassSimpleTest>(args);

private class OptionCodeClassSimpleTest
{
    public string? Name { get; set; }
}
```

# Configuring Values

## Class Configurations

```csharp
string[] args = ["--name", "John", "output.txt", "3", "extra", "args"];

var parser = new ArgumentParser();
parser.WithOptionsValuesFrom<ValueConfigClassSimpleTest>();

var options = parser.ParseTo<ValueConfigClassSimpleTest>(args);

private class ValueConfigClassSimpleTest
{
    [Option(Description = "Name to use")]
    [LongName("name")]
    public string? Name { get; set; }

    [Value(0, Description = "Output file", ValueName = "FILE")]
    public string? File { get; set; }

    [Value(1, Description = "Number", ValueName = "NUM")]
    public int? Number { get; set; }

    [Value(2, int.MaxValue, Description = "Extra values")]
    public string[]? Extra { get; set; }
}
```

## Code Configurations

```csharp
string[] args = ["--name", "John", "output.txt", "3", "extra", "args"];

var parser = new ArgumentParser();

parser.Options.Add(new Option("Name")
{
    LongNames = ["name"],
    Description = "Name to use",
    ValueCountRange = ValueRange.Exactly(1),
});

var file = Value.Single("File", 0);
file.Description = "Output file";
file.ValueName = "FILE";
parser.Values.Add(file);

var number = Value.Single("Number", 1);
number.Description = "Number";
number.ValueName = "NUM";
parser.Values.Add(number);

parser.Values.Add(new Value("Extra", 2, null)
{
    Description = "Extra values",
});

var options = parser.ParseTo<ValueConfigClassSimpleTest>(args);

private class ValueConfigClassSimpleTest
{
    [Option(Description = "Name to use")]
    [LongName("name")]
    public string? Name { get; set; }

    [Value(0, Description = "Output file", ValueName = "FILE")]
    public string? File { get; set; }

    [Value(1, Description = "Number", ValueName = "NUM")]
    public int? Number { get; set; }

    [Value(2, int.MaxValue, Description = "Extra values")]
    public string[]? Extra { get; set; }
}
```

# Configuring Commands

```csharp
```
