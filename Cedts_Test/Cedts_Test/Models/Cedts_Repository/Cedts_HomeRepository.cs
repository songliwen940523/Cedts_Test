using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Cedts_Test.Models
{
    public class Cedts_HomeRepository : ICedts_HomeRepository
    {
        #region 实例化Cedts_Entities
        CedtsEntities db;
        public Cedts_HomeRepository()
        {
            db = new CedtsEntities();
        }
        #endregion

        StatisticsScore ICedts_HomeRepository.TotalScore()
        {
            var Score = db.CEDTS_Test.OrderBy(p => p.TotalScore);

            StatisticsScore Ss = new StatisticsScore();
            Ss.Count = new List<int>();
            Ss.Score = new List<double>();
            Ss.Count.Add(0);
            Ss.Count.Add(0);
            Ss.Count.Add(0);
            Ss.Count.Add(0);
            Ss.Count.Add(0);
            Ss.Count.Add(0);
            Ss.Count.Add(0);
            Ss.Count.Add(0);
            Ss.Count.Add(0);
            if (Score.Count() >= 500)
            {
                foreach (var s in Score)
                {
                    if (0 <= s.TotalScore && s.TotalScore < 10)
                    {
                        Ss.Count[0] += 1;
                    }
                    if (10 <= s.TotalScore && s.TotalScore < 20)
                    {
                        Ss.Count[1] += 1;
                    }
                    if (20 <= s.TotalScore && s.TotalScore < 30)
                    {
                        Ss.Count[2] += 1;
                    }
                    if (30 <= s.TotalScore && s.TotalScore < 40)
                    {
                        Ss.Count[3] += 1;
                    }
                    if (40 <= s.TotalScore && s.TotalScore < 50)
                    {
                        Ss.Count[4] += 1;
                    }
                    if (50 <= s.TotalScore && s.TotalScore < 60)
                    {
                        Ss.Count[5] += 1;
                    }
                    if (60 <= s.TotalScore && s.TotalScore < 70)
                    {
                        Ss.Count[6] += 1;
                    }
                    if (70 <= s.TotalScore && s.TotalScore < 80)
                    {
                        Ss.Count[7] += 1;
                    }
                    if (80 <= s.TotalScore && s.TotalScore < 90)
                    {
                        Ss.Count[8] += 1;
                    }
                }
            }
            else
            {
                Ss.Count[0] = 10;
                Ss.Count[1] = 50;
                Ss.Count[2] = 110;
                Ss.Count[3] = 160;
                Ss.Count[4] = 110;
                Ss.Count[5] = 50;
                Ss.Count[6] = 10;
            }
            return Ss;
        }

        List<Knowledge> ICedts_HomeRepository.KPMaster()
        {
            List<Knowledge> knowList = new List<Knowledge>();

            var Kp = db.CEDTS_SampleKnowledgeInfomation;

            foreach (var m in Kp)
            {
                Knowledge know = new Knowledge();

                var kpname = (from s in db.CEDTS_KnowledgePoints where s.KnowledgePointID == m.KnowledgePointID select s.Title).FirstOrDefault();
                Regex regex = new Regex(@"[.\d]");//去除数字
                kpname = regex.Replace(kpname, "");
                double KP_MasterRate1 = m.KP_MasterRate.Value * 100;
                string KP_MasterRate = KP_MasterRate1 + "";

                if (KP_MasterRate.Length > 4)
                {
                    KP_MasterRate = KP_MasterRate.Substring(0, KP_MasterRate.IndexOf('.') + 2);
                }

                m.KP_MasterRate = Convert.ToDouble(KP_MasterRate);
                if (knowList.Count == 0)
                {
                    know.KPName = kpname;

                    know.UMi = m.KP_MasterRate.Value;
                    knowList.Add(know);
                }
                else
                {
                    int Right = 0;
                    for (int i = 0; i < knowList.Count; i++)
                    {
                        if (knowList[i].KPName == kpname)
                        {
                            knowList[i].UMi = (knowList[i].UMi + m.KP_MasterRate.Value) / 2;
                            Right = 1;
                        }
                    }
                    if (Right != 1)
                    {
                        know.KPName = kpname;
                        know.UMi = m.KP_MasterRate.Value;
                        knowList.Add(know);
                    }
                }
            }
            return knowList;
        }

        List<double> ICedts_HomeRepository.ItemInfo()
        {
            List<double> ItemInfo = new List<double>();
            var Item = (from m in db.CEDTS_SmapleAnswerTypeInfo orderby m.ItemTypeID select m.CorrectRate).ToList();
            foreach (var s in Item)
            {
                double s1 = s.Value * 100;
                string ss = s1 + "";
                if (ss.Length > 4)
                {
                    ss = ss.Substring(0, ss.IndexOf('.') + 2);
                }
                ItemInfo.Add(double.Parse(ss));
            }
            return ItemInfo;
        }

        StatisticsScore ICedts_HomeRepository.UserScore(string UserName)
        {
            var UserID = (from m in db.CEDTS_User where m.UserAccount == UserName select m.UserID).FirstOrDefault();
            var Score = (from m in db.CEDTS_Test where m.UserID == UserID orderby m.FinishDate descending select m).Take(10);

            StatisticsScore Ss = new StatisticsScore();
            Ss.Score = new List<double>();
            Ss.TypeName = new List<string>();
            foreach (var s in Score)
            {
                
                var typename = (from b in db.CEDTS_Paper where b.PaperID == s.PaperID select b).FirstOrDefault();
                Ss.Score.Add(s.TotalScore.Value/typename.Score.Value*100);
                Ss.TypeName.Add(typename.Title);
            }
            return Ss;
        }

        List<Knowledge> ICedts_HomeRepository.UserKpMaster(string UserName)
        {
            List<Knowledge> KnowList = new List<Knowledge>();
            var UserID = (from m in db.CEDTS_User where m.UserAccount == UserName select m.UserID).FirstOrDefault();
            var KpName = (from m in db.CEDTS_KnowledgePoints orderby m.Title select m);

            foreach (var kp in KpName)
            {
                string Name = string.Empty;
                Knowledge kown = new Knowledge();
                Regex regex = new Regex(@"[.\d]");
                Name = regex.Replace(kp.Title, "");
                var SMiInfo = (from m in db.CEDTS_SampleKnowledgeInfomation where m.KnowledgePointID == kp.KnowledgePointID select m.KP_MasterRate).FirstOrDefault();
                var UMiInfo = (from m in db.CEDTS_UserKnowledgeInfomation where m.UserID == UserID && m.KnowledgePointID == kp.KnowledgePointID select m.KP_MasterRate).FirstOrDefault();

                if (UMiInfo != null)
                {
                    double SMi1 = SMiInfo.Value * 100;
                    double UMi1 = UMiInfo.Value * 100;
                    string SMi = SMi1 + "";
                    string UMi = UMi1 + "";
                    if (SMi.Length > 4)
                    {
                        SMi = SMi.Substring(0, SMi.IndexOf('.') + 2);
                    }
                    if (UMi.Length > 4)
                    {
                        UMi = UMi.Substring(0, UMi.IndexOf('.') + 2);
                    }
                    kown.KPName = Name;
                    kown.SMi = double.Parse(SMi);
                    kown.UMi = double.Parse(UMi);
                    KnowList.Add(kown);
                }
            }
            return KnowList;
        }

        ItemScale ICedts_HomeRepository.UserItemInfo(string UserName)
        {
            ItemScale ItemInfo = new ItemScale();
            ItemInfo.Fortysix = new List<double>();
            ItemInfo.SScale = new List<double>();
            ItemInfo.UScale = new List<double>();

            ItemInfo.Fortysix.Add(12.4);
            ItemInfo.Fortysix.Add(9.87);
            ItemInfo.Fortysix.Add(8.64);
            ItemInfo.Fortysix.Add(12.4);
            ItemInfo.Fortysix.Add(13.6);
            ItemInfo.Fortysix.Add(12.4);
            ItemInfo.Fortysix.Add(18.5);
            ItemInfo.Fortysix.Add(12.4);
            ItemInfo.Fortysix.Add(12.4);

            var UserID = (from m in db.CEDTS_User where m.UserAccount == UserName select m.UserID).FirstOrDefault();
            var UItem = (from m in db.CEDTS_UserAnswerInfo where m.UserID == UserID orderby m.ItemTypeID select m);
            var SItem = (from m in db.CEDTS_SmapleAnswerTypeInfo orderby m.ItemTypeID select m);
            foreach (var s in SItem)
            {
                double s1 = 0.0;
                switch (s.ItemTypeID)
                {
                    case 1:
                        s1 = s.CorrectRate.Value * 1 * ItemInfo.Fortysix[0];
                        break;
                    case 2:
                        s1 = s.CorrectRate.Value * 1 * ItemInfo.Fortysix[1];
                        break;
                    case 3:
                        s1 = s.CorrectRate.Value * 1 * ItemInfo.Fortysix[2];
                        break;
                    case 4:
                        s1 = s.CorrectRate.Value * 1 * ItemInfo.Fortysix[3];
                        break;
                    case 5:
                        s1 = s.CorrectRate.Value * 1 * ItemInfo.Fortysix[4];
                        break;
                    case 6:
                        s1 = s.CorrectRate.Value * 1 * ItemInfo.Fortysix[5];
                        break;
                    case 7:
                        s1 = s.CorrectRate.Value * 1.5 * ItemInfo.Fortysix[6];
                        break;
                    case 8:
                        s1 = s.CorrectRate.Value * 0.5 * ItemInfo.Fortysix[7];
                        break;
                    case 9:
                        s1 = s.CorrectRate.Value * 1 * ItemInfo.Fortysix[8];
                        break;
                    default:
                        break;
                }
                string ss = s1 + "";
                if (ss.Length > 4)
                {
                    ss = ss.Substring(0, ss.IndexOf('.') + 3);
                }
                ItemInfo.SScale.Add(double.Parse(ss));
            }

            foreach (var u in UItem)
            {
                double u1 = 0.0;
                switch (u.ItemTypeID)
                {
                    case 1:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[0];
                        break;
                    case 2:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[1];
                        break;
                    case 3:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[2];
                        break;
                    case 4:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[3];
                        break;
                    case 5:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[4];
                        break;
                    case 6:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[5];
                        break;
                    case 7:
                        u1 = u.CorrectRate.Value * 1.5 * ItemInfo.Fortysix[6];
                        break;
                    case 8:
                        u1 = u.CorrectRate.Value * 0.5 * ItemInfo.Fortysix[7];
                        break;
                    case 9:
                        u1 = u.CorrectRate.Value * 1 * ItemInfo.Fortysix[8];
                        break;
                    default:
                        break;
                }
                string uu = u1 + "";
                if (uu.Length > 4)
                {
                    uu = uu.Substring(0, uu.IndexOf('.') + 3);
                }
                ItemInfo.UScale.Add(double.Parse(uu));
            }
            return ItemInfo;
        }

        UserKnowledgeInfo ICedts_HomeRepository.UserKnowledgeInfo(string UserName)
        {
            UserKnowledgeInfo UserKpInfo = new UserKnowledgeInfo();
            UserKpInfo.CorrectRate = new List<double>();
            UserKpInfo.KPMaster = new List<double>();
            UserKpInfo.KpName = new List<string>();
            UserKpInfo.Num = new List<int>();
            UserKpInfo.Time = new List<int>();
            var UserID = (from m in db.CEDTS_User where m.UserAccount == UserName select m.UserID).FirstOrDefault();
            var KpInfo = from m in db.CEDTS_UserKnowledgeInfomation
                         where m.UserID == UserID
                         from n in db.CEDTS_KnowledgePoints
                         where m.KnowledgePointID == n.KnowledgePointID
                         orderby m.KP_MasterRate
                         select new
                         {
                             KpName = n.Title,
                             KPMaster = m.KP_MasterRate,
                             CorrectRate = m.CorrectRate.Value,
                             Num = m.TotalCount,
                             Time = m.AverageTime
                         };

            foreach (var s in KpInfo)
            {
                Regex regex = new Regex(@"[.\d]");
                string KpName = regex.Replace(s.KpName, "");
                UserKpInfo.KpName.Add(KpName);

                string KpMaster = s.KPMaster * 100 + "";
                if (KpMaster.Length > 4)
                {
                    KpMaster = KpMaster.Substring(0, KpMaster.IndexOf('.') + 1);
                }
                UserKpInfo.KPMaster.Add(double.Parse(KpMaster));

                string CorrectRate = s.CorrectRate * 100 + "";
                if (CorrectRate.Length > 4)
                {
                    CorrectRate = CorrectRate.Substring(0, CorrectRate.IndexOf('.') + 1);
                }
                UserKpInfo.CorrectRate.Add(double.Parse(CorrectRate));

                UserKpInfo.Num.Add(s.Num.Value);
                UserKpInfo.Time.Add(Convert.ToInt32(s.Time));
            }
            return UserKpInfo;
        }

        ItemInfo ICedts_HomeRepository.UserItemList(string UserName)
        {
            ItemInfo Item = new ItemInfo();
            Item.CorrectRate = new List<double>();
            Item.ItemName = new List<string>();
            Item.Num = new List<int>();
            var UserID = (from m in db.CEDTS_User where m.UserAccount == UserName select m.UserID).FirstOrDefault();
            var Info = from m in db.CEDTS_UserAnswerInfo
                       where m.UserID == UserID
                       from n in db.CEDTS_ItemType
                       where m.ItemTypeID == n.ItemTypeID
                       orderby m.CorrectRate
                       select new
                       {
                           Name = n.TypeName_CN,
                           CorrecteRate = m.CorrectRate,
                           Num = m.TotalCount
                       };
            foreach (var s in Info)
            {
                Item.ItemName.Add(s.Name);
                string CorrectRate = s.CorrecteRate * 100 + "";

                if (CorrectRate.Length > 4)
                {
                    CorrectRate = CorrectRate.Substring(0, CorrectRate.IndexOf('.') + 1);
                }
                Item.CorrectRate.Add(double.Parse(CorrectRate));
                Item.Num.Add(s.Num.Value);
            }
            return Item;
        }

        #region 首页展示
        Front ICedts_HomeRepository.FrontInfo()
        {
            Front FrontInfo = new Front();
            FrontInfo.CoreFeatures = new List<string>();
            FrontInfo.SayContent = string.Empty;
            var SayInfo = (from m in db.CEDTS_Saying orderby m.Time descending select m).FirstOrDefault();
            var SystemOverviewInfo = (from m in db.CEDTS_SystemOverview orderby m.Time descending select m).FirstOrDefault();
            var CoreFeaturesInfo = (from m in db.CEDTS_CoreFeatures orderby m.Order select m);
            if (CoreFeaturesInfo != null)
            {
                foreach (var CoreFeatures in CoreFeaturesInfo)
                {
                    FrontInfo.CoreFeatures.Add(CoreFeatures.Intro);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    FrontInfo.CoreFeatures.Add("");
                }
            }
            if (SayInfo != null)
            {
                FrontInfo.SayContent = SayInfo.Content;
                FrontInfo.SayNote = SayInfo.Note;
            }
            else
            {
                FrontInfo.SayContent = "";
                FrontInfo.SayNote = "";
            }
            if (SystemOverviewInfo != null)
            {
                FrontInfo.SystemOverview = SystemOverviewInfo.Intro;
            }
            else
            {
                FrontInfo.SystemOverview = "";
            }
            return FrontInfo;
        }
        #endregion

        #region 联系我们
        ContactUsInfo ICedts_HomeRepository.ContactUsInfo()
        {
            ContactUsInfo ContactUsInfo = new ContactUsInfo();
            var info = (from m in db.CEDTS_Contact orderby m.ContactID descending select m).FirstOrDefault();
            if (info != null)
            {
                ContactUsInfo.Name = info.Name;
                ContactUsInfo.Tel = info.Tel;
                ContactUsInfo.ZipCode = info.ZipCode;
                ContactUsInfo.Address = info.Address;
                ContactUsInfo.Url = info.Url;
            }
            else
            {
                ContactUsInfo.Name = "";
                ContactUsInfo.Tel = "";
                ContactUsInfo.ZipCode = "";
                ContactUsInfo.Address = "";
                ContactUsInfo.Url = "";
            }
            return ContactUsInfo;
        }
        #endregion

        #region 特色功能
        List<CEDTS_CoreFeatures> ICedts_HomeRepository.CoreFeatures()
        {
            var FeaturesInfo = (from m in db.CEDTS_CoreFeatures orderby m.Order ascending select m).ToList();

            return FeaturesInfo;
        }
        #endregion

        #region 意见反馈
        void ICedts_HomeRepository.Feedback(CEDTS_Feedback feed)
        {
            CEDTS_Feedback feedinfo = new CEDTS_Feedback();
            feedinfo.Title = feed.Title;
            feedinfo.Content = feed.Content;
            feedinfo.Tel = feed.Tel;
            feedinfo.Time = feed.Time;
            db.AddToCEDTS_Feedback(feedinfo);
            db.SaveChanges();
        }
        #endregion

        #region 用户动态信息
        List<CEDTS_Testing> ICedts_HomeRepository.GetTestingInfo()
        {
            var testingList = db.CEDTS_Testing.OrderByDescending(p => p.Time).ToList();
            if (testingList.Count > 5)
            {
                testingList.RemoveRange(5, testingList.Count);
            }
            return testingList;
        }
        #endregion

        #region 获取系统信息
        string ICedts_HomeRepository.GetSys()
        {
            var sysinfo = (from m in db.CEDTS_SystemOverview orderby m.Time descending select m).FirstOrDefault();
            if (sysinfo != null)
            {
                return sysinfo.Content;
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region 获取使用说明信息
        string ICedts_HomeRepository.GetInstructions()
        {
            var Info = (from m in db.CEDTS_Instructions orderby m.InstructionsID descending select m).FirstOrDefault();
            if (Info != null)
            {
                return Info.Content;
            }
            else
            {
                return "";
            }
        }
        #endregion
    }
}