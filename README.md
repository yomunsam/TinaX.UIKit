# TinaX Framework - UIKit.

<a href="https://tinax.corala.space" target="_blank"><img src="https://github.com/yomunsam/TinaX.Core/raw/master/readme_res/logo.png" width = "420" height = "187" alt="logo" align=center /></a>

[![LICENSE](https://img.shields.io/badge/license-NPL%20(The%20996%20Prohibited%20License)-blue.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE)
<a href="https://996.icu"><img src="https://img.shields.io/badge/link-996.icu-red.svg" alt="996.icu"></a>
[![LICENSE](https://camo.githubusercontent.com/3867ce531c10be1c59fae9642d8feca417d39b58/68747470733a2f2f696d672e736869656c64732e696f2f6769746875622f6c6963656e73652f636f6f6b6965592f596561726e696e672e737667)](https://github.com/yomunsam/TinaX/blob/master/LICENSE)

基于UGUI的UI框架，包含MVC和MVVM实现

UGUI-based UI framework, including MVC and MVVM implementation.

<br>

## Install this package

### Install via git UPM:

您可以使用Unity Package Manager来安装使用该包。

You can use the Unity Package Manager to install and use this package.  

```
git://github.com/yomunsam/TinaX.XComponent.git
```

package name: `io.nekonya.tinax.xcomponent`

If you want to set a target version, you can use release tag like `#6.6.0-preview`. for detail you can see this page: [https://github.com/yomunsam/TinaX.UIKit/releases](https://github.com/yomunsam/TinaX.UIKit/releases)

<br>

### Install via npm (UPM)

Modify `Packages/manifest.json` file in your project, and add the following code before "dependencies" node of this file:

``` json
"scopedRegistries": [
    {
        "name": "TinaX",
        "url": "https://registry.npmjs.org",
        "scopes": [
            "io.nekonya"
        ]
    }
],
```

<br>

<br>
------

## Dependencies

This package does not depend on other packages.

在安装之前，请先确保已安装如下依赖：

Before setup `TinaX.UIKit`, please ensure the following dependencies are installed by `Unity Package Manager`:

- **[io.nekonya.tinax.core](https://github.com/yomunsam/tinax.core)** :`git://github.com/yomunsam/TinaX.Core.git`

- **[io.nekonya.tinax.xcomponent](https://github.com/yomunsam/tinax.XComponent)** :`git://github.com/yomunsam/TinaX.XComponent.git`

- **[com.neuecc.unirx](https://github.com/yomunsam/UniRx.UPM)** :`git://github.com/yomunsam/UniRx.UPM.git`

------

## Third-Party

本项目中使用了以下优秀的第三方库：

The following excellent third-party libraries are used in this project:

- **[TweenRx](https://github.com/fumobox/TweenRx)** : Reactive animation utility for Unity.

