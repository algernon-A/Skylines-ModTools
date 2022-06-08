using System;
using System.Reflection;
using ModTools.Utils;
using ObjUnity3D;
using UnityEngine;

namespace ModTools.Explorer
{
    internal static class GUIReflect
    {
        public static void OnSceneTreeReflect(SceneExplorerState state, ReferenceChain refChain, object obj, bool rawReflection, TypeUtil.SmartType smartType = TypeUtil.SmartType.Undefined, string filter = "")
        {
            if (!SceneExplorerCommon.SceneTreeCheckDepth(refChain))
            {
                return;
            }

            if (obj == null)
            {
                SceneExplorerCommon.OnSceneTreeMessage(refChain, "null");
                return;
            }

            var type = obj.GetType();

            if (!rawReflection)
            {
                if (!type.IsValueType && state.PreventCircularReferences.Contains(obj))
                {
                    try
                    {
                        GUI.contentColor = Color.yellow;
                        SceneExplorerCommon.OnSceneTreeMessage(refChain, "Circular reference detected");
                    }
                    finally
                    {
                        GUI.contentColor = Color.white;
                    }

                    return;
                }

                state.PreventCircularReferences.Add(obj);

                if (type == typeof(Transform))
                {
                    GUITransform.OnSceneTreeReflectUnityEngineTransform(refChain, (Transform)obj);
                    return;
                }

                if (TypeUtil.IsList(obj))
                {
                    GUIList.OnSceneTreeReflectIList(state, refChain, obj, smartType);
                    return;
                }

                if (TypeUtil.IsCollection(obj))
                {
                    GUICollection.OnSceneTreeReflectICollection(state, refChain, obj, smartType);
                    return;
                }

                if (TypeUtil.IsEnumerable(obj))
                {
                    GUIEnumerable.OnSceneTreeReflectIEnumerable(state, refChain, obj, smartType);
                    return;
                }

                if (type == typeof(Material))
                {
                    GUIMaterial.OnSceneReflectUnityEngineMaterial(state, refChain, (Material)obj);
                    return;
                }

                if (type == typeof(Mesh) && !((Mesh)obj).isReadable)
                {
                    SceneExplorerCommon.OnSceneTreeMessage(refChain, "Mesh is not readable");
                    return;
                }
            }

            var members = TypeUtil.GetAllMembers(type, MainWindow.Instance.Config.ShowInheritedMembers);

            if (MainWindow.Instance.Config.SortItemsAlphabetically)
            {
                Array.Sort(members, (x, y) => string.CompareOrdinal(x.ReflectionInfo.Name, y.ReflectionInfo.Name));
            }

            var matchingMembers = 0;
            foreach (var member in members)
            {
                var filterMatchFrom = -1;
                if (!filter.IsNullOrEmpty() && (filterMatchFrom = member.ReflectionInfo.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase)) < 0)
                {
                    continue;
                }

                matchingMembers++;
                switch (member.ReflectionInfo.MemberType)
                {
                    case MemberTypes.Field when MainWindow.Instance.Config.ShowFields:
                    {
                        var field = (FieldInfo)member.ReflectionInfo;

                        if (field.IsLiteral && !MainWindow.Instance.Config.ShowConsts)
                        {
                            continue;
                        }

                        try
                        {
                            GUIField.OnSceneTreeReflectField(state, refChain.Add(field), obj, field, TypeUtil.OverrideSmartType(member.DetectedType, field.Name, obj), filterMatchFrom, filter.Length);
                        }
                        catch (Exception ex)
                        {
                            SceneExplorerCommon.OnSceneTreeMessage(refChain, $"Exception when fetching field \"{field.Name}\" - {ex.Message}\n{ex.StackTrace}");
                        }

                        break;
                    }

                    case MemberTypes.Property when MainWindow.Instance.Config.ShowProperties:
                    {
                        var property = (PropertyInfo)member.ReflectionInfo;
                        if (property.GetIndexParameters().Length > 0)
                        {
                            continue; // TODO: add support for indexers
                        }

                        try
                        {
                            GUIProperty.OnSceneTreeReflectProperty(state, refChain.Add(property), obj, property, TypeUtil.OverrideSmartType(member.DetectedType, property.Name, obj), filterMatchFrom, filter.Length);
                        }
                        catch (Exception ex)
                        {
                            SceneExplorerCommon.OnSceneTreeMessage(refChain, $"Exception when fetching property \"{property.Name}\" - {ex.Message}\n{ex.StackTrace}");
                        }

                        break;
                    }

                    case MemberTypes.Method when MainWindow.Instance.Config.ShowMethods:
                    {
                        var method = (MethodInfo)member.ReflectionInfo;

                        try
                        {
                            GUIMethod.OnSceneTreeReflectMethod(refChain.Add(method), obj, method, filterMatchFrom, filter.Length);
                        }
                        catch (Exception ex)
                        {
                            SceneExplorerCommon.OnSceneTreeMessage(refChain, $"Exception when fetching method \"{method.Name}\" - {ex.Message}");
                        }

                        break;
                    }
                }
            }

            if (filter.IsNullOrEmpty() || members.Length <= 0 || matchingMembers != 0)
            {
                return;
            }

            GUI.contentColor = Color.yellow;
            SceneExplorerCommon.OnSceneTreeMessage(refChain, "No members matching the search term found!");
            GUI.contentColor = Color.white;
        }
    }
}