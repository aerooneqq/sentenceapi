using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLibrary.ActionResults.ResponseBuilder
{
    internal class ContentTypes
    {
        public static string ApplicationJson => "application/json";
        public static string ApplicationXml => "application/xml";

        public static string TextPlain => "text/plain";
        public static string TextHtml => "text/html";
        public static string TextCsv => "text/csv";
    }
}
