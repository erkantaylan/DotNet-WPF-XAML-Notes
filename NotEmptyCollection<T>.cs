public class NotEmptyCollection<T> : IReadOnlyCollection<T>
{
    public NotEmptyCollection(T head, params T[] tail)
    {
        if (head == null)
            throw new ArgumentNullException(nameof(head));
 
        this.Head = head;
        this.Tail = tail;
    }
 
    public T Head { get; }
 
    public IReadOnlyCollection<T> Tail { get; }
 
    public int Count { get => this.Tail.Count + 1; }
 
    public IEnumerator<T> GetEnumerator()
    {
        yield return this.Head;
        foreach (var item in this.Tail)
            yield return item;
    }
 
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
