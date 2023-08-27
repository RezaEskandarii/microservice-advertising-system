namespace AdvertisingSystem.Identity.Shared.Attribs;

 

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public   class @DeprecatedAttribute : Attribute
{
    private string message;

    public DeprecatedAttribute(string message)
    {
        this.message = message;
    }

    public string Message 
    {
        get { return message; }
    }
}
