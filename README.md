# DEMO SEMINAR: AUTHENTICATION & AUTHORIZATION TRONG ASP.NET CORE 8

Dự án Web API mẫu phục vụ báo cáo seminar về các cơ chế Xác thực (Authentication) và Phân quyền (Authorization) trong ASP.NET Core 8.

---

## 1. CÁC TÍNH NĂNG CHÍNH ĐƯỢC DEMO
1. **JWT Bearer Authentication**: Phát hành Token giả lập và xác thực Token qua Middleware.
2. **Policy-based Authorization (Custom Requirement)**: Kiểm tra điều kiện tuổi $\ge 18$ (`MinimumAgeRequirement` & `MinimumAgeHandler`).
3. **Resource-based Authorization**: Kiểm tra quyền chỉnh sửa tài nguyên động theo `OwnerId` (`DocumentOwnerRequirement` & `DocumentOwnerHandler`).
4. **Swagger UI Authorize**: Tích hợp nhập JWT Token trực tiếp trên giao diện Swagger.

---

## 2. HƯỚNG DẪN KHỞI CHẠY ỨNG DỤNG

### Cách A: Dùng Visual Studio (Khuyên dùng)
1. Mở solution / project trong Visual Studio.
2. Chọn profile **`https`** trên thanh công cụ và nhấn **`Ctrl + F5`** (Start Without Debugging).
3. Trình duyệt sẽ tự động mở Swagger UI tại:
   👉 **`https://localhost:7083/swagger`** (hoặc `http://localhost:5057/swagger`).

### Cách B: Dùng Terminal
```bash
dotnet run --project "D:\Learn\Project\SE310 - CNghe .NET\DEMO_SEMINAR_API\DEMO_SEMINAR_API"
```

---

## 3. LƯU Ý QUAN TRỌNG VỀ NÚT "AUTHORIZE" TRÊN SWAGGER

Nút **Authorize (hình chiếc ổ khóa)** nằm ở góc trên bên phải Swagger UI:
- **Định dạng chuẩn:** Header HTTP gửi lên server luôn phải có tiền tố `Bearer `, ví dụ:
  ```http
  Authorization: Bearer <chuỗi_token>
  ```
- **Swagger đã cấu hình `SecuritySchemeType.Http` (scheme `bearer`)**: Bạn chỉ cần dán **nguyên chuỗi token thô** vào ô Value, Swagger UI sẽ **tự động thêm** chữ `Bearer ` khi gửi.
- **Nếu dùng cấu hình cũ (`ApiKey`)**: Bạn bắt buộc phải gõ bằng tay: `Bearer <chuỗi_token>` (có chữ `Bearer` và dấu cách). Nếu thiếu chữ `Bearer `, server sẽ trả về lỗi **`401 Unauthorized`** kèm header `www-authenticate: Bearer`.

> 💡 **Cách kiểm tra nhanh**: Xem mục **Curl** sau khi Execute: Nếu thấy `-H 'Authorization: Bearer eyJ...'` là chuẩn.

---

## 4. KỊCH BẢN THỰC HÀNH 5 TÌNH HUỐNG DEMO

### Tình huống 1: Cơ chế Challenge (HTTP 401 Unauthorized)
- **Thao tác:** Chưa đăng nhập (chưa Authorize), gọi `GET /api/profile`.
- **Kết quả:** Trả về **`401 Unauthorized`**.
- **Giải thích:** `[Authorize]` kích hoạt Challenge vì `User.Identity.IsAuthenticated` là `false`.

### Tình huống 2: Xác thực thành công (HTTP 200 OK)
- **Thao tác:**
  1. Vào `GET /api/get-token`, nhập `username = EmBe`, `role = User`, `dob = 2015-05-10`, `userId = user_02` $\rightarrow$ Bấm **Execute**.
  2. Copy chuỗi token trong `response body`.
  3. Bấm nút **Authorize** góc trên bên phải $\rightarrow$ Dán token vào $\rightarrow$ Bấm **Authorize** $\rightarrow$ **Close**.
  4. Gọi lại `GET /api/profile` $\rightarrow$ Bấm **Execute**.
- **Kết quả:** Trả về **`200 OK`**: `"Xin chào EmBe! Bạn đã xác thực danh tính thành công."`.

### Tình huống 3: Cơ chế Forbid (HTTP 403 Forbidden) qua Custom Policy
- **Thao tác:** Vẫn dùng token của `EmBe` (sinh 2015, $< 18$ tuổi), gọi `GET /api/adult-only`.
- **Kết quả:** Trả về **`403 Forbidden`**.
- **Giải thích:** Đã xác thực danh tính thành công, nhưng `MinimumAgeHandler` kiểm tra thấy tuổi $< 18$ nên không gọi `Succeed()`. Hệ thống kích hoạt cơ chế Forbid.

### Tình huống 4: Thỏa mãn Policy (HTTP 200 OK)
- **Thao tác:**
  1. Vào `GET /api/get-token`, nhập `username = NguyenVanA`, `dob = 1998-08-20`, `userId = user_01` $\rightarrow$ Lấy token mới.
  2. Bấm nút **Authorize** $\rightarrow$ **Logout** $\rightarrow$ Dán token mới $\rightarrow$ **Authorize**.
  3. Gọi lại `GET /api/adult-only`.
- **Kết quả:** Trả về **`200 OK`**: `"Chào mừng bạn đến với khu vực nội dung dành cho người từ 18 tuổi trở lên."`.

### Tình huống 5: Resource-based Authorization (Phân quyền trên tài nguyên)
Trong API mẫu, Document có `OwnerId = "user_01"`.
- **5A (Đúng chủ):** Dùng token `user_01`, gọi `PUT /api/documents/1` $\rightarrow$ Trả về **`200 OK`** (`"Chỉnh sửa thành công Document ID = 1. Chủ sở hữu là user_01."`).
- **5B (Khác chủ):** Đổi sang token `user_02` (`EmBe`), gọi `PUT /api/documents/1` $\rightarrow$ Trả về **`403 Forbidden`** (bị chặn bởi `DocumentOwnerHandler`).

---

## 5. BẢNG TỔNG KẾT KẾT QUẢ THỰC NGHIỆM

| Tình huống | Token | Endpoint | Mã HTTP | Bản chất kỹ thuật |
| :--- | :--- | :--- | :---: | :--- |
| **1. Chưa đăng nhập** | Không có | `GET /api/profile` | **401** | Challenge do chưa xác thực danh tính |
| **2. Đăng nhập hợp lệ** | User `EmBe` (2015) | `GET /api/profile` | **200** | Authentication thành công, tạo `ClaimsPrincipal` |
| **3. Không đủ tuổi** | User `EmBe` (2015) | `GET /api/adult-only` | **403** | Forbid do vi phạm `MinimumAgeRequirement` |
| **4. Đủ điều kiện** | User `NguyenVanA` (1998) | `GET /api/adult-only` | **200** | Handler gọi `context.Succeed()` thỏa Policy |
| **5. Khác chủ tài nguyên** | User `user_02` | `PUT /api/documents/1` | **403** | Khác `OwnerId` của tài liệu |

---

## 6. XỬ LÝ SỰ CỐ THƯỜNG GẶP (TROUBLESHOOTING)
- **Bị 401 khi đã dán Token:** Kiểm tra xem khung Curl có chứa `-H 'Authorization: Bearer eyJ...'` không. Nếu thiếu `Bearer `, nhập theo cú pháp: `Bearer <token>`.
- **Lỗi file bị lock khi build:** Bấm nút **Stop** (`Shift + F5`) trong Visual Studio trước khi build lại.
- **Lỗi cảnh báo SSL Localhost:** Chạy lệnh `dotnet dev-certs https --trust` trong PowerShell rồi chọn Yes.
