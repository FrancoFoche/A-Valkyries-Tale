/// <summary>
/// Interface that looks at an IObservable for a change
/// </summary>
public interface IObserver 
{
    void Notify(ObservableActionTypes action);
}