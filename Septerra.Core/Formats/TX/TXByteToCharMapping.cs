using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Septerra.Core;

public class TXByteToCharMapping
{
    private readonly Dictionary<byte, char> _mapping;

    public TXByteToCharMapping()
        : this(new Dictionary<Byte, Char>())
    {
    }

    public TXByteToCharMapping(Dictionary<byte, char> mapping)
    {
        _mapping = mapping;
    }

    public IReadOnlyDictionary<byte, char> Mapping => _mapping;

    public void AddMapping(byte key, char value)
    {
        if (_mapping.ContainsKey(key))
        {
            throw new ArgumentException($"Key {key:X2} already exists.");
        }

        if (_mapping.ContainsValue(value))
        {
            throw new ArgumentException($"Value '{value}' already exists.");
        }

        _mapping[key] = value;
    }
    
    public void Apply(Dictionary<Byte, Char> encoding)
    {
        foreach (var pair in _mapping)
            encoding[pair.Key] = pair.Value;
    }

    public void SaveToFile(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var pair in _mapping)
            {
                writer.WriteLine($"0x{pair.Key:X2}: {pair.Value}");
            }
        }
    }

    public void LoadFromFile(string filePath)
    {
        _mapping.Clear();
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            var parts = line.Split(':');
            if (parts.Length == 2)
            {
                var keyString = parts[0].Trim();
                var valueString = parts[1].Trim();

                if (keyString.StartsWith("0x") && byte.TryParse(keyString.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out byte key))
                {
                    if (valueString.Length == 1)
                    {
                        char value = valueString[0];
                        _mapping[key] = value;
                    }
                }
            }
        }
    }
}