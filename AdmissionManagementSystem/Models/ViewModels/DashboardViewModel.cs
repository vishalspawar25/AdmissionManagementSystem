using System.Collections.Generic;
using System.Web.Mvc;

namespace AdmissionManagementSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<NameCount> NameCountList { get; set; }
        public List<SelectListItem> Courses { get; set; }
        public List<BatchBox> Batches { get; set; }
    }

    public class NameCount
    {
        public int Count { get; set; }
        public string Name { get; set; }
        public string NavigationUrl { get; set; }


    }

    public class BatchBox:NameCount
    {
        public int BatchId { get; set; }
    }
}