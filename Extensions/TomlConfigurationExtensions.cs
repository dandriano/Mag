using Microsoft.Extensions.Configuration;
using System.IO;
using Tomlyn.Model;

namespace Mag.Extensions
{
    public static class TomlConfigurationExtensions
    {
        public static IConfigurationBuilder AddTomlFile(this IConfigurationBuilder builder, string path, bool optional = false, bool reloadOnChange = false)
        {
            if (!File.Exists(path))
            {
                if (optional) return builder;
                throw new FileNotFoundException($"TOML file not found: {path}");
            }

            var tomlContent = File.ReadAllText(path);
            var tomlModel = Tomlyn.Toml.ToModel(tomlContent);
            var tomlConfigurationSource = new TomlConfigurationSource(tomlModel);

            return builder.Add(tomlConfigurationSource);
        }

        public class TomlConfigurationSource : IConfigurationSource
        {
            private readonly object _model;

            public TomlConfigurationSource(object model)
            {
                _model = model;
            }

            public IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                return new TomlConfigurationProvider(_model);
            }
        }

        public class TomlConfigurationProvider : ConfigurationProvider
        {
            private readonly object _model;

            public TomlConfigurationProvider(object model)
            {
                _model = model;
            }

            public override void Load()
            {
                // Convert the TOML model to key-value pairs
                if (_model is not TomlTable table)
                    return;

                LoadRecursive(table, string.Empty);
            }

            private void LoadRecursive(TomlTable table, string currentPrefix)
            {
                foreach (var kvp in table)
                {
                    // Create a new prefix for nested keys
                    var newPrefix = string.IsNullOrEmpty(currentPrefix) ? kvp.Key : $"{currentPrefix}:{kvp.Key}";
                    switch (kvp.Value)
                    {
                        case TomlTable nestedTable:
                            LoadRecursive(nestedTable, newPrefix);
                            break;
                        
                        case int value:
                            Data.Add(newPrefix, value.ToString());
                            break;
                        case long longValue when longValue >= int.MinValue && longValue <= int.MaxValue:
                            Data.Add(newPrefix, longValue.ToString()); 
                            break;
                        case string value:
                            Data.Add(newPrefix, value);
                            break;
                        default:
                            throw new System.Exception("Something went completely wrong");
                    }
                }
            }
        }
    }
}
