public enum ObservableActionTypes
{
    ItemContextActivated,
    ItemContextDeActivated,
    SkillContextActivated,
    SkillContextDeActivated,
    ChatActivated,
    ChatDeactivated,
    Paused,
    UnPaused
}

/// <summary>
/// Interface that notifies an IObservable about a change.
/// </summary>
public interface IObservable
{
    void AddToObserver(IObserver obs);
    void RemoveFromObserver(IObserver obs);
    void NotifyObserver(ObservableActionTypes action);
}
