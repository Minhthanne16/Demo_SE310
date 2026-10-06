using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DEMO_SEMINAR_API.Authorization;

// 1. Requirement: Chứa dữ liệu điều kiện thẩm định (Tuổi tối thiểu)
public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }
    public MinimumAgeRequirement(int minimumAge) => MinimumAge = minimumAge;
}

// 2. Handler: Chứa logic thẩm định nghiệp vụ
public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        MinimumAgeRequirement requirement)
    {
        var dobClaim = context.User.FindFirst(ClaimTypes.DateOfBirth);
        if (dobClaim == null)
        {
            // Không có Claim ngày sinh -> Thoát ra (chưa đạt)
            return Task.CompletedTask;
        }

        var dob = DateTime.Parse(dobClaim.Value);
        var age = DateTime.Today.Year - dob.Year;
        if (dob > DateTime.Today.AddYears(-age)) age--;

        if (age >= requirement.MinimumAge)
        {
            // Thỏa mãn điều kiện -> Đánh dấu ĐẠT
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
