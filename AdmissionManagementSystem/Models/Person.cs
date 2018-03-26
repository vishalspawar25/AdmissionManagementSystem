using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdmissionManagementSystem.Models
{
    public class Person
    {
        [DisplayName("First Name")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        
        public string Phone { get; set; }

        [Required]
        public DateTime DoB { get; set; }

        [DisplayName("Roll Number")]
        public string RollNo { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }
        public string ProfilePic { get; set; }


    }
}
