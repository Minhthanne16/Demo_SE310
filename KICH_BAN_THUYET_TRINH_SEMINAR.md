# KỊCH BẢN THUYẾT TRÌNH SEMINAR: AUTHENTICATION & AUTHORIZATION TRONG ASP.NET CORE 8
**Chủ đề:** *Cơ chế Xác thực (Authentication) & Phân quyền (Authorization) chuyên sâu trong ASP.NET Core 8 Web API*  
**Thời lượng dự kiến:** 25 - 35 phút  
**Công cụ sử dụng:** Visual Studio / VS Code, Swagger UI, Trình duyệt Web

---

## MỤC LỤC KỊCH BẢN
1. [Bảng tóm tắt phân bổ thời lượng](#1-bảng-tóm-tắt-phân-bổ-thời-lượng)
2. [Checklist chuẩn bị trước giờ G](#2-checklist-chuẩn-bị-trước-giờ-g)
3. [Phần 1: Mở đầu & Đặt vấn đề (00:00 - 02:30)](#phần-1-mở-đầu--đặt-vấn-đề-0000---0230)
4. [Phần 2: Nền tảng cốt lõi - Bản chất kỹ thuật (02:30 - 07:30)](#phần-2-nền-tảng-cốt-lõi---bản-chất-kỹ-thuật-0230---0730)
5. [Phần 3: Soi mã nguồn - Cấu hình hệ thống trong `Program.cs` (07:30 - 13:00)](#phần-3-soi-mã-nguồn---cấu-hình-hệ-thống-trong-programcs-0730---1300)
6. [Phần 4: LIVE DEMO 5 TÌNH HUỐNG THỰC NGHIỆM & CODE WALKTHROUGH (13:00 - 28:00)](#phần-4-live-demo-5-tình-huống-thực-nghiệm--code-walkthrough-1300---2800)
   - [Tình huống 1: Cơ chế Challenge - HTTP 401 Unauthorized](#tình-huống-1-cơ-chế-challenge---http-401-unauthorized)
   - [Tình huống 2: Xác thực danh tính thành công - HTTP 200 OK](#tình-huống-2-xác-thực-danh-tính-thành-công---http-200-ok)
   - [Tình huống 3: Cơ chế Forbid qua Custom Policy - HTTP 403 Forbidden](#tình-huống-3-cơ-chế-forbid-qua-custom-policy---http-403-forbidden)
   - [Tình huống 4: Thỏa mãn điều kiện Policy - HTTP 200 OK](#tình-huống-4-thỏa-mãn-điều-kiện-policy---http-200-ok)
   - [Tình huống 5: Phân quyền theo tài nguyên (Resource-based Authorization)](#tình-huống-5-phân-quyền-theo-tài-nguyên-resource-based-authorization)
7. [Phần 5: Tổng kết & Best Practices kiến trúc (28:00 - 31:00)](#phần-5-tổng-kết--best-practices-kiến-trúc-2800---3100)
8. [Phần 6: Bộ câu hỏi vấn đáp Q&A dự phòng (31:00 - 35:00)](#phần-6-bộ-câu-hỏi-vấn-đáp-qa-dự-phòng-3100---3500)

---

## 1. BẢNG TÓM TẮT PHÂN BỔ THỜI LƯỢNG

| Giai đoạn | Nội dung chính | Màn hình hiển thị | Thời lượng |
| :--- | :--- | :--- | :---: |
| **Phần 1** | Giới thiệu, dẫn nhập từ bài toán thực tế | Slide giới thiệu | ~2.5 phút |
| **Phần 2** | Lý thuyết cốt lõi (Authen vs Author, ClaimsPrincipal, Challenge vs Forbid) | Slide sơ đồ tư duy | ~5 phút |
| **Phần 3** | Phân tích kiến trúc Code & Pipeline trong `Program.cs` | Visual Studio (Program.cs) | ~5.5 phút |
| **Phần 4** | **Trọng tâm:** Thực hiện 5 tình huống Demo + Chiếu Code giải thích | Swagger UI $\leftrightarrow$ Code | ~15 phút |
| **Phần 5** | Bài học rút ra & Best Practices bảo mật | Slide tổng kết | ~3 phút |
| **Phần 6** | Giải đáp câu hỏi phản biện (Q&A) | Slide Q&A / Code | ~4 phút |

---

## 2. CHECKLIST CHUẨN BỊ TRƯỚC GIỜ G

- [ ] Dự án đã được build sạch: Chạy `dotnet build` không có warning/error.
- [ ] Khởi chạy sẵn Web API: `https://localhost:7083/swagger` (mở sẵn trên trình duyệt Chrome/Edge).
- [ ] Font size trình duyệt (Swagger) phóng to lên `110% - 125%` cho cả hội trường dễ nhìn.
- [ ] Visual Studio / VS Code đã mở sẵn các tab quan trọng:
  - `Program.cs`
  - `Controllers/DemoEndpointsController.cs`
  - `Authorization/MinimumAgeRequirement.cs`
  - `Authorization/DocumentOwnerHandler.cs`
- [ ] Font size trong code editor để cỡ `16px - 18px`.

---

## PHẦN 1: MỞ ĐẦU & ĐẶT VẤN ĐỀ (00:00 - 02:30)

### 🎯 Mục tiêu
Gây ấn tượng, xóa tan sự nhầm lẫn kinh điển giữa Authentication và Authorization, giúp người nghe tập trung ngay lập tức.

### 🖥️ Màn hình hiển thị
- Slide tiêu đề Seminar hoặc màn hình VS Code dự án.

### 🎙️ Lời thoại của người thuyết trình
> *"Kính chào thầy cô và tất cả các bạn.*  
>  
> *Khi xây dựng bất kỳ một hệ thống phần mềm hay Web API nào, bảo mật luôn là cánh cổng đầu tiên và quan trọng nhất. Tuy nhiên, trong quá trình học tập và làm dự án, rất nhiều lập trình viên chúng ta thường hay nói gộp hai khái niệm: **Authentication (Xác thực)** và **Authorization (Phân quyền)** thành một từ chung chung là 'bảo mật đăng nhập'.*  
>  
> *Sự nhầm lẫn này dẫn đến việc thiết kế hệ thống sai lệch: kiểm tra quyền hạn tùy tiện trong Controller, hard-code vai trò, hoặc không nắm được khi nào hệ thống trả về mã **401 Unauthorized** và khi nào trả về **403 Forbidden**.*  
>  
> *Hôm nay, nhóm mình xin gửi đến thầy cô và các bạn bài seminar chuyên sâu: **Cơ chế Authentication và Authorization trong ASP.NET Core 8**. Không chỉ dừng lại ở lý thuyết suông, nhóm mình đã xây dựng một hệ thống Web API thực nghiệm hoàn chỉnh để chứng minh từng cơ chế: từ JWT Bearer, Policy-based với Custom Requirement, cho đến kỹ thuật nâng cao nhất là **Resource-based Authorization** (phân quyền theo từng đối tượng dữ liệu).*  
>  
> *Ngay sau đây, chúng ta sẽ cùng đi qua bản chất kỹ thuật bên dưới của framework ASP.NET Core."*


---

## PHẦN 2: NỀN TẢNG CỐT LÕI - BẢN CHẤT KỸ THUẬT (02:30 - 07:30)

### 🎯 Mục tiêu
Làm rõ 3 trụ cột lý thuyết của ASP.NET Core:
1. Phân biệt Authentication vs Authorization bằng ví dụ đời thực dễ hiểu.
2. Mô hình định danh người dùng: `Claim` $\rightarrow$ `ClaimsIdentity` $\rightarrow$ `ClaimsPrincipal`.
3. Hai phản ứng bảo mật cốt lõi: **Challenge (401)** và **Forbid (403)**.

### 🖥️ Màn hình hiển thị
- Chiếu slide so sánh hoặc sơ đồ kiến trúc Claims.

### 🎙️ Lời thoại của người thuyết trình

#### 1. Ví dụ đời thực: Câu chuyện vé máy bay
> *"Để phân biệt dễ nhất, các bạn hãy hình dung việc chúng ta đi máy bay:*  
> - ***Authentication (Xác thực danh tính)***: *Bạn đưa Căn cước công dân hoặc Hộ chiếu cho nhân viên an ninh tại cổng soát vé. Nhân viên nhìn mặt bạn và đối chiếu giấy tờ để trả lời câu hỏi: **'Bạn có đúng là Nguyễn Văn A hay không?'**. Quá trình này chứng minh **BẠN LÀ AI**.*  
> - ***Authorization (Phân quyền)***: *Sau khi biết bạn là Nguyễn Văn A rồi, tiếp viên nhìn vào cuống vé của bạn: Vé thường hay vé Thương gia? Bạn có được bước vào phòng chờ VIP Lounges hay không? Bạn có được bước vào buồng lái phi công không? Đây là quá trình trả lời câu hỏi: **'Với danh tính này, BẠN ĐƯỢC PHÉP LÀM GÌ?'**.*  
>  
> *Nhớ quy tắc vàng: **Phải biết bạn là ai (Authentication) thì mới biết bạn được làm gì (Authorization)**."*

#### 2. ASP.NET Core quản lý danh tính như thế nào? (Claims-based Identity)
> *"Trong ASP.NET Core, danh tính không chỉ là một chuỗi username đơn giản, mà được thiết kế theo mô hình **Claims-based Identity**:*  
> 1. ***Claim (Tuyên bố)***: *Là một cặp Key-Value đại diện cho một đặc điểm của người dùng. Ví dụ: `Name = Khang`, `DateOfBirth = 2000-01-01`, `Role = Admin`.*  
> 2. ***ClaimsIdentity (Giấy tờ tùy thân)***: *Tập hợp nhiều Claim lại. Giống như một chiếc Hộ chiếu hoặc một chiếc Thẻ sinh viên.*  
> 3. ***ClaimsPrincipal (Chủ thể người dùng)***: *Một người có thể có nhiều thẻ (vừa có Thẻ căn cước, vừa có Bằng lái xe, vừa có Thẻ ngân hàng). Toàn bộ gói giấy tờ này được ASP.NET Core đóng gói thành đối tượng **`ClaimsPrincipal`**, và nó xuất hiện chính là biến **`User`** mà chúng ta hay gọi trong Controller (`User.Identity.Name`)."*

#### 3. Hai cơ chế phản hồi: Challenge (401) vs Forbid (403)
> *"Rất nhiều bạn đi phỏng vấn hay bị hỏi: **Khác biệt giữa HTTP 401 và HTTP 403 là gì?***  
> - ***HTTP 401 Unauthorized (Cơ chế Challenge)***: *Server nói: 'Tôi không biết bạn là ai cả, hãy xuất trình token/giấy tờ để chứng minh danh tính đi!' $\rightarrow$ Kích hoạt khi chưa xác thực.*  
> - ***HTTP 403 Forbidden (Cơ chế Forbid)***: *Server nói: 'Tôi biết rất rõ bạn là ai rồi (Authentication đã thành công), nhưng bạn KHÔNG CÓ ĐỦ QUYỀN để sờ vào tài nguyên này!' $\rightarrow$ Kích hoạt khi bị từ chối quyền.*  
>  
> *Bây giờ, chúng ta sẽ mở mã nguồn xem ASP.NET Core 8 hiện thực các cơ chế này thế nào."*


---

## PHẦN 3: SOI MÃ NGUỒN - CẤU HÌNH HỆ THỐNG TRONG `PROGRAM.CS` (07:30 - 13:00)

### 🎯 Mục tiêu
Chỉ trực tiếp code trong `Program.cs`, giải thích thứ tự Middleware bắt buộc và cách đăng ký Policy, Handler.

### 🖥️ Màn hình hiển thị
- Chuyển sang **Visual Studio**, mở file `Program.cs`. Cuộn chuột đến từng block code và bôi đen.

### 🎙️ Lời thoại của người thuyết trình

#### 1. Đăng ký Authentication với JWT Bearer (Dòng 38 - 57)
*(Chỉ chuột vào khối code `builder.Services.AddAuthentication`)*
> *"Đầu tiên, tại file `Program.cs`, chúng ta đăng ký dịch vụ Authentication bằng hàm `AddAuthentication`:*  
> ```csharp
> builder.Services.AddAuthentication(options =>
> {
>     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
>     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
> })
> .AddJwtBearer(options => { ... });
> ```
> *Ở đây, ta chỉ định Scheme mặc định là **JwtBearerDefaults**. Nghĩa là mỗi request gửi lên, Middleware sẽ tự động tìm header `Authorization: Bearer <token>`, giải mã chữ ký với `SecretKey` xem token có bị làm giả hay không, kiểm tra hạn sử dụng (`ValidateLifetime`), kiểm tra nhà phát hành (`ValidateIssuer`). Nếu token chuẩn, nó tự động trích xuất các Claim và gán vào `HttpContext.User`."*

#### 2. Cấu hình các Policy phân quyền (Dòng 59 - 72)
*(Chỉ chuột vào khối code `builder.Services.AddAuthorization`)*
> *"Tiếp theo là tầng Phân quyền. Thay vì dùng `[Authorize(Roles = "Admin")]` một cách cứng nhắc, ASP.NET Core hỗ trợ **Policy-based Authorization**:*  
> ```csharp
> builder.Services.AddAuthorization(options =>
> {
>     // Policy 1: Theo Role
>     options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
>
>     // Policy 2: Custom Requirement kiểm tra tuổi >= 18
>     options.AddPolicy("AtLeast18YearsOld", policy =>
>         policy.Requirements.Add(new MinimumAgeRequirement(18)));
>
>     // Policy 3: Resource-based kiểm tra quyền chủ sở hữu tài liệu
>     options.AddPolicy("DocumentOwnerPolicy", policy =>
>         policy.Requirements.Add(new DocumentOwnerRequirement()));
> });
> ```
> *Các bạn thấy ở đây nhóm mình đã định nghĩa 2 Policy nâng cao: `AtLeast18YearsOld` và `DocumentOwnerPolicy`."*

#### 3. Đăng ký Handler vào Dependency Injection Container (Dòng 74 - 76)
*(Chỉ chuột vào dòng `builder.Services.AddSingleton<IAuthorizationHandler, ...>`)*
> *"Để xử lý logic thẩm định cho các Policy trên, ta đăng ký các Handler vào DI Container:*  
> ```csharp
> builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
> builder.Services.AddSingleton<IAuthorizationHandler, DocumentOwnerHandler>();
> ```
> *Bất cứ khi nào Policy được gọi, ASP.NET Core sẽ tự động inject Handler tương ứng để thực thi nghiệp vụ thẩm định."*

#### 4. Quy tắc sống còn: Thứ tự Middleware (Dòng 87 - 89)
*(Highlight 2 dòng `app.UseAuthentication();` và `app.UseAuthorization();`)*
> *"Một lỗi kinh điển của người mới học là đảo ngược 2 dòng này:*  
> ```csharp
> app.UseAuthentication(); // 1. Xác thực: Biết bạn là ai -> Tạo User
> app.UseAuthorization();  // 2. Phân quyền: Căn cứ vào User vừa tạo để cấp quyền
> ```
> *Nếu bạn đặt `UseAuthorization` lên trước `UseAuthentication`, khi kiểm tra quyền thì đối tượng `User` vẫn là Anonymous (chưa được giải mã danh tính), dẫn đến API luôn bị chặn 401 hoặc 403! Thứ tự trong HTTP Request Pipeline của ASP.NET Core luôn luôn là **Authentication TRƯỚC, Authorization SAU**."*


---

## PHẦN 4: LIVE DEMO 5 TÌNH HUỐNG THỰC NGHIỆM & CODE WALKTHROUGH (13:00 - 28:00)

### 🎯 Mục tiêu
Thực hiện lần lượt 5 kịch bản trên Swagger UI. Mỗi khi có kết quả (401, 200, 403), quay lại Code giải thích chính xác dòng nào trong Controller / Handler đã tạo ra kết quả đó.

---

### Tình huống 1: Cơ chế Challenge - HTTP 401 Unauthorized

#### 🖥️ Thao tác màn hình
1. Mở giao diện Swagger UI (`https://localhost:7083/swagger`).
2. Đảm bảo nút **Authorize 🔓** đang ở trạng thái chưa đăng nhập (ổ khóa mở).
3. Mở endpoint **`GET /api/profile`** $\rightarrow$ Bấm **Try it out** $\rightarrow$ Bấm **Execute**.
4. Màn hình hiện response: **`401 Unauthorized`**.

#### 🎙️ Lời thoại & Phân tích Code
> *"Bây giờ mình bắt đầu với Tình huống 1. Mình đang là một người dùng vãng lai, chưa đăng nhập, chưa có Token. Mình gọi vào API `GET /api/profile`.*  
>  
> *(Bấm Execute $\rightarrow$ Chỉ vào mã 401 trên Swagger)*  
> *Ngay lập tức, server trả về **401 Unauthorized** kèm header `www-authenticate: Bearer`.*  
>  
> *(Chuyển sang Visual Studio, mở `Controllers/DemoEndpointsController.cs`, dòng 53 - 59)*  
> ```csharp
> [Authorize]
> [HttpGet("profile")]
> public IActionResult Profile() => Ok($"Xin chào {User.Identity?.Name}! Bạn đã xác thực danh tính thành công.");
> ```
> *Vì endpoint này được gắn thuộc tính `[Authorize]`. Khi request chạy qua `AuthenticationMiddleware`, do không có Bearer token hợp lệ trong Header nên `User.Identity.IsAuthenticated` bằng `false`. Framework tự động kích hoạt **Cơ chế Challenge**, dừng xử lý và trả về 401: Yêu cầu client phải đăng nhập."*

---

### Tình huống 2: Xác thực danh tính thành công - HTTP 200 OK

#### 🖥️ Thao tác màn hình
1. Mở endpoint **`GET /api/get-token`** trên Swagger $\rightarrow$ Bấm **Try it out**.
2. Nhập các tham số giả lập:
   - `username`: `EmBe`
   - `role`: `User`
   - `dob`: `2015-05-10`
   - `userId`: `user_02`
3. Bấm **Execute** $\rightarrow$ Response trả về chuỗi JSON chứa JWT `token`.
4. Copy chuỗi token thô (không lấy dấu ngoặc kép `"`).
5. Cuộn lên đầu trang Swagger, bấm nút **Authorize 🔓** $\rightarrow$ Dán token vào ô **Value** $\rightarrow$ Bấm **Authorize** $\rightarrow$ **Close** (ổ khóa chuyển sang trạng thái khóa lại 🔒).
6. Quay lại `GET /api/profile` $\rightarrow$ Bấm **Execute**.
7. Màn hình hiện response: **`200 OK`**: `"Xin chào EmBe! Bạn đã xác thực danh tính thành công."`

#### 🎙️ Lời thoại & Phân tích Code
> *"Bây giờ sang Tình huống 2: Chúng ta cấp token cho một người dùng tên là 'EmBe', sinh năm 2015.*  
>  
> *(Chỉ vào code `DemoEndpointsController.cs`, dòng 25 - 46)*  
> *Hàm `GetToken` đóng vai trò như Auth Server. Nó đóng gói các Claims: `Name`, `NameIdentifier`, `Role`, `DateOfBirth` vào một JWT Token và ký bằng thuật toán HMAC-SHA256 với SecretKey.*  
>  
> *(Quay lại Swagger sau khi Authorize và gọi lại Profile)*  
> *Lần này, server trả về **200 OK**: 'Xin chào EmBe! Bạn đã xác thực danh tính thành công'.*  
> *Điều này chứng minh: JwtBearer Middleware đã bóc tách Token gửi trong header, giải mã chữ ký hợp lệ và nạp đối tượng `User` vào context thành công."*


---

### Tình huống 3: Cơ chế Forbid qua Custom Policy - HTTP 403 Forbidden

#### 🖥️ Thao tác màn hình
1. Giữ nguyên token của `EmBe` (sinh năm 2015 $\rightarrow$ mới 11 tuổi, chưa đủ 18).
2. Tìm endpoint **`GET /api/adult-only`** $\rightarrow$ Bấm **Try it out** $\rightarrow$ Bấm **Execute**.
3. Màn hình hiện response: **`403 Forbidden`**.

#### 🎙️ Lời thoại & Phân tích Code
> *"Bây giờ là tình huống phân quyền: 'EmBe' đã có tài khoản hợp lệ, nhưng muốn truy cập vào khu vực người lớn `GET /api/adult-only`.*  
>  
> *(Bấm Execute $\rightarrow$ Chỉ vào mã 403 trên Swagger)*  
> *Các bạn thấy rõ ràng: Mã trả về là **403 Forbidden**, hoàn toàn khác với mã 401 lúc nãy!*  
> *- **401** là: 'Tôi không biết bạn là ai'.*  
> *- Còn **403** là: 'Tôi biết bạn là EmBe rồi, nhưng bạn KHÔNG ĐỦ ĐIỀU KIỆN để vào đây!'.*  
>  
> *(Chuyển sang Visual Studio, mở `Authorization/MinimumAgeRequirement.cs`)*  
> *Tại sao lại bị chặn? Hãy nhìn vào cách cài đặt Policy của chúng mình:*  
> ```csharp
> // 1. Requirement: Chứa dữ liệu điều kiện
> public class MinimumAgeRequirement : IAuthorizationRequirement
> {
>     public int MinimumAge { get; }
>     public MinimumAgeRequirement(int minimumAge) => MinimumAge = minimumAge;
> }
>
> // 2. Handler: Chứa logic thẩm định nghiệp vụ
> public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
> {
>     protected override Task HandleRequirementAsync(
>         AuthorizationHandlerContext context, 
>         MinimumAgeRequirement requirement)
>     {
>         var dobClaim = context.User.FindFirst(ClaimTypes.DateOfBirth);
>         if (dobClaim == null) return Task.CompletedTask;
>
>         var dob = DateTime.Parse(dobClaim.Value);
>         var age = DateTime.Today.Year - dob.Year;
>         if (dob > DateTime.Today.AddYears(-age)) age--;
>
>         if (age >= requirement.MinimumAge)
>         {
>             context.Succeed(requirement); // Chỉ khi gọi hàm này thì mới ĐẠT
>         }
>         return Task.CompletedTask;
>     }
> }
> ```
> *Khi `EmBe` gửi token, `MinimumAgeHandler` đọc Claim `DateOfBirth` (2015), tính ra tuổi nhỏ hơn 18, nên **KHÔNG GỌI** `context.Succeed()`. ASP.NET Core phát hiện Requirement không được thỏa mãn, liền lập tức kích hoạt **Cơ chế Forbid**, trả về mã **403 Forbidden**!"*

---

### Tình huống 4: Thỏa mãn điều kiện Policy - HTTP 200 OK

#### 🖥️ Thao tác màn hình
1. Quay lại `GET /api/get-token` $\rightarrow$ Nhập dữ liệu của một người lớn:
   - `username`: `NguyenVanA`
   - `role`: `User`
   - `dob`: `1998-08-20` (Đủ tuổi $\ge 18$)
   - `userId`: `user_01`
2. Bấm **Execute** $\rightarrow$ Copy chuỗi token mới.
3. Bấm **Authorize 🔒** $\rightarrow$ **Logout** $\rightarrow$ Dán token mới $\rightarrow$ **Authorize** $\rightarrow$ **Close**.
4. Quay lại endpoint `GET /api/adult-only` $\rightarrow$ Bấm **Execute**.
5. Màn hình hiện response: **`200 OK`**: `"Chào mừng bạn đến với khu vực nội dung dành cho người từ 18 tuổi trở lên."`

#### 🎙️ Lời thoại & Phân tích Code
> *"Bây giờ mình đổi sang tài khoản 'NguyenVanA' sinh năm 1998 (28 tuổi).*  
> *(Bấm Execute)*  
> *Lập tức kết quả trả về **200 OK**!*  
> *Lần này, `MinimumAgeHandler` tính ra tuổi $\ge 18$, hàm `context.Succeed(requirement)` được kích hoạt. Policy đánh giá người dùng ĐẠT ĐIỀU KIỆN và cho phép request đi tiếp vào Controller action.*  
>  
> *Ưu điểm vượt trội của mô hình Requirement & Handler là tách rời hoàn toàn: Controller không cần biết cách tính tuổi như thế nào, logic kiểm tra nghiệp vụ nằm gọn trong Handler, rất dễ tái sử dụng và viết Unit Test độc lập."*


---

### Tình huống 5: Phân quyền theo tài nguyên (Resource-based Authorization)

#### 🎯 Mục tiêu tình huống
Chứng minh trường hợp thực tế phức tạp nhất: Một người dùng có quyền sửa bài viết, nhưng **chỉ được sửa bài viết do chính mình tạo ra**, không được sửa bài của người khác.

#### 🖥️ Thao tác màn hình (Phần 5A: Thử sửa khi KHÔNG PHẢI CHỦ TÀI NGUYÊN)
1. Đang dùng token của `user_02` (hoặc tạo một token có `userId = "user_02"`).
2. Tìm endpoint **`PUT /api/documents/{id}`** $\rightarrow$ Bấm **Try it out**.
3. Nhập `id = 3` $\rightarrow$ Bấm **Execute**.
4. Màn hình hiện kết quả: **`403 Forbidden`** với JSON giải thích chi tiết:
   ```json
   {
     "status": 403,
     "error": "Forbidden",
     "message": "Từ chối quyền chỉnh sửa: Document #3 thuộc quyền sở hữu của 'user_01', tài khoản của bạn là 'user_02' nên không có quyền truy cập!",
     "currentUserId": "user_02",
     "requiredOwnerId": "user_01"
   }
   ```

#### 🎙️ Lời thoại & Phân tích Code (5A)
> *"Bây giờ là tình huống thực tế và cao cấp nhất: **Resource-based Authorization**.*  
> *Giả sử trên hệ thống có một tài liệu `Document #3` được tạo bởi tác giả có mã `user_01`.*  
> *Mình đang đăng nhập bằng tài khoản `user_02` và cố tình gửi lệnh sửa tài liệu này.*  
>  
> *(Bấm Execute $\rightarrow$ Hiện 403 Forbidden kèm JSON giải thích)*  
> *Hệ thống ngay lập tức từ chối và báo rõ ràng: Document #3 thuộc về 'user_01', tài khoản của bạn là 'user_02' nên không có quyền.*  
>  
> *(Chuyển sang Visual Studio, mở `Controllers/DemoEndpointsController.cs`, dòng 70 - 95)*  
> ```csharp
> [HttpPut("documents/{id}")]
> [Authorize]
> public async Task<IActionResult> EditDocument(int id)
> {
>     var sampleDoc = new Document { Id = id, Title = $"Tài liệu #{id}", OwnerId = "user_01" };
>
>     // Thẩm định quyền ĐỘNG dựa trên đối tượng sampleDoc
>     var authResult = await _authorizationService.AuthorizeAsync(User, sampleDoc, "DocumentOwnerPolicy");
>     if (!authResult.Succeeded)
>     {
>         return StatusCode(StatusCodes.Status403Forbidden, ...);
>     }
>     ...
> }
> ```
> *Các bạn chú ý dòng `_authorizationService.AuthorizeAsync(User, sampleDoc, "DocumentOwnerPolicy")`. Đây chính là điểm khác biệt cốt tử:*  
> *- Với `[Authorize]` trên đầu hàm, chúng ta chỉ kiểm tra được thông tin tĩnh của User trước khi hàm chạy.*  
> *- Nhưng với Resource-based, ta phải truy vấn dữ liệu từ DB lên (đối tượng `sampleDoc`), rồi mới truyền cả `User` và `sampleDoc` vào `IAuthorizationService` để kiểm tra."*  
>  
> *(Mở file `Authorization/DocumentOwnerHandler.cs`)*  
> ```csharp
> public class DocumentOwnerHandler : AuthorizationHandler<DocumentOwnerRequirement, Document>
> {
>     protected override Task HandleRequirementAsync(
>         AuthorizationHandlerContext context,
>         DocumentOwnerRequirement requirement,
>         Document resource)
>     {
>         var currentUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
>         if (currentUserId != null && resource.OwnerId == currentUserId)
>         {
>             context.Succeed(requirement);
>         }
>         return Task.CompletedTask;
>     }
> }
> ```
> *`DocumentOwnerHandler` kế thừa từ `AuthorizationHandler<TRequirement, TResource>`. Nó so sánh `ClaimTypes.NameIdentifier` của người đăng nhập với thuộc tính `resource.OwnerId`. Khác nhau $\rightarrow$ Không gọi `Succeed()` $\rightarrow$ Thất bại!"*

#### 🖥️ Thao tác màn hình (Phần 5B: Sửa thành công với ĐÚNG CHỦ TÀI NGUYÊN)
1. Lấy token mới cho `userId = "user_01"` từ `GET /api/get-token`.
2. Bấm **Authorize 🔓** $\rightarrow$ **Logout** $\rightarrow$ Dán token `user_01` $\rightarrow$ **Authorize**.
3. Quay lại **`PUT /api/documents/3`** $\rightarrow$ Bấm **Execute**.
4. Màn hình hiện kết quả: **`200 OK`**:
   ```json
   {
     "status": 200,
     "message": "Chỉnh sửa thành công Document ID = 3. Chủ sở hữu là user_01.",
     "documentId": 3,
     "ownerId": "user_01"
   }
   ```

#### 🎙️ Lời thoại (5B)
> *"Bây giờ mình đổi lại đúng token của `user_01` (chính chủ tài liệu) và Execute lại.*  
> *(Bấm Execute $\rightarrow$ Hiện 200 OK)*  
> *Lập tức trả về **200 OK**! `resource.OwnerId == currentUserId` đều là `user_01`, Handler đánh dấu thành công và cho phép chỉnh sửa tài liệu.*  
> *Đây là mô hình phân quyền bảo mật tiêu chuẩn mà các hệ thống lớn như Google Drive, Facebook hay GitHub sử dụng để kiểm tra quyền sở hữu tài nguyên."*


---

## PHẦN 5: TỔNG KẾT & BEST PRACTICES KIẾN TRÚC (28:00 - 31:00)

### 🎯 Mục tiêu
Đúc kết lại các kiến thức đắt giá nhất để tạo chiều sâu học thuật và kỹ thuật cho bài báo cáo.

### 🖥️ Màn hình hiển thị
- Chiếu slide Bảng tổng kết kết quả thực nghiệm và Best Practices.

### 🎙️ Lời thoại của người thuyết trình
> *"Qua 5 tình huống thực nghiệm vừa rồi, nhóm mình xin đúc kết lại các bài học kiến trúc quan trọng khi làm việc với ASP.NET Core 8:*  
>  
> 1. ***Tách bạch ranh giới rõ ràng:***  
>    - *Authentication lo việc cấp phát và giải mã danh tính (`ClaimsPrincipal`).*  
>    - *Authorization chỉ tập trung vào việc thẩm định điều kiện dựa trên Claims và Resource.*  
> 2. ***Luôn ưu tiên Policy-based thay vì Role-based thuần túy:***  
>    - *Nếu dùng `[Authorize(Roles = "Manager")]`, sau này khách hàng đổi nghiệp vụ muốn 'Trưởng phòng hoặc Nhân viên có thâm niên trên 5 năm được duyệt đơn' thì bạn phải sửa code ở khắp mọi Controller.*  
>    - *Nếu đóng gói thành Policy, bạn chỉ cần sửa logic trong Handler mà không cần đụng đến Controller.*  
> 3. ***Thứ tự Middleware là bất di bất dịch:***  
>    - *Luôn nhớ: `UseAuthentication()` đứng trước `UseAuthorization()`.*  
> 4. ***Bảo mật SecretKey:***  
>    - *Trong bài demo, SecretKey được để trực tiếp trong code để tiện theo dõi. Tuy nhiên trong môi trường Production thực tế, key này bắt buộc phải lưu trong Azure Key Vault, AWS Secrets Manager hoặc Environment Variables, tuyệt đối không commit lên Git repository."*

---

## PHẦN 6: BỘ CÂU HỎI VẤN ĐÁP Q&A DỰ PHÒNG (31:00 - 35:00)

Dưới đây là các câu hỏi mà Thầy Cô hoặc Hội đồng phản biện rất hay hỏi, kèm theo câu trả lời chuẩn xác nhất:

---

### ❓ Câu hỏi 1: JWT Token có nhược điểm gì về mặt bảo mật? Làm sao để thu hồi (revoke) một token khi người dùng bị khóa tài khoản hoặc bấm Đăng xuất?
> **Trả lời:**  
> *"Thưa thầy cô, bản chất của JWT là **Stateless** (phi trạng thái) – Server không lưu session trong database/RAM mà chỉ kiểm tra tính hợp lệ của chữ ký số. Do đó, nhược điểm lớn nhất là một khi Token đã phát hành thì Server không thể hủy ngang hiệu lực của nó trước khi nó hết hạn (`exp`).*  
> *Để giải quyết vấn đề này trong thực tế, chúng ta áp dụng 2 giải pháp:*  
> 1. *Đặt thời gian sống của Access Token cực ngắn (ví dụ 5 - 15 phút), kết hợp với cơ chế **Refresh Token** lưu trong DB.*  
> 2. *Sử dụng cơ chế **Token Blacklist** lưu trên Redis: Khi user Logout hoặc bị Ban, ta lưu `jti` (Token ID) vào Redis với thời gian TTL bằng thời gian sống còn lại của token. Trong Middleware, ta kiểm tra thêm xem token có nằm trong Blacklist hay không."*

---

### ❓ Câu hỏi 2: Tại sao trong `Program.cs`, bạn đăng ký Handler bằng `AddSingleton` mà không phải `AddScoped` hay `AddTransient`?
> **Trả lời:**  
> *"Thưa thầy cô:*  
> *- Với `MinimumAgeHandler`, bên trong nó chỉ tính toán tuổi thuần túy dựa trên Claims từ `context.User`, không phụ thuộc vào bất kỳ Service nào có State hoặc `DbContext`. Do đó đăng ký dạng **Singleton** là tối ưu nhất về hiệu năng, không tốn chi phí khởi tạo lại đối tượng qua mỗi request.*  
> *- Tuy nhiên, nếu một Handler cần inject `AppDbContext` để truy vấn cơ sở dữ liệu (ví dụ truy vấn bảng User xem tài khoản có bị khóa không), thì bắt buộc Handler đó phải được đăng ký là **Scoped** (`AddScoped`), vì `DbContext` trong ASP.NET Core có vòng đời là Scoped. Nếu đăng ký Singleton trong trường hợp đó sẽ vi phạm quy tắc Captive Dependency."*

---

### ❓ Câu hỏi 3: Nếu một Policy có nhiều Handler (`IAuthorizationHandler`) cùng xử lý thì ASP.NET Core sẽ đánh giá kết quả như thế nào?
> **Trả lời:**  
> *"Thưa thầy cô, ASP.NET Core xử lý theo cơ chế **OR (hoặc)** đối với các Handler của cùng một Requirement:*  
> *- Chỉ cần **ít nhất một Handler** gọi `context.Succeed(requirement)` và **không có Handler nào** gọi `context.Fail()`, thì Requirement đó được xem là ĐẠT.*  
> *- Nếu có bất kỳ Handler nào gọi rõ ràng `context.Fail()`, thì thẩm định sẽ thất bại ngay lập tức bất chấp các Handler khác có gọi `Succeed()` hay không (cơ chế dứt khoát).*  
> *- Nếu không có Handler nào gọi `Succeed()` lẫn `Fail()`, hệ thống mặc định coi là không thỏa mãn điều kiện."*

---

### ❓ Câu hỏi 4: Nút "Authorize" trên Swagger hoạt động thế nào? Tại sao lúc trước hay bị lỗi 401 dù đã dán Token?
> **Trả lời:**  
> *"Thưa thầy cô, giao thức HTTP quy định header xác thực Bearer phải có định dạng:*  
> `Authorization: Bearer <chuỗi_token>` (có chữ Bearer và dấu cách).  
> *Trước đây nếu cấu hình Swagger bằng `SecuritySchemeType.ApiKey`, Swagger chỉ gửi nguyên si chuỗi ta dán vào. Nếu ta quên gõ chữ `Bearer ` ở đằng trước thì server không nhận dạng được Scheme và trả về 401.*  
> *Trong bài báo cáo này, nhóm đã tối ưu cấu hình bằng `SecuritySchemeType.Http` với scheme `bearer`. Nhờ đó Swagger UI tự động nhận diện và tự động thêm tiền tố `Bearer ` vào header khi gửi request, loại bỏ hoàn toàn khả năng lỗi do người dùng nhập thiếu."*

---

## 7. LỜI KẾT
> *"Bài thuyết trình và phần live demo của nhóm chúng em đến đây là kết thúc. Cảm ơn Thầy Cô và các bạn đã chú ý lắng nghe. Nhóm rất mong nhận được những nhận xét và câu hỏi đóng góp từ Thầy Cô và các bạn!"*
