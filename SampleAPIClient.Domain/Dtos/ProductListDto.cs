
namespace SampleAPIClient.Domain.Dtos
{
    using SampleAPIClient.Domain.Models;

    public class ProductListDto
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public int Total { get; set; }
    }
}
