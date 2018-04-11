using AdmissionManagementSystem.Models;
using AdmissionManagementSystem.Models.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdmissionManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Dashboard
        public ActionResult Index()
        {
            DashboardViewModel vm = new DashboardViewModel();
            var CoursesCount = db.Courses.Count();
            var BatchesCount = db.Batches.Count();
            var DefaultersCount = db.PaymentDetails.Where(p => p.BalanceAmount != 0).ToList().Count();
            vm.NameCountList = new List<NameCount> {     new NameCount  {Name="New Admissions",Count= 9},
                                                         new NameCount { Name = "Courses", Count = CoursesCount },
                                                         new NameCount  {Name="Batches",Count= BatchesCount},
                                                         new NameCount  {Name="Fee Defaulters",Count= DefaultersCount}
                                                   };
            Common common = new Common();
            vm.Courses = common.GetCourses();
            var courseId = Convert.ToInt32(vm.Courses.FirstOrDefault().Value);
            vm.Batches = (from b in db.Batches

                          where b.Course_Id == courseId
                          select new BatchBox { BatchId = b.BatchId, Name = b.Name, Count = b.LstStudent.Count }).ToList();
            //  db.Batches.Where(b => b.Course_Id == 1).
            return View(vm);
        }

        [HttpPost]
        public JsonResult LoadBatches(int CourseId)
        {
            DashboardViewModel vm = new DashboardViewModel();
            vm.Batches = (from b in db.Batches

                          where b.Course_Id == CourseId
                          select new BatchBox { BatchId = b.BatchId, Name = b.Name, Count = b.LstStudent.Count, NavigationUrl = "Dashboard/LoadStudentsBatches" }).ToList();
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadStudentsBatches(int id)
        {
            Batch batch = new Batch();
            var Batches = (from b in db.Batches
                           from stud in b.LstStudent
                           where b.BatchId == id
                           select new { stud, b }).ToList();
            if (Batches.Count() > 0)
            {
                batch.Name = Batches[0].b.Name;
                batch.csvDays = Batches[0].b.csvDays;
                batch.StartDate = Batches[0].b.StartDate;
                batch.EndDate = Batches[0].b.EndDate;
                batch.Time = Batches[0].b.Time;
                foreach (var bt in Batches)
                {
                    batch.LstStudent.Add(bt.stud);

                }
            }
            else
            {
                batch = db.Batches.Find(id);

            }

            return View(batch);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult LoadStudentsBatches(string GridHtml)
        {
            string csspath = Server.MapPath("~/Content/Site.css");
            string csspath1 = Server.MapPath("~/Content/bootstrap.min.css");

            string cssText = System.IO.File.ReadAllText(csspath);

            cssText += System.IO.File.ReadAllText(csspath1);
            var cssMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssText));
            var htmlMemoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(GridHtml));

            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, htmlMemoryStream, cssMemoryStream);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "BatchDetails"+DateTime.Now+".pdf");
            }
        }
    }
}
