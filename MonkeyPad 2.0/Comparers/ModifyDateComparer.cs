using System.Collections.Generic;
using MonkeyPad2.Notes;

namespace MonkeyPad2.Comparers
{
    public class ModifyDateComparer : IComparer<Note>
    {
        #region IComparer<Note> Members

        public int Compare(Note x, Note y)
        {
            return decimal.Compare(y.ModifyDate, x.ModifyDate); // sort by modifydate
        }

        #endregion
    }
}