using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdmissionManagementSystem.Models;
using System.Web.Script.Serialization;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Web;
using System;
using Microsoft.Security.Application;

namespace AdmissionManagementSystem.Controllers
{
    public class StudentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Common common = new Common();
        // GET: Students
        public ActionResult Index()
        {

            return View(db.Students.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            common = new Common();
            Student student = new Student();

            student.Courses = common.GetCourses();

            return View(student);
        }


        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "StudentId,Standard,Stream,Year,College,FirstName,LastName,Email,Phone,DoB,RollNo,Address,City,SelectedCourses,Courses,PayingAmt")] Student student, HttpPostedFileBase dpUploader)
        {
            if (ModelState.IsValid)
            { /*Many to many relation One student can have many courses*/
                decimal totalCourseFee = 0;
                var selectedCourse = student.SelectedCourses;
                if (selectedCourse == null)
                { ModelState.AddModelError("SelectedCourses", "Please select minimum 1 course.");
                    student.Courses = common.GetCourses();
                    return View(student);
                }
                ICollection<Course> courses = new List<Course>();
                foreach (var item in selectedCourse)
                {
                    Course c = db.Courses.Find(item);
                    courses.Add(c);
                    totalCourseFee += c.Fee;
                }
                student.LstCourse = courses;/* many Courses*/

                student.Address = Sanitizer.GetSafeHtmlFragment(student.Address);
               
              

                if (dpUploader != null && dpUploader.ContentLength > 0)
                {

                    string _FileName = Path.GetFileName(dpUploader.FileName);
                    string _path = Path.Combine(Server.MapPath(@"~/UploadedFiles/ProfilePics/"), _FileName);
                    dpUploader.SaveAs(_path);
                    student.ProfilePic = _FileName;

                }
                db.Students.Add(student);
                db.SaveChanges();
                /* Payment information*/
                var payment = new PaymentDetail();
                payment.StudentId = student.StudentId;
                payment.TotalAmount = totalCourseFee;
                payment.PayingAmount = student.PayingAmt;
                payment.BalanceAmount = totalCourseFee - student.PayingAmt;
                payment.PaymentDate = DateTime.Now;
                db.PaymentDetails.Add(payment);


                db.SaveChanges();

                return RedirectToAction("Index");
            }
            common = new Common();
            student.Courses = common.GetCourses();
            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);

            if (student == null)
            {
                return HttpNotFound();
            }
            else
            {
                student.Courses = common.GetCourses();


                student.SelectedCourses = (from c in db.Courses
                                           from s in c.LstStudent
                                           where s.StudentId == id
                                           select c.Id).ToList();
                var paymentdetails = db.PaymentDetails.Where(p => p.StudentId == id).OrderByDescending(p => p.PaymentDate).FirstOrDefault();
                if (paymentdetails != null)
                { student.PayingAmt = paymentdetails.BalanceAmount; }
            }
            return View(student);
        }



        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "StudentId,Standard,Stream,Year,College,FirstName,LastName,Email,Phone,DoB,RollNo,Address,City,Courses,PayingAmt,SelectedCourses,ProfilePic")] Student student, HttpPostedFileBase dpUploader)
        {
            if (ModelState.IsValid)
            {
                student.Address = Sanitizer.GetSafeHtmlFragment(student.Address);
                var selectedCourse = student.SelectedCourses;

                decimal totalCourseFee = 0;

                /*to rmove all existing courses and add new */
                var item = db.Entry<Student>(student);
                item.State = EntityState.Modified;

                // item.Collection(i => i.LstCourse).Load();
                // student.LstCourse.Clear();
                PaymentDetail existingPaymentRecord = db.PaymentDetails.Where(p => p.StudentId == student.StudentId).OrderByDescending(p => p.PaymentDate).FirstOrDefault();
                var payment = new PaymentDetail();
                payment.StudentId = student.StudentId;

                payment.PayingAmount = student.PayingAmt;
                payment.PaymentDate = DateTime.Now;

                if (selectedCourse != null)
                {
                    foreach (var course in selectedCourse)
                    {

                        Course c = db.Courses.Find(course);
                        student.LstCourse.Add(c);
                        totalCourseFee += c.Fee;
                    }

                    if (existingPaymentRecord != null)
                    {
                        totalCourseFee += existingPaymentRecord.TotalAmount;
                    }
                    decimal prevSum = 0;
                    IEnumerable<PaymentDetail> details = db.PaymentDetails.Where(p => p.StudentId == student.StudentId).ToList();
                    if (details != null)
                    {
                        prevSum = details.Sum(s => s.PayingAmount);

                    }

                    payment.BalanceAmount = totalCourseFee - (student.PayingAmt + prevSum);
                    //(existingPaymentRecord.BalanceAmount - student.PayingAmt) + totalCourseFee;
                    payment.TotalAmount = totalCourseFee;
                    db.PaymentDetails.Add(payment);
                }
                else
                {
                    if (existingPaymentRecord != null && student.PayingAmt > 0)
                    {
                        payment.BalanceAmount = existingPaymentRecord.BalanceAmount - student.PayingAmt;
                        payment.TotalAmount = existingPaymentRecord.TotalAmount;
                        db.PaymentDetails.Add(payment);
                    }

                }


                if (dpUploader != null && dpUploader.ContentLength > 0)
                {
                   
                    string _FileName = Path.GetExtension(dpUploader.FileName);
                    string _path = Path.Combine(Server.MapPath(@"~/UploadedFiles/ProfilePics/"), _FileName);
                    dpUploader.SaveAs(_path);
                    student.ProfilePic = _FileName;


                }

                db.SaveChanges();


                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();

            string path = Server.MapPath("@~/UploadedFiles/ProfilePics/" + student.ProfilePic);
            FileInfo file = new FileInfo(path);
            if (file.Exists)//check file exsit or not
            {
                file.Delete();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetCourseFees(string[] list)
        {


            JavaScriptSerializer js = new JavaScriptSerializer();
            int[] lists = js.Deserialize<int[]>(list[0]);
            //double fee = (from cust in db.Courses
            //              where lists.Contains(cust.Id)
            //              select cust.Fee).ToList().AsEnumerable().Sum();

            IEnumerable<Course> LstCourses = (from course in db.Courses
                                              where lists.Contains(course.Id)
                                              select course).ToList();

            //double fee = LstCourses.Sum(s => s.Fee);

            return Json(LstCourses, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ViewReceipt(Student student)
        {

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public FileResult ViewReceipt(string GridHtml)
        {

            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "Grid.pdf");
            }
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
