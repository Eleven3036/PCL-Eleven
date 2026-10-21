# PCL-Eleven

**简体中文** | [English](README-EN.md) | [繁體中文](README-ZH_TW.md)

<div align="center">

<img src="Plain Craft Launcher 2/Images/icon.ico" alt="Logo" width="80" height="80">

# PCL-Eleven

[![Stars](https://img.shields.io/github/stars/Eleven3036/PCL-Eleven?style=for-the-badge&logo=github&label=stars)](https://github.com/Eleven3036/PCL-Eleven)
![GitHub Release](https://img.shields.io/github/v/release/Eleven3036/PCL-Eleven?label=release&logo=github&style=for-the-badge)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/Eleven3036/PCL-Eleven/build-test.yml?style=for-the-badge)

[![Issues](https://img.shields.io/github/issues/Eleven3036/PCL-Eleven?style=for-the-badge&label=issues&labelColor=444444&color=1F883D&logo=github)](https://github.com/Eleven3036/PCL-Eleven/issues)
[![Pull requests](https://img.shields.io/github/issues-pr/Eleven3036/PCL-Eleven?style=for-the-badge&label=pull%20requests&labelColor=444444&color=1F883D&logo=github)](https://github.com/Eleven3036/PCL-Eleven/pulls)

[下载最新版本](https://github.com/Eleven3036/PCL-Eleven/releases/latest) |
[提交问题](https://github.com/Eleven3036/PCL-Eleven/issues/new/choose)

</div>

PCL-Eleven 是基于 [PCL（Plain Craft Launcher）](https://github.com/Meloong-Git/PCL) 开源代码由 **Eleven** 二次开发的 Minecraft Java 版启动器独立分支。

- 目标：**.NET 10.0 / C# 14 / WPF**
- 版本：**v11.1.0**（首个独立品牌版本）
- 协议：**GPL-3.0**

> PCL-Eleven 的版本号与上游主线并非严格对应，请不要向上游仓库反馈本分支的问题。

> 原PCL作者：龙腾猫跃，社区版团队：PCLCommunity，由不可逆原因停止mc本体下载
>
> PCL-Eleven作者：Eleven二次开发创建PCL-Eleven恢复mc本体下载功能
>
> 如该启动器有问题请反馈给Eleven，请不要反馈给龙腾猫跃以及PCL Community。感谢配合

---

## 💻 支持平台

| 操作系统 | 支持情况 | 环境要求 |
|---|---|---|
| Windows 10 1809 (17763) 或更高 | ✅ 完整支持 | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 8 - Windows 10 1809 (17763) | ⚠️ 理论能跑，酌情提供社区支持 | [.NET 10 Desktop Runtime](https://get.dot.net/10) |
| Windows 7 或更低版本 | ❌ 不支持 | / |
| macOS / Linux / 其他操作系统 | ⚠️ 仅跨平台开发支持（交叉编译） | [.NET 10 SDK](https://get.dot.net/10) |

**✅ 完整支持**：尽可能提供一切相关支持，但必须确保启动器为最新版本。

**⚠️ 理论能跑，酌情提供社区支持**：PCL-Eleven 应该可以在这些平台上运行，但不保证功能完全可用。你可能需要升级到完整支持的系统版本以获得进一步社区技术支持。

**❌ 不支持**：PCL-Eleven 在这些平台的可用性较低，甚至压根打不开。请升级操作系统以使用 PCL-Eleven。

**⚠️ 仅跨平台开发支持（交叉编译）**：PCL-Eleven 的源代码可以在 macOS 与 Linux 平台编译，但无法直接运行。作为开发者，你可以在这些平台上进行开发，然后将编译产物转移到 Windows 系统测试。

**注**：
社区仅对最新版本的启动器提供支持。
取决于部分问题的特殊性（如系统不完整），有时你仍然必须升级操作系统以继续获得支持。
PCL-Eleven 始终建议使用最新版本的操作系统以获得最佳体验。
你仍然可以尝试在不受支持的系统上使用 PCL-Eleven，但可能会遇到很多额外问题。

## 🏗️ 构建

```bash
# 克隆仓库
git clone https://github.com/Eleven3036/PCL-Eleven.git
cd PCL-Eleven

# 构建（需要 .NET 10 SDK）
dotnet build "Plain Craft Launcher 2/Plain Craft Launcher 2.csproj" -c Release

# 发布单文件（win-x64 自包含）
dotnet publish "Plain Craft Launcher 2/Plain Craft Launcher 2.csproj" -c Release -r win-x64 --self-contained true
```

## 🔒 许可证

本项目采用 [GPL-3.0](LICENSE) 协议开源。

## ❤️ 贡献者

[![](https://contrib.rocks/image?repo=Eleven3036/PCL-Eleven)](https://github.com/Eleven3036/PCL-Eleven/graphs/contributors)
