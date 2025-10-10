using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
namespace WorldCreator.Core;
public class SortableObservableCollection<T> : ObservableCollection<T> where T : IComparable<T>
{
    private Comparison<T> _comparison;
    private bool _isSorted;

    public SortableObservableCollection() { }

    public SortableObservableCollection(IEnumerable<T> collection) : base(collection) { }

    public SortableObservableCollection(List<T> list) : base(list) { }

    public void Sort(Comparison<T> comparison)
    {
        _comparison = comparison;
        ApplySort();
    }

    public void Sort() 
    {
        ApplySort((x, y) => x.CompareTo(y));
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);

        if (_comparison != null && !_isSorted)
        {
            ApplySort();
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == "Count" && _comparison != null && !_isSorted)
        {
            ApplySort();
        }
    }

    private void ApplySort(Comparison<T> comparison)
    {
        _comparison = comparison;
        ApplySort();
    }

    private void ApplySort()
    {
        _isSorted = true;
        List<T> sortedItems = this.OrderBy(x => x, new ComparisonComparer<T>(_comparison)).ToList();

        using (var enumerator = sortedItems.GetEnumerator())
        {
            int i = 0;
            while (enumerator.MoveNext())
            {
                if (!Equals(this[i], enumerator.Current))
                {
                    Move(IndexOf(this[i]), i);
                }
                i++;
            }
        }
        _isSorted = false;
    }

    private class ComparisonComparer<T> : IComparer<T>
    {
        private readonly Comparison<T> _comparison;

        public ComparisonComparer(Comparison<T> comparison)
        {
            _comparison = comparison;
        }

        public int Compare(T x, T y)
        {
            return _comparison(x, y);
        }
    }
}
