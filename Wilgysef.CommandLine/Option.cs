using Wilgysef.CommandLine.Exceptions;

namespace Wilgysef.CommandLine;

public class Option
{
    public Option(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    public IList<char>? ShortNames { get; set; }

    public IList<string>? LongNames { get; set; }

    public string? ShortNamePrefix { get; set; }

    public string? LongNamePrefix { get; set; }

    public string? KeyValueSeparator { get; set; }

    public bool? ShortNameImmediateValue { get; set; }

    public ValueRange? ValueCountRange { get; set; }

    public bool LongNameCaseInsensitive { get; set; }

    public bool HasShortNames => ShortNames != null && ShortNames.Count > 0;

    public bool HasLongNames => LongNames != null && LongNames.Count > 0;

    public bool CanHaveValues => ValueCountRange != null
        && (!ValueCountRange.Max.HasValue || ValueCountRange.Max > 0);

    public static Option ShortOption(string name, char opt)
    {
        return new Option(name)
        {
            ShortNames = [opt],
        };
    }

    public static Option ShortOptionWithValue(string name, char opt)
    {
        return new Option(name)
        {
            ShortNames = [opt],
            ValueCountRange = new ValueRange(1, 1),
        };
    }

    public static Option LongOption(string name, string opt)
    {
        return new Option(name)
        {
            LongNames = [opt],
        };
    }

    public static Option LongOptionWithValue(string name, string opt)
    {
        return new Option(name)
        {
            LongNames = [opt],
            ValueCountRange = new ValueRange(1, 1),
        };
    }

    public static Option ShortAndLongOption(string name, char shortOpt, string longOpt)
    {
        return new Option(name)
        {
            ShortNames = [shortOpt],
            LongNames = [longOpt],
        };
    }

    public bool MatchesShortName(char ch)
    {
        return ShortNames?.Contains(ch) ?? false;
    }

    public void Validate()
    {
        ThrowIf(!HasShortNames && !HasLongNames, "Option does not have long or short names set");
        ThrowIf(HasLongNames && LongNames!.Any(n => n.Length == 0), "Long names must have a length greater than 0");

        ThrowIf(ShortNamePrefix != null && ShortNamePrefix.Length == 0, "Short name prefix cannot be empty");
        ThrowIf(LongNamePrefix != null && LongNamePrefix.Length == 0, "Long name prefix cannot be empty");

        ThrowIf(KeyValueSeparator != null && KeyValueSeparator.Length == 0, "Key-value separator cannot be empty");

        ValueCountRange?.Validate(Name);

        void ThrowIf(bool value, string message)
        {
            if (value)
            {
                throw new InvalidOptionException(Name, message);
            }
        }
    }

    public class ValueRange
    {
        public ValueRange(int value)
        {
            Min = value;
            Max = value;
        }

        public ValueRange(int min, int? max)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; set; }

        public int? Max { get; set; }

        public bool InRange(int num)
        {
            if (num < Min)
            {
                return false;
            }

            return UnderMax(num);
        }

        public bool UnderMax(int num)
        {
            return !Max.HasValue || num <= Max;
        }

        internal void Validate(string optionName)
        {
            if (Max.HasValue && Min > Max)
            {
                throw new InvalidOptionException(optionName, "Minimum cannot be greater than maximum");
            }
        }
    }
}
