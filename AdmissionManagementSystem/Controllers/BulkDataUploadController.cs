using AdmissionManagementSystem.Models;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdmissionManagementSystem.Controllers
{
    public class BulkDataUploadController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: BulkDataUpload
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, string type)
        {
            if (file != null)
            {

                string extension = System.IO.Path.GetExtension(file.FileName);
                try
                {


                    if (extension == ".xltm")
                    {
                        switch (type)
                        {
                            case "Students":
                                UploadStudents(file);
                                break;

                            case "Courses":
                                UploadCourses(file);
                                break;

                            case "Teachers":
                                UploadTeachers(file);
                                break;

                            case "StudentPayments":
                                UploadStudentPayment(file);
                                break;

                        }
                    }
                    ViewBag.error = "Data saved successfully";
                }
                catch (Exception)
                {

                    ViewBag.error = "Error in file upload";
                }
            }
            return View();
        }

        private int UploadStudents(HttpPostedFileBase file)
        {
            int success = 0;
            try
            {



                var result = ReadExelFile(file);
                if (result != null)
                {
                    if (result.Tables.Count > 1)
                    {
                        var rows = result.Tables["Students"].Rows;

                        if (rows.Count > 0)
                        {
                            db.Configuration.AutoDetectChangesEnabled = false;
                            foreach (DataRow item in rows)
                            {
                                Student student = new Student();
                                student.FirstName = Convert.ToString(item["FirstName"]);
                                student.LastName = Convert.ToString(item["LastName"]);
                                student.Email = Convert.ToString(item["Email"]);
                                student.Phone = Convert.ToString(item["Phone"]);
                                student.DoB = Convert.ToDateTime(item["DoB"]);
                                student.RollNo = Convert.ToString(item["RollNo"]);
                                student.Address = Convert.ToString(item["Address"]);
                                student.City = Convert.ToString(item["City"]);
                                student.Standard = Convert.ToString(item["Degree"]);
                                student.Stream = Convert.ToString(item["Stream"]);
                                student.Year = Convert.ToString(item["Year"]);
                                student.College = Convert.ToString(item["College"]);

                                var _courses = Convert.ToString(item["LstCourse"]).Split('/');

                                ICollection<Course> courses = new List<Course>();


                                foreach (var course in _courses)
                                {
                                    var id = Convert.ToInt32(course.Split(':')[0]);
                                    Course c = db.Courses.Find(id);
                                    courses.Add(c);
                                    c = null;
                                }

                                student.LstCourse = courses;
                                db.Students.Add(student);
                            }
                            success = db.SaveChanges();
                            db.Configuration.AutoDetectChangesEnabled = true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return success;
        }

        private int UploadCourses(HttpPostedFileBase file)
        {
            int success = 0;
            var result = ReadExelFile(file);
            if (result != null)
            {
                var rows = result.Tables["Courses"].Rows;

                if (rows.Count > 0)
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    foreach (DataRow item in rows)
                    {
                        Course c = new Course();
                        c.Name = Convert.ToString(item["Name"]).Split(':')[1];
                        c.Duration = Convert.ToInt32(item["Duration(In Months)"]);
                        c.Fee = Convert.ToDecimal(item["Fee"]);
                        db.Courses.Add(c);
                    }
                    success = db.SaveChanges();
                    db.Configuration.AutoDetectChangesEnabled = true;
                }
            }
            return success;

        }

        private int UploadStudentPayment(HttpPostedFileBase file)
        {
            int success = 0;
            var result = ReadExelFile(file);
            if (result != null)
            {

                var rows = result.Tables["StudentPayment"].Rows;

                if (rows.Count > 0)
                {
                    db.Configuration.AutoDetectChangesEnabled = false;
                    foreach (DataRow item in rows)
                    {
                        var payment = new PaymentDetail();
                        payment.StudentId = Convert.ToInt32(item["StudentId"]);
                        payment.TotalAmount = Convert.ToDecimal(item["TotalFee"]);
                        payment.PayingAmount = Convert.ToDecimal(item["Paid"]);
                        payment.BalanceAmount = payment.TotalAmount - payment.PayingAmount;
                        payment.PaymentDate = DateTime.Now;
                        db.PaymentDetails.Add(payment);



                    }
                    success = db.SaveChanges();
                    db.Configuration.AutoDetectChangesEnabled = true;
                }
            }
            return success;
        }
        private void UploadTeachers(HttpPostedFileBase file)
        { }
        private DataSet ReadExelFile(HttpPostedFileBase file)
        {
            try
            {


                DataSet result = new DataSet();
                if (file != null && file.ContentLength > 0)
                {

                    Stream stream = file.InputStream;

                    IExcelDataReader reader = null;

                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                    result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    reader.Close();

                }
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}