using System.Collections.Generic;
using ZeraSystems.CodeNanite.Expansion;

namespace ZeraSystems.SyncfusionControls
{
    public class General : ExpansionBase
    {
        public static Dictionary<string, string> GridConfiguration;

        public static string CreateRow(string tag, string properties, string textToInsert = null)
        {
            var result = string.Empty;
            if (!properties.IsBlank())
            {
                result = properties.OpenTag(tag);
                if (textToInsert != null)
                {
                    result += "".AddCarriage();
                    result += textToInsert.AddCarriage();
                }
                result += properties.CloseTag(tag);
            }

            return result;
        }

        public static string CreateRow(string tag, string properties, int indent) =>
            BlazorGridExtensions.Indent(CreateRow(tag, properties), indent);

        public static string GenerateRow(string text, int indent, string tagParent) =>
            text.TrimEnd('\r', '\n');

        /// <summary>
        /// Creates a 2-Level Html tag on the passed text
        /// </summary>
        /// <param name="text">Passed Text</param>
        /// <param name="tag1">Highest level of the tags</param>
        /// <param name="tag2">2nd/next level of the tags</param>
        /// <param name="indent"></param>
        /// <returns>Html tagged content</returns>
        public static string Wrap2TagLevels(string text, string tag1, string tag2, int indent)
        {
            var sortTag = Wrap1TagLevel(text, tag2, indent);
            return Wrap1TagLevel(sortTag, tag1, indent - 4);
        }
        /// <summary>
        /// Creates a 1-Level Html tag on passed text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <param name="indent"></param>
        /// <returns>Html tagged content</returns>
        public static string Wrap1TagLevel(string text, string tag, int indent)
        {
            var result =
                BlazorGridExtensions.Indent(("<" + tag + ">"), indent).AddCarriage() +
                text.AddCarriage() +
                BlazorGridExtensions.Indent(("</" + tag + ">"), indent);
            return result;
        }

        public static string SetValue(string property, string value = "true", bool useQuotes = true)
        {
            if (useQuotes)
                return " " + property.Trim() + "=" + value.AddQuotes();
            else
                return " " + property.Trim() + "=" + value;
        }

        //public static string GetConfigValue(string setting)
        //{
        //    return General.GridConfiguration.ContainsKey(setting) ? General.GridConfiguration[setting] : null;
        //}

        public bool ConfigValue(string setting)
        {
            var result = false;
            //var value = GetConfigValue(setting);
            var value = GetExpansionString(setting);
            if (!value.IsBlank())
            {
                value = value.ToLower();
                if (value == "yes" || value == "true" || value == "1")
                    result = true;
            }
            return result;
        }
        //public string GetExpansionString(string label)
        //{
        //    var text = Expander.Where(e => e.ExpansionLabel == label).Select(x => x.ExpansionString).FirstOrDefault();
        //    return text;
        //}
    }
}
