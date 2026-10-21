**简体中文** | [English](README-EN.md) | [繁體中文](README-ZH_TW.md)

<div align="center">

<img src="Plain Craft Launcher 2/Images/icon.ico" alt="Logo" width="80" height="80">

# PCL-Eleven

[![Stars](https://img.shields.io/github/stars/Eleven3036/PCL-Eleven?style=for-the-badge\&logo=data:image/svg%2bxml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZlcnNpb249IjEiIHdpZHRoPSIxNiIgaGVpZ2h0PSIxNiI+PHBhdGggZD0iTTggLjI1YS43NS43NSAwIDAgMSAuNjczLjQxOGwxLjg4MiAzLjgxNSA0LjIxLjYxMmEuNzUuNzUgMCAwIDEgLjQxNiAxLjI3OWwtMy4wNDYgMi45Ny43MTkgNC4xOTJhLjc1MS43NTEgMCAwIDEtMS4wODguNzkxTDggMTIuMzQ3bC0zLjc2NiAxLjk4YS43NS43NSAwIDAgMS0xLjA4OC0uNzlsLjcyLTQuMTk0TC44MTggNi4zNzRhLjc1Ljc1IDAgMCAxIC40MTYtMS4yOGw0LjIxLS42MTFMNy4zMjcuNjY4QS43NS43NSAwIDAgMSA4IC4yNVoiIGZpbGw9IiNlYWM1NGYiLz48L3N2Zz4=\&logoSize=auto\&label=stars\&labelColor=444444\&color=eac54f)](https://github.com/Eleven3036/PCL-Eleven/)
![GitHub Release](https://img.shields.io/github/v/release/Eleven3036/PCL-Eleven?label=release\&logo=github\&style=for-the-badge)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/Eleven3036/PCL-Eleven/build-test.yml?style=for-the-badge)

[![Issues](https://img.shields.io/github/issues/Eleven3036/PCL-Eleven?style=for-the-badge\&label=issues\&labelColor=444444\&color=1F883D\&logo=github)](https://github.com/Eleven3036/PCL-Eleven/issues)
[![Pull requests](https://img.shields.io/github/issues-pr/Eleven3036/PCL-Eleven?style=for-the-badge\&label=pull%20requests\&labelColor=444444\&color=1F883D\&logo=github)](https://github.com/Eleven3036/PCL-Eleven/pulls)
![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/Eleven3036/PCL-Eleven/total?style=for-the-badge)
[![License](https://img.shields.io/badge/license-GPL--3.0-green?style=for-the-badge\&labelColor=444444)](LICENSE)

[下载](https://github.com/Eleven3036/PCL-Eleven/releases/latest) |
[官网](https://pcl-eleven.furina3036.online/)

[提交问题](https://github.com/Eleven3036/PCL-Eleven/issues/new/choose)

</div>

> ⚠️ **责任声明**
>
> 原 PCL（Plain Craft Launcher）作者：龙腾猫跃    PCL Community Edition 社区团队：PCL-Community
>
>                                   （因不可逆原因停止 Minecraft 本体下载功能）
>
> PCL-Eleven 二次开发作者：Eleven
> （基于 PCL-Community/PCL-CE 源码二次开发，恢复了 Minecraft 本体下载
> 功能，作为独立项目发布）
>
> 如您在使用 PCL-Eleven 过程中遇到任何问题，请反馈给 Eleven：
> <https://github.com/Eleven3036/PCL-Eleven/issues>
>
> 请不要向龙腾猫跃或 PCL-Community 反馈 PCL-Eleven 的问题。感谢配合。

***

## 项目简介

**PCL-Eleven** 是基于 PCL（Plain Craft Launcher）开源代码二次开发的独立项目，由 **Eleven** 制作，GPL-3.0 协议。

原 PCL以及PCL-Community 社区版因不可逆原因停止了 Minecraft 本体下载功能，PCL-Eleven 恢复了该功能，并持续进行独立迭代开发。

## 技术栈

| 项目    | 详情                             |
| ----- | ------------------------------ |
| 目标框架  | .NET 10.0 (net10.0-windows)    |
| 语言版本  | C# 14                          |
| UI 框架 | WPF                            |
| 发布方式  | 单文件自包含发布 (win-x64 / win-arm64) |
| 协议    | GPL-3.0                        |

## 解决方案结构

```
PCL-Eleven/
├── Plain Craft Launcher 2/   ← 主启动器（WPF，WinExe）
├── PCL.Core/                  ← 核心库（IO / Minecraft / UI / App 框架）
├── PCL.Core.SourceGenerators/ ← 源码生成器
├── PCL.Core.Test/             ← 单元测试
└── PCL-Eleven.sln
```

## 开发注意事项

- 所有 API 密钥（MS\_CLIENT\_ID、CURSEFORGE\_API\_KEY 等）**不会**硬编码在源码里，请通过环境变量或 GitHub Actions Secrets 配置
- `Secrets.cs` 里所有密钥均为 `.ReplaceNullOrEmpty()`（无默认值），未配置时为空字符串

## License

本项目基于 **GNU General Public License v3.0** 开源，详见 [LICENSE](LICENSE) 文件。

PCL 开源基础工程基于 PCL-Community/PCL-CE 源码二次开发，保留原始版权声明。
