using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MonkeyPad2.Notes
{
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        public void Sort()
        {
            this.Sort(0, Count, null);
        }
        public void Sort(IComparer<T> comparer)
        {
            this.Sort(0, Count, comparer);
        }
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            ((List<T>)Items).Sort(index, count, comparer);
        }
    }
}
