// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/blob/210a7a9feec64b7fac5147656cddb199ec90cf75/src/libraries/Microsoft.Extensions.Configuration.Json/src/JsonConfigurationFileParser.cs

using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace NovoNordisk.Configuration.AzureBlob;
    
internal sealed class JsonConfigurationFileParser
{
    private JsonConfigurationFileParser() { }

    private readonly Dictionary<string, string?> _data = new(StringComparer.OrdinalIgnoreCase);
    private readonly Stack<string> _paths = new();

    public static IDictionary<string, string?> Parse(Stream input)
        => new JsonConfigurationFileParser().ParseStream(input);

    private Dictionary<string, string?> ParseStream(Stream input)
    {
        var jsonDocumentOptions = new JsonDocumentOptions
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        using (var reader = new StreamReader(input))
        using (JsonDocument doc = JsonDocument.Parse(reader.ReadToEnd(), jsonDocumentOptions))
        {
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new FormatException($"Invalid top-level JSON element ({doc.RootElement.ValueKind}).)");
            }
            VisitObjectElement(doc.RootElement);
        }

        return _data;
    }

    private void VisitObjectElement(JsonElement element)
    {
        var isEmpty = true;

        foreach (JsonProperty property in element.EnumerateObject())
        {
            isEmpty = false;
            EnterContext(property.Name);
            VisitValue(property.Value);
            ExitContext();
        }

        SetNullIfElementIsEmpty(isEmpty);
    }

    private void VisitArrayElement(JsonElement element)
    {
        int index = 0;

        foreach (JsonElement arrayElement in element.EnumerateArray())
        {
            EnterContext(index.ToString());
            VisitValue(arrayElement);
            ExitContext();
            index++;
        }

        SetNullIfElementIsEmpty(isEmpty: index == 0);
    }

    private void SetNullIfElementIsEmpty(bool isEmpty)
    {
        if (isEmpty && _paths.Count > 0)
        {
            _data[_paths.Peek()] = null;
        }
    }

    private void VisitValue(JsonElement value)
    {
        Debug.Assert(_paths.Count > 0);

        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                VisitObjectElement(value);
                break;

            case JsonValueKind.Array:
                VisitArrayElement(value);
                break;

            case JsonValueKind.Number:
            case JsonValueKind.String:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                var key = _paths.Peek();
                if (_data.ContainsKey(key))
                {
                    throw new FormatException($"Duplicate key '{key}'.");
                }
                _data[key] = value.ToString();
                break;

            default:
                throw new FormatException($"Unsupported JSON token ({value.ValueKind})");
        }
    }

    private void EnterContext(string context) =>
        _paths.Push(_paths.Count > 0 ?
            _paths.Peek() + ConfigurationPath.KeyDelimiter + context :
            context);

    private void ExitContext() => _paths.Pop();
}