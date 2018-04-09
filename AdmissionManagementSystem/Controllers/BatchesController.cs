using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdmissionManagementSystem.Models;
using System.Globalization;

namespace AdmissionManagementSystem.Controllers
{
    public class BatchesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Common common = new Common();
        // GET: Batches
        public ActionResult Index()
        {
            return View(db.Batches.ToList());
        }

        // GET: Batches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Batch batch = db.Batches.Find(id);
            var d = db.Courses.ToList();
            if (batch == null)
            {
                return HttpNotFound();
            }
            return View(batch);
        }

        // GET: Batches/Create
        public ActionResult Create()
        {
            Batch batch = new Batch();
            batch.WeekDaysList = common.WeekDaysList();
            batch.LstCourse = common.GetCourses();
            return View(batch);
        }

        // POST: Batches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BatchId,Name,StartDate,EndDate,Course_Id,Time")] Batch batch, string[] Selected)
        {
            if (ModelState.IsValid)
            {

                batch.csvDays = string.Join(",", Selected);
                db.Batches.Add(batch);
                db.SaveChanges();
              
                return RedirectToAction("Index");
            }
            else
            {
                batch.WeekDaysList = common.WeekDaysList();
                batch.LstCourse = common.GetCourses();
                if (Selected == null)
                { ModelState.AddModelError("csvDays", "The Days field is required."); }
                return View(batch);
            }



        }

        // GET: Batches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Batch batch = db.Batches.Find(id);
            batch.WeekDaysList = common.WeekDaysList();
            foreach (var item in batch.WeekDaysList)
            {
                foreach (var day in batch.csvDays.Split(','))
                {
                    if (item.Value == day)
                    { item.Selected = true; break; }
                    else
                    { item.Selected = false; }
                }
            }
            batch.LstCourse = common.GetCourses();



            if (batch == null)
            {
                return HttpNotFound();
            }
            return View(batch);
        }

        // POST: Batches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BatchId,Name,StartDate,EndDate,Course_Id,Time")] Batch batch, string[] Selected)
        {
            if (ModelState.IsValid)
            {
                batch.csvDays = string.Join(",", Selected);
                db.Entry(batch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                batch.LstCourse = common.GetCourses(); batch.WeekDaysList = common.WeekDaysList();

                if (Selected == null)
                { ModelState.AddModelError("csvDays", "The Days field is required."); }
                else
                {
                    foreach (var item in batch.WeekDaysList)
                    {
                        foreach (var day in Selected)
                        {
                            if (item.Value == day)
                            { item.Selected = true; break; }
                            else
                            { item.Selected = false; }
                        }
                    }
                }
                return View(batch);
            }

        }

        // GET: Batches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Batch batch = db.Batches.Find(id);
            if (batch == null)
            {
                return HttpNotFound();
            }
            return View(batch);
        }

        // POST: Batches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Batch batch = db.Batches.Find(id);
            db.Batches.Remove(batch);
            db.SaveChanges();
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
