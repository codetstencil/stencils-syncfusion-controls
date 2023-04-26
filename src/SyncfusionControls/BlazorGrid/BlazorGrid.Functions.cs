using System.Collections.Generic;
using System.Linq;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.SyncfusionControls
{
    public partial class BlazorGrid : ExpansionBase
    {
        private string _table;
        private string _tablesLower;

        private List<ISchemaItem> _columns;
        private List<ISchemaItem> _sortColumns;
        private List<ISchemaItem> _foreignKeys;


        /// <summary>
        /// Dictionary containing configuration values
        /// </summary>
        //private Dictionary<string, string> _gridConfiguration;
        private readonly string _true = "true".AddQuotes() + " ";

        private void MainFunction()
        {
            _table = GetTable(Input);
            _tablesLower = _table.Pluralize().ToLower();

            _columns = GetColumns(_table, false);
            _sortColumns = GetSortColumns(_table);
            _foreignKeys = GetForeignKeysInTable(_table);


            #region Razor tags

            const int indent = 4;
            AppendText();
            AppendText(("<SfGrid" + DataSourceSettings() + ">"), indent*3);

            if (_sortColumns.Any() && ConfigValue("SF_ALLOW_SORTING"))
                AppendText(General.Wrap2TagLevels(GridSortColumn(indent * 3), "GridSortSettings", "GridSortColumns", indent + 4));

            AppendText(GridEditSettings(indent), indent*3);
            AppendText(GridEvent(indent), indent*3);
            AppendText(General.Wrap1TagLevel(GridColumn(indent * 5), "GridColumns", indent*4), indent);
            AppendText("</SfGrid>", indent*3);

            #endregion Razor tags
        }

        private string ContextMenuString()
        {
            var result = 
                "AllowExcelExport = " + _true +
                 "AllowPdfExport= " + _true +
                 "ContextMenuItems=" +
                 (
                     "@(new List<object>() { " +
                     "AutoFit".AddQuotes() + "," +
                     "AutoFitAll".AddQuotes() + "," +
                     "SortAscending".AddQuotes() + "," +
                     "SortDescending".AddQuotes() + "," +
                     "Copy".AddQuotes() + "," +
                     "Edit".AddQuotes() + "," +
                     "Delete".AddQuotes() + "," +
                     "Save".AddQuotes() + "," +
                     "Cancel".AddQuotes() + "," +
                     "PdfExport".AddQuotes() + "," +
                     "ExcelExport".AddQuotes() + "," +
                     "CsvExport".AddQuotes() + "," +
                     "FirstPage".AddQuotes() + "," +
                     "PrevPage".AddQuotes() + "," +
                     "LastPage".AddQuotes() + "," +
                     "NextPage".AddQuotes() + "})"
                 ).AddQuotes();
            return result;
        }

        public override void AppendText(string text, int indent) => base.AppendText(text, indent, string.Empty);


        private string GridColumn(int indent)
        {
            BuildSnippet(null);
            foreach (var item in _columns)
            {
                if (!item.IsChecked && !item.IsPrimaryKey) continue;

                if (item.IsForeignKey)
                {
                    var properties = 
                        item.Field() + item.Primary()+ item.HeaderText() + 
                        item.ForeignKeyValue(GetTableLabel(item.RelatedTable))+ 
                        item.ForeignDataSource("@Lookup"+item.RelatedTable) + 
                        //item.ValidationRule() + 
                        item.Width(150) + item.Alignment();
                    var row = General.CreateRow("GridForeignColumn", properties);
                    BuildSnippet(row.AddCarriage(), indent, true);
                }
                else
                {
                    var properties =
                        item.Field() + item.Primary() + item.HeaderText() + item.ValidationRule() + item.Width(150) +
                        item.Alignment();
                    var row = General.CreateRow("GridColumn", properties);
                    BuildSnippet(row.AddCarriage(), indent, true);
                }
            }
            return General.GenerateRow(BuildSnippet(), indent, "GridColumns");
        }

        private string GridSortColumn(int indent)
        {
            BuildSnippet(null);
            foreach (var item in _sortColumns)
            {
                var properties = item.ColumnSortField() + item.ColumnSortDirection();
                var row = General.CreateRow("GridSortColumn", properties);
                BuildSnippet(row.AddCarriage(), indent, true);
            }
            return General.GenerateRow(BuildSnippet(), indent, "GridSortColumns");
        }

        private string GridEditSettings(int indent, bool setEditSettings = true)
        {
            var result = string.Empty;
            if (setEditSettings)
            {
                BuildSnippet(null);
                result = General.SetValue("AllowAdding") +
                         General.SetValue("AllowDeleting") +
                         General.SetValue("AllowEditing");
                    result+=GetMoreSettings(); 
                result = General.CreateRow("GridEditSettings", result, indent);
            }

            string GetMoreSettings()
            {
                var result2 = string.Empty;
                //Confirm Delete Action
                if (ConfigValue("SF_SHOW_DELETE_CONFIRM"))
                    result2 += General.SetValue("ShowDeleteConfirmDialog");
                    result2 += General.SetValue("Mode",GetExpansionString("SF_EDIT_MODE"));
                    return result2;
            }

            return result;
        }
        string GridEvent(int indent)
        {
            BuildSnippet(null);
            var events =
                General.SetValue("OnActionBegin", "ActionBegin") + 
                General.SetValue("OnActionComplete", "OnActionComplete") +
                General.SetValue("TValue", _table);
            events = General.CreateRow("GridEvents", events, indent);
            return events;
        }


        private string SfDataManager(int indent, bool setEditSettings = true)
        {
            BuildSnippet(null);
            var result = General.SetValue("Url", "/api/" + _table) + General.SetValue("Adaptor", "Adaptors.WebApiAdaptor");
            result = General.CreateRow("SfDataManager ", result, indent);
            return result;
        }

        private string DataSourceSettings(bool setSettings = true)
        {
            var toolbar = "@(new List<string> {" +
                          "Add".AddQuotes() + "," +
                          "Edit".AddQuotes() + "," +
                          "Delete".AddQuotes() + "," +
                          "Update".AddQuotes() + "," +
                          "Cancel".AddQuotes() + "," +
                          "Print".AddQuotes() + " })";
            //var result  = General.SetValue("@ref", "Gridview");
            var result = General.SetValue("DataSource", "@"+_tablesLower);
                result += General.SetValue("Toolbar", toolbar);
            if (setSettings)
            {
                BuildSnippet(null);

                //Use Context Menu
                if (ConfigValue("SF_USE_CONTEXT_MENU"))
                    result += ContextMenuString();

                //Use GridLines
                if (ConfigValue("SF_USE_GRID_LINES"))
                    result += General.SetValue("GridLines", "GridLine.Both");

                //Use Grouping
                if (ConfigValue("SF_ALLOW_GROUPING"))
                    result += General.SetValue("AllowGrouping");

                //Use EnablePersistence
                if (ConfigValue("SF_ENABLE_PERSISTENCE"))
                    result += General.SetValue("EnablePersistence");

                result = (result + General.SetValue("AllowPaging"));
            }
            return result;
        }

        public  bool ConfigValue(string setting)
        {
            var result = false;
            var value = GetExpansionString(setting);
            if (!value.IsBlank())
            {
                value = value.ToLower();
                if (value == "yes" || value == "true" || value == "1")
                    result = true;
            }
            return result;
        }

        public new string GetExpansionString(string label)
        {
            var text = Expander.Where(e => e.ExpansionLabel == label).Select(x => x.ExpansionString).FirstOrDefault();
            return text;
        }


    }
}