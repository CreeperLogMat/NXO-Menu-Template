using HarmonyLib;
using NXO.Mods;
using Photon.Pun;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace NXO.Utilities
{
    public class Variables
    {
        // --- Menu & Interaction Variables ---
        public static GameObject menuObj, background, canvasObj, clickerObj, BackToStartButton, DisconnectButton;
        public static Font ArialFont = UnityEngine.Resources.GetBuiltinResource<Font>("Arial.ttf");
        public static Text title;

        public static Category currentPage = Category.Home;
        public static int currentCategoryPage = 0;
        public static int ButtonsPerPage = 7;
        public static float ButtonSpacing = 0.094f;

        public static bool toggledisconnectButton = true;
        public static bool rightHandedMenu = false;
        public static bool toggleNotifications = true;
        public static bool notificationSent = false;
        public static bool PCMenuOpen = false;
        public static KeyCode PCMenuKey = KeyCode.LeftAlt;
        public static bool openMenu, menuOpen, InMenuCondition, InPcCondition;

        public static int fps;
        public static float lastFPSTime = 0f;

        // --- Player and Movement Variables ---
        public static GorillaLocomotion.GTPlayer playerInstance;
        public static GorillaTagger taggerInstance;
        public static VRRig vrrig;
        public static Material vrrigMaterial;

        // --- Camera Variables ---
        public static GameObject thirdPersonCamera, shoulderCamera, TransformCam, cm;
        public static bool didThirdPerson = false;

        // --- Shaders ---
        public static Shader guiShader = Shader.Find("GUI/Text Shader");
        public static Shader uberShader = Shader.Find("GorillaTag/UberShader");
        public static Shader uiShader = Shader.Find("UI/Default");
        public static Shader shinyShader = Shader.Find("Universal Render Pipeline/Lit");
    }
}
