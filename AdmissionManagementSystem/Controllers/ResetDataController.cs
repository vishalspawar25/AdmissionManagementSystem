using AdmissionManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdmissionManagementSystem.Controllers
{

    public class ResetDataController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: ResetData
        public ActionResult Index()
        {
            return View();

        }

        [ValidateAntiForgeryToken]
        public ActionResult Reset()
        {

            try
            {


                var students = db.Students.ToList();
                var payments = db.PaymentDetails.ToList();
                var courses = db.Courses.ToList();
                var teachers = db.Teachers.ToList();
                var batches = db.Batches.ToList();

               //1.Clear student courses
                foreach (var student in students)
                {


                    var item = db.Entry<Student>(student);
                    item.State = EntityState.Modified;
                    item.Collection(i => i.LstCourse).Load();
                    item.Collection(i => i.LstBatch).Load();
                    student.LstCourse.Clear();
                    student.LstBatch.Clear();

                }
                db.SaveChanges();

                foreach (var batch in batches)
                {
                    db.Batches.Remove(batch);
                }
                db.SaveChanges();
                //2.Clear payment details
                foreach (var payment in payments)
                {
                    db.PaymentDetails.Remove(payment);

                }
                db.SaveChanges();

                //3.Clear students
                foreach (var student in students)
                {
                    db.Students.Remove(student);

                }
                db.SaveChanges();

                //clear profile pic of all students
                foreach (var student in students)
                {

                    string path = Server.MapPath("@~/UploadedFiles/ProfilePics/" + student.ProfilePic);
                    FileInfo file = new FileInfo(path);
                    if (file.Exists)//check file exsit or not
                    {
                        file.Delete();
                    }
                }

                //4.Clear Courses
                foreach (var course in courses)
                {
                    db.Courses.Remove(course);
                }
                db.SaveChanges();

                //4.Clear Teachers
                foreach (var teacher in teachers)
                {
                    db.Teachers.Remove(teacher);
                }
                db.SaveChanges();


                db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Students', RESEED, 0)");
                db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('PaymentDetails', RESEED, 0)");
                db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Courses', RESEED, 0)");
                db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Teachers', RESEED, 0)");
                db.SaveChanges();
                ViewBag.message = "Data reset successfully.";
            }
            catch (Exception ex)
            {

                ViewBag.message = "Error occurred in Data Reset.";
            }
            return View("Index");
        }
    }
}