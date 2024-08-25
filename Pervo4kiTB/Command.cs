class Command
{
    public Command(params ValueType[] valueTypes)
    {
        ValueTypes = valueTypes;
    }

    public readonly ValueType[] ValueTypes;

    public bool IsValid(ParsedText text)
    {
        if (text.Values.Count != ValueTypes.Length && !ValueTypes.Contains(ValueType.ParamsLong) && !ValueTypes.Contains(ValueType.ParamsString))
            return false;

        for (var i = 0; i < text.Values.Count; i++)
        {
            var part = text.Values[i];
            var isValidPath = ValueTypes[i < ValueTypes.Length ? i : ValueTypes.Length - 1] switch
            {
                ValueType.String or ValueType.ParamsString => part.IsString,
                ValueType.Long or ValueType.ParamsLong => part.IsLong,
                _ => throw new ArgumentException()
            };

            if (!isValidPath)
                return false;
        }

        return true;
    }

    public object[] Parse(ParsedText text)
    {
        var result = new object[text.Values.Count];
        for (var i = 0; i < text.Values.Count; i++)
            result[i] = ValueTypes[i < ValueTypes.Length ? i : ValueTypes.Length - 1] switch
            {
                ValueType.String or ValueType.ParamsString => text.Values[i].String,                
                ValueType.Long or ValueType.ParamsLong => text.Values[i].Long,
                _ => throw new ArgumentException()
            };

        return result;
    }
}

public record Value(string Raw)
{
    public readonly bool IsString = true;
    public readonly bool IsLong = long.TryParse(Raw, out var longValue);

    public string String => Raw;
    public long Long => IsLong ? long.Parse(Raw) : default;
}

public enum ValueType
{
    String,
    Long,
    ParamsLong,
    ParamsString
}

class ParsedText
{
    ParsedText(List<Value> values) => Values = values;

    public readonly List<Value> Values;

    public static ParsedText Parse(string text)
    {
        if (text is null)
            return Parse("");

        List<Value> values = [];

        var level = 0;
        var nextIgnoreSpecial = false;
        var value = "";
        foreach (var c in text)
        {
            if (nextIgnoreSpecial)
            {
                value += c;
                nextIgnoreSpecial = false;
                continue;
            }

            if (c == '\\')
            {
                nextIgnoreSpecial = true;
                continue;
            }

            if (c == ' ' && level == 0)
            {
                values.Add(new Value(value));
                value = "";
            }
            else if (c == '\"')
            {
                if (level == 0)
                {
                    level++;
                    continue;
                }

                level++;
                value += c;
            }
            else value += c;
        }

        if (value.Length > 0)
            values.Add(new Value(value));

        return new(values);
    }
}