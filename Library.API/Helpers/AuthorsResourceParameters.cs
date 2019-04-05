namespace Library.API.Helpers
{
    public class AuthorsResourceParameters
    {
        public AuthorsResourceParameters()
        {
            PageNumber = 1;
            OrderBy = "Name";
        }

        const int maxPageSize = 20;

        public int PageNumber { get; set; }

        private int _pageSize = 10;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        public string Genre { get; set; }

        public string SearchQuery { get; set; }

        public string OrderBy { get; set; }

        public string Fields { get; set; }
    }
}
