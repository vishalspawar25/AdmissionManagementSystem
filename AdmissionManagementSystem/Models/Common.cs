using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdmissionManagementSystem.Models
{
    public class KeyValue

    {
        public int Value { get; set; }
        public string Text { get; set; }

    }

    public class Common
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public List<SelectListItem> GetCourses()
        {
            List<SelectListItem> LstCourses = new List<SelectListItem>();

            IEnumerable<KeyValue> Courses = new List<KeyValue>();

            Courses = db.Courses.Select(s => new KeyValue { Value = s.Id, Text = s.Name }).ToList();

            foreach (var item in Courses)
            {
                LstCourses.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value.ToString()
                });
            }
            return LstCourses;
        }

        
      
    }
}