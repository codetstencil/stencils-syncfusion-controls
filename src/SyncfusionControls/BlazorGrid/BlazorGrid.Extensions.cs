using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.SyncfusionControls
{
    public static class BlazorGridExtensions
    {
        private static string _space = " ";

        public static string Indent(this string text, int indent = 4) =>
            string.Empty.PadLeft(indent) + text;

        public static string Field(this ISchemaItem schemaItem)
        {
            return _space +
                   General.SetValue("Field", "@nameof(" + schemaItem.TableName + "." + schemaItem.ColumnName + ")", false);
        }

        public static string Primary(this ISchemaItem schemaItem, bool show = false)
        {
            var visible = "false";
            if (show) visible = "true";

            if (schemaItem.IsPrimaryKey)
                return _space +
                       "IsPrimaryKey=" + "true".AddQuotes() +
                       " Visible=" + visible.AddQuotes() +
                       " AllowEditing=" + "false".AddQuotes();
            else
                return "";
        }

        public static string HeaderText(this ISchemaItem schemaItem) =>
            _space + "HeaderText=" + schemaItem.ColumnLabel.AddQuotes();
        public static string ForeignKeyValue(this ISchemaItem schemaItem, string label) =>
            _space + "ForeignKeyValue=" + label.AddQuotes();

        public static string ForeignDataSource(this ISchemaItem schemaItem, string label) =>
            _space + "ForeignDataSource=" + label.AddQuotes();


        public static string ValidationRule(this ISchemaItem schemaItem)
        {
            if (schemaItem.IsRequired)
                return _space + General.SetValue("ValidationRules", "@(new ValidationRules{ Required=true})");
            else
                return "";
        }

        public static string Width(this ISchemaItem schemaItem, int width = 120) =>
            _space + "Width=" + width.ToString().AddQuotes();

        public static string Alignment(this ISchemaItem schemaItem)
        {
            //return _space + "Width=" + width.ToString().AddQuotes();
            return "";
        }

        public static string ColumnSortField(this ISchemaItem schemaItem) =>
            _space + "Field=" + schemaItem.ColumnName.AddQuotes();

        public static string ColumnSortDirection(this ISchemaItem schemaItem) =>
            _space + "Direction=" + "SortDirection.Ascending".AddQuotes();

        public static string OpenTag(this string text, string tag) =>
            "<" + tag + text + ">";

        public static string CloseTag(this string text, string tag) =>
            "</" + tag + ">";

        public static string OpenCloseTag(this string text, string tag) =>
            "<" + tag + text + "></" + tag + ">";

        public static string OpenCloseTag(this string text, string tag, int indent) =>
            (text.OpenCloseTag(tag)).Indent(indent);

        public static string OpenCloseTagWithCr(this string text, string tag, int indent = 4) =>
            //var result = (GetOpenTag(tag, 0) + text + GetCloseTag(tag,0)).Indent(indent) ;
            //return result;

            ("<" + tag + ">").Indent(indent).AddCarriage() +
                   text.Indent().AddCarriage() +
                   ("</" + tag + ">").Indent(indent);

        private static string GetOpenTag(string tag, int indent) =>
            ("<" + tag + ">").Indent(indent).AddCarriage();

        private static string GetCloseTag(string tag, int indent) =>
            "".AddCarriage() + ("</" + tag + ">").Indent(indent);

        //public static string Tag(this string text, string tag)
        //{
        //    return "<"+tag + text +"</"+tag+">";
        //}
        //public static string Tag(this string text, string tag, int indent)
        //{
        //    return ("<" + tag + ">").AddCarriage().Indent(indent) + text.AddCarriage() + "</" + tag + ">";
        //}
        //public static string Tag(this string text, string tag, string tag2, int indent = 4)
        //{
        //    return ("<" + tag + " "+ tag2 + ">").AddCarriage().Indent(indent) + text.AddCarriage().Indent(indent) + "</" + tag + ">";
        //}

        //public static string TrimCarriage(this string str) => str.TrimEnd(Convert.ToChar(Environment.NewLine));
    }
}