using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Model.Base;
using UI.Handler;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Net;
using System.Xml;
using NPOI;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using System.Collections;

namespace UI.Controllers
{
    [ErrorLog]
    public class ShiController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        #region 首页
        public ActionResult Default()
        {
            return View();
        }
        #endregion

        #region 获取数据
        public JsonResult GetShilist(ShiaqiangParam param)
        {
            var result = new ShiaqiangLogic().GetShiList(param);
            return Json(result);
        }
        #endregion

        #region 新增数据
        public JsonResult AddShi(ShiaqiangParam param)
        {
            var result = new ShiaqiangLogic().AddShi(param);
            return Json(result);
        }
        #endregion

        #region 删除数据
        public JsonResult DelShi(ShiaqiangParam param)
        {
            var result = new ShiaqiangLogic().DelShi(param);
            return Json(result);
        }
        #endregion

        #region 获取实体 根据id
        public JsonResult GetModelShi(string Id)
        {
            var result = new ShiaqiangLogic().GetModelShi(Id);
            return Json(result);
        }
        #endregion

        #region 修改
        public JsonResult UpShi(ShiaqiangParam param)
        {
            var result = new ShiaqiangLogic().UpShi(param);
            return Json(result);
        }
        #endregion

        #region 上传文件单个，多个
        public ActionResult UploadFile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadFile(string str)
        {
            HttpFileCollectionBase files = Request.Files;
            bool isOK = false;
            if (files == null)
            {
                return Content("没有文件！", "text/plain");
            }
            if (!Directory.Exists(Request.MapPath("~/Upload")))
            {
                Directory.CreateDirectory(Request.MapPath("~/Upload"));
            }
            StringBuilder strImg = new StringBuilder();
            StringBuilder strImgUrl=new StringBuilder();
            foreach (string s in files)
            {
                HttpPostedFileBase file = files[s];
                strImgUrl.Clear();
                strImgUrl.Append(Guid.NewGuid() + Path.GetFileName(file.FileName));
                var fileName = Path.Combine(Request.MapPath("~/Upload"), strImgUrl.ToString());
                try
                {
                    file.SaveAs(fileName);
                    isOK = true;
                    strImg.Append("<img src=\"/Upload/" + strImgUrl + "\" />");
                }
                catch
                {
                    isOK = false;
                }
            }
            return Content(isOK ? strImg.ToString() : "<span style=\"color:red\">上传失败！</span>", "text/html");
        }
        #endregion

        #region 上传多个文件
        public ActionResult UploadFiles() 
        {
            return View();
        }
        #endregion

        #region 上传简历读取到内存
        public ActionResult UploadJianliRead()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadJianliRead(string str)
        {
            HttpFileCollectionBase files = Request.Files;
            bool isOK = false;
            if (files == null)
            {
                return Content("没有文件！", "text/plain");
            }
            if (!Directory.Exists(Request.MapPath("~/Upload")))
            {
                Directory.CreateDirectory(Request.MapPath("~/Upload"));
            }
            string fileText = string.Empty;
            foreach (string s in files)
            {
                HttpPostedFileBase file = files[s];
                string strPostfix = file.FileName.Substring(file.FileName.LastIndexOf("."));
                //读取 word文档
                if (strPostfix.IndexOf(".doc") >= 0)
                {
                    string WordTableCellSeparator = "<br />";
                    string WordTableRowSeparator = "<br />";
                    string WordTableSeparator = "<br />";
                    StringBuilder sbFileText = new StringBuilder();
                    XWPFDocument document = null;
                    try
                    {
                        Stream stream = file.InputStream;
                        StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding("GB2312"));
                        string strReadEnd = streamReader.ReadToEnd();
                        var fileName = Path.Combine(Request.MapPath("~/Upload"), Guid.NewGuid() + Path.GetFileName(file.FileName));
                        try
                        {
                            file.SaveAs(fileName);
                            isOK = true;
                        }
                        catch
                        {
                            isOK = false;
                        }
                        using (FileStream fileWord = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                        {
                            document = new XWPFDocument(fileWord);
                        }
                        //表格段落
                        sbFileText.AppendLine("Capture Table Begin");
                        foreach (XWPFTable table in document.Tables)
                        {
                            //循环表格行
                            foreach (XWPFTableRow row in table.Rows)
                            {
                                foreach (XWPFTableCell cell in row.GetTableCells())
                                {
                                    sbFileText.Append(cell.GetText());
                                    sbFileText.Append(WordTableCellSeparator);
                                }
                                sbFileText.Append(WordTableRowSeparator);
                            }
                            sbFileText.Append(WordTableSeparator);
                        }
                        sbFileText.AppendLine("Capture Table End");
                        //正文段落
                        sbFileText.AppendLine("Capture Paragraph Begin");
                        foreach (XWPFParagraph paragraph in document.Paragraphs)
                        {
                            sbFileText.AppendLine(paragraph.ParagraphText);
                        }
                        sbFileText.AppendLine("Capture Paragraph End");

                        fileText = sbFileText.ToString();
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            return Content(fileText);
            //return Content(isOK ? "上传成功！" : "<span style='color:red'>上传失败！</span>", "text/plain");
        }
        #endregion

        #region 网易音乐网页版刷新页面音乐继续，
        public ActionResult WebMusic163()
        {
            return View();
        }
        #endregion

        #region 根据IP获取地址
        public ActionResult LoginLocation()
        {
            return View();
        }

        [HttpPost]
        public string GetLocation()
        {
            string strIP = GetIP();
            if (string.IsNullOrEmpty(strIP))
            {
                return "{\"errNum\":300202,\"errMsg\":\"IP不存在！\"}";
            }
            string url = "http://apis.baidu.com/apistore/iplookupservice/iplookup";
            string param = "ip=" + strIP;
            string result = request(url, param);
            return result;
        }

        public string GetIP()
        {
            string tempip = "";
            try
            {
                WebRequest wr = WebRequest.Create("http://www.ip138.com/ip2city.asp");
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                string all = sr.ReadToEnd(); //读取网站的数据

                int start = all.IndexOf("[") + 1;
                int end = all.IndexOf("]", start);
                tempip = all.Substring(start, end - start);
                sr.Close();
                s.Close();
            }
            catch
            {
            }
            return tempip;
        }
        #endregion

        #region 模拟请求http
        /// <summary>
        /// 发送HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="param">请求的参数</param>
        /// <returns>请求结果</returns>
        public static string request(string url, string param)
        {
            string strURL = url + '?' + param;
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "GET";
            //伪造请求地址
            request.Referer = "http://www.baidu.com";
            // 添加header
            request.Headers.Add("apikey", "5e62b542675667357a504335b4a9da01");
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream s;
            if (response.StatusCode == HttpStatusCode.OK)
            { }
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }
        #endregion

        #region C#队列实例 Queue
        private string result = "";
        public void QueueExample()
        {
            Queue q = new Queue();
            q.Enqueue(ReturnStr("1"));
            q.Enqueue(ReturnStr("2"));
            q.Enqueue(ReturnStr("3"));
            q.Enqueue(ReturnStr("4"));


        }
        public string ReturnStr(string i)
        {
            result = result + i + "---";
            return i;
        }
        #endregion

        #region 根据身份证获取身份证信息
        public ActionResult GetIdInfo() {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public string GetInfoById(string ID)
        {
            string strURL = "http://apis.baidu.com/apistore/idservice/id?id=" + ID;
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "GET";
            // 添加header
            request.Headers.Add("apikey", "5e62b542675667357a504335b4a9da01");
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }
        #endregion

        #region MySqlTest
        public ActionResult MySqlTest()
        {
            return View();
        }
        //获取数据
        public JsonResult GetHelplist(HelpParam param)
        {
            var result = new HelpLogic().GetHelpList(param);
            return Json(result);
        }

        #endregion

        #region 获取真随机数
        public ActionResult RandomOrg()
        {
            return View();
        }

        /// <param name="num">返回随机数个数</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        [HttpPost]
        public string GetRandomOrg(string num = "1", string min = "1", string max = "10")
        {
            string url = "https://www.random.org/integers/";
            string param = "num=" + num + "&min=" + min + "&max=" + max + "&col=1&base=10&format=plain&rnd=new";
            //模拟请求https
            string strRestulr = request(url, param);
            if (strRestulr.IndexOf("Error") >= 0)
            {
                return "error";
            }
            else {
                return strRestulr;
            }
        }
        #endregion
    }
}