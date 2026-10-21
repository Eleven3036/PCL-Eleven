using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using PCL.Core.App;
using PCL.Core.App.Localization;
using PCL.Core.IO.Net.Http;
using PCL.Core.Logging;

namespace PCL;

public partial class PageLivestreamRight : MyPageRight
{
    private const string ApiUrl = "https://pcl-eleven.furina3036.online/api/live_list.php";
    private const string SiteBase = "https://pcl-eleven.furina3036.online";

    // ============== 数据模型 ==============

    public class RoomItem
    {
        [JsonPropertyName("id")] public string Id { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("platform")] public string Platform { get; set; } = "";
        [JsonPropertyName("platform_label")] public string PlatformLabel { get; set; } = "";
        [JsonPropertyName("platform_proto")] public string PlatformProto { get; set; } = "";
        [JsonPropertyName("room_id")] public string RoomId { get; set; } = "";
        [JsonPropertyName("status")] public string Status { get; set; } = "";
        [JsonPropertyName("status_label")] public string StatusLabel { get; set; } = "";
        [JsonPropertyName("description")] public string Description { get; set; } = "";
        [JsonPropertyName("cover")] public string Cover { get; set; } = "";
        [JsonPropertyName("game_type")] public string GameType { get; set; } = "";
        [JsonPropertyName("live_url")] public string LiveUrl { get; set; } = "";
        [JsonPropertyName("play_url")] public string? PlayUrl { get; set; }
        [JsonPropertyName("supports_inapp")] public bool SupportsInApp { get; set; }
        [JsonPropertyName("sort_order")] public int SortOrder { get; set; }
    }

    public class LiveListResponse
    {
        [JsonPropertyName("ok")] public bool Ok { get; set; }
        [JsonPropertyName("enabled")] public bool Enabled { get; set; }
        [JsonPropertyName("server_time")] public long ServerTime { get; set; }
        [JsonPropertyName("rooms")] public List<RoomItem> Rooms { get; set; } = [];
    }

    // ============== 字段 ==============

    private readonly ObservableCollection<RoomItem> _visibleRooms = [];
    private List<RoomItem> _allRooms = [];
    private string? _currentFilter;
    private bool _webView2Initialized;
    private RoomItem? _currentPlayingRoom;

    // ============== 构造 ==============

    public PageLivestreamRight()
    {
        InitializeComponent();
        ListRooms.ItemsSource = _visibleRooms;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // 每次进入页面都刷新
        _ = ReloadFromApiAsync();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        // 离开页面时暂停播放器
        StopPlayer();
    }

    // ============== 公共接口（供左栏调用） ==============

    /// <summary>从 API 重新拉取直播间列表</summary>
    public async Task ReloadFromApi()
    {
        await ReloadFromApiAsync();
    }

    /// <summary>按平台过滤（null 表示全部）</summary>
    public void ApplyPlatformFilter(string? platform)
    {
        _currentFilter = platform;
        ApplyFilter();
    }

    // ============== API 拉取 ==============

    private async Task ReloadFromApiAsync()
    {
        ShowCard(CardLoading);
        HideCard(CardError);
        HideCard(CardEmpty);

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            using var client = new HttpClient();
            using var resp = await client.GetAsync(ApiUrl, cts.Token);

            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}");

            var content = await resp.Content.ReadAsStringAsync();
            var jsonOpts = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
            };
            var data = JsonSerializer.Deserialize<LiveListResponse>(content, jsonOpts);

            if (data is null || !data.Ok)
                throw new InvalidOperationException(Lang.Text("Livestream.Right.ApiFailed"));

            _allRooms = data.Rooms?.OrderBy(r => r.SortOrder).ToList() ?? [];
            ApplyFilter();
        }
        catch (Exception ex)
        {
            LogWrapper.Error(ex, "直播间列表加载失败");
            ShowError(Lang.Text("Livestream.Right.LoadFailed", ex.Message));
        }
    }

    private void ApplyFilter()
    {
        _visibleRooms.Clear();

        var query = _allRooms.AsEnumerable();
        if (!string.IsNullOrEmpty(_currentFilter))
            query = query.Where(r => string.Equals(r.Platform, _currentFilter, StringComparison.OrdinalIgnoreCase));

        foreach (var room in query)
            _visibleRooms.Add(room);

        HideCard(CardLoading);
        HideCard(CardError);

        if (_visibleRooms.Count == 0)
            ShowCard(CardEmpty);
        else
            HideCard(CardEmpty);

        LabSummary.Text = Lang.Text("Livestream.Right.Summary", _visibleRooms.Count,
            _allRooms.Count(r => string.Equals(r.Status, "online", StringComparison.OrdinalIgnoreCase)));
    }

    private void ShowError(string msg)
    {
        HideCard(CardLoading);
        HideCard(CardEmpty);
        LabError.Text = msg;
        ShowCard(CardError);
    }

    private static void ShowCard(FrameworkElement card) => card.Visibility = Visibility.Visible;
    private static void HideCard(FrameworkElement card) => card.Visibility = Visibility.Collapsed;

    // ============== 按钮事件 ==============

    private void BtnReload_Click(object sender, RoutedEventArgs e)
    {
        _ = ReloadFromApiAsync();
    }

    private void BtnWatch_Click(object sender, RoutedEventArgs e)
    {
        // 从 VisualTree 往上找 DataContext 是 RoomItem 的元素
        var dep = sender as DependencyObject;
        while (dep is not null)
        {
            if (dep is FrameworkElement fe && fe.DataContext is RoomItem r)
            {
                OpenPlayer(r);
                return;
            }
            dep = System.Windows.Media.VisualTreeHelper.GetParent(dep);
        }
    }

    // ============== 播放器 ==============

    private async void OpenPlayer(RoomItem room)
    {
        _currentPlayingRoom = room;
        LabPlayerTitle.Text = $"{room.Name} · {room.PlatformLabel}";
        PanList.Visibility = Visibility.Collapsed;
        PanPlayer.Visibility = Visibility.Visible;
        PanPlayerLoading.Visibility = Visibility.Visible;

        try
        {
            await EnsureWebView2CoreAsync();

            // 加载官网直播间页面（它包含完整的 flv.js / hls.js 播放逻辑）
            var roomPageUrl = $"{SiteBase}/room.html?id={room.RoomId}&platform={room.Platform}";
            WebView.Source = new Uri(roomPageUrl);

            WebView.NavigationCompleted -= OnWebViewNavigationCompleted;
            WebView.NavigationCompleted += OnWebViewNavigationCompleted;
        }
        catch (Exception ex)
        {
            LogWrapper.Error(ex, "WebView2 初始化失败");
            HintService.Hint(Lang.Text("Livestream.Right.WebView2InitFailed", ex.Message), HintType.Error);
            PanPlayer.Visibility = Visibility.Collapsed;
            PanList.Visibility = Visibility.Visible;
        }
    }

    private void OnWebViewNavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            PanPlayerLoading.Visibility = Visibility.Collapsed;
        });
    }

    private async Task EnsureWebView2CoreAsync()
    {
        if (_webView2Initialized) return;

        await WebView.EnsureCoreWebView2Async();
        WebView.CoreWebView2.Settings.AreDevToolsEnabled = false;
        WebView.CoreWebView2.Settings.IsStatusBarEnabled = false;
        _webView2Initialized = true;
    }

    private void BtnPlayerBack_Click(object sender, RoutedEventArgs e)
    {
        StopPlayer();
        PanPlayer.Visibility = Visibility.Collapsed;
        PanList.Visibility = Visibility.Visible;
    }

    private void BtnPlayerOpenExternal_Click(object sender, RoutedEventArgs e)
    {
        if (_currentPlayingRoom is { LiveUrl: var url } && !string.IsNullOrEmpty(url))
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                HintService.Hint(Lang.Text("Livestream.Right.OpenExternalFailed", ex.Message), HintType.Error);
            }
        }
    }

    private void StopPlayer()
    {
        try
        {
            if (_webView2Initialized)
            {
                WebView.Stop();
                WebView.Source = null;
            }
        }
        catch
        {
            // 忽略
        }
        PanPlayerLoading.Visibility = Visibility.Visible;
    }
}
