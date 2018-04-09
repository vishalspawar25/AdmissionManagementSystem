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
    public class Student:Person
    {
        public Student()
        {
            this.LstCourse = new HashSet<Course>();
            this.LstBatch = new HashSet<Batch>();
        }
        [Key]
        public int StudentId { get; set; }
        [DisplayName("Standard/Degree")]
        [Required]
        public string Standard { get; set; }      
        public string Stream { get; set; }
        public string Year { get; set; }
        public string College { get; set; }

        public ICollection<Course> LstCourse { get; set; }
        public ICollection<Batch> LstBatch { get; set; }
        [NotMapped]
        [DisplayName("Select Course")]
        public List<SelectListItem>  Courses { get; set; }
        [NotMapped]
     
        public List<int> SelectedCourses { get; set; }

        [NotMapped]
        public List<int> SelectedBatches { get; set; }

        [NotMapped]
        [DisplayName("Paying Amount")]
        [Required]
        public decimal PayingAmt { get; set; }


    }
}