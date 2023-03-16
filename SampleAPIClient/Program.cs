// See https://aka.ms/new-console-template for more information
using Polly;
using Polly.Contrib.WaitAndRetry;
using Refit;
using SampleAPIClient;
using SampleAPIClient.Domain.Dtos;
using SampleAPIClient.Domain.Models;
using System.Net;
using System.Text.Json;


 async Task Get()
{

    var policy = Policy
        .Handle<ApiException>(ex => ex.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryAsync(
            5,
            retryAttempt => TimeSpan.FromSeconds(retryAttempt),
            async (exception, timeSpan, retryCount, context) =>
            {
                Console.WriteLine($"Retry #{retryCount}, delay: {timeSpan}");
                Console.WriteLine($"Exception: {exception.Message}");
            });

    var productAPI = RestService.For<IProductAPIClient>("https://dummyjson.com/op");

    ProductListDto productListApiResponse = null;
    try
    {
        productListApiResponse = await policy.ExecuteAsync(async () => await productAPI.GetProductList());
    }
    catch (ApiException ex)
    {
        Console.WriteLine($"API returned {ex.StatusCode} - {ex.Message}");
    }
}


async Task Get2()
{

    var retryPolicy = Policy<IApiResponse>
            .Handle<ApiException>()
            .OrResult(x => x.StatusCode is >= HttpStatusCode.NotFound)
            .WaitAndRetryAsync(
            5,
            retryAttempt => TimeSpan.FromSeconds(retryAttempt),
            async (exception, timeSpan, retryCount, context) =>
            {
                Console.WriteLine($"Retry #{retryCount}, delay: {timeSpan}");
            });

    var productAPI = RestService.For<IProductAPIClient>("https://dummyjson.com/op");

  
    var func = async () => (IApiResponse)await  productAPI.GetProductList2();

    var FooInfo = (ApiResponse<ProductListDto>)await retryPolicy.ExecuteAsync(func);
;
    await FooInfo.EnsureSuccessStatusCodeAsync();
}


async Task get3()
{
    var retryPolicy = Policy<IApiResponse>
            .Handle<ApiException>()
            .OrResult(x => x.StatusCode is >= HttpStatusCode.NotFound)
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 10), async (exception, timeSpan, retryCount, context) =>
            {
                Console.WriteLine($"Retry #{retryCount}, delay: {timeSpan}");
            });

    var productAPI = RestService.For<IProductAPIClient>("https://dummyjson.com/op");

   
    var func = async () => (IApiResponse)await productAPI.GetProductList2();

    var FooInfo = (ApiResponse<ProductListDto>)await retryPolicy.ExecuteAsync(func);

}