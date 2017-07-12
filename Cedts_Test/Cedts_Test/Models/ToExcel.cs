using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// ToExcel 的摘要说明
/// </summary>
public class ToExcel
{
    public ToExcel()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    //listname 和 cols 个数　要相等 ，顺序一一对应
    public static void tableToExcel(DataTable tb, string[] listname, string[] cols)
    {
        if ((tb == null) || (tb.Rows.Count == 0))
        {
            return;
        }

        FileStream file;
        StreamWriter filewrite;
        Random r = new Random();
        string t = r.NextDouble().ToString().Remove(0, 2);

        string filename = GetRandom() + ".xls";
        string path = System.Web.HttpContext.Current.Server.MapPath("\\")  + filename;
        int i, j;
        file = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        filewrite = new StreamWriter(file, System.Text.Encoding.Unicode);
        StringBuilder strline = new StringBuilder();
        for (i = 1; i <= listname.Length; i++)
        {
            strline.Append(listname[i - 1]).Append(Convert.ToChar(9));
        }

        filewrite.WriteLine(strline.ToString());

        //表内容

        for (i = 1; i <= tb.Rows.Count; i++)
        {
            strline.Remove(0, strline.Length);//清空全部内容

            for (j = 1; j <= cols.Length; j++)
            {

                if (j == 1)
                {
                    strline.Append(tb.Rows[i - 1][cols[j - 1]]).Append(Convert.ToChar(9));
                }
                else
                {
                    strline.Append(tb.Rows[i - 1][cols[j - 1]]).Append(Convert.ToChar(9));
                }
            }
            filewrite.WriteLine(strline);
        }

        filewrite.Close();
        file.Close();

        //if (File.Exists(path))
        //{
        //    HttpContext.Current.Response.Clear();
        //    bool success = ResponseFile(HttpContext.Current.Request, HttpContext.Current.Response, filename, path, 1024000);
        //    if (!success)
        //        HttpContext.Current.Response.Write("下载文件出错！");
        //    HttpContext.Current.Response.End();
        //}
        //else
        //{
        //    HttpContext.Current.Response.Write("文件不存在！");
        //}

        //string l_strHtml = "<script language='JavaScript'>";
        //l_strHtml += " window.open('../tempdata/excel.xls','newwindow','height=800,width=1024,scrollbars=yes,resizable=yes,location=yes, status=yes,menubar=yes,toolbar=yes,titlebar=yes')";
        //l_strHtml += "</script>";
        //HttpContext.Current.Response.Write(l_strHtml);
    }

    public static bool ResponseFile(HttpRequest _Request, HttpResponse _Response, string _fileName, string _fullPath, long _speed)
    {
        try
        {
            FileStream myFile = new FileStream(_fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader br = new BinaryReader(myFile);
            try
            {
                _Response.AddHeader("Accept-Ranges", "bytes");
                _Response.Buffer = false;
                long fileLength = myFile.Length;
                long startBytes = 0;

                int pack = 10240; //10K bytes
                //int sleep = 200;   //每秒5次   即5*10K bytes每秒
                int sleep = (int)System.Math.Floor(1000 * pack * 1.0 / _speed) + 1;
                if (_Request.Headers["Range"] != null)
                {
                    _Response.StatusCode = 206;
                    string[] range = _Request.Headers["Range"].Split(new char[] { '=', '-' });
                    startBytes = Convert.ToInt64(range[1]);
                }
                _Response.AddHeader("Content-Length", (fileLength - startBytes).ToString());
                if (startBytes != 0)
                {
                    _Response.AddHeader("Content-Range", string.Format(" bytes {0}-{1}/{2}", startBytes, fileLength - 1, fileLength));
                }

                string filename = System.Web.HttpUtility.UrlEncode(System.Text.Encoding.UTF8.GetBytes(_fileName)).Replace("+", "%20");

                _Response.AddHeader("Connection", "Keep-Alive");
                _Response.ContentType = "application/octet-stream";
                _Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);  //HttpUtility.UrlEncode(_fileName));//HttpContext.Current.Server.UrlEncode(_fileName));//HttpUtility.UrlEncode(_fileName,System.Text.Encoding.Default));



                br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                int maxCount = (int)System.Math.Floor((fileLength - startBytes) * 1.0 / pack) + 1;

                for (int i = 0; i < maxCount; i++)
                {
                    if (_Response.IsClientConnected)
                    {
                        _Response.BinaryWrite(br.ReadBytes(pack));
                        System.Threading.Thread.Sleep(sleep);
                    }
                    else
                    {
                        i = maxCount;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                br.Close();
                myFile.Close();
            }
        }
        catch
        {
            return false;
        }
        return true;
    }

    #region 用当前时间生成随机文件名
    public static string GetRandom()
    {
        string random = DateTime.Now.ToFileTime().ToString();

        return random;
    }
    #endregion
}


