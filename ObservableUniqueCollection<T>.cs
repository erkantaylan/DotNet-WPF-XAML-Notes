//https://stackoverflow.com/a/52192794/3767991
public class ObservableUniqueCollection<T> : ObservableCollection<T>
{
    private readonly HashSet<T> hashSet;

    public ObservableUniqueCollection() : this(EqualityComparer<T>.Default)
    {
    }

    public ObservableUniqueCollection(IEqualityComparer<T> equalityComparer)
    {
        hashSet = new HashSet<T>(equalityComparer);
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            InsertItem(Count, item);
        }
    }

    protected override void InsertItem(int index, T item)
    {
        if (hashSet.Add(item))
        {
            base.InsertItem(index, item);
        }
    }

    protected override void ClearItems()
    {
        base.ClearItems();
        hashSet.Clear();
    }

    protected override void RemoveItem(int index)
    {
        T item = this[index];
        hashSet.Remove(item);
        base.RemoveItem(index);
    }

    protected override void SetItem(int index, T item)
    {
        if (hashSet.Add(item))
        {
            T oldItem = this[index];
            hashSet.Remove(oldItem);
            base.SetItem(index, item);
        }
    }
}
