using Microsoft.Win32;

using SimpleClassicTheme.Common.Logging;

using System;
using System.Reflection;

namespace SimpleClassicTheme.Taskbar.Helpers
{
    public static class RegistrySerializer
    {
        private const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;

        public static void SerializeToRegistry(RegistryKey key, object obj)
{
            foreach (var property in typeof(Config).GetProperties(bindingFlags))
            {
                if (!property.CanWrite)
                {
                    continue;
                }

                RegistryValueKind valueKind = RegistryValueKind.Unknown;
                object value = property.GetValue(obj);
                switch (value)
                {
                    case bool boolValue:
                        value = boolValue.ToString();
                        valueKind = RegistryValueKind.String;
                        break;

                    case int:
                    case Enum:
                        valueKind = RegistryValueKind.DWord;
                        break;

                    case string:
                        valueKind = RegistryValueKind.String;
                        break;

                    default:
                        if (value.GetType().IsClass)
                        {
                            using (var subKey = key.CreateSubKey(property.Name))
                            {
                                SerializeToRegistry(subKey, property.GetValue(obj));
                            }
                            continue;
                        }
                        else
                        {
                            Logger.Instance.Log(LoggerVerbosity.Basic, "RegistrySerializer", $"Ignoring property {property.Name} because {value?.GetType()} is an unknown type");
                        }

                        continue;
                }

                key.SetValue(property.Name, value, valueKind);
            }
        }

        public static void DeserializeFromRegistry(RegistryKey key, object obj)
        {
            foreach (var property in typeof(Config).GetProperties(bindingFlags))
            {
                if (!property.CanWrite)
                {
                    continue;
                }

                if (property.PropertyType.IsClass)
                {
                    using (var subKey = key.OpenSubKey(property.Name))
                    {
                        if (subKey != null)
                        {
                            DeserializeFromRegistry(subKey, property.GetValue(obj));
                        }
                    }
                    continue;
                }

                object value = property.GetValue(obj);
                var registryValue = key.GetValue(property.Name, value);

                // string → bool
                if (registryValue is string boolString && property.PropertyType == typeof(bool))
                {
                    registryValue = bool.Parse(boolString);
                }

                try
                {
                    property.SetValue(obj, registryValue);
                }
                catch
                {
                }
            }
        }
    }
}
