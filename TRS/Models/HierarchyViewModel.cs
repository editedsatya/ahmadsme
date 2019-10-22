using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TRS.Models
{
    public class HierarchyViewModel
    {
        public int Id { get; set; }
        public string text { get; set; }
        public int? perentId { get; set; }
        public string imageUrl { get; set; }
        public string imageHtml { get; set; }

        public bool @checked { get; set; }

        public virtual List<HierarchyViewModel> children { get; set; }
    }

    public class IData
    {
        public string code { get; set; }
        public string group { get; set; }
        public string name { get; set; }

        public IDataAttributes attributes { get; set; }

        public bool? nodeExpanded { get; set; }
        public bool? nodeSelected { get; set; }

        public bool? nodeDisabled { get; set; }
        public virtual List<IData> children { get; set; }
    }
    public class IDataAttributes
    {
        public string key { get; set; }

    }

    


}