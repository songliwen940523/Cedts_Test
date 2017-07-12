using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Cedts_Test
{
    /// <summary>
    /// Upload 的摘要说明
    /// </summary>
    public class Upload : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            HttpPostedFile file = context.Request.Files["file"];
            if (context.Request["type"] == "1")
            {
                //项目根目录的物理路径
                string serverPath = AppDomain.CurrentDomain.BaseDirectory + "SoundFile\\";
                //文件名称
                string sound = Guid.NewGuid().ToString();
                var filename = sound + ".mp3";
                //文件及物理路径
                var filepath = serverPath + filename;
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }
                file.SaveAs(filepath);
                context.Response.Write(sound);
                context.Response.End();
            }
            if (context.Request["type"] == "2")
            {
                //项目根目录的物理路径
                string serverPath = AppDomain.CurrentDomain.BaseDirectory + "TempSoundFile\\";
                //文件名称
                string sound = Guid.NewGuid().ToString();
                var filename = sound + ".mp3";
                //文件及物理路径
                var filepath = serverPath + filename;
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }
                file.SaveAs(filepath);
                context.Response.Write(sound);
                context.Response.End();
            }
            if (context.Request["type"] == "3")
            {
                //项目根目录的物理路径
                string serverPath = AppDomain.CurrentDomain.BaseDirectory + "TempExcel\\";
                //文件名称
                string filename = file.FileName;
                //文件及物理路径
                string filepath = serverPath + filename;
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }
                file.SaveAs(filepath);
                context.Response.Write(filepath);
                context.Response.End();
            }
        }



        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}