public class RangedNumber<T> : BindableBase where T : IComparable
{
    private T maximum;
    private T minimum;
    private T value;

    public RangedNumber()
    {
        PropertyChanged += OnPropertyChanged;
    }


    public RangedNumber(T minimum, T maximum, T value)
    {
        ValidateRanges(minimum, maximum);

        this.minimum = minimum;
        this.maximum = maximum;
        Value = value;
        PropertyChanged += OnPropertyChanged;
    }

    public T Minimum
    {
        get => minimum;
        set => SetProperty(ref minimum, value);
    }

    public T Maximum
    {
        get => maximum;
        set => SetProperty(ref maximum, value);
    }

    public T Value
    {
        get => value;
        set => SetProperty(ref this.value, value);
    }

    private static void ValidateRanges(T minimum, T maximum)
    {
        if ((minimum, maximum) != (null, null))
        {
            if (minimum.CompareTo(maximum) > 0)
            {
                throw new MinimumIsBiggerThanMaximumException($"Minimum value({minimum}) cannot be bigger than Maximum({maximum})");
            }
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        ValidateRanges(Minimum, Maximum);

        if (value.CompareTo(Minimum) < 0)
        {
            Value = Minimum;
        }
        else if (value.CompareTo(Maximum) > 0)
        {
            Value = Maximum;
        }
    }
}

[Serializable]
public class MinimumIsBiggerThanMaximumException : Exception
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public MinimumIsBiggerThanMaximumException() { }
    public MinimumIsBiggerThanMaximumException(string message) : base(message) { }
    public MinimumIsBiggerThanMaximumException(string message, Exception inner) : base(message, inner) { }


    protected MinimumIsBiggerThanMaximumException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
