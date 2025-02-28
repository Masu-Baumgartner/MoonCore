namespace MoonCore.Test;

public class OtherModel
{
    public string Other { get; set; }
    public List<Item> Abc { get; set; } = new();
    
    public class Item
    {
        public string A { get; set; }
    }
}