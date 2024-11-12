using Microsoft.AspNetCore.Mvc;
using MoonCore.Models;
using MoonCore.OAuth2.Requests;
using MoonCore.OAuth2.Responses;

namespace MoonCore.Extended.OAuth2.Consumer;

public class ControllerMethods
{
    public static async Task<StartResponse> Start<T>(ConsumerService<T> consumerService) where T : IUserModel
        => await consumerService.Start();
    
    public static async Task<TokenPair> Complete<T>(ConsumerService<T> consumerService, IServiceProvider serviceProvider, [FromBody] CompleteRequest request) where T : IUserModel
        => await consumerService.Complete(serviceProvider, request.Code);
    
    public static async Task<TokenPair> Refresh<T>(ConsumerService<T> consumerService, IServiceProvider serviceProvider, [FromBody] RefreshRequest request) where T : IUserModel
        => await consumerService.Refresh(serviceProvider, request.RefreshToken);
}