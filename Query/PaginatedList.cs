using HealthAppAPI.Entities;

public class PaginatedList<T>
{
    public IEnumerable<T> ListItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPage { get; set; }

    public PaginatedList(IEnumerable<T> List, int CurrentPage, int TotalPage) 
    {
        ListItems = List;
        this.CurrentPage = CurrentPage;
        this.TotalPage = TotalPage;
    }
}