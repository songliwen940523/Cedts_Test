using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;
using System.Xml.Linq;
using System.Transactions;
using System.Xml;
using System.IO;

namespace Cedts_Test.Areas.Admin.Controllers
{
    [Authorize]
    public class PaperController : Controller
    {
        #region 静态变量定义
        public static CEDTS_Paper _tempPaper = new CEDTS_Paper();//临时存储用户对试卷基本属性的设置
        public static List<int> listNumber = new List<int>();//临时存储用户对试卷试题数量的设置
        public static List<int> listTemp = new List<int>();//存放加载暂存试卷后的各类题型的题数
        public static List<SkimmingScanningPartCompletion> sspcList = new List<SkimmingScanningPartCompletion>();//临时存储快速阅读题型
        public static List<Listen> slpoList = new List<Listen>();//临时存储短对话听力题型
        public static List<Listen> llpoList = new List<Listen>();//临时存储长对话听力题型
        public static List<Listen> rlpoList = new List<Listen>();//临时存储听力理解题型
        public static List<Listen> lpcList = new List<Listen>();//临时存储复合型听力题型
        public static List<ReadingPartCompletion> rpcList = new List<ReadingPartCompletion>();//临时存储阅读理解选词填空题型
        public static List<ReadingPartOption> rpoList = new List<ReadingPartOption>();//临时存储阅读理解选择题型
        public static List<InfoMatchingCompletion> infoMatList = new List<InfoMatchingCompletion>();//临时存储信息匹配题型
        public static List<ClozePart> cpList = new List<ClozePart>();//临时存储完型填空题型
        public static CEDTS_PaperExpansion _paperExansion = new CEDTS_PaperExpansion();//试卷暂存表
        public static int Order = 0;//试题题号
        public static int sa;//1为暂存试卷
        public static int s = 0;//继续编辑后的题型号，从0开始
        public static string SGID = "00000000-0000-0000-0000-000000000000";
        public static Guid GID;//暂存试卷ID
        public static int ss = 0;//为1表示二次编辑
        public static string PSID = string.Empty;//上传音频文件后，服务器回调的数据
        #endregion

        #region 实例化接口
        ICedts_PaperRepository _paper;
        ICedts_ItemRepository _item;
        ICedts_QuestionRepository _question;
        ICedts_QuestionKnowledgeRepository _QuestionKnowledge;
        ICedts_ExpansionRepository _Expansion;
        ICedts_TemporaryPaperRepository _Tpaper;
        ICedts_TemporaryItemRepository _titem;
        ICedts_TemporaryQuestionRepository _Tquestion;
        ICedts_TemporaryQuestionKnowledgeRepository _TQuestionKnowledge;
        ICedts_TemporaryExpansionRepository _TExpansion;
        public PaperController(ICedts_PaperRepository r, ICedts_ItemRepository i, ICedts_QuestionRepository q, ICedts_QuestionKnowledgeRepository qk, ICedts_ExpansionRepository er, ICedts_TemporaryPaperRepository Tp, ICedts_TemporaryItemRepository Ti, ICedts_TemporaryQuestionRepository Tq, ICedts_TemporaryQuestionKnowledgeRepository Tqk, ICedts_TemporaryExpansionRepository Te)
        {
            _paper = r;
            _item = i;
            _question = q;
            _QuestionKnowledge = qk;
            _Expansion = er;
            _Tpaper = Tp;
            _titem = Ti;
            _Tquestion = Tq;
            _TQuestionKnowledge = Tqk;
            _TExpansion = Te;
        }
        #endregion

        #region 试卷管理首页
        [Filter.LogFilter(Description = "查看试卷列表")]
        public ActionResult Index(int? id)
        {
            s = 0;
            int UserID = _item.SelectUserID(User.Identity.Name);
            return View(_paper.SelectPaper(id, UserID));
        }
        #endregion

        #region 添加试卷，基本属性设置
        public ActionResult Create()
        {
            #region 清空全局变量
            PSID = string.Empty;
            Order = 0;
            listNumber.Clear();
            listTemp.Clear();
            sspcList.Clear();
            slpoList.Clear();
            llpoList.Clear();
            rlpoList.Clear();
            lpcList.Clear();
            rpcList.Clear();
            rpoList.Clear();
            cpList.Clear();
            infoMatList.Clear();
            _paperExansion = new CEDTS_PaperExpansion();
            #endregion
            return View();
        }

        [HttpPost]
        public ActionResult Create(CEDTS_Paper paper)
        {
            if (_tempPaper.PaperID != null)
            {
                CEDTS_Paper _temp = new CEDTS_Paper();
                _tempPaper = _temp;
            }
            _tempPaper.PaperID = Guid.NewGuid();
            _tempPaper.Title = paper.Title;
            _tempPaper.Description = paper.Description;
            _tempPaper.Difficult = paper.Difficult;
            _tempPaper.Score = paper.Score;
            _tempPaper.Duration = paper.Duration;
            _tempPaper.Type = paper.Type;
            _tempPaper.CreateTime = DateTime.Now;
            _tempPaper.UpdateTime = _tempPaper.CreateTime;
            return RedirectToAction("SetTopic");
        }
        #endregion

        #region 添加试卷，题型数量设置
        public ActionResult SetTopic()
        {
            return View(_paper.SelectPartType());
        }
        [HttpPost]
        public ActionResult SetTopic(FormCollection form)
        {
            PSID = form["PSID"];

            ss = 0;
            if (listNumber.Count != 0)
            {
                sspcList.Clear();
                slpoList.Clear();
                llpoList.Clear();
                rlpoList.Clear();
                lpcList.Clear();
                rpcList.Clear();
                rpoList.Clear();
                infoMatList.Clear();
                cpList.Clear();
                listNumber.Clear();
            }
            if (listTemp.Count > 0)
            {
                sspcList.Clear();
                slpoList.Clear();
                llpoList.Clear();
                rlpoList.Clear();
                lpcList.Clear();
                rpcList.Clear();
                rpoList.Clear();
                infoMatList.Clear();
                cpList.Clear();
                listTemp.Clear();
            }
            List<string> list = new List<string>();
            List<CEDTS_PartType> listPart = _paper.SelectPartType();
            for (int i = 0; i < listPart.Count; i++)
            {
                List<CEDTS_ItemType> listItem = _paper.SelectItemTypeByPartTypeID(listPart[i].PartTypeID);
                for (int j = 0; j < listItem.Count; j++)
                {
                    list.Add(listPart[i].PartTypeID.ToString() + "_" + (j + 1).ToString());
                }
            }
            for (int k = 1; k <= list.Count; k++)
            {
                listNumber.Add(int.Parse(form[k]));
                listTemp.Add(int.Parse(form[k]));
            }
            if (sspcList.Count > listTemp[0])
            {
                sspcList.RemoveRange(listTemp[0], sspcList.Count - listTemp[0]);
            }
            if (slpoList.Count > listTemp[1])
            {
                slpoList.RemoveRange(listTemp[1], slpoList.Count - listTemp[1]);
            }
            if (llpoList.Count > listTemp[2])
            {
                llpoList.RemoveRange(listTemp[2], llpoList.Count - listTemp[2]);
            }
            if (rlpoList.Count > listTemp[3])
            {
                rlpoList.RemoveRange(listTemp[3], rlpoList.Count - listTemp[3]);
            }
            if (lpcList.Count > listTemp[4])
            {
                lpcList.RemoveRange(listTemp[4], lpcList.Count - listTemp[4]);
            }
            if (rpcList.Count > listTemp[5])
            {
                rpcList.RemoveRange(listTemp[5], rpcList.Count - listTemp[5]);
            }
            if (rpoList.Count > listTemp[6])
            {
                rpoList.RemoveRange(listTemp[6], rpoList.Count - listTemp[6]);
            }
            if (infoMatList.Count > listTemp[7])
            {
                infoMatList.RemoveRange(listTemp[7], infoMatList.Count - listTemp[7]);
            }
            if (cpList.Count > listTemp[8])
            {
                cpList.RemoveRange(listTemp[8], cpList.Count - listTemp[8]);
            }
            return RedirectToAction("AddItem");
        }
        /// <summary>
        /// 题型数量设置，根据PartTypeID获取ItemType信息
        /// </summary>
        /// <param name="PartTypeID">题目大类型ID</param>
        /// <returns>返回ItemType表的局部视图</returns>
        [ChildActionOnly]
        public ActionResult ItemType(string PartTypeID)
        {
            return PartialView(_paper.SelectItemTypeByPartTypeID(int.Parse(PartTypeID)));
        }
        #endregion

        #region 添加试卷，下一步时对题型展示的控制
        public ActionResult AddItem()
        {
            if (listNumber[0] != 0)
            {
                return RedirectToAction("CreateSkimmingAndScanning");
            }
            if (listNumber[1] != 0)
            {
                return RedirectToAction("CreateShortListen");
            }
            if (listNumber[2] != 0)
            {
                return RedirectToAction("CreateLongListen");
            }
            if (listNumber[3] != 0)
            {
                return RedirectToAction("CreateComprehensionListen");
            }
            if (listNumber[4] != 0)
            {
                return RedirectToAction("CreateComplexListen");
            }
            if (listNumber[5] != 0)
            {
                return RedirectToAction("CreateBankedCloze");
            }
            if (listNumber[6] != 0)
            {
                return RedirectToAction("CreateMultipleChoice");
            }
            if (listNumber[7] != 0)
            {
                return RedirectToAction("CreateInfoMatching");
            }
            if (listNumber[8] != 0)
            {
                return RedirectToAction("CreateCloze");
            }
            else
            {
                return RedirectToAction("TotalPaper");
            }
        }
        #endregion

        #region 添加试卷，快速阅读
        public ActionResult CreateSkimmingAndScanning()
        {
            int IsLast = 1; 
            if (User.Identity.Name == "admin")
            {
                ViewData["角色"] = "Admin";
            } 
            else
            {
                ViewData["角色"] = "Teacher";
            }
            Order = 1;
            if (ss == 1)
            {
                ViewData["快速阅读"] = listTemp[0];
                //listNumber = listTemp;
                for (int i = 1; i < 9; i++)
                {
                    if (listTemp[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
            }
            else
            {
                ViewData["快速阅读"] = listNumber[0];
                for (int i = 1; i < 9; i++)
                {
                    if (listNumber[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
            }
            ViewData["ss"] = ss;
            ViewData["最后一项"] = IsLast;           
            listNumber[0] = 0;
            return View();
        }
        public JsonResult GetSkimmingAndScanning()
        {
            Order = 1;
            if (sspcList.Count > 0)
            {
                return Json(sspcList);
            }
            else
                return Json("1");
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult CreateSkimmingAndScanning(FormCollection form)
        {
            sa = int.Parse(form["sa"]);
            string role = form["checkrole"];
            if (sa == 1)//暂存
            {
                _paperExansion.SkimmingAndScanningNum = listTemp[0];
                _paperExansion.ShortListenNum = listTemp[1];
                _paperExansion.LongListenNum = listTemp[2];
                _paperExansion.ComprehensionListenNum = listTemp[3];
                _paperExansion.ComplexListenNum = listTemp[4];
                _paperExansion.BankedClozeNum = listTemp[5];
                _paperExansion.MultipleChoiceNum = listTemp[6];
                _paperExansion.InfoMatchingNum = listTemp[7];
                _paperExansion.ClozeNum = listTemp[8];
                _paperExansion.State = 1;
                listNumber[1] = 0;
                listNumber[2] = 0;
                listNumber[3] = 0;
                listNumber[4] = 0;
                listNumber[5] = 0;
                listNumber[6] = 0;
                listNumber[7] = 0;
                listNumber[8] = 0;
            }
            if (sspcList.Count != 0)
            {
                sspcList.Clear();
            }
            int totalnum = int.Parse(form["hidden"]);//快速阅读的题数
            for (int i = 1; i <= totalnum; i++)
            {
                SkimmingScanningPartCompletion sspc = new SkimmingScanningPartCompletion();

                sspc.Choices = new List<string>();
                sspc.ChoiceNum = int.Parse(form["selectnum_" + i]);
                sspc.TermNum = int.Parse(form["selectnumb_" + i]);
                sspc.Content = form["textarea_" + i];

                ItemBassInfo info = new ItemBassInfo();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.DifficultQuestion = new List<double>();
                info.Problem = new List<string>();
                info.KnowledgeID = new List<string>();
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.Knowledge = new List<string>();

                for (int j = 1; j <= sspc.ChoiceNum; j++)
                {
                    sspc.Choices.Add(form["optionA" + i + "_" + j]);
                    sspc.Choices.Add(form["optionB" + i + "_" + j]);
                    sspc.Choices.Add(form["optionC" + i + "_" + j]);
                    sspc.Choices.Add(form["optionD" + i + "_" + j]);
                    info.ScoreQuestion.Add(double.Parse(form["scorequestion" + i + "_" + j]));
                    info.TimeQuestion.Add(double.Parse(form["timequestion" + i + "_" + j]));
                    info.DifficultQuestion.Add(double.Parse(form["difficultquestion" + i + "_" + j]));
                    info.Problem.Add(form["question" + i + "_" + j]);
                    if (role == "Admin")
                    {
                        info.KnowledgeID.Add(form["hidden_a" + i + "_" + j]);
                        info.Knowledge.Add(form["txt_a" + i + "_" + j]);
                    }                    
                    info.AnswerValue.Add(form["answer" + i + "_" + j]);
                    info.Tip.Add(form["tip" + i + "_" + j]);
                    info.QuestionID.Add(Guid.NewGuid());
                }
                for (int k = 1; k <= sspc.TermNum; k++)
                {
                    info.ScoreQuestion.Add(double.Parse(form["scorequestion" + i + "_" + sspc.ChoiceNum + "_" + k]));
                    info.TimeQuestion.Add(double.Parse(form["timequestion" + i + "_" + sspc.ChoiceNum + "_" + k]));
                    info.DifficultQuestion.Add(double.Parse(form["difficultquestion" + i + "_" + sspc.ChoiceNum + "_" + k]));
                    info.Problem.Add(form["question" + i + "_" + sspc.ChoiceNum + "_" + k]);
                    if (role == "Admin")
                    {
                        info.KnowledgeID.Add(form["hidden_b" + i + "_" + sspc.ChoiceNum + "_" + k]);
                        info.Knowledge.Add(form["txt_b" + i + "_" + sspc.ChoiceNum + "_" + k]);
                    }
                    info.AnswerValue.Add(form["answer" + i + "_" + sspc.ChoiceNum + "_" + k]);
                    info.Tip.Add(form["tip" + i + "_" + sspc.ChoiceNum + "_" + k]);
                    info.QuestionID.Add(Guid.NewGuid());
                }
                info.QuestionCount = sspc.ChoiceNum + sspc.TermNum;
                info.AnswerResposn = "_";
                info.Diffcult = double.Parse(form["difficult_" + i]);
                info.ReplyTime = double.Parse(form["time_" + i]);
                info.Score = double.Parse(form["score_" + i]);
                info.ItemID = Guid.NewGuid();
                info.PartType = "1";
                info.ItemType = "1";
                info.ItemType_CN = "快速阅读";
                info.QustionInterval = "";
                sspc.Info = info;
                sspcList.Add(sspc);
            }
            return RedirectToAction("AddItem");
        }
        #endregion

        #region 添加试卷，短对话听力
        public ActionResult CreateShortListen()
        {
            int IsLast = 1; 
            if (User.Identity.Name == "admin")
            {
                ViewData["角色"] = "Admin";
            }
            else
            {
                ViewData["角色"] = "Teacher";
            }
            Order = 2;
            int LastOrder = 0;
            if (ss == 1)
            {
                ViewData["短对话听力"] = listTemp[1];
                for (int i = 2; i < 9; i++)
                {
                    if (listTemp[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                for (int i = Order - 1; i >= 1; i-- )
                {
                    if (listTemp[i-1] != 0)
                    {
                        LastOrder = i;
                        break;
                    }
                }
                ViewData["前一项"] = LastOrder;
            }
            else
            {
                ViewData["短对话听力"] = listNumber[1];
                for (int i = 2; i < 9; i++)
                {
                    if (listNumber[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                ViewData["前一项"] = 0;
            }
            ViewData["ss"] = ss;
            ViewData["最后一项"] = IsLast;
            listNumber[1] = 0;
            return View();
        }
        public JsonResult GetShortListen()
        {
            Order = 2;
            if (slpoList.Count > 0)
            {
                return Json(slpoList);
            }
            else
                return Json("1");
        }
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateShortListen(FormCollection form)
        {
            string role = form["checkrole"];
            sa = int.Parse(form["sa"]);
            if (sa == 1)
            {
                _paperExansion.SkimmingAndScanningNum = listTemp[0];
                _paperExansion.ShortListenNum = listTemp[1];
                _paperExansion.LongListenNum = listTemp[2];
                _paperExansion.ComprehensionListenNum = listTemp[3];
                _paperExansion.ComplexListenNum = listTemp[4];
                _paperExansion.BankedClozeNum = listTemp[5];
                _paperExansion.MultipleChoiceNum = listTemp[6];
                _paperExansion.InfoMatchingNum = listTemp[7];
                _paperExansion.ClozeNum = listTemp[8];
                _paperExansion.State = 1;
                listNumber[2] = 0;
                listNumber[3] = 0;
                listNumber[4] = 0;
                listNumber[5] = 0;
                listNumber[6] = 0;
                listNumber[7] = 0;
                listNumber[8] = 0;
            }

            if (slpoList.Count != 0)
            {
                slpoList.Clear();
            }

            int totalnum = int.Parse(form["hidden"]);
            string[] filename = form["filename"].Split(',');
            DirectoryInfo TheFolder = new DirectoryInfo(Server.MapPath("../../TempSoundFile"));
            for (int i = 1; i <= totalnum; i++)
            {
                Listen lpo = new Listen();
                lpo.Choices = new List<string>();
                lpo.Script = form["textarea_" + i];
                lpo.SoundFile = filename[i - 1];
                lpo.Choices.Add(form["optionA_" + i]);
                lpo.Choices.Add(form["optionB_" + i]);
                lpo.Choices.Add(form["optionC_" + i]);
                lpo.Choices.Add(form["optionD_" + i]);
                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.QuestionID = new List<Guid>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                info.Knowledge = new List<string>();
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                info.ItemID = Guid.Parse(lpo.SoundFile);
                info.ItemType = "2";
                info.ItemType_CN = "短对话听力";
                info.QustionInterval = form["interval_" + i];
                if (role == "Admin")
                {
                    info.KnowledgeID.Add(form["hidden_a_" + i]);
                    info.Knowledge.Add(form["txt_" + i]);
                }
                info.AnswerValue.Add(form["answer_" + i]);
                info.DifficultQuestion.Add(double.Parse(form["difficultquestion_" + i]));
                info.QuestionCount = 1;
                info.AnswerResposn = "_";
                info.PartType = "2";
                info.Problem.Add(form["question_" + i]);
                info.QuestionID.Add(Guid.NewGuid());
                info.ScoreQuestion.Add(double.Parse(form["scorequestion_" + i]));
                info.TimeQuestion.Add(double.Parse(form["timequestion_" + i]));
                info.Tip.Add(form["tip_" + i]);                
                info.questionSound.Add("");
                info.timeInterval.Add(0);
                lpo.Info = info;

                slpoList.Add(lpo);
            }

            return RedirectToAction("AddItem");
        }
        #endregion

        #region 添加试卷，长对话听力
        public ActionResult CreateLongListen()
        {
            int IsLast = 1; 
            if (User.Identity.Name == "admin")
            {
                ViewData["角色"] = "Admin";
            }
            else
            {
                ViewData["角色"] = "Teacher";
            }
            Order = 3;
            int LastOrder = 0;
            if (ss == 1)
            {
                ViewData["长对话听力"] = listTemp[2];
                for (int i = 3; i < 9; i++)
                {
                    if (listTemp[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                for (int i = Order - 1; i >= 1; i--)
                {
                    if (listTemp[i - 1] != 0)
                    {
                        LastOrder = i;
                        break;
                    }
                }
                ViewData["前一项"] = LastOrder;
            }
            else
            {
                ViewData["长对话听力"] = listNumber[2];
                for (int i = 3; i < 9; i++)
                {
                    if (listNumber[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                ViewData["前一项"] = 0;
            }
            ViewData["ss"] = ss;
            ViewData["最后一项"] = IsLast;
            listNumber[2] = 0;
            return View();
        }
        public JsonResult GetLongListen()
        {
            Order = 3;
            if (llpoList.Count > 0)
            {
                return Json(llpoList);
            }
            else
                return Json("1");
        }
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateLongListen(FormCollection form)
        {
            string role = form["checkrole"];
            sa = int.Parse(form["sa"]);
            if (sa == 1)
            {
                _paperExansion.SkimmingAndScanningNum = listTemp[0];
                _paperExansion.ShortListenNum = listTemp[1];
                _paperExansion.LongListenNum = listTemp[2];
                _paperExansion.ComprehensionListenNum = listTemp[3];
                _paperExansion.ComplexListenNum = listTemp[4];
                _paperExansion.BankedClozeNum = listTemp[5];
                _paperExansion.MultipleChoiceNum = listTemp[6];
                _paperExansion.InfoMatchingNum = listTemp[7];
                _paperExansion.ClozeNum = listTemp[8];
                _paperExansion.State = 1;

                listNumber[3] = 0;
                listNumber[4] = 0;
                listNumber[5] = 0;
                listNumber[6] = 0;
                listNumber[7] = 0;
                listNumber[8] = 0;
            }

            if (llpoList.Count != 0)
            {
                llpoList.Clear();
            }
            int totalnum = int.Parse(form["hidden"]);
            string[] filename = form["filename"].Split(',');
            DirectoryInfo TheFolder = new DirectoryInfo(Server.MapPath("../../TempSoundFile"));

            for (int i = 1; i <= totalnum; i++)
            {
                Listen lpo = new Listen();
                lpo.Choices = new List<string>();
                lpo.Script = form["textarea_" + i];
                lpo.SoundFile = filename[i - 1];

                string[] Tempfilename = form["filename_" + i].Split(',');

                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.QuestionID = new List<Guid>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                info.Knowledge = new List<string>();
                info.questionSound = new List<string>();
                info.QustionInterval = form["interval_" + i];
                info.timeInterval = new List<int>();
                info.ItemID = Guid.Parse(lpo.SoundFile);
                info.ItemType = "3";
                info.ItemType_CN = "长对话听力";
                info.AnswerResposn = "_";
                info.Diffcult = double.Parse(form["difficult_" + i]);
                info.PartType = "2";
                info.ReplyTime = double.Parse(form["time_" + i]);
                info.Score = double.Parse(form["score_" + i]);
                info.QuestionCount = int.Parse(form["selectnum_" + i]);
                for (int j = 1; j <= info.QuestionCount; j++)
                {
                    info.AnswerValue.Add(form["answer" + i + "_" + j]);
                    info.DifficultQuestion.Add(double.Parse(form["difficultquestion" + i + "_" + j]));
                    if (role == "Admin")
                    {
                        info.Knowledge.Add(form["txt_a" + i + "_" + j]);
                        info.KnowledgeID.Add(form["hidden_a" + i + "_" + j]);
                    }
                    info.Problem.Add(form["question" + i + "_" + j]);
                    info.QuestionID.Add(Guid.Parse(Tempfilename[j - 1]));
                    info.questionSound.Add(Tempfilename[j - 1]);
                    info.timeInterval.Add(int.Parse(form["interval" + i + "_" + j]));
                    info.ScoreQuestion.Add(double.Parse(form["scorequestion" + i + "_" + j]));
                    info.TimeQuestion.Add(double.Parse(form["timequestion" + i + "_" + j]));
                    info.Tip.Add(form["tip" + i + "_" + j]);
                    lpo.Choices.Add(form["optionA" + i + "_" + j]);
                    lpo.Choices.Add(form["optionB" + i + "_" + j]);
                    lpo.Choices.Add(form["optionC" + i + "_" + j]);
                    lpo.Choices.Add(form["optionD" + i + "_" + j]);
                }
                lpo.Info = info;
                llpoList.Add(lpo);
            }

            return RedirectToAction("AddItem");
        }
        #endregion

        #region 添加试卷，短文听力理解
        public ActionResult CreateComprehensionListen()
        {
            int IsLast = 1; 
            if (User.Identity.Name == "admin")
            {
                ViewData["角色"] = "Admin";
            }
            else
            {
                ViewData["角色"] = "Teacher";
            }
            Order = 4;
            int LastOrder = 0;
            if (ss == 1)
            {
                ViewData["短文听力理解"] = listTemp[3];
                for (int i = 4; i < 9; i++)
                {
                    if (listTemp[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                for (int i = Order - 1; i >= 1; i--)
                {
                    if (listTemp[i - 1] != 0)
                    {
                        LastOrder = i;
                        break;
                    }
                }
                ViewData["前一项"] = LastOrder;
            }
            else
            {
                ViewData["短文听力理解"] = listNumber[3];
                for (int i = 4; i < 9; i++)
                {
                    if (listNumber[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                ViewData["前一项"] = 0;
            }
            ViewData["ss"] = ss;
            ViewData["最后一项"] = IsLast;
            listNumber[3] = 0;
            return View();
        }
        public JsonResult GetComprehensionListen()
        {
            Order = 4;
            if (rlpoList.Count > 0)
            {
                return Json(rlpoList);
            }
            else
                return Json("1");
        }
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateComprehensionListen(FormCollection form)
        {
            string role = form["checkrole"];
            sa = int.Parse(form["sa"]);
            if (sa == 1)
            {
                _paperExansion.SkimmingAndScanningNum = listTemp[0];
                _paperExansion.ShortListenNum = listTemp[1];
                _paperExansion.LongListenNum = listTemp[2];
                _paperExansion.ComprehensionListenNum = listTemp[3];
                _paperExansion.ComplexListenNum = listTemp[4];
                _paperExansion.BankedClozeNum = listTemp[5];
                _paperExansion.MultipleChoiceNum = listTemp[6];
                _paperExansion.InfoMatchingNum = listTemp[7];
                _paperExansion.ClozeNum = listTemp[8];
                _paperExansion.State = 1;
                listNumber[4] = 0;
                listNumber[5] = 0;
                listNumber[6] = 0;
                listNumber[7] = 0;
                listNumber[8] = 0;
            }

            if (rlpoList.Count != 0)
            {
                rlpoList.Clear();
            }

            int totalnum = int.Parse(form["hidden"]);
            string[] filename = form["filename"].Split(',');
            DirectoryInfo TheFolder = new DirectoryInfo(Server.MapPath("../../TempSoundFile"));
            for (int i = 1; i <= totalnum; i++)
            {
                Listen lpo = new Listen();
                lpo.Choices = new List<string>();
                lpo.Script = form["textarea_" + i];
                lpo.SoundFile = filename[i - 1];

                string[] Tempfilename = form["filename_" + i].Split(',');

                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.QuestionID = new List<Guid>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                info.Knowledge = new List<string>();
                info.questionSound = new List<string>();
                info.QustionInterval = form["interval_" + i];
                info.timeInterval = new List<int>();
                info.ItemID = Guid.Parse(lpo.SoundFile);
                info.ItemType = "4";
                info.ItemType_CN = "短文听力理解";
                info.AnswerResposn = "_";
                info.Diffcult = double.Parse(form["difficult_" + i]);
                info.PartType = "2";
                info.ReplyTime = double.Parse(form["time_" + i]);
                info.Score = double.Parse(form["score_" + i]);
                info.QuestionCount = int.Parse(form["selectnum_" + i]);
                for (int j = 1; j <= info.QuestionCount; j++)
                {
                    info.AnswerValue.Add(form["answer" + i + "_" + j]);
                    info.DifficultQuestion.Add(double.Parse(form["difficultquestion" + i + "_" + j]));
                    if (role == "Admin")
                    {
                        info.Knowledge.Add(form["txt" + i + "_" + j]);
                        info.KnowledgeID.Add(form["hidden" + i + "_" + j]);
                    }
                    info.Problem.Add(form["question" + i + "_" + j]);
                    info.QuestionID.Add(Guid.Parse(Tempfilename[j - 1]));
                    info.questionSound.Add(Tempfilename[j - 1]);
                    info.timeInterval.Add(int.Parse(form["interval" + i + "_" + j]));
                    info.ScoreQuestion.Add(double.Parse(form["scorequestion" + i + "_" + j]));
                    info.TimeQuestion.Add(double.Parse(form["timequestion" + i + "_" + j]));
                    info.Tip.Add(form["tip" + i + "_" + j]);
                    lpo.Choices.Add(form["optionA" + i + "_" + j]);
                    lpo.Choices.Add(form["optionB" + i + "_" + j]);
                    lpo.Choices.Add(form["optionC" + i + "_" + j]);
                    lpo.Choices.Add(form["optionD" + i + "_" + j]);
                }
                lpo.Info = info;
                rlpoList.Add(lpo);
            }

            return RedirectToAction("AddItem");
        }
        #endregion

        #region 添加试卷，复合型听力
        public ActionResult CreateComplexListen()
        {
            int IsLast = 1; 
            if (User.Identity.Name == "admin")
            {
                ViewData["角色"] = "Admin";
            }
            else
            {
                ViewData["角色"] = "Teacher";
            }
            Order = 5;
            int LastOrder = 0;
            if (ss == 1)
            {
                ViewData["复合型听力"] = listTemp[4];
                for (int i = 5; i < 9; i++)
                {
                    if (listTemp[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                for (int i = Order - 1; i >= 1; i--)
                {
                    if (listTemp[i - 1] != 0)
                    {
                        LastOrder = i;
                        break;
                    }
                }
                ViewData["前一项"] = LastOrder;
            }
            else
            {
                ViewData["复合型听力"] = listNumber[4];
                for (int i = 5; i < 9; i++)
                {
                    if (listNumber[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                ViewData["前一项"] = 0;
            }
            ViewData["ss"] = ss;
            ViewData["最后一项"] = IsLast;
            listNumber[4] = 0;
            return View();
        }

        public JsonResult GetComplexListen()
        {
            Order = 5;
            if (lpcList.Count > 0)
            {
                return Json(lpcList);
            }
            else
                return Json("1");
        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateComplexListen(FormCollection form)
        {
            string role = form["checkrole"];
            sa = int.Parse(form["sa"]);
            if (sa == 1)
            {
                _paperExansion.SkimmingAndScanningNum = listTemp[0];
                _paperExansion.ShortListenNum = listTemp[1];
                _paperExansion.LongListenNum = listTemp[2];
                _paperExansion.ComprehensionListenNum = listTemp[3];
                _paperExansion.ComplexListenNum = listTemp[4];
                _paperExansion.BankedClozeNum = listTemp[5];
                _paperExansion.MultipleChoiceNum = listTemp[6];
                _paperExansion.InfoMatchingNum = listTemp[7];
                _paperExansion.ClozeNum = listTemp[8];
                _paperExansion.State = 1;
                listNumber[5] = 0;
                listNumber[6] = 0;
                listNumber[7] = 0;
                listNumber[8] = 0;
            }

            if (lpcList.Count != 0)
            {
                lpcList.Clear();
            }
            int totalnum = int.Parse(form["hidden"]);
            string[] filename = form["filename"].Split(',');
            DirectoryInfo TheFolder = new DirectoryInfo(Server.MapPath("../../TempSoundFile"));
            for (int i = 1; i <= totalnum; i++)
            {
                Listen lpc = new Listen();
                lpc.Script = form["textarea_" + i];
                lpc.SoundFile = filename[i - 1];

                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.QuestionID = new List<Guid>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                info.Knowledge = new List<string>();
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                info.ItemID = Guid.Parse(lpc.SoundFile);
                info.ItemType = "5";
                info.ItemType_CN = "复合型听力";
                info.AnswerResposn = "_";
                info.Diffcult = double.Parse(form["difficult_" + i]);
                info.PartType = "2";
                info.QustionInterval = form["interval_" + i];
                info.ReplyTime = double.Parse(form["time_" + i]);
                info.Score = double.Parse(form["score_" + i]);
                info.QuestionCount = int.Parse(form["selectnum_" + i]);
                for (int j = 1; j <= info.QuestionCount; j++)
                {
                    info.questionSound.Add("");
                    info.timeInterval.Add(0);
                    info.AnswerValue.Add(form["answer" + i + "_" + j]);
                    info.DifficultQuestion.Add(double.Parse(form["difficultquestion" + i + "_" + j]));
                    if (role == "Admin")
                    {
                        info.Knowledge.Add(form["txt_a" + i + "_" + j]);
                        info.KnowledgeID.Add(form["hidden_a" + i + "_" + j]);
                    }
                    info.Problem.Add(form["question" + i + "_" + j]);
                    info.QuestionID.Add(Guid.NewGuid());
                    info.ScoreQuestion.Add(double.Parse(form["scorequestion" + i + "_" + j]));
                    info.TimeQuestion.Add(double.Parse(form["timequestion" + i + "_" + j]));
                    info.Tip.Add(form["tip" + i + "_" + j]);
                }
                lpc.Info = info;
                lpcList.Add(lpc);
            }

            return RedirectToAction("AddItem");
        }
        #endregion

        #region 添加试卷，阅读选词填空
        public ActionResult CreateBankedCloze()
        {
            int IsLast = 1; 
            if (User.Identity.Name == "admin")
            {
                ViewData["角色"] = "Admin";
            }
            else
            {
                ViewData["角色"] = "Teacher";
            }
            Order = 6;
            int LastOrder = 0;
            if (ss == 1)
            {
                ViewData["阅读选词填空"] = listTemp[5];
                for (int i = 6; i < 9; i++)
                {
                    if (listTemp[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                for (int i = Order - 1; i >= 1; i--)
                {
                    if (listTemp[i - 1] != 0)
                    {
                        LastOrder = i;
                        break;
                    }
                }
                ViewData["前一项"] = LastOrder;
            }
            else
            {
                ViewData["阅读选词填空"] = listNumber[5];
                for (int i = 6; i < 9; i++)
                {
                    if (listNumber[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                ViewData["前一项"] = 0;
            }
            ViewData["ss"] = ss;
            ViewData["最后一项"] = IsLast;
            listNumber[5] = 0;
            return View();
        }
        public JsonResult GetBankedCloze()
        {
            Order = 6;
            if (rpcList.Count > 0)
            {
                return Json(rpcList);
            }
            else
                return Json("1");
        }
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateBankedCloze(FormCollection form)
        {
            string role = form["checkrole"];
            sa = int.Parse(form["sa"]);
            if (sa == 1)
            {
                _paperExansion.SkimmingAndScanningNum = listTemp[0];
                _paperExansion.ShortListenNum = listTemp[1];
                _paperExansion.LongListenNum = listTemp[2];
                _paperExansion.ComprehensionListenNum = listTemp[3];
                _paperExansion.ComplexListenNum = listTemp[4];
                _paperExansion.BankedClozeNum = listTemp[5];
                _paperExansion.MultipleChoiceNum = listTemp[6];
                _paperExansion.InfoMatchingNum = listTemp[7];
                _paperExansion.ClozeNum = listTemp[8];
                _paperExansion.State = 1;
                listNumber[6] = 0;
                listNumber[7] = 0;
                listNumber[8] = 0;
            }

            if (rpcList.Count != 0)
            {
                rpcList.Clear();
            }
            int totalnum = int.Parse(form["hidden"]);
            for (int i = 1; i <= totalnum; i++)
            {
                ReadingPartCompletion rpc = new ReadingPartCompletion();
                rpc.Content = form["textarea_" + i];
                rpc.WordList = new List<string>();
                for (int j = 1; j <= 15; j++)
                {
                    rpc.WordList.Add(form["option_" + i + "_" + j]);
                }

                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.QuestionID = new List<Guid>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                info.Knowledge = new List<string>();
                info.ItemID = Guid.NewGuid();
                info.ItemType = "6";
                info.ItemType_CN = "阅读选词填空";
                info.AnswerResposn = "_";
                info.Diffcult = double.Parse(form["difficult_" + i]);
                info.PartType = "3";
                info.QustionInterval = "";
                info.ReplyTime = double.Parse(form["time_" + i]);
                info.Score = double.Parse(form["score_" + i]);
                info.QuestionCount = 10;
                for (int j = 1; j <= 10; j++)
                {
                    info.AnswerValue.Add(form["answer_" + i + "_" + j]);
                    info.DifficultQuestion.Add(double.Parse(form["difficultquestion_" + i + "_" + j]));
                    if (role == "Admin")
                    {
                        info.Knowledge.Add(form["txt_a_" + i + "_" + j]);
                        info.KnowledgeID.Add(form["hidden_a_" + i + "_" + j]);
                    }
                    info.Problem.Add(form["question_" + i + "_" + j]);
                    info.QuestionID.Add(Guid.NewGuid());
                    info.ScoreQuestion.Add(double.Parse(form["scorequestion_" + i + "_" + j]));
                    info.TimeQuestion.Add(double.Parse(form["timequestion_" + i + "_" + j]));
                    info.Tip.Add(form["tip_" + i + "_" + j]);
                }
                rpc.Info = info;
                rpcList.Add(rpc);
            }
            return RedirectToAction("AddItem");
        }
        #endregion

        #region 添加试卷，阅读理解选择题型
        public ActionResult CreateMultipleChoice()
        {
            int IsLast = 1; 
            if (User.Identity.Name == "admin")
            {
                ViewData["角色"] = "Admin";
            }
            else
            {
                ViewData["角色"] = "Teacher";
            }
            Order = 7;
            int LastOrder = 0;
            if (ss == 1)
            {
                ViewData["阅读选择题型"] = listTemp[6];
                for (int i = 7; i < 9; i++)
                {
                    if (listTemp[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                for (int i = Order - 1; i >= 1; i--)
                {
                    if (listTemp[i - 1] != 0)
                    {
                        LastOrder = i;
                        break;
                    }
                }
                ViewData["前一项"] = LastOrder;
            }
            else
            {
                ViewData["阅读选择题型"] = listNumber[6];
                for (int i = 7; i < 9; i++)
                {
                    if (listNumber[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                ViewData["前一项"] = 0;
            }
            ViewData["ss"] = ss;
            ViewData["最后一项"] = IsLast;
            listNumber[6] = 0;
            return View();
        }
        public JsonResult GetMultipleChoice()
        {
            Order = 7;
            if (rpoList.Count > 0)
            {
                return Json(rpoList);
            }
            else
                return Json("1");
        }
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateMultipleChoice(FormCollection form)
        {
            string role = form["checkrole"];
            sa = int.Parse(form["sa"]);
            if (sa == 1)
            {
                _paperExansion.SkimmingAndScanningNum = listTemp[0];
                _paperExansion.ShortListenNum = listTemp[1];
                _paperExansion.LongListenNum = listTemp[2];
                _paperExansion.ComprehensionListenNum = listTemp[3];
                _paperExansion.ComplexListenNum = listTemp[4];
                _paperExansion.BankedClozeNum = listTemp[5];
                _paperExansion.MultipleChoiceNum = listTemp[6];
                _paperExansion.InfoMatchingNum = listTemp[7];
                _paperExansion.ClozeNum = listTemp[8];
                _paperExansion.State = 1;
                listNumber[7] = 0;
                listNumber[8] = 0;
            }

            if (rpoList.Count != 0)
            {
                rpoList.Clear();
            }
            int totalnum = int.Parse(form["hidden"]);
            for (int i = 1; i <= totalnum; i++)
            {
                ReadingPartOption rpo = new ReadingPartOption();
                rpo.Choices = new List<string>();
                rpo.Content = form["textarea_" + i];

                ItemBassInfo info = new ItemBassInfo();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.DifficultQuestion = new List<double>();
                info.Problem = new List<string>();
                info.KnowledgeID = new List<string>();
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.Knowledge = new List<string>();

                info.AnswerResposn = "_";
                info.Diffcult = double.Parse(form["difficult_" + i]);
                info.ReplyTime = double.Parse(form["time_" + i]);
                info.Score = double.Parse(form["score_" + i]);
                info.ItemID = Guid.NewGuid();
                info.PartType = "3";
                info.ItemType = "7";
                info.ItemType_CN = "阅读选择题型";
                info.QustionInterval = "";

                info.QuestionCount = int.Parse(form["selectnum_" + i]);
                for (int j = 1; j <= info.QuestionCount; j++)
                {
                    rpo.Choices.Add(form["optionA" + i + "_" + j]);
                    rpo.Choices.Add(form["optionB" + i + "_" + j]);
                    rpo.Choices.Add(form["optionC" + i + "_" + j]);
                    rpo.Choices.Add(form["optionD" + i + "_" + j]);
                    info.ScoreQuestion.Add(double.Parse(form["scorequestion" + i + "_" + j]));
                    info.TimeQuestion.Add(double.Parse(form["timequestion" + i + "_" + j]));
                    info.DifficultQuestion.Add(double.Parse(form["difficultquestion" + i + "_" + j]));
                    info.Problem.Add(form["question" + i + "_" + j]);
                    if (role == "Admin")
                    {
                        info.KnowledgeID.Add(form["hidden_a" + i + "_" + j]);
                        info.Knowledge.Add(form["txt_a" + i + "_" + j]);
                    }
                    info.AnswerValue.Add(form["answer" + i + "_" + j]);
                    info.Tip.Add(form["tip" + i + "_" + j]);
                    info.QuestionID.Add(Guid.NewGuid());
                }
                rpo.Info = info;
                rpoList.Add(rpo);
            }
            return RedirectToAction("AddItem");
        }
        #endregion

        #region 添加试卷，信息匹配
        public ActionResult CreateInfoMatching()
        {
            int IsLast = 1;
            if (User.Identity.Name == "admin")
            {
                ViewData["角色"] = "Admin";
            }
            else
            {
                ViewData["角色"] = "Teacher";
            }
            Order = 8;
            int LastOrder = 0;
            if (ss == 1)
            {
                ViewData["信息匹配题型"] = listTemp[7];
                for (int i = 8; i < 9; i++)
                {
                    if (listTemp[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                for (int i = Order - 1; i >= 1; i--)
                {
                    if (listTemp[i - 1] != 0)
                    {
                        LastOrder = i;
                        break;
                    }
                }
                ViewData["前一项"] = LastOrder;
            }
            else
            {
                ViewData["信息匹配题型"] = listNumber[7];
                for (int i = 8; i < 9; i++)
                {
                    if (listNumber[i] != 0)
                    {
                        IsLast = 0;
                    }
                }
                ViewData["前一项"] = 0;
            }
            ViewData["ss"] = ss;
            ViewData["最后一项"] = IsLast;
            listNumber[7] = 0;
            return View();
        }
        public JsonResult GetInfoMatching()
        {
            Order = 8;
            if (infoMatList.Count > 0)
            {
                return Json(infoMatList);
            }
            else
                return Json("1");
        }
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateInfoMatching(FormCollection form)
        {
            string role = form["checkrole"];
            sa = int.Parse(form["sa"]);
            if (sa == 1)
            {
                _paperExansion.SkimmingAndScanningNum = listTemp[0];
                _paperExansion.ShortListenNum = listTemp[1];
                _paperExansion.LongListenNum = listTemp[2];
                _paperExansion.ComprehensionListenNum = listTemp[3];
                _paperExansion.ComplexListenNum = listTemp[4];
                _paperExansion.BankedClozeNum = listTemp[5];
                _paperExansion.MultipleChoiceNum = listTemp[6];
                _paperExansion.InfoMatchingNum = listTemp[7];
                _paperExansion.ClozeNum = listTemp[8];
                _paperExansion.State = 1;
                listNumber[8] = 0;
            }

            if (infoMatList.Count != 0)
            {
                infoMatList.Clear();
            }
            int totalnum = int.Parse(form["hidden"]);
            for (int i = 1; i <= totalnum; i++)
            {
                InfoMatchingCompletion infoMat = new InfoMatchingCompletion();
                infoMat.Content = form["textarea_" + i];
                infoMat.WordList = new List<string>();
                for (int j = 1; j <= 15; j++)
                {
                    infoMat.WordList.Add(form["option_" + i + "_" + j]);
                }

                ItemBassInfo info = new ItemBassInfo();
                info.AnswerValue = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.KnowledgeID = new List<string>();
                info.Problem = new List<string>();
                info.QuestionID = new List<Guid>();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.Tip = new List<string>();
                info.Knowledge = new List<string>();
                info.ItemID = Guid.NewGuid();
                info.ItemType = "9";
                info.ItemType_CN = "信息匹配";
                info.AnswerResposn = "_";
                info.Diffcult = double.Parse(form["difficult_" + i]);
                info.PartType = "3";
                info.QustionInterval = "";
                info.ReplyTime = double.Parse(form["time_" + i]);
                info.Score = double.Parse(form["score_" + i]);
                info.QuestionCount = 10;
                for (int j = 1; j <= 10; j++)
                {
                    info.AnswerValue.Add(form["answer_" + i + "_" + j]);
                    info.DifficultQuestion.Add(double.Parse(form["difficultquestion_" + i + "_" + j]));
                    if (role == "Admin")
                    {
                        info.Knowledge.Add(form["txt_a_" + i + "_" + j]);
                        info.KnowledgeID.Add(form["hidden_a_" + i + "_" + j]);
                    }
                    info.Problem.Add(form["question_" + i + "_" + j]);
                    info.QuestionID.Add(Guid.NewGuid());
                    info.ScoreQuestion.Add(double.Parse(form["scorequestion_" + i + "_" + j]));
                    info.TimeQuestion.Add(double.Parse(form["timequestion_" + i + "_" + j]));
                    info.Tip.Add(form["tip_" + i + "_" + j]);
                }
                infoMat.Info = info;
                infoMatList.Add(infoMat);
            }
            return RedirectToAction("AddItem");
        }
        #endregion

        #region 添加试卷，完型填空
        public ActionResult CreateCloze()
        {
            
            if (User.Identity.Name == "admin")
            {
                ViewData["角色"] = "Admin";
            }
            else
            {
                ViewData["角色"] = "Teacher";
            }
            Order = 9;
            int LastOrder = 0;
            if (ss == 1)
            {
                ViewData["完型填空"] = listTemp[8];
                for (int i = Order - 1; i >= 1; i--)
                {
                    if (listTemp[i - 1] != 0)
                    {
                        LastOrder = i;
                        break;
                    }
                }
                ViewData["前一项"] = LastOrder;
            }
            else
            {
                ViewData["完型填空"] = listNumber[8];
                ViewData["前一项"] = 0;
            }
            ViewData["ss"] = ss;
            listNumber[8] = 0;
            return View();
        }
        public JsonResult GetCloze()
        {
            Order = 9;
            if (cpList.Count > 0)
            {
                return Json(cpList);
            }
            else
                return Json("1");
        }
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreateCloze(FormCollection form)
        {
            string role = form["checkrole"];
            sa = int.Parse(form["sa"]);
            if (cpList.Count > 0)
            {
                cpList.Clear();
            }
            int totalnum = int.Parse(form["hidden"]);
            for (int i = 1; i <= totalnum; i++)
            {
                ClozePart cp = new ClozePart();
                cp.Choices = new List<string>();
                cp.Content = form["textarea_" + i];

                ItemBassInfo info = new ItemBassInfo();
                info.ScoreQuestion = new List<double>();
                info.TimeQuestion = new List<double>();
                info.DifficultQuestion = new List<double>();
                info.Problem = new List<string>();
                info.KnowledgeID = new List<string>();
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.QuestionID = new List<Guid>();
                info.Knowledge = new List<string>();

                info.AnswerResposn = "_";
                info.Diffcult = double.Parse(form["difficult_" + i]);
                info.ReplyTime = double.Parse(form["time_" + i]);
                info.Score = double.Parse(form["score_" + i]);
                info.ItemID = Guid.NewGuid();
                info.PartType = "4";
                info.ItemType = "8";
                info.ItemType_CN = "完型填空";
                info.QustionInterval = "";

                info.QuestionCount = 20;
                for (int j = 1; j <= info.QuestionCount; j++)
                {
                    cp.Choices.Add(form["optionA" + i + "_" + j]);
                    cp.Choices.Add(form["optionB" + i + "_" + j]);
                    cp.Choices.Add(form["optionC" + i + "_" + j]);
                    cp.Choices.Add(form["optionD" + i + "_" + j]);
                    info.ScoreQuestion.Add(double.Parse(form["scorequestion" + i + "_" + j]));
                    info.TimeQuestion.Add(double.Parse(form["timequestion" + i + "_" + j]));
                    info.DifficultQuestion.Add(double.Parse(form["difficultquestion" + i + "_" + j]));
                    //info.Problem.Add(form["question" + i + "_" + j]);
                    if (role == "Admin")
                    {
                        info.KnowledgeID.Add(form["hidden_a" + i + "_" + j]);
                        info.Knowledge.Add(form["txt_a" + i + "_" + j]);
                    }
                    info.AnswerValue.Add(form["answer" + i + "_" + j]);
                    info.Tip.Add(form["tip" + i + "_" + j]);
                    info.QuestionID.Add(Guid.NewGuid());
                }
                cp.Info = info;
                cpList.Add(cp);
            }
            return RedirectToAction("AddItem");
        }
        #endregion

        #region 上一步
        public ActionResult BackUp()
        {
            Order--;
            if (Order >= 0 && Order < 9)
            {
                s = 0;
                listNumber[Order] = listTemp[Order];
                return Json(Order);
            }
            else
            {
                return Json("");
            }
        }
        #endregion

        #region 试卷信息总汇
        public ActionResult TotalPaper()
        {
            Order = 10;
            string num = string.Empty;
            foreach (int i in listTemp)
            {
                num += i + ",";
            }
            num = num.Substring(0, num.Length - 1);
            ViewData["题目数量"] = num;
            return View();
        }
        [ChildActionOnly]
        public ActionResult PaperInfo()
        {
            double score = 0;
            double duration = 0;
            double diffcult = 0;
            if (sspcList.Count != 0)
            {
                for (int i = 0; i < sspcList.Count; i++)
                {
                    score += sspcList[i].Info.Score;
                    duration += sspcList[i].Info.ReplyTime;
                    diffcult += sspcList[i].Info.Diffcult;
                }
            }
            if (slpoList.Count != 0)
            {
                for (int i = 0; i < slpoList.Count; i++)
                {
                    score += slpoList[i].Info.ScoreQuestion[0];
                    duration += slpoList[i].Info.TimeQuestion[0];
                    diffcult += slpoList[i].Info.DifficultQuestion[0];
                }
            }
            if (llpoList.Count != 0)
            {
                for (int i = 0; i < llpoList.Count; i++)
                {
                    score += llpoList[i].Info.Score;
                    duration += llpoList[i].Info.ReplyTime;
                    diffcult += llpoList[i].Info.Diffcult;
                }
            }
            if (rlpoList.Count != 0)
            {
                for (int i = 0; i < rlpoList.Count; i++)
                {
                    score += rlpoList[i].Info.Score;
                    duration += rlpoList[i].Info.ReplyTime;
                    diffcult += rlpoList[i].Info.Diffcult;
                }
            }
            if (lpcList.Count != 0)
            {
                for (int i = 0; i < lpcList.Count; i++)
                {
                    score += lpcList[i].Info.Score;
                    duration += lpcList[i].Info.ReplyTime;
                    diffcult += lpcList[i].Info.Diffcult;
                }
            }
            if (rpcList.Count != 0)
            {
                for (int i = 0; i < rpcList.Count; i++)
                {
                    score += rpcList[i].Info.Score;
                    duration += rpcList[i].Info.ReplyTime;
                    diffcult += rpcList[i].Info.Diffcult;
                }
            }
            if (rpoList.Count != 0)
            {
                for (int i = 0; i < rpoList.Count; i++)
                {
                    score += rpoList[i].Info.Score;
                    duration += rpoList[i].Info.ReplyTime;
                    diffcult += rpoList[i].Info.Diffcult;
                }
            }
            if (infoMatList.Count != 0)
            {
                for (int i = 0; i < infoMatList.Count; i++)
                {
                    score += infoMatList[i].Info.Score;
                    duration += infoMatList[i].Info.ReplyTime;
                    diffcult += infoMatList[i].Info.Diffcult;
                }
            }
            if (cpList.Count != 0)
            {
                for (int i = 0; i < cpList.Count; i++)
                {
                    score += cpList[i].Info.Score;
                    duration += cpList[i].Info.ReplyTime;
                    diffcult += cpList[i].Info.Diffcult;
                }
            }
            _tempPaper.Score = score;
            _tempPaper.Duration = duration;
            int count = infoMatList.Count + sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count + rpoList.Count + cpList.Count;
            diffcult = diffcult / count;
            diffcult = Math.Round(diffcult, 1);
            _tempPaper.Difficult = diffcult;
            return PartialView(_tempPaper);
        }
        [ChildActionOnly]
        public ActionResult PartTypeList()
        {
            return PartialView(_paper.SelectPartType());
        }
        [ChildActionOnly]
        public ActionResult ItemTypeList(string PartTypeID)
        {
            return PartialView(_paper.SelectItemTypeByPartTypeID(int.Parse(PartTypeID)));
        }
        [HttpPost]
        [Filter.LogFilter(Description = "添加试卷")]
        public ActionResult SavePaper()
        {
            try
            {
                using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 5, 0)))
                {

                    List<Guid> list = new List<Guid>();//存储听力题型的ID
                    List<Guid> soundQuestion = new List<Guid>();
                    int UserID = _item.SelectUserID(User.Identity.Name);
                    if (sa == 2)
                    {
                        #region 保存到数据库
                        
                        CEDTS_Paper paper = new CEDTS_Paper();
                        paper.PaperID = _tempPaper.PaperID;
                        paper.UserID = UserID;
                        paper.UpdateUserID = paper.UserID;
                        paper.CreateTime = DateTime.Now;
                        paper.UpdateTime = _tempPaper.CreateTime;
                        paper.Title = _tempPaper.Title;
                        paper.Description = _tempPaper.Description;
                        paper.Difficult = _tempPaper.Difficult;
                        paper.Score = _tempPaper.Score;
                        paper.Duration = _tempPaper.Duration;
                        paper.Type = _tempPaper.Type;
                        if (UserID == 1)
                        {
                            paper.State = 1;
                        } 
                        else
                        {
                            paper.State = 9;
                        }
                        

                        #region 保存试卷xml
                        PaperTotal pt = new PaperTotal();
                        pt.PaperID = paper.PaperID;
                        pt.Title = paper.Title;
                        pt.Type = paper.Type;
                        pt.Duration = paper.Duration.Value;
                        pt.Difficult = paper.Difficult.Value;
                        pt.Score = paper.Score.Value;
                        pt.Description = paper.Description;
                        pt.CreateTime = paper.CreateTime.Value;
                        pt.UserID = paper.UserID.Value;
                        pt.UpdateUserID = pt.UserID;
                        pt.SspcList = sspcList;
                        pt.SlpoList = slpoList;
                        pt.LlpoList = llpoList;
                        pt.LpcList = lpcList;
                        pt.RlpoList = rlpoList;
                        pt.RpcList = rpcList;
                        pt.RpoList = rpoList;
                        pt.CpList = cpList;
                        pt.InfMatList = infoMatList;
                        paper.PaperContent = _paper.CreatePaperXML(pt);//保存本地XML并向数据库中保存一份XML文件
                        #endregion

                        if (s != 0)
                        {
                            s = 0;
                            _paper.UpadatePaper(paper);

                        }
                        else
                        {
                            s = 0;
                            _paper.SavePaper(paper);//保存试卷到数据库

                        }
                        #endregion
                    }
                    else//暂存，存放临时表
                    {
                        #region 保存到数据库
                        _TQuestionKnowledge.Clear(_tempPaper.PaperID);
                        CEDTS_Paper paper = new CEDTS_Paper();
                        paper.PaperID = _tempPaper.PaperID;
                        paper.UserID = _item.SelectUserID(User.Identity.Name);
                        paper.UpdateUserID = paper.UserID;
                        paper.CreateTime = DateTime.Now;
                        paper.UpdateTime = _tempPaper.CreateTime;
                        paper.Title = _tempPaper.Title;
                        paper.Description = _tempPaper.Description;
                        paper.Difficult = _tempPaper.Difficult;
                        paper.Score = _tempPaper.Score;
                        paper.Duration = _tempPaper.Duration;
                        paper.Type = _tempPaper.Type;
                        paper.State = 2;
                        _paperExansion.PaperID = _tempPaper.PaperID;
                        if (s != 0)
                        {
                            _paper.UpadatePaper(paper);
                            _paper.UpdataPaperExpansion(_paperExansion);
                            s = 0;
                        }
                        else
                        {
                            _paper.SavePaper(paper);//保存试卷到数据库
                            _paper.SavePaperExpansion(_paperExansion);
                            s = 0;
                        }
                        #endregion
                    }

                    #region 保存快速阅读信息
                    if (sspcList.Count > 0)
                    {

                        for (int i = 0; i < sspcList.Count; i++)
                        {
                            List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                            List<CEDTS_TemporaryQuestion> tlistquestion = new List<CEDTS_TemporaryQuestion>();
                            _item.AddSspcItem(sspcList[i]);//保存到XML


                            if (sa == 2)
                            {
                                #region 保存试题到数据库
                                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                                item.UserID = UserID;
                                item.UpdateUserID = UserID;
                                item.SaveTime = DateTime.Now;
                                item.UpdateTime = DateTime.Now;
                                item.AssessmentItemID = sspcList[i].Info.ItemID;
                                item.ItemTypeID = 1;
                                item.QuestionCount = sspcList[i].Info.QuestionCount;
                                item.Description = "";
                                item.Difficult = sspcList[i].Info.Diffcult;
                                item.Duration = sspcList[i].Info.ReplyTime;
                                item.Original = sspcList[i].Content;
                                item.Score = sspcList[i].Info.Score;
                                item.Count = 0;
                                string name = "../../ExaminationItemLibrary/" + sspcList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();

                                _item.Create(item);
                                #endregion

                                #region 保存试题和试卷的关系表
                                CEDTS_PaperAssessment pa = new CEDTS_PaperAssessment();
                                pa.PaperID = _tempPaper.PaperID;
                                pa.AssessmentItemID = item.AssessmentItemID;
                                pa.ItemTypeID = 1;
                                pa.PartTypeID = 1;
                                pa.Weight = i + 1;
                                _paper.SavePaperAssessment(pa);


                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < sspcList[i].ChoiceNum; j++)
                                {
                                    CEDTS_Question cquestion = new CEDTS_Question();
                                    cquestion.QuestionID = sspcList[i].Info.QuestionID[j];
                                    cquestion.AssessmentItemID = sspcList[i].Info.ItemID;
                                    cquestion.QuestionContent = sspcList[i].Info.Problem[j];
                                    cquestion.ChooseA = sspcList[i].Choices[4 * j + 0];
                                    cquestion.ChooseB = sspcList[i].Choices[4 * j + 1];
                                    cquestion.ChooseC = sspcList[i].Choices[4 * j + 2];
                                    cquestion.ChooseD = sspcList[i].Choices[4 * j + 3];
                                    cquestion.Answer = sspcList[i].Info.AnswerValue[j];
                                    cquestion.Duration = sspcList[i].Info.TimeQuestion[j];
                                    cquestion.Score = sspcList[i].Info.ScoreQuestion[j];
                                    cquestion.Difficult = sspcList[i].Info.DifficultQuestion[j];
                                    cquestion.Analyze = sspcList[i].Info.Tip[j];

                                    cquestion.Order = (j + 1);
                                    listquestion.Add(cquestion);
                                }

                                for (int k = 0; k < sspcList[i].TermNum; k++)
                                {
                                    CEDTS_Question cquestion = new CEDTS_Question();
                                    cquestion.QuestionID = sspcList[i].Info.QuestionID[k + sspcList[i].ChoiceNum];
                                    cquestion.AssessmentItemID = sspcList[i].Info.ItemID;
                                    cquestion.QuestionContent = sspcList[i].Info.Problem[k + sspcList[i].ChoiceNum];
                                    cquestion.Answer = sspcList[i].Info.AnswerValue[k + sspcList[i].ChoiceNum];
                                    cquestion.Duration = sspcList[i].Info.TimeQuestion[k + sspcList[i].ChoiceNum];
                                    cquestion.Score = sspcList[i].Info.ScoreQuestion[k + sspcList[i].ChoiceNum];
                                    cquestion.Difficult = sspcList[i].Info.DifficultQuestion[k + sspcList[i].ChoiceNum];
                                    cquestion.Analyze = sspcList[i].Info.Tip[k + sspcList[i].ChoiceNum];
                                    cquestion.Order = (k + sspcList[i].ChoiceNum + 1);

                                    cquestion.ChooseA = "";
                                    cquestion.ChooseB = "";
                                    cquestion.ChooseC = "";
                                    cquestion.ChooseD = "";

                                    listquestion.Add(cquestion);
                                }
                                _question.CreateQuestion(listquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < listquestion.Count; m++)
                                    {

                                        string[] Knowledge = sspcList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                            qk.QuestionID = listquestion[m].QuestionID;
                                            qk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            qk.Weight = n + 1;
                                            _QuestionKnowledge.Create(qk);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                //暂存
                                #region 保存试题到数据库
                                CEDTS_TemporaryAssessmentItem Titem = new CEDTS_TemporaryAssessmentItem();

                                Titem.SaveTime = DateTime.Now;
                                Titem.UpdateTime = DateTime.Now;
                                Titem.AssessmentItemID = sspcList[i].Info.ItemID;
                                Titem.ItemTypeID = 1;
                                Titem.QuestionCount = sspcList[i].Info.QuestionCount;
                                Titem.Description = "";
                                Titem.Difficult = sspcList[i].Info.Diffcult;
                                Titem.Duration = sspcList[i].Info.ReplyTime;
                                Titem.Original = sspcList[i].Content;
                                Titem.Score = sspcList[i].Info.Score;
                                Titem.Count = 0;
                                Titem.UserID = UserID;
                                Titem.UpdateUserID = UserID;
                                string tname = "../../ExaminationItemLibrary/" + sspcList[i].Info.ItemID + ".xml";
                                string tMapPath = HttpContext.Server.MapPath(tname);
                                XDocument tdoc = XDocument.Load(tMapPath);
                                Titem.Content = "<?xml version='1.0' encoding='gb2312'?>" + tdoc.ToString();

                                _titem.Create(Titem);
                                #endregion

                                #region 保存试题和试卷的临时关系表
                                CEDTS_TemporaryPaperAssessment tpa = new CEDTS_TemporaryPaperAssessment();
                                tpa.PaperID = _tempPaper.PaperID;
                                tpa.AssessmentItemID = Titem.AssessmentItemID;
                                tpa.ItemTypeID = 1;
                                tpa.PartTypeID = 1;
                                tpa.Weight = i + 1;
                                _Tpaper.SavePaperAssessment(tpa);


                                #endregion

                                #region 保存小问题到临时数据库
                                for (int j = 0; j < sspcList[i].ChoiceNum; j++)
                                {
                                    CEDTS_TemporaryQuestion tcquestion = new CEDTS_TemporaryQuestion();
                                    tcquestion.QuestionID = sspcList[i].Info.QuestionID[j];
                                    tcquestion.AssessmentItemID = sspcList[i].Info.ItemID;
                                    tcquestion.QuestionContent = sspcList[i].Info.Problem[j];
                                    tcquestion.ChooseA = sspcList[i].Choices[4 * j + 0];
                                    tcquestion.ChooseB = sspcList[i].Choices[4 * j + 1];
                                    tcquestion.ChooseC = sspcList[i].Choices[4 * j + 2];
                                    tcquestion.ChooseD = sspcList[i].Choices[4 * j + 3];
                                    tcquestion.Answer = sspcList[i].Info.AnswerValue[j];
                                    tcquestion.Duration = sspcList[i].Info.TimeQuestion[j];
                                    tcquestion.Score = sspcList[i].Info.ScoreQuestion[j];
                                    tcquestion.Difficult = sspcList[i].Info.DifficultQuestion[j];
                                    tcquestion.Analyze = sspcList[i].Info.Tip[j];
                                    tcquestion.Order = (j + 1);



                                    tlistquestion.Add(tcquestion);
                                }

                                for (int k = 0; k < sspcList[i].TermNum; k++)
                                {
                                    CEDTS_TemporaryQuestion tcquestion = new CEDTS_TemporaryQuestion();
                                    tcquestion.QuestionID = sspcList[i].Info.QuestionID[k + sspcList[i].ChoiceNum];
                                    tcquestion.AssessmentItemID = sspcList[i].Info.ItemID;
                                    tcquestion.QuestionContent = sspcList[i].Info.Problem[k + sspcList[i].ChoiceNum];
                                    tcquestion.Answer = sspcList[i].Info.AnswerValue[k + sspcList[i].ChoiceNum];
                                    tcquestion.Duration = sspcList[i].Info.TimeQuestion[k + sspcList[i].ChoiceNum];
                                    tcquestion.Score = sspcList[i].Info.ScoreQuestion[k + sspcList[i].ChoiceNum];
                                    tcquestion.Difficult = sspcList[i].Info.DifficultQuestion[k + sspcList[i].ChoiceNum];
                                    tcquestion.Analyze = sspcList[i].Info.Tip[k + sspcList[i].ChoiceNum];
                                    tcquestion.Order = (k + sspcList[i].ChoiceNum + 1);
                                    tcquestion.ChooseA = "";
                                    tcquestion.ChooseB = "";
                                    tcquestion.ChooseC = "";
                                    tcquestion.ChooseD = "";
                                    tlistquestion.Add(tcquestion);
                                }
                                _Tquestion.CreateQuestion(tlistquestion);
                                #endregion

                                #region 保存小问题和知识点临时关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < tlistquestion.Count; m++)
                                    {

                                        string[] Knowledge = sspcList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_TemporaryQuestionKnowledge tqk = new CEDTS_TemporaryQuestionKnowledge();
                                            tqk.QuestionID = tlistquestion[m].QuestionID;
                                            tqk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            tqk.Weight = n + 1;
                                            _TQuestionKnowledge.Create(tqk);
                                        }
                                    }
                                }




                                #endregion
                            }

                        }
                    }
                    #endregion

                    #region 保存短对话听力

                    if (slpoList.Count > 0)
                    {
                        for (int i = 0; i < slpoList.Count; i++)
                        {
                            List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                            List<CEDTS_TemporaryQuestion> tlistquestion = new List<CEDTS_TemporaryQuestion>();
                            list.Add(slpoList[i].Info.ItemID);

                            _item.AddItem(slpoList[i]);//保存到XML

                            if (sa == 2)
                            {
                                #region 保存试题到数据库
                                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();

                                item.SaveTime = DateTime.Now;
                                item.UpdateTime = DateTime.Now;
                                item.AssessmentItemID = slpoList[i].Info.ItemID;
                                item.SoundFile = slpoList[i].SoundFile;
                                item.ItemTypeID = 2;
                                item.QuestionCount = slpoList[i].Info.QuestionCount;
                                item.Description = "";
                                item.Score = slpoList[i].Info.ScoreQuestion[0];
                                item.Difficult = slpoList[i].Info.DifficultQuestion[0];
                                item.Duration = slpoList[i].Info.TimeQuestion[0];
                                item.Interval = int.Parse(slpoList[i].Info.QustionInterval);
                                item.UserID = _item.SelectUserID(User.Identity.Name);
                                item.UpdateUserID = item.UserID;
                                item.Count = 0;
                                item.Unit = "";
                                item.Course = "";
                                item.Original = slpoList[i].Script;
                                string name = "../../ExaminationItemLibrary/" + slpoList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _item.Create(item);

                                #endregion

                                #region 保存试题和试卷的关系表
                                CEDTS_PaperAssessment pa = new CEDTS_PaperAssessment();
                                pa.PaperID = _tempPaper.PaperID;
                                pa.AssessmentItemID = item.AssessmentItemID;
                                pa.ItemTypeID = 2;
                                pa.PartTypeID = 2;
                                if (sspcList.Count != 0)
                                    pa.Weight = i + sspcList.Count;
                                else
                                    pa.Weight = i + 1;
                                _paper.SavePaperAssessment(pa);

                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < slpoList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_Question cquestion = new CEDTS_Question();
                                    cquestion.QuestionID = slpoList[i].Info.QuestionID[j];
                                    cquestion.Interval = 0;
                                    cquestion.Sound = "";
                                    cquestion.AssessmentItemID = slpoList[i].Info.ItemID;
                                    cquestion.QuestionContent = slpoList[i].Info.Problem[j];
                                    cquestion.Duration = slpoList[i].Info.TimeQuestion[j];
                                    cquestion.Score = slpoList[i].Info.ScoreQuestion[j];
                                    cquestion.Difficult = slpoList[i].Info.DifficultQuestion[j];
                                    cquestion.ChooseA = slpoList[i].Choices[4 * j + 0];
                                    cquestion.ChooseB = slpoList[i].Choices[4 * j + 1];
                                    cquestion.ChooseC = slpoList[i].Choices[4 * j + 2];
                                    cquestion.ChooseD = slpoList[i].Choices[4 * j + 3];
                                    cquestion.Answer = slpoList[i].Info.AnswerValue[j];
                                    cquestion.Analyze = slpoList[i].Info.Tip[j];
                                    cquestion.Order = (j + 1);
                                    listquestion.Add(cquestion);
                                }
                                _question.CreateQuestion(listquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < listquestion.Count; m++)
                                    {

                                        string[] Knowledge = slpoList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                            qk.QuestionID = listquestion[m].QuestionID;
                                            qk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            qk.Weight = n + 1;
                                            _QuestionKnowledge.Create(qk);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                //暂存
                                #region 保存试题到数据库
                                CEDTS_TemporaryAssessmentItem titem = new CEDTS_TemporaryAssessmentItem();

                                titem.SaveTime = DateTime.Now;
                                titem.UpdateTime = DateTime.Now;
                                titem.AssessmentItemID = slpoList[i].Info.ItemID;
                                titem.ItemTypeID = 2;
                                titem.QuestionCount = slpoList[i].Info.QuestionCount;
                                titem.Description = "";
                                titem.Score = slpoList[i].Info.ScoreQuestion[0];
                                titem.Difficult = slpoList[i].Info.DifficultQuestion[0];
                                titem.Duration = slpoList[i].Info.TimeQuestion[0];
                                titem.Interval = int.Parse(slpoList[i].Info.QustionInterval);
                                titem.UserID = _item.SelectUserID(User.Identity.Name);
                                titem.UpdateUserID = titem.UserID;
                                titem.Count = 0;
                                titem.Unit = "";
                                titem.Course = "";
                                titem.Count = 0;
                                titem.Original = slpoList[i].Script;
                                titem.SoundFile = slpoList[i].SoundFile;
                                string name = "../../ExaminationItemLibrary/" + slpoList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                titem.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _titem.Create(titem);

                                #endregion

                                #region 保存试题和试卷的关系表
                                CEDTS_TemporaryPaperAssessment tpa = new CEDTS_TemporaryPaperAssessment();
                                tpa.PaperID = _tempPaper.PaperID;
                                tpa.AssessmentItemID = titem.AssessmentItemID;
                                tpa.ItemTypeID = 2;
                                tpa.PartTypeID = 2;
                                if (sspcList.Count != 0)
                                    tpa.Weight = i + sspcList.Count;
                                else
                                    tpa.Weight = i + 1;
                                _Tpaper.SavePaperAssessment(tpa);

                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < slpoList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_TemporaryQuestion tcquestion = new CEDTS_TemporaryQuestion();
                                    tcquestion.QuestionID = slpoList[i].Info.QuestionID[j];
                                    tcquestion.Interval = 0;
                                    tcquestion.Sound = "";
                                    tcquestion.AssessmentItemID = slpoList[i].Info.ItemID;
                                    tcquestion.QuestionContent = slpoList[i].Info.Problem[j];
                                    tcquestion.Duration = slpoList[i].Info.TimeQuestion[j];
                                    tcquestion.Score = slpoList[i].Info.ScoreQuestion[j];
                                    tcquestion.Difficult = slpoList[i].Info.DifficultQuestion[j];
                                    tcquestion.ChooseA = slpoList[i].Choices[4 * j + 0];
                                    tcquestion.ChooseB = slpoList[i].Choices[4 * j + 1];
                                    tcquestion.ChooseC = slpoList[i].Choices[4 * j + 2];
                                    tcquestion.ChooseD = slpoList[i].Choices[4 * j + 3];
                                    tcquestion.Answer = slpoList[i].Info.AnswerValue[j];
                                    tcquestion.Analyze = slpoList[i].Info.Tip[j];
                                    tcquestion.Order = (j + 1);
                                    tlistquestion.Add(tcquestion);
                                }
                                _Tquestion.CreateQuestion(tlistquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < tlistquestion.Count; m++)
                                    {

                                        string[] Knowledge = slpoList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_TemporaryQuestionKnowledge tqk = new CEDTS_TemporaryQuestionKnowledge();
                                            tqk.QuestionID = tlistquestion[m].QuestionID;
                                            tqk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            tqk.Weight = n + 1;
                                            _TQuestionKnowledge.Create(tqk);
                                        }
                                    }
                                }
                                #endregion

                            }
                        }
                    }
                    #endregion

                    #region 保存长对话听力

                    if (llpoList.Count > 0)
                    {

                        for (int i = 0; i < llpoList.Count; i++)
                        {
                            List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                            List<CEDTS_TemporaryQuestion> tlistquestion = new List<CEDTS_TemporaryQuestion>();
                            list.Add(llpoList[i].Info.ItemID);

                            _item.AddItem(llpoList[i]);//保存到XML
                            if (sa == 2)
                            {
                                #region 保存试题到数据库
                                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();

                                item.SaveTime = DateTime.Now;
                                item.UpdateTime = DateTime.Now;
                                _item.AddItem(llpoList[i]);
                                item.AssessmentItemID = llpoList[i].Info.ItemID;
                                item.SoundFile = llpoList[i].SoundFile;
                                item.ItemTypeID = 3;
                                item.QuestionCount = llpoList[i].Info.QuestionCount;
                                item.Description = "";
                                item.Score = llpoList[i].Info.Score;
                                item.Difficult = llpoList[i].Info.Diffcult;
                                item.Duration = llpoList[i].Info.ReplyTime;
                                item.Interval = int.Parse(llpoList[i].Info.QustionInterval);
                                item.UserID = _item.SelectUserID(User.Identity.Name);
                                item.UpdateUserID = item.UserID;
                                item.Count = 0;
                                item.Unit = "";
                                item.Course = "";
                                item.Original = llpoList[i].Script;
                                string name = "../../ExaminationItemLibrary/" + llpoList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _item.Create(item);
                                #endregion

                                #region 保存试卷和试题的关系表
                                CEDTS_PaperAssessment pa = new CEDTS_PaperAssessment();
                                pa.PaperID = _tempPaper.PaperID;
                                pa.AssessmentItemID = item.AssessmentItemID;
                                pa.ItemTypeID = 3;
                                pa.PartTypeID = 2;
                                if (sspcList.Count + slpoList.Count != 0)
                                    pa.Weight = i + sspcList.Count + slpoList.Count;
                                else
                                    pa.Weight = i + 1;
                                _paper.SavePaperAssessment(pa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < llpoList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_Question cquestion = new CEDTS_Question();
                                    cquestion.QuestionID = llpoList[i].Info.QuestionID[j];
                                    soundQuestion.Add(cquestion.QuestionID);
                                    cquestion.Interval = llpoList[i].Info.timeInterval[j];
                                    cquestion.Sound = cquestion.QuestionID.ToString();
                                    cquestion.AssessmentItemID = llpoList[i].Info.ItemID;
                                    cquestion.QuestionContent = llpoList[i].Info.Problem[j];
                                    cquestion.Duration = llpoList[i].Info.TimeQuestion[j];
                                    cquestion.Score = llpoList[i].Info.ScoreQuestion[j];
                                    cquestion.Difficult = llpoList[i].Info.DifficultQuestion[j];
                                    cquestion.ChooseA = llpoList[i].Choices[4 * j + 0];
                                    cquestion.ChooseB = llpoList[i].Choices[4 * j + 1];
                                    cquestion.ChooseC = llpoList[i].Choices[4 * j + 2];
                                    cquestion.ChooseD = llpoList[i].Choices[4 * j + 3];
                                    cquestion.Answer = llpoList[i].Info.AnswerValue[j];
                                    cquestion.Analyze = llpoList[i].Info.Tip[j];
                                    cquestion.Order = (j + 1);
                                    listquestion.Add(cquestion);
                                }
                                _question.CreateQuestion(listquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < listquestion.Count; m++)
                                    {
                                        string[] Knowledge = llpoList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                            qk.QuestionID = listquestion[m].QuestionID;
                                            qk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            qk.Weight = n + 1;
                                            _QuestionKnowledge.Create(qk);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else//暂存临时表
                            {
                                #region 保存试题到数据库
                                CEDTS_TemporaryAssessmentItem titem = new CEDTS_TemporaryAssessmentItem();

                                titem.SaveTime = DateTime.Now;
                                titem.UpdateTime = DateTime.Now;
                                titem.AssessmentItemID = llpoList[i].Info.ItemID;
                                titem.ItemTypeID = 3;
                                titem.QuestionCount = llpoList[i].Info.QuestionCount;
                                titem.Description = "";
                                titem.Score = llpoList[i].Info.Score;
                                titem.Difficult = llpoList[i].Info.Diffcult;
                                titem.Duration = llpoList[i].Info.ReplyTime;
                                titem.Interval = int.Parse(llpoList[i].Info.QustionInterval);
                                titem.UserID = _item.SelectUserID(User.Identity.Name);
                                titem.UpdateUserID = titem.UserID;
                                titem.Count = 0;
                                titem.Unit = "";
                                titem.Course = "";
                                titem.Original = llpoList[i].Script;
                                titem.SoundFile = llpoList[i].SoundFile;
                                string name = "../../ExaminationItemLibrary/" + llpoList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                titem.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _titem.Create(titem);
                                #endregion

                                #region 保存试卷和试题的关系表
                                CEDTS_TemporaryPaperAssessment tpa = new CEDTS_TemporaryPaperAssessment();
                                tpa.PaperID = _tempPaper.PaperID;
                                tpa.AssessmentItemID = titem.AssessmentItemID;
                                tpa.ItemTypeID = 3;
                                tpa.PartTypeID = 2;
                                if (sspcList.Count + slpoList.Count != 0)
                                    tpa.Weight = i + sspcList.Count + slpoList.Count;
                                else
                                    tpa.Weight = i + 1;
                                _Tpaper.SavePaperAssessment(tpa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < llpoList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_TemporaryQuestion tcquestion = new CEDTS_TemporaryQuestion();
                                    tcquestion.QuestionID = llpoList[i].Info.QuestionID[j];
                                    soundQuestion.Add(tcquestion.QuestionID);
                                    tcquestion.Interval = llpoList[i].Info.timeInterval[j];
                                    tcquestion.Sound = tcquestion.QuestionID.ToString();
                                    tcquestion.AssessmentItemID = llpoList[i].Info.ItemID;
                                    tcquestion.QuestionContent = llpoList[i].Info.Problem[j];
                                    tcquestion.Duration = llpoList[i].Info.TimeQuestion[j];
                                    tcquestion.Score = llpoList[i].Info.ScoreQuestion[j];
                                    tcquestion.Difficult = llpoList[i].Info.DifficultQuestion[j];
                                    tcquestion.ChooseA = llpoList[i].Choices[4 * j + 0];
                                    tcquestion.ChooseB = llpoList[i].Choices[4 * j + 1];
                                    tcquestion.ChooseC = llpoList[i].Choices[4 * j + 2];
                                    tcquestion.ChooseD = llpoList[i].Choices[4 * j + 3];
                                    tcquestion.Answer = llpoList[i].Info.AnswerValue[j];
                                    tcquestion.Analyze = llpoList[i].Info.Tip[j];
                                    tcquestion.Order = (j + 1);
                                    tlistquestion.Add(tcquestion);
                                }
                                _Tquestion.CreateQuestion(tlistquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < tlistquestion.Count; m++)
                                    {
                                        string[] Knowledge = llpoList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_TemporaryQuestionKnowledge tqk = new CEDTS_TemporaryQuestionKnowledge();
                                            tqk.QuestionID = tlistquestion[m].QuestionID;
                                            tqk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            tqk.Weight = n + 1;
                                            _TQuestionKnowledge.Create(tqk);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion

                    #region 保存听力理解

                    if (rlpoList.Count > 0)
                    {

                        for (int i = 0; i < rlpoList.Count; i++)
                        {
                            List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                            List<CEDTS_TemporaryQuestion> tlistquestion = new List<CEDTS_TemporaryQuestion>();
                            list.Add(rlpoList[i].Info.ItemID);

                            _item.AddItem(rlpoList[i]);//保存到XML

                            if (sa == 2)
                            {
                                #region 保存试题到数据库
                                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                                item.SaveTime = DateTime.Now;
                                item.UpdateTime = DateTime.Now;
                                item.AssessmentItemID = rlpoList[i].Info.ItemID;
                                item.SoundFile = rlpoList[i].SoundFile;
                                item.ItemTypeID = 4;
                                item.QuestionCount = rlpoList[i].Info.QuestionCount;
                                item.Description = "";
                                item.Score = rlpoList[i].Info.Score;
                                item.Difficult = rlpoList[i].Info.Diffcult;
                                item.Duration = rlpoList[i].Info.ReplyTime;
                                item.Interval = int.Parse(rlpoList[i].Info.QustionInterval);
                                item.UserID = _item.SelectUserID(User.Identity.Name);
                                item.UpdateUserID = item.UserID;
                                item.Count = 0;
                                item.Unit = "";
                                item.Course = "";
                                item.Original = rlpoList[i].Script;
                                string name = "../../ExaminationItemLibrary/" + rlpoList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _item.Create(item);

                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_PaperAssessment pa = new CEDTS_PaperAssessment();
                                pa.PaperID = _tempPaper.PaperID;
                                pa.AssessmentItemID = item.AssessmentItemID;
                                pa.ItemTypeID = 4;
                                pa.PartTypeID = 2;
                                if (sspcList.Count + slpoList.Count + llpoList.Count != 0)
                                    pa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count;
                                else
                                    pa.Weight = i + 1;
                                _paper.SavePaperAssessment(pa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < rlpoList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_Question cquestion = new CEDTS_Question();
                                    cquestion.QuestionID = rlpoList[i].Info.QuestionID[j];
                                    soundQuestion.Add(cquestion.QuestionID);
                                    cquestion.Interval = rlpoList[i].Info.timeInterval[j];
                                    cquestion.Sound = cquestion.QuestionID.ToString();
                                    cquestion.AssessmentItemID = rlpoList[i].Info.ItemID;
                                    cquestion.QuestionContent = rlpoList[i].Info.Problem[j];
                                    cquestion.Duration = rlpoList[i].Info.TimeQuestion[j];
                                    cquestion.Score = rlpoList[i].Info.ScoreQuestion[j];
                                    cquestion.Difficult = rlpoList[i].Info.DifficultQuestion[j];
                                    cquestion.ChooseA = rlpoList[i].Choices[4 * j + 0];
                                    cquestion.ChooseB = rlpoList[i].Choices[4 * j + 1];
                                    cquestion.ChooseC = rlpoList[i].Choices[4 * j + 2];
                                    cquestion.ChooseD = rlpoList[i].Choices[4 * j + 3];
                                    cquestion.Answer = rlpoList[i].Info.AnswerValue[j];
                                    cquestion.Analyze = rlpoList[i].Info.Tip[j];
                                    cquestion.Order = (j + 1);
                                    listquestion.Add(cquestion);
                                }
                                _question.CreateQuestion(listquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < listquestion.Count; m++)
                                    {
                                        string[] Knowledge = rlpoList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                            qk.QuestionID = listquestion[m].QuestionID;
                                            qk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            qk.Weight = n + 1;
                                            _QuestionKnowledge.Create(qk);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else//暂存临时表
                            {
                                #region 保存试题到数据库
                                CEDTS_TemporaryAssessmentItem titem = new CEDTS_TemporaryAssessmentItem();
                                titem.SaveTime = DateTime.Now;
                                titem.UpdateTime = DateTime.Now;
                                titem.AssessmentItemID = rlpoList[i].Info.ItemID;
                                titem.ItemTypeID = 4;
                                titem.QuestionCount = rlpoList[i].Info.QuestionCount;
                                titem.Description = "";
                                titem.Score = rlpoList[i].Info.Score;
                                titem.Difficult = rlpoList[i].Info.Diffcult;
                                titem.Duration = rlpoList[i].Info.ReplyTime;
                                titem.Interval = int.Parse(rlpoList[i].Info.QustionInterval);
                                titem.UserID = _item.SelectUserID(User.Identity.Name);
                                titem.UpdateUserID = titem.UserID;
                                titem.Count = 0;
                                titem.Unit = "";
                                titem.Course = "";
                                titem.Original = rlpoList[i].Script;
                                titem.SoundFile = rlpoList[i].SoundFile;
                                string name = "../../ExaminationItemLibrary/" + rlpoList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                titem.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _titem.Create(titem);

                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_TemporaryPaperAssessment tpa = new CEDTS_TemporaryPaperAssessment();
                                tpa.PaperID = _tempPaper.PaperID;
                                tpa.AssessmentItemID = titem.AssessmentItemID;
                                tpa.ItemTypeID = 4;
                                tpa.PartTypeID = 2;
                                if (sspcList.Count + slpoList.Count + llpoList.Count != 0)
                                    tpa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count;
                                else
                                    tpa.Weight = i + 1;
                                _Tpaper.SavePaperAssessment(tpa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < rlpoList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_TemporaryQuestion tcquestion = new CEDTS_TemporaryQuestion();
                                    tcquestion.QuestionID = rlpoList[i].Info.QuestionID[j];
                                    soundQuestion.Add(tcquestion.QuestionID);
                                    tcquestion.Interval = rlpoList[i].Info.timeInterval[j];
                                    tcquestion.Sound = tcquestion.QuestionID.ToString();
                                    tcquestion.AssessmentItemID = rlpoList[i].Info.ItemID;
                                    tcquestion.QuestionContent = rlpoList[i].Info.Problem[j];
                                    tcquestion.Duration = rlpoList[i].Info.TimeQuestion[j];
                                    tcquestion.Score = rlpoList[i].Info.ScoreQuestion[j];
                                    tcquestion.Difficult = rlpoList[i].Info.DifficultQuestion[j];
                                    tcquestion.ChooseA = rlpoList[i].Choices[4 * j + 0];
                                    tcquestion.ChooseB = rlpoList[i].Choices[4 * j + 1];
                                    tcquestion.ChooseC = rlpoList[i].Choices[4 * j + 2];
                                    tcquestion.ChooseD = rlpoList[i].Choices[4 * j + 3];
                                    tcquestion.Answer = rlpoList[i].Info.AnswerValue[j];
                                    tcquestion.Analyze = rlpoList[i].Info.Tip[j];
                                    tcquestion.Order = (j + 1);
                                    tlistquestion.Add(tcquestion);
                                }
                                _Tquestion.CreateQuestion(tlistquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < tlistquestion.Count; m++)
                                    {
                                        string[] Knowledge = rlpoList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_TemporaryQuestionKnowledge tqk = new CEDTS_TemporaryQuestionKnowledge();
                                            tqk.QuestionID = tlistquestion[m].QuestionID;
                                            tqk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            tqk.Weight = n + 1;
                                            _TQuestionKnowledge.Create(tqk);
                                        }
                                    }
                                }
                                #endregion
                            }


                        }
                    }
                    #endregion

                    #region 保存复合型听力

                    if (lpcList.Count > 0)
                    {

                        for (int i = 0; i < lpcList.Count; i++)
                        {
                            List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                            List<CEDTS_TemporaryQuestion> tlistquestion = new List<CEDTS_TemporaryQuestion>();
                            list.Add(lpcList[i].Info.ItemID);

                            _item.AddComplexItem(lpcList[i]);//保存到XML
                            if (sa == 2)
                            {
                                #region 保存试题到数据库
                                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                                item.AssessmentItemID = lpcList[i].Info.ItemID;
                                item.SoundFile = lpcList[i].SoundFile;
                                item.ItemTypeID = 5;
                                item.QuestionCount = lpcList[i].Info.QuestionCount;
                                item.Description = "";
                                item.Difficult = lpcList[i].Info.Diffcult;
                                item.Duration = lpcList[i].Info.ReplyTime;
                                item.SaveTime = DateTime.Now;
                                item.Score = lpcList[i].Info.Score;
                                item.UpdateTime = DateTime.Now;
                                item.Interval = int.Parse(lpcList[i].Info.QustionInterval);
                                item.UserID = _item.SelectUserID(User.Identity.Name);
                                item.UpdateUserID = item.UserID;
                                item.Count = 0;
                                item.Unit = "";
                                item.Course = "";
                                item.Original = lpcList[i].Script;
                                string name = "../../ExaminationItemLibrary/" + lpcList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _item.Create(item);
                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_PaperAssessment pa = new CEDTS_PaperAssessment();
                                pa.PaperID = _tempPaper.PaperID;
                                pa.AssessmentItemID = item.AssessmentItemID;
                                pa.ItemTypeID = 5;
                                pa.PartTypeID = 2;
                                if (sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count != 0)
                                    pa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count;
                                else
                                    pa.Weight = i + 1;
                                _paper.SavePaperAssessment(pa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < lpcList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_Question cquestion = new CEDTS_Question();
                                    cquestion.QuestionID = lpcList[i].Info.QuestionID[j];
                                    cquestion.AssessmentItemID = lpcList[i].Info.ItemID;
                                    cquestion.Sound = "";
                                    cquestion.Interval = 0;
                                    cquestion.Duration = lpcList[i].Info.TimeQuestion[j];
                                    cquestion.Score = lpcList[i].Info.ScoreQuestion[j];
                                    cquestion.Difficult = lpcList[i].Info.DifficultQuestion[j];
                                    cquestion.Answer = lpcList[i].Info.AnswerValue[j];
                                    cquestion.Analyze = lpcList[i].Info.Tip[j];
                                    cquestion.Order = (j + 1);
                                    listquestion.Add(cquestion);
                                }
                                _question.CreateQuestion(listquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < listquestion.Count; m++)
                                    {
                                        string[] Knowledge = lpcList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                            qk.QuestionID = listquestion[m].QuestionID;
                                            qk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            qk.Weight = n + 1;
                                            _QuestionKnowledge.Create(qk);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else//暂存临时表
                            {
                                #region 保存试题到数据库
                                CEDTS_TemporaryAssessmentItem titem = new CEDTS_TemporaryAssessmentItem();
                                titem.AssessmentItemID = lpcList[i].Info.ItemID;
                                titem.ItemTypeID = 5;
                                titem.QuestionCount = lpcList[i].Info.QuestionCount;
                                titem.Description = "";
                                titem.Difficult = lpcList[i].Info.Diffcult;
                                titem.Duration = lpcList[i].Info.ReplyTime;
                                titem.SaveTime = DateTime.Now;
                                titem.Score = lpcList[i].Info.Score;
                                titem.UpdateTime = DateTime.Now;
                                titem.Interval = int.Parse(lpcList[i].Info.QustionInterval);
                                titem.UserID = _item.SelectUserID(User.Identity.Name);
                                titem.UpdateUserID = titem.UserID;
                                titem.Count = 0;
                                titem.Unit = "";
                                titem.Course = "";
                                titem.Original = lpcList[i].Script;
                                titem.SoundFile = lpcList[i].SoundFile;
                                string name = "../../ExaminationItemLibrary/" + lpcList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                titem.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _titem.Create(titem);
                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_TemporaryPaperAssessment tpa = new CEDTS_TemporaryPaperAssessment();
                                tpa.PaperID = _tempPaper.PaperID;
                                tpa.AssessmentItemID = titem.AssessmentItemID;
                                tpa.ItemTypeID = 5;
                                tpa.PartTypeID = 2;
                                if (sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count != 0)
                                    tpa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count;
                                else
                                    tpa.Weight = i + 1;
                                _Tpaper.SavePaperAssessment(tpa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < lpcList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_TemporaryQuestion tcquestion = new CEDTS_TemporaryQuestion();
                                    tcquestion.QuestionID = lpcList[i].Info.QuestionID[j];
                                    tcquestion.AssessmentItemID = lpcList[i].Info.ItemID;
                                    tcquestion.Sound = "";
                                    tcquestion.Interval = 0;
                                    tcquestion.Duration = lpcList[i].Info.TimeQuestion[j];
                                    tcquestion.Score = lpcList[i].Info.ScoreQuestion[j];
                                    tcquestion.Difficult = lpcList[i].Info.DifficultQuestion[j];
                                    tcquestion.Answer = lpcList[i].Info.AnswerValue[j];
                                    tcquestion.Analyze = lpcList[i].Info.Tip[j];
                                    tcquestion.Order = (j + 1);
                                    tlistquestion.Add(tcquestion);
                                }
                                _Tquestion.CreateQuestion(tlistquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < tlistquestion.Count; m++)
                                    {
                                        string[] Knowledge = lpcList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_TemporaryQuestionKnowledge tqk = new CEDTS_TemporaryQuestionKnowledge();
                                            tqk.QuestionID = tlistquestion[m].QuestionID;
                                            tqk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            tqk.Weight = n + 1;
                                            _TQuestionKnowledge.Create(tqk);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion

                    #region 保存阅读理解选词填空
                    if (rpcList.Count > 0)
                    {

                        for (int i = 0; i < rpcList.Count; i++)
                        {
                            List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                            List<CEDTS_TemporaryQuestion> tlistquestion = new List<CEDTS_TemporaryQuestion>();
                            _item.AddRpcItem(rpcList[i]);//保存到XML

                            if (sa == 2)
                            {
                                #region 保存试题到数据库
                                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                                item.AssessmentItemID = rpcList[i].Info.ItemID;
                                item.ItemTypeID = 6;
                                item.QuestionCount = rpcList[i].Info.QuestionCount;
                                item.Description = "";
                                item.Difficult = rpcList[i].Info.Diffcult;
                                item.Duration = rpcList[i].Info.ReplyTime;
                                item.SaveTime = DateTime.Now;
                                item.Score = rpcList[i].Info.Score;
                                item.UpdateTime = DateTime.Now;
                                item.Count = 0;
                                item.Original = rpcList[i].Content;
                                item.UserID = UserID;
                                item.UpdateUserID = UserID;
                                string name = "../../ExaminationItemLibrary/" + rpcList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _item.Create(item);
                                #endregion

                                #region 保存选项内容到数据库
                                CEDTS_Expansion Expansion = new CEDTS_Expansion();
                                Expansion.AssessmentItemID = rpcList[i].Info.ItemID;
                                Expansion.ChoiceA = rpcList[i].WordList[0];
                                Expansion.ChoiceB = rpcList[i].WordList[1];
                                Expansion.ChoiceC = rpcList[i].WordList[2];
                                Expansion.ChoiceD = rpcList[i].WordList[3];
                                Expansion.ChoiceE = rpcList[i].WordList[4];
                                Expansion.ChoiceF = rpcList[i].WordList[5];
                                Expansion.ChoiceG = rpcList[i].WordList[6];
                                Expansion.ChoiceH = rpcList[i].WordList[7];
                                Expansion.ChoiceI = rpcList[i].WordList[8];
                                Expansion.ChoiceJ = rpcList[i].WordList[9];
                                Expansion.ChoiceK = rpcList[i].WordList[10];
                                Expansion.ChoiceL = rpcList[i].WordList[11];
                                Expansion.ChoiceM = rpcList[i].WordList[12];
                                Expansion.ChoiceN = rpcList[i].WordList[13];
                                Expansion.ChoiceO = rpcList[i].WordList[14];
                                _Expansion.CreateExpansion(Expansion);
                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_PaperAssessment pa = new CEDTS_PaperAssessment();
                                pa.PaperID = _tempPaper.PaperID;
                                pa.AssessmentItemID = item.AssessmentItemID;
                                pa.ItemTypeID = 6;
                                pa.PartTypeID = 3;
                                if (sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count != 0)
                                    pa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count;
                                else
                                    pa.Weight = i + 1;
                                _paper.SavePaperAssessment(pa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < rpcList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_Question cquestion = new CEDTS_Question();
                                    cquestion.QuestionID = rpcList[i].Info.QuestionID[j];
                                    cquestion.AssessmentItemID = rpcList[i].Info.ItemID;
                                    cquestion.Duration = rpcList[i].Info.TimeQuestion[j];
                                    cquestion.Score = rpcList[i].Info.ScoreQuestion[j];
                                    cquestion.Difficult = rpcList[i].Info.DifficultQuestion[j];
                                    cquestion.Answer = rpcList[i].Info.AnswerValue[j];
                                    cquestion.Analyze = rpcList[i].Info.Tip[j];
                                    cquestion.Order = (j + 1);
                                    listquestion.Add(cquestion);
                                }
                                _question.CreateQuestion(listquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < listquestion.Count; m++)
                                    {
                                        string[] Knowledge = rpcList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                            qk.QuestionID = listquestion[m].QuestionID;
                                            qk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            qk.Weight = n + 1;
                                            _QuestionKnowledge.Create(qk);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else//暂存临时表
                            {
                                #region 保存试题到数据库
                                CEDTS_TemporaryAssessmentItem titem = new CEDTS_TemporaryAssessmentItem();
                                titem.AssessmentItemID = rpcList[i].Info.ItemID;
                                titem.ItemTypeID = 6;
                                titem.QuestionCount = rpcList[i].Info.QuestionCount;
                                titem.Description = "";
                                titem.Difficult = rpcList[i].Info.Diffcult;
                                titem.Duration = rpcList[i].Info.ReplyTime;
                                titem.SaveTime = DateTime.Now;
                                titem.Score = rpcList[i].Info.Score;
                                titem.UpdateTime = DateTime.Now;
                                titem.Count = 0;
                                titem.Original = rpcList[i].Content;
                                titem.UserID = UserID;
                                titem.UpdateUserID = UserID;
                                string name = "../../ExaminationItemLibrary/" + rpcList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                titem.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _titem.Create(titem);
                                #endregion

                                #region 保存选项内容到数据库
                                CEDTS_TemporaryExpansion tExpansion = new CEDTS_TemporaryExpansion();
                                tExpansion.AssessmentItemID = rpcList[i].Info.ItemID;
                                tExpansion.ChoiceA = rpcList[i].WordList[0];
                                tExpansion.ChoiceB = rpcList[i].WordList[1];
                                tExpansion.ChoiceC = rpcList[i].WordList[2];
                                tExpansion.ChoiceD = rpcList[i].WordList[3];
                                tExpansion.ChoiceE = rpcList[i].WordList[4];
                                tExpansion.ChoiceF = rpcList[i].WordList[5];
                                tExpansion.ChoiceG = rpcList[i].WordList[6];
                                tExpansion.ChoiceH = rpcList[i].WordList[7];
                                tExpansion.ChoiceI = rpcList[i].WordList[8];
                                tExpansion.ChoiceJ = rpcList[i].WordList[9];
                                tExpansion.ChoiceK = rpcList[i].WordList[10];
                                tExpansion.ChoiceL = rpcList[i].WordList[11];
                                tExpansion.ChoiceM = rpcList[i].WordList[12];
                                tExpansion.ChoiceN = rpcList[i].WordList[13];
                                tExpansion.ChoiceO = rpcList[i].WordList[14];
                                _TExpansion.CreateExpansion(tExpansion);
                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_TemporaryPaperAssessment tpa = new CEDTS_TemporaryPaperAssessment();
                                tpa.PaperID = _tempPaper.PaperID;
                                tpa.AssessmentItemID = titem.AssessmentItemID;
                                tpa.ItemTypeID = 6;
                                tpa.PartTypeID = 3;
                                if (sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count != 0)
                                    tpa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count;
                                else
                                    tpa.Weight = i + 1;
                                _Tpaper.SavePaperAssessment(tpa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < rpcList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_TemporaryQuestion tcquestion = new CEDTS_TemporaryQuestion();
                                    tcquestion.QuestionID = rpcList[i].Info.QuestionID[j];
                                    tcquestion.AssessmentItemID = rpcList[i].Info.ItemID;
                                    tcquestion.Duration = rpcList[i].Info.TimeQuestion[j];
                                    tcquestion.Score = rpcList[i].Info.ScoreQuestion[j];
                                    tcquestion.Difficult = rpcList[i].Info.DifficultQuestion[j];
                                    tcquestion.Answer = rpcList[i].Info.AnswerValue[j];
                                    tcquestion.Analyze = rpcList[i].Info.Tip[j];
                                    tcquestion.Order = (j + 1);
                                    tlistquestion.Add(tcquestion);
                                }
                                _Tquestion.CreateQuestion(tlistquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < tlistquestion.Count; m++)
                                    {
                                        string[] Knowledge = rpcList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_TemporaryQuestionKnowledge tqk = new CEDTS_TemporaryQuestionKnowledge();
                                            tqk.QuestionID = tlistquestion[m].QuestionID;
                                            tqk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            tqk.Weight = n + 1;
                                            _TQuestionKnowledge.Create(tqk);
                                        }
                                    }
                                }
                                #endregion
                            }

                        }
                    }
                    #endregion

                    #region 保存阅读理解选择题型

                    if (rpoList.Count > 0)
                    {

                        for (int i = 0; i < rpoList.Count; i++)
                        {
                            List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                            List<CEDTS_TemporaryQuestion> tlistquestion = new List<CEDTS_TemporaryQuestion>();
                            _item.AddIntensiveRead(rpoList[i]);//保存到XML
                            if (sa == 2)
                            {
                                #region 保存试题到数据库
                                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                                item.AssessmentItemID = rpoList[i].Info.ItemID;
                                item.ItemTypeID = 7;
                                item.QuestionCount = rpoList[i].Info.QuestionCount;
                                item.Description = "";
                                item.Difficult = rpoList[i].Info.Diffcult;
                                item.Duration = rpoList[i].Info.ReplyTime;
                                item.SaveTime = DateTime.Now;
                                item.Score = rpoList[i].Info.Score;
                                item.UpdateTime = DateTime.Now;
                                item.Count = 0;
                                item.Original = rpoList[i].Content;
                                item.UserID = UserID;
                                item.UpdateUserID = UserID;
                                string name = "../../ExaminationItemLibrary/" + rpoList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _item.Create(item);
                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_PaperAssessment pa = new CEDTS_PaperAssessment();
                                pa.PaperID = _tempPaper.PaperID;
                                pa.AssessmentItemID = item.AssessmentItemID;
                                pa.ItemTypeID = 7;
                                pa.PartTypeID = 3;
                                if (sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count != 0)
                                    pa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count;
                                else
                                    pa.Weight = i + 1;
                                _paper.SavePaperAssessment(pa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < rpoList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_Question cquestion = new CEDTS_Question();
                                    cquestion.QuestionID = rpoList[i].Info.QuestionID[j];
                                    cquestion.AssessmentItemID = rpoList[i].Info.ItemID;
                                    cquestion.QuestionContent = rpoList[i].Info.Problem[j];
                                    cquestion.ChooseA = rpoList[i].Choices[4 * j + 0];
                                    cquestion.ChooseB = rpoList[i].Choices[4 * j + 1];
                                    cquestion.ChooseC = rpoList[i].Choices[4 * j + 2];
                                    cquestion.ChooseD = rpoList[i].Choices[4 * j + 3];
                                    cquestion.Answer = rpoList[i].Info.AnswerValue[j];
                                    cquestion.Duration = rpoList[i].Info.TimeQuestion[j];
                                    cquestion.Score = rpoList[i].Info.ScoreQuestion[j];
                                    cquestion.Difficult = rpoList[i].Info.DifficultQuestion[j];
                                    cquestion.Analyze = rpoList[i].Info.Tip[j];
                                    cquestion.Order = (j + 1);
                                    listquestion.Add(cquestion);
                                }
                                _question.CreateQuestion(listquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < listquestion.Count; m++)
                                    {
                                        string[] Knowledge = rpoList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                            qk.QuestionID = listquestion[m].QuestionID;
                                            qk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            qk.Weight = n + 1;
                                            _QuestionKnowledge.Create(qk);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else//暂存临时表
                            {
                                #region 保存试题到数据库
                                CEDTS_TemporaryAssessmentItem titem = new CEDTS_TemporaryAssessmentItem();
                                titem.AssessmentItemID = rpoList[i].Info.ItemID;
                                titem.ItemTypeID = 7;
                                titem.QuestionCount = rpoList[i].Info.QuestionCount;
                                titem.Description = "";
                                titem.Difficult = rpoList[i].Info.Diffcult;
                                titem.Duration = rpoList[i].Info.ReplyTime;
                                titem.SaveTime = DateTime.Now;
                                titem.Score = rpoList[i].Info.Score;
                                titem.UpdateTime = DateTime.Now;
                                titem.Count = 0;
                                titem.Original = rpoList[i].Content;
                                titem.UserID = UserID;
                                titem.UpdateUserID = UserID;
                                string name = "../../ExaminationItemLibrary/" + rpoList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                titem.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _titem.Create(titem);
                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_TemporaryPaperAssessment tpa = new CEDTS_TemporaryPaperAssessment();
                                tpa.PaperID = _tempPaper.PaperID;
                                tpa.AssessmentItemID = titem.AssessmentItemID;
                                tpa.ItemTypeID = 7;
                                tpa.PartTypeID = 3;
                                if (sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count != 0)
                                    tpa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count;
                                else
                                    tpa.Weight = i + 1;
                                _Tpaper.SavePaperAssessment(tpa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < rpoList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_TemporaryQuestion tcquestion = new CEDTS_TemporaryQuestion();
                                    tcquestion.QuestionID = rpoList[i].Info.QuestionID[j];
                                    tcquestion.AssessmentItemID = rpoList[i].Info.ItemID;
                                    tcquestion.QuestionContent = rpoList[i].Info.Problem[j];
                                    tcquestion.ChooseA = rpoList[i].Choices[4 * j + 0];
                                    tcquestion.ChooseB = rpoList[i].Choices[4 * j + 1];
                                    tcquestion.ChooseC = rpoList[i].Choices[4 * j + 2];
                                    tcquestion.ChooseD = rpoList[i].Choices[4 * j + 3];
                                    tcquestion.Answer = rpoList[i].Info.AnswerValue[j];
                                    tcquestion.Duration = rpoList[i].Info.TimeQuestion[j];
                                    tcquestion.Score = rpoList[i].Info.ScoreQuestion[j];
                                    tcquestion.Difficult = rpoList[i].Info.DifficultQuestion[j];
                                    tcquestion.Analyze = rpoList[i].Info.Tip[j];
                                    tcquestion.Order = (j + 1);
                                    tlistquestion.Add(tcquestion);
                                }
                                _Tquestion.CreateQuestion(tlistquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < tlistquestion.Count; m++)
                                    {
                                        string[] Knowledge = rpoList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_TemporaryQuestionKnowledge tqk = new CEDTS_TemporaryQuestionKnowledge();
                                            tqk.QuestionID = tlistquestion[m].QuestionID;
                                            tqk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            tqk.Weight = n + 1;
                                            _TQuestionKnowledge.Create(tqk);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }

                    #endregion

                    #region 保存信息匹配
                    if (infoMatList.Count > 0)
                    {

                        for (int i = 0; i < infoMatList.Count; i++)
                        {
                            List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                            List<CEDTS_TemporaryQuestion> tlistquestion = new List<CEDTS_TemporaryQuestion>();
                            _item.AddInfMItem(infoMatList[i]);//保存到XML

                            if (sa == 2)
                            {
                                #region 保存试题到数据库
                                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                                item.AssessmentItemID = infoMatList[i].Info.ItemID;
                                item.ItemTypeID = 9;
                                item.QuestionCount = infoMatList[i].Info.QuestionCount;
                                item.Description = "";
                                item.Difficult = infoMatList[i].Info.Diffcult;
                                item.Duration = infoMatList[i].Info.ReplyTime;
                                item.SaveTime = DateTime.Now;
                                item.Score = infoMatList[i].Info.Score;
                                item.UpdateTime = DateTime.Now;
                                item.Count = 0;
                                item.Original = infoMatList[i].Content;
                                item.UserID = UserID;
                                item.UpdateUserID = UserID;
                                string name = "../../ExaminationItemLibrary/" + infoMatList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _item.Create(item);
                                #endregion

                                #region 保存选项内容到数据库
                                CEDTS_Expansion Expansion = new CEDTS_Expansion();
                                Expansion.AssessmentItemID = infoMatList[i].Info.ItemID;
                                Expansion.ChoiceA = infoMatList[i].WordList[0];
                                Expansion.ChoiceB = infoMatList[i].WordList[1];
                                Expansion.ChoiceC = infoMatList[i].WordList[2];
                                Expansion.ChoiceD = infoMatList[i].WordList[3];
                                Expansion.ChoiceE = infoMatList[i].WordList[4];
                                Expansion.ChoiceF = infoMatList[i].WordList[5];
                                Expansion.ChoiceG = infoMatList[i].WordList[6];
                                Expansion.ChoiceH = infoMatList[i].WordList[7];
                                Expansion.ChoiceI = infoMatList[i].WordList[8];
                                Expansion.ChoiceJ = infoMatList[i].WordList[9];
                                Expansion.ChoiceK = infoMatList[i].WordList[10];
                                Expansion.ChoiceL = infoMatList[i].WordList[11];
                                Expansion.ChoiceM = infoMatList[i].WordList[12];
                                Expansion.ChoiceN = infoMatList[i].WordList[13];
                                Expansion.ChoiceO = infoMatList[i].WordList[14];
                                _Expansion.CreateExpansion(Expansion);
                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_PaperAssessment pa = new CEDTS_PaperAssessment();
                                pa.PaperID = _tempPaper.PaperID;
                                pa.AssessmentItemID = item.AssessmentItemID;
                                pa.ItemTypeID = 9;
                                pa.PartTypeID = 3;
                                if (sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count + rpoList.Count != 0)
                                    pa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count + rpoList.Count;
                                else
                                    pa.Weight = i + 1;
                                _paper.SavePaperAssessment(pa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < infoMatList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_Question cquestion = new CEDTS_Question();
                                    cquestion.QuestionID = infoMatList[i].Info.QuestionID[j];
                                    cquestion.AssessmentItemID = infoMatList[i].Info.ItemID;
                                    cquestion.Duration = infoMatList[i].Info.TimeQuestion[j];
                                    cquestion.Score = infoMatList[i].Info.ScoreQuestion[j];
                                    cquestion.Difficult = infoMatList[i].Info.DifficultQuestion[j];
                                    cquestion.Answer = infoMatList[i].Info.AnswerValue[j];
                                    cquestion.Analyze = infoMatList[i].Info.Tip[j];
                                    cquestion.Order = (j + 1);
                                    listquestion.Add(cquestion);
                                }
                                _question.CreateQuestion(listquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < listquestion.Count; m++)
                                    {
                                        string[] Knowledge = infoMatList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                            qk.QuestionID = listquestion[m].QuestionID;
                                            qk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            qk.Weight = n + 1;
                                            _QuestionKnowledge.Create(qk);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else//暂存临时表
                            {
                                #region 保存试题到数据库
                                CEDTS_TemporaryAssessmentItem titem = new CEDTS_TemporaryAssessmentItem();
                                titem.AssessmentItemID = infoMatList[i].Info.ItemID;
                                titem.ItemTypeID = 9;
                                titem.QuestionCount = infoMatList[i].Info.QuestionCount;
                                titem.Description = "";
                                titem.Difficult = infoMatList[i].Info.Diffcult;
                                titem.Duration = infoMatList[i].Info.ReplyTime;
                                titem.SaveTime = DateTime.Now;
                                titem.Score = infoMatList[i].Info.Score;
                                titem.UpdateTime = DateTime.Now;
                                titem.Count = 0;
                                titem.Original = infoMatList[i].Content;
                                string name = "../../ExaminationItemLibrary/" + infoMatList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                titem.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _titem.Create(titem);
                                #endregion

                                #region 保存选项内容到数据库
                                CEDTS_TemporaryExpansion tExpansion = new CEDTS_TemporaryExpansion();
                                tExpansion.AssessmentItemID = infoMatList[i].Info.ItemID;
                                tExpansion.ChoiceA = infoMatList[i].WordList[0];
                                tExpansion.ChoiceB = infoMatList[i].WordList[1];
                                tExpansion.ChoiceC = infoMatList[i].WordList[2];
                                tExpansion.ChoiceD = infoMatList[i].WordList[3];
                                tExpansion.ChoiceE = infoMatList[i].WordList[4];
                                tExpansion.ChoiceF = infoMatList[i].WordList[5];
                                tExpansion.ChoiceG = infoMatList[i].WordList[6];
                                tExpansion.ChoiceH = infoMatList[i].WordList[7];
                                tExpansion.ChoiceI = infoMatList[i].WordList[8];
                                tExpansion.ChoiceJ = infoMatList[i].WordList[9];
                                tExpansion.ChoiceK = infoMatList[i].WordList[10];
                                tExpansion.ChoiceL = infoMatList[i].WordList[11];
                                tExpansion.ChoiceM = infoMatList[i].WordList[12];
                                tExpansion.ChoiceN = infoMatList[i].WordList[13];
                                tExpansion.ChoiceO = infoMatList[i].WordList[14];
                                _TExpansion.CreateExpansion(tExpansion);
                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_TemporaryPaperAssessment tpa = new CEDTS_TemporaryPaperAssessment();
                                tpa.PaperID = _tempPaper.PaperID;
                                tpa.AssessmentItemID = titem.AssessmentItemID;
                                tpa.ItemTypeID = 9;
                                tpa.PartTypeID = 3;
                                if (sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count + rpoList.Count != 0)
                                    tpa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count + rpoList.Count;
                                else
                                    tpa.Weight = i + 1;
                                _Tpaper.SavePaperAssessment(tpa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < infoMatList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_TemporaryQuestion tcquestion = new CEDTS_TemporaryQuestion();
                                    tcquestion.QuestionID = infoMatList[i].Info.QuestionID[j];
                                    tcquestion.AssessmentItemID = infoMatList[i].Info.ItemID;
                                    tcquestion.Duration = infoMatList[i].Info.TimeQuestion[j];
                                    tcquestion.Score = infoMatList[i].Info.ScoreQuestion[j];
                                    tcquestion.Difficult = infoMatList[i].Info.DifficultQuestion[j];
                                    tcquestion.Answer = infoMatList[i].Info.AnswerValue[j];
                                    tcquestion.Analyze = infoMatList[i].Info.Tip[j];
                                    tcquestion.Order = (j + 1);
                                    tlistquestion.Add(tcquestion);
                                }
                                _Tquestion.CreateQuestion(tlistquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < tlistquestion.Count; m++)
                                    {
                                        string[] Knowledge = infoMatList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_TemporaryQuestionKnowledge tqk = new CEDTS_TemporaryQuestionKnowledge();
                                            tqk.QuestionID = tlistquestion[m].QuestionID;
                                            tqk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            tqk.Weight = n + 1;
                                            _TQuestionKnowledge.Create(tqk);
                                        }
                                    }
                                }
                                #endregion
                            }

                        }
                    }
                    #endregion

                    #region 保存完型填空
                    if (cpList.Count > 0)
                    {
                        for (int i = 0; i < cpList.Count; i++)
                        {
                            List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                            List<CEDTS_TemporaryQuestion> tlistquestion = new List<CEDTS_TemporaryQuestion>();
                            _item.AddClozeItem(cpList[i]);//保存到XML
                            if (sa == 2)
                            {
                                #region 保存试题到数据库
                                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                                item.AssessmentItemID = cpList[i].Info.ItemID;
                                item.ItemTypeID = 8;
                                item.QuestionCount = cpList[i].Info.QuestionCount;
                                item.Description = "";
                                item.Difficult = cpList[i].Info.Diffcult;
                                item.Duration = cpList[i].Info.ReplyTime;
                                item.SaveTime = DateTime.Now;
                                item.Original = cpList[i].Content;
                                item.Score = cpList[i].Info.Score;
                                item.Count = 0;
                                item.UpdateTime = DateTime.Now;
                                item.UserID = UserID;
                                item.UpdateUserID = UserID;
                                string name = "../../ExaminationItemLibrary/" + cpList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _item.Create(item);
                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_PaperAssessment pa = new CEDTS_PaperAssessment();
                                pa.PaperID = _tempPaper.PaperID;
                                pa.AssessmentItemID = item.AssessmentItemID;
                                pa.ItemTypeID = 8;
                                pa.PartTypeID = 4;
                                if (sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count + infoMatList.Count + rpoList.Count != 0)
                                    pa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count + infoMatList.Count + rpoList.Count;
                                else
                                    pa.Weight = i + 1;
                                _paper.SavePaperAssessment(pa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < cpList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_Question cquestion = new CEDTS_Question();
                                    cquestion.QuestionID = cpList[i].Info.QuestionID[j];
                                    cquestion.AssessmentItemID = cpList[i].Info.ItemID;
                                    cquestion.ChooseA = cpList[i].Choices[4 * j + 0];
                                    cquestion.ChooseB = cpList[i].Choices[4 * j + 1];
                                    cquestion.ChooseC = cpList[i].Choices[4 * j + 2];
                                    cquestion.ChooseD = cpList[i].Choices[4 * j + 3];
                                    cquestion.Answer = cpList[i].Info.AnswerValue[j];
                                    cquestion.Duration = cpList[i].Info.TimeQuestion[j];
                                    cquestion.Score = cpList[i].Info.ScoreQuestion[j];
                                    cquestion.Difficult = cpList[i].Info.DifficultQuestion[j];
                                    cquestion.Analyze = cpList[i].Info.Tip[j];
                                    cquestion.Order = (j + 1);
                                    listquestion.Add(cquestion);
                                }
                                _question.CreateQuestion(listquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < listquestion.Count; m++)
                                    {
                                        string[] Knowledge = cpList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                            qk.QuestionID = listquestion[m].QuestionID;
                                            qk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            qk.Weight = n + 1;
                                            _QuestionKnowledge.Create(qk);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else//暂存临时表
                            {
                                #region 保存试题到数据库
                                CEDTS_TemporaryAssessmentItem titem = new CEDTS_TemporaryAssessmentItem();
                                titem.AssessmentItemID = cpList[i].Info.ItemID;
                                titem.ItemTypeID = 8;
                                titem.QuestionCount = cpList[i].Info.QuestionCount;
                                titem.Description = "";
                                titem.Difficult = cpList[i].Info.Diffcult;
                                titem.Duration = cpList[i].Info.ReplyTime;
                                titem.SaveTime = DateTime.Now;
                                titem.Original = cpList[i].Content;
                                titem.Score = cpList[i].Info.Score;
                                titem.Count = 0;
                                titem.UpdateTime = DateTime.Now;
                                titem.UserID = UserID;
                                titem.UpdateUserID = UserID;
                                string name = "../../ExaminationItemLibrary/" + cpList[i].Info.ItemID + ".xml";
                                string MapPath = HttpContext.Server.MapPath(name);
                                XDocument doc = XDocument.Load(MapPath);
                                titem.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                _titem.Create(titem);
                                #endregion

                                #region 保存试卷和试题关系表
                                CEDTS_TemporaryPaperAssessment tpa = new CEDTS_TemporaryPaperAssessment();
                                tpa.PaperID = _tempPaper.PaperID;
                                tpa.AssessmentItemID = titem.AssessmentItemID;
                                tpa.ItemTypeID = 8;
                                tpa.PartTypeID = 4;
                                if (sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count + infoMatList.Count + rpoList.Count != 0)
                                    tpa.Weight = i + sspcList.Count + slpoList.Count + llpoList.Count + rlpoList.Count + lpcList.Count + rpcList.Count + infoMatList.Count + rpoList.Count;
                                else
                                    tpa.Weight = i + 1;
                                _Tpaper.SavePaperAssessment(tpa);
                                #endregion

                                #region 保存小问题到数据库
                                for (int j = 0; j < cpList[i].Info.QuestionCount; j++)
                                {
                                    CEDTS_TemporaryQuestion tcquestion = new CEDTS_TemporaryQuestion();
                                    tcquestion.QuestionID = cpList[i].Info.QuestionID[j];
                                    tcquestion.AssessmentItemID = cpList[i].Info.ItemID;
                                    tcquestion.ChooseA = cpList[i].Choices[4 * j + 0];
                                    tcquestion.ChooseB = cpList[i].Choices[4 * j + 1];
                                    tcquestion.ChooseC = cpList[i].Choices[4 * j + 2];
                                    tcquestion.ChooseD = cpList[i].Choices[4 * j + 3];
                                    tcquestion.Answer = cpList[i].Info.AnswerValue[j];
                                    tcquestion.Duration = cpList[i].Info.TimeQuestion[j];
                                    tcquestion.Score = cpList[i].Info.ScoreQuestion[j];
                                    tcquestion.Difficult = cpList[i].Info.DifficultQuestion[j];
                                    tcquestion.Analyze = cpList[i].Info.Tip[j];
                                    tcquestion.Order = (j + 1);
                                    tlistquestion.Add(tcquestion);
                                }
                                _Tquestion.CreateQuestion(tlistquestion);
                                #endregion

                                #region 保存小问题和知识点关系表
                                if (UserID == 1)
                                {
                                    for (int m = 0; m < tlistquestion.Count; m++)
                                    {
                                        string[] Knowledge = cpList[i].Info.KnowledgeID[m].Split(',');
                                        for (int n = 0; n < Knowledge.Length; n++)
                                        {
                                            CEDTS_TemporaryQuestionKnowledge tqk = new CEDTS_TemporaryQuestionKnowledge();
                                            tqk.QuestionID = tlistquestion[m].QuestionID;
                                            tqk.KnowledgePointID = Guid.Parse(Knowledge[n]);
                                            tqk.Weight = n + 1;
                                            _TQuestionKnowledge.Create(tqk);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion

                    #region 保存听力音频文件
                    DirectoryInfo TheFolder = new DirectoryInfo(Server.MapPath("../../TempSoundFile"));
                    string serverPath = Server.MapPath("../../SoundFile");
                    if (!Directory.Exists(serverPath))
                    {
                        Directory.CreateDirectory(serverPath);
                    }
                    foreach (FileInfo NextFile in TheFolder.GetFiles())
                    {
                        string fileName = NextFile.Name.Substring(0, NextFile.Name.LastIndexOf('.'));
                        if (fileName == PSID)
                        {
                            NextFile.MoveTo(Server.MapPath("../../SoundFile/" + _tempPaper.PaperID.ToString() + ".mp3"));
                            break;
                        }
                    }
                    if (list.Count > 0)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            foreach (FileInfo NextFile in TheFolder.GetFiles())
                            {
                                string fileName = NextFile.Name.Substring(0, NextFile.Name.LastIndexOf('.'));

                                if (fileName == list[i].ToString())
                                {
                                    NextFile.MoveTo(Server.MapPath("../../SoundFile/" + fileName + ".mp3"));
                                    break;
                                }
                            }
                        }

                    }
                    if (soundQuestion.Count > 0)
                    {
                        for (int i = 0; i < soundQuestion.Count; i++)
                        {
                            foreach (FileInfo NextFile in TheFolder.GetFiles())
                            {
                                string fileName = NextFile.Name.Substring(0, NextFile.Name.LastIndexOf('.'));
                                if (fileName == soundQuestion[i].ToString())
                                {
                                    NextFile.MoveTo(Server.MapPath("../../SoundFile/" + fileName + ".mp3"));
                                    break;
                                }
                            }
                        }
                    }
                    #endregion

                    tran.Complete();
                }
                #region 清空全局变量
                listNumber.Clear();
                listTemp.Clear();
                sspcList.Clear();
                slpoList.Clear();
                llpoList.Clear();
                rlpoList.Clear();
                lpcList.Clear();
                rpcList.Clear();
                rpoList.Clear();
                cpList.Clear();
                s = 0;
                ss = 0;
                sa = 2;
                _tempPaper = new CEDTS_Paper();
                _paperExansion = new CEDTS_PaperExpansion();
                Order = 0;
                #endregion
            }
            catch (Exception ex)
            {
                return Json(ex.Message.ToString());
            }
            return Json(1);
        }

        #endregion

        #region 试卷名称验证
        public bool AjaxTitle(string name)
        {
            return _paper.CheckName(name);
        }
        #endregion

        #region 试卷删除
        [Filter.LogFilter(Description = "删除试卷")]
        public JsonResult DeletePaper(Guid id)
        {
            return Json(_paper.DeletePaper(id));
        }
        #endregion

        #region 试卷编辑
        [Filter.LogFilter(Description = "修改试卷")]
        public ActionResult Edit(Guid id)
        {
            int UserID = _item.SelectUserID(User.Identity.Name);
            GID = id;//暂存试卷id
            PSID = GID.ToString();
            var tempPaper = _Tpaper.SelectEditPaper(id);
            _tempPaper.PaperID = tempPaper.PaperID;
            _tempPaper.PaperContent = tempPaper.PaperContent;
            _tempPaper.CreateTime = tempPaper.CreateTime;
            _tempPaper.Description = tempPaper.Description;
            _tempPaper.Difficult = tempPaper.Difficult;
            _tempPaper.Duration = tempPaper.Duration;
            _tempPaper.Score = tempPaper.Score;
            _tempPaper.State = tempPaper.State;
            _tempPaper.Title = tempPaper.Title;
            _tempPaper.Type = tempPaper.Type;
            _tempPaper.UpdateTime = tempPaper.UpdateTime;
            _tempPaper.UpdateUserID = tempPaper.UpdateUserID;
            _tempPaper.UserID = tempPaper.UserID;

            listNumber.Clear();
            listTemp.Clear();

            var tempPE = _Tpaper.SelectEditPaperExpansion(id);

            listTemp.Add(tempPE.SkimmingAndScanningNum.Value);
            listTemp.Add(tempPE.ShortListenNum.Value);
            listTemp.Add(tempPE.LongListenNum.Value);
            listTemp.Add(tempPE.ComprehensionListenNum.Value);
            listTemp.Add(tempPE.ComplexListenNum.Value);
            listTemp.Add(tempPE.BankedClozeNum.Value);
            listTemp.Add(tempPE.MultipleChoiceNum.Value);
            listTemp.Add(tempPE.InfoMatchingNum.Value);
            listTemp.Add(tempPE.ClozeNum.Value);

            s = _Tpaper.Judge(id);
            if (s == 1)
            {
                sspcList = _Tpaper.SelectEditSspc(id, UserID);
                listNumber.Add(0);
                listNumber.Add(listTemp[1]);
                listNumber.Add(listTemp[2]);
                listNumber.Add(listTemp[3]);
                listNumber.Add(listTemp[4]);
                listNumber.Add(listTemp[5]);
                listNumber.Add(listTemp[6]);
                listNumber.Add(listTemp[7]);
                listNumber.Add(listTemp[8]);
                ss = 1;
                return RedirectToAction("CreateShortListen");

            }
            if (s == 2)
            {
                sspcList = _Tpaper.SelectEditSspc(id, UserID);
                slpoList = _Tpaper.SelectEditSlpo(id, UserID);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(listTemp[2]);
                listNumber.Add(listTemp[3]);
                listNumber.Add(listTemp[4]);
                listNumber.Add(listTemp[5]);
                listNumber.Add(listTemp[6]);
                listNumber.Add(listTemp[7]);
                listNumber.Add(listTemp[8]);
                ss = 1;
                return RedirectToAction("CreateLongListen");
            }
            if (s == 3)
            {
                sspcList = _Tpaper.SelectEditSspc(id, UserID);
                slpoList = _Tpaper.SelectEditSlpo(id, UserID);
                llpoList = _Tpaper.SelectEditLlpo(id, UserID);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(listTemp[3]);
                listNumber.Add(listTemp[4]);
                listNumber.Add(listTemp[5]);
                listNumber.Add(listTemp[6]);
                listNumber.Add(listTemp[7]);
                listNumber.Add(listTemp[8]);
                ss = 1;
                return RedirectToAction("CreateComprehensionListen");
            }
            if (s == 4)
            {
                sspcList = _Tpaper.SelectEditSspc(id, UserID);
                slpoList = _Tpaper.SelectEditSlpo(id, UserID);
                llpoList = _Tpaper.SelectEditLlpo(id, UserID);
                rlpoList = _Tpaper.SelectEditRlpo(id, UserID);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(listTemp[4]);
                listNumber.Add(listTemp[5]);
                listNumber.Add(listTemp[6]);
                listNumber.Add(listTemp[7]);
                listNumber.Add(listTemp[8]);
                ss = 1;
                return RedirectToAction("CreateComplexListen");
            }
            if (s == 5)
            {
                sspcList = _Tpaper.SelectEditSspc(id, UserID);
                slpoList = _Tpaper.SelectEditSlpo(id, UserID);
                llpoList = _Tpaper.SelectEditLlpo(id, UserID);
                rlpoList = _Tpaper.SelectEditRlpo(id, UserID);
                lpcList = _Tpaper.SelectEditLpc(id, UserID);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(listTemp[5]);
                listNumber.Add(listTemp[6]);
                listNumber.Add(listTemp[7]);
                listNumber.Add(listTemp[8]);
                ss = 1;
                return RedirectToAction("CreateBankedCloze");
            }
            if (s == 6)
            {
                sspcList = _Tpaper.SelectEditSspc(id, UserID);
                slpoList = _Tpaper.SelectEditSlpo(id, UserID);
                llpoList = _Tpaper.SelectEditLlpo(id, UserID);
                rlpoList = _Tpaper.SelectEditRlpo(id, UserID);
                lpcList = _Tpaper.SelectEditLpc(id, UserID);
                rpcList = _Tpaper.SelectEditRpc(id, UserID);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(listTemp[6]);
                listNumber.Add(listTemp[7]);
                listNumber.Add(listTemp[8]);
                ss = 1;
                return RedirectToAction("CreateMultipleChoice");
            }
            if (s == 7)
            {
                sspcList = _Tpaper.SelectEditSspc(id, UserID);
                slpoList = _Tpaper.SelectEditSlpo(id, UserID);
                llpoList = _Tpaper.SelectEditLlpo(id, UserID);
                rlpoList = _Tpaper.SelectEditRlpo(id, UserID);
                lpcList = _Tpaper.SelectEditLpc(id, UserID);
                rpcList = _Tpaper.SelectEditRpc(id, UserID);
                infoMatList = _Tpaper.SlelectEditInfoMat(id, UserID);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(listTemp[7]);
                listNumber.Add(listTemp[8]);
                ss = 1;
                return RedirectToAction("CreateInfoMatching");
            }
            if (s == 8)
            {
                sspcList = _Tpaper.SelectEditSspc(id, UserID);
                slpoList = _Tpaper.SelectEditSlpo(id, UserID);
                llpoList = _Tpaper.SelectEditLlpo(id, UserID);
                rlpoList = _Tpaper.SelectEditRlpo(id, UserID);
                lpcList = _Tpaper.SelectEditLpc(id, UserID);
                rpcList = _Tpaper.SelectEditRpc(id, UserID);
                infoMatList = _Tpaper.SlelectEditInfoMat(id, UserID);
                rpoList = _Tpaper.SlelectEditRpo(id, UserID);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(0);
                listNumber.Add(listTemp[8]);
                ss = 1;
                return RedirectToAction("CreateCloze");
            }

            else
            {
                return null;
            }
        }
        #endregion

        #region 编辑上一步
        public ActionResult Up(int Up, int LastItem)
        {
            if (Up == 1)
            {
                Order = LastItem;
                string lastitemname = String.Empty;
                if (Order > 0 && Order <= 9)
                {
                    listTemp.Clear();
                    var tempPE = _Tpaper.SelectEditPaperExpansion(GID);
                    listTemp.Add(tempPE.SkimmingAndScanningNum.Value);
                    listTemp.Add(tempPE.ShortListenNum.Value);
                    listTemp.Add(tempPE.LongListenNum.Value);
                    listTemp.Add(tempPE.ComprehensionListenNum.Value);
                    listTemp.Add(tempPE.ComplexListenNum.Value);
                    listTemp.Add(tempPE.BankedClozeNum.Value);
                    listTemp.Add(tempPE.MultipleChoiceNum.Value);
                    listTemp.Add(tempPE.InfoMatchingNum.Value);
                    listTemp.Add(tempPE.ClozeNum.Value);
                    for (int i = 0; i < Order - 1; i++ )
                    {
                        listNumber.Add(0);
                    }
                    for (int i = Order - 1; i < 9; i++)
                    {
                        listNumber[i] = listTemp[i];
                    }
                }
                switch (LastItem)
                {
                    case 1:
                        lastitemname = "CreateSkimmingAndScanning";
                        break;
                    case 2:
                        lastitemname = "CreateShortListen";
                        break;
                    case 3:
                        lastitemname = "CreateLongListen";
                        break;
                    case 4:
                        lastitemname = "CreateComprehensionListen";
                        break;
                    case 5:
                        lastitemname = "CreateComplexListen";
                        break;
                    case 6:
                        lastitemname = "CreateBankedCloze";
                        break;
                    case 7:
                        lastitemname = "CreateInfoMatching";
                        break;
                    case 8:
                        lastitemname = "CreateMultipleChoice";
                        break;
                    default:
                        lastitemname = "Index";
                        break;
                }
                return Content(lastitemname);
            }
            else
            {
                return View();
            }
        }
        #endregion
    }
}
