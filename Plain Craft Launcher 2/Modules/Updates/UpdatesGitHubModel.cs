using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using PCL.Core.App;
using PCL.Core.App.Localization;
using PCL.Core.Utils;
using PCL.Network;
using PCL.Network.Loaders;
using PCL.Core.IO.Net.Http;

namespace PCL;

/// <summary>
///     GitHub Releases 更新源 — 从 GitHub 仓库的 Releases API 获取最新版本和下载链接。
/// </summary>
public class UpdatesGitHubModel : IUpdateSource
{
    private readonly string _owner;
    private readonly string _repo;
    private GitHubReleaseModel? _latestRelease;

    public UpdatesGitHubModel(string owner, string repo, string name = "GitHub")
    {
        _owner = owner;
        _repo = repo;
        SourceName = name;
    }

    public string SourceName { get; set; }

    public bool IsAvailable() => !string.IsNullOrWhiteSpace(_owner) && !string.IsNullOrWhiteSpace(_repo);

    public bool RefreshCache()
    {
        try
        {
            var url = $"https://api.github.com/repos/{_owner}/{_repo}/releases/latest";
            using var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)  // 15s 超时，防止 GitHub API 卡住阻塞主线程
            };
            client.DefaultRequestHeaders.Add("User-Agent", "PCL-Eleven-Launcher");
            // 关键：ConfigureAwait(false) 防止在 UI 线程上调用 .GetResult() 造成死锁。
            // 这个方法会从 PageSetupUpdate.CheckUpdate (UI 线程 await) 链路调用，
            // 若没有 ConfigureAwait(false)，async 调用完成后会尝试回到 UI 线程，
            // 但 UI 线程此时正卡在 .GetResult() 上 —— 经典死锁。
            var json = client.GetStringAsync(url).ConfigureAwait(false).GetAwaiter().GetResult();
            _latestRelease = JsonSerializer.Deserialize<GitHubReleaseModel>(json, JsonCompat.SerializerOptions);
            return _latestRelease is not null;
        }
        catch (Exception ex)
        {
            ModBase.Log(ex, "[Update] GitHub Releases 获取失败");
            return false;
        }
    }

    public VersionDataModel GetLatestVersion(UpdateChannel channel, UpdateArch arch)
    {
        if (_latestRelease is null) RefreshCache();
        if (_latestRelease is null)
            throw new Exception("无法从 GitHub 获取最新版本信息");

        var assetNames = _latestRelease.Assets?.Select(a => a.Name).ToList() ?? new List<string>();
        var versionName = ExtractVersion(new[]
        {
            _latestRelease.TagName,
            _latestRelease.Name ?? "",
            string.Join(" ", assetNames)
        });

        return new VersionDataModel
        {
            VersionName = versionName,
            VersionCode = ExtractVersionCode(versionName),
            Source = SourceName,
            Changelog = _latestRelease.Body ?? "",
            Sha256 = ""
        };
    }

    public bool IsLatest(UpdateChannel channel, UpdateArch arch, SemVer currentVersion, int currentVersionCode)
    {
        if (_latestRelease is null) RefreshCache();
        if (_latestRelease is null) return true;

        try
        {
            var latest = GetLatestVersion(channel, arch);
            return SemVer.Parse(latest.VersionName) <= currentVersion;
        }
        catch
        {
            return true;
        }
    }

    public VersionAnnouncementDataModel GetAnnouncementList() => new();

    public List<ModLoader.LoaderBase> GetDownloadLoader(UpdateChannel channel, UpdateArch arch, string output)
    {
        if (_latestRelease is null) RefreshCache();
        if (_latestRelease is null)
            throw new Exception("无法从 GitHub 获取下载链接");

        var asset = _latestRelease.Assets?
            .FirstOrDefault(a => a.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase));

        if (asset is null)
            throw new Exception("GitHub Release 中未找到 .exe 下载资源");

        ModBase.Log($"[Update] GitHub 下载链接: {asset.BrowserDownloadUrl}");

        return new List<ModLoader.LoaderBase>
        {
            new LoaderDownload(Lang.Text("Update.Task.DownloadFile"),
                new List<DownloadFile> { new(new[] { asset.BrowserDownloadUrl }, output) })
        };
    }

    private static string ExtractVersion(string[] sources)
    {
        foreach (var source in sources.Where(s => !string.IsNullOrWhiteSpace(s)))
        {
            var match = System.Text.RegularExpressions.Regex.Match(source, @"(\d+\.\d+\.\d+)");
            if (match.Success) return match.Groups[1].Value;
        }
        return "0.0.0";
    }

    private static int ExtractVersionCode(string versionName)
    {
        var parts = versionName.Split('.');
        if (parts.Length >= 3 &&
            int.TryParse(parts[0], out var major) &&
            int.TryParse(parts[1], out var minor) &&
            int.TryParse(parts[2], out var patch))
        {
            return major * 10000 + minor * 100 + patch;
        }
        return 0;
    }

    private class GitHubReleaseModel
    {
        [JsonPropertyName("tag_name")] public string TagName { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("body")] public string? Body { get; set; }
        [JsonPropertyName("assets")] public List<GitHubAsset>? Assets { get; set; }
        [JsonPropertyName("draft")] public bool Draft { get; set; }
        [JsonPropertyName("prerelease")] public bool Prerelease { get; set; }
    }

    private class GitHubAsset
    {
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("browser_download_url")] public string BrowserDownloadUrl { get; set; } = "";
    }
}
