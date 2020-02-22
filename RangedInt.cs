public class RangedInt : BindableBase
{
    private int? maximum;
    private int? minimum;
    private int? value;

    public RangedInt()
    {
        PropertyChanged += OnPropertyChanged;
    }

    public RangedInt(int? minimum, int? maximum, int? value)
    {
        ValidateRanges(minimum, maximum);

        this.minimum = minimum;
        this.maximum = maximum;
        Value = value;
        PropertyChanged += OnPropertyChanged;
    }

    public int? Minimum
    {
        get => minimum;
        set
        {
            ValidateRanges(Minimum, Maximum);
            SetProperty(ref minimum, value);
        }
    }

    public int? Maximum
    {
        get => maximum;
        set
        {
            ValidateRanges(Minimum, Maximum);
            SetProperty(ref maximum, value);
        }
    }

    public int? Value
    {
        get => value;
        set => SetProperty(ref this.value, value);
    }

    private static void ValidateRanges(int? minimum, int? maximum)
    {
        if ((minimum, maximum) != (null, null))
        {
            if (minimum > maximum)
            {
                throw new Exception("Minimum value cannot be bigger than Maximum");
            }
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        ValidateRanges(Minimum, Maximum);

        if (value < Minimum)
        {
            Value = Minimum;
        }
        else if (value > Maximum)
        {
            Value = Maximum;
        }
    }
}
