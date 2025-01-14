﻿using System.IO;
using System.Reflection;
using PretiaArCloud;
using Utf8Json;
using Utf8Json.Resolvers;

namespace PretiaArCloudEditor
{
    public class Utf8JsonEditorSerializer : IJsonSerializer, IJsonFormatterResolver
    {
        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, this);
        }

        public T Deserialize<T>(Stream stream)
        {
            return JsonSerializer.Deserialize<T>(stream, this);
        }

        public string ToJsonString<T>(T data)
        {
            return JsonSerializer.ToJsonString(data, this);
        }

        public byte[] Serialize<T>(T data)
        {
            return JsonSerializer.Serialize<T>(data, this);
        }

        public IJsonFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        public T Deserialize<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }

        // configure your resolver and formatters.
        static IJsonFormatter[] formatters = new IJsonFormatter[]{
            new PoseFormatter(),
        };

        static readonly IJsonFormatterResolver[] resolvers = new[]
        {
            BuiltinResolver.Instance,
            Utf8Json.Unity.UnityResolver.Instance,
            DynamicGenericResolver.Instance,
            EnumResolver.Default,
            AttributeFormatterResolver.Instance,
            DynamicObjectResolver.AllowPrivateExcludeNull,
        };

        static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                foreach (var item in formatters)
                {
                    foreach (var implInterface in item.GetType().GetTypeInfo().ImplementedInterfaces)
                    {
                        var ti = implInterface.GetTypeInfo();
                        if (ti.IsGenericType && ti.GenericTypeArguments[0] == typeof(T))
                        {
                            formatter = (IJsonFormatter<T>)item;
                            return;
                        }
                    }
                }

                foreach (var item in resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        formatter = f;
                        return;
                    }
                }
            }
        }
    }
}