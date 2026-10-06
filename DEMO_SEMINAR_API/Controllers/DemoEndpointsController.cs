using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DEMO_SEMINAR_API.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DEMO_SEMINAR_API.Controllers;

[ApiController]
[Route("api")]
public class DemoEndpointsController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private const string SecretKey = "Khoa_Bi_Mat_Cuc_Ky_An_Toan_Cho_Seminar_2026!";

    public DemoEndpointsController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    // 1. Endpoint phát hành JWT Token để test
    [HttpGet("get-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetToken([FromQuery] string username, [FromQuery] string role, [FromQuery] string dob, [FromQuery] string userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Role, role),
            new(ClaimTypes.DateOfBirth, dob) // Định dạng: YYYY-MM-DD
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "SeminarAuthServer",
            audience: "SeminarClients",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    // 2. Endpoint công khai: Ai cũng truy cập được
    [HttpGet("public")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult PublicEndpoint() => Ok("Ai cũng có thể thấy thông điệp này!");

    // 3. Endpoint yêu cầu chỉ cần xác thực danh tính (Authentication)
    [Authorize]
    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Profile() => Ok($"Xin chào {User.Identity?.Name}! Bạn đã xác thực danh tính thành công.");

    // 4. Endpoint phân quyền Policy theo Tuổi (Authorization - Custom Policy)
    [Authorize(Policy = "AtLeast18YearsOld")]
    [HttpGet("adult-only")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult AdultContent() => Ok("Chào mừng bạn đến với khu vực nội dung dành cho người từ 18 tuổi trở lên.");

    // 5. Endpoint Resource-based: Kiểm tra quyền sở hữu tài nguyên cụ thể
    [HttpPut("documents/{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> EditDocument(int id)
    {
        // Giả lập đọc tài nguyên từ DB: Document mẫu có OwnerId là "user_01"
        var sampleDoc = new Document { Id = id, Title = $"Tài liệu #{id}", OwnerId = "user_01" };

        // Thẩm định quyền với đối tượng sampleDoc thông qua DocumentOwnerPolicy
        var authResult = await _authorizationService.AuthorizeAsync(User, sampleDoc, "DocumentOwnerPolicy"); //Context: Resource-based Authorization
        if (!authResult.Succeeded)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Ẩn danh";
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                status = 403,
                error = "Forbidden",
                message = $"Từ chối quyền chỉnh sửa: Document #{id} thuộc quyền sở hữu của '{sampleDoc.OwnerId}', tài khoản của bạn là '{currentUserId}' nên không có quyền truy cập!",
                currentUserId = currentUserId,
                requiredOwnerId = sampleDoc.OwnerId,
                giaiThich = "Đây là kết quả của cơ chế Resource-based Authorization (Phân quyền theo tài nguyên). Chỉ tài khoản có userId trùng với OwnerId của tài liệu mới được phép chỉnh sửa.",
                huongDan = $"Để chỉnh sửa thành công (200 OK), hãy gọi GET /api/get-token và nhập userId là '{sampleDoc.OwnerId}', sau đó Authorize lại bằng token mới."
            });
        }

        return Ok(new
        {
            status = 200,
            message = $"Chỉnh sửa thành công Document ID = {id}. Chủ sở hữu là {sampleDoc.OwnerId}.",
            documentId = id,
            ownerId = sampleDoc.OwnerId
        });
    }
}
