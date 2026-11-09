# Nâng cấp Shooting Color

## Bộ sưu tập nền Jelly Pop mở rộng

Đã thêm **6 ảnh nền dọc 1024 × 1536** tạo bằng image_gen tích hợp: Biển kem, Vườn dâu, Rừng oải hương, Khinh khí cầu chanh, Đêm trăng kẹo và San hô ngọc trai. Cộng ba ảnh cũ thành **9 ảnh riêng**, với **10 lựa chọn** gồm cả biến thể Mây đêm. Các nền mới nằm ở `Assets/Resources/JellyPop/Backgrounds/`; prompt đầy đủ trong `Art/JellyPop/background-expansion-prompts.json`.

Mở **Cài đặt → Bộ sưu tập hình nền**, dùng **Trước/Sau** để duyệt ba trang. Chạm tên để áp dụng ngay cho menu, màn chơi và bảng màu giao diện; lựa chọn được lưu qua chuyển màn và mở lại game. Khi mở cài đặt, bộ chọn tự đến trang có nền hiện tại. Bốn chỉ số nền cũ được giữ nguyên để không làm đổi lựa chọn đã lưu.

Ảnh được hiển thị theo tỉ lệ gốc và cắt vừa khung, không kéo méo. Kiểm tra Node đã giải mã cả chín ảnh, xác nhận kích thước, tài nguyên của mười lựa chọn và độ dài các bảng màu tương ứng; kiểm tra 100 màn vẫn đạt. Đã mở rộng EditMode tests cho danh mục nền; chưa chạy Unity Test Runner, Play Mode hoặc build Android vì không có Unity Editor. Cần xem hình nền và phân trang trên thiết bị thật.

## Công tắc rung toàn game

Mở **Cài đặt → Rung toàn bộ game** từ trang chủ hoặc bất kỳ màn chơi nào. Công tắc Bật/Tắt lưu ngay bằng PlayerPrefs, giữ qua chuyển màn, đóng/mở game và đặt lại hành trình. Mọi phản hồi rung của nút bấm, phát bắn, tô đúng màu và hoàn thành màn đều đi qua `GameHaptics.Pulse` và tuân theo công tắc chung.

Khi tắt, thanh mức rung và nút Thử bị khóa, nhưng mức rung đã chọn vẫn được giữ cho lần bật sau. Khi mức rung là 0%, nút Thử cũng bị khóa. Tắt rung hoặc kéo mức về 0 sẽ gọi `cancel` để dừng xung đang phát trên Android; iOS chặn các xung tiếp theo vì API Handheld tích hợp không cung cấp hàm hủy xung đang phát. Bật lại cho phép thử rung ngay.

Đã bổ sung kiểm thử EditMode cho cổng rung chung, giữ mức rung khi đổi công tắc và trường hợp mức 0%. Cần chạy Unity Test Runner và kiểm tra phần cứng trên điện thoại; môi trường thực hiện chưa có Unity Editor.

## Jelly Pop — phong cách B đã chọn

Đã tích hợp bộ hình mới vào lớp giao diện runtime dùng chung. Mở `Assets/Scenes/Levels/MainMenu.unity` và bấm **Play** để xem; scene ở chế độ chỉnh sửa vẫn giữ canvas gốc. Có thể vào thẳng bất kỳ màn chơi nào để kiểm tra.

- **Menu, HUD, popup và cài đặt:** nút tím bóng, thẻ trắng viền lavender, icon nổi, sao vàng khi hoàn thành; nút nhún khi nhấn và popup mở nhẹ bằng thời gian unscaled.
- **Nền:** ba ảnh dọc riêng: Đảo mây, Thung lũng kẹo, Hoàng hôn đào. Mây đêm dùng ảnh Đảo mây với sắc tím tối. Chọn trong Cài đặt → Màu giao diện; giữ chỉ số giao diện đã lưu.
- **Khối và pháo:** shader Jelly Pop với highlight mềm; khối dùng mesh bo cạnh, giữ nguyên bounds của mesh nguồn, transform, collider, tên và màu dữ liệu. Pháo giữ mô hình 3D, phần đế chuyển lavender; trạng thái đã bắn được làm dịu màu bằng shader và phục hồi khi hoàn tác.
- **Hiệu ứng:** đạn bóng, vệt đạn thu nhỏ dần, tia lấp lánh, vòng sáng mở rộng, sao khi thắng. Công tắc hiệu ứng hiện có vẫn điều khiển các hiệu ứng mới.
- **Gameplay:** không đổi dữ liệu 100 scene, hướng/tốc độ bắn, luật tô màu, hoàn tác, thời gian, các lượt, tiến độ hay quảng cáo.

### Tài nguyên

- `Assets/Resources/JellyPop/UI/`: 16 PNG RGBA riêng, có alpha thật; icon, nút trống để đặt chữ tiếng Việt, panel, khối minh họa, sao và sprite hiệu ứng.
- `Assets/Resources/JellyPop/Backgrounds/`: 3 nền PNG 512 × 1024. Không phóng lớn giả độ phân giải; cần xem chất lượng trên thiết bị mục tiêu.
- `Assets/Resources/JellyPop/JellySurface.shader` và `JellyBackdrop.shader`: giữ trong Resources để shader được đưa vào player build.
- `Art/JellyPop/`: ảnh concept đã chọn, master UI trong suốt, master ba nền và prompt tạo ảnh. Đây là thư mục nguồn, không đóng gói vào player.

Các khối và pháo trong gameplay dùng 3D thật để giữ góc nhìn, đổi màu và va chạm; PNG khối phục vụ giao diện. Concept là hướng mỹ thuật, không phải ảnh chụp Play Mode.

### Kiểm tra Jelly Pop

Đã chạy thành công `node Tools/validate-jelly-pop.cjs`: giải mã pixel PNG để xác nhận 16 sprite có alpha trong suốt và nội dung nhìn thấy, xác nhận kích thước ba nền, metadata/GUID và tài nguyên shader. Đã chạy lại `node Tools/validate-upgrade.cjs`: 101 mục build, 100 cặp bàn/mẫu và 1.032 ô vẫn hợp lệ. `git diff --check` không có lỗi khoảng trắng.

Đã bổ sung `Assets/Tests/Editor/JellyPopTests.cs` để kiểm tra tài nguyên Resources và việc bo khối giữ màu/collider/transform/bounds, cùng chiều mặt tam giác. **Chưa chạy Unity Test Runner, biên dịch Unity, Play Mode hay build APK vì môi trường này không có Unity Editor.** Các kiểm tra Node không thay thế việc chạy game.

Trong Unity cần kiểm tra: menu và popup ở màn hình dọc; cả bốn nền; mẫu tham chiếu so với bàn chơi; bắn/hoàn tác/pháo đã dùng; chuyển lượt ở màn 51/76; tắt hiệu ứng; màu và độ mượt trên điện thoại. Chạy EditMode tests rồi build Android để xác nhận shader và ảnh trong player.

Phần dưới ghi lại đợt nâng cấp trước Jelly Pop; bảng màu và cách trình bày cũ đã được thay bởi mục này.

## Những thay đổi đã triển khai

| Hạng mục | Trước đây | Sau nâng cấp |
| --- | --- | --- |
| Cài đặt | Bật/tắt âm thanh, menu cũ | Bảng cài đặt chung từ trang chủ và trong mọi màn chơi |
| Rung | Chưa có | Bật/tắt, thanh mức 0–100%, nút thử; lưu qua lần mở game |
| Giao diện | Các canvas/prefab rời rạc | Bạc hà, oải hương, đào và đêm; đổi tức thì cho menu, HUD, popup, nền camera và mặt sân |
| Trình bày | Nút ảnh cũ, bố cục thiếu thống nhất | Thẻ bo góc, chữ tiếng Việt, nút lớn, khoảng trắng, vùng an toàn màn hình, tiến độ tô màu và mẫu tham chiếu |
| Phản hồi | Particle va chạm cũ | Vệt đạn, nổ màu có trọng lực, khối và pháo nhún, combo theo các ô đúng màu liên tiếp trong một phát bắn |
| Màn chơi | 50 scene | 100 scene được đăng ký trong Build Settings; Android profile dùng danh sách chung |
| Thử thách | Một lượt giải | 51–75 có hai lượt; 76–100 có ba lượt; đổi màu sau mỗi lượt và thêm đồng hồ |

### Lý do chọn cách triển khai này

UI mới được tạo ở runtime bởi `CalmGameUI`, thay thế phần trình bày của canvas cũ. Các tham chiếu gameplay và prefab cũ vẫn được giữ để tránh làm mất liên kết của 50 màn đã có. Chỉ có một bảng cài đặt và một hệ thống màu nên đổi giao diện được áp dụng xuyên suốt. Vào **Play** để xem UI mới; chế độ chỉnh scene vẫn hiển thị canvas gốc.

Màu đạn, pháo và mẫu là dữ liệu của câu đố. Đổi giao diện chỉ đổi các bề mặt trình bày, giữ khả năng phân biệt màu mục tiêu. Font động ưu tiên Segoe UI/Roboto/Noto Sans, có font Unity dự phòng.

Màn 51–100 là **biến thể nâng cao của 50 bố cục gốc**, không phải 50 bố cục được thiết kế thủ công mới. Mỗi lượt sau hoán vị đồng thời tên và màu của pháo, mẫu và các ô được tô sẵn bằng cùng một ánh xạ một-một. Vì thế thứ tự giải của bố cục gốc vẫn hợp lệ. Các lượt yêu cầu giải lại với màu mới; đồng hồ tạo thêm áp lực thời gian.

Đồng hồ bắt đầu khi bắn lần đầu trong từng lượt, giảm từ khoảng 120 xuống 60 giây theo số màn. Màn nhiều pháo được tối thiểu 9 giây/pháo. Mở cài đặt hoặc đưa ứng dụng xuống nền sẽ dừng đồng hồ. Đạn đang bay vẫn hoàn thành hành trình khi bảng cài đặt mở. Hết giờ có lựa chọn chơi lại hoặc về trang chủ. Hoàn tác không hoàn lại thời gian.

### Sửa lỗi liên quan

- Đi tiếp theo số màn, tránh chọn ngẫu nhiên cả scene menu sau màn cuối. Hoàn thành màn 100 trở về trang chủ.
- Mở khóa màn kế tiếp ngay khi thắng, trước khi bấm nút đi tiếp; lưu ngay xuống PlayerPrefs.
- Không đọc snapshot hoàn tác của màn khác/lần chơi trước. Chỉ giữ số lượt hoàn tác giữa các lần chơi.
- Snapshot giữ tham chiếu đúng khẩu pháo đã bắn, xử lý cả trường hợp nhiều pháo trùng tên trong scene gốc.
- Chặn bắn chồng khi đạn đang bay, đang mở popup hoặc đã kết thúc màn/lượt.
- Kiểm tra thắng sau khi đạn kết thúc, thay cho mốc cố định 1,5 giây có thể kiểm tra quá sớm.
- Đạn di chuyển trong FixedUpdate ở tốc độ ổn định, không bị phụ thuộc FPS như công thức cũ. Giữ ưu tiên hướng của dữ liệu scene cũ khi có nhiều cờ hướng cùng bật.
- Nút chơi lại hoạt động; reset hành trình giữ lại âm thanh, rung và giao diện.
- Nhận thưởng hoàn tác vẫn an toàn nếu đã rời màn trong lúc xem quảng cáo.

### Rung và chuyển động

Android 8 trở lên dùng VibrationEffect, điều chỉnh biên độ và thời lượng; thiết bị không điều khiển được biên độ có thể chỉ phản ánh thay đổi thời lượng. Android cũ dùng thời lượng. Thư viện manifest bổ sung quyền VIBRATE, không cần hộp thoại cấp quyền runtime.

Thư viện `.androidlib` được đặt trong Assets theo [hướng dẫn nhập Android Library của Unity](https://docs.unity.cn/6000.0/Documentation/Manual/android-library-project-import.html).

iOS dùng xung hệ thống qua Handheld.Vibrate nên không điều khiển được biên độ bằng API tích hợp này. Editor/máy tính không phát rung. Cần kiểm tra cảm giác thực tế bằng nút **Thử** trên điện thoại.

Cài đặt hiệu ứng tắt các hiệu ứng tạo mới, dừng particle màu, ẩn vệt đạn và dừng chuyển động trang trí. Gameplay, đạn vật lý và màu mục tiêu vẫn hoạt động. Các hiệu ứng nhún đang chạy kết thúc trong khoảng 0,32 giây. Particle không tạo mảnh vỡ Rigidbody, giảm chi phí vật lý.

## Kiểm tra

Đã chạy kiểm tra asset độc lập với Unity:

```text
node Tools/validate-upgrade.cjs
```

Kiểm tra 101 mục build, 100 cặp bàn chơi/mẫu với 1.032 ô, màu mục tiêu có pháo tương ứng, hướng bắn, danh tính riêng của pháo, GUID scene/meta và Android profile/quyền rung. Script cũng xác nhận hình học các biến thể giữ nguyên nguồn gốc.

Các kiểm thử Unity EditMode nằm tại `Assets/Tests/Editor`: danh sách scene, chặn tiến độ ngoài phạm vi, mức rung, lưu công tắc rung độc lập và giới hạn chỉ số giao diện. Các test dùng reflection để không cần đổi assembly của gameplay/quảng cáo.

**Môi trường thực hiện không có Unity Editor, Unity assemblies, .NET SDK hoặc thiết bị Android. Chưa chạy biên dịch Unity, Unity Test Runner, Play Mode hay build APK.** Kiểm tra asset không xác nhận lời giải bằng mô phỏng vật lý, chất lượng hình ảnh, hiệu năng GPU hay khả năng rung thực tế.

### Danh sách nghiệm thu trong Unity 6000.2.6f2

1. Mở MainMenu, bấm Play. Xem tiếng Việt, nút, vùng an toàn và bố cục trên màn hình điện thoại dọc.
2. Đổi cả bốn giao diện từ menu và trong game. Đóng/mở game để xác nhận lưu lựa chọn.
3. Chỉnh rung ở 0%, 25%, 100%; thử khi tắt rung. Kiểm tra trên Android có mô-tơ rung.
4. Chơi màn 1, 4, 48, 50: kiểm tra các cờ hướng cũ, pháo trùng tên, đường đạn, hoàn tác và điều kiện thắng.
5. Mở trực tiếp màn 51, 75, 76, 100: kiểm tra hai/ba lượt, mẫu/pháo đổi màu cùng nhau, đồng hồ bắt đầu từ phát đầu và dừng khi mở cài đặt.
6. Kiểm tra hết giờ, hết pháo, hết lượt hoàn tác, quảng cáo chưa sẵn sàng và phần thưởng quảng cáo.
7. Thắng màn 100, trở về menu, tiếp tục; reset hành trình và xác nhận các cài đặt được giữ.
8. Tắt hiệu ứng, kiểm tra bàn chơi vẫn hoạt động; đo FPS ở màn nhiều ô trên thiết bị mục tiêu.
9. Chạy Window > General > Test Runner > EditMode, sau đó build Android để xác minh tích hợp manifest và LevelPlay.
