using System;

namespace MonkeyPad2.Notes
{
    public class NoteUtils
    {
        public static void FormatDisplayInfo(Note note)
        {
            string noteTitle = "";
            string noteDescription = "";

            if (note.Content.Trim().Length < 40)
            {
                noteTitle = note.Content.Trim();
            }
            else if (note.Content.Trim().Length > 40 && note.Content.Trim().Length < 120)
            {
                noteTitle = note.Content.Trim().Substring(0, 37) + "...";
                noteDescription = note.Content.Trim().Substring(37);
            }
            else
            {
                noteTitle = note.Content.Trim().Substring(0, 37) + "...";
                noteDescription = note.Content.Trim().Substring(37, 120 - 37) + "...";
            }
            note.DisplayContent = noteDescription;
            note.DisplayTitle = noteTitle;
            note.DisplayDate = MonkeyPadDateFormatShort(FromUnixEpochTime(note.ModifyDate));
        }

        public static DateTime FromUnixEpochTime(decimal unixTime)
        {
            var d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            d = d.AddSeconds(Convert.ToDouble(unixTime));
            return d.ToLocalTime();
        }

        public static string MonkeyPadDateFormatShort(DateTime dateTime)
        {
            return dateTime.ToString("MMM dd");
        }

        public static string MonkeyPadDateFormatLong(DateTime dateTime)
        {
            return dateTime.ToString("dddd MMMM d, yyyy @ h:mm tt");
        }
    }
}