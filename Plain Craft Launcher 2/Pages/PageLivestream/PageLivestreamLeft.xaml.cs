using System.Windows;
using System.Windows.Controls;

namespace PCL;

public partial class PageLivestreamLeft : MyPageLeft
{
    /// <summary>当前过滤平台：null 表示全部，其他为平台名</summary>
    public string? CurrentPlatformFilter { get; private set; }

    public PageLivestreamLeft()
    {
        InitializeComponent();
        AnimatedControl = PanItem;
    }

    /// <summary>
    /// 左栏点击事件（平台过滤）
    /// </summary>
    private void PageCheck(object senderRaw, ModBase.RouteEventArgs e)
    {
        if (senderRaw is not MyListItem sender || sender.Tag is null)
            return;

        var tag = sender.Tag.ToString();
        CurrentPlatformFilter = tag == "0" ? null : tag;

        // 通知右栏刷新过滤
        ModMain.frmLivestreamRight?.ApplyPlatformFilter(CurrentPlatformFilter);
    }

    /// <summary>
    /// 刷新按钮：通知右栏重新从 API 拉取
    /// </summary>
    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        ModMain.frmLivestreamRight?.ReloadFromApi();
    }
}
