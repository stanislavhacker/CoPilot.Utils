using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CoPilot.Utils.Exception
{
    /// <summary>
    /// Info
    /// </summary>
    public class ExceptionInfo 
    {
        public String Message { get; set; }
        public String StackTrace { get; set; }
    }

    /// <summary>
    /// Collector
    /// </summary>
    public class ExceptionCollector
    {
        /// <summary>
        /// Collect data
        /// </summary>
        /// <param name="messages"></param>
        public static void Collect(List<ExceptionInfo> messages) {
            //save message
            XDocument x = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
            //root
            XElement root = new XElement(XName.Get("copilot", "error"));
            x.Add(root);

            //error list
            XElement list = new XElement(XName.Get("list", "errors"));
            foreach(var msg in messages) 
            {
                XElement error = new XElement(XName.Get("error", "errors"));
                //message
                XElement errorMessage = new XElement(XName.Get("message", "errors"));
                errorMessage.Add(new XText(msg.Message));
                //stack trace
                XElement stacktraceMessage = new XElement(XName.Get("stack-trace", "errors"));
                stacktraceMessage.Add(new XCData(msg.StackTrace));

                //add
                error.Add(errorMessage);
                error.Add(stacktraceMessage);
                //add into list
                list.Add(error);
            }
            root.Add(list);

            //verion
            XElement version = new XElement(XName.Get("version", "assembly"));
            version.Add(new XText(Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            root.Add(version);

            //date
            XElement date = new XElement(XName.Get("date", "assembly"));
            date.Add(new XText(DateTime.Now.ToString()));
            root.Add(date);

            //language
            XElement language = new XElement(XName.Get("language", "assembly"));
            language.Add(new XText(CultureInfo.CurrentCulture.EnglishName));
            language.Add(new XAttribute(XName.Get("iso"), CultureInfo.CurrentCulture.TwoLetterISOLanguageName));
            root.Add(language);

            var message = x.ToString();
            //save to isolated storage
            Settings.Add("error", message);
        }
    }
}
