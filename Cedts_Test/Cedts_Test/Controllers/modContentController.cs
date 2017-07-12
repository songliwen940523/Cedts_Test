using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cedts_Test.Controllers
{
    public class modContentController : Controller
    {
        //
        // GET: /modContent/
        Cedts_Test.Models.CedtsEntities db = new Models.CedtsEntities();
  
        public ActionResult Index(string id)
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            int typeID = int.Parse( form["typeId"]);
            var ass = (from m in db.CEDTS_AssessmentItem where m.ItemTypeID == typeID select m).ToList();
            foreach (var a in ass)
            {
                string org = a.Original;
                //org = org.Replace("<br/><br/><br/><br/><br/>", "<br/>");
                //org = org.Replace("<br/><br/><br/><br/>", "<br/>");
                //org = org.Replace("<br/><br/><br/>", "<br/>");
                //org = org.Replace("<br />", "<br/>");
                org = org.Replace("<br /><br/>\t<br /><br/>\t", " ");
                //org = org.Replace("<br />\r\n\t<br />\r\n\t", " ");
               
                Cedts_Test.Models.CEDTS_AssessmentItem assessment = new Models.CEDTS_AssessmentItem();
                assessment.Original = org;
                //if (a.AssessmentItemID.CompareTo(new Guid("f4c8ebd6-b448-4247-bdf9-a78a5e2670ef")) == 0)
                {
                    assessment.AssessmentItemID = a.AssessmentItemID;
                }
                //else
                //{
                //    continue;
                //}
                assessment.ItemTypeID = a.ItemTypeID;
                assessment.Description = a.Description;
                assessment.UserID = a.UserID;
                assessment.UpdateUserID = a.UpdateUserID;
                assessment.SaveTime = a.SaveTime;
                assessment.Duration = a.Duration;
                assessment.Difficult = a.Difficult;
                assessment.Score = a.Score;

                assessment.Count = a.Count;
                assessment.Content = a.Content;
                assessment.QuestionCount = a.QuestionCount;
                assessment.UpdateTime = a.UpdateTime;
                assessment.Course = a.Course;
                assessment.Unit = a.Unit;
                assessment.SoundFile = a.SoundFile;
                assessment.Interval = a.Interval;
                string jj = a.EntityKey.EntitySetName;
                db.ApplyCurrentValues(a.EntityKey.EntitySetName, assessment);
                db.SaveChanges();
                
            }
   
            return View();
    

        }
    }
}
