using BepInEx;
using HarmonyLib;
using NXO.Mods;
using NXO.Utilities;
using Photon.Pun;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static NXO.Initialization.PluginInfo;
using static NXO.Menu.ButtonHandler;
using static NXO.Menu.Optimizations;
using static NXO.Utilities.ColorLib;
using static NXO.Utilities.InputHandler;
using static NXO.Utilities.Variables;

namespace NXO.Menu
{
    [HarmonyPatch(typeof(GorillaLocomotion.GTPlayer), "LateUpdate")]
    public class Main : MonoBehaviour
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            try
            {
                if (playerInstance == null || taggerInstance == null)
                {
                    Debug.LogError("Player instance or GorillaTagger is null. Skipping menu updates.");
                    return;
                }

                foreach (var button in ModButtons.buttons)
                {
                    if (button.Enabled && button.onEnable != null)
                    {
                        try { button.onEnable.Invoke(); }
                        catch (Exception ex) { Debug.LogError($"Error invoking button action: {button.buttonText}. {ex}"); }
                    }
                }

                if (UnityInput.Current.GetKeyDown(PCMenuKey))
                    PCMenuOpen = !PCMenuOpen;

                HandleMenuInteraction();

                try
                {
                    if (!taggerInstance.offlineVRRig.enabled)
                    {
                        if (vrrig == null)
                        {
                            vrrig = UnityEngine.Object.Instantiate(taggerInstance.offlineVRRig, playerInstance.transform.position, playerInstance.transform.rotation);
                            vrrig.headBodyOffset = Vector3.zero;
                            vrrig.enabled = true;

                            vrrig.transform.Find("VR Constraints/LeftArm/Left Arm IK/SlideAudio").gameObject.SetActive(false);
                            vrrig.transform.Find("VR Constraints/RightArm/Right Arm IK/SlideAudio").gameObject.SetActive(false);
                            vrrig.transform.Find("RigAnchor/rig/bodySlideAudio").gameObject.SetActive(false);
                        }

                        if (vrrigMaterial == null)
                            vrrigMaterial = new Material(guiShader);

                        vrrigMaterial.color = GreyTransparent;
                        vrrig.mainSkin.material = vrrigMaterial;

                        vrrig.headConstraint.transform.position = playerInstance.headCollider.transform.position;
                        vrrig.headConstraint.transform.rotation = playerInstance.headCollider.transform.rotation;

                        vrrig.leftHandTransform.position = playerInstance.leftControllerTransform.position;
                        vrrig.leftHandTransform.rotation = playerInstance.leftControllerTransform.rotation;

                        vrrig.rightHandTransform.position = playerInstance.rightControllerTransform.position;
                        vrrig.rightHandTransform.rotation = playerInstance.rightControllerTransform.rotation;

                        vrrig.transform.position = playerInstance.transform.position;
                        vrrig.transform.rotation = playerInstance.transform.rotation;
                    }
                    else
                    {
                        if (vrrig != null)
                        {
                            vrrig.Destroy();
                            vrrig = null;
                        }
                        if (vrrigMaterial != null)
                        {
                            vrrigMaterial.Destroy();
                            vrrigMaterial = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Error updating ghost rig. Exception: {ex}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in Prefix!: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }

        public void Start()
        {
            taggerInstance = GorillaTagger.Instance;
            playerInstance = GorillaLocomotion.GTPlayer.Instance;
            thirdPersonCamera = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
            cm = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera/CM vcam1");
        }

        public static void HandleMenuInteraction()
        {
            try
            {
                // PC Menu
                if (PCMenuOpen && !InMenuCondition && !LPrimary() && !RPrimary() && !menuOpen)
                {
                    InPcCondition = true;
                    cm?.SetActive(false);

                    if (menuObj == null)
                    {
                        Draw();
                    }
                    else
                    {
                        AddButtonClicker(thirdPersonCamera?.transform);
                        AddTitleAndFPSCounter();

                        if (thirdPersonCamera != null)
                        {
                            var cam = thirdPersonCamera.transform;
                            cam.position = new Vector3(-65.8373f, 21.6568f, -80.9763f);
                            cam.rotation = Quaternion.identity;
                            menuObj.transform.SetParent(cam, true);
                            menuObj.transform.position = cam.position + cam.rotation * Vector3.forward * 0.65f;
                            menuObj.transform.rotation = Quaternion.LookRotation(cam.position - menuObj.transform.position, Vector3.up);
                            menuObj.transform.Rotate(Vector3.up, -90f);
                            menuObj.transform.Rotate(Vector3.right, -90f);

                            try
                            {
                                if (Mouse.current.leftButton.isPressed)
                                {
                                    var ray = thirdPersonCamera.GetComponent<Camera>().ScreenPointToRay(Mouse.current.position.ReadValue());
                                    if (Physics.Raycast(ray, out RaycastHit hit))
                                    {
                                        var btnCollider = hit.collider?.GetComponent<BtnCollider>();
                                        if (btnCollider != null && clickerObj != null)
                                            btnCollider.OnTriggerEnter(clickerObj.GetComponent<Collider>());
                                    }
                                }
                                else if (clickerObj != null)
                                {
                                    UnityEngine.Object.Destroy(clickerObj);
                                    clickerObj = null;
                                }
                            }
                            catch (Exception ex) { Debug.LogError($"Error handling mouse click: {ex}"); }
                        }
                    }
                }
                else if (!PCMenuOpen && InPcCondition && menuObj != null)
                {
                    InPcCondition = false;
                    cm?.SetActive(true);

                    FullCleanupMenu();
                }

                openMenu = rightHandedMenu ? RSecondary() : LSecondary();

                // Controller Menu 
                if (openMenu && !InPcCondition)
                {
                    InMenuCondition = true;
                    if (menuObj == null)
                    {
                        Draw();
                        AddButtonClicker(rightHandedMenu ? playerInstance.leftControllerTransform : playerInstance.rightControllerTransform);
                    }
                    else
                    {
                        PositionMenuForHand();
                        AddTitleAndFPSCounter();
                    }
                }
                else if (menuObj != null && InMenuCondition)
                {
                    InMenuCondition = false;

                    FullCleanupMenu();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error handling menu interaction. Exception: {ex}");
            }

            GameObject GetHand() => rightHandedMenu ? taggerInstance.offlineVRRig.rightHandPlayer.gameObject : taggerInstance.offlineVRRig.leftHandPlayer.gameObject;
        }

        public static void Draw()
        {
            CreateBaseMenuObject();
            CreateMenuBackground();
            CreateMenuCanvasAndTitle();
            AddTopMenuButton();
            AddReturnButton();
            AddPageButton(">");
            AddPageButton("<");

            ButtonPool.ResetPool();

            var PageToDraw = GetButtonInfoByPage(currentPage).Skip(currentCategoryPage * ButtonsPerPage).Take(ButtonsPerPage).ToArray();
            for (int i = 0; i < PageToDraw.Length; i++)
                AddModButtons(i * ButtonSpacing, PageToDraw[i]);
        }

        private static void CreateBaseMenuObject()
        {
            menuObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            menuObj.GetComponent<Rigidbody>().Destroy();
            menuObj.GetComponent<BoxCollider>().Destroy();
            menuObj.GetComponent<Renderer>().Destroy();
            menuObj.name = "menu";
            menuObj.transform.localScale = new Vector3(0.1f, 0.3f, 0.45f);
        }

        private static void CreateMenuBackground()
        {
            background = GameObject.CreatePrimitive(PrimitiveType.Cube);
            background.GetComponent<Rigidbody>().Destroy();
            background.GetComponent<BoxCollider>().Destroy();
            background.transform.localRotation = Quaternion.identity;
            background.transform.parent = menuObj.transform;
            background.name = "menucolor";

            background.transform.localScale = new Vector3(0.01f, 0.9f, 0.9f);
            background.transform.position = new Vector3(0.05f, 0, 0.025f);

            background.GetComponent<MeshRenderer>().material.color = Black;
        }

        public static void AddTopMenuButton()
        {
            DisconnectButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject.Destroy(DisconnectButton.GetComponent<Rigidbody>());
            DisconnectButton.GetComponent<BoxCollider>().isTrigger = true;
            DisconnectButton.transform.parent = menuObj.transform;
            DisconnectButton.transform.rotation = Quaternion.identity;
            DisconnectButton.transform.localScale = new Vector3(0.005f, 0.8625f, 0.05875f);
            DisconnectButton.transform.localPosition = new Vector3(0.5f, 0f, 0.55f);

            DisconnectButton.GetComponent<Renderer>().material.color = Black;

            Text discText = new GameObject { transform = { parent = canvasObj.transform } }.AddComponent<Text>();
            discText.font = ArialFont;
            discText.fontStyle = FontStyle.Normal;
            discText.fontSize = 3;
            discText.color = White;
            discText.alignment = TextAnchor.MiddleCenter;
            discText.resizeTextForBestFit = true;
            discText.resizeTextMinSize = 0;

            RectTransform rectt = discText.GetComponent<RectTransform>();
            rectt.sizeDelta = new Vector2(0.2f, 0.02f);
            rectt.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            rectt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            rectt.localPosition = new Vector3(0.051f, 0f, 0.2475f);

            discText.text = "Disconnect";
            DisconnectButton.AddComponent<BtnCollider>().clickedButton = new ButtonHandler.Button("DisconnectButton", Category.Home, false, false, PhotonNetwork.Disconnect);
        }
        
        public static void AddTitleAndFPSCounter()
        {
            if (Time.time - lastFPSTime >= 1f)
            {
                fps = Mathf.CeilToInt(1f / Time.smoothDeltaTime);
                lastFPSTime = Time.time;
            }

            string pageName = currentPage.ToString().Replace("_", " ");

            title.text = $"<size=1>{menuName} v{menuVersion}</size>\n{pageName}: {currentCategoryPage + 1} | FPS: {fps}";
        }

        private static void CreateMenuCanvasAndTitle()
        {
            canvasObj = new GameObject();
            canvasObj.transform.parent = menuObj.transform;
            canvasObj.name = "canvas";
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            CanvasScaler canvasScale = canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 100;
            canvasScale.dynamicPixelsPerUnit = 1000;

            GameObject titleObj = new GameObject();
            titleObj.transform.parent = canvasObj.transform;
            titleObj.transform.localScale = new Vector3(1f, 1f, 1f);
            title = titleObj.AddComponent<Text>();
            title.font = ArialFont;
            title.fontStyle = FontStyle.Normal;
            title.color = White;
            title.supportRichText = true;
            title.fontSize = 5;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.position = new Vector3(0.051f, 0f, 0.2f);
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            titleTransform.sizeDelta = new Vector2(0.26f, 0.053f);
        }

        public static void AddModButtons(float offset, ButtonHandler.Button button)
        {
            GameObject ModBtns = ButtonPool.GetButton();
            ModBtns.transform.SetParent(menuObj.transform, false);
            ModBtns.transform.rotation = Quaternion.identity;
            ModBtns.transform.localScale = new Vector3(0.005f, 0.82f, 0.08f);

            ModBtns.transform.localPosition = new Vector3(0.5075f, 0f, 0.3325f - offset);
            BtnCollider btnColScript = ModBtns.GetComponent<BtnCollider>() ?? ModBtns.AddComponent<BtnCollider>();
            btnColScript.clickedButton = button;

            Rigidbody btnRigidbody = ModBtns.GetComponent<Rigidbody>();
            if (btnRigidbody != null)
                Destroy(btnRigidbody);

            BoxCollider btnCollider = ModBtns.GetComponent<BoxCollider>();
            if (btnCollider != null)
                btnCollider.isTrigger = true;

            Renderer btnRenderer = ModBtns.GetComponent<Renderer>();
            if (btnRenderer != null)
            {
                if (button.Enabled)
                    btnRenderer.material.color = DarkerGrey;
                else
                    btnRenderer.material.color = SuperDarkGrey;
            }

            button.GameObject = ModBtns;

            GameObject titleObj = TextPool.GetTextObject();
            titleObj.transform.SetParent(canvasObj.transform, false);
            titleObj.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            Text title = titleObj.GetComponent<Text>();
            title.text = button.buttonText;
            title.font = ArialFont;
            title.fontStyle = FontStyle.Normal;
            title.color = White;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = new Vector3(0.05125f, 0f, 0.15f - offset / 2.225f);
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            titleTransform.sizeDelta = new Vector2(0.22f, 0.01725f);
        }

        public static void AddPageButton(string button)
        {
            GameObject PageButtons = GameObject.CreatePrimitive(PrimitiveType.Cube);
            PageButtons.GetComponent<Rigidbody>().Destroy();
            PageButtons.GetComponent<BoxCollider>().isTrigger = true;
            PageButtons.transform.parent = menuObj.transform;
            PageButtons.transform.rotation = Quaternion.identity;
            PageButtons.transform.localScale = new Vector3(0.005f, 0.25f, 0.08f);
            PageButtons.transform.localPosition = new Vector3(0.5075f, button.Contains("<") ? 0.285f : -0.285f, -0.33f);
            PageButtons.AddComponent<BtnCollider>().clickedButton = new ButtonHandler.Button(button, Category.Home, false, false, null, null);
            PageButtons.GetComponent<Renderer>().material.color = SuperDarkGrey;

            GameObject titleObj = new GameObject();
            titleObj.transform.parent = canvasObj.transform;
            titleObj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            Text title = titleObj.AddComponent<Text>();
            title.font = ArialFont;
            title.color = White;
            title.fontSize = 3;
            title.fontStyle = FontStyle.Normal;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.2f, 0.03f);
            title.text = button.Contains("<") ? "<" : ">";
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            titleTransform.position = new Vector3(0.05125f, button.Contains("<") ? 0.0875f : -0.0875f, -0.1475f);
        }

        public static void AddReturnButton()
        {
            BackToStartButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BackToStartButton.GetComponent<Rigidbody>().Destroy();
            BackToStartButton.GetComponent<BoxCollider>().isTrigger = true;
            BackToStartButton.transform.parent = menuObj.transform;
            BackToStartButton.transform.rotation = Quaternion.identity;
            BackToStartButton.transform.localScale = new Vector3(0.005f, 0.2925f, 0.08f);
            BackToStartButton.transform.localPosition = new Vector3(0.5075f, 0f, -0.33f);
            BackToStartButton.AddComponent<BtnCollider>().clickedButton = new ButtonHandler.Button("ReturnButton", Category.Home, false, false, null, null);
            BackToStartButton.GetComponent<Renderer>().material.color = SuperDarkGrey;

            GameObject titleObj = new GameObject();
            titleObj.transform.parent = canvasObj.transform;
            titleObj.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            titleObj.transform.localPosition = new Vector3(0.85f, 0.85f, 0.85f);
            Text title = titleObj.AddComponent<Text>();
            title.font = ArialFont;
            title.fontStyle = FontStyle.Normal;
            title.text = "Home";
            title.color = White;
            title.fontSize = 3;
            title.alignment = TextAnchor.MiddleCenter;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 0;
            RectTransform titleTransform = title.GetComponent<RectTransform>();
            titleTransform.localPosition = Vector3.zero;
            titleTransform.sizeDelta = new Vector2(0.2f, 0.02f);
            titleTransform.localPosition = new Vector3(0.05125f, 0f, -0.1475f);
            titleTransform.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }

        public static void AddButtonClicker(Transform parentTransform)
        {
            if (clickerObj != null) return;

            clickerObj = new GameObject("buttonclicker");

            var clickerCollider = clickerObj.AddComponent<BoxCollider>();
            clickerCollider.isTrigger = true;

            var meshFilter = clickerObj.AddComponent<MeshFilter>();
            meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");

            var clickerRenderer = clickerObj.AddComponent<MeshRenderer>();
            clickerRenderer.material.color = Color.white;
            clickerRenderer.material.shader = guiShader;

            if (parentTransform != null)
            {
                clickerObj.transform.SetParent(parentTransform);
                clickerObj.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                clickerObj.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            }
        }

        private static void PositionMenuForHand()
        {
            if (rightHandedMenu)
            {
                menuObj.transform.position = playerInstance.rightControllerTransform.position;
                Vector3 rotation = playerInstance.rightControllerTransform.rotation.eulerAngles;
                rotation += new Vector3(0f, 0f, 180f);
                menuObj.transform.rotation = Quaternion.Euler(rotation);
            }
            else
            {
                menuObj.transform.position = playerInstance.leftControllerTransform.position;
                menuObj.transform.rotation = playerInstance.leftControllerTransform.rotation;
            }
        }

        public static void FullCleanupMenu()
        {
            ButtonPool.ResetPool();

            if (background != null) { UnityEngine.Object.Destroy(background); background = null; }
            if (DisconnectButton != null) { UnityEngine.Object.Destroy(DisconnectButton); DisconnectButton = null; }
            if (canvasObj != null) { UnityEngine.Object.Destroy(canvasObj); canvasObj = null; }
            if (BackToStartButton != null) { UnityEngine.Object.Destroy(BackToStartButton); BackToStartButton = null; }
            if (menuObj != null) { UnityEngine.Object.Destroy(menuObj); menuObj = null; }
        }

        public static void RefreshMenu()
        {
            FullCleanupMenu();
            Draw();
        }
    }
}