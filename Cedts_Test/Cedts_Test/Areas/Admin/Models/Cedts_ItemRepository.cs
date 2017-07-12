using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Transactions;
using System.Web.Mvc;
using System.Text;
using System.IO;
using Webdiyer.WebControls.Mvc;
namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_ItemRepository : ICedts_ItemRepository
    {
        Cedts_Entities db;
        public Cedts_ItemRepository()
        {
            db = new Cedts_Entities();
        }

        XmlDocument doc;
        XmlNode node;
        XmlElement elem;

        #region 成员
        /// <summary>
        /// 新增听力理解XML－－“单句听力”，“长对话”，“短对话”，“听力短文理解”
        /// </summary>
        /// <param name="listen">组XML文档信息</param>
        void ICedts_ItemRepository.AddItem(Listen listen)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", listen.Info.ItemID.ToString());
            elem.SetAttribute("PartType", listen.Info.PartType);
            elem.SetAttribute("ItemType", listen.Info.ItemType);
            elem.SetAttribute("ItemType_CN", listen.Info.ItemType_CN);
            elem.SetAttribute("Score", listen.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", listen.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", listen.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", listen.Info.QustionInterval);
            elem.SetAttribute("Course", listen.Info.Course);
            elem.SetAttribute("Unit", listen.Info.Unit);
            doc.AppendChild(elem);



            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            if (listen.SoundFile != null)
            {
                XmlElement soundFile = doc.CreateElement("soundFile");//创建一个<soundFile>节点
                soundFile.InnerText = listen.SoundFile;
                AssessmentItem.AppendChild(soundFile);//添加到<AssessmentItem>节点中
            }
            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", listen.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            for (int i = 0; i < listen.Info.QuestionCount; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);

                XmlElement choices = doc.CreateElement("choices");
                question.AppendChild(choices);

                XmlElement choice1 = doc.CreateElement("choice");
                choice1.SetAttribute("id", "A");
                choice1.InnerText = listen.Choices[(0 + i * 4)];
                choices.AppendChild(choice1);


                XmlElement choice2 = doc.CreateElement("choice");
                choice2.SetAttribute("id", "B");
                choice2.InnerText = listen.Choices[(1 + i * 4)];
                choices.AppendChild(choice2);

                XmlElement choice3 = doc.CreateElement("choice");
                choice3.SetAttribute("id", "C");
                choice3.InnerText = listen.Choices[(2 + i * 4)];
                choices.AppendChild(choice3);

                XmlElement choice4 = doc.CreateElement("choice");
                choice4.SetAttribute("id", "D");
                choice4.InnerText = listen.Choices[(3 + i * 4)];
                choices.AppendChild(choice4);

                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = listen.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = listen.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = listen.Info.Tip[i];
                question.AppendChild(tip);

                XmlElement problem = doc.CreateElement("Script");
                problem.InnerText = listen.Info.Problem[i];
                question.AppendChild(problem);

                XmlElement sound = doc.CreateElement("Sound");
                sound.InnerText = listen.Info.questionSound[i];
                sound.SetAttribute("duration", listen.Info.timeInterval[i].ToString());
                question.AppendChild(sound);
            }

            XmlElement script = doc.CreateElement("Script");
            script.InnerText = listen.Script;
            AssessmentItem.AppendChild(script);

            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + listen.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }

        /// <summary>
        /// 新增复合型听力
        /// </summary>
        /// <param name="Complex">组复合型听力XML文档信息</param>
        void ICedts_ItemRepository.AddComplexItem(Listen Complex)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", Complex.Info.ItemID.ToString());
            elem.SetAttribute("PartType", Complex.Info.PartType);
            elem.SetAttribute("ItemType", Complex.Info.ItemType);
            elem.SetAttribute("ItemType_CN", Complex.Info.ItemType_CN);
            elem.SetAttribute("Score", Complex.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", Complex.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", Complex.Info.Diffcult.ToString());
            elem.SetAttribute("Course", Complex.Info.Course);
            elem.SetAttribute("Unit", Complex.Info.Unit);
            elem.SetAttribute("QustionInterval", Complex.Info.QustionInterval);
            doc.AppendChild(elem);



            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement script = doc.CreateElement("Content");
            script.InnerText = Complex.Script;
            AssessmentItem.AppendChild(script);
            XmlElement soundFile = doc.CreateElement("SoundFile");//创建一个<soundFile>节点
            soundFile.InnerText = Complex.SoundFile;
            AssessmentItem.AppendChild(soundFile);//添加到<AssessmentItem>节点中
            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", Complex.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            for (int i = 0; i < Complex.Info.QuestionCount; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);
                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = Complex.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = Complex.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = Complex.Info.Tip[i];
                question.AppendChild(tip);
            }


            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + Complex.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }

        /// <summary>
        /// 新增快速阅读
        /// </summary>
        /// <param name="Sspc">组快速阅读XMl文档信息</param>
        void ICedts_ItemRepository.AddSspcItem(SkimmingScanningPartCompletion Sspc)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", Sspc.Info.ItemID.ToString());
            elem.SetAttribute("PartType", Sspc.Info.PartType);
            elem.SetAttribute("ItemType", Sspc.Info.ItemType);
            elem.SetAttribute("ItemType_CN", Sspc.Info.ItemType_CN);
            elem.SetAttribute("Score", Sspc.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", Sspc.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", Sspc.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", Sspc.Info.QustionInterval);
            elem.SetAttribute("Course", Sspc.Info.Course);
            elem.SetAttribute("Unit", Sspc.Info.Unit);
            doc.AppendChild(elem);



            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement Content = doc.CreateElement("Content");
            Content.InnerText = Sspc.Content;
            AssessmentItem.AppendChild(Content);

            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", Sspc.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            int temp = Sspc.Choices.Count / Sspc.ChoiceNum;

            for (int i = 0; i < Sspc.ChoiceNum; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);

                XmlElement prompt = doc.CreateElement("prompt");
                prompt.InnerText = Sspc.Info.Problem[i];
                question.AppendChild(prompt);

                XmlElement choices = doc.CreateElement("choices");
                question.AppendChild(choices);

                XmlElement choice1 = doc.CreateElement("choice");
                choice1.SetAttribute("id", "A");
                choice1.InnerText = Sspc.Choices[(0 + i * temp)];
                choices.AppendChild(choice1);


                XmlElement choice2 = doc.CreateElement("choice");
                choice2.SetAttribute("id", "B");
                choice2.InnerText = Sspc.Choices[(1 + i * temp)];
                choices.AppendChild(choice2);

                XmlElement choice3 = doc.CreateElement("choice");
                choice3.SetAttribute("id", "C");
                choice3.InnerText = Sspc.Choices[(2 + i * temp)];
                choices.AppendChild(choice3);

                if (temp == 4)
                {
                    XmlElement choice4 = doc.CreateElement("choice");
                    choice4.SetAttribute("id", "D");
                    choice4.InnerText = Sspc.Choices[(3 + i * temp)];
                    choices.AppendChild(choice4);
                }

                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = Sspc.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = Sspc.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = Sspc.Info.Tip[i];
                question.AppendChild(tip);
            }

            for (int j = 0; j < Sspc.TermNum; j++)
            {
                XmlElement question1 = doc.CreateElement("question");
                question1.SetAttribute("id", (j + Sspc.ChoiceNum).ToString());
                Questions.AppendChild(question1);

                XmlElement prompt1 = doc.CreateElement("Prompt");
                prompt1.InnerText = Sspc.Info.Problem[j + Sspc.ChoiceNum];
                question1.AppendChild(prompt1);

                XmlElement Answer1 = doc.CreateElement("Answer");
                question1.AppendChild(Answer1);

                XmlElement value1 = doc.CreateElement("Value");
                value1.InnerText = Sspc.Info.AnswerValue[j + Sspc.ChoiceNum];
                Answer1.AppendChild(value1);

                XmlElement response1 = doc.CreateElement("Response");
                response1.InnerText = Sspc.Info.AnswerResposn;
                Answer1.AppendChild(response1);

                XmlElement tip1 = doc.CreateElement("Tip");
                tip1.InnerText = Sspc.Info.Tip[j + Sspc.ChoiceNum];
                question1.AppendChild(tip1);
            }

            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + Sspc.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }

        /// <summary>
        /// 新增快速阅读---选择题型
        /// </summary>
        /// <param name="Rpo">组快速阅读---选择题型XML文档信息</param>
        void ICedts_ItemRepository.AddRpoItem(ReadingPartOption Rpo)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", Rpo.Info.ItemID.ToString());
            elem.SetAttribute("PartType", Rpo.Info.PartType);
            elem.SetAttribute("ItemType", Rpo.Info.ItemType);
            elem.SetAttribute("ItemType_CN", Rpo.Info.ItemType_CN);
            elem.SetAttribute("Score", Rpo.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", Rpo.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", Rpo.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", Rpo.Info.QustionInterval);
            elem.SetAttribute("Course", Rpo.Info.Course);
            elem.SetAttribute("Unit", Rpo.Info.Unit);
            doc.AppendChild(elem);



            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement Content = doc.CreateElement("Content");
            Content.InnerText = Rpo.Content;
            AssessmentItem.AppendChild(Content);

            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", Rpo.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            for (int i = 0; i < Rpo.Info.QuestionCount; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);

                XmlElement prompt = doc.CreateElement("prompt");
                prompt.InnerText = Rpo.Info.Problem[i];
                question.AppendChild(prompt);

                XmlElement choices = doc.CreateElement("choices");
                question.AppendChild(choices);

                XmlElement choice1 = doc.CreateElement("choice");
                choice1.SetAttribute("id", "A");
                choice1.InnerText = Rpo.Choices[(0 + i * 4)];
                choices.AppendChild(choice1);


                XmlElement choice2 = doc.CreateElement("choice");
                choice2.SetAttribute("id", "B");
                choice2.InnerText = Rpo.Choices[(1 + i * 4)];
                choices.AppendChild(choice2);

                XmlElement choice3 = doc.CreateElement("choice");
                choice3.SetAttribute("id", "C");
                choice3.InnerText = Rpo.Choices[(2 + i * 4)];
                choices.AppendChild(choice3);

                XmlElement choice4 = doc.CreateElement("choice");
                choice4.SetAttribute("id", "D");
                choice4.InnerText = Rpo.Choices[(3 + i * 4)];
                choices.AppendChild(choice4);

                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = Rpo.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = Rpo.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = Rpo.Info.Tip[i];
                question.AppendChild(tip);
            }

            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + Rpo.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }

        /// <summary>
        /// 新增完形填空Cloze
        /// </summary>
        /// <param name="Cloze">组完形填空XML文档信息</param>
        void ICedts_ItemRepository.AddClozeItem(ClozePart Cloze)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", Cloze.Info.ItemID.ToString());
            elem.SetAttribute("PartType", Cloze.Info.PartType);
            elem.SetAttribute("ItemType", Cloze.Info.ItemType);
            elem.SetAttribute("ItemType_CN", Cloze.Info.ItemType_CN);
            elem.SetAttribute("Score", Cloze.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", Cloze.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", Cloze.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", Cloze.Info.QustionInterval);
            elem.SetAttribute("Course", Cloze.Info.Course);
            elem.SetAttribute("Unit", Cloze.Info.Unit);
            doc.AppendChild(elem);

            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement Content = doc.CreateElement("Content");
            Content.InnerText = Cloze.Content;
            AssessmentItem.AppendChild(Content);

            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", Cloze.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            for (int i = 0; i < Cloze.Info.QuestionCount; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);

                XmlElement choices = doc.CreateElement("choices");
                question.AppendChild(choices);

                XmlElement choice1 = doc.CreateElement("choice");
                choice1.SetAttribute("id", "A");
                choice1.InnerText = Cloze.Choices[(0 + i * 4)];
                choices.AppendChild(choice1);


                XmlElement choice2 = doc.CreateElement("choice");
                choice2.SetAttribute("id", "B");
                choice2.InnerText = Cloze.Choices[(1 + i * 4)];
                choices.AppendChild(choice2);

                XmlElement choice3 = doc.CreateElement("choice");
                choice3.SetAttribute("id", "C");
                choice3.InnerText = Cloze.Choices[(2 + i * 4)];
                choices.AppendChild(choice3);

                XmlElement choice4 = doc.CreateElement("choice");
                choice4.SetAttribute("id", "D");
                choice4.InnerText = Cloze.Choices[(3 + i * 4)];
                choices.AppendChild(choice4);

                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = Cloze.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = Cloze.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = Cloze.Info.Tip[i];
                question.AppendChild(tip);
            }

            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + Cloze.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }

        /// <summary>
        /// 新增阅读理解-精读题型
        /// </summary>
        /// <param name="Rpo">组阅读理解-精读题型XML文档信息</param>
        void ICedts_ItemRepository.AddIntensiveRead(ReadingPartOption Rpo)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", Rpo.Info.ItemID.ToString());
            elem.SetAttribute("PartType", Rpo.Info.PartType);
            elem.SetAttribute("ItemType", Rpo.Info.ItemType);
            elem.SetAttribute("ItemType_CN", Rpo.Info.ItemType_CN);
            elem.SetAttribute("Score", Rpo.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", Rpo.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", Rpo.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", Rpo.Info.QustionInterval);
            elem.SetAttribute("Course", Rpo.Info.Course);
            elem.SetAttribute("Unit", Rpo.Info.Unit);
            doc.AppendChild(elem);



            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement Content = doc.CreateElement("Content");
            Content.InnerText = Rpo.Content;
            AssessmentItem.AppendChild(Content);

            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", Rpo.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            for (int i = 0; i < Rpo.Info.QuestionCount; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);

                XmlElement prompt = doc.CreateElement("prompt");
                prompt.InnerText = Rpo.Info.Problem[i];
                question.AppendChild(prompt);

                XmlElement choices = doc.CreateElement("choices");
                question.AppendChild(choices);

                XmlElement choice1 = doc.CreateElement("choice");
                choice1.SetAttribute("id", "A");
                choice1.InnerText = Rpo.Choices[(0 + i * 4)];
                choices.AppendChild(choice1);


                XmlElement choice2 = doc.CreateElement("choice");
                choice2.SetAttribute("id", "B");
                choice2.InnerText = Rpo.Choices[(1 + i * 4)];
                choices.AppendChild(choice2);

                XmlElement choice3 = doc.CreateElement("choice");
                choice3.SetAttribute("id", "C");
                choice3.InnerText = Rpo.Choices[(2 + i * 4)];
                choices.AppendChild(choice3);

                XmlElement choice4 = doc.CreateElement("choice");
                choice4.SetAttribute("id", "D");
                choice4.InnerText = Rpo.Choices[(3 + i * 4)];
                choices.AppendChild(choice4);

                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = Rpo.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = Rpo.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = Rpo.Info.Tip[i];
                question.AppendChild(tip);
            }

            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + Rpo.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }

        /// <summary>
        /// 新增阅读理解-选词填空
        /// </summary>
        /// <param name="Rpc">组阅读理解-选词填空XML文档信息</param>
        void ICedts_ItemRepository.AddRpcItem(ReadingPartCompletion Rpc)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", Rpc.Info.ItemID.ToString());
            elem.SetAttribute("PartType", Rpc.Info.PartType);
            elem.SetAttribute("ItemType", Rpc.Info.ItemType);
            elem.SetAttribute("ItemType_CN", Rpc.Info.ItemType_CN);
            elem.SetAttribute("Score", Rpc.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", Rpc.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", Rpc.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", Rpc.Info.QustionInterval);
            elem.SetAttribute("Course", Rpc.Info.Course);
            elem.SetAttribute("Unit", Rpc.Info.Unit);
            doc.AppendChild(elem);



            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement Content = doc.CreateElement("Content");
            Content.InnerText = Rpc.Content;
            AssessmentItem.AppendChild(Content);

            XmlElement WordList = doc.CreateElement("WordList");
            AssessmentItem.AppendChild(WordList);

            XmlElement WordA = doc.CreateElement("Word");
            WordA.SetAttribute("id", "A");
            WordA.InnerText = Rpc.WordList[0];
            WordList.AppendChild(WordA);

            XmlElement WordB = doc.CreateElement("Word");
            WordB.SetAttribute("id", "B");
            WordB.InnerText = Rpc.WordList[1];
            WordList.AppendChild(WordB);

            XmlElement WordC = doc.CreateElement("Word");
            WordC.SetAttribute("id", "C");
            WordC.InnerText = Rpc.WordList[2];
            WordList.AppendChild(WordC);

            XmlElement WordD = doc.CreateElement("Word");
            WordA.SetAttribute("id", "D");
            WordA.InnerText = Rpc.WordList[3];
            WordList.AppendChild(WordA);

            XmlElement WordE = doc.CreateElement("Word");
            WordE.SetAttribute("id", "E");
            WordE.InnerText = Rpc.WordList[4];
            WordList.AppendChild(WordE);

            XmlElement WordF = doc.CreateElement("Word");
            WordF.SetAttribute("id", "F");
            WordF.InnerText = Rpc.WordList[5];
            WordList.AppendChild(WordF);

            XmlElement WordG = doc.CreateElement("Word");
            WordG.SetAttribute("id", "G");
            WordG.InnerText = Rpc.WordList[6];
            WordList.AppendChild(WordG);

            XmlElement WordH = doc.CreateElement("Word");
            WordH.SetAttribute("id", "H");
            WordH.InnerText = Rpc.WordList[7];
            WordList.AppendChild(WordH);

            XmlElement WordI = doc.CreateElement("Word");
            WordI.SetAttribute("id", "I");
            WordI.InnerText = Rpc.WordList[8];
            WordList.AppendChild(WordI);

            XmlElement WordJ = doc.CreateElement("Word");
            WordJ.SetAttribute("id", "J");
            WordJ.InnerText = Rpc.WordList[9];
            WordList.AppendChild(WordJ);

            XmlElement WordK = doc.CreateElement("Word");
            WordK.SetAttribute("id", "K");
            WordK.InnerText = Rpc.WordList[10];
            WordList.AppendChild(WordK);

            XmlElement WordL = doc.CreateElement("Word");
            WordL.SetAttribute("id", "L");
            WordL.InnerText = Rpc.WordList[11];
            WordList.AppendChild(WordL);

            XmlElement WordM = doc.CreateElement("Word");
            WordM.SetAttribute("id", "M");
            WordM.InnerText = Rpc.WordList[12];
            WordList.AppendChild(WordM);

            XmlElement WordN = doc.CreateElement("Word");
            WordN.SetAttribute("id", "N");
            WordN.InnerText = Rpc.WordList[13];
            WordList.AppendChild(WordN);

            XmlElement WordO = doc.CreateElement("Word");
            WordO.SetAttribute("id", "O");
            WordO.InnerText = Rpc.WordList[14];
            WordList.AppendChild(WordO);

            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", Rpc.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            for (int i = 0; i < Rpc.Info.QuestionCount; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);


                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = Rpc.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = Rpc.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = Rpc.Info.Tip[i];
                question.AppendChild(tip);
            }


            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + Rpc.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);

        }

        /// <summary>
        /// 修改听力理解XML－－“单句听力”，“长对话”，“短对话”，“听力短文理解”
        /// </summary>
        /// <param name="listen">组XML文档信息</param>
        void ICedts_ItemRepository.EditItem(Listen listen)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", listen.Info.ItemID.ToString());
            elem.SetAttribute("PartType", listen.Info.PartType);
            elem.SetAttribute("ItemType", listen.Info.ItemType);
            elem.SetAttribute("ItemType_CN", listen.Info.ItemType_CN);
            elem.SetAttribute("Score", listen.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", listen.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", listen.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", listen.Info.QustionInterval);
            elem.SetAttribute("Course", listen.Info.Course);
            elem.SetAttribute("Unit", listen.Info.Unit);
            doc.AppendChild(elem);



            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement soundFile = doc.CreateElement("soundFile");//创建一个<soundFile>节点
            soundFile.InnerText = listen.SoundFile;
            AssessmentItem.AppendChild(soundFile);//添加到<AssessmentItem>节点中
            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", listen.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            for (int i = 0; i < listen.Info.QuestionCount; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);

                XmlElement choices = doc.CreateElement("choices");
                question.AppendChild(choices);

                XmlElement choice1 = doc.CreateElement("choice");
                choice1.SetAttribute("id", "A");
                choice1.InnerText = listen.Choices[(0 + i * 4)];
                choices.AppendChild(choice1);


                XmlElement choice2 = doc.CreateElement("choice");
                choice2.SetAttribute("id", "B");
                choice2.InnerText = listen.Choices[(1 + i * 4)];
                choices.AppendChild(choice2);

                XmlElement choice3 = doc.CreateElement("choice");
                choice3.SetAttribute("id", "C");
                choice3.InnerText = listen.Choices[(2 + i * 4)];
                choices.AppendChild(choice3);

                XmlElement choice4 = doc.CreateElement("choice");
                choice4.SetAttribute("id", "D");
                choice4.InnerText = listen.Choices[(3 + i * 4)];
                choices.AppendChild(choice4);

                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = listen.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = listen.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = listen.Info.Tip[i];
                question.AppendChild(tip);

                XmlElement problem = doc.CreateElement("Script");
                problem.InnerText = listen.Info.Problem[i];
                question.AppendChild(problem);
            }

            XmlElement script = doc.CreateElement("Script");
            script.InnerText = listen.Script;
            AssessmentItem.AppendChild(script);

            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + listen.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }

        /// <summary>
        /// 编辑修改复合型听力信息
        /// </summary>
        /// <param name="Complex">组XML文档信息</param>
        void ICedts_ItemRepository.EditComplex(Listen Complex)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", Complex.Info.ItemID.ToString());
            elem.SetAttribute("PartType", Complex.Info.PartType);
            elem.SetAttribute("ItemType", Complex.Info.ItemType);
            elem.SetAttribute("ItemType_CN", Complex.Info.ItemType_CN);
            elem.SetAttribute("Score", Complex.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", Complex.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", Complex.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", Complex.Info.QustionInterval);
            elem.SetAttribute("Course", Complex.Info.Course);
            elem.SetAttribute("Unit", Complex.Info.Unit);
            doc.AppendChild(elem);



            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement script = doc.CreateElement("Content");
            script.InnerText = Complex.Script;
            AssessmentItem.AppendChild(script);
            XmlElement soundFile = doc.CreateElement("SoundFile");//创建一个<soundFile>节点
            soundFile.InnerText = Complex.SoundFile;
            AssessmentItem.AppendChild(soundFile);//添加到<AssessmentItem>节点中
            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", Complex.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            for (int i = 0; i < Complex.Info.QuestionCount; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);
                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = Complex.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = Complex.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = Complex.Info.Tip[i];
                question.AppendChild(tip);
            }


            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + Complex.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }

        /// <summary>
        /// 编辑修该快速阅读信息
        /// </summary>
        /// <param name="Sspc">组XML文档信息</param>
        void ICedts_ItemRepository.EditSspc(SkimmingScanningPartCompletion Sspc)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", Sspc.Info.ItemID.ToString());
            elem.SetAttribute("PartType", Sspc.Info.PartType);
            elem.SetAttribute("ItemType", Sspc.Info.ItemType);
            elem.SetAttribute("ItemType_CN", Sspc.Info.ItemType_CN);
            elem.SetAttribute("Score", Sspc.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", Sspc.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", Sspc.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", Sspc.Info.QustionInterval);
            elem.SetAttribute("Course", Sspc.Info.Course);
            elem.SetAttribute("Unit", Sspc.Info.Unit);
            doc.AppendChild(elem);



            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement Content = doc.CreateElement("Content");
            Content.InnerText = Sspc.Content;
            AssessmentItem.AppendChild(Content);

            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", Sspc.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);
            int tempnum = Sspc.Choices.Count / Sspc.ChoiceNum;
            for (int i = 0; i < Sspc.ChoiceNum; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);

                XmlElement prompt = doc.CreateElement("prompt");
                prompt.InnerText = Sspc.Info.Problem[i];
                question.AppendChild(prompt);

                XmlElement choices = doc.CreateElement("choices");
                question.AppendChild(choices);

                XmlElement choice1 = doc.CreateElement("choice");
                choice1.SetAttribute("id", "A");
                choice1.InnerText = Sspc.Choices[(0 + i * tempnum)];
                choices.AppendChild(choice1);


                XmlElement choice2 = doc.CreateElement("choice");
                choice2.SetAttribute("id", "B");
                choice2.InnerText = Sspc.Choices[(1 + i * tempnum)];
                choices.AppendChild(choice2);

                XmlElement choice3 = doc.CreateElement("choice");
                choice3.SetAttribute("id", "C");
                choice3.InnerText = Sspc.Choices[(2 + i * tempnum)];
                choices.AppendChild(choice3);

                if (tempnum == 4)
                {
                    XmlElement choice4 = doc.CreateElement("choice");
                    choice4.SetAttribute("id", "D");
                    choice4.InnerText = Sspc.Choices[(3 + i * tempnum)];
                    choices.AppendChild(choice4);
                }

                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = Sspc.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = Sspc.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = Sspc.Info.Tip[i];
                question.AppendChild(tip);
            }

            for (int j = 0; j < Sspc.TermNum; j++)
            {
                XmlElement question1 = doc.CreateElement("question");
                question1.SetAttribute("id", (j + Sspc.ChoiceNum).ToString());
                Questions.AppendChild(question1);

                XmlElement prompt1 = doc.CreateElement("Prompt");
                prompt1.InnerText = Sspc.Info.Problem[j + Sspc.ChoiceNum];
                question1.AppendChild(prompt1);

                XmlElement Answer1 = doc.CreateElement("Answer");
                question1.AppendChild(Answer1);

                XmlElement value1 = doc.CreateElement("Value");
                value1.InnerText = Sspc.Info.AnswerValue[j + Sspc.ChoiceNum];
                Answer1.AppendChild(value1);

                XmlElement response1 = doc.CreateElement("Response");
                response1.InnerText = Sspc.Info.AnswerResposn;
                Answer1.AppendChild(response1);

                XmlElement tip1 = doc.CreateElement("Tip");
                tip1.InnerText = Sspc.Info.Tip[j + Sspc.ChoiceNum];
                question1.AppendChild(tip1);
            }

            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + Sspc.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }

        /// <summary>
        /// 编辑修改完型填空信息
        /// </summary>
        /// <param name="Cloze">组XMl文档信息</param>
        void ICedts_ItemRepository.EditCloze(ClozePart Cloze)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", Cloze.Info.ItemID.ToString());
            elem.SetAttribute("PartType", Cloze.Info.PartType);
            elem.SetAttribute("ItemType", Cloze.Info.ItemType);
            elem.SetAttribute("ItemType_CN", Cloze.Info.ItemType_CN);
            elem.SetAttribute("Score", Cloze.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", Cloze.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", Cloze.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", Cloze.Info.QustionInterval);
            elem.SetAttribute("Course", Cloze.Info.Course);
            elem.SetAttribute("Unit", Cloze.Info.Unit);
            doc.AppendChild(elem);

            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement Content = doc.CreateElement("Content");
            Content.InnerText = Cloze.Content;
            AssessmentItem.AppendChild(Content);

            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", Cloze.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            for (int i = 0; i < Cloze.Info.QuestionCount; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);

                XmlElement choices = doc.CreateElement("choices");
                question.AppendChild(choices);

                XmlElement choice1 = doc.CreateElement("choice");
                choice1.SetAttribute("id", "A");
                choice1.InnerText = Cloze.Choices[(0 + i * 4)];
                choices.AppendChild(choice1);


                XmlElement choice2 = doc.CreateElement("choice");
                choice2.SetAttribute("id", "B");
                choice2.InnerText = Cloze.Choices[(1 + i * 4)];
                choices.AppendChild(choice2);

                XmlElement choice3 = doc.CreateElement("choice");
                choice3.SetAttribute("id", "C");
                choice3.InnerText = Cloze.Choices[(2 + i * 4)];
                choices.AppendChild(choice3);

                XmlElement choice4 = doc.CreateElement("choice");
                choice4.SetAttribute("id", "D");
                choice4.InnerText = Cloze.Choices[(3 + i * 4)];
                choices.AppendChild(choice4);

                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = Cloze.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = Cloze.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = Cloze.Info.Tip[i];
                question.AppendChild(tip);
            }

            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + Cloze.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }

        /// <summary>
        /// 编辑修改阅读理解-选择题型信息
        /// </summary>
        /// <param name="Rpo">组XML文档信息</param>
        void ICedts_ItemRepository.EditRpo(ReadingPartOption Rpo)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", Rpo.Info.ItemID.ToString());
            elem.SetAttribute("PartType", Rpo.Info.PartType);
            elem.SetAttribute("ItemType", Rpo.Info.ItemType);
            elem.SetAttribute("ItemType_CN", Rpo.Info.ItemType_CN);
            elem.SetAttribute("Score", Rpo.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", Rpo.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", Rpo.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", Rpo.Info.QustionInterval);
            elem.SetAttribute("Course", Rpo.Info.Course);
            elem.SetAttribute("Unit", Rpo.Info.Unit);
            doc.AppendChild(elem);



            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement Content = doc.CreateElement("Content");
            Content.InnerText = Rpo.Content;
            AssessmentItem.AppendChild(Content);

            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", Rpo.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            for (int i = 0; i < Rpo.Info.QuestionCount; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);

                XmlElement prompt = doc.CreateElement("prompt");
                prompt.InnerText = Rpo.Info.Problem[i];
                question.AppendChild(prompt);

                XmlElement choices = doc.CreateElement("choices");
                question.AppendChild(choices);

                XmlElement choice1 = doc.CreateElement("choice");
                choice1.SetAttribute("id", "A");
                choice1.InnerText = Rpo.Choices[(0 + i * 4)];
                choices.AppendChild(choice1);


                XmlElement choice2 = doc.CreateElement("choice");
                choice2.SetAttribute("id", "B");
                choice2.InnerText = Rpo.Choices[(1 + i * 4)];
                choices.AppendChild(choice2);

                XmlElement choice3 = doc.CreateElement("choice");
                choice3.SetAttribute("id", "C");
                choice3.InnerText = Rpo.Choices[(2 + i * 4)];
                choices.AppendChild(choice3);

                XmlElement choice4 = doc.CreateElement("choice");
                choice4.SetAttribute("id", "D");
                choice4.InnerText = Rpo.Choices[(3 + i * 4)];
                choices.AppendChild(choice4);

                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = Rpo.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = Rpo.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = Rpo.Info.Tip[i];
                question.AppendChild(tip);
            }

            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + Rpo.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }

        /// <summary>
        /// 编辑修改阅读理解-选词填空系你想
        /// </summary>
        /// <param name="Rpc">组XML文档信息</param>
        void ICedts_ItemRepository.EditRpc(ReadingPartCompletion Rpc)
        {
            doc = new XmlDocument();
            //加入XML的声明段落
            node = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(node);


            //加入一个根元素
            elem = doc.CreateElement("", "AssessmentItem", "");
            elem.SetAttribute("ItemID", Rpc.Info.ItemID.ToString());
            elem.SetAttribute("PartType", Rpc.Info.PartType);
            elem.SetAttribute("ItemType", Rpc.Info.ItemType);
            elem.SetAttribute("ItemType_CN", Rpc.Info.ItemType_CN);
            elem.SetAttribute("Score", Rpc.Info.Score.ToString());
            elem.SetAttribute("ReplyTime", Rpc.Info.ReplyTime.ToString());
            elem.SetAttribute("Diffcult", Rpc.Info.Diffcult.ToString());
            elem.SetAttribute("QustionInterval", Rpc.Info.QustionInterval);
            elem.SetAttribute("Course", Rpc.Info.Course);
            elem.SetAttribute("Unit", Rpc.Info.Unit);
            doc.AppendChild(elem);



            XmlNode AssessmentItem = doc.SelectSingleNode("AssessmentItem");//查找<AssessmentItem> 
            XmlElement Content = doc.CreateElement("Content");
            Content.InnerText = Rpc.Content;
            AssessmentItem.AppendChild(Content);

            XmlElement WordList = doc.CreateElement("WordList");
            AssessmentItem.AppendChild(WordList);

            XmlElement WordA = doc.CreateElement("Word");
            WordA.SetAttribute("id", "A");
            WordA.InnerText = Rpc.WordList[0];
            WordList.AppendChild(WordA);

            XmlElement WordB = doc.CreateElement("Word");
            WordB.SetAttribute("id", "B");
            WordB.InnerText = Rpc.WordList[1];
            WordList.AppendChild(WordB);

            XmlElement WordC = doc.CreateElement("Word");
            WordC.SetAttribute("id", "C");
            WordC.InnerText = Rpc.WordList[2];
            WordList.AppendChild(WordC);

            XmlElement WordD = doc.CreateElement("Word");
            WordA.SetAttribute("id", "D");
            WordA.InnerText = Rpc.WordList[3];
            WordList.AppendChild(WordA);

            XmlElement WordE = doc.CreateElement("Word");
            WordE.SetAttribute("id", "E");
            WordE.InnerText = Rpc.WordList[4];
            WordList.AppendChild(WordE);

            XmlElement WordF = doc.CreateElement("Word");
            WordF.SetAttribute("id", "F");
            WordF.InnerText = Rpc.WordList[5];
            WordList.AppendChild(WordF);

            XmlElement WordG = doc.CreateElement("Word");
            WordG.SetAttribute("id", "G");
            WordG.InnerText = Rpc.WordList[6];
            WordList.AppendChild(WordG);

            XmlElement WordH = doc.CreateElement("Word");
            WordH.SetAttribute("id", "H");
            WordH.InnerText = Rpc.WordList[7];
            WordList.AppendChild(WordH);

            XmlElement WordI = doc.CreateElement("Word");
            WordI.SetAttribute("id", "I");
            WordI.InnerText = Rpc.WordList[8];
            WordList.AppendChild(WordI);

            XmlElement WordJ = doc.CreateElement("Word");
            WordJ.SetAttribute("id", "J");
            WordJ.InnerText = Rpc.WordList[9];
            WordList.AppendChild(WordJ);

            XmlElement WordK = doc.CreateElement("Word");
            WordK.SetAttribute("id", "K");
            WordK.InnerText = Rpc.WordList[10];
            WordList.AppendChild(WordK);

            XmlElement WordL = doc.CreateElement("Word");
            WordL.SetAttribute("id", "L");
            WordL.InnerText = Rpc.WordList[11];
            WordList.AppendChild(WordL);

            XmlElement WordM = doc.CreateElement("Word");
            WordM.SetAttribute("id", "M");
            WordM.InnerText = Rpc.WordList[12];
            WordList.AppendChild(WordM);

            XmlElement WordN = doc.CreateElement("Word");
            WordN.SetAttribute("id", "N");
            WordN.InnerText = Rpc.WordList[13];
            WordList.AppendChild(WordN);

            XmlElement WordO = doc.CreateElement("Word");
            WordO.SetAttribute("id", "O");
            WordO.InnerText = Rpc.WordList[14];
            WordList.AppendChild(WordO);

            XmlElement Questions = doc.CreateElement("Questions");
            Questions.SetAttribute("Count", Rpc.Info.QuestionCount.ToString());
            AssessmentItem.AppendChild(Questions);

            for (int i = 0; i < Rpc.Info.QuestionCount; i++)
            {
                XmlElement question = doc.CreateElement("question");
                question.SetAttribute("id", i.ToString());
                Questions.AppendChild(question);


                XmlElement Answer = doc.CreateElement("Answer");
                question.AppendChild(Answer);

                XmlElement value = doc.CreateElement("Value");
                value.InnerText = Rpc.Info.AnswerValue[i];
                Answer.AppendChild(value);

                XmlElement response = doc.CreateElement("Response");
                response.InnerText = Rpc.Info.AnswerResposn;
                Answer.AppendChild(response);

                XmlElement tip = doc.CreateElement("Tip");
                tip.InnerText = Rpc.Info.Tip[i];
                question.AppendChild(tip);
            }


            string path = AppDomain.CurrentDomain.BaseDirectory + "ExaminationItemLibrary\\" + Rpc.Info.ItemID + ".xml";
            //保存创建好的XML文档
            doc.Save(path);
        }
        /// <summary>
        /// 新增AssessmentItem信息
        /// </summary>
        /// <param name="item">AssessmentItem表基本信息</param>
        void ICedts_ItemRepository.Create(CEDTS_AssessmentItem item)
        {
            db.AddToCEDTS_AssessmentItem(item);
            db.SaveChanges();
        }

        /// <summary>
        /// 删除AssessmentItem信息
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>1</returns>
        int ICedts_ItemRepository.DeleteItem(Guid? id)
        {
            db.DeleteAssessmentItem(id);
            return 1;
        }

        public List<ExaminationItem> GetAllItems()
        {
            IQueryable<ExaminationItem> q = from m in db.CEDTS_AssessmentItem
                                            from s in db.CEDTS_ItemType
                                            from g in db.CEDTS_PaperAssessment
                                            from h in db.CEDTS_Paper
                                            orderby m.SaveTime descending
                                            where m.ItemTypeID == s.ItemTypeID
                                            && m.AssessmentItemID == g.AssessmentItemID
                                            && g.PaperID == h.PaperID
                                            select new ExaminationItem
                                            {
                                                AssessmentItemID = m.AssessmentItemID,
                                                ItemName = s.TypeName_CN,
                                                Difficult = m.Difficult.Value,
                                                Duration = m.Duration.Value,
                                                Score = m.Score.Value,
                                                SaveTime = m.SaveTime.Value,
                                                UpdateTime = m.UpdateTime.Value,
                                                Count = m.Count.Value,
                                                PaperName = h.Title
                                            };

            IQueryable<ExaminationItem> p = from s in db.CEDTS_ItemType
                                            from a in db.CEDTS_AssessmentItem
                                            orderby a.SaveTime descending
                                            where !(from b in db.CEDTS_PaperAssessment select b.AssessmentItemID).Contains(a.AssessmentItemID)
                                            && a.ItemTypeID == s.ItemTypeID
                                            select new ExaminationItem
                                            {
                                                AssessmentItemID = a.AssessmentItemID,
                                                ItemName = s.TypeName_CN,
                                                Difficult = a.Difficult.Value,
                                                Duration = a.Duration.Value,
                                                Score = a.Score.Value,
                                                SaveTime = a.SaveTime.Value,
                                                UpdateTime = a.UpdateTime.Value,
                                                Count = a.Count.Value,
                                                PaperName = ""
                                            };

            return p.Concat(q).OrderByDescending(m => m.SaveTime).ToList();
        }

        PagedList<ExaminationItem> ICedts_ItemRepository.SelectItemsByCondition(int? id, string condition, string txt)
        {
            List<ExaminationItem> examitem = GetAllItems();
            if (examitem.Count == 0)
            {
                condition = string.Empty;
            }
            int defaultPageSize = 10;
            if (condition == "1")
            {
                IQueryable<ExaminationItem> q = examitem.Where(p => p.ItemName.Contains(txt)).ToList().AsQueryable<ExaminationItem>();
                PagedList<ExaminationItem> ss = q.ToPagedList(id ?? 1, defaultPageSize);

                return ss;
            }
            if (condition == "2")
            {
                IQueryable<ExaminationItem> q = examitem.Where(p => p.PaperName.Contains(txt)).ToList().AsQueryable<ExaminationItem>();
                PagedList<ExaminationItem> ss = q.ToPagedList(id ?? 1, defaultPageSize);

                return ss;
            }
            else
            {
                var query = examitem;
                IQueryable<ExaminationItem> d = query.AsQueryable<ExaminationItem>();
                PagedList<ExaminationItem> ss = d.ToPagedList(id ?? 1, defaultPageSize);
                return ss;
            }
        }



        /// <summary>
        /// 查询当前点”听力短文理解“，“单句听力”，"长对话"，“短对话”的信息
        /// </summary>
        /// <param name="item">AssessmentItemID</param>
        /// <returns>当前点击”听力短文理解“，“单句听力”，"长对话"，“短对话”的信息</returns>
        Listen ICedts_ItemRepository.SelectAll(Guid item)
        {
            var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
            var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).FirstOrDefault();
            var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).FirstOrDefault();
            var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).FirstOrDefault();



            ItemBassInfo info = new ItemBassInfo();
            info.AnswerValue = new List<string>();
            info.Problem = new List<string>();
            info.Tip = new List<string>();
            info.QuestionID = new List<Guid>();
            info.DifficultQuestion = new List<double>();
            info.ScoreQuestion = new List<double>();
            Listen listen = new Listen();
            info.Knowledge = new List<string>();
            info.KnowledgeID = new List<string>();
            listen.Choices = new List<string>();
            info.TimeQuestion = new List<double>();
            info.questionSound = new List<string>();
            info.timeInterval = new List<int>();
            info.ItemID = item;
            info.Count = Convert.ToInt32(assess.Count);
            info.ItemType = Itemtype.TypeName;
            info.ItemType_CN = Itemtype.ItemTypeID.ToString();
            info.PartType = PartType.TypeName;
            info.QuestionCount = Convert.ToInt32(assess.QuestionCount);
            info.ReplyTime = Convert.ToInt32(assess.Duration);
            info.Diffcult = Convert.ToDouble(assess.Difficult);
            info.Score = Convert.ToInt32(assess.Score);
            info.QustionInterval = assess.Interval.ToString();

            listen.Script = assess.Original;
            listen.SoundFile = item.ToString();

            string QkID = null;
            foreach (var qu in question)
            {
                var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                foreach (var q in QkList)
                {
                    QkID += q + ",";
                }
            }
            QkID = QkID.Substring(0, QkID.LastIndexOf(","));
            info.QuestionKnowledgeID = QkID;

            foreach (var list in question)
            {
                info.QuestionID.Add(list.QuestionID);
                info.Problem.Add(list.QuestionContent);
                info.ScoreQuestion.Add(list.Score.Value);
                info.TimeQuestion.Add(list.Duration.Value);
                info.DifficultQuestion.Add(list.Difficult.Value);
                info.questionSound.Add(list.Sound);
                if (assess.ItemTypeID != 2 && assess.ItemTypeID != 5)
                {
                    info.timeInterval.Add(list.Interval.Value);
                }
                listen.Choices.Add(list.ChooseA);
                listen.Choices.Add(list.ChooseB);
                listen.Choices.Add(list.ChooseC);
                listen.Choices.Add(list.ChooseD);
                info.AnswerValue.Add(list.Answer);

                info.Tip.Add(list.Analyze);
                string name = string.Empty;
                string id = string.Empty;


                var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                foreach (var Knowledge in KnowledgeList)
                {
                    var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).FirstOrDefault();
                    id += KnowledgeName.KnowledgePointID + ",";
                    name += KnowledgeName.Title + ",";
                }
                name = name.Substring(0, name.LastIndexOf(","));
                id = id.Substring(0, id.LastIndexOf(','));
                info.KnowledgeID.Add(id);
                info.Knowledge.Add(name);
            }
            listen.Info = info;
            return listen;
        }

        /// <summary>
        /// 查询当前点击的复合型听力信息
        /// </summary>
        /// <param name="item">AssessmentItemID</param>
        /// <returns>当前点击的复合型听力信息</returns>
        Listen ICedts_ItemRepository.SelectComplex(Guid item)
        {
            var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
            var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).First();
            var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).First();
            var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).First();

            ItemBassInfo info = new ItemBassInfo();
            info.AnswerValue = new List<string>();

            info.Tip = new List<string>();
            info.QuestionID = new List<Guid>();
            info.DifficultQuestion = new List<double>();
            info.ScoreQuestion = new List<double>();
            Listen Complex = new Listen();
            info.Knowledge = new List<string>();
            info.KnowledgeID = new List<string>();
            info.TimeQuestion = new List<double>();
            info.questionSound = new List<string>();
            info.timeInterval = new List<int>();
            info.ItemID = item;
            info.Count = Convert.ToInt32(assess.Count);
            info.ItemType = Itemtype.TypeName;
            info.ItemType_CN = Itemtype.ItemTypeID.ToString();
            info.PartType = PartType.TypeName;
            info.QuestionCount = Convert.ToInt32(assess.QuestionCount);
            info.ReplyTime = Convert.ToInt32(assess.Duration);
            info.Diffcult = Convert.ToDouble(assess.Difficult);
            info.Score = Convert.ToInt32(assess.Score);
            info.QustionInterval = assess.Interval.ToString();
            Complex.Script = assess.Original;
            Complex.SoundFile = item.ToString();

            string QkID = null;
            foreach (var qu in question)
            {
                var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                foreach (var q in QkList)
                {
                    QkID += q + ",";
                }
            }
            QkID = QkID.Substring(0, QkID.LastIndexOf(","));
            info.QuestionKnowledgeID = QkID;

            foreach (var list in question)
            {
                info.QuestionID.Add(list.QuestionID);
                info.ScoreQuestion.Add(list.Score.Value);
                info.TimeQuestion.Add(list.Duration.Value);
                info.DifficultQuestion.Add(list.Difficult.Value);
                info.questionSound.Add(list.Sound);
                info.AnswerValue.Add(list.Answer);
                info.Tip.Add(list.Analyze);
                string name = string.Empty;
                string id = string.Empty;


                var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                foreach (var Knowledge in KnowledgeList)
                {
                    var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                    id += KnowledgeName.KnowledgePointID + ",";
                    name += KnowledgeName.Title + ",";
                }
                name = name.Substring(0, name.LastIndexOf(","));
                id = id.Substring(0, id.LastIndexOf(','));
                info.KnowledgeID.Add(id);
                info.Knowledge.Add(name);
            }
            Complex.Info = info;
            return Complex;
        }

        /// <summary>
        /// 查询当前点击的快速阅读信息
        /// </summary>
        /// <param name="item">AssessmentItemID</param>
        /// <returns>当前点击的快速阅读信息</returns>
        SkimmingScanningPartCompletion ICedts_ItemRepository.SelectSspc(Guid item)
        {
            var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
            var ChoiceNum = (from m in db.CEDTS_Question where m.AssessmentItemID == item && m.ChooseA != "" orderby m.Order select m).ToList();
            var TermNum = (from m in db.CEDTS_Question where m.AssessmentItemID == item && m.ChooseA == "" orderby m.Order select m).ToList();
            var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).First();
            var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).First();
            var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).First();


            ItemBassInfo info = new ItemBassInfo();
            info.AnswerValue = new List<string>();
            info.Problem = new List<string>();
            info.Tip = new List<string>();
            info.QuestionID = new List<Guid>();
            info.DifficultQuestion = new List<double>();
            info.ScoreQuestion = new List<double>();
            SkimmingScanningPartCompletion Sspc = new SkimmingScanningPartCompletion();
            info.Knowledge = new List<string>();
            info.KnowledgeID = new List<string>();
            Sspc.Choices = new List<string>();
            info.TimeQuestion = new List<double>();
            info.ItemID = item;
            info.Count = Convert.ToInt32(assess.Count);
            info.ItemType = Itemtype.TypeName;
            info.ItemType_CN = Itemtype.ItemTypeID.ToString();
            info.PartType = PartType.TypeName;
            info.QuestionCount = Convert.ToInt32(assess.QuestionCount);
            info.ReplyTime = Convert.ToInt32(assess.Duration);
            info.Diffcult = Convert.ToDouble(assess.Difficult);
            info.Score = Convert.ToInt32(assess.Score);
            Sspc.ChoiceNum = ChoiceNum.Count;
            Sspc.TermNum = TermNum.Count;
            Sspc.Content = assess.Original;

            string QkID = null;
            foreach (var qu in question)
            {
                var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                foreach (var q in QkList)
                {
                    QkID += q + ",";
                }
            }
            QkID = QkID.Substring(0, QkID.LastIndexOf(","));
            info.QuestionKnowledgeID = QkID;

            foreach (var choic in ChoiceNum)
            {
                info.QuestionID.Add(choic.QuestionID);
                info.Problem.Add(choic.QuestionContent);
                info.ScoreQuestion.Add(choic.Score.Value);
                info.TimeQuestion.Add(choic.Duration.Value);
                info.DifficultQuestion.Add(choic.Difficult.Value);
                Sspc.Choices.Add(choic.ChooseA);
                Sspc.Choices.Add(choic.ChooseB);
                Sspc.Choices.Add(choic.ChooseC);
                Sspc.Choices.Add(choic.ChooseD);
                info.AnswerValue.Add(choic.Answer);
                info.Tip.Add(choic.Analyze);
            }

            foreach (var term in TermNum)
            {
                info.QuestionID.Add(term.QuestionID);
                info.Problem.Add(term.QuestionContent);
                info.ScoreQuestion.Add(term.Score.Value);
                info.TimeQuestion.Add(term.Duration.Value);
                info.DifficultQuestion.Add(term.Difficult.Value);

                info.AnswerValue.Add(term.Answer);
                info.Tip.Add(term.Analyze);
            }

            foreach (var list in question)
            {

                string name = string.Empty;
                string id = string.Empty;

                var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                foreach (var Knowledge in KnowledgeList)
                {
                    var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                    id += KnowledgeName.KnowledgePointID + ",";
                    name += KnowledgeName.Title + ",";
                }
                name = name.Substring(0, name.LastIndexOf(","));
                id = id.Substring(0, id.LastIndexOf(','));
                info.KnowledgeID.Add(id);
                info.Knowledge.Add(name);
            }
            Sspc.Info = info;
            return Sspc;
        }
        /// <summary>
        /// 查询当前点击的完型填空信息
        /// </summary>
        /// <param name="item">AssessmentItemID</param>
        /// <returns>当前点击的完型填空信息</returns>
        ClozePart ICedts_ItemRepository.SelectCloze(Guid item)
        {
            var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
            var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).First();
            var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).First();
            var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).First();

            ItemBassInfo info = new ItemBassInfo();
            ClozePart Cloze = new ClozePart();
            info.AnswerValue = new List<string>();
            info.Tip = new List<string>();
            info.QuestionID = new List<Guid>();
            info.DifficultQuestion = new List<double>();
            info.ScoreQuestion = new List<double>();

            info.Knowledge = new List<string>();
            info.KnowledgeID = new List<string>();
            Cloze.Choices = new List<string>();
            info.TimeQuestion = new List<double>();
            info.ItemID = item;
            info.Count = Convert.ToInt32(assess.Count);
            info.ItemType = Itemtype.TypeName;
            info.ItemType_CN = Itemtype.ItemTypeID.ToString();
            info.PartType = PartType.TypeName;
            info.QuestionCount = Convert.ToInt32(assess.QuestionCount);
            info.ReplyTime = Convert.ToInt32(assess.Duration);
            info.Diffcult = Convert.ToDouble(assess.Difficult);
            info.Score = Convert.ToInt32(assess.Score);

            Cloze.Content = assess.Original;

            string QkID = null;
            foreach (var qu in question)
            {
                var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                foreach (var q in QkList)
                {
                    QkID += q + ",";
                }
            }
            QkID = QkID.Substring(0, QkID.LastIndexOf(","));
            info.QuestionKnowledgeID = QkID;

            foreach (var list in question)
            {
                info.QuestionID.Add(list.QuestionID);
                info.ScoreQuestion.Add(list.Score.Value);
                info.TimeQuestion.Add(list.Duration.Value);
                info.DifficultQuestion.Add(list.Difficult.Value);
                Cloze.Choices.Add(list.ChooseA);
                Cloze.Choices.Add(list.ChooseB);
                Cloze.Choices.Add(list.ChooseC);
                Cloze.Choices.Add(list.ChooseD);
                info.AnswerValue.Add(list.Answer);
                info.Tip.Add(list.Analyze);
                string name = string.Empty;
                string id = string.Empty;


                var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                foreach (var Knowledge in KnowledgeList)
                {
                    var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).FirstOrDefault();
                    id += KnowledgeName.KnowledgePointID + ",";
                    name += KnowledgeName.Title + ",";
                }
                name = name.Substring(0, name.LastIndexOf(","));
                id = id.Substring(0, id.LastIndexOf(','));
                info.KnowledgeID.Add(id);
                info.Knowledge.Add(name);
            }
            Cloze.Info = info;
            return Cloze;
        }

        /// <summary>
        /// 查询当前点的阅读理解-选择题型信息
        /// </summary>
        /// <param name="item">AssessmentItemID</param>
        /// <returns>当前点击的阅读理解-选择题型信息</returns>
        ReadingPartOption ICedts_ItemRepository.SelectRpo(Guid item)
        {
            var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
            var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).First();
            var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).First();
            var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).First();



            ItemBassInfo info = new ItemBassInfo();
            info.AnswerValue = new List<string>();
            info.Problem = new List<string>();
            info.Tip = new List<string>();
            info.QuestionID = new List<Guid>();
            info.DifficultQuestion = new List<double>();
            info.ScoreQuestion = new List<double>();
            ReadingPartOption Rpo = new ReadingPartOption();
            info.Knowledge = new List<string>();
            info.KnowledgeID = new List<string>();
            Rpo.Choices = new List<string>();
            info.TimeQuestion = new List<double>();
            info.ItemID = item;
            info.Count = Convert.ToInt32(assess.Count);
            info.ItemType = Itemtype.TypeName;
            info.ItemType_CN = Itemtype.ItemTypeID.ToString();
            info.PartType = PartType.TypeName;
            info.QuestionCount = Convert.ToInt32(assess.QuestionCount);
            info.ReplyTime = Convert.ToInt32(assess.Duration);
            info.Diffcult = Convert.ToDouble(assess.Difficult);
            info.Score = Convert.ToInt32(assess.Score);

            Rpo.Content = assess.Original;

            string QkID = null;
            foreach (var qu in question)
            {
                var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                foreach (var q in QkList)
                {
                    QkID += q + ",";
                }
            }
            QkID = QkID.Substring(0, QkID.LastIndexOf(","));
            info.QuestionKnowledgeID = QkID;

            foreach (var list in question)
            {
                info.QuestionID.Add(list.QuestionID);
                info.Problem.Add(list.QuestionContent);
                info.ScoreQuestion.Add(list.Score.Value);
                info.TimeQuestion.Add(list.Duration.Value);
                info.DifficultQuestion.Add(list.Difficult.Value);
                Rpo.Choices.Add(list.ChooseA);
                Rpo.Choices.Add(list.ChooseB);
                Rpo.Choices.Add(list.ChooseC);
                Rpo.Choices.Add(list.ChooseD);
                info.AnswerValue.Add(list.Answer);
                info.Tip.Add(list.Analyze);
                string name = string.Empty;
                string id = string.Empty;


                var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                foreach (var Knowledge in KnowledgeList)
                {
                    var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                    id += KnowledgeName.KnowledgePointID + ",";
                    name += KnowledgeName.Title + ",";
                }
                name = name.Substring(0, name.LastIndexOf(","));
                id = id.Substring(0, id.LastIndexOf(','));
                info.KnowledgeID.Add(id);
                info.Knowledge.Add(name);
            }
            Rpo.Info = info;
            return Rpo;
        }

        /// <summary>
        /// 查询当前点击的阅读理解-选词填空信息
        /// </summary>
        /// <param name="item">AssessmentItemID</param>
        /// <returns>当前点击的阅读理解-选词填空信息</returns>
        ReadingPartCompletion ICedts_ItemRepository.SlelectRpc(Guid item)
        {
            var question = (from m in db.CEDTS_Question where m.AssessmentItemID == item orderby m.Order ascending select m).ToList();
            var assess = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item select m).First();
            var Itemtype = (from m in db.CEDTS_ItemType where m.ItemTypeID == assess.ItemTypeID select m).First();
            var PartType = (from m in db.CEDTS_PartType where m.PartTypeID == Itemtype.PartTypeID select m).First();
            var Expansion = (from m in db.CEDTS_Expansion where m.AssessmentItemID == item select m).First();


            ItemBassInfo info = new ItemBassInfo();
            info.AnswerValue = new List<string>();
            info.Problem = new List<string>();
            info.Tip = new List<string>();
            info.QuestionID = new List<Guid>();
            info.DifficultQuestion = new List<double>();
            info.ScoreQuestion = new List<double>();
            ReadingPartCompletion Rpc = new ReadingPartCompletion();
            info.Knowledge = new List<string>();
            info.KnowledgeID = new List<string>();
            info.TimeQuestion = new List<double>();
            Rpc.WordList = new List<string>();
            info.ItemID = item;
            info.Count = Convert.ToInt32(assess.Count);
            info.ItemType = Itemtype.TypeName;
            info.ItemType_CN = Itemtype.ItemTypeID.ToString();
            info.PartType = PartType.TypeName;
            info.QuestionCount = Convert.ToInt32(assess.QuestionCount);
            info.ReplyTime = Convert.ToInt32(assess.Duration);
            info.Diffcult = Convert.ToDouble(assess.Difficult);
            info.Score = Convert.ToInt32(assess.Score);

            Rpc.Content = assess.Original;

            Rpc.ExpansionID = Expansion.ExpansionID;
            Rpc.WordList.Add(Expansion.ChoiceA);
            Rpc.WordList.Add(Expansion.ChoiceB);
            Rpc.WordList.Add(Expansion.ChoiceC);
            Rpc.WordList.Add(Expansion.ChoiceD);
            Rpc.WordList.Add(Expansion.ChoiceE);
            Rpc.WordList.Add(Expansion.ChoiceF);
            Rpc.WordList.Add(Expansion.ChoiceG);
            Rpc.WordList.Add(Expansion.ChoiceH);
            Rpc.WordList.Add(Expansion.ChoiceI);
            Rpc.WordList.Add(Expansion.ChoiceJ);
            Rpc.WordList.Add(Expansion.ChoiceK);
            Rpc.WordList.Add(Expansion.ChoiceL);
            Rpc.WordList.Add(Expansion.ChoiceM);
            Rpc.WordList.Add(Expansion.ChoiceN);
            Rpc.WordList.Add(Expansion.ChoiceO);


            string QkID = null;
            foreach (var qu in question)
            {
                var QkList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == qu.QuestionID orderby m.Weight ascending select m.QuestionKnowledgeID).ToList();
                foreach (var q in QkList)
                {
                    QkID += q + ",";
                }
            }
            QkID = QkID.Substring(0, QkID.LastIndexOf(","));
            info.QuestionKnowledgeID = QkID;

            foreach (var list in question)
            {
                info.QuestionID.Add(list.QuestionID);
                info.ScoreQuestion.Add(list.Score.Value);
                info.TimeQuestion.Add(list.Duration.Value);
                info.DifficultQuestion.Add(list.Difficult.Value);
                info.AnswerValue.Add(list.Answer);
                info.Tip.Add(list.Analyze);
                string name = string.Empty;
                string id = string.Empty;


                var KnowledgeList = (from m in db.CEDTS_QuestionKnowledge where m.QuestionID == list.QuestionID orderby m.Weight ascending select m.KnowledgePointID).ToList();
                foreach (var Knowledge in KnowledgeList)
                {
                    var KnowledgeName = (from m in db.CEDTS_KnowledgePoints where m.KnowledgePointID == Knowledge select m).First();
                    id += KnowledgeName.KnowledgePointID + ",";
                    name += KnowledgeName.Title + ",";
                }
                name = name.Substring(0, name.LastIndexOf(","));
                id = id.Substring(0, id.LastIndexOf(','));
                info.KnowledgeID.Add(id);
                info.Knowledge.Add(name);
            }
            Rpc.Info = info;
            return Rpc;
        }

        /// <summary>
        /// 获取ItemID
        /// </summary>
        /// <param name="id">AssessmentItemID</param>
        /// <returns>ItemID</returns>
        int? ICedts_ItemRepository.GetEditItemID(Guid id)
        {
            var ItemID = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == id select m.ItemTypeID).First();
            return ItemID;
        }

        /// <summary>
        /// 更新当前点击AssessmentItem信息
        /// </summary>
        /// <param name="item">当前点击AssessmentItem信息</param>
        void ICedts_ItemRepository.UpdateItem(CEDTS_AssessmentItem item)
        {
            var originalitem = (from m in db.CEDTS_AssessmentItem where m.AssessmentItemID == item.AssessmentItemID select m).First();
            item.SaveTime = originalitem.SaveTime;
            item.UserID = originalitem.UserID;
            item.SoundFile = originalitem.SoundFile;
            item.Unit = originalitem.Unit;
            item.Course = originalitem.Course;
            db.ApplyCurrentValues(originalitem.EntityKey.EntitySetName, item);

            db.SaveChanges();
        }

        /// <summary>
        /// 新增后返回当前选中的Part
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<SelectListItem> ICedts_ItemRepository.GetPart(int id)
        {
            List<SelectListItem> type = new List<SelectListItem>();
            var type1 = (from m in db.CEDTS_PartType select m).ToList();
            foreach (var typename1 in type1)
            {
                if (typename1.PartTypeID == id)
                {
                    type.Add(new SelectListItem { Text = typename1.TypeName_CN, Value = typename1.PartTypeID.ToString(), Selected = true });
                }
                else
                {
                    type.Add(new SelectListItem { Text = typename1.TypeName_CN, Value = typename1.PartTypeID.ToString() });
                }
            }
            return type;
        }

        /// <summary>
        /// 新增后返回当前选中的Item
        /// </summary>
        /// <param name="partid">PartID</param>
        /// <param name="id">ItemID</param>
        /// <returns></returns>
        List<SelectListItem> ICedts_ItemRepository.GetItem(int partid, int id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var items1 = (from m in db.CEDTS_ItemType where m.PartTypeID == partid select m).ToList();
            foreach (var itemname1 in items1)
            {
                if (itemname1.ItemTypeID == id)
                {
                    items.Add(new SelectListItem { Text = itemname1.TypeName_CN, Value = itemname1.ItemTypeID.ToString(), Selected = true });
                }
                else
                {
                    items.Add(new SelectListItem { Text = itemname1.TypeName_CN, Value = itemname1.ItemTypeID.ToString() });
                }
            }
            return items;
        }

        List<SelectListItem> ICedts_ItemRepository.GetItems(int? id)
        {
            var plan = (from m in db.CEDTS_ItemType where m.PartTypeID == id select m).ToList();
            if (plan.Count == 0)
            {
                List<SelectListItem> item = new List<SelectListItem>();
                item.Insert(0, new SelectListItem { Text = "暂无元素-请添加元素", Value = "" });
                return item;
            }
            else
            {
                List<SelectListItem> itemtype = new List<SelectListItem>();
                foreach (var s in plan)
                {
                    itemtype.Add(new SelectListItem { Text = s.TypeName_CN.ToString(), Value = s.ItemTypeID.ToString() });
                }
                return itemtype;
            }
        }

        /// <summary>
        /// 首次加载是获取Item
        /// </summary>
        /// <param name="id">PartID</param>
        /// <returns></returns>
        List<CEDTS_ItemType> ICedts_ItemRepository.FirstItem(int? id)
        {
            var planList = db.CEDTS_ItemType.Where(p => p.PartTypeID == id).ToList();
            return planList;
        }

        /// <summary>
        /// 新增时获取知识点
        /// </summary>
        /// <param name="PartType">PartTypeID</param>
        /// <returns>string（Content）</returns>
        string ICedts_ItemRepository.GetPoint()
        {
            var Points = db.CEDTS_KnowledgePoints.Where(p => p.Level == 2).OrderBy(p => p.Title).ToList();
            StringBuilder builder = new StringBuilder();
            builder.Append("{\"datalist\":[");
            for (int i = 0; i < Points.Count; i++)
            {
                KnowPoint k = new KnowPoint();
                k.KnowID = Points[i].KnowledgePointID;
                Guid UperKnowledgeID = Guid.Parse(Points[i].UperKnowledgeID.ToString());
                string UpKownledgeName = db.CEDTS_KnowledgePoints.Where(p => p.KnowledgePointID == UperKnowledgeID).FirstOrDefault().Title;
                k.PointName = Points[i].Title;
                if (i == Points.Count - 1)
                {
                    builder.Append("{\"data\":" + "\"" + k.KnowID + "\"" + ",\"text\":" + "\"" + k.PointName + "(" + UpKownledgeName + ")" + "\"" + "}");
                }
                else
                {
                    builder.Append("{\"data\":" + "\"" + k.KnowID + "\"" + ",\"text\":" + "\"" + k.PointName + "(" + UpKownledgeName + ")" + "\"" + "},");
                }
            }
            builder.Append("]}");
            return builder.ToString();
        }

        IQueryable<CEDTS_PartType> ICedts_ItemRepository.GetPartType()
        {
            return db.CEDTS_PartType;
        }

        /// <summary>
        /// 获取Item英文名称
        /// </summary>
        /// <param name="id">ItemID</param>
        /// <returns>Item英文名称</returns>
        string ICedts_ItemRepository.ItemName(int id)
        {
            var name = (from m in db.CEDTS_ItemType where m.ItemTypeID == id select m.TypeName).First();
            return name;
        }

        /// <summary>
        /// 获取Item中文名称
        /// </summary>
        /// <param name="id">ItemID</param>
        /// <returns>Item中文名称</returns>
        string ICedts_ItemRepository.ItemName_CN(int id)
        {
            var name_cn = (from m in db.CEDTS_ItemType where m.ItemTypeID == id select m.TypeName_CN).First();
            return name_cn;
        }

        /// <summary>
        /// 获取Part英文名称
        /// </summary>
        /// <param name="id">PartID</param>
        /// <returns>Part英文名称</returns>
        string ICedts_ItemRepository.PartName(int id)
        {
            var name = (from m in db.CEDTS_PartType where m.PartTypeID == id select m.TypeName).First();
            return name;
        }

        /// <summary>
        /// 根据ItemName获取ItemID
        /// </summary>
        /// <param name="name">ItemName</param>
        /// <returns>ItemID</returns>
        int ICedts_ItemRepository.GetItemID(string name)
        {
            var ItemID = (from m in db.CEDTS_ItemType where m.TypeName == name select m.ItemTypeID).First();
            return ItemID;
        }

        int ICedts_ItemRepository.SelectUserID(string UserAccount)
        {
            var UserID = (from m in db.CEDTS_User where m.UserAccount == UserAccount select m.UserID).FirstOrDefault();
            return UserID;
        }
        #endregion


    }
}