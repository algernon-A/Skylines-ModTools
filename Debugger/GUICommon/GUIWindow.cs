﻿using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

namespace ModTools
{
    public class GUIWindow : MonoBehaviour
    {
        private static Configuration config => ModTools.Instance.config;

        public delegate void OnDraw();

        public delegate void OnException(Exception ex);

        public delegate void OnUnityGUI();

        public delegate void OnOpen();

        public delegate void OnClose();

        public delegate void OnResize(Vector2 size);

        public delegate void OnMove(Vector2 position);

        public delegate void OnUnityDestroy();

        public OnDraw onDraw;
        public OnException onException;
        public OnUnityGUI onUnityGUI;
        public OnOpen onOpen;
        public OnClose onClose;
        public OnResize onResize;
        public OnMove onMove;
        public OnUnityDestroy onUnityDestroy;

        public Rect rect = new Rect(0, 0, 64, 64);

        public static GUISkin skin;
        public static Texture2D bgTexture;
        public static Texture2D resizeNormalTexture;
        public static Texture2D resizeHoverTexture;

        public static Texture2D closeNormalTexture;
        public static Texture2D closeHoverTexture;

        public static Texture2D moveNormalTexture;
        public static Texture2D moveHoverTexture;

        public static GUIWindow resizingWindow;
        public static Vector2 resizeDragHandle = Vector2.zero;

        public static GUIWindow movingWindow;
        public static Vector2 moveDragHandle = Vector2.zero;

        public static float uiScale = 1.0f;
        private bool _visible;

        public bool visible
        {
            get => _visible;

            set
            {
                _visible = value;
                GUI.BringWindowToFront(id);
                UpdateClickCatcher();

                if (_visible && onOpen != null)
                {
                    onOpen();
                }
            }
        }

        public bool resizable = true;
        public bool hasCloseButton = true;
        public bool hasTitlebar = true;

        public string title = "Window";

        private readonly int id;

        private Vector2 minSize = Vector2.zero;

        private static readonly List<GUIWindow> windows = new List<GUIWindow>();

        private readonly UIPanel clickCatcher;

        public GUIWindow(string _title, Rect _rect, GUISkin _skin)
        {
            id = UnityEngine.Random.Range(1024, int.MaxValue);
            title = _title;
            rect = _rect;
            skin = _skin;
            minSize = new Vector2(64.0f, 64.0f);
            windows.Add(this);

            UIView uiView = FindObjectOfType<UIView>();
            if (uiView != null)
            {
                clickCatcher = uiView.AddUIComponent(typeof(UIPanel)) as UIPanel;
                if (clickCatcher != null)
                {
                    clickCatcher.name = "_ModToolsInternal";
                }
            }
            UpdateClickCatcher();
        }

        private void UpdateClickCatcher()
        {
            if (clickCatcher == null)
            {
                return;
            }

            //adjust rect from unity pixels to C:S pixels via GetUIView().ratio
            float ratio = UIView.GetAView().ratio;

            clickCatcher.absolutePosition = new Vector3(rect.position.x * ratio, rect.position.y * ratio);
            clickCatcher.size = new Vector2(rect.width * ratio, rect.height * ratio);
            clickCatcher.isVisible = visible;
            clickCatcher.zOrder = int.MaxValue;
        }

        public void OnDestroy()
        {
            onUnityDestroy?.Invoke();

            if (clickCatcher != null)
            {
                Destroy(clickCatcher.gameObject);
            }

            windows.Remove(this);
        }

        public static void UpdateFont()
        {
            skin.font = Font.CreateDynamicFontFromOSFont(config.fontName, config.fontSize);
            ModTools.Instance.sceneExplorer.RecalculateAreas();
        }

        public static void UpdateMouseScrolling()
        {
            Vector3 mouse = Input.mousePosition;
            mouse.y = Screen.height - mouse.y;
            bool mouseInsideGuiWindow = windows.Any(window => window.visible && window.rect.Contains(mouse));
            Util.SetMouseScrolling(!mouseInsideGuiWindow);
        }

        public void OnGUI()
        {
            if (skin == null)
            {
                bgTexture = new Texture2D(1, 1);
                bgTexture.SetPixel(0, 0, config.backgroundColor);
                bgTexture.Apply();

                resizeNormalTexture = new Texture2D(1, 1);
                resizeNormalTexture.SetPixel(0, 0, Color.white);
                resizeNormalTexture.Apply();

                resizeHoverTexture = new Texture2D(1, 1);
                resizeHoverTexture.SetPixel(0, 0, Color.blue);
                resizeHoverTexture.Apply();

                closeNormalTexture = new Texture2D(1, 1);
                closeNormalTexture.SetPixel(0, 0, Color.red);
                closeNormalTexture.Apply();

                closeHoverTexture = new Texture2D(1, 1);
                closeHoverTexture.SetPixel(0, 0, Color.white);
                closeHoverTexture.Apply();

                moveNormalTexture = new Texture2D(1, 1);
                moveNormalTexture.SetPixel(0, 0, config.titlebarColor);
                moveNormalTexture.Apply();

                moveHoverTexture = new Texture2D(1, 1);
                moveHoverTexture.SetPixel(0, 0, config.titlebarColor * 1.2f);
                moveHoverTexture.Apply();

                skin = ScriptableObject.CreateInstance<GUISkin>();
                skin.box = new GUIStyle(GUI.skin.box);
                skin.button = new GUIStyle(GUI.skin.button);
                skin.horizontalScrollbar = new GUIStyle(GUI.skin.horizontalScrollbar);
                skin.horizontalScrollbarLeftButton = new GUIStyle(GUI.skin.horizontalScrollbarLeftButton);
                skin.horizontalScrollbarRightButton = new GUIStyle(GUI.skin.horizontalScrollbarRightButton);
                skin.horizontalScrollbarThumb = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
                skin.horizontalSlider = new GUIStyle(GUI.skin.horizontalSlider);
                skin.horizontalSliderThumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
                skin.label = new GUIStyle(GUI.skin.label);
                skin.scrollView = new GUIStyle(GUI.skin.scrollView);
                skin.textArea = new GUIStyle(GUI.skin.textArea);
                skin.textField = new GUIStyle(GUI.skin.textField);
                skin.toggle = new GUIStyle(GUI.skin.toggle);
                skin.verticalScrollbar = new GUIStyle(GUI.skin.verticalScrollbar);
                skin.verticalScrollbarDownButton = new GUIStyle(GUI.skin.verticalScrollbarDownButton);
                skin.verticalScrollbarThumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);
                skin.verticalScrollbarUpButton = new GUIStyle(GUI.skin.verticalScrollbarUpButton);
                skin.verticalSlider = new GUIStyle(GUI.skin.verticalSlider);
                skin.verticalSliderThumb = new GUIStyle(GUI.skin.verticalSliderThumb);
                skin.window = new GUIStyle(GUI.skin.window);
                skin.window.normal.background = bgTexture;
                skin.window.onNormal.background = bgTexture;

                skin.settings.cursorColor = GUI.skin.settings.cursorColor;
                skin.settings.cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
                skin.settings.doubleClickSelectsWord = GUI.skin.settings.doubleClickSelectsWord;
                skin.settings.selectionColor = GUI.skin.settings.selectionColor;
                skin.settings.tripleClickSelectsLine = GUI.skin.settings.tripleClickSelectsLine;

                UpdateFont();
            }

            if (visible)
            {
                GUISkin oldSkin = GUI.skin;
                if (skin != null)
                {
                    GUI.skin = skin;
                }

                Matrix4x4 matrix = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(new Vector3(uiScale, uiScale, uiScale));

                rect = GUI.Window(id, rect, i =>
                {
                    if (onDraw != null)
                    {
                        GUILayout.Space(8.0f);

                        try
                        {
                            onDraw();
                        }
                        catch (Exception ex)
                        {
                            if (onException != null)
                            {
                                onException(ex);
                            }
                            else
                            {
                                throw;
                            }
                        }

                        GUILayout.Space(16.0f);

                        Vector3 mouse = Input.mousePosition;
                        mouse.y = Screen.height - mouse.y;

                        DrawBorder();

                        if (hasTitlebar)
                        {
                            DrawTitlebar(mouse);
                        }

                        if (hasCloseButton)
                        {
                            DrawCloseButton(mouse);
                        }

                        if (resizable)
                        {
                            DrawResizeHandle(mouse);
                        }
                    }
                }, "");

                onUnityGUI?.Invoke();

                GUI.matrix = matrix;

                GUI.skin = oldSkin;
            }
        }

        private void DrawBorder()
        {
            var leftRect = new Rect(0.0f, 0.0f, 1.0f, rect.height);
            var rightRect = new Rect(rect.width - 1.0f, 0.0f, 1.0f, rect.height);
            var bottomRect = new Rect(0.0f, rect.height - 1.0f, rect.width, 1.0f);
            GUI.DrawTexture(leftRect, moveNormalTexture);
            GUI.DrawTexture(rightRect, moveNormalTexture);
            GUI.DrawTexture(bottomRect, moveNormalTexture);
        }

        private void DrawTitlebar(Vector3 mouse)
        {
            var moveRect = new Rect(rect.x * uiScale, rect.y * uiScale, rect.width * uiScale, 20.0f);
            Texture2D moveTex = moveNormalTexture;

            if (movingWindow != null)
            {
                if (movingWindow == this)
                {
                    moveTex = moveHoverTexture;

                    if (Input.GetMouseButton(0))
                    {
                        Vector2 pos = new Vector2(mouse.x, mouse.y) + moveDragHandle;
                        rect.x = pos.x;
                        rect.y = pos.y;
                        if (rect.x < 0.0f)
                        {
                            rect.x = 0.0f;
                        }
                        if (rect.x + rect.width > Screen.width)
                        {
                            rect.x = Screen.width - rect.width;
                        }

                        if (rect.y < 0.0f)
                        {
                            rect.y = 0.0f;
                        }
                        if (rect.y + rect.height > Screen.height)
                        {
                            rect.y = Screen.height - rect.height;
                        }
                    }
                    else
                    {
                        movingWindow = null;
                        ModTools.Instance.SaveConfig();

                        UpdateClickCatcher();

                        onMove?.Invoke(rect.position);
                    }
                }
            }
            else if (moveRect.Contains(mouse))
            {
                moveTex = moveHoverTexture;
                if (Input.GetMouseButton(0) && resizingWindow == null)
                {
                    movingWindow = this;
                    moveDragHandle = new Vector2(rect.x, rect.y) - new Vector2(mouse.x, mouse.y);
                }
            }

            GUI.DrawTexture(new Rect(0.0f, 0.0f, rect.width * uiScale, 20.0f), moveTex, ScaleMode.StretchToFill);
            GUI.contentColor = config.titlebarTextColor;
            GUI.Label(new Rect(0.0f, 0.0f, rect.width * uiScale, 20.0f), title);
            GUI.contentColor = Color.white;
        }

        private void DrawCloseButton(Vector3 mouse)
        {
            var closeRect = new Rect(rect.x * uiScale + rect.width * uiScale - 20.0f, rect.y * uiScale, 16.0f, 8.0f);
            Texture2D closeTex = closeNormalTexture;

            if (closeRect.Contains(mouse))
            {
                closeTex = closeHoverTexture;

                if (Input.GetMouseButton(0))
                {
                    resizingWindow = null;
                    movingWindow = null;
                    visible = false;
                    ModTools.Instance.SaveConfig();

                    UpdateClickCatcher();

                    onClose?.Invoke();
                }
            }

            GUI.DrawTexture(new Rect(rect.width - 20.0f, 0.0f, 16.0f, 8.0f), closeTex, ScaleMode.StretchToFill);
        }

        private void DrawResizeHandle(Vector3 mouse)
        {
            var resizeRect = new Rect(rect.x * uiScale + rect.width * uiScale - 16.0f, rect.y * uiScale + rect.height * uiScale - 8.0f, 16.0f, 8.0f);
            Texture2D resizeTex = resizeNormalTexture;

            if (resizingWindow != null)
            {
                if (resizingWindow == this)
                {
                    resizeTex = resizeHoverTexture;

                    if (Input.GetMouseButton(0))
                    {
                        Vector2 size = new Vector2(mouse.x, mouse.y) + resizeDragHandle - new Vector2(rect.x, rect.y);

                        if (size.x < minSize.x)
                        {
                            size.x = minSize.x;
                        }

                        if (size.y < minSize.y)
                        {
                            size.y = minSize.y;
                        }

                        rect.width = size.x;
                        rect.height = size.y;

                        if (rect.x + rect.width >= Screen.width)
                        {
                            rect.width = Screen.width - rect.x;
                        }

                        if (rect.y + rect.height >= Screen.height)
                        {
                            rect.height = Screen.height - rect.y;
                        }
                    }
                    else
                    {
                        resizingWindow = null;
                        ModTools.Instance.SaveConfig();

                        UpdateClickCatcher();

                        onResize?.Invoke(rect.size);
                    }
                }
            }
            else if (resizeRect.Contains(mouse))
            {
                resizeTex = resizeHoverTexture;
                if (Input.GetMouseButton(0) && movingWindow == null)
                {
                    resizingWindow = this;
                    resizeDragHandle = new Vector2(rect.x + rect.width, rect.y + rect.height) - new Vector2(mouse.x, mouse.y);
                }
            }

            GUI.DrawTexture(new Rect(rect.width - 16.0f, rect.height - 8.0f, 16.0f, 8.0f), resizeTex, ScaleMode.StretchToFill);
        }
    }
}