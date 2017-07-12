using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_ItemXMLRepository : ICedts_ItemXMLRepository
    {
        Cedts_Entities db;
        public Cedts_ItemXMLRepository()
        {
            db = new Cedts_Entities();
        }

        XmlDocument doc;
        XmlNode node;
        XmlElement elem;

        List<Guid> ICedts_ItemXMLRepository.GetPointID()
        {
            return db.CEDTS_KnowledgePoints.Where(p =>p.Level == 2).Select(p => p.KnowledgePointID).ToList();
        }

        List<ItemBank> ICedts_ItemXMLRepository.GetOldItem()
        {
            return db.ItemBank.ToList();
        }

        Listen ICedts_ItemXMLRepository.Getlisten(string text, string type)
        {
            Listen listen = new Listen();
            ItemBassInfo info = new ItemBassInfo();
            doc = new XmlDocument();
            doc.LoadXml(text);
            node = doc.SelectSingleNode("assessmentItem");//查找<assessmentItem>节点
            elem = (XmlElement)node;
            info.ItemID = Guid.Parse(elem.GetAttribute("identifier"));
            info.Course = elem.GetAttribute("course");
            info.Unit = elem.GetAttribute("unit");
            XmlNodeList nodeList = node.ChildNodes;//<assessmentItem>下的所有子节点
            if (type == "2")
            {
                info.QuestionCount = nodeList.Count;
            }
            else if (type == "5")
            {
                info.QuestionCount = nodeList.Count - 2;
            }
            else
            {
                info.QuestionCount = nodeList.Count - 1;
            }
            info.ItemType = type;
            info.PartType = "2";
            info.AnswerResposn = "_";
            info.QustionInterval = "";
            info.QuestionID = new List<Guid>();
            info.AnswerValue = new List<string>();
            info.Tip = new List<string>();
            info.Problem = new List<string>();
            info.DifficultQuestion = new List<double>();
            info.ScoreQuestion = new List<double>();
            info.questionSound = new List<string>();
            info.TimeQuestion = new List<double>();
            info.timeInterval = new List<int>();

            listen.Choices = new List<string>();

            info.Score = 0.0;
            info.ReplyTime = 0.0;
            info.Diffcult = 0.0;

            switch (type)
            {
                #region 短对话听力

                case "2":
                    {
                        info.ItemType_CN = "短对话听力";
                        for (int i = 0; i < nodeList.Count; i++)
                        {
                            XmlElement qElem = (XmlElement)nodeList[i];//question节点

                            XmlNodeList qList = nodeList[i].ChildNodes;//question下的所有子节点

                            for (int j = 0; j < qList.Count; j++)
                            {
                                XmlElement pElem = (XmlElement)qList[j];//question下的一个子节点

                                //判断节点名为prompt
                                if (pElem.Name == "prompt")
                                {
                                    XmlNodeList soundList = qList[j].ChildNodes;//prompt下的所有子节点
                                    for (int k = 0; k < soundList.Count; k++)
                                    {
                                        XmlElement sElem = (XmlElement)soundList[k];//prompt下的一个子节点
                                        if (sElem.GetAttribute("src") != "")
                                        {
                                            info.QustionInterval = sElem.GetAttribute("duration");
                                            string src = sElem.GetAttribute("src");
                                            listen.SoundFile = src.Substring(0, src.LastIndexOf('.'));
                                            XmlNodeList scriptList = soundList[k].ChildNodes;//sound下的所有子节点
                                            for (int m = 0; m < scriptList.Count; m++)
                                            {
                                                XmlElement tElem = (XmlElement)scriptList[m];
                                                if (tElem.Name == "transcript")
                                                {
                                                    listen.Script = tElem.InnerXml;
                                                }
                                            }
                                        }
                                    }
                                }

                                //判断节点名为choice
                                if (pElem.Name == "choice")
                                {
                                    XmlNodeList optionList = qList[j].ChildNodes;
                                    for (int k = 0; k < optionList.Count; k++)
                                    {
                                        XmlElement oElem = (XmlElement)optionList[k];
                                        if (oElem.Name == "option")
                                        {
                                            listen.Choices.Add(oElem.InnerXml);
                                        }
                                    }
                                }

                                //判断节点名为key
                                if (pElem.Name == "key")
                                {
                                    string[] answers = { "A", "B", "C", "D" };
                                    int temp = int.Parse(pElem.InnerXml) - 1;
                                    info.AnswerValue.Add(answers[temp]);
                                }

                            }

                            info.questionSound.Add("");
                            info.timeInterval.Add(0);
                            info.Tip.Add("");
                            info.Problem.Add("");
                            info.ScoreQuestion.Add(1);
                            info.Score += 1;
                            info.TimeQuestion.Add(1.0);
                            info.ReplyTime += 1.0;
                            info.DifficultQuestion.Add(0.2);
                            info.Diffcult += 0.2;
                        }

                        info.Diffcult = info.Diffcult / info.QuestionCount;
                        listen.Info = info;
                        break;
                    }

                #endregion

                #region 长对话听力

                case "3":
                    {
                        info.ItemType_CN = "长对话听力";
                        for (int i = 0; i < nodeList.Count; i++)
                        {
                            if (i == 0)
                            {
                                XmlElement pElem = (XmlElement)nodeList[i];//prompt节点

                                XmlNodeList soundList = nodeList[i].ChildNodes;

                                for (int j = 0; j < soundList.Count; j++)
                                {
                                    XmlElement sElem = (XmlElement)soundList[j];
                                    if (sElem.Name == "sound" && sElem.GetAttribute("src") != "")
                                    {
                                        info.QustionInterval = sElem.GetAttribute("duration");
                                        listen.SoundFile = sElem.GetAttribute("src").Substring(0, sElem.GetAttribute("src").LastIndexOf('.'));
                                        listen.Script = sElem.FirstChild.InnerXml;
                                    }
                                }
                                continue;
                            }

                            XmlElement qElem = (XmlElement)nodeList[i];//question节点

                            XmlNodeList qList = nodeList[i].ChildNodes;//question下的所有子节点

                            for (int j = 0; j < qList.Count; j++)
                            {
                                XmlElement pElem = (XmlElement)qList[j];//question下的一个子节点

                                //判断节点名为prompt
                                if (pElem.Name == "prompt")
                                {
                                    XmlNodeList soundList = qList[j].ChildNodes;//prompt下的所有子节点
                                    for (int k = 0; k < soundList.Count; k++)
                                    {
                                        XmlElement sElem = (XmlElement)soundList[k];//prompt下的一个子节点
                                        if (sElem.GetAttribute("src") != "")
                                        {
                                            string src = sElem.GetAttribute("src");
                                            info.questionSound.Add(src.Substring(0, src.LastIndexOf('.')));
                                            info.timeInterval.Add(int.Parse(sElem.GetAttribute("duration")));
                                            XmlNodeList scriptList = soundList[k].ChildNodes;//sound下的所有子节点
                                            for (int m = 0; m < scriptList.Count; m++)
                                            {
                                                XmlElement tElem = (XmlElement)scriptList[m];
                                                if (tElem.Name == "transcript")
                                                {
                                                    info.Problem.Add(tElem.InnerXml);
                                                }
                                            }
                                        }
                                    }
                                }

                                //判断节点名为choice
                                if (pElem.Name == "choice")
                                {
                                    XmlNodeList optionList = qList[j].ChildNodes;
                                    for (int k = 0; k < optionList.Count; k++)
                                    {
                                        XmlElement oElem = (XmlElement)optionList[k];
                                        if (oElem.Name == "option")
                                        {
                                            listen.Choices.Add(oElem.InnerXml);
                                        }
                                    }
                                }

                                //判断节点名为key
                                if (pElem.Name == "key")
                                {
                                    string[] answers = { "A", "B", "C", "D" };
                                    int temp = int.Parse(pElem.InnerXml) - 1;
                                    info.AnswerValue.Add(answers[temp]);
                                }

                            }

                            info.Tip.Add("");
                            info.ScoreQuestion.Add(1);
                            info.Score += 1;
                            info.TimeQuestion.Add(1.0);
                            info.ReplyTime += 1.0;
                            info.DifficultQuestion.Add(0.2);
                            info.Diffcult += 0.2;
                        }

                        info.Diffcult = info.Diffcult / info.QuestionCount;
                        listen.Info = info;
                        break;
                    }

                #endregion

                #region 听力短文理解

                case "4":
                    {
                        info.ItemType_CN = "听力短文理解";
                        for (int i = 0; i < nodeList.Count; i++)
                        {
                            if (i == 0)
                            {
                                XmlElement pElem = (XmlElement)nodeList[i];//prompt节点

                                XmlNodeList soundList = nodeList[i].ChildNodes;

                                for (int j = 0; j < soundList.Count; j++)
                                {
                                    XmlElement sElem = (XmlElement)soundList[j];
                                    if (sElem.Name == "sound" && sElem.GetAttribute("src") != "")
                                    {
                                        info.QustionInterval = sElem.GetAttribute("duration");
                                        listen.SoundFile = sElem.GetAttribute("src").Substring(0, sElem.GetAttribute("src").LastIndexOf('.'));
                                        listen.Script = sElem.FirstChild.InnerXml;
                                    }
                                }
                                continue;
                            }

                            XmlElement qElem = (XmlElement)nodeList[i];//question节点

                            XmlNodeList qList = nodeList[i].ChildNodes;//question下的所有子节点

                            for (int j = 0; j < qList.Count; j++)
                            {
                                XmlElement pElem = (XmlElement)qList[j];//question下的一个子节点

                                //判断节点名为prompt
                                if (pElem.Name == "prompt")
                                {
                                    XmlNodeList soundList = qList[j].ChildNodes;//prompt下的所有子节点
                                    for (int k = 0; k < soundList.Count; k++)
                                    {
                                        XmlElement sElem = (XmlElement)soundList[k];//prompt下的一个子节点
                                        if (sElem.GetAttribute("src") != "")
                                        {
                                            string src = sElem.GetAttribute("src");
                                            info.questionSound.Add(src.Substring(0, src.LastIndexOf('.')));
                                            info.timeInterval.Add(int.Parse(sElem.GetAttribute("duration")));
                                            XmlNodeList scriptList = soundList[k].ChildNodes;//sound下的所有子节点
                                            for (int m = 0; m < scriptList.Count; m++)
                                            {
                                                XmlElement tElem = (XmlElement)scriptList[m];
                                                if (tElem.Name == "transcript")
                                                {
                                                    info.Problem.Add(tElem.InnerXml);
                                                }
                                            }
                                        }
                                    }
                                }

                                //判断节点名为choice
                                if (pElem.Name == "choice")
                                {
                                    XmlNodeList optionList = qList[j].ChildNodes;
                                    for (int k = 0; k < optionList.Count; k++)
                                    {
                                        XmlElement oElem = (XmlElement)optionList[k];
                                        if (oElem.Name == "option")
                                        {
                                            listen.Choices.Add(oElem.InnerXml);
                                        }
                                    }
                                }

                                //判断节点名为key
                                if (pElem.Name == "key")
                                {
                                    string[] answers = { "A", "B", "C", "D" };
                                    int temp = int.Parse(pElem.InnerXml) - 1;
                                    info.AnswerValue.Add(answers[temp]);
                                }

                            }

                            info.Tip.Add("");
                            info.ScoreQuestion.Add(1);
                            info.Score += 1;
                            info.TimeQuestion.Add(1.0);
                            info.ReplyTime += 1.0;
                            info.DifficultQuestion.Add(0.2);
                            info.Diffcult += 0.2;
                        }

                        info.Diffcult = info.Diffcult / info.QuestionCount;
                        listen.Info = info;
                        break;
                    }

                #endregion

                #region 复合型听力

                default:
                    {
                        info.ItemType_CN = "复合型听力";

                        for (int i = 0; i < nodeList.Count - 1; i++)
                        {
                            if (i == 0)
                            {
                                XmlElement pElem = (XmlElement)nodeList[i];//prompt节点

                                XmlNodeList soundList = nodeList[i].ChildNodes;

                                for (int j = 0; j < soundList.Count; j++)
                                {
                                    XmlElement sElem = (XmlElement)soundList[j];
                                    if (sElem.Name == "sound" && sElem.GetAttribute("src") != "")
                                    {
                                        info.QustionInterval = sElem.GetAttribute("duration");
                                        listen.SoundFile = sElem.GetAttribute("src").Substring(0, sElem.GetAttribute("src").LastIndexOf('.'));
                                    }
                                    if (sElem.Name == "text")
                                    {
                                        int a = 0;
                                        while (sElem.InnerXml.IndexOf("<tag type=\"text\" />") > 0)
                                        {
                                            a++;
                                            int index = sElem.InnerXml.IndexOf("<tag type=\"text\" />");
                                            int length = "<tag type=\"text\" />".Length;
                                            sElem.InnerXml = sElem.InnerXml.Substring(0, index) + "(_" + a + "_)" + sElem.InnerXml.Substring(index + length);
                                        }
                                        listen.Script = sElem.InnerXml;
                                    }
                                }
                                continue;
                            }

                            XmlElement qElem = (XmlElement)nodeList[i];//question节点

                            XmlNodeList qList = nodeList[i].ChildNodes;//question下的所有子节点

                            for (int j = 0; j < qList.Count; j++)
                            {
                                XmlElement pElem = (XmlElement)qList[j];//question下的一个子节点

                                //判断节点名为key
                                if (pElem.Name == "key")
                                {
                                    info.AnswerValue.Add(pElem.InnerXml);
                                }

                            }

                            info.questionSound.Add("");
                            info.timeInterval.Add(0);
                            info.Tip.Add("");
                            info.Problem.Add("");
                            info.ScoreQuestion.Add(1);
                            info.Score += 1;
                            info.TimeQuestion.Add(1.0);
                            info.ReplyTime += 1.0;
                            info.DifficultQuestion.Add(0.2);
                            info.Diffcult += 0.2;
                        }

                        info.Diffcult = info.Diffcult / info.QuestionCount;
                        listen.Info = info;

                        break;
                    }

                #endregion

            }

            return listen;
        }

        SkimmingScanningPartCompletion ICedts_ItemXMLRepository.GetSspc(string text, string type)
        {
            SkimmingScanningPartCompletion sspc = new SkimmingScanningPartCompletion();
            ItemBassInfo info = new ItemBassInfo();
            doc = new XmlDocument();
            doc.LoadXml(text);
            node = doc.SelectSingleNode("assessmentItem");//查找<assessmentItem>节点
            elem = (XmlElement)node;
            info.ItemID = Guid.Parse(elem.GetAttribute("identifier"));
            info.Course = elem.GetAttribute("course");
            info.Unit = elem.GetAttribute("unit");
            XmlNodeList nodeList = node.ChildNodes;//<assessmentItem>下的所有子节点
            info.QuestionCount = nodeList.Count - 1;
            info.ItemType = type;
            info.ItemType_CN = "快速阅读";
            info.PartType = "1";
            info.AnswerResposn = "_";
            info.QustionInterval = "";
            info.QuestionID = new List<Guid>();
            info.AnswerValue = new List<string>();
            info.Tip = new List<string>();
            info.Problem = new List<string>();
            info.DifficultQuestion = new List<double>();
            info.ScoreQuestion = new List<double>();
            info.TimeQuestion = new List<double>();

            sspc.Choices = new List<string>();

            info.Score = 0.0;
            info.ReplyTime = 0.0;
            info.Diffcult = 0.0;
            int tempnum = 0;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (i == 0)
                {
                    XmlElement pElem = (XmlElement)nodeList[i];//prompt节点

                    sspc.Content = pElem.InnerXml;

                    continue;
                }

                XmlElement qElem = (XmlElement)nodeList[i];//question节点
                if (qElem.GetAttribute("type") == "choice")
                {
                    tempnum++;
                    XmlNodeList qList = nodeList[i].ChildNodes;//question下的所有子节点

                    for (int j = 0; j < qList.Count; j++)
                    {
                        XmlElement pElem = (XmlElement)qList[j];

                        if (pElem.Name == "prompt")
                        {
                            info.Problem.Add(pElem.InnerXml);
                        }

                        if (pElem.Name == "choice")
                        {
                            XmlNodeList optionList = qList[j].ChildNodes;
                            for (int k = 0; k < optionList.Count; k++)
                            {
                                XmlElement oElem = (XmlElement)optionList[k];
                                if (oElem.Name == "option")
                                {
                                    sspc.Choices.Add(oElem.InnerXml);
                                }
                            }
                        }

                        if (pElem.Name == "key")
                        {
                            string[] answers = { "A", "B", "C", "D" };
                            int temp = int.Parse(pElem.InnerXml) - 1;
                            info.AnswerValue.Add(answers[temp]);
                        }
                    }

                    info.Tip.Add("");
                    info.ScoreQuestion.Add(1);
                    info.Score += 1;
                    info.TimeQuestion.Add(1.5);
                    info.ReplyTime += 1.5;
                    info.DifficultQuestion.Add(0.2);
                    info.Diffcult += 0.2;
                }
                else
                {
                    XmlNodeList qList = nodeList[i].ChildNodes;//question下的所有子节点

                    for (int j = 0; j < qList.Count; j++)
                    {
                        XmlElement pElem = (XmlElement)qList[j];

                        if (pElem.Name == "prompt")
                        {
                            while (pElem.InnerXml.IndexOf("<tag type=\"text\" />") > 0)
                            {
                                int index = pElem.InnerXml.IndexOf("<tag type=\"text\" />");
                                int length = "<tag type=\"text\" />".Length;
                                pElem.InnerXml = pElem.InnerXml.Substring(0, index) + "____" + pElem.InnerXml.Substring(index + length);
                            }
                            info.Problem.Add(pElem.InnerXml);
                        }

                        if (pElem.Name == "key")
                        {
                            info.AnswerValue.Add(pElem.InnerXml);
                        }
                    }

                    info.Tip.Add("");
                    info.ScoreQuestion.Add(1);
                    info.Score += 1;
                    info.TimeQuestion.Add(1.5);
                    info.ReplyTime += 1.5;
                    info.DifficultQuestion.Add(0.4);
                    info.Diffcult += 0.4;
                }
            }

            info.Diffcult = info.Diffcult / info.QuestionCount;
            sspc.Info = info;
            sspc.ChoiceNum = tempnum;
            sspc.TermNum = info.QuestionCount - tempnum;

            return sspc;
        }

        ClozePart ICedts_ItemXMLRepository.GetCp(string text, string type)
        {
            ClozePart cp = new ClozePart();
            ItemBassInfo info = new ItemBassInfo();
            doc = new XmlDocument();
            doc.LoadXml(text);
            node = doc.SelectSingleNode("assessmentItem");//查找<assessmentItem>节点
            elem = (XmlElement)node;
            info.ItemID = Guid.Parse(elem.GetAttribute("identifier"));
            info.Course = elem.GetAttribute("course");
            info.Unit = elem.GetAttribute("unit");
            XmlNodeList nodeList = node.ChildNodes;//<assessmentItem>下的所有子节点
            info.QuestionCount = nodeList.Count - 1;
            info.ItemType = type;
            info.ItemType_CN = "完型填空";
            info.PartType = "4";
            info.AnswerResposn = "_";
            info.QustionInterval = "";
            info.QuestionID = new List<Guid>();
            info.AnswerValue = new List<string>();
            info.Tip = new List<string>();
            info.Problem = new List<string>();
            info.DifficultQuestion = new List<double>();
            info.ScoreQuestion = new List<double>();
            info.TimeQuestion = new List<double>();

            cp.Choices = new List<string>();

            info.Score = 0.0;
            info.ReplyTime = 0.0;
            info.Diffcult = 0.0;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (i == 0)
                {
                    XmlElement pElem = (XmlElement)nodeList[i];//prompt节点
                    int a = 0;
                    while (pElem.InnerXml.IndexOf("<tag type=\"choice\" />") > 0)
                    {
                        a++;
                        int index = pElem.InnerXml.IndexOf("<tag type=\"choice\" />");
                        int length = "<tag type=\"choice\" />".Length;
                        pElem.InnerXml = pElem.InnerXml.Substring(0, index) + "(_" + a + "_)" + pElem.InnerXml.Substring(index + length);
                    }
                    cp.Content = pElem.InnerXml;
                    continue;
                }

                XmlNodeList qList = nodeList[i].ChildNodes;//question下的所有子节点

                for (int j = 0; j < qList.Count; j++)
                {
                    XmlElement pElem = (XmlElement)qList[j];

                    if (pElem.Name == "choice")
                    {
                        XmlNodeList optionList = qList[j].ChildNodes;
                        for (int k = 0; k < optionList.Count; k++)
                        {
                            XmlElement oElem = (XmlElement)optionList[k];
                            if (oElem.Name == "option")
                            {
                                cp.Choices.Add(oElem.InnerXml);
                            }
                        }
                    }

                    if (pElem.Name == "key")
                    {
                        string[] answers = { "A", "B", "C", "D" };
                        int temp = int.Parse(pElem.InnerXml) - 1;
                        info.AnswerValue.Add(answers[temp]);
                    }
                }

                info.Tip.Add("");
                info.Problem.Add("");
                info.ScoreQuestion.Add(0.5);
                info.Score += 0.5;
                info.TimeQuestion.Add(0.75);
                info.ReplyTime += 0.75;
                info.DifficultQuestion.Add(0.4);
                info.Diffcult += 0.4;
            }

            info.Diffcult = info.Diffcult / info.QuestionCount;
            cp.Info = info;

            return cp;
        }

        ReadingPartCompletion ICedts_ItemXMLRepository.GetRpc(string text, string type)
        {
            ReadingPartCompletion rpc = new ReadingPartCompletion();
            ItemBassInfo info = new ItemBassInfo();
            doc = new XmlDocument();
            doc.LoadXml(text);
            node = doc.SelectSingleNode("assessmentItem");//查找<assessmentItem>节点
            elem = (XmlElement)node;
            info.ItemID = Guid.Parse(elem.GetAttribute("identifier"));
            info.Course = elem.GetAttribute("course");
            info.Unit = elem.GetAttribute("unit");
            XmlNodeList nodeList = node.ChildNodes;//<assessmentItem>下的所有子节点
            info.QuestionCount = nodeList.Count - 2;
            info.ItemType = type;
            info.ItemType_CN = "阅读理解选词填空";
            info.PartType = "3";
            info.AnswerResposn = "_";
            info.QustionInterval = "";
            info.QuestionID = new List<Guid>();
            info.AnswerValue = new List<string>();
            info.Tip = new List<string>();
            info.Problem = new List<string>();
            info.DifficultQuestion = new List<double>();
            info.ScoreQuestion = new List<double>();
            info.TimeQuestion = new List<double>();

            rpc.WordList = new List<string>();

            info.Score = 0.0;
            info.ReplyTime = 0.0;
            info.Diffcult = 0.0;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (i == 0)
                {
                    XmlElement pElem = (XmlElement)nodeList[i];//prompt节点
                    int a = 0;
                    while (pElem.InnerXml.IndexOf("<tag type=\"choice\" />") > 0)
                    {
                        a++;
                        int index = pElem.InnerXml.IndexOf("<tag type=\"choice\" />");
                        int length = "<tag type=\"choice\" />".Length;
                        pElem.InnerXml = pElem.InnerXml.Substring(0, index) + "(_" + a + "_)" + pElem.InnerXml.Substring(index + length);
                    }
                    rpc.Content = pElem.InnerXml;

                    continue;
                }

                if (i == 1)
                {
                    XmlElement cElem = (XmlElement)nodeList[i];//Chioce节点

                    XmlNodeList optionList = nodeList[i].ChildNodes;
                    for (int k = 0; k < optionList.Count; k++)
                    {
                        XmlElement oElem = (XmlElement)optionList[k];
                        if (oElem.Name == "option")
                        {
                            rpc.WordList.Add(oElem.InnerXml);
                        }
                    }
                    continue;
                }

                XmlElement qElem = (XmlElement)nodeList[i];//question节点

                string[] answers = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O" };
                int tempnum = int.Parse(qElem.FirstChild.InnerXml);
                info.AnswerValue.Add(answers[tempnum - 1]);

                info.Tip.Add("");
                info.Problem.Add("");
                info.ScoreQuestion.Add(1);
                info.Score += 1;
                info.TimeQuestion.Add(0.75);
                info.ReplyTime += 0.75;
                info.DifficultQuestion.Add(0.4);
                info.Diffcult += 0.4;
            }

            info.Diffcult = info.Diffcult / info.QuestionCount;
            rpc.Info = info;

            return rpc;
        }

        ReadingPartOption ICedts_ItemXMLRepository.GetRpo(string text, string type)
        {
            ReadingPartOption rpo = new ReadingPartOption();
            ItemBassInfo info = new ItemBassInfo();
            doc = new XmlDocument();
            doc.LoadXml(text);
            node = doc.SelectSingleNode("assessmentItem");//查找<assessmentItem>节点
            elem = (XmlElement)node;
            info.ItemID = Guid.Parse(elem.GetAttribute("identifier").ToUpper());
            info.Course = elem.GetAttribute("course");
            info.Unit = elem.GetAttribute("unit");
            XmlNodeList nodeList = node.ChildNodes;//<assessmentItem>下的所有子节点
            info.QuestionCount = nodeList.Count - 1;
            info.ItemType = type;
            info.ItemType_CN = "阅读理解选择题型";
            info.PartType = "3";
            info.AnswerResposn = "_";
            info.QustionInterval = "";
            info.QuestionID = new List<Guid>();
            info.AnswerValue = new List<string>();
            info.Tip = new List<string>();
            info.Problem = new List<string>();
            info.DifficultQuestion = new List<double>();
            info.ScoreQuestion = new List<double>();
            info.TimeQuestion = new List<double>();

            rpo.Choices = new List<string>();

            info.Score = 0.0;
            info.ReplyTime = 0.0;
            info.Diffcult = 0.0;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (i == 0)
                {
                    XmlElement pElem = (XmlElement)nodeList[i];//prompt节点

                    rpo.Content = pElem.InnerXml;

                    continue;
                }

                XmlElement qElem = (XmlElement)nodeList[i];//question节点

                XmlNodeList qList = nodeList[i].ChildNodes;//question下的所有子节点

                for (int j = 0; j < qList.Count; j++)
                {
                    XmlElement pElem = (XmlElement)qList[j];

                    if (pElem.Name == "prompt")
                    {
                        info.Problem.Add(pElem.InnerXml);
                    }

                    if (pElem.Name == "choice")
                    {
                        XmlNodeList optionList = qList[j].ChildNodes;
                        for (int k = 0; k < optionList.Count; k++)
                        {
                            XmlElement oElem = (XmlElement)optionList[k];
                            if (oElem.Name == "option")
                            {
                                rpo.Choices.Add(oElem.InnerXml);
                            }
                        }
                    }

                    if (pElem.Name == "key")
                    {
                        string[] answers = { "A", "B", "C", "D" };
                        int temp = int.Parse(pElem.InnerXml) - 1;
                        info.AnswerValue.Add(answers[temp]);
                    }
                }

                info.Tip.Add("");
                info.ScoreQuestion.Add(1.5);
                info.Score += 1.5;
                info.TimeQuestion.Add(1.5);
                info.ReplyTime += 1.5;
                info.DifficultQuestion.Add(0.4);
                info.Diffcult += 0.4;
            }

            info.Diffcult = info.Diffcult / info.QuestionCount;
            rpo.Info = info;

            return rpo;
        }

    }
}