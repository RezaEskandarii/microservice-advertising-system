using System.Text;
using System.Text.Json;
using FrontEnd.Models;

namespace FrontEnd.Extensions;

public static class ModelMethods
{
    public static StringContent ToStringContent(this BaseModel model)
    {
        var json = JsonSerializer.Serialize(model);
        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        return httpContent;
    }
    
}