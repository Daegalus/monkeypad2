using System;
using System.Text;

namespace MonkeyPad2.Requests
{
    public class RequestUtils
    {
        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
    }
}
