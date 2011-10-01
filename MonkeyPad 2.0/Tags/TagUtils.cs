using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MonkeyPad2.Tags
{
    public class TagUtils
    {
        public static string tagsToString(string[] tags)
        {
            string formattedTags = "";
            int counter = 0;
            foreach (string tag in tags)
            {
                if (counter < tags.Length)
                    formattedTags += tag + ", ";
                else
                    formattedTags += tag;

            }
            return formattedTags;
        }
    }
}
