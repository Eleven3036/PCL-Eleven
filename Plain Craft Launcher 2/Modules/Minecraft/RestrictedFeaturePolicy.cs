using PCL.Core.App;
using PCL.Core.App.Region;
using PCL.Core.App.Localization;
using PCL.Core.UI;

namespace PCL;

internal static class RestrictedFeaturePolicy
{
    /// <summary>
    /// 地区限制总开关（配置项 RegionRestrictionEnabled）。
    /// 默认关闭：所有地区均可正常下载、补全、安装整合包。
    /// 设为 true 时恢复 PCL 原版逻辑（中国大陆/日本等地区禁用）。
    /// </summary>
    internal static bool RestrictionEnabled => Config.Preference.RegionRestrictionEnabled;

    internal static RegionAssessment Assess() => RegionPolicy.Evaluate(RegionPolicy.ReadWindowsSignals());

    internal static bool IsRestricted => RestrictionEnabled && Assess().IsRestricted;

    internal static bool Check(string feature, bool showHint = true)
    {
        if (!RestrictionEnabled) return true;

        var assessment = Assess();
        if (!assessment.IsRestricted) return true;

        ModBase.Log($"[RegionPolicy] 已阻止功能 {feature}，判定原因：{assessment.Reason}");
        if (showHint)
        {
            HintService.Hint(Lang.Text(assessment.Classification == RegionClassification.Unknown
                ? "Region.Feature.Unknown"
                : "Region.Feature.Restricted"), HintType.Error);
        }
        return false;
    }

    internal static void ThrowIfRestricted(string feature)
    {
        if (Check(feature, false)) return;
        throw new ModBase.CancelledException();
    }
}
