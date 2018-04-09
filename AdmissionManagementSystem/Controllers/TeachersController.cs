using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdmissionManagementSystem.Models;
using System.IO;
using Microsoft.Security.Application;

namespace AdmissionManagementSystem.Controllers
{
    public class TeachersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Common common = new Common();

        // GET: Teachers
        public ActionResult Index()
        {
            return View(db.Teachers.ToList());
        }

        // GET: Teachers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // GET: Teachers/Create
        public ActionResult Create()
        {
            common = new Common();

            Teacher teacher = new Teacher();
            teacher.Courses = common.GetCourses();
            return View(teacher);
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Email,Phone,DoB,RollNo,Address,City,SelectedCourses,Courses")] Teacher teacher, HttpPostedFileBase dpUploader)
        {

            if (ModelState.IsValid)
            {
                var selectedCourse = teacher.SelectedCourses;
                if (selectedCourse != null)
                {
                    ICollection<Course> courses = new List<Course>();
                    foreach (var item in selectedCourse)
                    {
                        Course c = db.Courses.Find(item);
                        courses.Add(c);
                    }
                    teacher.LstCourse = courses;/* many Courses*/
                }
                if (dpUploader != null && dpUploader.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(dpUploader.FileName);
                    string _path = Path.Combine(Server.MapPath(@"~/UploadedFiles/ProfilePics/"), _FileName);
                    dpUploader.SaveAs(_path);
                    teacher.ProfilePic = _FileName;

                }
                db.Teachers.Add(teacher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            common = new Common();
            teacher.Courses = common.GetCourses();
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            else
            {
                teacher.Courses = common.GetCourses();
                teacher.SelectedCourses = (from c in db.Courses
                                           from t in c.LstTeacher
                                           where t.Id == id
                                           select c.Id).ToList();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email,Phone,DoB,RollNo,Address,City,SelectedCourses")] Teacher teacher, HttpPostedFileBase dpUploader)
        {
            if (ModelState.IsValid)
            {
                var item = db.Entry<Teacher>(teacher);
                item.State = EntityState.Modified;
                item.Collection(i => i.LstCourse).Load();
                teacher.LstCourse.Clear();
                var selectedCourse = teacher.SelectedCourses;
                if (selectedCourse != null)
                {
                    foreach (var course in selectedCourse)
                    {

                        Course c = db.Courses.Find(course);
                        teacher.LstCourse.Add(c);

                    }
                }
                teacher.Address = Sanitizer.GetSafeHtmlFragment(teacher.Address);

                if (dpUploader != null && dpUploader.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(dpUploader.FileName);
                    string _path = Path.Combine(Server.MapPath(@"~/UploadedFiles/ProfilePics/"), _FileName);
                    dpUploader.SaveAs(_path);
                    teacher.ProfilePic = _FileName;


                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Teacher teacher = db.Teachers.Find(id);
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Teacher teacher = db.Teachers.Find(id);
            db.Teachers.Remove(teacher);
            db.SaveChanges();
            string path = Server.MapPath("@~/UploadedFiles/ProfilePics/" + teacher.ProfilePic);
            FileInfo file = new FileInfo(path);
            if (file.Exists)//check file exsit or not
            {
                file.Delete();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
