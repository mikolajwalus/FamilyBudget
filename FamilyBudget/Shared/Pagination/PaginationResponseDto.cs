namespace FamilyBudget.Shared.Pagination
{
    public class PaginationResponseDto
    {
        public PaginationResponseDto()
        {
        }

        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
