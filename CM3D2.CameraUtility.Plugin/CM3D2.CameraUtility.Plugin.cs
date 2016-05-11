using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;

namespace CM3D2.CameraUtility.Plugin
{
    [PluginFilter("CM3D2x64"),
    PluginFilter("CM3D2x86"),
    PluginFilter("CM3D2VRx64"),
    PluginFilter("CM3D2OHx64"),
    PluginFilter("CM3D2OHx86"),
    PluginFilter("CM3D2OHVRx64"),
    PluginName("Camera Utility"),
    PluginVersion("2.3.2.0")]
    public class CameraUtility : PluginBase
    {
        #region Constants

        /// <summary>CM3D2のシーンリスト</summary>
        private enum Scene
        {
            /// <summary>メイド選択(夜伽、品評会の前など)</summary>
            SceneCharacterSelect = 1,

            /// <summary>品評会</summary>
            SceneCompetitiveShow = 2,

            /// <summary>昼夜メニュー、仕事結果</summary>
            SceneDaily = 3,

            /// <summary>ダンス1(ドキドキ Fallin' Love)</summary>
            SceneDance_DDFL = 4,

            /// <summary>メイドエディット</summary>
            SceneEdit = 5,

            /// <summary>メーカーロゴ</summary>
            SceneLogo = 6,

            /// <summary>メイド管理</summary>
            SceneMaidManagement = 7,

            /// <summary>ショップ</summary>
            SceneShop = 8,

            /// <summary>タイトル画面</summary>
            SceneTitle = 9,

            /// <summary>トロフィー閲覧</summary>
            SceneTrophy = 10,

            /// <summary>Chu-B Lip 夜伽</summary>
            SceneYotogi_ChuB = 10,

            /// <summary>？？？</summary>
            SceneTryInfo = 11,

            /// <summary>主人公エディット</summary>
            SceneUserEdit = 12,

            /// <summary>起動時警告画面</summary>
            SceneWarning = 13,

            /// <summary>夜伽</summary>
            SceneYotogi = 14,

            /// <summary>ADVパート(kgスクリプト処理)</summary>
            SceneADV = 15,

            /// <summary>日付画面</summary>
            SceneStartDaily = 16,

            /// <summary>タイトルに戻る</summary>
            SceneToTitle = 17,

            /// <summary>MVP</summary>
            SceneSingleEffect = 18,

            /// <summary>スタッフロール</summary>
            SceneStaffRoll = 19,

            /// <summary>ダンス2(Entrance To You)</summary>
            SceneDance_ETY = 20,

            /// <summary>ダンス3(Scarlet Leap)</summary>
            SceneDance_SL = 22,

            /// <summary>回想モード</summary>
            SceneRecollection = 24,

            /// <summary>撮影モード</summary>
            ScenePhotoMode = 27,
        }

        /// <summary>FPS モードを有効化するシーンリスト</summary>
        private static int[] EnableFpsScenes = {
            (int)Scene.SceneYotogi,
            (int)Scene.SceneADV,
            (int)Scene.SceneRecollection,
        };

        /// <summary>Chu-B Lip で FPS モードを有効化するシーンリスト</summary>
        private static int[] EnableFpsScenesChuB = {
            (int)Scene.SceneYotogi_ChuB,
        };

        /// <summary>Hide UI モードを有効化するシーンリスト</summary>
        private static int[] EnableHideUIScenes = {
            (int)Scene.SceneEdit,
            (int)Scene.SceneYotogi,
            (int)Scene.SceneADV,
            (int)Scene.SceneRecollection,
            (int)Scene.ScenePhotoMode,
        };

        /// <summary>Chu-B Lip で Hide UI モードを有効化するシーンリスト</summary>
        private static int[] EnableHideUIScenesChuB = {
            (int)Scene.SceneYotogi_ChuB,
        };

        /// <summary>モディファイアキー</summary>
        private enum ModifierKey
        {
            None = 0,
            Shift,
            Alt,
            Ctrl
        }

        /// <summary>状態変化チェック間隔</summary>
        private const float stateCheckInterval = 1f;

        #endregion
        #region Configuration

        /// <summary>CM3D2.CameraUtility.Plugin 設定ファイル</summary>
        class CameraUtilityConfig : BaseConfig<CameraUtilityConfig>
        {

            [Description("通常キー設定")]
            public class KeyConfig
            {
                //移動関係キー設定
                [Description("背景(メイド) 左移動")]
                public KeyCode bgLeftMove = KeyCode.LeftArrow;
                [Description("背景(メイド) 右移動")]
                public KeyCode bgRightMove = KeyCode.RightArrow;
                [Description("背景 前移動")]
                public KeyCode bgForwardMove = KeyCode.UpArrow;
                [Description("背景 後移動")]
                public KeyCode bgBackMove = KeyCode.DownArrow;
                [Description("背景 上移動")]
                public KeyCode bgUpMove = KeyCode.PageUp;
                [Description("背景 下移動")]
                public KeyCode bgDownMove = KeyCode.PageDown;
                [Description("背景(メイド) 左回転")]
                public KeyCode bgLeftRotate = KeyCode.Delete;
                [Description("背景(メイド) 右回転")]
                public KeyCode bgRightRotate = KeyCode.End;
                [Description("背景 左ロール")]
                public KeyCode bgLeftRoll = KeyCode.Insert;
                [Description("背景 右ロール")]
                public KeyCode bgRightRoll = KeyCode.Home;
                [Description("背景 初期化")]
                public KeyCode bgInitialize = KeyCode.Backspace;

                //カメラ操作関係キー設定
                [Description("カメラ 左ロール")]
                public KeyCode cameraLeftRoll = KeyCode.Period;
                [Description("カメラ 右ロール")]
                public KeyCode cameraRightRoll = KeyCode.Backslash;
                [Description("カメラ 水平")]
                public KeyCode cameraRollInitialize = KeyCode.Slash;
                [Description("カメラ 視野拡大")]
                public KeyCode cameraFoVPlus = KeyCode.RightBracket;
                [Description("カメラ 視野縮小 (初期値 Equals は日本語キーボードでは [; + れ])")]
                public KeyCode cameraFoVMinus = KeyCode.Equals;
                [Description("カメラ 視野初期化 (初期値 Semicolon は日本語キーボードでは [: * け])")]
                public KeyCode cameraFoVInitialize = KeyCode.Semicolon;

                //こっち見てキー設定
                [Description("こっち見て／通常切り替え (トグル)")]
                public KeyCode eyetoCamToggle = KeyCode.G;
                [Description("視線及び顔の向き切り替え (ループ)")]
                public KeyCode eyetoCamChange = KeyCode.T;

                //UI表示トグルキー設定
                [Description("操作パネル表示切り替え (トグル)")]
                public KeyCode hideUIToggle = KeyCode.Tab;

                //FPSモード切替キー設定
                [Description("夜伽時一人称視点切り替え")]
                public KeyCode cameraFPSModeToggle = KeyCode.F;

                //男切替キー設定
                [Description("FPSの対象とする男切り替え(ループ)")]
                public KeyCode manHeadChange = KeyCode.R;

                //モディファイアキー設定
                [Description("低速移動モード (押下中は移動速度が減少)\n設定値: Shift, Alt, Ctrl")]
                public ModifierKey speedDownModifier = ModifierKey.Shift;
                [Description("初期化モード (押下中に移動キーを押すと対象の軸が初期化)\n設定値: Shift, Alt, Ctrl")]
                public ModifierKey initializeModifier = ModifierKey.Alt;
            }

            [Description("VRモード用キー設定")]
            public class OVRKeyConfig : KeyConfig
            {
                public OVRKeyConfig()
                {
                    //移動関係キー設定
                    bgLeftMove = KeyCode.J;
                    bgRightMove = KeyCode.L;
                    bgForwardMove = KeyCode.I;
                    bgBackMove = KeyCode.K;
                    bgUpMove = KeyCode.Alpha0;
                    bgDownMove = KeyCode.P;
                    bgLeftRotate = KeyCode.U;
                    bgRightRotate = KeyCode.O;
                    bgLeftRoll = KeyCode.Alpha8;
                    bgRightRoll = KeyCode.Alpha9;
                    bgInitialize = KeyCode.Backspace;
                }
            }

            [Description("カメラ設定")]
            public class CameraConfig
            {
                [Description("背景 移動速度")]
                public float bgMoveSpeed = 3f;
                [Description("背景(メイド) 回転速度")]
                public float bgRotateSpeed = 120f;
                [Description("カメラ 回転速度")]
                public float cameraRotateSpeed = 60f;
                [Description("視野 変更速度")]
                public float cameraFoVChangeSpeed = 15f;
                [Description("低速移動モード倍率")]
                public float speedMagnification = 0.1f;

                [Description("FPSモード 視野")]
                public float fpsModeFoV = 60f;
                [Description("FPSモード カメラ位置調整 前後\n"
                           + "(カメラ位置を男の目の付近にするには、以下の数値を設定する)\n"
                           + "(メイドが男の喉あたりを見ているため視線が合わない場合がある)\n"
                           + "  fpsOffsetForward = 0.1\n"
                           + "  fpsOffsetUp = 0.12")]
                public float fpsOffsetForward = 0.02f;
                [Description("FPSモード カメラ位置調整 上下")]
                public float fpsOffsetUp = -0.06f;
                [Description("FPSモード カメラ位置調整 左右")]
                public float fpsOffsetRight = 0f;
            }

            [Description("CM3D2.CameraUtility.Plugin 設定ファイル\n\n"
                       + "カメラ設定")]
            public CameraConfig Camera = new CameraConfig();
            [Description("通常キー設定\n"
                       + "設定値: http://docs.unity3d.com/ja/current/ScriptReference/KeyCode.html を参照")]
            public KeyConfig Keys = new KeyConfig();
            [Description("VRモード用キー設定")]
            public OVRKeyConfig OVRKeys = new OVRKeyConfig();
        }

        #endregion
        #region Variables

        //設定
        private CameraUtilityConfig config;

        //オブジェクト
        private Maid maid;
        private CameraMain mainCamera;
        private Transform mainCameraTransform;
        private Transform maidTransform;
        private Transform bg;
        private GameObject manHead;
        private GameObject uiObject;
        private GameObject profilePanel;

        //状態フラグ
        private bool isOVR = false;
        private bool isChuBLip = false;
        private bool fpsMode = false;
        private bool fpsShakeCorrection = false;
        private bool eyetoCamToggle = false;
        private int eyeToCamIndex = 0;
        private bool uiVisible = true;
        private int targetManNumber = 0;
        private int sceneLevel;

        //状態退避変数
        private float defaultFoV = 35f;
        private Vector3 oldCameraPos;
        private Vector3 oldTargetPos;
        private float oldDistance;
        private float oldFoV;
        private Quaternion oldRotation;
        private int oldEyeToCamIndex;
        private Vector3 lastCameraPos = Vector3.zero;

        //コルーチン一覧
        private LinkedList<Coroutine> mainCoroutines = new LinkedList<Coroutine>();

        #endregion
        #region Override Methods

        public void Awake()
        {
            GameObject.DontDestroyOnLoad(this);

            string path = Application.dataPath;
            isChuBLip = path.Contains("CM3D2OH");
            isOVR = path.Contains("VRx64");

            config = CameraUtilityConfig.FromPreferences(Preferences);
            config.SavePreferences();
            SaveConfig();
        }

        public void Start()
        {
            mainCameraTransform = Camera.main.gameObject.transform;
        }

        public void OnLevelWasLoaded(int level)
        {
            sceneLevel = level;
            Log("OnLevelWasLoaded: {0}", sceneLevel);
            StopMainCoroutines();
            config.LoadPreferences();
            if (InitializeSceneObjects())
            {
                StartMainCoroutines();
            }
            VisibleAllManHead();
        }

        #endregion
        #region Properties

        private CameraUtilityConfig.KeyConfig Keys
        {
            get
            {
                return isOVR ? config.OVRKeys : config.Keys;
            }
        }

        private bool AllowUpdate
        {
            get
            {
                // 文字入力パネルがアクティブの場合 false
                return profilePanel == null || !profilePanel.activeSelf;
            }
        }

        #endregion
        #region Private Methods

        #region Methods for Main

        private bool InitializeSceneObjects()
        {
            maid = GameMain.Instance.CharacterMgr.GetMaid(0);
            maidTransform = maid ? maid.body0.transform : null;
            bg = GameObject.Find("__GameMain__/BG").transform;
            mainCamera = GameMain.Instance.MainCamera;
            manHead = null;

            if (isOVR)
            {
                uiObject = GameObject.Find("ovr_screen");
            }
            else
            {
                uiObject = GameObject.Find("/UI Root/Camera");
                if (uiObject == null)
                {
                    uiObject = GameObject.Find("SystemUI Root/Camera");
                }
                defaultFoV = Camera.main.fieldOfView;
            }

            if (sceneLevel == (int)Scene.SceneEdit)
            {
                GameObject uiRoot = GameObject.Find("/UI Root");
                profilePanel = uiRoot.transform.Find("ProfilePanel").gameObject;
            }
            else if (sceneLevel == (int)Scene.SceneUserEdit)
            {
                GameObject uiRoot = GameObject.Find("/UI Root");
                profilePanel = uiRoot.transform.Find("UserEditPanel").gameObject;
            }
            else
            {
                profilePanel = null;
            }

            lastCameraPos = Vector3.zero;
            fpsShakeCorrection = false;
            fpsMode = false;

            return maid && maidTransform && bg && mainCamera;
        }

        private void StartMainCoroutines()
        {
            // Start FirstPersonCamera
            if ((isChuBLip && EnableFpsScenesChuB.Contains(sceneLevel)) || EnableFpsScenes.Contains(sceneLevel))
            {
                if (isOVR)
                {
                    mainCoroutines.AddLast(StartCoroutine(OVRFirstPersonCameraCoroutine()));
                }
                else
                {
                    mainCoroutines.AddLast(StartCoroutine(FirstPersonCameraCoroutine()));
                }
            }

            // Start LookAtThis
            mainCoroutines.AddLast(StartCoroutine(LookAtThisCoroutine()));

            // Start FloorMover
            mainCoroutines.AddLast(StartCoroutine(FloorMoverCoroutine()));

            // Start ExtendedCameraHandle
            if (!isOVR)
            {
                mainCoroutines.AddLast(StartCoroutine(ExtendedCameraHandleCoroutine()));
            }

            // Start HideUI
            if ((isChuBLip && EnableHideUIScenesChuB.Contains(sceneLevel)) || EnableHideUIScenes.Contains(sceneLevel))
            {
                if (!isOVR)
                {
                    mainCoroutines.AddLast(StartCoroutine(HideUICoroutine()));
                }
            }
        }

        private void StopMainCoroutines()
        {
            foreach (var coroutine in mainCoroutines)
            {
                StopCoroutine(coroutine);
            }
            mainCoroutines.Clear();
        }

        #endregion

        #region Methods for Misc

        private void Log(string format, params object[] args)
        {
            Debug.Log(Name + ": " + string.Format(format, args));
        }

        private bool IsModKeyPressing(ModifierKey key)
        {
            switch (key)
            {
                case ModifierKey.Shift:
                    return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));

                case ModifierKey.Alt:
                    return (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));

                case ModifierKey.Ctrl:
                    return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));

                default:
                    return false;
            }
        }

        private void SaveCameraPos()
        {
            Assert.IsNotNull(mainCamera);
            Assert.IsNotNull(mainCameraTransform);

            oldCameraPos = mainCamera.GetPos();
            oldTargetPos = mainCamera.GetTargetPos();
            oldDistance = mainCamera.GetDistance();
            oldRotation = mainCameraTransform.rotation;
            oldFoV = Camera.main.fieldOfView;
        }

        private void LoadCameraPos()
        {
            Assert.IsNotNull(mainCamera);
            Assert.IsNotNull(mainCameraTransform);

            mainCameraTransform.rotation = oldRotation;
            mainCamera.SetPos(oldCameraPos);
            mainCamera.SetTargetPos(oldTargetPos, true);
            mainCamera.SetDistance(oldDistance, true);
            Camera.main.fieldOfView = oldFoV;
        }

        private Vector3 GetYotogiPlayPosition()
        {
            Assert.IsNotNull(mainCamera);
            var field = mainCamera.GetType().GetField("m_vCenter", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            return (Vector3)field.GetValue(mainCamera);
        }

        private int GetFadeState()
        {
            Assert.IsNotNull(mainCamera);
            var field = mainCamera.GetType().GetField("m_eFadeState", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            return (int)field.GetValue(mainCamera);
        }

        private GameObject FindByNameInChildren(GameObject parent, string name)
        {
            Assert.IsNotNull(parent);
            Assert.IsNotNull(name);
            foreach (Transform transform in parent.transform)
            {
                if (transform.name.IndexOf(name) > -1)
                {
                    return transform.gameObject;
                }
            }
            return null;
        }

        private void SetRendererEnabled(GameObject obj, bool enabled)
        {
            Assert.IsNotNull(obj);
            var renderer = obj.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.enabled = enabled;
            }
        }

        #endregion

        #region Methods for FirstPersonCamera

        private void OVRToggleFirstPersonCameraMode()
        {
            if (uiObject && manHead)
            {
                //SetEyeToCamToggle(false);
                Vector3 localPos = uiObject.transform.localPosition;
                mainCamera.SetPos(manHead.transform.position);
                uiObject.transform.position = manHead.transform.position;
                uiObject.transform.localPosition = localPos;
            }
        }

        private void ToggleFirstPersonCameraMode()
        {
            if (fpsShakeCorrection)
            {
                fpsShakeCorrection = false;
                SetFirstPersonCameraMode(false);
            }
            else if (fpsMode)
            {
                fpsShakeCorrection = true;
            }
            else
            {
                SetFirstPersonCameraMode(true);
            }
            Log("FpsMode = {0}, ShakeCorrection = {1}", fpsMode, fpsShakeCorrection);
        }

        private void SetFirstPersonCameraMode(bool enable)
        {
            if (fpsMode == enable) return;

            Assert.IsNotNull(maid);
            Assert.IsNotNull(manHead);
            Assert.IsNotNull(mainCameraTransform);

            fpsMode = enable;
            if (fpsMode)
            {
                // save status
                SaveCameraPos();
                lastCameraPos = oldCameraPos;
                Camera.main.fieldOfView = config.Camera.fpsModeFoV;
                oldEyeToCamIndex = eyeToCamIndex;
                // reset EyeToCam
                SetEyeToCamToggle(false);
                // setup camera
                mainCameraTransform.rotation = Quaternion.LookRotation(-manHead.transform.up);
                // hide ManHead
                SetRendererEnabled(manHead, false);
            }
            else
            {
                UpdateFirstPersonCamera();
                LoadCameraPos();
                SetEyeToCamIndex(oldEyeToCamIndex);
                SetRendererEnabled(manHead, true);
            }
        }

        private void UpdateFirstPersonCamera()
        {
            Assert.IsNotNull(manHead);
            Assert.IsNotNull(mainCamera);
            Assert.IsNotNull(mainCameraTransform);

            Vector3 cameraTargetPosFromScript = GetYotogiPlayPosition();
            if (oldTargetPos != cameraTargetPosFromScript)
            {
                Log("Position Changed!");
                mainCameraTransform.rotation = Quaternion.LookRotation(-manHead.transform.up);
                oldTargetPos = cameraTargetPosFromScript;
            }

            Vector3 cameraPos = manHead.transform.position
                + manHead.transform.up * config.Camera.fpsOffsetUp
                + manHead.transform.right * config.Camera.fpsOffsetRight
                + manHead.transform.forward * config.Camera.fpsOffsetForward;
            if (fpsShakeCorrection)
            {
                cameraPos = Vector3.Lerp(cameraPos, lastCameraPos, 0.9f);
            }
            lastCameraPos = cameraPos;
            mainCamera.SetPos(cameraPos);
            mainCamera.SetTargetPos(cameraPos, true);
            mainCamera.SetDistance(0f, true);
        }

        #endregion

        #region Methods for ManHead

        private void FindAndChangeManHead(int manNumber)
        {
            var manCount = GetVisibleManCount();
            if (manNumber < 0 || manCount <= manNumber) return;

            var newManHead = FindManHead(manNumber);
            if (newManHead)
            {
                ChangeManHead(newManHead);
            }
        }

        private void ChangeManHead(GameObject newManHead)
        {
            Assert.IsNotNull(mainCameraTransform);
            Assert.IsNotNull(newManHead);

            if (fpsMode)
            {
                if (manHead)
                {
                    SetRendererEnabled(manHead, true);
                }

                manHead = newManHead;
                mainCameraTransform.rotation = Quaternion.LookRotation(-manHead.transform.up);
                SetRendererEnabled(manHead, false);
            }
            else
            {
                manHead = newManHead;
            }
        }

        private GameObject FindManHead(int manNumber)
        {
            var man = GameMain.Instance.CharacterMgr.GetMan(manNumber);
            if (!man || !man.body0 || !man.body0.trsHead) return null;
            var head = man.body0.trsHead.gameObject;
            if (!head) return null;
            var mhead = FindByNameInChildren(head, "mhead");
            if (!mhead) return null;
            return FindByNameInChildren(mhead, "ManHead");
        }

        private int GetVisibleManCount()
        {
            var characterMgr = GameMain.Instance.CharacterMgr;
            var manCount = characterMgr.GetManCount();
            for (int number = 0; number < manCount; number++)
            {
                var man = characterMgr.GetMan(number);
                if (!man.Visible) return number;
            }
            return manCount;
        }

        private void VisibleAllManHead()
        {
            var manCount = GameMain.Instance.CharacterMgr.GetManCount();
            for (int number = 0; number < manCount; number++)
            {
                var head = FindManHead(number);
                if (head != null)
                {
                    SetRendererEnabled(head, true);
                }
            }
        }

        private void IncreseTargetManNumber()
        {
            targetManNumber = targetManNumber + 1;
            var visibleManCount = GetVisibleManCount();
            if (visibleManCount <= targetManNumber)
            {
                targetManNumber = 0;
            }
            Log("Change ManHeadNumber: " + targetManNumber);
        }

        private bool IsIllegalTargetMan()
        {
            var visibleManCount = GetVisibleManCount();
            if (visibleManCount == 0) return false;
            return visibleManCount <= targetManNumber;
        }

        #endregion

        #region Methods for ExtendedCameraHandle

        private void UpdateCameraFoV()
        {
            if (Input.GetKey(Keys.cameraFoVInitialize))
            {
                Camera.main.fieldOfView = defaultFoV;
                return;
            }

            float fovChangeSpeed = config.Camera.cameraFoVChangeSpeed * Time.deltaTime;
            if (IsModKeyPressing(Keys.speedDownModifier))
            {
                fovChangeSpeed *= config.Camera.speedMagnification;
            }

            if (Input.GetKey(Keys.cameraFoVMinus))
            {
                Camera.main.fieldOfView += -fovChangeSpeed;
            }
            if (Input.GetKey(Keys.cameraFoVPlus))
            {
                Camera.main.fieldOfView += fovChangeSpeed;
            }
        }

        private void UpdateCameraRotation()
        {
            Assert.IsNotNull(mainCameraTransform);

            if (Input.GetKey(Keys.cameraRollInitialize))
            {
                mainCameraTransform.eulerAngles = new Vector3(
                        mainCameraTransform.rotation.eulerAngles.x,
                        mainCameraTransform.rotation.eulerAngles.y,
                        0f);
                return;
            }

            float rotateSpeed = config.Camera.cameraRotateSpeed * Time.deltaTime;
            if (IsModKeyPressing(Keys.speedDownModifier))
            {
                rotateSpeed *= config.Camera.speedMagnification;
            }

            if (Input.GetKey(Keys.cameraLeftRoll))
            {
                mainCameraTransform.Rotate(0, 0, rotateSpeed);
            }
            if (Input.GetKey(Keys.cameraRightRoll))
            {
                mainCameraTransform.Rotate(0, 0, -rotateSpeed);
            }
        }

        #endregion

        #region Methods for FloorMover

        private void UpdateBackgroudPosition()
        {
            Assert.IsNotNull(bg);

            if (Input.GetKeyDown(Keys.bgInitialize))
            {
                bg.localPosition = Vector3.zero;
                bg.RotateAround(maidTransform.position, Vector3.up, -bg.rotation.eulerAngles.y);
                bg.RotateAround(maidTransform.position, Vector3.right, -bg.rotation.eulerAngles.x);
                bg.RotateAround(maidTransform.position, Vector3.forward, -bg.rotation.eulerAngles.z);
                bg.RotateAround(maidTransform.position, Vector3.up, -bg.rotation.eulerAngles.y);
                return;
            }

            if (IsModKeyPressing(Keys.initializeModifier))
            {
                if (Input.GetKey(Keys.bgLeftRotate) || Input.GetKey(Keys.bgRightRotate))
                {
                    bg.RotateAround(maidTransform.position, Vector3.up, -bg.rotation.eulerAngles.y);
                }
                if (Input.GetKey(Keys.bgLeftRoll) || Input.GetKey(Keys.bgRightRoll))
                {
                    bg.RotateAround(maidTransform.position, Vector3.forward, -bg.rotation.eulerAngles.z);
                    bg.RotateAround(maidTransform.position, Vector3.right, -bg.rotation.eulerAngles.x);
                }
                if (Input.GetKey(Keys.bgLeftMove) || Input.GetKey(Keys.bgRightMove) || Input.GetKey(Keys.bgBackMove) || Input.GetKey(Keys.bgForwardMove))
                {
                    bg.localPosition = new Vector3(0f, bg.localPosition.y, 0f);
                }
                if (Input.GetKey(Keys.bgUpMove) || Input.GetKey(Keys.bgDownMove))
                {
                    bg.localPosition = new Vector3(bg.localPosition.x, 0f, bg.localPosition.z);
                }
                return;
            }

            Vector3 cameraForward = mainCameraTransform.TransformDirection(Vector3.forward);
            Vector3 cameraRight = mainCameraTransform.TransformDirection(Vector3.right);
            Vector3 cameraUp = mainCameraTransform.TransformDirection(Vector3.up);
            Vector3 direction = Vector3.zero;

            float moveSpeed = config.Camera.bgMoveSpeed * Time.deltaTime;
            float rotateSpeed = config.Camera.bgRotateSpeed * Time.deltaTime;
            if (IsModKeyPressing(Keys.speedDownModifier))
            {
                moveSpeed *= config.Camera.speedMagnification;
                rotateSpeed *= config.Camera.speedMagnification;
            }

            if (Input.GetKey(Keys.bgLeftMove))
            {
                direction += new Vector3(cameraRight.x, 0f, cameraRight.z) * moveSpeed;
            }
            if (Input.GetKey(Keys.bgRightMove))
            {
                direction += new Vector3(cameraRight.x, 0f, cameraRight.z) * -moveSpeed;
            }
            if (Input.GetKey(Keys.bgBackMove))
            {
                direction += new Vector3(cameraForward.x, 0f, cameraForward.z) * moveSpeed;
            }
            if (Input.GetKey(Keys.bgForwardMove))
            {
                direction += new Vector3(cameraForward.x, 0f, cameraForward.z) * -moveSpeed;
            }
            if (Input.GetKey(Keys.bgUpMove))
            {
                direction += new Vector3(0f, cameraUp.y, 0f) * -moveSpeed; }
            if (Input.GetKey(Keys.bgDownMove))
            {
                direction += new Vector3(0f, cameraUp.y, 0f) * moveSpeed;
            }

            //bg.position += direction;
            bg.localPosition += direction;

            if (Input.GetKey(Keys.bgLeftRotate))
            {
                bg.RotateAround(maidTransform.transform.position, Vector3.up, rotateSpeed);
            }
            if (Input.GetKey(Keys.bgRightRotate))
            {
                bg.RotateAround(maidTransform.transform.position, Vector3.up, -rotateSpeed);
            }
            if (Input.GetKey(Keys.bgLeftRoll))
            {
                bg.RotateAround(maidTransform.transform.position, new Vector3(cameraForward.x, 0f, cameraForward.z), rotateSpeed);
            }
            if (Input.GetKey(Keys.bgRightRoll))
            {
                bg.RotateAround(maidTransform.transform.position, new Vector3(cameraForward.x, 0f, cameraForward.z), -rotateSpeed);
            }
        }

        #endregion

        #region Methods for EyeToCam

        private void SetEyeToCamIndex(int index)
        {
            Assert.IsNotNull(maid);

            var eyeMoveTypes = (Maid.EyeMoveType[])Enum.GetValues(typeof(Maid.EyeMoveType));
            eyeToCamIndex = index % eyeMoveTypes.Length;
            if (eyeToCamIndex < 0)
            {
                eyeToCamIndex += eyeMoveTypes.Length;
            }
            var eyeMoveType = eyeMoveTypes[eyeToCamIndex];
            maid.EyeToCamera(eyeMoveType, 0f);
            eyetoCamToggle = (eyeMoveType != Maid.EyeMoveType.無し);
            Log("EyeToCam = {0}, EyeMoveType = [{1}]{2}", eyetoCamToggle, eyeToCamIndex, eyeMoveType);
        }

        private void SetEyeToCamToggle(bool enable)
        {
            Assert.IsNotNull(maid);

            eyetoCamToggle = enable;
            var eyeMoveType = (eyetoCamToggle) ? Maid.EyeMoveType.目と顔を向ける : Maid.EyeMoveType.無し;
            maid.EyeToCamera(eyeMoveType, 0f);
            var eyeMoveTypes = (Maid.EyeMoveType[])Enum.GetValues(typeof(Maid.EyeMoveType));
            eyeToCamIndex = Array.IndexOf(eyeMoveTypes, eyeMoveType);
            Log("EyeToCam = {0}, EyeMoveType = [{1}]{2}", eyetoCamToggle, eyeToCamIndex, eyeMoveType);
        }

        #endregion

        #region Methods for HideUI

        private void ToggleUIVisible()
        {
            uiVisible = !uiVisible;
            if (uiObject)
            {
                uiObject.SetActive(uiVisible);
                Log("UIVisible:{0}", uiVisible);
            }
        }

        #endregion

        #endregion
        #region Coroutines

        private IEnumerator OVRFirstPersonCameraCoroutine()
        {
            while (!manHead)
            {
                FindAndChangeManHead(targetManNumber);
                yield return new WaitForSeconds(stateCheckInterval);
            }
            while (true)
            {
                while (!AllowUpdate)
                {
                    yield return new WaitForSeconds(stateCheckInterval);
                }
                if (Input.GetKeyDown(Keys.cameraFPSModeToggle))
                {
                    OVRToggleFirstPersonCameraMode();
                }
                if (Input.GetKeyDown(Keys.manHeadChange) || IsIllegalTargetMan())
                {
                    IncreseTargetManNumber();
                    FindAndChangeManHead(targetManNumber);
                }
                yield return null;
            }
        }

        private IEnumerator FirstPersonCameraCoroutine()
        {
            while (!manHead)
            {
                FindAndChangeManHead(targetManNumber);
                yield return new WaitForSeconds(stateCheckInterval);
            }
            while (true)
            {
                while (!AllowUpdate)
                {
                    yield return new WaitForSeconds(stateCheckInterval);
                }
                if (Input.GetKeyDown(Keys.cameraFPSModeToggle))
                {
                    ToggleFirstPersonCameraMode();
                }
                if (Input.GetKeyDown(Keys.manHeadChange) || IsIllegalTargetMan())
                {
                    IncreseTargetManNumber();
                     FindAndChangeManHead(targetManNumber);
                }
                if (fpsMode)
                {
                    UpdateFirstPersonCamera();
                }
                yield return null;
            }
        }

        private IEnumerator FloorMoverCoroutine()
        {
            while (true)
            {
                UpdateBackgroudPosition();
                yield return null;
            }
        }

        private IEnumerator ExtendedCameraHandleCoroutine()
        {
            while (true)
            {
                UpdateCameraFoV();
                UpdateCameraRotation();
                yield return null;
            }
        }

        private IEnumerator LookAtThisCoroutine()
        {
            while (true)
            {
                if (Input.GetKeyDown(Keys.eyetoCamChange))
                {
                    SetEyeToCamIndex(eyeToCamIndex + 1);
                }
                if (Input.GetKeyDown(Keys.eyetoCamToggle))
                {
                    SetEyeToCamToggle(!eyetoCamToggle);
                }
                yield return null;
            }
        }

        private IEnumerator HideUICoroutine()
        {
            while (true)
            {
                if (Input.GetKeyDown(Keys.hideUIToggle))
                {
                    if (GetFadeState() == 0)
                    {
                        ToggleUIVisible();
                    }
                }
                yield return null;
            }
        }

        #endregion
    }

    #region Helper Classes

    public abstract class BaseConfig<TConfig> where TConfig: BaseConfig<TConfig>
    {
        private ExIni.IniFile Preferences;

        public static TConfig FromPreferences(ExIni.IniFile prefs)
        {
            TConfig config = (TConfig)Activator.CreateInstance(typeof(TConfig));
            config.Preferences = prefs;
            config.LoadPreferences();
            return config;
        }

        public void LoadPreferences()
        {
            foreach (var field in typeof(TConfig).GetFields())
            {
                var sectionName = field.Name;
                var sectionType = field.FieldType;
                var getSectionMethod = typeof(TConfig)
                    .GetMethod("GetSection", new Type[] {typeof(string)})
                    .MakeGenericMethod(sectionType);
                var section = getSectionMethod.Invoke(this, new object[] {sectionName});
                field.SetValue(this, section);
            }
        }

        public void SavePreferences()
        {
            foreach (var field in typeof(TConfig).GetFields())
            {
                var sectionName = field.Name;
                var sectionType = field.FieldType;
                var setSectionMethod = typeof(TConfig).GetMethod("SetSection").MakeGenericMethod(sectionType);
                var config = field.GetValue(this);
                setSectionMethod.Invoke(this, new object[] {sectionName, config});
                UpdateComment(field, Preferences[sectionName].Comments);
            }
            UpdateComment(typeof(TConfig), Preferences.Comments);
        }

        public T GetSection<T>(string sectionName)
        {
            T config = (T)Activator.CreateInstance(typeof(T));
            return GetSection(sectionName, config);
        }

        public T GetSection<T>(string sectionName, T config)
        {
            Assert.IsNotNull(sectionName);
            Assert.IsNotNull(config);
            var section = Preferences.GetSection(sectionName);
            if (section != null)
            {
                foreach (var field in typeof(T).GetFields())
                {
                    string keyName = field.Name;
                    var key = section.GetKey(keyName);
                    if (key != null)
                    {
                        try
                        {
                            var converter = TypeDescriptor.GetConverter(field.FieldType);
                            var value = converter.ConvertFromString(key.RawValue);
                            field.SetValue(config, value);
                        }
                        catch
                        {
                            Debug.LogWarning(string.Format("{0}: Config read error: [{1}]{2}", GetType().Name, sectionName, keyName));
                        }
                    }
                }
            }
            return config;
        }

        public void SetSection<T>(string sectionName, T config)
        {
            Assert.IsNotNull(sectionName);
            Assert.IsNotNull(config);
            var section = Preferences[sectionName];
            foreach (var field in typeof(T).GetFields())
            {
                string keyName = field.Name;
                var key = section[keyName];
                var value = field.GetValue(config);
                var converter = TypeDescriptor.GetConverter(field.FieldType);
                key.Value = converter.ConvertToString(value);
                UpdateComment(field, key.Comments);
            }
            UpdateComment(typeof(T), section.Comments);
        }

        private static void UpdateComment(MemberInfo info, ExIni.IniComment comment)
        {
            var desc = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            if (desc != null && !string.IsNullOrEmpty(desc.Description))
            {
                var lines = desc.Description.Split(new string[] {"\n"}, StringSplitOptions.None);
                comment.Comments = new List<string>(lines);
            }
        }
    }

    public static class Assert
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void IsNotNull(object obj)
        {
            if (obj == null)
            {
                string msg = "Assertion failed. Value is null.";
                UnityEngine.Debug.LogError(msg);
            }
        }
    }

    #endregion
}

