namespace SubastaYa.API.DTOs
{
    public class CreateAuctionRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public DateTime EndDate { get; set; }
        public string UrlImage { get; set; } = "https://via.placeholder.com/150";
        public int CategoryId { get; set; }
    }
}