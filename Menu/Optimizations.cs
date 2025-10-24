using System.Collections;
using System.Collections.Generic;
using NXO.Utilities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static NXO.Utilities.ColorLib;

namespace NXO.Menu
{
    public class Optimizations
    {
        public static class ButtonPool
        {
            private static List<GameObject> buttonPool = new List<GameObject>();
            private static int currentIndex = 0;

            public static GameObject GetButton()
            {
                if (currentIndex < buttonPool.Count)
                {
                    GameObject button = buttonPool[currentIndex];
                    if (button == null)
                    {
                        button = CreateNewButton();
                        buttonPool[currentIndex] = button;
                    }
                    button.SetActive(true);
                    currentIndex++;

                    return button;
                }
                else
                {
                    GameObject newButton = CreateNewButton();
                    buttonPool.Add(newButton);
                    currentIndex++;
                    return newButton;
                }
            }

            public static void ResetPool()
            {
                currentIndex = 0;
                foreach (GameObject obj in buttonPool)
                {
                    if (obj != null)
                    {
                        obj.SetActive(false);

                        var renderer = obj.GetComponent<Renderer>();
                        if (renderer != null)
                            renderer.enabled = true; 
                    }
                }
            }

            private static GameObject CreateNewButton()
            {
                GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
                button.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                return button;
            }
        }

        public static class TextPool
        {
            private static List<GameObject> textPool = new List<GameObject>();
            private static int currentIndex = 0;

            public static GameObject GetTextObject()
            {
                if (currentIndex < textPool.Count)
                {
                    GameObject textObj = textPool[currentIndex];
                    if (textObj == null)
                    {
                        textObj = CreateNewTextObject();
                        textPool[currentIndex] = textObj;
                    }
                    textObj.SetActive(true);
                    currentIndex++;
                    return textObj;
                }
                else
                {
                    GameObject newTextObj = CreateNewTextObject();
                    textPool.Add(newTextObj);
                    currentIndex++;
                    return newTextObj;
                }
            }

            public static void ResetPool()
            {
                currentIndex = 0;
                foreach (GameObject textObj in textPool)
                {
                    if (textObj != null)
                        textObj.SetActive(false);
                }
            }

            private static GameObject CreateNewTextObject()
            {
                GameObject textObj = new GameObject("PooledText");
                textObj.hideFlags = HideFlags.HideAndDontSave;

                Text text = textObj.AddComponent<Text>();
                text.font = Variables.ArialFont;
                text.fontStyle = FontStyle.Normal;
                text.color = White;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;

                return textObj;
            }
        }
    }
}
