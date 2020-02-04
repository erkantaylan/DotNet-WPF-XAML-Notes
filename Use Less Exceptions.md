Source ```https://blog.ploeh.dk/2020/02/03/non-exceptional-averages/```

```C#
public static TimeSpan Average(this IEnumerable<TimeSpan> timeSpans)
{
    if (!timeSpans.Any())
        throw new ArgumentOutOfRangeException(
            nameof(timeSpans),
            "Can't calculate the average of an empty collection.");
...

```

vs


[NotEmptyCollection.cs](NotEmptyCollection<T>.cs)

```C#

public static TimeSpan Average(this NotEmptyCollection<TimeSpan> timeSpans)
{
    var sum = timeSpans.Aggregate((x, y) => x + y);
    return sum / timeSpans.Count;
}

```

Other usefull sources

- [Parse, don't validate](https://lexi-lambda.github.io/blog/2019/11/05/parse-don-t-validate/)
