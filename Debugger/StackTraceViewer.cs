﻿using System;
using System.Diagnostics;
using UnityEngine;

namespace ModTools
{
    public sealed class StackTraceViewer : GUIWindow
    {
        private static Configuration config => ModTools.Instance.config;

        private StackTrace trace;

        private Vector2 scrollPos = Vector2.zero;

        public static StackTraceViewer CreateStackTraceViewer(StackTrace trace)
        {
            var go = new GameObject("StackTraceViewer");
            go.transform.parent = ModTools.Instance.transform;
            StackTraceViewer viewer = go.AddComponent<StackTraceViewer>();
            viewer.trace = trace;
            return viewer;
        }

        private StackTraceViewer() : base("Stack-trace viewer", new Rect(16.0f, 16.0f, 512.0f, 256.0f), skin)
        {
            onDraw = DrawWindow;
            onException = HandleException;
            onClose = HandleClosed;
        }

        private void HandleClosed() => Destroy(this);

        private void HandleException(Exception ex) => Log.Error("Exception in StackTraceViewer - " + ex.Message);

        private void DrawWindow()
        {
            if (trace == null)
            {
                return;
            }

            StackFrame[] stackFrames = trace.GetFrames();
            if (stackFrames == null)
            {
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            int count = 0;
            foreach (StackFrame frame in stackFrames)
            {
                GUILayout.BeginHorizontal(skin.box);
                System.Reflection.MethodBase method = frame.GetMethod();

                GUILayout.Label(count.ToString(), GUILayout.ExpandWidth(false));

                GUI.contentColor = config.nameColor;

                GUILayout.Label(method.ToString(), GUILayout.ExpandWidth(false));

                GUI.contentColor = config.typeColor;

                if (method.DeclaringType != null)
                {
                    GUILayout.Label(" @ " + method.DeclaringType.ToString(), GUILayout.ExpandWidth(false));
                }

                GUI.contentColor = Color.white;

                GUILayout.EndHorizontal();
                count++;
            }

            GUILayout.EndScrollView();
        }
    }
}