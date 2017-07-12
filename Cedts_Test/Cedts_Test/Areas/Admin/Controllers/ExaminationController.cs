using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;
using System.Transactions;
using System.Web.Mvc.Ajax;
using System.Xml;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Cedts_Test.Areas.Admin.Controllers
{
    [Authorize]
    public class ExaminationController : Controller
    {
        ICedts_ItemRepository _item;
        ICedts_ExpansionRepository _Expansion;
        ICedts_QuestionRepository _question;
        ICedts_QuestionKnowledgeRepository _QuestionKnowledge;
        public ExaminationController(ICedts_ItemRepository i, ICedts_QuestionRepository q, ICedts_QuestionKnowledgeRepository qk, ICedts_ExpansionRepository Exp)
        {
            _item = i;
            _question = q;
            _QuestionKnowledge = qk;
            _Expansion = Exp;
        }

        public static string condition = string.Empty;
        public static string txt = string.Empty;
        public static List<string> file = new List<string>();

        /// <summary>
        /// 试题信息展示
        /// </summary>
        /// <returns>View</returns>
        [Filter.LogFilter(Description = "查看试题列表")]
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                txt = "";
                condition = "";
            }
            TempData["info"] = txt + "," + condition;
            return View(_item.SelectItemsByCondition(id, condition, txt));
        }

        [HttpPost]
        [Filter.LogFilter(Description = "搜索试题列表")]
        public ActionResult Index(int? id, FormCollection form)
        {
            id = 1;
            condition = form["condition"];
            txt = form["txtSearch"];
            TempData["info"] = txt + "," + condition;
            return View(_item.SelectItemsByCondition(id, condition, txt));
        }
        /// <summary>
        /// 新建试题首次加载中两个Dropdownlist赋值
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Create()
        {
            int? TypeID = null;
            IQueryable<CEDTS_PartType> queryResult = null;
            queryResult = _item.GetPartType();
            if (queryResult.ToList().Count == 0)
            {
                List<SelectListItem> item = new List<SelectListItem>();
                item.Insert(0, new SelectListItem { Text = "暂无元素-请添加元素", Value = "" });
                return View(item);
            }
            else
            {
                var TypeList = queryResult.ToList();
                TypeID = TypeList[0].PartTypeID;
                SelectList listItem = new SelectList(TypeList.AsEnumerable(), "PartTypeID", "TypeName_CN");
                ViewData.Add("PartType", listItem);

                SelectList itemtype = new SelectList(_item.FirstItem(TypeID).AsEnumerable(), "ItemTypeID", "TypeName_CN");
                ViewData.Add("ItemType", itemtype);
                return View();
            }
        }

        /// <summary>
        /// 新建试题表单提交后执行的Action
        /// </summary>
        /// <param name="form">表单内容</param>
        /// <returns>View</returns>

        [Filter.LogFilter(Description = "添加试题")]
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(FormCollection form)
        {
            ViewData["IsOk"] = "0";
            //获取选中的PatyTypeID
            int typename = int.Parse(form["PartType"]);
            //获取选中的ItemTypeID
            int itemname = int.Parse(form["ItemType"]);
            //获取音频文件Guid名

            List<string> file = new List<string>();
            string filename = form["filename"];

            if (filename != null)
            {
                filename = filename.Substring(0, filename.LastIndexOf(','));
                file = filename.Split(',').ToList();
            }

            #region 判断ItemID为“长对话”
            if (itemname == 3)
            {
                Listen listen = new Listen();
                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                ItemBassInfo info = new ItemBassInfo();
                info.ItemID = Guid.Parse(file[0]);
                info.ItemType = _item.ItemName(itemname);
                info.ItemType_CN = _item.ItemName_CN(itemname);
                info.PartType = _item.PartName(typename);
                info.AnswerResposn = "_";
                info.QuestionCount = int.Parse(form["LongConversations_selectnum"]);
                info.Diffcult = double.Parse(form["difficult"]);
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                info.QustionInterval = form["interval"];
                listen.SoundFile = info.ItemID.ToString();
                listen.Script = form["textarea"];
                listen.Choices = new List<string>();
                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["LongConversations_tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["LongConversations_D" + (j + 1).ToString()].ToString());
                    info.Problem.Add(form["LongConversations_Question" + (j + 1).ToString()].ToString());
                    info.timeInterval.Add(int.Parse(form["interval_" + (j + 1).ToString()]));
                    info.questionSound.Add(file[j + 1]);

                }
                for (int i = 0; i < 4 * info.QuestionCount; i++)
                {
                    listen.Choices.Add(form["LongConversations_Option" + (i + 1).ToString()].ToString());
                }
                listen.Info = info;

                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.Parse(info.questionSound[i]);
                    cquestion.Sound = info.questionSound[i];
                    cquestion.Interval = listen.Info.timeInterval[i];
                    cquestion.AssessmentItemID = listen.Info.ItemID;
                    cquestion.QuestionContent = info.Problem[i];
                    cquestion.Duration = double.Parse(form["LongConversations_timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["LongConversations_scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["LongConversations_difficultquestion" + (i + 1)]);
                    cquestion.ChooseA = form["LongConversations_Option" + ((i * 4) + 1)];
                    cquestion.ChooseB = form["LongConversations_Option" + ((i * 4) + 2)];
                    cquestion.ChooseC = form["LongConversations_Option" + ((i * 4) + 3)];
                    cquestion.ChooseD = form["LongConversations_Option" + ((i * 4) + 4)];
                    cquestion.Answer = form["LongConversations_D" + (i + 1)];
                    cquestion.Analyze = form["LongConversations_tip" + (i + 1)];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);

                }

                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {

                        _item.AddItem(listen);
                        item.AssessmentItemID = listen.Info.ItemID;
                        item.SoundFile = listen.SoundFile;
                        item.Interval = int.Parse(listen.Info.QustionInterval);
                        item.ItemTypeID = itemname;
                        item.QuestionCount = listen.Info.QuestionCount;
                        item.Description = "";
                        item.Score = listen.Info.Score;
                        item.Difficult = listen.Info.Diffcult;
                        item.Duration = listen.Info.ReplyTime;
                        item.SaveTime = DateTime.Now;
                        item.UpdateTime = DateTime.Now;
                        item.Count = 0;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.UpdateUserID = item.UserID;
                        item.Original = listen.Script;
                        string name = "../../ExaminationItemLibrary/" + listen.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();

                        _item.Create(item);
                        _question.CreateQuestion(listquestion);
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["LongConversations_hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException fileerror)
                {
                    ViewData["message"] = fileerror.Message.ToString();
                    return View("Errmessage");
                }
                catch (XmlException xml)
                {
                    ViewData["message"] = xml.Message.ToString();
                    return View("Errmessage");
                }

                catch (Exception ex)
                {
                    ViewData["message"] = ex.Message.ToString();
                    return View("Errmessage");
                }
            }

            #endregion

            #region 判断ItemID为“短对话”
            if (itemname == 2)
            {
                Listen listen = new Listen();
                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                ItemBassInfo info = new ItemBassInfo();
                info.ItemID = Guid.Parse(file[0]);
                info.ItemType = _item.ItemName(itemname);
                info.ItemType_CN = _item.ItemName_CN(itemname);
                info.PartType = _item.PartName(typename);
                info.AnswerResposn = "_";
                info.QuestionCount = 1;
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = form["interval_1"];
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                listen.Script = form["textarea"];
                listen.SoundFile = file[0];
                listen.Choices = new List<string>();
                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["ShortConversations_tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["ShortConversations_D" + (j + 1).ToString()].ToString());
                    info.Problem.Add(form["ShortConversations_Question" + (j + 1).ToString()].ToString());
                    info.questionSound.Add("");
                    info.timeInterval.Add(0);
                }

                for (int i = 0; i < 4 * info.QuestionCount; i++)
                {
                    listen.Choices.Add(form["ShortConversations_Option" + (i + 1).ToString()].ToString());
                }
                listen.Info = info;

                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.NewGuid();
                    cquestion.Sound = "";
                    cquestion.Interval = 0;
                    cquestion.AssessmentItemID = listen.Info.ItemID;
                    cquestion.QuestionContent = info.Problem[i];
                    cquestion.Duration = info.ReplyTime;
                    cquestion.Score = info.Score;
                    cquestion.Difficult = info.Diffcult;
                    cquestion.ChooseA = form["ShortConversations_Option" + ((i * 4) + 1)];
                    cquestion.ChooseB = form["ShortConversations_Option" + ((i * 4) + 2)];
                    cquestion.ChooseC = form["ShortConversations_Option" + ((i * 4) + 3)];
                    cquestion.ChooseD = form["ShortConversations_Option" + ((i * 4) + 4)];
                    cquestion.Answer = info.AnswerValue[i];
                    cquestion.Analyze = info.Tip[i];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);

                }

                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {

                        _item.AddItem(listen);
                        item.AssessmentItemID = listen.Info.ItemID;
                        item.SoundFile = listen.SoundFile;
                        item.ItemTypeID = itemname;
                        item.QuestionCount = listen.Info.QuestionCount;
                        item.Description = "";
                        item.Score = listen.Info.Score;
                        item.Difficult = listen.Info.Diffcult;
                        item.Duration = listen.Info.ReplyTime;
                        item.SaveTime = DateTime.Now;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.UpdateUserID = item.UserID;
                        item.UpdateTime = DateTime.Now;
                        item.Count = 0;
                        item.Original = listen.Script;
                        item.Unit = "";
                        item.Course = "";
                        item.Interval = int.Parse(listen.Info.QustionInterval);
                        string name = "../../ExaminationItemLibrary/" + listen.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();

                        _item.Create(item);
                        _question.CreateQuestion(listquestion);
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["ShortConversations_hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException fileerror)
                {
                    ViewData["message"] = fileerror.Message.ToString();
                    return View("Errmessage");
                }
                catch (XmlException xml)
                {
                    ViewData["message"] = xml.Message.ToString();
                    return View("Errmessage");
                }

                catch (Exception ex)
                {
                    ViewData["message"] = ex.Message.ToString();
                    return View("Errmessage");
                }
            }

            #endregion

            #region 判断ItemID为“听力短文理解”
            if (itemname == 4)
            {
                Listen listen = new Listen();
                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                ItemBassInfo info = new ItemBassInfo();
                info.ItemID = Guid.Parse(file[0]);
                info.QustionInterval = form["interval"];
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                info.ItemType = _item.ItemName(itemname);
                info.ItemType_CN = _item.ItemName_CN(itemname);
                info.PartType = _item.PartName(typename);
                info.AnswerResposn = "_";
                info.QuestionCount = int.Parse(form["Listen_selectnum"]);
                info.Diffcult = double.Parse(form["difficult"]);
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();
                info.DifficultQuestion = new List<double>();
                info.ScoreQuestion = new List<double>();
                listen.SoundFile = info.ItemID.ToString();
                listen.Script = form["textarea"];
                listen.Choices = new List<string>();
                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["Listen_tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["Listen_D" + (j + 1).ToString()].ToString());
                    info.Problem.Add(form["Listen_Question" + (j + 1).ToString()].ToString());
                    info.questionSound.Add(file[j + 1]);
                    info.timeInterval.Add(int.Parse(form["interval_" + (j + 1).ToString()]));
                }

                for (int i = 0; i < 4 * info.QuestionCount; i++)
                {
                    listen.Choices.Add(form["Listen_Option" + (i + 1).ToString()].ToString());
                }
                listen.Info = info;



                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.Parse(info.questionSound[i]);
                    cquestion.Sound = info.questionSound[i];
                    cquestion.Interval = listen.Info.timeInterval[i];
                    cquestion.AssessmentItemID = listen.Info.ItemID;
                    cquestion.QuestionContent = listen.Info.Problem[i];
                    cquestion.Duration = double.Parse(form["Listen_timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["Listen_scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["Listen_difficultquestion" + (i + 1)]);
                    cquestion.ChooseA = form["Listen_Option" + ((i * 4) + 1)];
                    cquestion.ChooseB = form["Listen_Option" + ((i * 4) + 2)];
                    cquestion.ChooseC = form["Listen_Option" + ((i * 4) + 3)];
                    cquestion.ChooseD = form["Listen_Option" + ((i * 4) + 4)];
                    cquestion.Answer = info.AnswerValue[i];
                    cquestion.Analyze = info.Tip[i];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);

                }

                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {

                        _item.AddItem(listen);
                        item.Interval = int.Parse(listen.Info.QustionInterval);
                        item.AssessmentItemID = listen.Info.ItemID;
                        item.SoundFile = listen.SoundFile;
                        item.ItemTypeID = itemname;
                        item.QuestionCount = listen.Info.QuestionCount;
                        item.Description = "";
                        item.Score = listen.Info.Score;
                        item.Difficult = listen.Info.Diffcult;
                        item.Duration = listen.Info.ReplyTime;
                        item.SaveTime = DateTime.Now;
                        item.UpdateTime = DateTime.Now;
                        item.Count = 0;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.UpdateUserID = item.UserID;
                        item.Original = listen.Script;
                        string name = "../../ExaminationItemLibrary/" + listen.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();

                        _item.Create(item);
                        _question.CreateQuestion(listquestion);
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["Listen_hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException fileerror)
                {
                    ViewData["message"] = fileerror.Message.ToString();
                    return View("Errmessage");
                }
                catch (XmlException xml)
                {
                    ViewData["message"] = xml.Message.ToString();
                    return View("Errmessage");
                }

                catch (Exception ex)
                {
                    ViewData["message"] = ex.Message.ToString();
                    return View("Errmessage");
                }
            }

            #endregion

            #region 判断ItemID为“复合型听力”

            if (itemname == 5)
            {
                Listen complex = new Listen();
                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                ItemBassInfo info = new ItemBassInfo();
                info.ItemID = Guid.Parse(file[0]);

                info.QustionInterval = form["interval"];
                info.questionSound = new List<string>();
                info.timeInterval = new List<int>();
                info.ItemType = _item.ItemName(itemname);
                info.ItemType_CN = _item.ItemName_CN(itemname);
                info.PartType = _item.PartName(typename);
                info.AnswerResposn = "_";
                info.QuestionCount = int.Parse(form["Complex_selectnum"]);
                info.Diffcult = double.Parse(form["difficult"]);
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                complex.SoundFile = info.ItemID.ToString();
                complex.Script = form["textarea"];
                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.questionSound.Add("");
                    info.timeInterval.Add(0);
                    info.Tip.Add(form["Complex_tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["Complex_textanswer" + (j + 1).ToString()].ToString());
                }
                complex.Info = info;



                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.NewGuid();
                    cquestion.Sound = "";
                    cquestion.Interval = 0;
                    cquestion.AssessmentItemID = complex.Info.ItemID;
                    cquestion.Duration = double.Parse(form["Complex_timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["Complex_scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["Complex_difficultquestion" + (i + 1)]);
                    cquestion.Answer = info.AnswerValue[i];
                    cquestion.Analyze = info.Tip[i];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }
                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        _item.AddComplexItem(complex);
                        item.Interval = int.Parse(complex.Info.QustionInterval);
                        item.AssessmentItemID = complex.Info.ItemID;
                        item.SoundFile = complex.SoundFile;
                        item.ItemTypeID = itemname;
                        item.QuestionCount = complex.Info.QuestionCount;
                        item.Description = "";
                        item.Difficult = complex.Info.Diffcult;
                        item.Duration = complex.Info.ReplyTime;
                        item.SaveTime = DateTime.Now;
                        item.Score = complex.Info.Score;
                        item.UpdateTime = DateTime.Now;
                        item.Count = 0;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.UpdateUserID = item.UserID;
                        item.Original = complex.Script;
                        string name = "../../ExaminationItemLibrary/" + complex.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();

                        _item.Create(item);
                        _question.CreateQuestion(listquestion);
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["Complex_hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }

            #endregion

            #region 判断ItemID为“快速阅读”
            if (itemname == 1)
            {
                SkimmingScanningPartCompletion Sspc = new SkimmingScanningPartCompletion();
                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                ItemBassInfo info = new ItemBassInfo();
                info.ItemID = Guid.NewGuid();
                info.ItemType = _item.ItemName(itemname);
                info.ItemType_CN = _item.ItemName_CN(itemname);
                info.PartType = _item.PartName(typename);
                info.AnswerResposn = "_";
                Sspc.ChoiceNum = int.Parse(form["SkimmingAndScanning_selectnum"]);
                Sspc.TermNum = int.Parse(form["selectnum1"]);
                info.QuestionCount = Sspc.ChoiceNum + Sspc.TermNum;
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = "";
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();

                Sspc.Content = form["textarea"];
                //Sspc.Content = Sspc.Content.Substring(Sspc.Content.IndexOf('y') + 6, Sspc.Content.LastIndexOf('y') - Sspc.Content.IndexOf('y') - 14);
                Sspc.Choices = new List<string>();
                for (int j = 0; j < Sspc.ChoiceNum; j++)
                {
                    info.Tip.Add(form["SkimmingAndScanning_tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["SkimmingAndScanning_D" + (j + 1).ToString()].ToString());
                    info.Problem.Add(form["SkimmingAndScanning_Question" + (j + 1).ToString()].ToString());
                }
                for (int i = 0; i < 4 * Sspc.ChoiceNum; i++)
                {
                    Sspc.Choices.Add(form["SkimmingAndScanning_Option" + (i + 1).ToString()].ToString());
                }

                for (int k = 0; k < Sspc.TermNum; k++)
                {
                    info.Tip.Add(form["SkimmingAndScanning_tip" + (k + Sspc.ChoiceNum + 1).ToString()]);
                    info.AnswerValue.Add(form["SkimmingAndScanning_textanswer" + (k + Sspc.ChoiceNum + 1).ToString()].ToString());
                    info.Problem.Add(form["SkimmingAndScanning_Question" + (k + Sspc.ChoiceNum + 1).ToString()].ToString());
                }
                Sspc.Info = info;



                for (int i = 0; i < Sspc.ChoiceNum; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.NewGuid();
                    cquestion.AssessmentItemID = Sspc.Info.ItemID;
                    cquestion.QuestionContent = info.Problem[i];
                    cquestion.ChooseA = form["SkimmingAndScanning_Option" + ((i * 4) + 1)];
                    cquestion.ChooseB = form["SkimmingAndScanning_Option" + ((i * 4) + 2)];
                    cquestion.ChooseC = form["SkimmingAndScanning_Option" + ((i * 4) + 3)];
                    cquestion.ChooseD = form["SkimmingAndScanning_Option" + ((i * 4) + 4)];
                    cquestion.Answer = form["SkimmingAndScanning_D" + (i + 1)];
                    cquestion.Duration = double.Parse(form["SkimmingAndScanning_timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["SkimmingAndScanning_scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["SkimmingAndScanning_difficultquestion" + (i + 1)]);
                    cquestion.Analyze = form["SkimmingAndScanning_tip" + (i + 1)];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }

                for (int i = 0; i < Sspc.TermNum; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.NewGuid();
                    cquestion.AssessmentItemID = Sspc.Info.ItemID;
                    cquestion.QuestionContent = info.Problem[i + Sspc.ChoiceNum];
                    cquestion.Answer = form["SkimmingAndScanning_textanswer" + (i + Sspc.ChoiceNum + 1)];
                    cquestion.Duration = double.Parse(form["SkimmingAndScanning_timequestion" + (i + Sspc.ChoiceNum + 1)]);
                    cquestion.Score = double.Parse(form["SkimmingAndScanning_scorequestion" + (i + Sspc.ChoiceNum + 1)]);
                    cquestion.Difficult = double.Parse(form["SkimmingAndScanning_difficultquestion" + (i + Sspc.ChoiceNum + 1)]);
                    cquestion.Analyze = form["SkimmingAndScanning_tip" + (i + Sspc.ChoiceNum + 1)];
                    cquestion.Order = (i + Sspc.ChoiceNum + 1);

                    cquestion.ChooseA = "";
                    cquestion.ChooseB = "";
                    cquestion.ChooseC = "";
                    cquestion.ChooseD = "";

                    listquestion.Add(cquestion);
                }

                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        _item.AddSspcItem(Sspc);
                        item.AssessmentItemID = Sspc.Info.ItemID;
                        item.ItemTypeID = itemname;
                        item.QuestionCount = Sspc.Info.QuestionCount;
                        item.Description = "";
                        item.Difficult = Sspc.Info.Diffcult;
                        item.Duration = Sspc.Info.ReplyTime;
                        item.SaveTime = DateTime.Now;
                        item.Original = Sspc.Content;
                        item.Score = Sspc.Info.Score;
                        item.Count = 0;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.UpdateUserID = item.UserID;
                        item.UpdateTime = DateTime.Now;
                        string name = "../../ExaminationItemLibrary/" + Sspc.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();

                        _item.Create(item);
                        _question.CreateQuestion(listquestion);
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["SkimmingAndScanning_hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }
            #endregion

            #region 判断ItemTypeID为“完形填空”
            if (itemname == 8)
            {
                ClozePart Cloze = new ClozePart();
                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                ItemBassInfo info = new ItemBassInfo();

                info.ItemID = Guid.NewGuid();
                info.ItemType = _item.ItemName(itemname);
                info.ItemType_CN = _item.ItemName_CN(itemname);
                info.PartType = _item.PartName(typename);
                info.AnswerResposn = "_";
                info.QuestionCount = 20;
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = "";
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();

                Cloze.Content = form["textarea"];
                Cloze.Choices = new List<string>();
                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["Cloze_tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["Cloze_D" + (j + 1).ToString()].ToString());
                }
                for (int i = 0; i < 4 * info.QuestionCount; i++)
                {
                    Cloze.Choices.Add(form["Cloze_Option" + (i + 1).ToString()].ToString());
                }
                Cloze.Info = info;



                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.NewGuid();
                    cquestion.AssessmentItemID = Cloze.Info.ItemID;
                    cquestion.ChooseA = form["Cloze_Option" + ((i * 4) + 1)];
                    cquestion.ChooseB = form["Cloze_Option" + ((i * 4) + 2)];
                    cquestion.ChooseC = form["Cloze_Option" + ((i * 4) + 3)];
                    cquestion.ChooseD = form["Cloze_Option" + ((i * 4) + 4)];
                    cquestion.Answer = info.AnswerValue[i];
                    cquestion.Duration = double.Parse(form["Cloze_timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["Cloze_scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["Cloze_difficultquestion" + (i + 1)]);
                    cquestion.Analyze = info.Tip[i];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }
                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        _item.AddClozeItem(Cloze);
                        item.AssessmentItemID = Cloze.Info.ItemID;
                        item.ItemTypeID = itemname;
                        item.QuestionCount = Cloze.Info.QuestionCount;
                        item.Description = "";
                        item.Difficult = Cloze.Info.Diffcult;
                        item.Duration = Cloze.Info.ReplyTime;
                        item.SaveTime = DateTime.Now;
                        item.Original = Cloze.Content;
                        item.Score = Cloze.Info.Score;
                        item.Count = 0;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.UpdateUserID = item.UserID;
                        item.UpdateTime = DateTime.Now;
                        string name = "../../ExaminationItemLibrary/" + Cloze.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();

                        _item.Create(item);
                        _question.CreateQuestion(listquestion);
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["Cloze_hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }

            }
            #endregion

            #region 判断ItemTypeID为"阅读理解——选择题型“
            if (itemname == 7)
            {
                ReadingPartOption Rpo = new ReadingPartOption();
                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                ItemBassInfo info = new ItemBassInfo();
                info.ItemID = Guid.NewGuid();
                info.ItemType = _item.ItemName(itemname);
                info.ItemType_CN = _item.ItemName_CN(itemname);
                info.PartType = _item.PartName(typename);
                info.AnswerResposn = "_";
                info.QuestionCount = int.Parse(form["MultipleChoice_selectnum"]);
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = "";
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();

                Rpo.Content = form["textarea"];
                Rpo.Choices = new List<string>();
                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["MultipleChoice_tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["MultipleChoice_D" + (j + 1).ToString()].ToString());
                    info.Problem.Add(form["MultipleChoice_Question" + (j + 1).ToString()].ToString());
                }
                for (int i = 0; i < 4 * info.QuestionCount; i++)
                {
                    Rpo.Choices.Add(form["MultipleChoice_Option" + (i + 1).ToString()].ToString());
                }
                Rpo.Info = info;



                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.NewGuid();
                    cquestion.AssessmentItemID = Rpo.Info.ItemID;
                    cquestion.QuestionContent = info.Problem[i];
                    cquestion.ChooseA = form["MultipleChoice_Option" + ((i * 4) + 1)];
                    cquestion.ChooseB = form["MultipleChoice_Option" + ((i * 4) + 2)];
                    cquestion.ChooseC = form["MultipleChoice_Option" + ((i * 4) + 3)];
                    cquestion.ChooseD = form["MultipleChoice_Option" + ((i * 4) + 4)];
                    cquestion.Answer = info.AnswerValue[i];
                    cquestion.Duration = double.Parse(form["MultipleChoice_timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["MultipleChoice_scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["MultipleChoice_difficultquestion" + (i + 1)]);
                    cquestion.Analyze = info.Tip[i];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }
                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        _item.AddIntensiveRead(Rpo);
                        item.AssessmentItemID = Rpo.Info.ItemID;
                        item.ItemTypeID = itemname;
                        item.QuestionCount = Rpo.Info.QuestionCount;
                        item.Description = "";
                        item.Difficult = Rpo.Info.Diffcult;
                        item.Duration = Rpo.Info.ReplyTime;
                        item.SaveTime = DateTime.Now;
                        item.Original = Rpo.Content;
                        item.Score = Rpo.Info.Score;
                        item.Count = 0;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.UpdateUserID = item.UserID;
                        item.UpdateTime = DateTime.Now;
                        string name = "../../ExaminationItemLibrary/" + Rpo.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();

                        _item.Create(item);
                        _question.CreateQuestion(listquestion);
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["MultipleChoice_hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }
            #endregion

            #region 判断ItemTypeID为“阅读理解-选词填空”
            if (itemname == 6)
            {
                ReadingPartCompletion Rpc = new ReadingPartCompletion();
                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                ItemBassInfo info = new ItemBassInfo();
                info.ItemID = Guid.NewGuid();
                info.ItemType = _item.ItemName(itemname);
                info.ItemType_CN = _item.ItemName_CN(itemname);
                info.PartType = _item.PartName(typename);
                info.AnswerResposn = "_";
                info.QuestionCount = 10;
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = "";
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                Rpc.Content = form["textarea"];
                Rpc.WordList = new List<string>();
                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["BankedCloze_tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["BankedCloze_textanswer" + (j + 1).ToString()].ToString());
                }
                Rpc.Info = info;

                for (int n = 0; n < 15; n++)
                {
                    Rpc.WordList.Add(form["BankedCloze_Option" + (n + 1)]);
                }
                CEDTS_Expansion Expansion = new CEDTS_Expansion();
                Expansion.AssessmentItemID = Rpc.Info.ItemID;
                Expansion.ChoiceA = form["BankedCloze_Option1"];
                Expansion.ChoiceB = form["BankedCloze_Option2"];
                Expansion.ChoiceC = form["BankedCloze_Option3"];
                Expansion.ChoiceD = form["BankedCloze_Option4"];
                Expansion.ChoiceE = form["BankedCloze_Option5"];
                Expansion.ChoiceF = form["BankedCloze_Option6"];
                Expansion.ChoiceG = form["BankedCloze_Option7"];
                Expansion.ChoiceH = form["BankedCloze_Option8"];
                Expansion.ChoiceI = form["BankedCloze_Option9"];
                Expansion.ChoiceJ = form["BankedCloze_Option10"];
                Expansion.ChoiceK = form["BankedCloze_Option11"];
                Expansion.ChoiceL = form["BankedCloze_Option12"];
                Expansion.ChoiceM = form["BankedCloze_Option13"];
                Expansion.ChoiceN = form["BankedCloze_Option14"];
                Expansion.ChoiceO = form["BankedCloze_Option15"];


                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.NewGuid();
                    cquestion.AssessmentItemID = Rpc.Info.ItemID;
                    cquestion.Duration = double.Parse(form["BankedCloze_timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["BankedCloze_scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["BankedCloze_difficultquestion" + (i + 1)]);
                    cquestion.Answer = info.AnswerValue[i];
                    cquestion.Analyze = info.Tip[i];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }
                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        _item.AddRpcItem(Rpc);

                        item.AssessmentItemID = Rpc.Info.ItemID;
                        item.ItemTypeID = itemname;
                        item.QuestionCount = Rpc.Info.QuestionCount;
                        item.Description = "";
                        item.Difficult = Rpc.Info.Diffcult;
                        item.Duration = Rpc.Info.ReplyTime;
                        item.SaveTime = DateTime.Now;
                        item.Score = Rpc.Info.Score;
                        item.UpdateTime = DateTime.Now;
                        item.Count = 0;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.UpdateUserID = item.UserID;
                        item.Original = Rpc.Content;
                        string name = "../../ExaminationItemLibrary/" + Rpc.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();

                        _item.Create(item);

                        _Expansion.CreateExpansion(Expansion);
                        _question.CreateQuestion(listquestion);
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["BankedCloze_hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }
            #endregion

            #region 判断ItemTypeID为“信息匹配”
            if (itemname == 9)
            {
                InfoMatchingCompletion InfoMat = new InfoMatchingCompletion();
                CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
                ItemBassInfo info = new ItemBassInfo();
                info.ItemID = Guid.NewGuid();
                info.ItemType = _item.ItemName(itemname);
                info.ItemType_CN = _item.ItemName_CN(itemname);
                info.PartType = _item.PartName(typename);
                info.AnswerResposn = "_";
                info.QuestionCount = 10;
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = "";
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                InfoMat.Content = form["textarea"];
                InfoMat.WordList = new List<string>();
                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["InfoMatching_tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["InfoMatching_textanswer" + (j + 1).ToString()].ToString());
                }
                InfoMat.Info = info;

                for (int n = 0; n < 15; n++)
                {
                    InfoMat.WordList.Add(form["InfoMatching_Option" + (n + 1)]);
                }
                CEDTS_Expansion Expansion = new CEDTS_Expansion();
                Expansion.AssessmentItemID = InfoMat.Info.ItemID;
                Expansion.ChoiceA = form["InfoMatching_Option1"];
                Expansion.ChoiceB = form["InfoMatching_Option2"];
                Expansion.ChoiceC = form["InfoMatching_Option3"];
                Expansion.ChoiceD = form["InfoMatching_Option4"];
                Expansion.ChoiceE = form["InfoMatching_Option5"];
                Expansion.ChoiceF = form["InfoMatching_Option6"];
                Expansion.ChoiceG = form["InfoMatching_Option7"];
                Expansion.ChoiceH = form["InfoMatching_Option8"];
                Expansion.ChoiceI = form["InfoMatching_Option9"];
                Expansion.ChoiceJ = form["InfoMatching_Option10"];
                Expansion.ChoiceK = form["BankedCloze_Option11"];
                Expansion.ChoiceL = form["BankedCloze_Option12"];
                Expansion.ChoiceM = form["BankedCloze_Option13"];
                Expansion.ChoiceN = form["BankedCloze_Option14"];
                Expansion.ChoiceO = form["BankedCloze_Option15"];


                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.NewGuid();
                    cquestion.AssessmentItemID = InfoMat.Info.ItemID;
                    cquestion.Duration = double.Parse(form["InfoMatching_timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["InfoMatching_scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["InfoMatching_difficultquestion" + (i + 1)]);
                    cquestion.Answer = info.AnswerValue[i];
                    cquestion.Analyze = info.Tip[i];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }
                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        _item.AddInfMItem(InfoMat);

                        item.AssessmentItemID = InfoMat.Info.ItemID;
                        item.ItemTypeID = itemname;
                        item.QuestionCount = InfoMat.Info.QuestionCount;
                        item.Description = "";
                        item.Difficult = InfoMat.Info.Diffcult;
                        item.Duration = InfoMat.Info.ReplyTime;
                        item.SaveTime = DateTime.Now;
                        item.Score = InfoMat.Info.Score;
                        item.UpdateTime = DateTime.Now;
                        item.Count = 0;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.UpdateUserID = item.UserID;
                        item.Original = InfoMat.Content;
                        string name = "../../ExaminationItemLibrary/" + InfoMat.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();

                        _item.Create(item);

                        _Expansion.CreateExpansion(Expansion);
                        _question.CreateQuestion(listquestion);
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["InfoMatching_hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }
            #endregion

            ViewData["PartType"] = _item.GetPart(typename);
            ViewData["ItemType"] = _item.GetItem(typename, itemname);
            ViewData["IsOk"] = "1";
            return View();
        }

        /// <summary>
        /// 局部视图选择
        /// </summary>
        /// <param name="item">ItemTypeName</param>
        /// <returns>PartialView</returns>
        public ActionResult Partial(string item)
        {
            if (item == "2")
            {
                return PartialView("ShortConversations");
            }
            if (item == "3")
            {
                return PartialView("LongConversations");
            }
            if (item == "4")
            {
                return PartialView("Listen");
            }
            if (item == "5")
            {
                return PartialView("Complex");
            }
            if (item == "1")
            {
                return PartialView("SkimmingAndScanning");
            }
            if (item == "8")
            {
                return PartialView("Cloze");
            }
            if (item == "6")
            {
                return PartialView("BankedClozeReading");
            }
            if (item == "9")
            {
                return PartialView("InfoMatching");
            }
            else
            {
                return PartialView("MultipleChoiceReading");
            }

        }

        /// <summary>
        /// Dropdownlist联动时获取并绑定ItemType值
        /// </summary>
        /// <param name="type">PartTypeID</param>
        /// <returns>Json</returns>
        public JsonResult GetItem(int? type)
        {
            return Json(_item.GetItems(type));
        }

        #region 局部视图
        /// <summary>
        /// 单句听力局部视图
        /// </summary>
        /// <returns>PartialView</returns>
        public ActionResult Listen()
        {
            return PartialView();
        }

        /// <summary>
        /// 复合型听力局部视图
        /// </summary>
        /// <returns>PartialView</returns>
        public ActionResult Complex()
        {
            return PartialView();
        }

        /// <summary>
        /// 完型填空局部视图
        /// </summary>
        /// <returns>PartialView</returns>
        public ActionResult Cloze()
        {
            return PartialView();
        }

        /// <summary>
        /// 阅读理解选择题型局部视图
        /// </summary>
        /// <returns>PartialView</returns>
        public ActionResult MultipleChoiceReading()
        {
            return PartialView();
        }

        /// <summary>
        /// 阅读理解选词填空类型局部视图
        /// </summary>
        /// <returns>PartialView</returns>
        public ActionResult BankedClozeReading()
        {
            return PartialView();
        }
        #endregion


        /// <summary>
        /// 编辑试题
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>View</returns>
        public ActionResult EditListen(Guid id)
        {
            ViewData["ID"] = id;
            int? ItemID = _item.GetEditItemID(id);
            if (ItemID == 1)
            {
                return View("EditSkimmingAndScanning");
            }
            if (ItemID == 5)
            {
                return View("EditComplex");
            }

            if (ItemID == 6)
            {
                return View("EditBankedCloze");
            }
            if (ItemID == 7)
            {
                return View("EditMultipleChoice");
            }
            if (ItemID == 8)
            {
                return View("EditCloze");
            }
            if (ItemID == 9)
            {
                return View("EditInfoMatching");
            }
            else
            {
                return View();
            }

        }

        /// <summary>
        /// 编辑试题
        /// </summary>
        /// <param name="form">表单内容</param>
        /// <returns>跳转到试题首页</returns>
        [Filter.LogFilter(Description = "修改听力题目")]
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditListen(FormCollection form)
        {

            CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
            List<CEDTS_Question> listquestion = new List<CEDTS_Question>();
            Listen listen = new Listen();
            Listen Complex = new Listen();
            SkimmingScanningPartCompletion Sspc = new SkimmingScanningPartCompletion();
            ReadingPartOption Rpo = new ReadingPartOption();
            ReadingPartCompletion Rpc = new ReadingPartCompletion();
            InfoMatchingCompletion InfoMatch = new InfoMatchingCompletion();
            ClozePart Cloze = new ClozePart();
            ItemBassInfo info = new ItemBassInfo();
            info.ItemType_CN = form["ItemType_CN"];

            #region 编辑“短对话”，“长对话”，“短文听力理解”信息
            if (info.ItemType_CN == "2" || info.ItemType_CN == "3" || info.ItemType_CN == "4")
            {
                info.ItemID = Guid.Parse(form["AssessID"]);
                info.ItemType = form["ItemType"];

                info.PartType = form["PartType"];
                info.Count = int.Parse(form["Count"]);
                info.AnswerResposn = "_";
                info.QuestionCount = int.Parse(form["QuestionCount"]);
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = form["interval"];
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();
                info.timeInterval = new List<int>();
                listen.SoundFile = info.ItemID.ToString();

                listen.Script = form["textarea"];
                listen.Choices = new List<string>();
                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["D" + (j + 1).ToString()].ToString());
                    info.Problem.Add(form["Question" + (j + 1).ToString()].ToString());
                    if (info.ItemType_CN != "2")
                    {
                        info.timeInterval.Add(int.Parse(form["interval" + (j + 1)]));
                    }
                }
                for (int i = 0; i < 4 * info.QuestionCount; i++)
                {
                    listen.Choices.Add(form["Option" + (i + 1).ToString()].ToString());
                }
                listen.Info = info;


                string ques = form["QuestionID"];
                string[] QuestionID = ques.Split(',');
                List<string> l = new List<string>();
                for (int i = 0; i < QuestionID.Length; i++)
                {
                    l.Add(QuestionID[i]);
                }

                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.Parse(QuestionID[i]);
                    cquestion.AssessmentItemID = listen.Info.ItemID;
                    cquestion.QuestionContent = info.Problem[i];
                    cquestion.Duration = double.Parse(form["timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["difficultquestion" + (i + 1)]);
                    if (info.ItemType_CN != "2")
                    {
                        cquestion.Interval = int.Parse(form["interval" + (i + 1)]);
                    }
                    cquestion.ChooseA = form["Option" + ((i * 4) + 1)];
                    cquestion.ChooseB = form["Option" + ((i * 4) + 2)];
                    cquestion.ChooseC = form["Option" + ((i * 4) + 3)];
                    cquestion.ChooseD = form["Option" + ((i * 4) + 4)];
                    cquestion.Answer = form["D" + (i + 1)];
                    cquestion.Analyze = form["Tip" + (i + 1)];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }

                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {

                        _item.EditItem(listen);
                        item.SoundFile = listen.SoundFile;
                        item.AssessmentItemID = info.ItemID;
                        item.ItemTypeID = _item.GetItemID(info.ItemType);
                        item.Count = info.Count;
                        item.QuestionCount = listen.Info.QuestionCount;
                        item.Description = "";
                        item.Interval = int.Parse(listen.Info.QustionInterval);
                        item.Difficult = listen.Info.Diffcult;
                        item.Duration = listen.Info.ReplyTime;
                        item.Score = listen.Info.Score;
                        item.UpdateTime = DateTime.Now;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.Original = listen.Script;
                        string name = "../../../ExaminationItemLibrary/" + listen.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                        _item.UpdateItem(item);

                        _question.UpdateQuestion(listquestion);
                        //删除QuestionKnowledge中数据
                        string qkIDs = form["QkID"];
                        string[] qkID = qkIDs.Split(',');
                        List<int> q = new List<int>();
                        for (int i = 0; i < qkID.Length; i++)
                        {
                            q.Add(Int32.Parse(qkID[i]));
                        }
                        _QuestionKnowledge.Delete(q);
                        //新增QuestionKnowledge中数据
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }
            #endregion

            #region 编辑复合型听力信息
            if (info.ItemType_CN == "5")
            {
                info.ItemID = Guid.Parse(form["AssessID"]);
                info.ItemType = form["ItemType"];

                info.PartType = form["PartType"];
                info.Count = int.Parse(form["Count"]);
                info.AnswerResposn = "_";
                info.QuestionCount = int.Parse(form["QuestionCount"]);
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = form["interval"];
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();
                Complex.SoundFile = info.ItemID.ToString();
                Complex.Script = form["textarea"];

                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["textanswer" + (j + 1).ToString()].ToString());
                }

                Complex.Info = info;


                string ques = form["QuestionID"];
                string[] QuestionID = ques.Split(',');
                List<string> l = new List<string>();
                for (int i = 0; i < QuestionID.Length; i++)
                {
                    l.Add(QuestionID[i]);
                }

                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.Parse(QuestionID[i]);
                    cquestion.AssessmentItemID = Complex.Info.ItemID;
                    cquestion.Duration = double.Parse(form["timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["difficultquestion" + (i + 1)]);
                    cquestion.Answer = info.AnswerValue[i];
                    cquestion.Analyze = info.Tip[i];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }

                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {

                        _item.EditComplex(Complex);
                        item.SoundFile = Complex.SoundFile;
                        item.AssessmentItemID = info.ItemID;
                        item.ItemTypeID = _item.GetItemID(info.ItemType);
                        item.Count = info.Count;
                        item.QuestionCount = Complex.Info.QuestionCount;
                        item.Description = "";
                        item.Interval = int.Parse(Complex.Info.QustionInterval);
                        item.Difficult = Complex.Info.Diffcult;
                        item.Duration = Complex.Info.ReplyTime;
                        item.Score = Complex.Info.Score;
                        item.UpdateTime = DateTime.Now;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.Original = Complex.Script;
                        string name = "../../../ExaminationItemLibrary/" + Complex.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                        _item.UpdateItem(item);

                        _question.UpdateQuestion(listquestion);
                        //删除QuestionKnowledge中数据
                        string qkIDs = form["QkID"];
                        string[] qkID = qkIDs.Split(',');
                        List<int> q = new List<int>();
                        for (int i = 0; i < qkID.Length; i++)
                        {
                            q.Add(Int32.Parse(qkID[i]));
                        }
                        _QuestionKnowledge.Delete(q);
                        //新增QuestionKnowledge中数据
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }
            #endregion

            #region 快速阅读
            if (info.ItemType_CN == "1")
            {
                info.ItemID = Guid.Parse(form["AssessID"]);
                info.ItemType = form["ItemType"];

                info.PartType = form["PartType"];
                info.Count = int.Parse(form["Count"]);
                info.AnswerResposn = "_";
                info.QuestionCount = int.Parse(form["QuestionCount"]);
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = "";
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();
                Sspc.Choices = new List<string>();
                Sspc.Content = form["textarea"];
                Sspc.ChoiceNum = int.Parse(form["ChoiceNum"]);
                Sspc.TermNum = int.Parse(form["TermNum"]);

                for (int s = 0; s < Sspc.ChoiceNum; s++)
                {
                    info.Tip.Add(form["tip" + (s + 1).ToString()]);
                    info.AnswerValue.Add(form["D" + (s + 1).ToString()].ToString());
                    info.Problem.Add(form["Question" + (s + 1).ToString()].ToString());
                }

                for (int k = 0; k < 4 * Sspc.ChoiceNum; k++)
                {
                    try
                    {
                        Sspc.Choices.Add(form["Option" + (k + 1).ToString()].ToString());
                    }
                    catch
                    {
                        continue;
                    }
                }

                for (int j = 0; j < Sspc.TermNum; j++)
                {
                    info.Problem.Add(form["Question" + (j + Sspc.ChoiceNum + 1).ToString()].ToString());
                    info.Tip.Add(form["tip" + (j + Sspc.ChoiceNum + 1).ToString()]);
                    info.AnswerValue.Add(form["textanswer" + (j + Sspc.ChoiceNum + 1).ToString()].ToString());
                }

                Sspc.Info = info;


                string ques = form["QuestionID"];
                string[] QuestionID = ques.Split(',');
                List<string> l = new List<string>();
                for (int i = 0; i < QuestionID.Length; i++)
                {
                    l.Add(QuestionID[i]);
                }

                for (int m = 0; m < Sspc.ChoiceNum; m++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.Parse(QuestionID[m]);
                    cquestion.AssessmentItemID = Sspc.Info.ItemID;
                    cquestion.QuestionContent = info.Problem[m];
                    cquestion.Duration = double.Parse(form["timequestion" + (m + 1)]);
                    cquestion.Score = double.Parse(form["scorequestion" + (m + 1)]);
                    cquestion.Difficult = double.Parse(form["difficultquestion" + (m + 1)]);

                    cquestion.ChooseA = form["Option" + ((m * 4) + 1)];
                    cquestion.ChooseB = form["Option" + ((m * 4) + 2)];
                    cquestion.ChooseC = form["Option" + ((m * 4) + 3)];
                    cquestion.ChooseD = form["Option" + ((m * 4) + 4)];
                    cquestion.Answer = form["D" + (m + 1)];
                    cquestion.Analyze = form["Tip" + (m + 1)];
                    cquestion.Order = (m + 1);
                    listquestion.Add(cquestion);
                }
                for (int i = 0; i < Sspc.TermNum; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.Parse(QuestionID[(i + Sspc.ChoiceNum)]);
                    cquestion.AssessmentItemID = Sspc.Info.ItemID;
                    cquestion.QuestionContent = info.Problem[(i + Sspc.ChoiceNum)];
                    cquestion.Duration = double.Parse(form["timequestion" + (i + Sspc.ChoiceNum + 1)]);
                    cquestion.Score = double.Parse(form["scorequestion" + (i + Sspc.ChoiceNum + 1)]);
                    cquestion.Difficult = double.Parse(form["difficultquestion" + (i + Sspc.ChoiceNum + 1)]);
                    cquestion.Answer = form["D" + (i + Sspc.ChoiceNum)];
                    cquestion.Analyze = form["Tip" + (i + Sspc.ChoiceNum + 1)];
                    cquestion.Answer = form["textanswer" + (i + Sspc.ChoiceNum + 1)];
                    cquestion.Order = (i + Sspc.ChoiceNum + 1);

                    cquestion.ChooseA = "";
                    cquestion.ChooseB = "";
                    cquestion.ChooseC = "";
                    cquestion.ChooseD = "";
                    listquestion.Add(cquestion);
                }

                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {

                        _item.EditSspc(Sspc);

                        item.AssessmentItemID = info.ItemID;
                        item.ItemTypeID = _item.GetItemID(info.ItemType);
                        item.Count = info.Count;
                        item.QuestionCount = Sspc.Info.QuestionCount;
                        item.Description = "";
                        item.Difficult = Sspc.Info.Diffcult;
                        item.Duration = Sspc.Info.ReplyTime;
                        item.Score = Sspc.Info.Score;
                        item.UpdateTime = DateTime.Now;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.Original = Sspc.Content;
                        string name = "../../../ExaminationItemLibrary/" + Sspc.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                        _item.UpdateItem(item);

                        _question.UpdateQuestion(listquestion);
                        //删除QuestionKnowledge中数据
                        string qkIDs = form["QkID"];
                        string[] qkID = qkIDs.Split(',');
                        List<int> q = new List<int>();
                        for (int i = 0; i < qkID.Length; i++)
                        {
                            q.Add(Int32.Parse(qkID[i]));
                        }
                        _QuestionKnowledge.Delete(q);
                        //新增QuestionKnowledge中数据
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }
            #endregion

            #region 完形填空
            if (info.ItemType_CN == "8")
            {
                info.ItemID = Guid.Parse(form["AssessID"]);
                info.ItemType = form["ItemType"];

                info.PartType = form["PartType"];
                info.Count = int.Parse(form["Count"]);
                info.AnswerResposn = "_";
                info.QuestionCount = int.Parse(form["QuestionCount"]);
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = "";
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                Cloze.Content = form["textarea"];
                Cloze.Choices = new List<string>();
                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["D" + (j + 1).ToString()].ToString());
                }
                for (int i = 0; i < 4 * info.QuestionCount; i++)
                {
                    Cloze.Choices.Add(form["Option" + (i + 1).ToString()].ToString());
                }
                Cloze.Info = info;


                string ques = form["QuestionID"];
                string[] QuestionID = ques.Split(',');
                List<string> l = new List<string>();
                for (int i = 0; i < QuestionID.Length; i++)
                {
                    l.Add(QuestionID[i]);
                }

                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.Parse(QuestionID[i]);
                    cquestion.AssessmentItemID = Cloze.Info.ItemID;
                    cquestion.Duration = double.Parse(form["timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["difficultquestion" + (i + 1)]);

                    cquestion.ChooseA = form["Option" + ((i * 4) + 1)];
                    cquestion.ChooseB = form["Option" + ((i * 4) + 2)];
                    cquestion.ChooseC = form["Option" + ((i * 4) + 3)];
                    cquestion.ChooseD = form["Option" + ((i * 4) + 4)];
                    cquestion.Answer = form["D" + (i + 1)];
                    cquestion.Analyze = form["Tip" + (i + 1)];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }

                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {

                        _item.EditCloze(Cloze);

                        item.AssessmentItemID = info.ItemID;
                        item.ItemTypeID = _item.GetItemID(info.ItemType);
                        item.Count = info.Count;
                        item.QuestionCount = Cloze.Info.QuestionCount;
                        item.Description = "";
                        item.Difficult = Cloze.Info.Diffcult;
                        item.Duration = Cloze.Info.ReplyTime;
                        item.Score = Cloze.Info.Score;
                        item.UpdateTime = DateTime.Now;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.Original = Cloze.Content;
                        string name = "../../../ExaminationItemLibrary/" + Cloze.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();

                        _item.UpdateItem(item);

                        _question.UpdateQuestion(listquestion);
                        //删除QuestionKnowledge中数据
                        string qkIDs = form["QkID"];
                        string[] qkID = qkIDs.Split(',');
                        List<int> q = new List<int>();
                        for (int i = 0; i < qkID.Length; i++)
                        {
                            q.Add(Int32.Parse(qkID[i]));
                        }
                        _QuestionKnowledge.Delete(q);
                        //新增QuestionKnowledge中数据
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }
            #endregion

            #region 阅读理解-选择题型
            if (info.ItemType_CN == "7")
            {
                info.ItemID = Guid.Parse(form["AssessID"]);
                info.ItemType = form["ItemType"];

                info.PartType = form["PartType"];
                info.Count = int.Parse(form["Count"]);
                info.AnswerResposn = "_";
                info.QuestionCount = int.Parse(form["QuestionCount"]);
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = "";
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();
                Rpo.Content = form["textarea"];
                Rpo.Choices = new List<string>();
                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["D" + (j + 1).ToString()].ToString());
                    info.Problem.Add(form["Question" + (j + 1).ToString()].ToString());
                }
                for (int i = 0; i < 4 * info.QuestionCount; i++)
                {
                    Rpo.Choices.Add(form["Option" + (i + 1).ToString()].ToString());
                }
                Rpo.Info = info;


                string ques = form["QuestionID"];
                string[] QuestionID = ques.Split(',');
                List<string> l = new List<string>();
                for (int i = 0; i < QuestionID.Length; i++)
                {
                    l.Add(QuestionID[i]);
                }

                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.Parse(QuestionID[i]);
                    cquestion.AssessmentItemID = Rpo.Info.ItemID;
                    cquestion.QuestionContent = info.Problem[i];
                    cquestion.Duration = double.Parse(form["timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["difficultquestion" + (i + 1)]);

                    cquestion.ChooseA = form["Option" + ((i * 4) + 1)];
                    cquestion.ChooseB = form["Option" + ((i * 4) + 2)];
                    cquestion.ChooseC = form["Option" + ((i * 4) + 3)];
                    cquestion.ChooseD = form["Option" + ((i * 4) + 4)];
                    cquestion.Answer = form["D" + (i + 1)];
                    cquestion.Analyze = form["Tip" + (i + 1)];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }

                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {

                        _item.EditRpo(Rpo);

                        item.AssessmentItemID = info.ItemID;
                        item.ItemTypeID = _item.GetItemID(info.ItemType);
                        item.Count = info.Count;
                        item.QuestionCount = Rpo.Info.QuestionCount;
                        item.Description = "";
                        item.Difficult = Rpo.Info.Diffcult;
                        item.Duration = Rpo.Info.ReplyTime;
                        item.Score = Rpo.Info.Score;
                        item.UpdateTime = DateTime.Now;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.Original = Rpo.Content;
                        string name = "../../../ExaminationItemLibrary/" + Rpo.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                        _item.UpdateItem(item);

                        _question.UpdateQuestion(listquestion);
                        //删除QuestionKnowledge中数据
                        string qkIDs = form["QkID"];
                        string[] qkID = qkIDs.Split(',');
                        List<int> q = new List<int>();
                        for (int i = 0; i < qkID.Length; i++)
                        {
                            q.Add(Int32.Parse(qkID[i]));
                        }
                        _QuestionKnowledge.Delete(q);
                        //新增QuestionKnowledge中数据
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }
            #endregion

            #region 阅读理解-选词填空
            if (info.ItemType_CN == "6")
            {
                info.ItemID = Guid.Parse(form["AssessID"]);
                info.ItemType = form["ItemType"];
                Rpc.ExpansionID = int.Parse(form["ExpansionID"]);
                info.PartType = form["PartType"];
                info.Count = int.Parse(form["Count"]);
                info.AnswerResposn = "_";
                info.QuestionCount = int.Parse(form["QuestionCount"]);
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = "";
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                Rpc.WordList = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();
                Rpc.Content = form["textarea"];


                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["textanswer" + (j + 1).ToString()].ToString());
                }

                Rpc.Info = info;


                for (int n = 0; n < 15; n++)
                {
                    Rpc.WordList.Add(form["Option" + (n + 1)]);
                }
                CEDTS_Expansion Expansion = new CEDTS_Expansion();
                Expansion.ExpansionID = Rpc.ExpansionID;
                Expansion.AssessmentItemID = info.ItemID;
                Expansion.ChoiceA = form["Option1"];
                Expansion.ChoiceB = form["Option2"];
                Expansion.ChoiceC = form["Option3"];
                Expansion.ChoiceD = form["Option4"];
                Expansion.ChoiceE = form["Option5"];
                Expansion.ChoiceF = form["Option6"];
                Expansion.ChoiceG = form["Option7"];
                Expansion.ChoiceH = form["Option8"];
                Expansion.ChoiceI = form["Option9"];
                Expansion.ChoiceJ = form["Option10"];
                Expansion.ChoiceK = form["Option11"];
                Expansion.ChoiceL = form["Option12"];
                Expansion.ChoiceM = form["Option13"];
                Expansion.ChoiceN = form["Option14"];
                Expansion.ChoiceO = form["Option15"];

                string ques = form["QuestionID"];
                string[] QuestionID = ques.Split(',');
                List<string> l = new List<string>();
                for (int i = 0; i < QuestionID.Length; i++)
                {
                    l.Add(QuestionID[i]);
                }

                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.Parse(QuestionID[i]);
                    cquestion.AssessmentItemID = info.ItemID;
                    cquestion.Duration = double.Parse(form["timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["difficultquestion" + (i + 1)]);
                    cquestion.Answer = info.AnswerValue[i];
                    cquestion.Analyze = info.Tip[i];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }

                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {

                        _item.EditRpc(Rpc);

                        item.AssessmentItemID = info.ItemID;
                        item.ItemTypeID = _item.GetItemID(info.ItemType);
                        item.Count = info.Count;
                        item.QuestionCount = info.QuestionCount;
                        item.Description = "";
                        item.Difficult = Rpc.Info.Diffcult;
                        item.Duration = Rpc.Info.ReplyTime;
                        item.Score = Rpc.Info.Score;
                        item.UpdateTime = DateTime.Now;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.Original = Rpc.Content;
                        string name = "../../../ExaminationItemLibrary/" + Rpc.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                        _item.UpdateItem(item);

                        _Expansion.UpdateExpansion(Expansion);
                        _question.UpdateQuestion(listquestion);
                        //删除QuestionKnowledge中数据
                        string qkIDs = form["QkID"];
                        string[] qkID = qkIDs.Split(',');
                        List<int> q = new List<int>();
                        for (int i = 0; i < qkID.Length; i++)
                        {
                            q.Add(Int32.Parse(qkID[i]));
                        }
                        _QuestionKnowledge.Delete(q);
                        //新增QuestionKnowledge中数据
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }
            #endregion

            #region 阅读理解-信息匹配
            if (info.ItemType_CN == "9")
            {
                info.ItemID = Guid.Parse(form["AssessID"]);
                info.ItemType = form["ItemType"];
                InfoMatch.ExpansionID = int.Parse(form["ExpansionID"]);
                info.PartType = form["PartType"];
                info.Count = int.Parse(form["Count"]);
                info.AnswerResposn = "_";
                info.QuestionCount = int.Parse(form["QuestionCount"]);
                info.Diffcult = double.Parse(form["difficult"]);
                info.QustionInterval = "";
                info.ReplyTime = int.Parse(form["time"]);
                info.Score = int.Parse(form["score"]);
                info.AnswerValue = new List<string>();
                InfoMatch.WordList = new List<string>();
                info.Tip = new List<string>();
                info.Problem = new List<string>();
                InfoMatch.Content = form["textarea"];


                for (int j = 0; j < info.QuestionCount; j++)
                {
                    info.Tip.Add(form["tip" + (j + 1).ToString()]);
                    info.AnswerValue.Add(form["textanswer" + (j + 1).ToString()].ToString());
                }

                InfoMatch.Info = info;


                for (int n = 0; n < 15; n++)
                {
                    InfoMatch.WordList.Add(form["Option" + (n + 1)]);
                }
                CEDTS_Expansion Expansion = new CEDTS_Expansion();
                Expansion.ExpansionID = InfoMatch.ExpansionID;
                Expansion.AssessmentItemID = info.ItemID;
                Expansion.ChoiceA = form["Option1"];
                Expansion.ChoiceB = form["Option2"];
                Expansion.ChoiceC = form["Option3"];
                Expansion.ChoiceD = form["Option4"];
                Expansion.ChoiceE = form["Option5"];
                Expansion.ChoiceF = form["Option6"];
                Expansion.ChoiceG = form["Option7"];
                Expansion.ChoiceH = form["Option8"];
                Expansion.ChoiceI = form["Option9"];
                Expansion.ChoiceJ = form["Option10"];
                Expansion.ChoiceK = form["Option11"];
                Expansion.ChoiceL = form["Option12"];
                Expansion.ChoiceM = form["Option13"];
                Expansion.ChoiceN = form["Option14"];
                Expansion.ChoiceO = form["Option15"];

                string ques = form["QuestionID"];
                string[] QuestionID = ques.Split(',');
                List<string> l = new List<string>();
                for (int i = 0; i < QuestionID.Length; i++)
                {
                    l.Add(QuestionID[i]);
                }

                for (int i = 0; i < info.QuestionCount; i++)
                {
                    CEDTS_Question cquestion = new CEDTS_Question();
                    cquestion.QuestionID = Guid.Parse(QuestionID[i]);
                    cquestion.AssessmentItemID = info.ItemID;
                    cquestion.Duration = double.Parse(form["timequestion" + (i + 1)]);
                    cquestion.Score = double.Parse(form["scorequestion" + (i + 1)]);
                    cquestion.Difficult = double.Parse(form["difficultquestion" + (i + 1)]);
                    cquestion.Answer = info.AnswerValue[i];
                    cquestion.Analyze = info.Tip[i];
                    cquestion.Order = (i + 1);
                    listquestion.Add(cquestion);
                }

                try
                {
                    using (TransactionScope tran = new TransactionScope())
                    {

                        _item.EditInfoMatch(InfoMatch);

                        item.AssessmentItemID = info.ItemID;
                        item.ItemTypeID = _item.GetItemID(info.ItemType);
                        item.Count = info.Count;
                        item.QuestionCount = info.QuestionCount;
                        item.Description = "";
                        item.Difficult = InfoMatch.Info.Diffcult;
                        item.Duration = InfoMatch.Info.ReplyTime;
                        item.Score = InfoMatch.Info.Score;
                        item.UpdateTime = DateTime.Now;
                        item.UserID = _item.SelectUserID(User.Identity.Name);
                        item.Original = InfoMatch.Content;
                        string name = "../../../ExaminationItemLibrary/" + InfoMatch.Info.ItemID + ".xml";
                        string MapPath = HttpContext.Server.MapPath(name);
                        XDocument doc = XDocument.Load(MapPath);
                        item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                        _item.UpdateItem(item);

                        _Expansion.UpdateExpansion(Expansion);
                        _question.UpdateQuestion(listquestion);
                        //删除QuestionKnowledge中数据
                        string qkIDs = form["QkID"];
                        string[] qkID = qkIDs.Split(',');
                        List<int> q = new List<int>();
                        for (int i = 0; i < qkID.Length; i++)
                        {
                            q.Add(Int32.Parse(qkID[i]));
                        }
                        _QuestionKnowledge.Delete(q);
                        //新增QuestionKnowledge中数据
                        for (int i = 0; i < listquestion.Count; i++)
                        {
                            string[] Knowledge = form["hidden" + (i + 1)].ToString().Split(',');
                            for (int k = 0; k < Knowledge.Length; k++)
                            {
                                CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                qk.QuestionID = listquestion[i].QuestionID;
                                qk.KnowledgePointID = Guid.Parse(Knowledge[k]);
                                qk.Weight = k + 1;
                                _QuestionKnowledge.Create(qk);
                            }
                        }
                        tran.Complete();
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
                catch (XmlException)
                {
                    return null;
                }

                catch (Exception ex)
                {
                    ex.Message.ToString();
                    return null;
                }
            }
            #endregion

            return RedirectToAction("index");
        }


        /// <summary>
        /// 编辑时，获取Listen信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>Json</returns>
        [HttpPost]
        public JsonResult GetListen(Guid id)
        {
            return Json(_item.SelectAll(id));
        }

        /// <summary>
        /// 编辑时获取复合型听力信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>Json</returns>
        public JsonResult GetComplex(Guid id)
        {
            return Json(_item.SelectComplex(id));
        }

        /// <summary>
        /// 编辑时获取快速阅读信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>json</returns>
        public JsonResult GetSspc(Guid id)
        {
            return Json(_item.SelectSspc(id));
        }

        /// <summary>
        /// 编辑时获取当前点击的完型填空信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>JSON</returns>
        public JsonResult GetCloze(Guid id)
        {
            return Json(_item.SelectCloze(id));
        }

        /// <summary>
        /// 编辑是获取当前点击的阅读理解-选词填空信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>json</returns>
        public JsonResult GetRpo(Guid id)
        {
            return Json(_item.SelectRpo(id));
        }

        /// <summary>
        /// 编辑时获取当前点击的阅读理解-选词填空信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>Json</returns>
        public JsonResult GetRpc(Guid id)
        {
            return Json(_item.SlelectRpc(id));
        }

        /// <summary>
        /// 编辑时获取当前点击的阅读理解-信息匹配信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>Json</returns>
        public JsonResult GetInfoMatch(Guid id)
        {
            return Json(_item.SelectInfoMatch(id));
        }

        /// <summary>
        /// 删除试题
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>Json</returns>
        [Filter.LogFilter(Description = "删除试题")]
        public JsonResult DeleteItem(Guid? id)
        {

            return Json(_item.DeleteItem(id));
        }

        /// <summary>
        /// 获取知识点
        /// </summary>
        /// <param name="PartType">PartTypeID</param>
        /// <returns>Content</returns>
        public ActionResult GetPoint()
        {
            return Content(_item.GetPoint());
        }

        /// <summary>
        /// 编辑时获取知识点
        /// </summary>
        /// <param name="PartType">PartTypeName</param>
        /// <returns>Content</returns>
        public ActionResult EditGetPoint()
        {
            return Content(_item.GetPoint());
        }
    }
}
