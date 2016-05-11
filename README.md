# CM3D2.CameraUtility.Plugin

CM3D2のカメラ機能を拡張します。その他主に撮影用の機能を追加します。
ロール回転、パース変更、背景位置変更、一人称視点、こっち見て機能、操作パネル消去の機能を加えます。
キーボード操作のみの対応です。

*注意* 表中のキーは日本語キーボードでの表記です。



## 機能・操作方法紹介

### カメラ操作 (ロール回転、パース変更) *VR非対応*

| 機能          | キー   |
|:-------------:|:------:|
| 視野:拡張     | ] (む) |
| 視野:縮小     | ; (れ) |
| 視野初期化    | : (け) |
| ロール回転:左 | . (る) |
| ロール回転:右 | \ (め) |
| ロール初期化  | / (ろ) |

メインカメラのパース(視野、FoV)変更、およびロール回転機能です。
「け」「め」がそれぞれ初期化です。
キーの対応はIllusion系のゲームに合わせています。


### 背景位置変更

| 機能          | キー | VR キー  |
|:-------------:|:----:|:--------:|
| 移動:前       | ↑   | I        |
| 移動:後       | ↓   | K        |
| 移動:左       | ←   | J        |
| 移動:右       | →   | L        |
| 移動:上       | PgUp | 0 (zero) |
| 移動:下       | PgDn | P        |
| 水平回転:左   | Del  | U        |
| 水平回転:右   | End  | O        |
| ロール回転:左 | Ins  | 8        |
| ロール回転:右 | Home | 9        |
| 初期化        | BS   | BS       |

`Shift` キーを押しながらだと移動距離が1/10になります。微調整にどうぞ。
`Alt` キーを押しながら各キーを押すとそれぞれの操作分が初期化されます。
`BS` キーを押すと移動関係をまとめて初期化します。
背景が変更されると移動は初期化されます。
ロール回転を使うと操作が混乱しやすいので極力最後に使うことをお薦めします。
VR版はゲーム内で使われているキーと一部重複するため操作が変わります。


### こっち見て機能

| 機能                     | キー |
|:------------------------:|:----:|
| こっち見て／通常切り替え | G    |
| 視線及び顔の向き切り替え | T    |

* 視線及び顔の向き種別
  * 無し = 0
  * 無視する = 1
  * 顔を向ける = 2
  * 顔だけ動かす = 3
  * 顔をそらす = 4
  * 目と顔を向ける = 5
  * 目だけ向ける = 6
  * 目だけそらす = 7

「T」キーで上記の8種を順に切り替えます。切り替え時の番号はコンソールに表示されます。
「G」キーは上記の表での 0 および 5 の切り替えです。
一人称視点時に "こっち見て" を使うと角度制限で見てくれないことがあるので
自動的に「無し」の状態になりますが、そこから更に変更することは可能です。


### 夜伽時一人称視点 *VR一部対応*

| 機能               | キー |
|:------------------:|:----:|
| 一人称視点切り替え | F    |
| 男切り替え         | R    |

夜伽時に「F」キーで主人公らしき物体の視点に切り替えます。
もう一度押すとブレ補正モードに切り替えます。
さらにもう一度押すと元に戻ります。
主人公が表示されないシーンでは妙な場所に飛ぶので手動で解除してください。
「R」キーで主人公らしき物体を順に切り替えます。
男変更MODへの対応のため一人称視点時には男の頭が消えます。
VRモードで追従すると酔いの原因となるので主人公の視点位置に移動のみします。
体勢が変わっても追従しないので適時キーを押してください。


### パネル表示切り替え *VR非対応*

| 機能               | キー |
|:------------------:|:----:|
| パネル表示切り替え | Tab  |

エディット及び夜伽時にステータスや操作パネルを消します。
もう一度押すと表示します。
各パネルには折りたたみ機能がありますが、こちらは一括で消し去ります。


### 設定ファイル

ゲームを起動すると UnityInjector\Config\CameraUtility.ini が生成されます。
各操作キーの変更、移動速度などの数値の変更が可能です。
詳細は Ini ファイルの中を参照してください。



## 使い方

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

* 2.3.2.0
  * NullReferenceException を修正
  * Unity 5.3.4p4 対応

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
