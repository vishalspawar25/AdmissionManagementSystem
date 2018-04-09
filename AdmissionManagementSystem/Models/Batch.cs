using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdmissionManagementSystem.Models
{
    public class Batch
    {
        public Batch()
        {
            this.LstStudent = new HashSet<Student>();
            this.LstTeacher = new HashSet<Teacher>();
        }
        [Key]
        public int BatchId { get; set; }
        [Required]
        public string Name { get; set; }
        
        public string Time { get; set; }

        [Required]
        [Display (Name ="Days")]
        public string csvDays { get; set; }

        [NotMapped]
        public ICollection<SelectListItem> WeekDaysList { get; set; }

        [Required]
        
        public DateTime StartDate { get; set; }
        [Required]
       
        public DateTime EndDate { get; set; }
        public ICollection<Student> LstStudent { get; set; }
        public ICollection<Teacher> LstTeacher { get; set; }

        [NotMapped]
        public List<SelectListItem> LstCourse { get; set; }
    
        [NotMapped]
        
        public int SelectedCourse { get; set; }

        [ForeignKey("Course")]
       
        [Display(Name = "Subject")]
        public int Course_Id { get; set; }
        public virtual Course Course { get; set; }

    }
}