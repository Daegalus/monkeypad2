namespace MonkeyPad2.Tags
{
    public class TagUtils
    {
        public static string tagsToString(string[] tags)
        {
            var formattedTags = "";
            var counter = 0;
            foreach (string tag in tags)
            {
                if (counter < tags.Length - 1)
                    formattedTags += tag + ", ";
                else
                    formattedTags += tag;
                counter++;
            }
            return formattedTags;
        }
    }
}