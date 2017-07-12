using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webdiyer.WebControls.Mvc;

namespace Cedts_Test.Areas.Admin.Models
{
    public class Cedts_CardRepository : ICedts_CardRepository
    {
        #region 实例化Cedts_Entities
        Cedts_Entities db;
        public Cedts_CardRepository()
        {
            db = new Cedts_Entities();
        }
        #endregion

        PagedList<CEDTS_Card> ICedts_CardRepository.SelectAllCard(int? id, int CKind, int CType, int AState, int SCondition, string TCondition)
        {
            if (CKind == 4 && CType == 4 && AState == 4 && TCondition == "")
                return db.CEDTS_Card.OrderByDescending(p => p.CreateTime).ToPagedList(id ?? 1, 10);
            else
            {
                var CList = db.CEDTS_Card.ToList();
                var RList = new List<CEDTS_Card>();
                if (CKind != 4)
                {
                    RList = CList.Where(p => p.CardKind == CKind).ToList();
                    CList.Clear();
                    CList.AddRange(RList);
                }
                if (CType != 4)
                {
                    RList.Clear();
                    RList = CList.Where(p => p.CardType == CType).ToList();
                    CList.Clear();
                    CList.AddRange(RList);
                }
                if (AState != 4)
                {
                    RList.Clear();
                    RList = CList.Where(p => p.ActivationState == AState).ToList();
                    CList.Clear();
                    CList.AddRange(RList);
                }
                if (TCondition != "")
                {
                    switch (SCondition)
                    {
                        case 1:
                            RList.Clear();
                            var CreateUserID = db.CEDTS_User.Where(p => p.UserAccount == TCondition).Select(p => p.UserID).FirstOrDefault();
                            RList = CList.Where(p => p.CreateUser == CreateUserID).ToList();
                            CList.Clear();
                            CList.AddRange(RList);
                            break;
                        case 2:
                            RList.Clear();
                            var ActivationUserID = db.CEDTS_User.Where(p => p.UserAccount == TCondition).Select(p => p.UserID).FirstOrDefault();
                            RList = CList.Where(p => p.ActivationUser == ActivationUserID).ToList();
                            CList.Clear();
                            CList.AddRange(RList);
                            break;
                        case 3:
                            RList.Clear();
                            double Discount = 0.0;
                            double.TryParse(TCondition, out Discount);
                            RList = CList.Where(p => p.Discount == Discount).ToList();
                            CList.Clear();
                            CList.AddRange(RList);
                            break;
                        case 4:
                            RList.Clear();
                            var PartnerID = db.CEDTS_Partner.Where(p => p.PartnerName == TCondition).Select(p => p.PartnerID).FirstOrDefault();
                            RList = CList.Where(p => p.PartnerID == PartnerID).ToList();
                            CList.Clear();
                            CList.AddRange(RList);
                            break;
                        case 5:
                            RList.Clear();
                            double Money = 0.0;
                            double.TryParse(TCondition, out Money);
                            RList = CList.Where(p => p.Money == Money).ToList();
                            CList.Clear();
                            CList.AddRange(RList);
                            break;
                        default: break;
                    }
                }
                return CList.OrderByDescending(p => p.CreateTime).AsQueryable().ToPagedList(id ?? 1, 10);
            }
        }

        List<CEDTS_Partner> ICedts_CardRepository.SelectPartner()
        {
            return db.CEDTS_Partner.ToList();
        }

        int ICedts_CardRepository.GetUserIDbyAccount(string Account)
        {
            return db.CEDTS_User.Where(p => p.UserAccount == Account).Select(p => p.UserID).FirstOrDefault();
        }

        void ICedts_CardRepository.CreateCard(CEDTS_Card card)
        {
            db.AddToCEDTS_Card(card);
            db.SaveChanges();
        }

        string ICedts_CardRepository.DeleteCard(int id)
        {
            var card = db.CEDTS_Card.Where(p => p.ID == id).FirstOrDefault();
            db.DeleteObject(card);
            db.SaveChanges();
            return "1";
        }

        PagedList<CEDTS_Card> ICedts_CardRepository.GetCardByUserID(int UserID, int? id)
        {
            var cardList = db.CEDTS_UserCard.Where(p => p.UserID == UserID).Select(p => p.CardID).ToList();
            return db.CEDTS_Card.Where(p => cardList.Contains(p.ID)).OrderBy(p => p.CreateTime).ToPagedList(id ?? 1, 10);
        }

        CEDTS_Card ICedts_CardRepository.SelectCardByID(int ID)
        {
            return db.CEDTS_Card.Where(p => p.ID == ID).FirstOrDefault();
        }

        void ICedts_CardRepository.UpdateCard(CEDTS_Card card)
        {
            int id = card.ID;
            var b_card = db.CEDTS_Card.Where(p => p.ID == id).FirstOrDefault();
            db.ApplyCurrentValues(b_card.EntityKey.EntitySetName, card);
            db.SaveChanges();
        }

        CEDTS_Card ICedts_CardRepository.CheckCard(string SerialNumber, string PassWord)
        {
            return db.CEDTS_Card.Where(p => p.SerialNumber == SerialNumber && p.PassWord == PassWord).FirstOrDefault();
        }

        void ICedts_CardRepository.CreatUserCard(int UserID, int CardID)
        {
            CEDTS_UserCard uc = new CEDTS_UserCard();
            uc.UserID = UserID;
            uc.CardID = CardID;
            db.AddToCEDTS_UserCard(uc);
            db.SaveChanges();
        }

        #region 密码加密
        string ICedts_CardRepository.HashPassword(string str)
        {
            string rethash = string.Empty;
            System.Security.Cryptography.SHA1 hash = System.Security.Cryptography.SHA1.Create();
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            byte[] combined = encoder.GetBytes(str);
            hash.ComputeHash(combined);
            rethash = Convert.ToBase64String(hash.Hash);
            return rethash;
        }
        #endregion
    }
}