using System.Collections.Generic;
using System.Linq;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.SyncfusionControls
{
    public partial class BlazorGridInit : ExpansionBase
    {
        private string _table;
        private string _tablesLower;
        private List<ISchemaItem> _foreignKeys;

        private void MainFunction()
        {
            _table = GetTable(Input);
            _tablesLower = _table.Pluralize().ToLower();
            _foreignKeys = GetForeignKeysInTable(_table);

            const int indent = 4;
            AppendText();
            AppendText("public List<" + _table + "> " + _tablesLower + " { get; set; }", indent);
            AppendText(InitForeignKeyLists(), indent);
            AppendText("protected override async Task OnInitializedAsync()", indent);
            AppendText("{", indent);
            AppendText(_tablesLower + " = await client.GetFromJsonAsync<List<" + _table + ">>(" + ("api/" + _table.ToLower()).AddQuotes() + ");", indent * 2);
            AppendText(GetForeignKeyLists(), indent * 2);
            AppendText("}", indent);

        }

        private string InitForeignKeyLists()
        {
            var result = string.Empty;
            if (!_foreignKeys.Any()) return result;

            foreach (var table in _foreignKeys)
            {
                if (!string.IsNullOrEmpty(result))
                    result += "".AddCarriage();
                result += "public List<" + table.RelatedTable + "> Lookup" + table.RelatedTable + " { get; set; }";
            }
            return result;
        }

        private string GetForeignKeyLists()
        {
            var result = string.Empty;
            if (!_foreignKeys.Any()) return result;

            foreach (var table in _foreignKeys)
            {
                if (!string.IsNullOrEmpty(result))
                    result += "".AddCarriage();
                result += "Lookup" + table.RelatedTable + " = await client.GetFromJsonAsync<List<" + table.RelatedTable +
                          ">>(" + ("api/" + table.RelatedTable.ToLower()).AddQuotes() + ");";
            }
            return result;
        }

        public override void AppendText(string text, int indent) => base.AppendText(text, indent, string.Empty);
    }
}