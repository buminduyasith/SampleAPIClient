
namespace SampleAPIClient
{
    using Refit;
    using SampleAPIClient.Domain.Dtos;

    public interface IProductAPIClient
    {
        [Get("/products")]
        Task<ProductListDto> GetProductList();

        [Get("/products")]
        Task<ApiResponse<ProductListDto>> GetProductList2();
    }
}
