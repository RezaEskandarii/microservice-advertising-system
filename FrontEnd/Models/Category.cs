namespace FrontEnd.Models;

public class Category
{
    public int id { get; set; }
    public string name { get; set; }
    public List<Category> subcategories { get; set; } = new List<Category>();
    public List<Property> properties { get; set; } = new List<Property>();

    public bool expanded { get; set; }
}

public class Property
{
    public string name { get; set; }
    public string data_type { get; set; }
}
