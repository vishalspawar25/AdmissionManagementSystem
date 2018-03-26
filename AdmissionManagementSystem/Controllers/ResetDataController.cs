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
                ////Clear student courses
                foreach (var student in students)
                {


                    var item = db.Entry<Student>(student);
                    item.State = EntityState.Modified;
                    item.Collection(i => i.LstCourse).Load();
                    student.LstCourse.Clear();

                }
                db.SaveChanges();
                //clear payment details
                foreach (var payment in payments)
                {
                    db.PaymentDetails.Remove(payment);


                }
                db.SaveChanges();
                //Clear students

                foreach (var student in students)
                {

                    db.Students.Remove(student);

                }
                db.SaveChanges();

                foreach (var student in students)
                {

                    string path = Server.MapPath("@~/UploadedFiles/ProfilePics/" + student.ProfilePic);
                    FileInfo file = new FileInfo(path);
                    if (file.Exists)//check file exsit or not
                    {
                        file.Delete();
                    }
                }
                //Clear Courses
                foreach (var course in courses)
                {

                    db.Courses.Remove(course);

                }
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Students', RESEED, 0)");
                db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('PaymentDetails', RESEED, 0)");
                db.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Courses', RESEED, 0)");
                db.SaveChanges();
                ViewBag.message = "Data reset successfully.";
            }
            catch (Exception)
            {

                ViewBag.message = "Error occurred in Data Reset.";
            }
            return View("Index");
        }
    }
}