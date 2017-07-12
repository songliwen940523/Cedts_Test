using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cedts_Test.Models
{
    public class Front
    {

        public string SayContent { get; set; }

        public string SayNote { get; set; }
        //系统概况
        public string SystemOverview { get; set; }
        //特色功能
        public List<string> CoreFeatures { get; set; }
    }
    public class ContactUsInfo
    {
        //名称
        public string Name { get; set; }
        //联系电话
        public string Tel { get; set; }
        //邮编
        public string ZipCode { get; set; }
        //联系地址
        public string Address { get; set; }
        //官网地址
        public string Url { get; set; }
    }

    public class Excel1
    {
        public int id { get; set; }

        public Isright ii { get; set; }
    }
    public class Isright
    {
       public List<bool> rigth { get; set; }
    }
}