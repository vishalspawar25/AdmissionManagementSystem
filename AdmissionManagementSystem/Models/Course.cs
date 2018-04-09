using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AdmissionManagementSystem.Models
{
    public class Course
    {
        public Course()
        {
            this.LstStudent = new HashSet<Student>();
            this.LstTeacher = new HashSet<Teacher>();
            
        }


        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1,48)]
        public int Duration { get; set; }
        [Required]
        public decimal Fee { get; set; }

        public ICollection<Student> LstStudent { get; set; }
        public ICollection<Teacher> LstTeacher { get; set; }
       
    }
}