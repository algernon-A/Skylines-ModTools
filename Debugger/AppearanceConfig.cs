using System;
using ModTools.UI;
using UnityEngine;

namespace ModTools
{
    internal sealed class AppearanceConfig : GUIWindow, IAwakingObject
    {
        private string[] availableFonts;
        private int selectedFont;

        public AppearanceConfig()
            : base("Appearance configuration", new Rect(16.0f, 16.0f, 600.0f, 490.0f), resizable: false)
        {
        }

        public void Awake() {
            availableFonts = Font.GetOSInstalledFontNames();
            selectedFont = Array.IndexOf(availableFonts, MainWindow.Instance.Config.FontName);
        }

        protected override void DrawWindow()
        {
            var config = MainWindow.Instance.Config;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Font");
            GUILayout.FlexibleSpace();

            var newSelectedFont = GUIComboBox.Box(selectedFont, availableFonts, "AppearanceSettingsFonts");
            if (newSelectedFont != selectedFont && newSelectedFont >= 0)
            {
                config.FontName = availableFonts[newSelectedFont];
                selectedFont = newSelectedFont;
                UpdateFont();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            GUILayout.Label("Font size");

            var newFontSize = (int)GUILayout.HorizontalSlider(config.FontSize, 13.0f, 39.0f, GUILayout.Width(256));

            if (newFontSize != config.FontSize)
            {
                config.FontSize = newFontSize;
                UpdateFont();
            }

            GUILayout.EndHorizontal();

            var newColor = DrawColorControl("Background", config.BackgroundColor);
            if (newColor != config.BackgroundColor)
            {
                config.BackgroundColor = newColor;
                BgTexture.SetPixel(0, 0, config.BackgroundColor);
                BgTexture.Apply();
            }

            newColor = DrawColorControl("Title bar", config.TitleBarColor);
            if (newColor != config.TitleBarColor)
            {
                config.TitleBarColor = newColor;
                MoveNormalTexture.SetPixel(0, 0, config.TitleBarColor);
                MoveNormalTexture.Apply();

                MoveHoverTexture.SetPixel(0, 0, config.TitleBarColor * 1.2f);
                MoveHoverTexture.Apply();
            }

            config.TitleBarTextColor = DrawColorControl("Title bar text", config.TitleBarTextColor);

            config.GameObjectColor = DrawColorControl("GameObject", config.GameObjectColor);
            config.EnabledComponentColor = DrawColorControl("Component (enabled)", config.EnabledComponentColor);
            config.DisabledComponentColor = DrawColorControl("Component (disabled)", config.DisabledComponentColor);
            config.SelectedComponentColor = DrawColorControl("Selected component", config.SelectedComponentColor);
            config.KeywordColor = DrawColorControl("Keyword", config.KeywordColor);
            config.NameColor = DrawColorControl("Member name", config.NameColor);
            config.TypeColor = DrawColorControl("Member type", config.TypeColor);
            config.ModifierColor = DrawColorControl("Member modifier", config.ModifierColor);
            config.MemberTypeColor = DrawColorControl("Field type", config.MemberTypeColor);
            config.ValueColor = DrawColorControl("Member value", config.ValueColor);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("OK", GUILayout.Width(100)))
            {
                MainWindow.Instance.SaveConfig();
                Visible = false;
            }

            if (GUILayout.Button("Defaults", GUILayout.Width(100)))
            {
                ResetDoDefault(config);
                selectedFont = Array.IndexOf(availableFonts, config.FontName);
            }

            GUILayout.EndHorizontal();
        }

        protected override void HandleException(Exception ex)
        {
            Logger.Error("Exception in AppearanceConfig - " + ex.Message);
            Visible = false;
        }

        private static void ResetDoDefault(ModConfiguration config)
        {
            var template = new ModConfiguration();

            config.BackgroundColor = template.BackgroundColor;
            BgTexture.SetPixel(0, 0, config.BackgroundColor);
            BgTexture.Apply();

            config.TitleBarColor = template.TitleBarColor;
            MoveNormalTexture.SetPixel(0, 0, config.TitleBarColor);
            MoveNormalTexture.Apply();

            MoveHoverTexture.SetPixel(0, 0, config.TitleBarColor * 1.2f);
            MoveHoverTexture.Apply();

            config.TitleBarTextColor = template.TitleBarTextColor;

            config.GameObjectColor = template.GameObjectColor;
            config.EnabledComponentColor = template.EnabledComponentColor;
            config.DisabledComponentColor = template.DisabledComponentColor;
            config.SelectedComponentColor = template.SelectedComponentColor;
            config.NameColor = template.NameColor;
            config.TypeColor = template.TypeColor;
            config.ModifierColor = template.ModifierColor;
            config.MemberTypeColor = template.MemberTypeColor;
            config.ValueColor = template.ValueColor;
            config.FontName = template.FontName;
            config.FontSize = template.FontSize;
        }

        private Color DrawColorControl(string name, Color value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name);
            GUILayout.FlexibleSpace();
            var newColor = GUIControls.CustomValueField(name, string.Empty, GUIControls.PresentColor, value);
            GUILayout.EndHorizontal();
            return newColor;
        }
    }
}