# Jelly Pop — nguồn mỹ thuật

Đợt mở rộng bổ sung sáu ảnh nền gốc **1024 × 1536** ở `Assets/Resources/JellyPop/Backgrounds/`: `icecream-coast`, `strawberry-garden`, `lavender-forest`, `lemon-balloons`, `moonlit-candy`, `pearl-reef`. Các ảnh được tạo riêng từng nền bằng image_gen tích hợp, sao chép nguyên bản vào dự án, không cắt từ atlas hoặc phóng lớn. Prompt từng ảnh nằm trong `background-expansion-prompts.json`. Tổng cộng hiện có chín ảnh riêng và mười lựa chọn giao diện (Mây đêm là biến thể tối của Đảo mây). Lệnh cắt master bên dưới chỉ tái tạo bộ ba nền và UI ban đầu, giữ nguyên sáu ảnh mới.

`approved-direction.png` là concept B đã được chọn, không phải ảnh chụp game. `ui-master.png` và `backgrounds-master.png` là ảnh được tạo mới bằng công cụ **image_gen tích hợp**. Không dùng CLI/API trả phí riêng.

Ảnh dùng trong Unity đã được cắt riêng ở `Assets/Resources/JellyPop/`. Việc cắt giữ nguyên pixel và alpha, không vẽ lại hoặc phóng lớn ảnh. Các nút dùng 9-slice tại runtime, chữ được Unity render riêng để hỗ trợ tiếng Việt. Các icon dùng nguyên tỉ lệ. Khối/pháo gameplay dùng hình học và shader 3D, không dùng ảnh chụp giả 3D.

Tái tạo bản cắt từ hai master trong thư mục này bằng PowerShell:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tools/extract-jelly-assets.ps1
node Tools/prepare-jelly-meta.cjs
node Tools/validate-jelly-pop.cjs
```

ExecutionPolicy chỉ áp dụng cho tiến trình thực hiện lệnh này. Script metadata chỉ bổ sung các `.meta` còn thiếu, không đổi GUID hiện có.

Các PNG nền là 512 × 1024; icon khoảng 300 px. Tài nguyên có alpha thật, đã kiểm tra qua dữ liệu pixel PNG. Cần kiểm tra viền ảnh và độ nét ở kích thước hiển thị thực trên điện thoại. Không tuyên bố ảnh là 4K.

## Prompt tạo master nền

Use case: stylized-concept. Production background texture for portrait mobile color puzzle game, Jelly Pop art direction: premium soft 3D pastel candy world, lavender clouds and peach sky, rounded floating islands, distant tiny rounded candy hills and subtle waterfalls. Create a landscape image split into EXACTLY THREE EQUAL-WIDTH vertical edge-to-edge portrait backgrounds, no margins, no separators, no text, no UI, no blocks or cannons. Left third: blue sky and lavender cloud islands. Middle third: mint and pink candy valley. Right third: peach sunset cloud islands. Each of the three panels is an independently composed portrait game background. Essential constraint: middle 65% of each panel almost empty, smooth pale pastel atmosphere, scenery only at extreme edges and bottom 15%; gameplay colored objects will overlay the open center. Muted contrasts, delicate soft light, polished toy-rendered aesthetic, no detailed castles or characters, no stars floating in center. Seamless clean full bleed illustrations. This will be sliced into three portrait textures.

## Prompt tạo master UI

Use case: ui-mockup. Production game UI atlas on a truly transparent alpha background. Jelly Pop premium 3D toy aesthetic: glossy lavender/purple resin, soft sculpted white symbols, crisp rounded forms, soft shadows contained inside each cell. EXACT 4 by 4 evenly sized square grid of isolated assets. Every asset centered in its cell with generous 18% transparent padding and no contact with another cell. No text or labels anywhere. Row 1 left to right: rounded square purple button with white HOME symbol; rounded square purple button with white RESTART circular arrow; rounded square purple button with white SETTINGS gear; rounded square purple button with white INFO symbol. Row 2: rounded square purple button with white SPEAKER ON symbol; rounded square purple button with white SPEAKER MUTED symbol; rounded square purple button with white CLOSE X symbol; rounded square purple button with white UNDO curved left arrow. Row 3: blank horizontal pill-shaped purple glossy button (width ~80% of cell, height ~40%); blank horizontal pill-shaped pale lavender glossy button same dimensions; blank WHITE pearl rounded square panel with subtly lavender edge and softly beveled rim; gold soft glossy five-point star. Row 4: blank WHITE glossy jelly cube front view, rounded square face with bevels, white highlight upper left, no symbol, meant to tint to arbitrary game colors; soft WHITE four-point sparkle; WHITE soft circular glow sprite fading to alpha at edges; thin WHITE circular impact ring. All 16 cells strictly aligned on equal 4x4 grid. Straight-on orthographic view for UI and cube, no perspective tilt. No checkerboard painted into image. Consistent bright soft studio lighting. Icons legible at small sizes. Transparent outside actual individual assets.
