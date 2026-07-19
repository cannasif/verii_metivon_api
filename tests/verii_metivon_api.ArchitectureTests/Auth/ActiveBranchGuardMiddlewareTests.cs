using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using verii_metivon_api.Core.Auth;
using Xunit;

namespace verii_metivon_api.ArchitectureTests.Auth;

public sealed class ActiveBranchGuardMiddlewareTests
{
    [Fact]
    public async Task Matching_body_branch_is_allowed_and_exposed_to_the_request_scope()
    {
        var nextCalled = false;
        var middleware = new ActiveBranchGuardMiddleware(context =>
        {
            nextCalled = true;
            Assert.Equal(7L, context.Items["ActiveBranchId"]);
            return Task.CompletedTask;
        });
        var context = CreateContext("{\"branchId\":7}", 7, "MAIN");

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task Tampered_body_branch_is_rejected()
    {
        var nextCalled = false;
        var middleware = new ActiveBranchGuardMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });
        var context = CreateContext("{\"branchId\":99}", 7, "MAIN");

        await middleware.InvokeAsync(context);

        Assert.False(nextCalled);
        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    [Fact]
    public async Task Tampered_branch_header_is_rejected()
    {
        var middleware = new ActiveBranchGuardMiddleware(_ => Task.CompletedTask);
        var context = CreateContext("{}", 7, "MAIN");
        context.Request.Headers["X-Branch-Code"] = "OTHER";

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    [Fact]
    public async Task Tampered_nested_branch_is_rejected()
    {
        var middleware = new ActiveBranchGuardMiddleware(_ => Task.CompletedTask);
        var context = CreateContext("{\"branchId\":7,\"assignments\":[{\"branchId\":99}]}", 7, "MAIN");

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    private static DefaultHttpContext CreateContext(string body, long branchId, string branchCode)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/purchase-orders";
        context.Request.Method = Microsoft.AspNetCore.Http.HttpMethods.Post;
        context.Request.ContentType = "application/json";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        context.Response.Body = new MemoryStream();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim("branchId", branchId.ToString()),
            new Claim("branchCode", branchCode),
        ], "Test"));
        return context;
    }
}
