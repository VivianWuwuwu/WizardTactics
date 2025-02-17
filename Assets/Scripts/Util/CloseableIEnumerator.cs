using System.Collections;

public class ClosableIEnumerator : IEnumerator
{
    private IEnumerator _enumerator;
    private System.Action _onClose;
    private bool _closed;

    public ClosableIEnumerator(IEnumerator enumerator, System.Action onClose)
    {
        _enumerator = enumerator;
        _onClose = onClose;
    }

    public bool MoveNext()
    {
        if (_closed) return false;
        return _enumerator.MoveNext();
    }

    public void Reset()
    {
        _enumerator.Reset();
    }

    public object Current => _enumerator.Current;

    public void Close()
    {
        if (!_closed)
        {
            _closed = true;
            _onClose?.Invoke();
        }
    }
}
