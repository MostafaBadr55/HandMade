namespace HandMade.ViewModels.Category
{
    public class GetAllCategoriesRequestVM
    {
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
