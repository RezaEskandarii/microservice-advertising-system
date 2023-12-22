namespace FrontEnd;

public class Events
{
    public event Action OnSignIn;

    public void InvokeSignIn()
    {
        OnSignIn?.Invoke();
    }
}