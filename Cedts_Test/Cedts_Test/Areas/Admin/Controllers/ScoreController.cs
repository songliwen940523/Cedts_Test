using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;
using System.Text.RegularExpressions;

namespace Cedts_Test.Areas.Admin.Controllers
{
    public class ScoreController : Controller
    {
        //
        // GET: /Admin/Score/
        ICedts_UserRepository _user;
        ICedts_PaperRepository _paper;
        static public string BeginTime;
        static public string EndTime;
        static public Guid ClassID;
        static public string ClassName;
        static public string ActionName;
        static public Guid SinglePaperID;
        static public List<Guid> MultiplePaperID = new List<Guid>();
        public ScoreController(ICedts_UserRepository r, ICedts_PaperRepository pa)
        {
            _user = r;
            _paper = pa;
        }

        public ActionResult Index()
        {
            List<SelectListItem> ClassIDList = new List<SelectListItem>();
            int UserID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            List<CEDTS_Class> ClassList = _user.GetClassbyUserID(UserID);
            for (int i = 0; i < ClassList.Count; i++)
            {
                ClassIDList.Add(new SelectListItem { Text = ClassList[i].ClassName, Value = ClassList[i].ClassID.ToString() });
            }
            ClassIDList[0].Selected = true;
            ViewData["ClassIDList"] = ClassIDList;
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            BeginTime = form["CalenderBegin"];
            EndTime = form["CalenderEnd"];
            ClassID = Guid.Parse(form["ClassIDList"]);
            ClassName = _user.GetClassbyID(ClassID).ClassName;
            switch (form["FirstLevel"])
            {
                case "所有练习":
                    ActionName = "AllExercise";
                    break;
                case "布置练习":
                    ActionName = "AssignedExercise";
                    break;
                case "自主练习":
                    ActionName = "SelfExercise";
                    break;
                default:
                    ActionName = "Index";
                    break;
            }
            return RedirectToAction(ActionName);
        }

        public ActionResult AllExercise(int? id)
        {
            List<ScoreInfo> scorelist = new List<ScoreInfo>();
            int UserID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            List<Guid?> paclist = _paper.SelectPaperByPeriod(BeginTime, EndTime, UserID, ClassID);
            List<CEDTS_User> userlist = _user.SelectUserByClassID(ClassID);
            foreach (var user in userlist)
            {
                ScoreInfo eachscore = new ScoreInfo();
                eachscore.UserID = user.UserID;
                eachscore.StudNum = user.StudentNumber;
                eachscore.StudName = user.UserNickname;
                eachscore.TestNames = new List<string>();
                eachscore.TestScore = new List<double>();
                List<CEDTS_Test> usertest = _paper.SelectTestByPeriod(BeginTime, EndTime, user.UserID);
                foreach (var pac in paclist)
                {
                    CEDTS_Paper paper = _paper.GetPaperByID(Guid.Parse(pac.ToString()));
                    eachscore.TestNames.Add(paper.Title);
                    CEDTS_Test test = _paper.GetTestByPaperID(Guid.Parse(pac.ToString()), user.UserID);
                    if (test != null && test.IsFinished == true)
                        eachscore.TestScore.Add(double.Parse(test.TotalScore.ToString()) / double.Parse(paper.Score.ToString()) * 100);
                    else
                        eachscore.TestScore.Add(0);
                }
                
                int donenum = 0;
                double userscore = 0.0;
                double totalscore = 0.0;
                foreach (var test in usertest)
                {
                    if (!paclist.Contains(test.PaperID))
                    {
                        List<CEDTS_PaperAssessment> assessment = _paper.SelectPaperAssessmentItem(test.PaperID);
                        foreach (var ass in assessment)
                        {
                            donenum += _paper.SelectQNByAssessmentID(Guid.Parse(ass.AssessmentItemID.ToString()));
                        }
                        userscore += double.Parse(test.TotalScore.ToString());
                        totalscore += double.Parse(_paper.SelectPaper(test.PaperID).Score.ToString());
                    }
                }
                eachscore.DoneNum = donenum;
                if (totalscore == 0)
                    eachscore.DoneScore = 0.0;
                else
                    eachscore.DoneScore = userscore / totalscore * 100;
                double avescore = 0.0;
                foreach (var score in eachscore.TestScore)
                {
                    avescore += score;
                }
                eachscore.AveScore = (avescore + eachscore.DoneScore) / (eachscore.TestScore.Count + 1);
                scorelist.Add(eachscore);
            }
            ViewData["StartDate"] = BeginTime;
            ViewData["EndDate"] = EndTime;
            ViewData["ClassName"] = ClassName;
            return View(_paper.ScoreInfoPaged(id, scorelist));
        }

        public ActionResult Detail(int id)
        {
            ViewData["UserID"] = id;
            return View();
        }

        public ActionResult UserScore(int UserID)
        {
            List<CEDTS_Test> usertest = _paper.SelectTestByPeriod(BeginTime, EndTime, UserID);
            StatisticsScore Ss = new StatisticsScore();
            Ss.Score = new List<double>();
            Ss.TypeName = new List<string>();
            foreach (var s in usertest)
            {
                CEDTS_Paper paper = _paper.SelectPaper(s.PaperID);
                if (paper.State != 9 && paper.State != 10)
                {
                    Ss.Score.Add(s.TotalScore.Value / paper.Score.Value * 100);
                    Ss.TypeName.Add(paper.Title);
                }
                
            }
            return Json(Ss);
        }

        public ActionResult UserKPInfo(int UserID)
        {
            List<Knowledge> KnowList = new List<Knowledge>();
            List<CEDTS_KnowledgePoints> KnowPointList = _paper.GetAllKnowledges();
            foreach (var kp in KnowPointList)
            {
                string Name = string.Empty;
                Knowledge kown = new Knowledge();
                Regex regex = new Regex(@"[.\d]");
                Name = regex.Replace(kp.Title, "");
                List<CEDTS_TestAnswerKnowledgePoint> UMiInfo = _paper.SelectTAKByPeriod(BeginTime, EndTime, UserID, kp.KnowledgePointID);
                List<CEDTS_TestAnswerKnowledgePoint> SMiInfo = _paper.SelectATAKByPeriod(BeginTime, EndTime, kp.KnowledgePointID);
                if (UMiInfo.Count != 0)
                {
                    double SMi1 = 0;
                    double UMi1 = 0;
                    foreach (var tak in UMiInfo)
                    {
                        UMi1 += tak.KP_MasterRate.Value;
                    }
                    foreach (var tak in SMiInfo)
                    {
                        SMi1 += tak.KP_MasterRate.Value;
                    }                
                    SMi1 = SMi1 / SMiInfo.Count * 100;
                    UMi1 = UMi1 / UMiInfo.Count * 100;
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
            return Json(KnowList);
        }

        public ActionResult UserItemInfo(int UserID)
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

            int ItemTypeNum = _paper.SelectItemType().Count;
            for (int i = 1; i <= ItemTypeNum; i++)
            {
                List<CEDTS_TestAnswerTypeInfo> UItem = _paper.SelectTATByPeriod(BeginTime, EndTime, UserID, i);
                List<CEDTS_TestAnswerTypeInfo> SItem = _paper.SelectATATByPeriod(BeginTime, EndTime, i);
                if (UItem.Count != 0)
                {
                    double u1 = 0.0;
                    double s1 = 0.0;
                    foreach (var item in UItem)
                    {
                        u1 += item.CorrectRate.Value;
                    }
                    foreach (var item in SItem)
                    {
                        s1 += item.CorrectRate.Value;
                    }
                    u1 = u1 / UItem.Count;
                    s1 = s1 / SItem.Count;
                    switch (i)
                    {
                        case 1:
                            s1 = s1 * 1 * ItemInfo.Fortysix[0];
                            u1 = u1 * 1 * ItemInfo.Fortysix[0];
                            break;
                        case 2:
                            s1 = s1 * 1 * ItemInfo.Fortysix[1];
                            u1 = u1 * 1 * ItemInfo.Fortysix[1];
                            break;
                        case 3:
                            s1 = s1 * 1 * ItemInfo.Fortysix[2];
                            u1 = u1 * 1 * ItemInfo.Fortysix[2];
                            break;
                        case 4:
                            s1 = s1 * 1 * ItemInfo.Fortysix[3];
                            u1 = u1 * 1 * ItemInfo.Fortysix[3];
                            break;
                        case 5:
                            s1 = s1 * 1 * ItemInfo.Fortysix[4];
                            u1 = u1 * 1 * ItemInfo.Fortysix[4];
                            break;
                        case 6:
                            s1 = s1 * 1 * ItemInfo.Fortysix[5];
                            u1 = u1 * 1 * ItemInfo.Fortysix[5];
                            break;
                        case 7:
                            s1 = s1 * 1.5 * ItemInfo.Fortysix[6];
                            u1 = u1 * 1 * ItemInfo.Fortysix[6];
                            break;
                        case 8:
                            s1 = s1 * 0.5 * ItemInfo.Fortysix[8];
                            u1 = u1 * 1 * ItemInfo.Fortysix[8];
                            break;
                        case 9:
                            s1 = s1 * 1 * ItemInfo.Fortysix[7];
                            u1 = u1 * 1 * ItemInfo.Fortysix[7];
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
                    string uu = u1 + "";
                    if (uu.Length > 4)
                    {
                        uu = uu.Substring(0, uu.IndexOf('.') + 3);
                    }
                    ItemInfo.UScale.Add(double.Parse(uu));
                    
                }
                else
                {
                    ItemInfo.SScale.Add(0.00);
                    ItemInfo.UScale.Add(0.00);
                }
                
            }
            double temp1 = ItemInfo.SScale[7];
            ItemInfo.SScale[7] = ItemInfo.SScale[8];
            ItemInfo.SScale[8] = temp1;
            double temp2 = ItemInfo.UScale[7];
            ItemInfo.UScale[7] = ItemInfo.UScale[8];
            ItemInfo.UScale[8] = temp2;
            return Json(ItemInfo);
        }

        public ActionResult AssignedExercise(int? id)
        {
            List<ScoreInfo> scorelist = new List<ScoreInfo>();
            int UserID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            List<Guid?> paclist = _paper.SelectPaperByPeriod(BeginTime, EndTime, UserID, ClassID);
            List<CEDTS_User> userlist = _user.SelectUserByClassID(ClassID);
            foreach (var user in userlist)
            {
                ScoreInfo eachscore = new ScoreInfo();
                eachscore.UserID = user.UserID;
                eachscore.StudNum = user.StudentNumber;
                eachscore.StudName = user.UserNickname;
                eachscore.TestNames = new List<string>();
                eachscore.TestScore = new List<double>();
                List<CEDTS_Test> usertest = _paper.SelectTestByPeriod(BeginTime, EndTime, user.UserID);
                foreach (var pac in paclist)
                {
                    CEDTS_Paper paper = _paper.GetPaperByID(Guid.Parse(pac.ToString()));
                    eachscore.TestNames.Add(paper.Title);
                    CEDTS_Test test = _paper.GetTestByPaperID(Guid.Parse(pac.ToString()), user.UserID);
                    if (test != null && test.IsFinished == true )
                        eachscore.TestScore.Add(double.Parse(test.TotalScore.ToString()) / double.Parse(paper.Score.ToString()) * 100);
                    else
                        eachscore.TestScore.Add(0);
                }
                double totalscore = 0.0;
                foreach (var score in eachscore.TestScore)
                {
                    totalscore += score;
                }
                eachscore.AveScore = totalscore / eachscore.TestScore.Count;
                scorelist.Add(eachscore);
                
            }
            ViewData["StartDate"] = BeginTime;
            ViewData["EndDate"] = EndTime;
            ViewData["ClassName"] = ClassName;
            return View(_paper.ScoreInfoPaged(id, scorelist));
        }

        public ActionResult SelfExercise(int? id)
        {
            List<ScoreInfo> scorelist = new List<ScoreInfo>();
            int UserID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            List<Guid?> paclist = _paper.SelectPaperByPeriod(BeginTime, EndTime, UserID, ClassID);
            List<CEDTS_User> userlist = _user.SelectUserByClassID(ClassID);
            foreach (var user in userlist)
            {
                ScoreInfo eachscore = new ScoreInfo();
                eachscore.UserID = user.UserID;
                eachscore.StudNum = user.StudentNumber;
                eachscore.StudName = user.UserNickname;
                eachscore.TestNames = new List<string>();
                eachscore.TestScore = new List<double>();
                List<CEDTS_Test> usertest = _paper.SelectTestByPeriod(BeginTime, EndTime, user.UserID);
                
                int donenum = 0;
                int correctnum = 0;
                foreach (var test in usertest)
                {
                    if (!paclist.Contains(test.PaperID))
                    {
                        donenum += _paper.SelectQNByTestID(test.TestID);
                        correctnum += _paper.SelectCQNByTestID(test.TestID);
                    }
                }
                eachscore.DoneNum = donenum;
                eachscore.CorrectNum = correctnum;
                if (donenum != 0)
                    eachscore.DoneScore = correctnum * 1.0 / donenum * 100;
                else
                    eachscore.DoneScore = 0.0;
                scorelist.Add(eachscore);
            }
            ViewData["StartDate"] = BeginTime;
            ViewData["EndDate"] = EndTime;
            ViewData["ClassName"] = ClassName;
            return View(_paper.ScoreInfoPaged(id, scorelist));
        }

        public ActionResult Single()
        {
            List<SelectListItem> ClassIDList = new List<SelectListItem>();
            int UserID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            List<CEDTS_Class> ClassList = _user.GetClassbyUserID(UserID);
            for (int i = 0; i < ClassList.Count; i++)
            {
                ClassIDList.Add(new SelectListItem { Text = ClassList[i].ClassName, Value = ClassList[i].ClassID.ToString() });
            }
            ClassIDList[0].Selected = true;
            ViewData["ClassIDList"] = ClassIDList;
            return View();
        }
        [HttpPost]
        public ActionResult Single(FormCollection form)
        {
            BeginTime = form["CalenderBegin"];
            EndTime = form["CalenderEnd"];
            ClassID = Guid.Parse(form["ClassIDList"]);
            ClassName = _user.GetClassbyID(ClassID).ClassName;
            switch (form["FirstLevel"])
            {
                case "单次练习":
                    ActionName = "SingleExercise";
                    break;
                case "多次练习":
                    ActionName = "MultipleExercise";
                    break;
                default:
                    ActionName = "Single";
                    break;
            }
            return RedirectToAction(ActionName);
        }

        public ActionResult SingleExercise()
        {
            int UserID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            List<Guid?> paclist = _paper.SelectPaperByPeriod(BeginTime, EndTime, UserID, ClassID);
            List<SelectListItem> PaperNameList = new List<SelectListItem>();
            foreach (var pac in paclist)
            {
                CEDTS_Paper paper = _paper.GetPaperByID(Guid.Parse(pac.ToString()));
                PaperNameList.Add(new SelectListItem { Text = paper.Title, Value = paper.PaperID.ToString() });
            }
            ViewData["PaperNameList"] = PaperNameList;
            return View();
        }
        [HttpPost]
        public ActionResult SingleExercise(FormCollection form)
        {
            SinglePaperID = Guid.Parse(form["PaperNameList"]);
            switch (form["SelectWay"])
            {
                case "学生成绩":
                    ActionName = "SingleExerciseScore";
                    break;
                case "试卷分析":
                    ActionName = "SingleExercisePaper";
                    break;
                default:
                    ActionName = "SingleExercise";
                    break;
            }
            return RedirectToAction(ActionName);
        }
        public ActionResult SingleExerciseScore()
        {
            List<SingleScoreInfo> SingleScoreInfoList = GetSingleScoreList(ClassID, SinglePaperID);
            SingleStatistics SingleStatisticsInfo = new SingleStatistics();
            ClassName = _user.GetClassbyID(ClassID).ClassName;
            CEDTS_Paper paper = _paper.GetPaperByID(SinglePaperID);
            ViewData["TaskName"] = paper.Title;
            ViewData["ClassName"] = ClassName;            
            if (SingleScoreInfoList.Count == 0)
            {
                ViewData["Empty"] = "Empty";
            }
            else
            {
                ViewData["Empty"] = "";
                bool doneflag = false;
                foreach (var singlescore in SingleScoreInfoList)
                {
                    if (singlescore.Done)
                    {
                        doneflag = true;
                        break;
                    }
                }
                if (doneflag)
                {
                    int donum = 0;
                    int undonum = SingleScoreInfoList.Count;
                    double totalstudscore = 0.0;
                    double highscore = double.MinValue;
                    double lowscore = double.MaxValue;
                    string highname = "";
                    string lowname = "";
                    foreach (var singlescore in SingleScoreInfoList)
                    {
                        if (singlescore.Done)
                        {
                            donum++;
                            undonum--;
                            totalstudscore += singlescore.Score;
                            if (singlescore.Score <= lowscore)
                            {                                
                                if (singlescore.Score == lowscore)
                                {
                                    lowname = lowname + "、" + singlescore.StudName;
                                }
                                else
                                {
                                    lowscore = singlescore.Score;
                                    lowname = singlescore.StudName;
                                }
                            }
                            if (singlescore.Score >= highscore)
                            {
                                if (singlescore.Score == highscore)
                                {
                                    highname = highname + "、" + singlescore.StudName;
                                }
                                else
                                {
                                    highscore = singlescore.Score;
                                    highname = singlescore.StudName;
                                }
                            }                           
                        }
                    }
                    SingleStatisticsInfo.AveScore = totalstudscore / donum;
                    SingleStatisticsInfo.AvePercent = SingleStatisticsInfo.AveScore / paper.Score.Value * 100;
                    SingleStatisticsInfo.DoNum = donum;
                    SingleStatisticsInfo.UndoNum = undonum;
                    SingleStatisticsInfo.HighestScore = highscore;
                    SingleStatisticsInfo.HighestStudName = highname;
                    SingleStatisticsInfo.LowestScore = lowscore;
                    SingleStatisticsInfo.LowestStudName = lowname;
                    double temp = 0.0;
                    foreach (var singlescore in SingleScoreInfoList)
                    {
                        if (singlescore.Done)
                        {
                            temp += Math.Pow((singlescore.Score - SingleStatisticsInfo.AveScore), 2);
                        }
                    }
                    SingleStatisticsInfo.Variance = temp / donum;
                    SingleStatisticsInfo.StandardDeviation = Math.Sqrt(SingleStatisticsInfo.Variance);
                }
                else
                {
                    SingleStatisticsInfo.DoNum = 0;
                    SingleStatisticsInfo.UndoNum = SingleScoreInfoList.Count;
                }
            }
            return View(SingleStatisticsInfo);
        }

        public ActionResult SingleScoreList(int? id)
        {
            if (id == null)
            {
                if (TempData["id"] != null)
                {
                    id = int.Parse(TempData["id"].ToString());
                    TempData["id"] = id;
                }
                return View(_paper.SingleScoreInfoPaged(id, GetSingleScoreList(ClassID, SinglePaperID)));
            }
            else
            {
                TempData["id"] = id;
                return RedirectToAction("SingleExerciseScore");
            }            
        }

        public List<SingleScoreInfo> GetSingleScoreList(Guid ClassIDNum, Guid PaperIDNum)
        {
            List<SingleScoreInfo> SingleScoreInfoList = new List<SingleScoreInfo>();
            List<CEDTS_User> userlist = _user.SelectUserByClassID(ClassIDNum);
            CEDTS_Paper paper = _paper.GetPaperByID(PaperIDNum);
            double totalscore = paper.Score.Value;
            foreach (var user in userlist)
            {
                SingleScoreInfo eachsinglescore = new SingleScoreInfo();
                eachsinglescore.UserID = user.UserID;
                eachsinglescore.StudNum = user.StudentNumber;
                eachsinglescore.StudName = user.UserNickname;
                eachsinglescore.TotalScore = totalscore;
                CEDTS_Test test = _paper.GetTestByPaperID(SinglePaperID, user.UserID);
                if (test == null || test.IsFinished == false)
                {
                    eachsinglescore.Done = false;
                    eachsinglescore.Score = 0;
                    eachsinglescore.Percent = 0;
                }
                else
                {
                    eachsinglescore.Done = true;
                    eachsinglescore.Score = test.TotalScore.Value;
                    eachsinglescore.Percent = test.TotalScore.Value / totalscore * 100;
                }
                SingleScoreInfoList.Add(eachsinglescore);
            }
            return SingleScoreInfoList;
        }

        public ActionResult SingleTest(int id)
        {
            CEDTS_Paper paper = _paper.GetPaperByID(SinglePaperID);
            ViewData["UserID"] = id;
            ViewData["PaperID"] = SinglePaperID;
            ViewData["PaperName"] = paper.Title;
            ViewData["PaperState"] = paper.State;
            CEDTS_Test test = _paper.GetTestByPaperID(SinglePaperID, id);
            if (test == null || test.IsFinished == false)
            {
                ViewData["TestID"] = "";
            }
            else
            {
                ViewData["TestID"] = test.TestID;
            }
            return View();
        }
        public ActionResult Suggestions(int UserID, Guid PaperID)
        {
            CEDTS_Test test = _paper.GetTestByPaperID(PaperID, UserID);
            string Content = String.Empty;
            if (test == null || test.IsFinished == false)
            {
                Content = "该生暂未练习。";
            }
            else
            {
                double score = test.TotalScore.Value;
                double paperscore = _paper.GetPaperByID(PaperID).Score.Value;
                string face = string.Empty;
                double ScoreRate = score / paperscore;
                if (ScoreRate >= 0.9)
                {
                    face = "优秀<img src='../../../../Images/1.1.gif'></img>";
                }
                if (ScoreRate < 0.9 && ScoreRate >= 0.8)
                {
                    face = "良好<img src='../../../../Images/1.2.gif'></img>";
                }
                if (ScoreRate < 0.8 && ScoreRate >= 0.7)
                {
                    face = "一般<img src='../../../../Images/1.3.gif'></img>";
                }
                if (ScoreRate < 0.7 && ScoreRate >= 0.6)
                {
                    face = "及格<img src='../../../../Images/1.4.gif'></img>";
                }
                if (ScoreRate < 0.6 && ScoreRate >= 0.5)
                {
                    face = "不及格<img src='../../../../Images/1.5.gif'></img>";
                }
                if (ScoreRate < 0.5)
                {
                    face = "很差<img src='../../../../Images/1.6.gif'></img>";
                }
                Content = "<b>试卷总分：</b>" + paperscore + "分, <b>实际得分：</b>" + score + "分," + face + "。";
            }
            return Json(Content);
        }

        public ActionResult Item(int UserID, Guid TestID)
        {
            return Json(_paper.UserItemInfo(UserID, TestID));
        }
        public ActionResult ItemList(int UserID, Guid TestID)
        {
            return Json(_paper.ItemList(UserID, TestID));
        }
        public ActionResult Knowledge(int UserID, Guid TestID)
        {
            return Json(_paper.UserKpMaster(UserID, TestID));
        }
        #region 知识点详情列表
        public ActionResult KpList(int UserID, Guid TestID)
        {
            return Json(_paper.KpList(UserID, TestID));
        }
        #endregion

        public ActionResult SingleExercisePaper()
        {
            ClassName = _user.GetClassbyID(ClassID).ClassName;
            CEDTS_Paper paper = _paper.GetPaperByID(SinglePaperID);
            ViewData["TaskName"] = paper.Title;
            ViewData["ClassName"] = ClassName;
            List<CEDTS_User> StudList = _user.SelectUserByClassID(ClassID);
            if (StudList.Count == 0)
            {
                ViewData["Empty"] = "Empty";
            }
            else
            {
                ViewData["Empty"] = "";
            }
            return View();
        }
        public ActionResult SingleItemList()
        {
            List<WrongItemInfo> WrongItemList = new List<WrongItemInfo>();
            List<CEDTS_User> StudList = _user.SelectUserByClassID(ClassID);
            List<Guid> TestIDList = _paper.SelectTestIDListByPaperID(SinglePaperID);
            List<int> StudIDList = new List<int>();
            foreach (var stud in StudList)
            {
                StudIDList.Add(stud.UserID);
            }
            WrongItemList = _paper.SelectWIIByPUList(SinglePaperID, StudIDList);
            for (int i = 0; i < WrongItemList.Count; i++ )
            {
                WrongItemList[i].ItemName = _paper.SelectItemType(WrongItemList[i].ItemTypeNum).TypeName_CN;
                List<QuestionDoneInfo> Qlist = _paper.SelectQDIByTUList(TestIDList, StudIDList, WrongItemList[i].ItemTypeNum);
                int Num = Qlist.Count < 3 ? Qlist.Count : 3;
                WrongItemList[i].WrongNum = new List<int>();
                for (int j = 0; j < Num; j++ )
                {
                    WrongItemList[i].WrongNum.Add(Qlist[j].QuestionNum);
                }
            }
            return PartialView(WrongItemList);
        }
        public ActionResult SingleKnowledgeList()
        {
            List<WrongKnowledgeInfo> WrongKnowledgeList = new List<WrongKnowledgeInfo>();
            if (_paper.SelectPaper(SinglePaperID).State == 8)
            {
                List<CEDTS_User> StudList = _user.SelectUserByClassID(ClassID);
                List<int> StudIDList = new List<int>();
                foreach (var stud in StudList)
                {
                    StudIDList.Add(stud.UserID);
                }
                WrongKnowledgeList = _paper.SelectWKIByPUList(SinglePaperID, StudIDList);
                for (int i = 0; i < WrongKnowledgeList.Count; i++)
                {
                    WrongKnowledgeList[i].KnowledgeName = _paper.SelectKnowledgeName(WrongKnowledgeList[i].KnowledgeID);
                }
            }
            return PartialView(WrongKnowledgeList);
        }

        public ActionResult MultipleExercise()
        {
            int UserID = _user.SelectUserByAccout(User.Identity.Name).UserID;
            List<Guid?> paclist = _paper.SelectPaperByPeriod(BeginTime, EndTime, UserID, ClassID);
            List<SelectListItem> PaperNameList = new List<SelectListItem>();
            foreach (var pac in paclist)
            {
                CEDTS_Paper paper = _paper.GetPaperByID(Guid.Parse(pac.ToString()));
                PaperNameList.Add(new SelectListItem { Text = paper.Title, Value = paper.PaperID.ToString() });
            }
            ViewData["PaperNameList"] = PaperNameList;
            return View(PaperNameList);
        }
        [HttpPost]
        public ActionResult MultipleExercise(FormCollection form)
        {
            MultiplePaperID.Clear();
            string names = form["PaperName"];
            string[] PaperID = names.Split(',');
            string PaperNames = String.Empty;
            foreach (var id in PaperID)
            {
                MultiplePaperID.Add(Guid.Parse(id));
            }
            return RedirectToAction("MultipleExercisePaper");
        }

        public ActionResult MultipleExercisePaper()
        {
            ClassName = _user.GetClassbyID(ClassID).ClassName;
            ViewData["ClassName"] = ClassName;
            string PaperName = String.Empty;
            int PaperCount = MultiplePaperID.Count > 2 ? 3 : MultiplePaperID.Count;
            for (int i = 0; i < PaperCount; i++ )
            {
                PaperName = PaperName + _paper.SelectPaper(MultiplePaperID[i]).Title + ",";
            }            
            PaperName = PaperName.Substring(0, PaperName.Length - 1);
            if (PaperCount > 3)
            {
                PaperName = PaperName + "等" + PaperCount + "次练习";
            }
            ViewData["PaperName"] = PaperName;
            List<CEDTS_User> StudList = _user.SelectUserByClassID(ClassID);
            if (StudList.Count == 0)
            {
                ViewData["Empty"] = "Empty";
            }
            else
            {
                ViewData["Empty"] = "";
            }
            return View();
        }

        public ActionResult MultipleItemList()
        {
            List<CEDTS_User> StudList = _user.SelectUserByClassID(ClassID);
            List<int> StudIDList = new List<int>();
            foreach (var stud in StudList)
            {
                StudIDList.Add(stud.UserID);
            }
            List<WrongItemInfo> WrongItemList = _paper.SelectWIIByPUList(MultiplePaperID,StudIDList);                       
            return PartialView(WrongItemList);
        }

        public ActionResult MultipleKnowledgeList()
        {
            List<CEDTS_User> StudList = _user.SelectUserByClassID(ClassID);
            List<int> StudIDList = new List<int>();
            List<Guid> MultiplePaperIDForKnow = new List<Guid>();
            foreach (var item in MultiplePaperID)
            {
                MultiplePaperIDForKnow.Add(item);
            }
            List<WrongKnowledgeInfo> WrongKnowledgeList = new List<WrongKnowledgeInfo>();
            foreach (var stud in StudList)
            {
                StudIDList.Add(stud.UserID);
            }
            foreach (var paperid in MultiplePaperID)
            {
                if (_paper.SelectPaper(paperid).State != 8)
                    MultiplePaperIDForKnow.Remove(paperid);
            }
            if (MultiplePaperIDForKnow.Count != 0)
            {
                WrongKnowledgeList = _paper.SelectWKIByPUList(MultiplePaperIDForKnow, StudIDList);
                for (int i = 0; i < WrongKnowledgeList.Count; i++)
                {
                    WrongKnowledgeList[i].KnowledgeName = _paper.SelectKnowledgeName(WrongKnowledgeList[i].KnowledgeID);
                }
            }
            return PartialView(WrongKnowledgeList);
        }
    }
}
