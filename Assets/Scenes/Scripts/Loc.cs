using System.Collections.Generic;
using UnityEngine;

// Minimal localisation. Game text is authored in Vietnamese (the source language) and used as the lookup key;
// English is looked up in a table. Unknown strings fall back to the text itself, so adding a language is just a new table.
public static class Loc
{
    public const int Vietnamese = 0, English = 1;
    public static readonly string[] LanguageNames = { "Tiếng Việt", "English" };

    // Text with {0}-style placeholders is stored as a format string and filled in with Loc.F.
    static readonly Dictionary<string, string> en = new Dictionary<string, string>
    {
        // Styles and wallpapers
        { "Tech tối", "Dark Tech" },
        { "Neon sắc màu", "Colorful neon" }, { "Công nghệ neon", "Neon tech" }, { "Ngọt ngào như kẹo", "Sweet as candy" },
        { "Tinh thể lấp lánh", "Sparkling crystal" }, { "Đáng yêu như hoạt hình", "Cute as a cartoon" },
        { "Khối 3D hiện đại", "Modern 3D blocks" }, { "Xanh mát thiên nhiên", "Fresh green nature" },
        { "Đảo mây", "Cloud Island" }, { "Thung lũng kẹo", "Candy Valley" }, { "Hoàng hôn đào", "Peach Sunset" },
        { "Mây đêm", "Night Clouds" }, { "Biển kem", "Cream Sea" }, { "Vườn dâu", "Strawberry Garden" },
        { "Rừng oải hương", "Lavender Forest" }, { "Khinh khí cầu chanh", "Lemon Balloon" },
        { "Đêm trăng kẹo", "Candy Moonlight" }, { "San hô ngọc trai", "Pearl Coral" },
        // Quests
        { "Thắng 3 màn", "Win 3 levels" }, { "Đạt 3 sao ở 1 màn", "Get 3 stars in 1 level" }, { "Bắn 30 phát", "Fire 30 shots" },
        // Main menu
        { "Ghép màu thông minh", "Smart color matching" },
        { "100 màn chơi · {0} · {1}", "100 levels · {0} · {1}" },
        { "100 màn chơi · Thế giới kẹo sắc màu", "100 levels · A world of candy colors" },
        { "Chạm nhẹ. Bật sắc màu!", "Tap lightly. Pop the colors!" },
        { "Chơi Ngay\nMàn {0}", "Play Now\nLevel {0}" },
        { "Nhiệm vụ", "Quests" }, { "Quà ngày", "Daily Gift" }, { "Cửa hàng", "Shop" }, { "Chọn màn", "Levels" },
        { "Sao đã đạt: {0} / {1}", "Stars earned: {0} / {1}" },
        { "Âm thanh: {0}", "Sound: {0}" }, { "Âm thanh", "Sound" }, { "Bật", "On" }, { "Tắt", "Off" },
        { "Cách chơi", "How to play" },
        // In-level HUD
        { "Màn {0}", "Level {0}" }, { "Mẫu cần phối", "Target pattern" },
        { "Hoàn tác", "Undo" }, { "Hoàn tác · {0}", "Undo · {0}" }, { "Chơi lại", "Restart" },
        { "{0} / {1} ô đúng màu   ·   Lượt {2}/{3}", "{0} / {1} cells matched   ·   Round {2}/{3}" },
        { "{0} giây", "{0} s" }, { "Thư giãn", "Relaxed" },
        { "Lượt {0} · Đổi sắc màu!", "Round {0} · Colors change!" },
        { "Quảng cáo chưa sẵn sàng, thử lại sau nhé.", "The ad isn't ready, please try again later." },
        // Result dialogs
        { "Chưa đúng màu", "Not quite right" },
        { "Đã bắn hết súng nhưng chưa khớp mẫu.\nHãy hoàn tác hoặc chơi lại để đổi thứ tự bắn.",
          "All cannons fired but the pattern doesn't match.\nUndo or restart to change the shooting order." },
        { "Thử một nhịp mới", "Try a new rhythm" },
        { "Hết thời gian lượt này.\nMột thứ tự bắn khác đang chờ bạn khám phá.",
          "Time's up for this round.\nAnother shooting order is waiting for you." },
        { "Chơi lại màn này", "Replay this level" }, { "Về trang chủ", "Back to home" },
        { "THẮNG!", "VICTORY!" }, { "Hoàn thành!", "Complete!" },
        { "Bạn vừa làm thế giới thêm một chút màu.", "You just added a little color to the world." },
        { "MÀN {0}   ·   +{1} xu", "LEVEL {0}   ·   +{1} coins" },
        { "Trọn vẹn 100 màn · Về nhà", "All 100 levels done · Go home" }, { "Màn tiếp theo  →", "Next level  →" },
        // Settings
        { "Không gian của bạn", "Your space" }, { "Chỉnh một chút, chơi thật thoải mái.", "Tweak a little, play comfortably." },
        { "Đóng cài đặt", "Close settings" },
        { "Rung toàn bộ game · {0}", "Game vibration · {0}" }, { "Mức rung · {0}%", "Vibration level · {0}%" }, { "Thử", "Test" },
        { "Đã tắt rung cho mọi thao tác và màn chơi.", "Vibration is off for all actions and levels." },
        { "Mức rung đang ở 0%. Tăng mức để thử rung.", "Vibration level is 0%. Raise it to try." },
        { "Tự lưu cho toàn bộ game · Mức rung tùy thiết bị.", "Saved for the whole game · Strength depends on the device." },
        { "Tự lưu cho toàn bộ game · Thử rung trên điện thoại.", "Saved for the whole game · Try vibration on a phone." },
        { "Phong cách {0} dùng bộ màu riêng,\nkhông có bộ sưu tập hình nền.", "The {0} style has its own palette,\nso there is no wallpaper collection." },
        { "BỘ SƯU TẬP HÌNH NỀN", "WALLPAPER COLLECTION" },
        { "← Trước", "← Prev" }, { "Sau →", "Next →" },
        { "{0} / {1} · {2} lựa chọn", "{0} / {1} · {2} options" },
        { "Phong cách giao diện  ·  {0}  ›", "UI style  ·  {0}  ›" },
        { "Sẽ áp dụng khi chơi lại hoặc sang màn mới.", "Applies when you replay or start a new level." },
        { "Hiệu ứng chuyển động", "Motion effects" }, { "Đặt lại hành trình…", "Reset journey…" }, { "Xong", "Done" },
        { "Đóng", "Close" },
        // Shop
        { "Cửa hàng giao diện", "Style shop" },
        { "Mở khóa bằng xu kiếm từ màn chơi,\nnhiệm vụ và quà mỗi ngày.", "Unlock with coins earned from levels,\nquests and daily gifts." },
        { "Xu của bạn: {0}", "Your coins: {0}" },
        { "Trong màn chơi: áp dụng khi chơi lại hoặc sang màn mới.", "In a level: applies when you replay or start a new level." },
        { "Đổi xong, trang chủ sẽ tải lại ngay.", "The home screen reloads right after you switch." },
        { "{0} xu", "{0} coins" },
        { "Cần thêm {0} xu. Thắng màn hoặc làm nhiệm vụ nhé!", "You need {0} more coins. Win levels or do quests!" },
        // Level select, quests, daily gift
        { "Màn {0} – {1}   ·   {2} sao", "Levels {0} – {1}   ·   {2} stars" },
        { "Nhiệm vụ hôm nay", "Today's quests" }, { "Làm mới mỗi ngày.\nHoàn thành để nhận xu.", "Refreshes daily.\nComplete them to earn coins." },
        { "{0} / {1}   ·   +{2} xu", "{0} / {1}   ·   +{2} coins" },
        { "Đã nhận", "Claimed" }, { "Nhận", "Claim" }, { "Chưa xong", "Not done" },
        { "Quà mỗi ngày", "Daily gift" },
        { "Điểm danh liên tiếp 7 ngày\nđể nhận quà lớn hơn.", "Check in 7 days in a row\nfor bigger gifts." },
        { "Ngày {0}", "Day {0}" }, { "Nhận +{0} xu", "Claim +{0} coins" }, { "Hẹn gặp lại ngày mai", "See you tomorrow" },
        // Reset and help
        { "Bắt đầu lại?", "Start over?" },
        { "Tiến độ và lượt hoàn tác sẽ về ban đầu.\nCài đặt âm thanh, rung và giao diện vẫn được giữ.",
          "Progress and undos will be reset.\nSound, vibration and style settings are kept." },
        { "Giữ hành trình", "Keep my journey" }, { "Đặt lại và về trang chủ", "Reset and go home" },
        { "Chơi cùng sắc màu", "Play with colors" },
        { "Quan sát mẫu, chạm từng khẩu pháo\nđể tô các ô thành đúng màu.", "Study the pattern, tap each cannon\nto paint the cells in the right color." },
        { "Đóng hướng dẫn", "Close guide" },
        { "Thứ tự bắn tạo nên lời giải.\nHoàn tác khi cần thử một hướng khác.\nBắn thừa ít → nhiều sao → nhiều xu.\nXu mở giao diện mới trong Cửa hàng.\n\nMàn 51–75: hai lượt; 76–100: ba lượt.\nMàu đổi mỗi lượt, thời gian giảm dần.\nĐồng hồ chạy từ phát bắn đầu mỗi lượt.\nMở cài đặt để tạm dừng đồng hồ.",
          "The shooting order is the solution.\nUndo when you want to try another way.\nFewer spare shots → more stars → more coins.\nCoins unlock new styles in the Shop.\n\nLevels 51–75: two rounds; 76–100: three rounds.\nColors change every round and time shrinks.\nThe clock starts at the first shot of each round.\nOpen settings to pause the clock." },
        { "Đã hiểu", "Got it" },
    };

    public static int Language => GamePreferences.Language;

    // Translate a Vietnamese source string into the current language.
    public static string T(string vi)
    {
        if (string.IsNullOrEmpty(vi) || Language == Vietnamese) return vi;
        return en.TryGetValue(vi, out var text) ? text : vi;
    }

    // Translate a format string, then fill in its placeholders.
    public static string F(string vi, params object[] args) { return string.Format(T(vi), args); }

    public static int DefaultLanguage() { return Application.systemLanguage == SystemLanguage.Vietnamese ? Vietnamese : English; }
}
