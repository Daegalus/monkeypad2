using System;
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
        private static bool _done;
        private static string _email = "";
        private static string _password = "";
        private static Notes.Note _note = null;
        private static Tag _tag = null;

        public static HttpWebRequest CreateLoginRequest(string content, string email, string password)
        {
            var request = (HttpWebRequest) WebRequest.Create("https://simple-note.appspot.com/api/login");
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            _email = email;
            _password = password;
            request.BeginGetRequestStream(LoginReadCallback, request);
            while (!_done) ;
            _done = false;
            return request;
        }

        public static HttpWebRequest CreateNoteRequest(string method, Uri uri, Note note, string email, string authToken)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("https://simple-note.appspot.com/api2/data/");
            if (method == "POST" && note.Key != null)
                stringBuilder.Append(note.Key);
            stringBuilder.Append("?auth=");
            stringBuilder.Append(authToken);
            stringBuilder.Append("&email=");
            stringBuilder.Append(email);
            var request = (HttpWebRequest) WebRequest.Create(stringBuilder.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = method;
            _note = note;
            switch(method)
            {
                case "GET":
                    return request;
                case "POST":
                    request.BeginGetRequestStream(NoteReadCallback, request);
                    while(!_done);
                    _done = false;
                    return request;
                default:
                    return null;
            }
        }

        public static HttpWebRequest CreateTagRequest(string method, Uri uri, Tag tag, string email, string authToken)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("https://simple-note.appspot.com/api2/data/");
            if (method == "POST" && tag.Version > 0)
                stringBuilder.Append(tag.Name);
            stringBuilder.Append("?auth=");
            stringBuilder.Append(authToken);
            stringBuilder.Append("&email=");
            stringBuilder.Append(email);
            var request = (HttpWebRequest)WebRequest.Create(stringBuilder.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = method;
            _tag = tag;
            switch (method)
            {
                case "GET":
                    return request;
                case "POST":
                    request.BeginGetRequestStream(TagReadCallback, request);
                    while (!_done) ;
                    _done = false;
                    return request;
                default:
                    return null;
            }
        }

        private static void LoginReadCallback(IAsyncResult result)
        {
            var request = (HttpWebRequest) result.AsyncState;
            Stream postStream = request.EndGetRequestStream(result);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("email=");
            stringBuilder.Append(_email);
            stringBuilder.Append("&password");
            stringBuilder.Append(_password);
            string postData = stringBuilder.ToString();
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            postStream.Write(byteArray, 0, postData.Length);
            postStream.Close();
            _done = true;
            _password = "";
        }

        private static void NoteReadCallback(IAsyncResult result)
        {
            var request = (HttpWebRequest)result.AsyncState;
            Stream postStream = request.EndGetRequestStream(result);
            var data = JsonProcessor.ToJson(_note);
            string postData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            postStream.Write(byteArray, 0, postData.Length);
            postStream.Close();
            _done = true;
        }

        private static void TagReadCallback(IAsyncResult result)
        {
            var request = (HttpWebRequest)result.AsyncState;
            Stream postStream = request.EndGetRequestStream(result);
            var data = JsonProcessor.ToJson(_tag);
            string postData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            postStream.Write(byteArray, 0, postData.Length);
            postStream.Close();
            _done = true;
        }
    }
}