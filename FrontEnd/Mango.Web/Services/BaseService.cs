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
    private readonly ITokenProvider _tokenProvider;

    public BaseService(IHttpClientFactory clientFactory, ITokenProvider tokenProvider)
    {
        _clientFactory = clientFactory;
        _tokenProvider = tokenProvider;
    }

    public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withToken = true)
    {
        try
        {
            HttpClient client = _clientFactory.CreateClient("MangoAPI");
            HttpRequestMessage message = new(HttpMethod.Get, requestDto.Url);

            if (withToken)
            {
                var accessToken = _tokenProvider.GetToken();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

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
                    message = new HttpRequestMessage(HttpMethod.Get, requestDto.Url);
                    break;
            }

            if (requestDto.Data is not null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8,
                    "application/json");
            }

            if (requestDto.AccessToken is not null && requestDto.AccessToken is not "")
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", requestDto.AccessToken?.ToString());
            }
            
            HttpResponseMessage apiResponse = await client.SendAsync(message);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var responseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            return responseDto;
        }
        catch (Exception e)
        {
            var dto = new ResponseDto
            {
                Message = e.Message.ToString(),
                IsSuccess = false
            };
            return dto;
        }
    }
}