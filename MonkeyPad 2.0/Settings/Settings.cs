namespace MonkeyPad2.Settings
{
    public class Settings
    {
        #region sortMethods enum

        public enum sortMethods
        {
            ModifyDate,
            CreateDate,
            Alphabetical
        }

        #endregion

        public string Email { get; set; }
        public string AuthToken { get; set; }
        public int Theme { get; set; }
        public int Fontsize { get; set; }
        public int NumberOfNotes { get; set; }
        public bool RefreshOnLogin { get; set; }
        public int RegreshSinceLastTime { get; set; }
        public bool ParseMarkdown { get; set; }
        public bool ConvertLists { get; set; }
        public int SortMethod { get; set; }
    }
}