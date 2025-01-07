public class CategoryItem
{
    public int Id { get; }
    public string Name { get; }

    public CategoryItem(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
} 