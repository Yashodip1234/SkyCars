using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SkyCars.Core.DomainEntity.Grid
{
    public class GridRequestModel
    {
        public int Draw { get; set; }
        public List<GridColumn> Columns { get; set; }
       // public List<GridOrder> Order { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public ExportTypeEnum ExportType { get; set; }
        public List<GridOrder> Order { get; set; }
        public List<SearchGrid> SearchParams { get; set; }

        public GridRequestModel()
        {
            this.Columns = new List<GridColumn>();
            this.SearchParams = new List<SearchGrid>();
            this.Draw = 0;
            this.Start = 0;
            this.Length = 10;
        }
    }

    public class GridColumn
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public GridSearch Search { get; set; }
    }

    public class GridSearch
    {
        public string Value { get; set; }
        public bool Regex { get; set; }
    }

    public class GridOrder
    {
        public int Column { get; set; }
        public string Dir { get; set; }
    }

    public class SearchGrid
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string OpType { get; set; }
    }
    public enum ExportTypeEnum
    {
        Excel = 1, Pdf = 2
    }
}
