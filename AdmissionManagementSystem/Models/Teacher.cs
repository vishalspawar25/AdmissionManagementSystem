using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdmissionManagementSystem.Models
{
    public class Teacher : Person
    {
        public Teacher()
        {
            this.LstCourse = new HashSet<Course>();
        }
        [Key]
        public int Id { get; set; }
        public ICollection<Course> LstCourse { get; set; }

        [NotMapped]
        [DisplayName("Select Course")]
        public List<SelectListItem> Courses { get; set; }
        [NotMapped]
        public List<int> SelectedCourses { get; set; }
    }
}