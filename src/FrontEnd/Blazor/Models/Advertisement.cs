namespace FrontEnd.Models;

using System;
using System.Collections.Generic;

public class Advertisement
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public object Address { get; set; }
    public int CategoryId { get; set; }
    public List<string> Thumbnails { get; set; }
    public List<AdvertisementProperty> Properties { get; set; }
    public List<string> Tags { get; set; }
}

public class AdvertisementProperty
{
    public string Key { get; set; }
    public string Value { get; set; }
}