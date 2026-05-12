namespace ChatTwo.Util;

public class Lender<T>
{
    private readonly Func<T> Ctor;
    private readonly List<T> Items = [];
    private int Counter;

    public Lender(Func<T> ctor)
    {
        Ctor = ctor;
    }

    public void ResetCounter()
    {
        Counter = 0;
    }

    public T Borrow()
    {
        if (Items.Count <= Counter)
            Items.Add(Ctor());

        return Items[Counter++];
    }
}
