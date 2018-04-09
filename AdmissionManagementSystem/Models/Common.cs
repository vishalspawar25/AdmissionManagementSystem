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

    public class WeekKeyValue

    {
        public string Value { get; set; }
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

        public ICollection<SelectListItem> WeekDaysList()
        {

            ICollection<SelectListItem> lst = new List<SelectListItem>(new List<SelectListItem> {

                          new SelectListItem { Text = "Sun", Value = "Sun",Group=new SelectListGroup {Name="WeekDays" } },
                          new SelectListItem { Text = "Mon", Value = "Mon" ,Group=new SelectListGroup {Name="WeekDays" }},
                          new SelectListItem { Text = "Tue", Value = "Tue" ,Group=new SelectListGroup {Name="WeekDays" }},
                          new SelectListItem { Text = "Wed", Value = "Wed" ,Group=new SelectListGroup {Name="WeekDays" }},
                          new SelectListItem { Text = "Thu", Value = "Thu" ,Group=new SelectListGroup {Name="WeekDays" }},
                          new SelectListItem { Text = "Fri", Value = "Fri" ,Group=new SelectListGroup {Name="WeekDays" }},
                          new SelectListItem { Text = "Sat", Value = "Sat" ,Group=new SelectListGroup {Name="WeekDays" }}
            });




            return lst;


        }

    }

    public static class MyExtensionMethods
    {
        public static DateTime Tomorrow(this DateTime date)
        {
            return date.AddDays(1);
        }
    }
}