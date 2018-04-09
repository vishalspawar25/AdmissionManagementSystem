using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdmissionManagementSystem.Models.ViewModels
{
    public class ConfigureBatchViewModel
    {
        public int UserId { get; set; }
        public List<string> Courses { get; set; }
        public List<int> Selected { get; set; }
        public List<KeyValue> LstBatches { get; set; }
    }
}