using System.IO;
using System.Net;
using System.Text;
using MonkeyPad2.Notes;
using MonkeyPad2.Processors;
using MonkeyPad2.Tags;

namespace MonkeyPad2.Requests
{
    public class RequestFactory
    {
        private const string UserAgent = "MonkeyPad/2.0";
        private static bool _done;
        private static string _email = "";
        private static string _password = "";
        private static Note _note;
        private static Tag _tag;

        public static HttpWebRequest CreateLoginRequest(string email, string password)
        {
            var request = (HttpWebRequest) WebRequest.Create("https://simple-note.appspot.com/api/login");
            request.ContentType = "text/plain";
            request.UserAgent = UserAgent;
            request.Method = "POST";
            _email = email;
            _password = password;
            request.BeginGetRequestStream(result =>
                                              {
                                                  var sRequest = (HttpWebRequest) result.AsyncState;
                                                  Stream postStream = sRequest.EndGetRequestStream(result);
                                                  var stringBuilder = new StringBuilder();
                                                  stringBuilder.Append("email=");
                                                  stringBuilder.Append(_email);
                                                  stringBuilder.Append("&password=");
                                                  stringBuilder.Append(_password);
                                                  string postData = RequestUtils.EncodeTo64(stringBuilder.ToString());
                                                  byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                                                  postStream.Write(byteArray, 0, postData.Length);
                                                  postStream.Close();
                                                  _done = true;
                                                  _password = "";
                                              }, request);
            while (!_done) ;
            _done = false;
            return request;
        }

        public static HttpWebRequest CreateListRequest(int limit, decimal since, string mark, string email,
                                                       string authToken)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("https://simple-note.appspot.com/api2/index?limit=");
            stringBuilder.Append(limit);
            if (since > 0)
            {
                stringBuilder.Append("&since=");
                stringBuilder.Append(since);
            }
            if (mark != null || mark.Length > 0)
            {
                stringBuilder.Append("&mark=");
                stringBuilder.Append(mark);
            }
            stringBuilder.Append("&auth=");
            stringBuilder.Append(authToken);
            stringBuilder.Append("&email=");
            stringBuilder.Append(email);
            var request = (HttpWebRequest) WebRequest.Create(stringBuilder.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = UserAgent;
            request.Method = "GET";
            return request;
        }

        public static HttpWebRequest CreateNoteRequest(string method, Note note, string email, string authToken)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("https://simple-note.appspot.com/api2/data/");
            if (note.Key != null)
                stringBuilder.Append(note.Key);
            stringBuilder.Append("?auth=");
            stringBuilder.Append(authToken);
            stringBuilder.Append("&email=");
            stringBuilder.Append(email);
            var request = (HttpWebRequest) WebRequest.Create(stringBuilder.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = UserAgent;
            request.Method = method;
            _note = note;
            switch (method)
            {
                case "GET":
                    return request;
                case "POST":
                    request.BeginGetRequestStream(result =>
                                                      {
                                                          var sRequest = (HttpWebRequest) result.AsyncState;
                                                          Stream postStream = sRequest.EndGetRequestStream(result);
                                                          string data = JsonProcessor.ToJson(_note);
                                                          string postData = data;
                                                          byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                                                          postStream.Write(byteArray, 0, postData.Length);
                                                          postStream.Close();
                                                          _done = true;
                                                      }, request);
                    while (!_done) ;
                    _done = false;
                    return request;
                case "DELETE":
                    return request;
                default:
                    return null;
            }
        }

        public static HttpWebRequest CreateTagRequest(string method, Tag tag, string email, string authToken)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("https://simple-note.appspot.com/api2/data/");
            if ((method == "POST" || method == "DELETE") && tag.Version > 0)
                stringBuilder.Append(tag.Name);
            stringBuilder.Append("?auth=");
            stringBuilder.Append(authToken);
            stringBuilder.Append("&email=");
            stringBuilder.Append(email);
            var request = (HttpWebRequest) WebRequest.Create(stringBuilder.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = UserAgent;
            request.Method = method;
            _tag = tag;
            switch (method)
            {
                case "GET":
                    return request;
                case "POST":
                    request.BeginGetRequestStream(result =>
                                                      {
                                                          var sRequest = (HttpWebRequest) result.AsyncState;
                                                          Stream postStream = sRequest.EndGetRequestStream(result);
                                                          string data = JsonProcessor.ToJson(_tag);
                                                          string postData = data;
                                                          byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                                                          postStream.Write(byteArray, 0, postData.Length);
                                                          postStream.Close();
                                                          _done = true;
                                                      }, request);
                    while (!_done) ;
                    _done = false;
                    return request;
                case "DELETE":
                    return request;
                default:
                    return null;
            }
        }
    }
}