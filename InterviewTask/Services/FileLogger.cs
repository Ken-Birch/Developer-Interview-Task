using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace InterviewTask.Services
{
    public class FileLogger
    {
        private string sFileFormat;
        private string sLogTime;
        private string sUserName;

        public FileLogger()
        {
            sFileFormat = HostingEnvironment.MapPath(@"~/App_Data") + @"\logs\" + DateTime.Now.ToString("yyyyMMdd.HHmm.log");
            sLogTime = DateTime.Now.ToString("yyyy-MMM-dd hh:mm:ss");
            sUserName = "Guest"; //###
            //sUserName = RequestContext.HttpContext.User.Identity.GetUserName(); //###
        }

        public void ErrorLog(string Message)
        {
            string sMess = "Error: " + sLogTime + " User='" + sUserName + "' " + " == " + Message;
            LogData(sFileFormat, sMess);
        }

        public void StateLog(string Message)
        {
            string sMess = "State: " + sLogTime + " == " + Message;
            LogData(sFileFormat, sMess);
        }

        private void LogData(string PathName,string Message)
        {
            try
            {
                StreamWriter sw = new StreamWriter(PathName, true);
                sw.WriteLine(Message);
                sw.Flush();
                sw.Close();
            }
            catch (DirectoryNotFoundException ex)
            { // retry
                FileInfo fi = new FileInfo(PathName);
                Directory.CreateDirectory(fi.DirectoryName);
                LogData(PathName, Message);
            }

        }

    }
}