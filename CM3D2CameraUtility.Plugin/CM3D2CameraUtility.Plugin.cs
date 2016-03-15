// CM3D2CameraUtility.Plugin v2.0.1.2 改変の改変（非公式)
// 改変元 したらば改造スレその5 >>693
// http://pastebin.com/NxzuFaUe

// 20160220
// ・Chu-B Lip 対応
// ・回想モードでのFPSモード有効化

// 20160103
// ・FPSモードでのカメラブレの補正機能追加
// ・VIPでのFPSモード有効化
// ・UIパネルを非表示にできるシーンの拡張
// 　(シーンレベル15)

// ■カメラブレ補正について
// Fキー(デフォルトの場合)を一回押下でオリジナルのFPSモード、もう一回押下でブレ補正モード。
// 再度押下でFPSモード解除。

// FPSモードの視点は男の頭の位置にカメラがセットされますが、
// 男の動きが激しい体位では視線がガクガクと大きく揺れます。
// 新しく追加したブレ補正モードではこの揺れを小さく抑えます。
// ただし男の目の位置とカメラ位置が一致しなくなるので、男の透明度を上げていると
// 体位によっては男の胴体の一部がちらちらと映り込みます。
// これの改善のため首の描画を消そうと思いましたが、男モデルは「頭部」「体」の2種類しか
// レンダリングされていないようで無理っぽかった。
// 気になる人は男の透明度を下げてください。


// CM3D2CameraUtility.Plugin v2.0.1.2 改変（非公式)
// Original by k8PzhOFo0 (https://github.com/k8PzhOFo0/CM3D2CameraUtility.Plugin)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;

namespace CM3D2CameraUtility
{
    [PluginFilter("CM3D2x64"),
    PluginFilter("CM3D2x86"),
    PluginFilter("CM3D2VRx64"),
    PluginFilter("CM3D2OHx64"),
    PluginName("Camera Utility"),
    PluginVersion("2.0.1.2-20160220")]
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

        #endregion
        #region Configuration

        /// <summary>キー設定</summary>
        class KeyConfig
        {
            //移動関係キー設定
            public KeyCode bgLeftMove = KeyCode.LeftArrow;
            public KeyCode bgRightMove = KeyCode.RightArrow;
            public KeyCode bgForwardMove = KeyCode.UpArrow;
            public KeyCode bgBackMove = KeyCode.DownArrow;
            public KeyCode bgUpMove = KeyCode.PageUp;
            public KeyCode bgDownMove = KeyCode.PageDown;
            public KeyCode bgLeftRotate = KeyCode.Delete;
            public KeyCode bgRightRotate = KeyCode.End;
            public KeyCode bgLeftPitch = KeyCode.Insert;
            public KeyCode bgRightPitch = KeyCode.Home;
            public KeyCode bgInitialize = KeyCode.Backspace;

            //カメラ操作関係キー設定
            public KeyCode cameraLeftPitch = KeyCode.Period;
            public KeyCode cameraRightPitch = KeyCode.Backslash;
            public KeyCode cameraPitchInitialize = KeyCode.Slash;
            public KeyCode cameraFoVPlus = KeyCode.RightBracket;

            //Equalsになっているが日本語キーボードだとセミコロン
            public KeyCode cameraFoVMinus = KeyCode.Equals;

            //Semicolonになっているが日本語キーボードだとコロン
            public KeyCode cameraFoVInitialize = KeyCode.Semicolon;

            //こっち見てキー設定
            public KeyCode eyetoCamToggle = KeyCode.G;
            public KeyCode eyetoCamChange = KeyCode.T;

            //UI表示トグルキー設定
            public KeyCode hideUIToggle = KeyCode.Tab;

            //FPSモード切替キー設定
            public KeyCode cameraFPSModeToggle = KeyCode.F;

            //モディファイアキー設定
            public ModifierKey bgSpeedDownModifier = ModifierKey.Shift;
            public ModifierKey bgResetModifier = ModifierKey.Alt;
        }

        /// <summary>OVR用キー設定</summary>
        class OVRKeyConfig : KeyConfig
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
                bgLeftPitch = KeyCode.Alpha8;
                bgRightPitch = KeyCode.Alpha9;
                bgInitialize = KeyCode.Backspace;
            }
        }

        /// <summary>カメラ設定</summary>
        class CameraConfig
        {
            public float fpsModeFoV = 60f;

            public float cameraRotateSpeed = 60f;
            public float cameraFOVChangeSpeed = 15f;
            public float floorMoveSpeed = 3f;
            public float maidRotateSpeed = 120f;

            public float fpsOffsetForward = 0.02f;
            public float fpsOffsetUp = -0.06f;
            public float fpsOffsetRight = 0f;

            ////以下の数値だと男の目の付近にカメラが移動しますが
            ////うちのメイドはデフォで顔ではなく喉元見てるのであんまりこっち見てくれません
            //public float fpsOffsetForward = 0.1f;
            //public float fpsOffsetUp = 0.12f;
        }

        #endregion
        #region Variables

        private KeyConfig keys;
        private CameraConfig cameraConfig;

        //オブジェクト
        private Maid maid;
        private CameraMain mainCamera;
        private Transform mainCameraTransform;
        private Transform maidTransform;
        private Transform bg;
        private GameObject manHead;
        private GameObject uiObject;

        private bool occulusVR = false;
        private bool chubLip = false;
        private bool fpsMode = false;
        private bool eyetoCamToggle = false;

        private float defaultFOV = 35f;

        private int sceneLevel;

        private Vector3 oldPos;
        private Vector3 oldTargetPos;
        private float oldDistance;
        private float oldFoV;
        private Quaternion oldRotation;

        private bool oldEyetoCamToggle;
        private int eyeToCamIndex = 0;

        private bool uiVisible = true;
        private GameObject profilePanel;

        private Vector3 cameraOffset = Vector3.zero;
        private bool bFpsShakeCorrection = false;

        private float stateCheckInterval = 1f;
        private LinkedList<Coroutine> mainCoroutines = new LinkedList<Coroutine>();

        #endregion
        #region Override Methods

        public void Awake()
        {
            GameObject.DontDestroyOnLoad(this);

            string path = Application.dataPath;
            chubLip = path.Contains("CM3D2OHx64");
            occulusVR = path.Contains("CM3D2VRx64");

            keys = GetConfig<KeyConfig>("Keys");
            OVRKeyConfig ovrKeys = GetConfig<OVRKeyConfig>("OVRKeys");
            cameraConfig = GetConfig<CameraConfig>("Camera");
            SaveConfig();

            if (occulusVR)
            {
                keys = ovrKeys;
            }
        }

        public void Start()
        {
            mainCameraTransform = Camera.main.gameObject.transform;
        }

        public void OnLevelWasLoaded(int level)
        {
            sceneLevel = level;
            StopMainCoroutines();
            if (InitializeSceneObjects())
            {
                StartMainCoroutines();
            }
        }

        #endregion
        #region Private Methods

        private bool AllowUpdate
        {
            get
            {
                // 文字入力パネルがアクティブの場合 false
                return profilePanel == null || !profilePanel.activeSelf;
            }
        }

        private bool InitializeSceneObjects()
        {
            maid = GameMain.Instance.CharacterMgr.GetMaid(0);
            maidTransform = maid ? maid.body0.transform : null;
            bg = GameObject.Find("__GameMain__/BG").transform;
            mainCamera = GameMain.Instance.MainCamera;
            manHead = null;

            if (occulusVR)
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
                defaultFOV = Camera.main.fieldOfView;
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

            cameraOffset = Vector3.zero;
            bFpsShakeCorrection = false;
            fpsMode = false;

            return maid && maidTransform && bg && mainCamera;
        }

        private void StartMainCoroutines()
        {
            // Start FirstPersonCamera
            if ((chubLip && EnableFpsScenesChuB.Contains(sceneLevel)) || EnableFpsScenes.Contains(sceneLevel))
            {
                if (occulusVR)
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
            if (!occulusVR)
            {
                mainCoroutines.AddLast(StartCoroutine(ExtendedCameraHandleCoroutine()));
            }

            // Start HideUI
            if ((chubLip && EnableHideUIScenesChuB.Contains(sceneLevel)) || EnableHideUIScenes.Contains(sceneLevel))
            {
                if (!occulusVR)
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

            oldPos = mainCamera.GetPos();
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
            mainCamera.SetPos(oldPos);
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

        private GameObject FindManHead()
        {
            GameObject manExHead = GameObject.Find("__GameMain__/Character/Active/AllOffset/Man[0]");
            Transform[] manExHeadTransforms = manExHead ? manExHead.GetComponentsInChildren<Transform>() : new Transform[0];
            Transform[] manHedas = manExHeadTransforms.Where(trans => trans.name.IndexOf("_SM_") > -1).ToArray();
            foreach (Transform mh in manHedas)
            {
                GameObject smManHead = mh.gameObject;
                foreach (Transform smmh in smManHead.transform)
                {
                    if (smmh.name.IndexOf("ManHead") > -1)
                    {
                        return smmh.gameObject;
                    }
                }
            }
            return null;
        }

        private void OVRToggleFirstPersonCameraMode()
        {
            if (uiObject)
            {
                //    eyetoCamToggle = false;
                //    maid.EyeToCamera(Maid.EyeMoveType.無し, 0f);
                Vector3 localPos = uiObject.transform.localPosition;
                mainCamera.SetPos(manHead.transform.position);
                uiObject.transform.position = manHead.transform.position;
                uiObject.transform.localPosition = localPos;
            }
        }

        private void ToggleFirstPersonCameraMode()
        {
            Assert.IsNotNull(maid);
            Assert.IsNotNull(manHead);
            Assert.IsNotNull(mainCameraTransform);

            if (bFpsShakeCorrection)
            {
                bFpsShakeCorrection = false;
                fpsMode = false;
                Console.WriteLine("FpsMode = Disable");
            }
            else if(fpsMode && !bFpsShakeCorrection)
            {
                bFpsShakeCorrection = true;
                Console.WriteLine("FpsMode = Enable : ShakeCorrection = Enable");
            }
            else
            {
                fpsMode = true;
                SaveCameraPos();
                Console.WriteLine("FpsMode = Enable : ShakeCorrection = Disable");
            }

            if (fpsMode)
            {
                Camera.main.fieldOfView = cameraConfig.fpsModeFoV;
                eyetoCamToggle = false;
                maid.EyeToCamera(Maid.EyeMoveType.無し, 0f);

                mainCameraTransform.rotation = Quaternion.LookRotation(-manHead.transform.up);

                manHead.renderer.enabled = false;
            }
            else
            {
                Vector3 cameraTargetPosFromScript = GetYotogiPlayPosition();

                if (oldTargetPos != cameraTargetPosFromScript)
                {
                    Console.WriteLine("Position Changed!");
                    oldTargetPos = cameraTargetPosFromScript;
                }
                manHead.renderer.enabled = true;

                LoadCameraPos();
                eyetoCamToggle = oldEyetoCamToggle;
                oldEyetoCamToggle = eyetoCamToggle;
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
                Console.WriteLine("Position Changed!");
                mainCameraTransform.rotation = Quaternion.LookRotation(-manHead.transform.up);
                oldTargetPos = cameraTargetPosFromScript;
            }

            Vector3 cameraPos = manHead.transform.position
                + manHead.transform.up * cameraConfig.fpsOffsetUp
                + manHead.transform.right * cameraConfig.fpsOffsetRight
                + manHead.transform.forward * cameraConfig.fpsOffsetForward;
            if (bFpsShakeCorrection)
            {
                cameraOffset = Vector3.Lerp(cameraPos, cameraOffset, 0.9f);
            }
            else
            {
                cameraOffset = cameraPos;
            }
            mainCamera.SetPos(cameraOffset);
            mainCamera.SetTargetPos(cameraOffset, true);
            mainCamera.SetDistance(0f, true);
        }

        private void UpdateCameraFOV()
        {
            if (Input.GetKey(keys.cameraFoVInitialize))
            {
                Camera.main.fieldOfView = defaultFOV;
                return;
            }

            float fovChangeSpeed = cameraConfig.cameraFOVChangeSpeed * Time.deltaTime;
            if (Input.GetKey(keys.cameraFoVMinus))
            {
                Camera.main.fieldOfView += -fovChangeSpeed;
            }
            if (Input.GetKey(keys.cameraFoVPlus))
            {
                Camera.main.fieldOfView += fovChangeSpeed;
            }
        }

        private void UpdateCameraPitch()
        {
            Assert.IsNotNull(mainCameraTransform);

            if (Input.GetKey(keys.cameraPitchInitialize))
            {
                mainCameraTransform.eulerAngles = new Vector3(
                        mainCameraTransform.rotation.eulerAngles.x,
                        mainCameraTransform.rotation.eulerAngles.y,
                        0f);
                return;
            }

            float rotateSpeed = cameraConfig.cameraRotateSpeed * Time.deltaTime;
            if (Input.GetKey(keys.cameraLeftPitch))
            {
                mainCameraTransform.Rotate(0, 0, rotateSpeed);
            }
            if (Input.GetKey(keys.cameraRightPitch))
            {
                mainCameraTransform.Rotate(0, 0, -rotateSpeed);
            }
        }

        private void UpdateBackgroudPosition()
        {
            Assert.IsNotNull(bg);

            if (Input.GetKeyDown(keys.bgInitialize))
            {
                bg.localPosition = Vector3.zero;
                bg.RotateAround(maidTransform.position, Vector3.up, -bg.rotation.eulerAngles.y);
                bg.RotateAround(maidTransform.position, Vector3.right, -bg.rotation.eulerAngles.x);
                bg.RotateAround(maidTransform.position, Vector3.forward, -bg.rotation.eulerAngles.z);
                bg.RotateAround(maidTransform.position, Vector3.up, -bg.rotation.eulerAngles.y);
                return;
            }

            if (IsModKeyPressing(keys.bgResetModifier))
            {
                if (Input.GetKey(keys.bgLeftRotate) || Input.GetKey(keys.bgRightRotate))
                {
                    bg.RotateAround(maidTransform.position, Vector3.up, -bg.rotation.eulerAngles.y);
                }
                if (Input.GetKey(keys.bgLeftPitch) || Input.GetKey(keys.bgRightPitch))
                {
                    bg.RotateAround(maidTransform.position, Vector3.forward, -bg.rotation.eulerAngles.z);
                    bg.RotateAround(maidTransform.position, Vector3.right, -bg.rotation.eulerAngles.x);
                }
                if (Input.GetKey(keys.bgLeftMove) || Input.GetKey(keys.bgRightMove) || Input.GetKey(keys.bgBackMove) || Input.GetKey(keys.bgForwardMove))
                {
                    bg.localPosition = new Vector3(0f, bg.localPosition.y, 0f);
                }
                if (Input.GetKey(keys.bgUpMove) || Input.GetKey(keys.bgDownMove))
                {
                    bg.localPosition = new Vector3(bg.localPosition.x, 0f, bg.localPosition.z);
                }
                return;
            }

            Vector3 cameraForward = mainCameraTransform.TransformDirection(Vector3.forward);
            Vector3 cameraRight = mainCameraTransform.TransformDirection(Vector3.right);
            Vector3 cameraUp = mainCameraTransform.TransformDirection(Vector3.up);
            Vector3 direction = Vector3.zero;

            float moveSpeed = cameraConfig.floorMoveSpeed * Time.deltaTime;
            float rotateSpeed = cameraConfig.maidRotateSpeed * Time.deltaTime;
            if (IsModKeyPressing(keys.bgSpeedDownModifier))
            {
                moveSpeed *= 0.1f;
                rotateSpeed *= 0.1f;
            }

            if (Input.GetKey(keys.bgLeftMove))
            {
                direction += new Vector3(cameraRight.x, 0f, cameraRight.z) * moveSpeed;
            }
            if (Input.GetKey(keys.bgRightMove))
            {
                direction += new Vector3(cameraRight.x, 0f, cameraRight.z) * -moveSpeed;
            }
            if (Input.GetKey(keys.bgBackMove))
            {
                direction += new Vector3(cameraForward.x, 0f, cameraForward.z) * moveSpeed;
            }
            if (Input.GetKey(keys.bgForwardMove))
            {
                direction += new Vector3(cameraForward.x, 0f, cameraForward.z) * -moveSpeed;
            }
            if (Input.GetKey(keys.bgUpMove))
            {
                direction += new Vector3(0f, cameraUp.y, 0f) * -moveSpeed; }
            if (Input.GetKey(keys.bgDownMove))
            {
                direction += new Vector3(0f, cameraUp.y, 0f) * moveSpeed;
            }

            //bg.position += direction;
            bg.localPosition += direction;

            if (Input.GetKey(keys.bgLeftRotate))
            {
                bg.RotateAround(maidTransform.transform.position, Vector3.up, rotateSpeed);
            }
            if (Input.GetKey(keys.bgRightRotate))
            {
                bg.RotateAround(maidTransform.transform.position, Vector3.up, -rotateSpeed);
            }
            if (Input.GetKey(keys.bgLeftPitch))
            {
                bg.RotateAround(maidTransform.transform.position, new Vector3(cameraForward.x, 0f, cameraForward.z), rotateSpeed);
            }
            if (Input.GetKey(keys.bgRightPitch))
            {
                bg.RotateAround(maidTransform.transform.position, new Vector3(cameraForward.x, 0f, cameraForward.z), -rotateSpeed);
            }
        }

        private void ChangeEyeToCam()
        {
            Assert.IsNotNull(maid);

            if (eyeToCamIndex == Enum.GetNames(typeof(Maid.EyeMoveType)).Length - 1)
            {
                eyetoCamToggle = false;
                eyeToCamIndex = 0;
            }
            else
            {
                eyeToCamIndex++;
                eyetoCamToggle = true;
            }
            maid.EyeToCamera((Maid.EyeMoveType)eyeToCamIndex, 0f);
            Console.WriteLine("EyeToCam:{0}", eyeToCamIndex);
        }

        private void ToggleEyeToCam()
        {
            Assert.IsNotNull(maid);

            eyetoCamToggle = !eyetoCamToggle;
            if (!eyetoCamToggle)
            {
                maid.EyeToCamera(Maid.EyeMoveType.無し, 0f);
                eyeToCamIndex = 0;
                Console.WriteLine("EyeToCam:{0}", eyeToCamIndex);
            }
            else
            {
                maid.EyeToCamera(Maid.EyeMoveType.目と顔を向ける, 0f);
                eyeToCamIndex = 5;
                Console.WriteLine("EyeToCam:{0}", eyeToCamIndex);
            }
        }

        private void ToggleUIVisible()
        {
            uiVisible = !uiVisible;
            if (uiObject)
            {
                uiObject.SetActive(uiVisible);
                Console.WriteLine("UIVisible:{0}", uiVisible);
            }
        }

        #endregion
        #region Coroutines

        private IEnumerator OVRFirstPersonCameraCoroutine()
        {
            while (!manHead)
            {
                manHead = FindManHead();
                yield return new WaitForSeconds(stateCheckInterval);
            }
            while (true)
            {
                while (!AllowUpdate)
                {
                    yield return new WaitForSeconds(stateCheckInterval);
                }
                if (Input.GetKeyDown(keys.cameraFPSModeToggle))
                {
                    OVRToggleFirstPersonCameraMode();
                }
                yield return null;
            }
        }

        private IEnumerator FirstPersonCameraCoroutine()
        {
            while (!manHead)
            {
                manHead = FindManHead();
                yield return new WaitForSeconds(stateCheckInterval);
            }
            while (true)
            {
                while (!AllowUpdate)
                {
                    yield return new WaitForSeconds(stateCheckInterval);
                }
                if (Input.GetKeyDown(keys.cameraFPSModeToggle))
                {
                    ToggleFirstPersonCameraMode();
                }
                if (fpsMode)
                {
                    UpdateFirstPersonCamera();
                }
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
                UpdateCameraFOV();
                UpdateCameraPitch();
                yield return null;
            }
        }

        private IEnumerator LookAtThisCoroutine()
        {
            while (true)
            {
                if (Input.GetKeyDown(keys.eyetoCamChange))
                {
                    ChangeEyeToCam();
                }
                if (Input.GetKeyDown(keys.eyetoCamToggle))
                {
                    ToggleEyeToCam();
                }
                yield return null;
            }
        }

        private IEnumerator HideUICoroutine()
        {
            while (true)
            {
                if (Input.GetKeyDown(keys.hideUIToggle))
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
        #region Configuration Methods

        private T GetConfig<T>(string sectionName)
        {
            T config = (T)Activator.CreateInstance(typeof(T));
            return GetConfig(sectionName, config);
        }

        private T GetConfig<T>(string sectionName, T config)
        {
            Assert.IsNotNull(sectionName);
            Assert.IsNotNull(config);
            var section = Preferences[sectionName];
            foreach (var field in typeof(T).GetFields())
            {
                var converter = TypeDescriptor.GetConverter(field.FieldType);
                string keyName = field.Name;
                string strValue = section[keyName].RawValue;
                if (strValue == null)
                {
                    var value = field.GetValue(config);
                    section[keyName].Value = converter.ConvertToString(value);
                }
                else
                {
                    try
                    {
                        var value = converter.ConvertFromString(strValue);
                        field.SetValue(config, value);
                    }
                    catch
                    {
                        Debug.LogWarning(string.Format("{0}: Config read error: [{1}]{2}", Name, sectionName, keyName));
                    }
                }
            }
            return config;
        }

        private void SetConfig<T>(string sectionName, T config)
        {
            Assert.IsNotNull(sectionName);
            Assert.IsNotNull(config);
            var section = Preferences[sectionName];
            foreach (var field in typeof(T).GetFields())
            {
                string keyName = field.Name;
                var value = field.GetValue(config);
                var converter = TypeDescriptor.GetConverter(typeof(T));
                section[keyName].Value = converter.ConvertToString(value);
            }
        }

        #endregion
    }

    #region Helper Classes

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
