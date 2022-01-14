using System;
using System.Globalization;
using UnityEngine;
using Valve.Newtonsoft.Json;

/// <summary>
/// Custom Newtonsoft.Json converter <see cref="JsonConverter"/> for the Unity Vector3 type <see cref="Vector3"/>.
/// </summary>
public class Vector3Converter : JsonConverter
{
    /// <summary>
    /// Read the specified properties to the object.
    /// </summary>
    /// <returns>The object value.</returns>
    /// <param name="reader">The <c>Newtonsoft.Json.JsonReader</c> to read from.</param>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="existingValue">The existing value of object being read.</param>
    /// <param name="serializer">The calling serializer.</param>
    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            bool isNullableStruct = objectType.IsGenericType
                && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);

            return isNullableStruct ? null : (object)default(Vector3);
        }

        return InternalReadJson(reader, serializer, existingValue);
    }

    private Vector3 InternalReadJson(JsonReader reader, JsonSerializer serializer, object existingValue)
    {
        if (reader.TokenType != JsonToken.StartObject)
        {
            //throw reader.CreateSerializationException($"Failed to read type '{typeof(T).Name}'. Expected object start, got '{reader.TokenType}' <{reader.Value}>");
        }

        reader.Read();

        Vector3 value = (Vector3)existingValue;

        string previousName = null;

        while (reader.TokenType == JsonToken.PropertyName)
        {
            if (reader.Value.GetType() == typeof(string))
            {
                string name = reader.Value as string;
                if (name == previousName)
                {
                    //throw reader.CreateSerializationException($"Failed to read type '{typeof(T).Name}'. Possible loop when reading property '{name}'");
                }

                previousName = name;
                ReadValue(ref value, name, reader, serializer);
            }
            else
            {
                reader.Skip();
            }

            reader.Read();
        }

        return value;
    }

    protected void ReadValue(ref Vector3 value, string name, JsonReader reader, JsonSerializer serializer)
    {
        switch (name)
        {
            case "x":
                value.x = reader.ReadAsFloat() ?? 0f;
                break;
            case "y":
                value.y = reader.ReadAsFloat() ?? 0f;
                break;
            case "z":
                value.z = reader.ReadAsFloat() ?? 0f;
                break;
        }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Determine if the object type is <typeparamref name="T"/>
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns><c>true</c> if this can convert the specified type; otherwise, <c>false</c>.</returns>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector3)
            || (objectType.IsGenericType
                && objectType.GetGenericTypeDefinition() == typeof(Nullable<>)
                && objectType.GetGenericArguments()[0] == typeof(Vector3));
    }
}

public static class JsonExtensions
{
    public static float? ReadAsFloat(this JsonReader reader)
    {
        // https://github.com/jilleJr/Newtonsoft.Json-for-Unity.Converters/issues/46

        var str = reader.ReadAsString();
        float valueParsed;

        if (string.IsNullOrEmpty(str))
        {
            return null;
        }
        else if (float.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out valueParsed))
        {
            return valueParsed;
        }
        else
        {
            return 0f;
        }
    }
}