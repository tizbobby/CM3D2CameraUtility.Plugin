# CM3D2.CameraUtility.Plugin

Extends the camera function of CM3D2. Mainly adds functions for screenshots.
Adds roll rotation, parse change, background position change, first person viewpoint, Looking at functions, Erase operation panel.
It corresponds to only keyboard operation.

* Attention The keys in the table is written in Japanese keyboard.

## Functions / operation method introduction

### Camera operation (roll rotation, parsing change) * VR not supported *

| Action       | Key |
|:------------:|:---:|
| FOV up       | ]   |
| FOV down     | ;   |
| Reset FOV    | :   |
| Roll left    | .   |
| Roll right   | \   |
| Reset roll   | /   |

### 背景位置変更

| Action        | Key   |  VR Key  |
|:-------------:|:-----:|:--------:|
| Move forward  | ↑     | I        |
| Move back     | ↓     | K        |
| Move left     | ←     | J        |
| Move right    | →     | L        |
| Move up       | PgUp  | 0 (zero) |
| Move down     | PgDn  | P        |
| Roll left     | Del   | U        |
| Roll right    | End   | O        |
| Turn left     | Ins   | 8        |
| Turn right    | Home  | 9        |
| Reset camera  | BkSpc | BkSpc    |

Hold down the `Shift` key and the moving distance will be 1/10. Please fine-tune.
Hold down the `Alt` key and press each key to initialize each operation.
Pressing the `Backspace` key resets the camera movement.
Movement is initialized when the background is changed.
It is recommended to use it last as much as possible because the operation is confusing when using roll rotation.
Since the VR version partially overlaps with the mod's keys, they've been changed.

### Look at this function

| Action                          | Key |
|:-------------------------------:|:---:|
| Toggle "maid faces camera"      |  G  |
| Change "maid faces camera" mode |  T  |

* Gaze direction and face orientation type
   * None = 0
   * Ignore = 1
   * Turning face = 2
   * Move only the face = 3
   * Distracting face = 4
   * Turn your eyes and face = 5
   * Turn only eyes = 6
   * Distract only eyes = 7

Use the "T" key to cycle through the 8 types above. The switching number is displayed on the console.
"G" key is toggle between 0 and 5 in the above table.
When using "look here" at the first person viewpoint, there are things that you will not see by angle restriction
It automatically becomes "None", but you can further change it from there.


### Night-time camera controls *Not in VR*

| Action                     | Key |
|:--------------------------:|:---:|
| Toggle first person mode   |  F  |
| Switch first person target |  R  |

Switch to the viewpoint of a player-like object with the "F" key during Night Service.
Press it again to switch to the blur correction mode.
Press it again to restore it.
In the scene where the hero is not displayed, it will fly to a strange place so please manually cancel it.
Use the "R" key to switch objects that seem to be the main character in order.
The man's head disappears at the time of first person viewpoint to correspond to MOD change MOD.
Since it will cause sickness if you follow in VR mode, I will only move to the position of the hero's point of view.
Even if the position changes, do not follow, so press the key as appropriate.

### UI Controls *Not in VR*

| Action    | Key |
|:---------:|:---:|
| Toggle UI | Tab |

Erase the status and operation panel at editing and night scattering.
Press it again to display it.
Each panel has a folding function, but it is erased all at once.

### Settings File

When you start the game, `UnityInjector\Config\CameraUtility.ini` is generated.
Change of each operation key, numerical value such as moving speed etc. are possible.
For details, refer to the Ini file.

## How to install

### ReiPatcher + UnityInjector

1. ReiPatcher を使用し UnityInjector を導入します。導入方法は割愛。
2. DLL を CM3D2\UnityInjector にコピーします。
   *CM3D2\UnityInjector\UnityInjector にならないように！*
3. ゲームを起動します。
   ゲームと共に起動するコンソールに

        Loaded Plugin: 'Camera Utility 2.3.0.0'
        ...
        Adding Component: 'CameraUtility'

   という表示があれば導入は成功です。


### Sybaris

1. Sybaris を導入します。
2. DLL を Sybaris\Plugins\UnityInjector にコピーします。
3. ゲームを起動します。(同上)


### (重要) DLL ファイル名について

DLL ファイル名が変更されています。
旧バージョンを使用していた場合、重複しないように前のファイルを削除してください。

* CM3D2.CameraUtility.Plugin.dll : 現在のファイル名
* CM3D2CameraUtility.Plugin.dll : 2.0.1.2 までのファイル名
* CM3D2CamerUtility.Plugin.dll : 1.0.0.0 までのファイル名

## ソースについて

ソースのビルドに必要な追加の参照設定は以下になります。

* UnityEngine.dll
* UnityInjector.dll
* Assembly-CSharp.dll

**アップデータ Ver.1.30** より Unity のバージョンが変わりました。
ビルド時の UnityEngine.dll によって対応バージョンが決定するためご注意ください。
以前のバージョンでビルドされた DLL は互換性がありません。


#### AutoCompile (Sybaris)

Sybaris の AutoCompile に対応しています。

1. CM3D2.CameraUtility.Plugin.cs を `Sybaris\Plugins\UnityInjector\AutoCompile` にコピーします。
  - 既に `Sybaris\Plugins\UnityInjector\CM3D2.CameraUtility.Plugin.dll` が存在する場合は削除します。
2. ゲームを起動します。
   `Sybaris\Plugins\UnityInjector` に DLL が生成されます。

*DLL を再生成する場合、先に古い DLL を削除するのを忘れないようにしてください。*



## 変更履歴

* 2.3.4.0
  * NullReferenceException を修正
  * Unity 5.3.4p4 対応
  * VR 対応修正

* 2.3.0.0
  * Chu-B Lip + VR 対応

* 2.2.0.0
  Author: [qucucu](https://github.com/qucucu)
  * 男切り替え機能を追加

* 2.1.1.0
  * FPSモードのクラッシュを修正

* 2.1.0.0
  Author: [Milly](https://github.com/Milly)
  * ファイル名変更
  * 設定ファイルに対応
  * 撮影モードでのUIパネル非表示に対応

* 2.0.1.2-20160220
  Source: [GitHub](https://github.com/MeidoDev/cm3d2-camera-utility)
  Author: [MeidoDev](https://github.com/MeidoDev)
  * Chu-B Lip 対応
  * 回想モードでのFPSモード有効化

* 2.0.1.2-20160103
  Source: [Pastebin](http://pastebin.com/yAUXC30u)
  Author: [したらば改造スレその6 >>944](http://jbbs.shitaraba.net/bbs/read.cgi/game/55179/1447428811/944)
  * FPSモードでのカメラブレの補正機能追加
  * VIPでのFPSモード有効化
  * UIパネルを非表示にできるシーンの拡張(シーンレベル15)

* 2.0.1.2-20151107
  Source: [Pastebin](http://pastebin.com/NxzuFaUe)
  Author: [したらば改造スレその5 >>693](http://jbbs.shitaraba.net/bbs/read.cgi/game/55179/1445294932/693)
  * 一人称視点の男変更MODへの対応修正。

* 2.0.1.2
  * 夜伽開始時にNullReferenceExceptionを吐く不具合を修正。

* 2.0.1.1
  * 一人称視点の男変更MODへの対応。

* 2.0.0.0
  * 一人称視点、こっち見て機能、パネル消去実装。
  * 立ち位置変更の縦回転、初期化機能実装。

* 1.1.0.1
  * 移動量を減らすキーをCtrlからShiftへ変更。
  * VRモードに対応(移動のみ)。

* 1.1.0.0
  * 立ち位置変更機能追加。

* 1.0.0.0
  Source: [Github](https://github.com/k8PzhOFo0/CM3D2CameraUtility.Plugin)
  Author: [k8PzhOFo0](https://github.com/k8PzhOFo0)
  * 初版
