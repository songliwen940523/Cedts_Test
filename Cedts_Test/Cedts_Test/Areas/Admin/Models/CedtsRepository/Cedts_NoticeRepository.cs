using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_NoticeRepository : ICedts_NoticeRepository
    {
        #region 实例化Cedts_Entities
        Cedts_Entities db;
        public Cedts_NoticeRepository()
        {
            db = new Cedts_Entities();
        }
        #endregion

        CEDTS_SystemOverview ICedts_NoticeRepository.GetSystemInfo()
        {
            var item = db.CEDTS_SystemOverview.FirstOrDefault();
            if (item == null)
            {
                CEDTS_SystemOverview so = new CEDTS_SystemOverview();
                so.Content = string.Empty;
                so.Intro = string.Empty;
                so.Time = DateTime.Now;
                so.SystemOverviewID = 1;
                db.AddToCEDTS_SystemOverview(so);
                db.SaveChanges();
                item = so;
            }
            return item;
        }

        void ICedts_NoticeRepository.EditSystem(string content, string intro)
        {
            var system = db.CEDTS_SystemOverview.FirstOrDefault();
            if (system != null)
            {
                var tempSystem = system;
                tempSystem.Content = content;
                tempSystem.Intro = intro;
                tempSystem.Time = DateTime.Now;
                db.ApplyCurrentValues(system.EntityKey.EntitySetName, tempSystem);
            }
            else
            {
                system = new CEDTS_SystemOverview();
                system.Content = content;
                system.Intro = intro;
                system.Time = DateTime.Now;
                db.AddToCEDTS_SystemOverview(system);
            }
            db.SaveChanges();
        }

        List<CEDTS_CoreFeatures> ICedts_NoticeRepository.GetFeaturesInfo()
        {
            var featuresList = db.CEDTS_CoreFeatures.OrderBy(p => p.Order).ToList();
            if (featuresList.Count > 0)
            {
                return featuresList;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    CEDTS_CoreFeatures cf = new CEDTS_CoreFeatures();
                    cf.Content = string.Empty;
                    cf.Intro = string.Empty;
                    cf.Order = i + 1;
                    db.AddToCEDTS_CoreFeatures(cf);
                }
                db.SaveChanges();
                return db.CEDTS_CoreFeatures.OrderBy(p => p.Order).ToList();
            }
        }

        void ICedts_NoticeRepository.EditFeatures(List<CEDTS_CoreFeatures> featuresList)
        {
            var id1 = featuresList[0].CoreFeatures_ID;
            var id2 = featuresList[1].CoreFeatures_ID;
            var id3 = featuresList[2].CoreFeatures_ID;
            var old1 = db.CEDTS_CoreFeatures.Where(p => p.CoreFeatures_ID == id1).FirstOrDefault();
            var old2 = db.CEDTS_CoreFeatures.Where(p => p.CoreFeatures_ID == id2).FirstOrDefault();
            var old3 = db.CEDTS_CoreFeatures.Where(p => p.CoreFeatures_ID == id3).FirstOrDefault();
            db.ApplyCurrentValues(old1.EntityKey.EntitySetName, featuresList[0]);
            db.ApplyCurrentValues(old2.EntityKey.EntitySetName, featuresList[1]);
            db.ApplyCurrentValues(old3.EntityKey.EntitySetName, featuresList[2]);
            db.SaveChanges();
        }

        CEDTS_Instructions ICedts_NoticeRepository.GetInstructionsInfo()
        {
            var item = db.CEDTS_Instructions.FirstOrDefault();
            if (item == null)
            {
                CEDTS_Instructions instrucion = new CEDTS_Instructions();
                instrucion.Content = string.Empty;
                instrucion.InstructionsID = 1;
                db.AddToCEDTS_Instructions(instrucion);
                db.SaveChanges();
                item = instrucion;
            }
            return item;
        }

        void ICedts_NoticeRepository.EidtInstructions(string content)
        {
            var instructions = db.CEDTS_Instructions.FirstOrDefault();
            if (instructions != null)
            {
                var tempInstructions = instructions;
                tempInstructions.Content = content;
                db.ApplyCurrentValues(instructions.EntityKey.EntitySetName, tempInstructions);
            }
            else
            {
                instructions = new CEDTS_Instructions();
                instructions.Content = content;
                db.AddToCEDTS_Instructions(instructions);
            }
            db.SaveChanges();
        }

        CEDTS_Contact ICedts_NoticeRepository.GetContactInfo()
        {
            var item = db.CEDTS_Contact.FirstOrDefault();
            if (item == null)
            {
                CEDTS_Contact contact = new CEDTS_Contact();
                contact.Address = string.Empty;
                contact.ContactID = 1;
                contact.Name = string.Empty;
                contact.Tel = string.Empty;
                contact.Url = string.Empty;
                contact.ZipCode = string.Empty;
                db.AddToCEDTS_Contact(contact);
                db.SaveChanges();
                item = contact;
            }
            return item;
        }

        void ICedts_NoticeRepository.EidtContact(CEDTS_Contact contact)
        {
            var old = db.CEDTS_Contact.FirstOrDefault();
            if (old != null)
            {
                contact.ContactID = old.ContactID;
                db.ApplyCurrentValues(old.EntityKey.EntitySetName, contact);
            }
            else
            {
                db.AddToCEDTS_Contact(contact);
            }
            db.SaveChanges();
        }

        CEDTS_Saying ICedts_NoticeRepository.GetSayingInfo()
        {
            var item = db.CEDTS_Saying.FirstOrDefault();
            if (item == null)
            {
                CEDTS_Saying saying = new CEDTS_Saying();
                saying.Content = string.Empty;
                saying.Note = string.Empty;
                saying.SayingID = 1;
                saying.Time = DateTime.Now;
                db.AddToCEDTS_Saying(saying);
                db.SaveChanges();
                item = saying;
            }
            return item;
        }

        void ICedts_NoticeRepository.EidtSaying(string content, string note)
        {
            var say = db.CEDTS_Saying.FirstOrDefault();
            if (say != null)
            {
                var tempsay = say;
                tempsay.Content = content;
                tempsay.Note = note;
                tempsay.Time = DateTime.Now;
                db.ApplyCurrentValues(tempsay.EntityKey.EntitySetName, say);
            }
            else
            {
                say = new CEDTS_Saying();
                say.Content = content;
                say.Note = note;
                say.Time = DateTime.Now;
                db.AddToCEDTS_Saying(say);
            }
            db.SaveChanges();
        }

        List<CEDTS_Feedback> ICedts_NoticeRepository.GetFeedback()
        {
            return db.CEDTS_Feedback.ToList();
        }

        void ICedts_NoticeRepository.DeleteFeedBackbyID(int id)
        {
            var item = db.CEDTS_Feedback.Where(p => p.FeedbackID == id).FirstOrDefault();
            db.DeleteObject(item);
            db.SaveChanges();
        }

        void ICedts_NoticeRepository.DeleteFeedBackAll()
        {
            var itemList = db.CEDTS_Feedback.ToList();
            while (itemList.Count > 0)
            {
                int tempID = itemList[0].FeedbackID;
                db.DeleteObject(itemList[0]);
                itemList.Remove(itemList[0]);
            }
            db.SaveChanges();
        }
    }
}