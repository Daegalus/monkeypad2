using MonkeyPad2.Notes;

namespace MonkeyPad2.Processors
{
    public class NoteProcessor
    {
        public static void ProcessIndex(Index index, bool isPartial)
        {
            if (isPartial)
            {
                //TODO: Add partial parsing.
            }
            else
            {
                App.ViewModel.NoteIndex = index;
            }
        }

        public static Note ProcessNote(Note note)
        {
            Note savedNote = null;
            foreach (Note noteItem in App.ViewModel.NoteIndex.Data)
            {
                if (noteItem.Key.Equals(note.Key))
                {
                    noteItem.Content = note.Content;
                    noteItem.CreateDate = note.CreateDate;
                    noteItem.Deleted = note.Deleted;
                    noteItem.MinVersion = note.MinVersion;
                    noteItem.ModifyDate = note.ModifyDate;
                    noteItem.PublishKey = note.PublishKey;
                    noteItem.ShareKey = note.ShareKey;
                    noteItem.SyncNum = note.SyncNum;
                    noteItem.SystemTags = note.SystemTags;
                    noteItem.Tags = note.Tags;
                    noteItem.Version = note.Version;
                    savedNote = noteItem;
                    break;
                }
            }

            if (savedNote != null)
                NoteUtils.FormatDisplayInfo(savedNote);
            return savedNote;
        }
    }
}