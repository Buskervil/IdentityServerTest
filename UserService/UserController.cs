using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace UserService;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _identityServerUrl;

    public UserController(IHttpClientFactory httpClientFactory, IOptions<IdentityServerConfig> config)
    {
        _httpClient = httpClientFactory.CreateClient();
        _identityServerUrl = config.Value.IdentityServerUrl;
    }

    [Authorize(Policy = "APIAccess")]
    [HttpGet]
    public async Task<IActionResult> GetUserInfo()
    {
        var userAccessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        
        var disco = await _httpClient.GetDiscoveryDocumentAsync(_identityServerUrl);
        if (disco.IsError)
            return BadRequest(disco.Error);

        var userInfoResponse = await _httpClient.GetUserInfoAsync(new UserInfoRequest
        {
            Address = disco.UserInfoEndpoint,
            Token = userAccessToken
        });

        if (userInfoResponse.IsError)
            return BadRequest(userInfoResponse.Error);

        return Ok(userInfoResponse.Claims);
    }
}