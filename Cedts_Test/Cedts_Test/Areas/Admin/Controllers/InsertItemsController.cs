using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cedts_Test.Areas.Admin.Models;
using System.Transactions;
using System.Xml.Linq;

namespace Cedts_Test.Areas.Admin.Controllers
{
    public class InsertItemsController : Controller
    {
        ICedts_ItemRepository _item;
        ICedts_ItemXMLRepository _itemXML;
        ICedts_ExpansionRepository _Expansion;
        ICedts_QuestionRepository _question;
        ICedts_QuestionKnowledgeRepository _QuestionKnowledge;
        public InsertItemsController(ICedts_ItemXMLRepository x, ICedts_ItemRepository i, ICedts_QuestionRepository q, ICedts_QuestionKnowledgeRepository qk, ICedts_ExpansionRepository Exp)
        {
            _item = i;
            _question = q;
            _QuestionKnowledge = qk;
            _Expansion = Exp;
            _itemXML = x;
        }

        [Authorize]
        public ActionResult Index()
        {
            int a = 0;
            int UserId = _item.SelectUserID(User.Identity.Name);
            List<ItemBank> itemBank = _itemXML.GetOldItem();
            try
            {
                for (int n = 0; n < itemBank.Count; n++)
                {
                    using (TransactionScope tran = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(2, 0, 0)))
                    {
                        a++;
                        CEDTS_AssessmentItem item = new CEDTS_AssessmentItem();
                        List<CEDTS_Question> listquestion = new List<CEDTS_Question>();

                        switch (itemBank[n].ItemTypeID)
                        {
                            case 3://短对话听力
                                {
                                    #region 短对话听力

                                    Listen listen = _itemXML.Getlisten(itemBank[n].Content, "2");
                                    //保存小问题到question表中
                                    for (int i = 0; i < listen.Info.QuestionCount; i++)
                                    {
                                        CEDTS_Question cquestion = new CEDTS_Question();
                                        cquestion.QuestionID = Guid.NewGuid();
                                        cquestion.Sound = "";
                                        cquestion.Interval = 0;
                                        cquestion.AssessmentItemID = listen.Info.ItemID;
                                        cquestion.QuestionContent = listen.Info.Problem[i];
                                        cquestion.Duration = listen.Info.TimeQuestion[i];
                                        cquestion.Score = listen.Info.ScoreQuestion[i];
                                        cquestion.Difficult = listen.Info.DifficultQuestion[i];
                                        cquestion.ChooseA = listen.Choices[i * 4 + 0];
                                        cquestion.ChooseB = listen.Choices[i * 4 + 1];
                                        cquestion.ChooseC = listen.Choices[i * 4 + 2];
                                        cquestion.ChooseD = listen.Choices[i * 4 + 3];
                                        cquestion.Answer = listen.Info.AnswerValue[i];
                                        cquestion.Analyze = listen.Info.Tip[i];
                                        cquestion.Order = (i + 1);
                                        listquestion.Add(cquestion);
                                    }

                                    _item.AddItem(listen);//生成XML

                                    item.AssessmentItemID = listen.Info.ItemID;
                                    item.SoundFile = listen.SoundFile;
                                    item.ItemTypeID = int.Parse(listen.Info.ItemType);
                                    item.Interval = int.Parse(listen.Info.QustionInterval);
                                    item.Course = listen.Info.Course;
                                    item.Unit = listen.Info.Unit;
                                    item.QuestionCount = listen.Info.QuestionCount;
                                    item.Description = "";
                                    item.Score = listen.Info.Score;
                                    item.Difficult = listen.Info.Diffcult;
                                    item.Duration = listen.Info.ReplyTime;
                                    item.SaveTime = DateTime.Now;
                                    item.UserID = UserId;
                                    item.UpdateUserID = item.UserID;
                                    item.UpdateTime = DateTime.Now;
                                    item.Count = 0;
                                    item.Original = listen.Script;
                                    string MapPath = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + listen.Info.ItemID + ".xml";
                                    XDocument doc = XDocument.Load(MapPath);
                                    item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                    _item.Create(item);//保存试题到AssessmentItem

                                    _question.CreateQuestion(listquestion);//保存试题中所有的question

                                    List<Guid> Knowledge = _itemXML.GetPointID();//获取相关的知识点

                                    for (int i = 0; i < listquestion.Count; i++)
                                    {
                                        CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                        qk.QuestionID = listquestion[i].QuestionID;
                                        Random rand = new Random();
                                        int temp = rand.Next(Knowledge.Count);
                                        qk.KnowledgePointID = Knowledge[temp];
                                        qk.Weight = 1;
                                        _QuestionKnowledge.Create(qk);
                                    }

                                    #endregion

                                    break;
                                }

                            case 14://长对话听力
                                {
                                    #region 长对话听力

                                    Listen listen = _itemXML.Getlisten(itemBank[n].Content, "3");
                                    //保存小问题到question表中
                                    for (int i = 0; i < listen.Info.QuestionCount; i++)
                                    {
                                        CEDTS_Question cquestion = new CEDTS_Question();
                                        cquestion.QuestionID = Guid.NewGuid();
                                        cquestion.Interval = listen.Info.timeInterval[i];
                                        cquestion.Sound = listen.Info.questionSound[i];
                                        cquestion.AssessmentItemID = listen.Info.ItemID;
                                        cquestion.QuestionContent = listen.Info.Problem[i];
                                        cquestion.Duration = listen.Info.TimeQuestion[i];
                                        cquestion.Score = listen.Info.ScoreQuestion[i];
                                        cquestion.Difficult = listen.Info.DifficultQuestion[i];
                                        cquestion.ChooseA = listen.Choices[i * 4 + 0];
                                        cquestion.ChooseB = listen.Choices[i * 4 + 1];
                                        cquestion.ChooseC = listen.Choices[i * 4 + 2];
                                        cquestion.ChooseD = listen.Choices[i * 4 + 3];
                                        cquestion.Answer = listen.Info.AnswerValue[i];
                                        cquestion.Analyze = listen.Info.Tip[i];
                                        cquestion.Order = (i + 1);
                                        listquestion.Add(cquestion);
                                    }

                                    _item.AddItem(listen);//生成XML

                                    item.AssessmentItemID = listen.Info.ItemID;
                                    item.SoundFile = listen.SoundFile;
                                    item.ItemTypeID = int.Parse(listen.Info.ItemType);
                                    item.Interval = int.Parse(listen.Info.QustionInterval);
                                    item.Course = listen.Info.Course;
                                    item.Unit = listen.Info.Unit;
                                    item.QuestionCount = listen.Info.QuestionCount;
                                    item.Description = "";
                                    item.Score = listen.Info.Score;
                                    item.Difficult = listen.Info.Diffcult;
                                    item.Duration = listen.Info.ReplyTime;
                                    item.SaveTime = DateTime.Now;
                                    item.UserID = UserId;
                                    item.UpdateUserID = item.UserID;
                                    item.UpdateTime = DateTime.Now;
                                    item.Count = 0;
                                    item.Original = listen.Script;
                                    string MapPath = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + listen.Info.ItemID + ".xml";
                                    XDocument doc = XDocument.Load(MapPath);
                                    item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                    _item.Create(item);//保存试题到AssessmentItem

                                    _question.CreateQuestion(listquestion);//保存试题中所有的question

                                    List<Guid> Knowledge = _itemXML.GetPointID();//获取相关的知识点

                                    for (int i = 0; i < listquestion.Count; i++)
                                    {
                                        CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                        qk.QuestionID = listquestion[i].QuestionID;
                                        Random rand = new Random();
                                        int temp = rand.Next(Knowledge.Count);
                                        qk.KnowledgePointID = Knowledge[temp];
                                        qk.Weight = 1;
                                        _QuestionKnowledge.Create(qk);
                                    }

                                    #endregion

                                    break;
                                }

                            case 4://听力理解
                                {
                                    #region 听力理解

                                    Listen listen = _itemXML.Getlisten(itemBank[n].Content, "4");
                                    //保存小问题到question表中
                                    for (int i = 0; i < listen.Info.QuestionCount; i++)
                                    {
                                        CEDTS_Question cquestion = new CEDTS_Question();
                                        cquestion.QuestionID = Guid.NewGuid();
                                        cquestion.Interval = listen.Info.timeInterval[i];
                                        cquestion.Sound = listen.Info.questionSound[i];
                                        cquestion.AssessmentItemID = listen.Info.ItemID;
                                        cquestion.QuestionContent = listen.Info.Problem[i];
                                        cquestion.Duration = listen.Info.TimeQuestion[i];
                                        cquestion.Score = listen.Info.ScoreQuestion[i];
                                        cquestion.Difficult = listen.Info.DifficultQuestion[i];
                                        cquestion.ChooseA = listen.Choices[i * 4 + 0];
                                        cquestion.ChooseB = listen.Choices[i * 4 + 1];
                                        cquestion.ChooseC = listen.Choices[i * 4 + 2];
                                        cquestion.ChooseD = listen.Choices[i * 4 + 3];
                                        cquestion.Answer = listen.Info.AnswerValue[i];
                                        cquestion.Analyze = listen.Info.Tip[i];
                                        cquestion.Order = (i + 1);
                                        listquestion.Add(cquestion);
                                    }

                                    _item.AddItem(listen);//生成XML

                                    item.AssessmentItemID = listen.Info.ItemID;
                                    item.SoundFile = listen.SoundFile;
                                    item.ItemTypeID = int.Parse(listen.Info.ItemType);
                                    item.Interval = int.Parse(listen.Info.QustionInterval);
                                    item.Course = listen.Info.Course;
                                    item.Unit = listen.Info.Unit;
                                    item.QuestionCount = listen.Info.QuestionCount;
                                    item.Description = "";
                                    item.Score = listen.Info.Score;
                                    item.Difficult = listen.Info.Diffcult;
                                    item.Duration = listen.Info.ReplyTime;
                                    item.SaveTime = DateTime.Now;
                                    item.UserID = UserId;
                                    item.UpdateUserID = item.UserID;
                                    item.UpdateTime = DateTime.Now;
                                    item.Count = 0;
                                    item.Original = listen.Script;
                                    string MapPath = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + listen.Info.ItemID + ".xml";
                                    XDocument doc = XDocument.Load(MapPath);
                                    item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                    _item.Create(item);//保存试题到AssessmentItem

                                    _question.CreateQuestion(listquestion);//保存试题中所有的question

                                    List<Guid> Knowledge = _itemXML.GetPointID();//获取相关的知识点

                                    for (int i = 0; i < listquestion.Count; i++)
                                    {
                                        CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                        qk.QuestionID = listquestion[i].QuestionID;
                                        Random rand = new Random();
                                        int temp = rand.Next(Knowledge.Count);
                                        qk.KnowledgePointID = Knowledge[temp];
                                        qk.Weight = 1;
                                        _QuestionKnowledge.Create(qk);
                                    }

                                    #endregion

                                    break;
                                }

                            case 5://复合型听力
                                {
                                    #region 复合型听力

                                    Listen listen = _itemXML.Getlisten(itemBank[n].Content, "5");
                                    //保存小问题到question表中
                                    for (int i = 0; i < listen.Info.QuestionCount; i++)
                                    {
                                        CEDTS_Question cquestion = new CEDTS_Question();
                                        cquestion.QuestionID = Guid.NewGuid();
                                        cquestion.Sound = "";
                                        cquestion.Interval = 0;
                                        cquestion.AssessmentItemID = listen.Info.ItemID;
                                        cquestion.QuestionContent = listen.Info.Problem[i];
                                        cquestion.Duration = listen.Info.TimeQuestion[i];
                                        cquestion.Score = listen.Info.ScoreQuestion[i];
                                        cquestion.Difficult = listen.Info.DifficultQuestion[i];
                                        cquestion.Answer = listen.Info.AnswerValue[i];
                                        cquestion.Analyze = listen.Info.Tip[i];
                                        cquestion.Order = (i + 1);
                                        listquestion.Add(cquestion);
                                    }

                                    _item.AddComplexItem(listen);//生成XML

                                    item.AssessmentItemID = listen.Info.ItemID;
                                    item.SoundFile = listen.SoundFile;
                                    item.ItemTypeID = int.Parse(listen.Info.ItemType);
                                    item.Interval = int.Parse(listen.Info.QustionInterval);
                                    item.Course = listen.Info.Course;
                                    item.Unit = listen.Info.Unit;
                                    item.QuestionCount = listen.Info.QuestionCount;
                                    item.Description = "";
                                    item.Score = listen.Info.Score;
                                    item.Difficult = listen.Info.Diffcult;
                                    item.Duration = listen.Info.ReplyTime;
                                    item.SaveTime = DateTime.Now;
                                    item.UserID = UserId;
                                    item.UpdateUserID = item.UserID;
                                    item.UpdateTime = DateTime.Now;
                                    item.Count = 0;
                                    item.Original = listen.Script;
                                    string MapPath = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + listen.Info.ItemID + ".xml";
                                    XDocument doc = XDocument.Load(MapPath);
                                    item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                    _item.Create(item);//保存试题到AssessmentItem

                                    _question.CreateQuestion(listquestion);//保存试题中所有的question

                                    List<Guid> Knowledge = _itemXML.GetPointID();//获取相关的知识点

                                    for (int i = 0; i < listquestion.Count; i++)
                                    {
                                        CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                        qk.QuestionID = listquestion[i].QuestionID;
                                        Random rand = new Random();
                                        int temp = rand.Next(Knowledge.Count);
                                        qk.KnowledgePointID = Knowledge[temp];
                                        qk.Weight = 1;
                                        _QuestionKnowledge.Create(qk);
                                    }

                                    #endregion

                                    break;
                                }

                            case 15://快速阅读
                                {
                                    if (a == 2849)
                                    {
                                        break;
                                    }
                                    #region 快速阅读

                                    SkimmingScanningPartCompletion sspc = _itemXML.GetSspc(itemBank[n].Content, "1");

                                    for (int i = 0; i < sspc.Info.QuestionCount; i++)
                                    {
                                        int scount = sspc.Choices.Count / sspc.ChoiceNum;
                                        CEDTS_Question cquestion = new CEDTS_Question();
                                        cquestion.QuestionID = Guid.NewGuid();
                                        cquestion.AssessmentItemID = sspc.Info.ItemID;
                                        cquestion.QuestionContent = sspc.Info.Problem[i];
                                        cquestion.Duration = sspc.Info.TimeQuestion[i];
                                        cquestion.Score = sspc.Info.ScoreQuestion[i];
                                        cquestion.Difficult = sspc.Info.DifficultQuestion[i];
                                        if (i < sspc.ChoiceNum)
                                        {
                                            cquestion.ChooseA = sspc.Choices[i * scount + 0];
                                            cquestion.ChooseB = sspc.Choices[i * scount + 1];
                                            cquestion.ChooseC = sspc.Choices[i * scount + 2];
                                            if (scount == 4)
                                            {
                                                cquestion.ChooseD = sspc.Choices[i * scount + 3];
                                            }
                                            else cquestion.ChooseD = "";
                                        }
                                        else
                                        {
                                            cquestion.ChooseA = "";
                                            cquestion.ChooseB = "";
                                            cquestion.ChooseC = "";
                                            cquestion.ChooseD = "";
                                        }
                                        cquestion.Answer = sspc.Info.AnswerValue[i];
                                        cquestion.Analyze = sspc.Info.Tip[i];
                                        cquestion.Order = (i + 1);
                                        listquestion.Add(cquestion);
                                    }

                                    _item.AddSspcItem(sspc);//生成XML

                                    item.AssessmentItemID = sspc.Info.ItemID;
                                    item.ItemTypeID = int.Parse(sspc.Info.ItemType);
                                    item.Course = sspc.Info.Course;
                                    item.Unit = sspc.Info.Unit;
                                    item.QuestionCount = sspc.Info.QuestionCount;
                                    item.Description = "";
                                    item.Score = sspc.Info.Score;
                                    item.Difficult = sspc.Info.Diffcult;
                                    item.Duration = sspc.Info.ReplyTime;
                                    item.SaveTime = DateTime.Now;
                                    item.UserID = UserId;
                                    item.UpdateUserID = item.UserID;
                                    item.UpdateTime = DateTime.Now;
                                    item.Count = 0;
                                    item.Original = sspc.Content;
                                    string MapPath = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + sspc.Info.ItemID + ".xml";
                                    XDocument doc = XDocument.Load(MapPath);
                                    item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                    _item.Create(item);//保存试题到AssessmentItem

                                    _question.CreateQuestion(listquestion);//保存试题中所有的question

                                    List<Guid> Knowledge = _itemXML.GetPointID();//获取相关的知识点

                                    for (int i = 0; i < listquestion.Count; i++)
                                    {
                                        CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                        qk.QuestionID = listquestion[i].QuestionID;
                                        Random rand = new Random();
                                        int temp = rand.Next(Knowledge.Count);
                                        qk.KnowledgePointID = Knowledge[temp];
                                        qk.Weight = 1;
                                        _QuestionKnowledge.Create(qk);
                                    }

                                    #endregion

                                    break;
                                }

                            case 8://完型填空
                                {
                                    #region 完型填空

                                    ClozePart cp = _itemXML.GetCp(itemBank[n].Content, "8");

                                    for (int i = 0; i < cp.Info.QuestionCount; i++)
                                    {
                                        CEDTS_Question cquestion = new CEDTS_Question();
                                        cquestion.QuestionID = Guid.NewGuid();
                                        cquestion.AssessmentItemID = cp.Info.ItemID;
                                        cquestion.QuestionContent = cp.Info.Problem[i];
                                        cquestion.Duration = cp.Info.TimeQuestion[i];
                                        cquestion.Score = cp.Info.ScoreQuestion[i];
                                        cquestion.Difficult = cp.Info.DifficultQuestion[i];
                                        cquestion.ChooseA = cp.Choices[i * 4 + 0];
                                        cquestion.ChooseB = cp.Choices[i * 4 + 1];
                                        cquestion.ChooseC = cp.Choices[i * 4 + 2];
                                        cquestion.ChooseD = cp.Choices[i * 4 + 3];
                                        cquestion.Answer = cp.Info.AnswerValue[i];
                                        cquestion.Analyze = cp.Info.Tip[i];
                                        cquestion.Order = (i + 1);
                                        listquestion.Add(cquestion);
                                    }

                                    _item.AddClozeItem(cp);//生成XML

                                    item.AssessmentItemID = cp.Info.ItemID;
                                    item.ItemTypeID = int.Parse(cp.Info.ItemType);
                                    item.Course = cp.Info.Course;
                                    item.Unit = cp.Info.Unit;
                                    item.QuestionCount = cp.Info.QuestionCount;
                                    item.Description = "";
                                    item.Score = cp.Info.Score;
                                    item.Difficult = cp.Info.Diffcult;
                                    item.Duration = cp.Info.ReplyTime;
                                    item.SaveTime = DateTime.Now;
                                    item.UserID = UserId;
                                    item.UpdateUserID = item.UserID;
                                    item.UpdateTime = DateTime.Now;
                                    item.Count = 0;
                                    item.Original = cp.Content;
                                    string MapPath = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + cp.Info.ItemID + ".xml";
                                    XDocument doc = XDocument.Load(MapPath);
                                    item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                    _item.Create(item);//保存试题到AssessmentItem

                                    _question.CreateQuestion(listquestion);//保存试题中所有的question

                                    List<Guid> Knowledge = _itemXML.GetPointID();//获取相关的知识点

                                    for (int i = 0; i < listquestion.Count; i++)
                                    {
                                        CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                        qk.QuestionID = listquestion[i].QuestionID;
                                        Random rand = new Random();
                                        int temp = rand.Next(Knowledge.Count);
                                        qk.KnowledgePointID = Knowledge[temp];
                                        qk.Weight = 1;
                                        _QuestionKnowledge.Create(qk);
                                    }

                                    #endregion

                                    break;
                                }

                            case 16://选词填空
                                {
                                    #region 选词填空

                                    ReadingPartCompletion rpc = _itemXML.GetRpc(itemBank[n].Content, "6");

                                    CEDTS_Expansion Expansion = new CEDTS_Expansion();
                                    Expansion.AssessmentItemID = rpc.Info.ItemID;
                                    Expansion.ChoiceA = rpc.WordList[0];
                                    Expansion.ChoiceB = rpc.WordList[1];
                                    Expansion.ChoiceC = rpc.WordList[2];
                                    Expansion.ChoiceD = rpc.WordList[3];
                                    Expansion.ChoiceE = rpc.WordList[4];
                                    Expansion.ChoiceF = rpc.WordList[5];
                                    Expansion.ChoiceG = rpc.WordList[6];
                                    Expansion.ChoiceH = rpc.WordList[7];
                                    Expansion.ChoiceI = rpc.WordList[8];
                                    Expansion.ChoiceJ = rpc.WordList[9];
                                    Expansion.ChoiceK = rpc.WordList[10];
                                    Expansion.ChoiceL = rpc.WordList[11];
                                    Expansion.ChoiceM = rpc.WordList[12];
                                    Expansion.ChoiceN = rpc.WordList[13];
                                    Expansion.ChoiceO = rpc.WordList[14];

                                    for (int i = 0; i < rpc.Info.QuestionCount; i++)
                                    {
                                        CEDTS_Question cquestion = new CEDTS_Question();
                                        cquestion.QuestionID = Guid.NewGuid();
                                        cquestion.AssessmentItemID = rpc.Info.ItemID;
                                        cquestion.QuestionContent = rpc.Info.Problem[i];
                                        cquestion.Duration = rpc.Info.TimeQuestion[i];
                                        cquestion.Score = rpc.Info.ScoreQuestion[i];
                                        cquestion.Difficult = rpc.Info.DifficultQuestion[i];
                                        cquestion.Answer = rpc.Info.AnswerValue[i];
                                        cquestion.Analyze = rpc.Info.Tip[i];
                                        cquestion.Order = (i + 1);
                                        listquestion.Add(cquestion);
                                    }

                                    _item.AddRpcItem(rpc);//生成XML

                                    item.AssessmentItemID = rpc.Info.ItemID;
                                    item.ItemTypeID = int.Parse(rpc.Info.ItemType);
                                    item.Course = rpc.Info.Course;
                                    item.Unit = rpc.Info.Unit;
                                    item.QuestionCount = rpc.Info.QuestionCount;
                                    item.Description = "";
                                    item.Score = rpc.Info.Score;
                                    item.Difficult = rpc.Info.Diffcult;
                                    item.Duration = rpc.Info.ReplyTime;
                                    item.SaveTime = DateTime.Now;
                                    item.UserID = UserId;
                                    item.UpdateUserID = item.UserID;
                                    item.UpdateTime = DateTime.Now;
                                    item.Count = 0;
                                    item.Original = rpc.Content;
                                    string MapPath = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + rpc.Info.ItemID + ".xml";
                                    XDocument doc = XDocument.Load(MapPath);
                                    item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                    _item.Create(item);//保存试题到AssessmentItem

                                    _Expansion.CreateExpansion(Expansion);//保存选项
                                    _question.CreateQuestion(listquestion);//保存试题中所有的question

                                    List<Guid> Knowledge = _itemXML.GetPointID();//获取相关的知识点

                                    for (int i = 0; i < listquestion.Count; i++)
                                    {
                                        CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                        qk.QuestionID = listquestion[i].QuestionID;
                                        Random rand = new Random();
                                        int temp = rand.Next(Knowledge.Count);
                                        qk.KnowledgePointID = Knowledge[temp];
                                        qk.Weight = 1;
                                        _QuestionKnowledge.Create(qk);
                                    }

                                    #endregion

                                    break;
                                }

                            case 9://阅读选择
                                {
                                    #region 阅读选择

                                    ReadingPartOption rpo = _itemXML.GetRpo(itemBank[n].Content, "7");
                                    for (int i = 0; i < rpo.Info.QuestionCount; i++)
                                    {
                                        CEDTS_Question cquestion = new CEDTS_Question();
                                        cquestion.QuestionID = Guid.NewGuid();
                                        cquestion.AssessmentItemID = rpo.Info.ItemID;
                                        cquestion.QuestionContent = rpo.Info.Problem[i];
                                        cquestion.Duration = rpo.Info.TimeQuestion[i];
                                        cquestion.Score = rpo.Info.ScoreQuestion[i];
                                        cquestion.Difficult = rpo.Info.DifficultQuestion[i];
                                        cquestion.ChooseA = rpo.Choices[i * 4 + 0];
                                        cquestion.ChooseB = rpo.Choices[i * 4 + 1];
                                        cquestion.ChooseC = rpo.Choices[i * 4 + 2];
                                        cquestion.ChooseD = rpo.Choices[i * 4 + 3];
                                        cquestion.Answer = rpo.Info.AnswerValue[i];
                                        cquestion.Analyze = rpo.Info.Tip[i];
                                        cquestion.Order = (i + 1);
                                        listquestion.Add(cquestion);
                                    }

                                    _item.AddRpoItem(rpo);//生成XML

                                    item.AssessmentItemID = rpo.Info.ItemID;
                                    item.ItemTypeID = int.Parse(rpo.Info.ItemType);
                                    item.Course = rpo.Info.Course;
                                    item.Unit = rpo.Info.Unit;
                                    item.QuestionCount = rpo.Info.QuestionCount;
                                    item.Description = "";
                                    item.Score = rpo.Info.Score;
                                    item.Difficult = rpo.Info.Diffcult;
                                    item.Duration = rpo.Info.ReplyTime;
                                    item.SaveTime = DateTime.Now;
                                    item.UserID = UserId;
                                    item.UpdateUserID = item.UserID;
                                    item.UpdateTime = DateTime.Now;
                                    item.Count = 0;
                                    item.Original = rpo.Content;
                                    string MapPath = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + rpo.Info.ItemID + ".xml";
                                    XDocument doc = XDocument.Load(MapPath);
                                    item.Content = "<?xml version='1.0' encoding='gb2312'?>" + doc.ToString();
                                    _item.Create(item);//保存试题到AssessmentItem

                                    _question.CreateQuestion(listquestion);//保存试题中所有的question

                                    List<Guid> Knowledge = _itemXML.GetPointID();//获取相关的知识点

                                    for (int i = 0; i < listquestion.Count; i++)
                                    {
                                        CEDTS_QuestionKnowledge qk = new CEDTS_QuestionKnowledge();
                                        qk.QuestionID = listquestion[i].QuestionID;
                                        Random rand = new Random();
                                        int temp = rand.Next(Knowledge.Count);
                                        qk.KnowledgePointID = Knowledge[temp];
                                        qk.Weight = 1;
                                        _QuestionKnowledge.Create(qk);
                                    }

                                    #endregion

                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                        tran.Complete();
                    }
                }
                int b = a;
                ViewData["a"] = "导入成功";
                return View();
            }
            catch (Exception ex)
            {
                ViewData["a"] = ex.Message.ToString();
                return View();
            }
        }
    }
}
