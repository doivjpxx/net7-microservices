using System.Net.Http.Headers;
using System.Text;
using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Mango.Web.Utils;
using Newtonsoft.Json;

namespace Mango.Web.Services;

public class BaseService : IBaseService
{
    private readonly IHttpClientFactory _clientFactory;
    
    public BaseService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }
    
    public Task<ResponseDto?> SendAsync(RequestDto requestDto)
    {
        HttpClient client = _clientFactory.CreateClient("MangoAPI");
        HttpRequestMessage message = new(HttpMethod.Get, requestDto.Url);
        
        switch (requestDto.ApiType)
        {
            case SD.ApiType.POST:
                message = new HttpRequestMessage(HttpMethod.Post, requestDto.Url);
                break;
            case SD.ApiType.PUT:
                message = new HttpRequestMessage(HttpMethod.Put, requestDto.Url);
                break;
            case SD.ApiType.DELETE:
                message = new HttpRequestMessage(HttpMethod.Delete, requestDto.Url);
                break;
            default:
                break;
        }
        
        if (requestDto.Data is not null)
        {
            message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
        }
        
        if (requestDto.AccessToken is not null && requestDto.AccessToken is not "")
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestDto.AccessToken?.ToString());
        }
        
        return client.SendAsync(message).Result.Content.ReadFromJsonAsync<ResponseDto>();
    }
}