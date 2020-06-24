using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TRS.ViewModel;

namespace TRS.Models
{
    public class ViewModelList
    {
        public IEnumerable<ReportsViewModel> brandModels { get; set; }
        public IEnumerable<Point> items { get; set; }
    }
}