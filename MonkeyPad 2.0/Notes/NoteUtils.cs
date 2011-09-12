using System;

namespace MonkeyPad2.Notes
{
    public class NoteUtils
    {
        public static void FormatDisplayInfo(Note note)
        {
            string noteTitle = "";
            string noteDescription = "";
            string workNote = note.Content.Trim();

            string[] noteLines = workNote.Split('\n');

            string concatLines = "";
            foreach(var line in noteLines)
            {
                concatLines += line + " ";
            }

            concatLines = concatLines.Substring(noteLines[0].Length, concatLines.Length - noteLines[0].Length).Trim();

            if (noteLines.Length == 0)
            {
                noteTitle = " ";
                noteDescription = " ";
            }
            else if (noteLines.Length == 1 && noteLines[0].Length < 30)
            {
                noteTitle = noteLines[0];
                noteDescription = " ";
            }
            else if (noteLines.Length == 1 && noteLines[0].Length >= 30)
            {
                noteTitle = noteLines[0].Substring(0,27).Trim() + "...";
                noteDescription = " ";
            }
            else if (noteLines.Length > 1 && noteLines[0].Length < 30 && concatLines.Length < 100)
            {
                noteTitle = noteLines[0];
                noteDescription = concatLines;
            }
            else if (noteLines.Length > 1 && noteLines[0].Length >= 30 && concatLines.Length < 100)
            {
                noteTitle = noteLines[0].Substring(0, 27).Trim() + "...";
                noteDescription = concatLines;
            }
            else if (noteLines.Length > 1 && noteLines[0].Length < 30 && concatLines.Length >= 100)
            {
                noteTitle = noteLines[0];
                noteDescription = concatLines.Substring(0,97).Trim() + "...";
            }
            else
            {
                noteTitle = noteLines[0].Substring(0, 27) + "...";
                noteDescription = concatLines.Substring(0, 97).Trim() + "...";
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