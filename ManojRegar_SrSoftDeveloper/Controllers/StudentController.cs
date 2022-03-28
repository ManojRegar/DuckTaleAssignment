using ManojRegar_SrSoftDeveloper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ManojRegar_SrSoftDeveloper.Controllers
{
    public class StudentController : Controller
    {
        DBContext db = new DBContext();
        // GET: Student
        public ActionResult Index(FormCollection collection)
        {
            string searchKey = "";
            string filterKey = "";
            try
            {
                searchKey = collection["txtSearch"];
                filterKey = collection["ddlFilter"];
            }
            catch(Exception)
            {
                
            }
            
            var StudentsData = db.tbl_Students.AsEnumerable().ToList();
            //if (string.IsNullOrEmpty(searchKey) == false && string.IsNullOrEmpty(filterKey))
            //{
            //    searchKey = searchKey.ToUpper();
            //    StudentsData = StudentsData.Where(x => x.FirstName.ToUpper().Contains(searchKey) || x.LastName.ToUpper().Contains(searchKey) ||
            //    x.Class.ToUpper().Contains(searchKey)).ToList();
            //    foreach (var item in StudentsData)
            //    {
            //        item.Subjects = item.Subjects.Where(s => s.Name.ToUpper().Contains(searchKey) || s.Marks.ToString().ToUpper().Contains(searchKey)).ToList();
            //    }
            //}
            //else
            if (string.IsNullOrEmpty(filterKey) == false && string.IsNullOrEmpty(searchKey) == false)
            {
                if(filterKey.ToUpper() == "CLS")
                {
                    StudentsData = StudentsData.Where(x => x.Class.ToUpper().Contains(searchKey.ToUpper())).ToList();
                }
                else
                {
                    foreach (var item in StudentsData)
                    {
                        item.Subjects = item.Subjects.Where(s => s.Name.ToUpper().Contains(searchKey.ToUpper())).ToList();
                    }
                    StudentsData = StudentsData.Where(x => x.Subjects.Count() > 0).ToList();
                }
            }
            TempData["txtSearch"] = searchKey;
            TempData["ddlFilter"] = filterKey;
            return View(StudentsData.OrderBy(x=>x.FirstName).ThenBy(x => x.LastName).ToList());
        }

        public ActionResult CreateStudent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateStudent(Students model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Students student = new Students();
                    student.FirstName = model.FirstName;
                    student.LastName = model.LastName;
                    student.Class = model.Class;
                    db.tbl_Students.Add(student);
                    db.SaveChanges();
                    TempData["StudentFormSuccess"] = "Student record saved successfully";
                }
                catch (Exception ex)
                {
                    TempData["StudentFormError"] = "Something went Wrong!";
                }
            }
            return View(model);
        }

        public ActionResult EditStudent(int id)
        {
            return View(db.tbl_Students.Find(id));
        }

        [HttpPost]
        public ActionResult EditStudent(Students model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Students student = db.tbl_Students.Find(model.ID);
                    if (student != null)
                    {
                        student.FirstName = model.FirstName;
                        student.LastName = model.LastName;
                        student.Class = model.Class;

                        db.SaveChanges();
                        TempData["StudentFormSuccess"] = "Student record updated successfully.";
                    }
                    else
                    {
                        TempData["StudentFormError"] = "Student record not found!";
                    }
                }
                catch (Exception ex)
                {
                    TempData["StudentFormError"] = "Something went Wrong!";
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteStudent(int id)
        {
            string msg = "";
            try
            {
                var student = db.tbl_Students.Find(id);
                if (student != null)
                {
                    List<int> ids = student.Subjects.Select(x => x.ID).ToList();

                    foreach (var subId in ids)
                    {
                        db.tbl_Subjects.Remove(db.tbl_Subjects.Find(subId));
                        db.SaveChanges();
                    }
                    db.tbl_Students.Remove(student);
                    db.SaveChanges();
                    msg = "Student record deleted successfully";
                }
                else
                {
                    msg = "Student record not found!";
                }
            }
            catch (Exception ex)
            {
                msg = "Something went wrong!";
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateSubject()
        {
            ViewBag.StudentId = db.tbl_Students.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.ID.ToString() }).ToList<SelectListItem>();
            return View();
        }

        [HttpPost]
        public ActionResult CreateSubject(Subjects model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Subjects subject = new Subjects();
                    subject.Name = model.Name;
                    subject.Marks = model.Marks;
                    subject.StudentId = model.StudentId;
                    db.tbl_Subjects.Add(subject);
                    db.SaveChanges();
                    TempData["SubjectFormSuccess"] = "Subject record saved successfully.";
                }
                catch (Exception ex)
                {
                    TempData["SubjectFormError"] = "Something went Wrong!";
                }
            }
            ViewBag.StudentId = db.tbl_Students.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.ID.ToString() }).ToList<SelectListItem>();
            return View(model);
        }

        public ActionResult EditSubject(int id,bool isSample2 = false)
        {
            TempData["IsSample2"] = isSample2;
            ViewBag.StudentId = db.tbl_Students.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.ID.ToString() }).ToList<SelectListItem>();
            return View(db.tbl_Subjects.Find(id));
        }

        [HttpPost]
        public ActionResult EditSubject(Subjects model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Subjects subject = db.tbl_Subjects.Find(model.ID);
                    if (subject != null)
                    {
                        subject.Name = model.Name;
                        subject.Marks = model.Marks;
                        subject.StudentId = model.StudentId;
                        db.SaveChanges();
                        TempData["SubjectFormSuccess"] = "Subject record updated successfully.";
                    }
                    else
                    {
                        TempData["SubjectFormError"] = "Subject details not found!";
                    }

                }
                catch (Exception ex)
                {
                    TempData["SubjectFormError"] = "Something went wrong!";
                }
            }
            TempData["IsSample2"] = TempData["IsSample2"]!= null ? Convert.ToBoolean(TempData["IsSample2"]) : false;
            ViewBag.StudentId = db.tbl_Students.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.ID.ToString() }).ToList<SelectListItem>();
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteSubject(int id)
        {
            string msg = "";
            try
            {
                var subject = db.tbl_Subjects.Find(id);
                if (subject != null)
                {
                    db.tbl_Subjects.Remove(subject);
                    db.SaveChanges();
                    msg = "Subject deleted successfully";
                }
                else
                {
                    msg = "Subject details not found!";
                }
            }
            catch (Exception)
            {
                msg = "Something went wrong!";
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StudentsList2()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetStudentsList(string searchKey="", string filterKey="")
        {
            var StudentsData = db.tbl_Students.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).AsEnumerable().ToList();
            if (string.IsNullOrEmpty(filterKey) == false && string.IsNullOrEmpty(searchKey) == false)
            {
                if (filterKey.ToUpper() == "CLS")
                {
                    StudentsData = StudentsData.Where(x => x.Class.ToUpper().Contains(searchKey.ToUpper())).ToList();
                }
                else
                {
                    foreach (var item in StudentsData)
                    {
                        item.Subjects = item.Subjects.Where(s => s.Name.ToUpper().Contains(searchKey.ToUpper())).ToList();
                    }
                    StudentsData = StudentsData.Where(x => x.Subjects.Count() > 0).ToList();
                }
            }
            List<StudentListModel> sdtList = new List<StudentListModel>();

            foreach(var stdData in StudentsData)
            {
                if (stdData.Subjects.Count() > 0)
                {
                    foreach (var subData in stdData.Subjects)
                    {
                        if (sdtList.Where(x=>x.StudentID == stdData.ID).Count() == 0)
                        {
                            StudentListModel std = new StudentListModel();
                            std.FirstName = stdData.FirstName;
                            std.LastName = stdData.LastName;
                            std.Class = stdData.Class;
                            std.SubjectName = subData.Name;
                            std.Marks = subData.Marks.ToString("#0.00");
                            std.SubjectID = subData.ID;
                            std.StudentID = stdData.ID;
                            std.IsFirst = true;
                            sdtList.Add(std);
                        }
                        else
                        {
                            StudentListModel std = new StudentListModel();
                            std.SubjectName = subData.Name;
                            std.Marks = subData.Marks.ToString("#0.00");
                            std.SubjectID = subData.ID;
                            std.StudentID = stdData.ID;
                            std.IsFirst = false;
                            sdtList.Add(std);
                        }
                    }
                }
                else
                {
                    StudentListModel std = new StudentListModel();
                    std.FirstName = stdData.FirstName;
                    std.LastName = stdData.LastName;
                    std.Class = stdData.Class;
                    std.StudentID = stdData.ID;
                    std.IsFirst = true;
                    sdtList.Add(std);
                }
                
            }
            return Json(sdtList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateStudent2()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateStudent2(Students model)
        {
            var msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    Students student = new Students();
                    student.FirstName = model.FirstName;
                    student.LastName = model.LastName;
                    student.Class = model.Class;
                    db.tbl_Students.Add(student);
                    db.SaveChanges();

                    if(model.Subjects != null && model.Subjects.Count() > 0)
                    {
                        foreach(var sub in model.Subjects)
                        {
                            Subjects subject = new Subjects();
                            subject.Name = sub.Name;
                            subject.Marks = sub.Marks;
                            subject.StudentId = student.ID;
                            db.tbl_Subjects.Add(subject);
                            db.SaveChanges();
                        }
                    }
                    msg = "Record saved successfully";
                }
                catch (Exception ex)
                {
                    msg = "Something went Wrong!";
                }
            }
            return Json(msg,JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditStudent2(int id)
        {
            return View(db.tbl_Students.Find(id));
        }

        [HttpPost]
        public ActionResult EditStudent2(Students model)
        {
            var msg = "";
            if (ModelState.IsValid)
            {
                try
                {
                    Students student = db.tbl_Students.Find(model.ID);
                    if(student != null)
                    {
                        student.FirstName = model.FirstName;
                        student.LastName = model.LastName;
                        student.Class = model.Class;
                        db.SaveChanges();

                        if (model.Subjects != null && model.Subjects.Count() > 0)
                        {
                            foreach (var sub in model.Subjects)
                            {
                                Subjects subject = db.tbl_Subjects.Find(sub.ID);
                                if(subject != null)
                                {
                                    subject.Name = sub.Name;
                                    subject.Marks = sub.Marks;
                                    subject.StudentId = student.ID;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    subject = new Subjects();
                                    subject.Name = sub.Name;
                                    subject.Marks = sub.Marks;
                                    subject.StudentId = student.ID;
                                    db.tbl_Subjects.Add(subject);
                                    db.SaveChanges();
                                }
                                
                            }
                        }
                        msg = "Student record updated successfully";
                    }
                    else
                    {
                        msg = "Student record not found!";
                    }

                }
                catch (Exception ex)
                {
                    msg = "Something went Wrong!";
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

    }
}