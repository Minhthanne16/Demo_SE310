using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DEMO_SEMINAR_API.Authorization;

// Model tài nguyên cần bảo vệ
public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
}

// Requirement kiểm tra quyền sở hữu
public class DocumentOwnerRequirement : IAuthorizationRequirement { }

// Handler kiểm tra Resource-based Authorization
public class DocumentOwnerHandler : AuthorizationHandler<DocumentOwnerRequirement, Document>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, //thong tin nguoi dung hien tai
        DocumentOwnerRequirement requirement, //yeu cau kiem tra quyen so huu tai nguyen
        Document resource) //tai nguyen can bao ve
    {
        var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != null && resource.OwnerId == currentUserId)
        {
            context.Succeed(requirement); // Người dùng hiện tại là chủ sở hữu của tài nguyên
        }
        return Task.CompletedTask;
    }
}
