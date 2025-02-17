﻿using ColossalFramework;
using ColossalFramework.IO;
using ColossalFramework.Math;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using static EManagersLib.EPropManager;

namespace EManagersLib.Patches {
    internal readonly struct EPropManagerPatch {
        private static IEnumerable<CodeInstruction> AwakeTranspiler(IEnumerable<CodeInstruction> instructions) {
            FieldInfo m_props = AccessTools.Field(typeof(PropManager), nameof(PropManager.m_props));
            FieldInfo m_updatedProps = AccessTools.Field(typeof(PropManager), nameof(PropManager.m_updatedProps));
            FieldInfo m_propGrid = AccessTools.Field(typeof(PropManager), nameof(PropManager.m_propGrid));
            MethodInfo createItem = AccessTools.Method(typeof(Array16<PropInstance>), nameof(Array16<PropInstance>.CreateItem), new Type[] { typeof(ushort).MakeByRefType() });
            using (var codes = instructions.GetEnumerator()) {
                while (codes.MoveNext()) {
                    var cur = codes.Current;
                    if (cur.opcode == OpCodes.Ldarg_0 && codes.MoveNext()) {
                        var next = codes.Current;
                        if (next.LoadsConstant() && codes.MoveNext()) {
                            var next1 = codes.Current;
                            if ((next1.opcode == OpCodes.Newobj || next1.opcode == OpCodes.Newarr) && codes.MoveNext()) {
                                var next2 = codes.Current;
                                if (next2.opcode == OpCodes.Stfld &&
                                    (next2.operand != m_props && next2.operand != m_updatedProps && next2.operand != m_propGrid)) {
                                    yield return cur;
                                    yield return next;
                                    yield return next1;
                                    yield return next2;
                                }
                            } else {
                                yield return cur;
                                yield return next;
                                yield return next1;
                            }
                        } else if (next.opcode == OpCodes.Ldfld && next.operand == m_props && codes.MoveNext()) {
                            var next1 = codes.Current;
                            if (next1.opcode == OpCodes.Ldloca_S && codes.MoveNext()) {
                                var next2 = codes.Current;
                                if (next2.opcode == OpCodes.Callvirt && next2.operand == createItem && codes.MoveNext()) {
                                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManager), nameof(EPropManager.Awake)));
                                } else {
                                    yield return cur;
                                    yield return next;
                                    yield return next1;
                                    yield return next2;
                                }
                            } else {
                                yield return cur;
                                yield return next;
                                yield return next1;
                            }
                        } else {
                            yield return cur;
                            yield return next;
                        }
                    } else {
                        yield return cur;
                    }
                }
            }
        }

        /* This method is completely overriden, do not touch */
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> AfterTerrainUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManager), nameof(EPropManager.AfterTerrainUpdate)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> CalculateGroupDataTranspiler(IEnumerable<CodeInstruction> instructions) {
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Ldarg_3);
            yield return new CodeInstruction(OpCodes.Ldarg_S, 4);
            yield return new CodeInstruction(OpCodes.Ldarg_S, 5);
            yield return new CodeInstruction(OpCodes.Ldarg_S, 6);
            yield return new CodeInstruction(OpCodes.Ldarg_S, 7);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManager), nameof(EPropManager.CalculateGroupData)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> CheckLimitsTranspiler(IEnumerable<CodeInstruction> instructions) {
            foreach (var code in instructions) {
                if (code.LoadsConstant(EPropManager.DEFAULT_MAP_PROPS)) {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(EPropManager), nameof(EPropManager.MAX_MAP_PROPS_LIMIT)));
                } else if (code.LoadsConstant(EPropManager.DEFAULT_GAME_PROPS_LIMIT)) {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(EPropManager), nameof(EPropManager.MAX_GAME_PROPS_LIMIT)));
                } else {
                    yield return code;
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> EndRenderingImplTranspiler(IEnumerable<CodeInstruction> instructions) {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManager), nameof(EPropManager.EndRenderingImpl)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> PopulateGroupDataTranspiler(IEnumerable<CodeInstruction> instructions) {
            yield return new CodeInstruction(OpCodes.Ldarg_1); /* groupX */
            yield return new CodeInstruction(OpCodes.Ldarg_2); /* groupZ */
            yield return new CodeInstruction(OpCodes.Ldarg_3); /* layer */
            yield return new CodeInstruction(OpCodes.Ldarg_S, 4); /* vertexIndex */
            yield return new CodeInstruction(OpCodes.Ldarg_S, 5); /* triangleIndex */
            yield return new CodeInstruction(OpCodes.Ldarg_S, 6); /* groupPosition */
            yield return new CodeInstruction(OpCodes.Ldarg_S, 7); /* data */
            yield return new CodeInstruction(OpCodes.Ldarg_S, 8); /* min */
            yield return new CodeInstruction(OpCodes.Ldarg_S, 9); /* max */
            yield return new CodeInstruction(OpCodes.Ldarg_S, 10); /* maxRenderDistance */
            yield return new CodeInstruction(OpCodes.Ldarg_S, 11); /* maxInstanceDistance */
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManager), nameof(EPropManager.PopulateGroupData)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> SampleSmoothHeightTranspiler(IEnumerable<CodeInstruction> instructions) {
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManager), nameof(EPropManager.SampleSmoothHeight)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> SimulationStepImplTranspiler(IEnumerable<CodeInstruction> instructions) {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManager), nameof(EPropManager.SimulationStepImpl)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> TerrainUpdatedTranspiler(IEnumerable<CodeInstruction> instructions) {
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManager), nameof(EPropManager.TerrainUpdated)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        private static IEnumerable<CodeInstruction> UpdateDataTranspiler(IEnumerable<CodeInstruction> instructions) {
            bool skip = false;
            MethodInfo updateData = AccessTools.Method(typeof(SimulationManagerBase<PropManager, PropProperties>), nameof(SimulationManagerBase<PropManager, PropProperties>.UpdateData));
            MethodInfo getLMInstance = AccessTools.PropertyGetter(typeof(Singleton<LoadingManager>), nameof(Singleton<LoadingManager>.instance));
            foreach (var code in instructions) {
                if (!skip && code.opcode == OpCodes.Call && code.operand == updateData) {
                    skip = true;
                    yield return code;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManager), nameof(EPropManager.UpdateData)));
                } else if (skip && code.opcode == OpCodes.Call && code.operand == getLMInstance) {
                    skip = false;
                    yield return code;
                } else if (!skip) {
                    yield return code;
                }
            }
        }

        private static IEnumerable<CodeInstruction> OverlapQuadTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il) {
            LocalBuilder propID = il.DeclareLocal(typeof(uint));
            FieldInfo propGrid = AccessTools.Field(typeof(PropManager), nameof(PropManager.m_propGrid));
            FieldInfo m_props = AccessTools.Field(typeof(PropManager), nameof(PropManager.m_props));
            FieldInfo m_buffer = AccessTools.Field(typeof(Array16<PropInstance>), nameof(Array16<PropInstance>.m_buffer));
            FieldInfo m_nextGridProp = AccessTools.Field(typeof(PropInstance), nameof(PropInstance.m_nextGridProp));
            MethodInfo getPosition = AccessTools.PropertyGetter(typeof(PropInstance), nameof(PropInstance.Position));
            MethodInfo overlapQuad = AccessTools.Method(typeof(PropInstance), nameof(PropInstance.OverlapQuad));
            using (IEnumerator<CodeInstruction> codes = instructions.GetEnumerator()) {
                while (codes.MoveNext()) {
                    var cur = codes.Current;
                    if (cur.opcode == OpCodes.Ldarg_0 && codes.MoveNext()) {
                        var next = codes.Current;
                        if (next.opcode == OpCodes.Ldfld && next.operand == propGrid) {
                            yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(EPropManager), nameof(m_propGrid))).WithLabels(cur.labels);
                        } else if (next.opcode == OpCodes.Ldfld && next.operand == m_props && codes.MoveNext()) {
                            yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(EPropManager), nameof(EPropManager.m_props))).WithLabels(cur.labels);
                            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Array32<EPropInstance>), nameof(Array32<EPropInstance>.m_buffer)));
                        } else if ((next.opcode == OpCodes.Stloc_S || next.opcode == OpCodes.Ldloc_S) && (next.operand as LocalBuilder).LocalType == typeof(ushort)) {
                            yield return new CodeInstruction(next.opcode, propID).WithLabels(cur.labels);
                        } else {
                            yield return cur;
                            yield return next;
                        }
                    } else if ((cur.opcode == OpCodes.Stloc_S || cur.opcode == OpCodes.Ldloc_S) && (cur.operand as LocalBuilder).LocalType == typeof(ushort)) {
                        yield return new CodeInstruction(cur.opcode, propID).WithLabels(cur.labels);
                    } else if (cur.opcode == OpCodes.Ldelem_U2) {
                        yield return new CodeInstruction(OpCodes.Ldelem_U4);
                    } else if (cur.opcode == OpCodes.Ldelema && cur.operand == typeof(PropInstance)) {
                        yield return new CodeInstruction(OpCodes.Ldelema, typeof(EPropInstance));
                    } else if (cur.opcode == OpCodes.Call && cur.operand == getPosition) {
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(EPropInstance), nameof(EPropInstance.Position)));
                    } else if (cur.opcode == OpCodes.Call && cur.operand == overlapQuad) {
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropInstance), nameof(EPropInstance.OverlapQuad)));
                    } else if (cur.opcode == OpCodes.Ldfld && cur.operand == propGrid) {
                        yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(EPropManager), nameof(m_propGrid))).WithLabels(cur.labels);
                    } else if (cur.opcode == OpCodes.Ldfld && cur.operand == m_nextGridProp) {
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(EPropInstance), nameof(EPropInstance.m_nextGridProp)));
                    } else if (cur.opcode == OpCodes.Ldc_I4 && (cur.operand as int?).Value == DEFAULT_PROP_LIMIT) {
                        yield return new CodeInstruction(OpCodes.Ldc_I4, MAX_PROP_LIMIT);
                    } else {
                        yield return cur;
                    }
                }
            }
        }

        private static IEnumerable<CodeInstruction> RayCastTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il) {
            LocalBuilder propID = il.DeclareLocal(typeof(uint));
            FieldInfo propGrid = AccessTools.Field(typeof(PropManager), nameof(PropManager.m_propGrid));
            FieldInfo m_props = AccessTools.Field(typeof(PropManager), nameof(PropManager.m_props));
            FieldInfo m_buffer = AccessTools.Field(typeof(Array16<PropInstance>), nameof(Array16<PropInstance>.m_buffer));
            FieldInfo m_flags = AccessTools.Field(typeof(PropInstance), nameof(PropInstance.m_flags));
            FieldInfo m_nextGridProp = AccessTools.Field(typeof(PropInstance), nameof(PropInstance.m_nextGridProp));
            MethodInfo getPosition = AccessTools.PropertyGetter(typeof(PropInstance), nameof(PropInstance.Position));
            MethodInfo overlapQuad = AccessTools.Method(typeof(PropInstance), nameof(PropInstance.OverlapQuad));
            MethodInfo getInfo = AccessTools.PropertyGetter(typeof(PropInstance), nameof(PropInstance.Info));
            MethodInfo propRaycast = AccessTools.Method(typeof(PropInstance), nameof(PropInstance.RayCast));
            using (IEnumerator<CodeInstruction> codes = instructions.GetEnumerator()) {
                while (codes.MoveNext()) {
                    var cur = codes.Current;
                    if (cur.opcode == OpCodes.Ldarg_0 && codes.MoveNext()) {
                        var next = codes.Current;
                        if (next.opcode == OpCodes.Ldfld && next.operand == propGrid) {
                            yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(EPropManager), nameof(m_propGrid))).WithLabels(cur.labels);
                        } else if (next.opcode == OpCodes.Ldfld && next.operand == m_props && codes.MoveNext()) {
                            yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(EPropManager), nameof(EPropManager.m_props))).WithLabels(cur.labels);
                            yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Array32<EPropInstance>), nameof(Array32<EPropInstance>.m_buffer)));
                        } else if ((next.opcode == OpCodes.Stloc_S || next.opcode == OpCodes.Ldloc_S) && (next.operand as LocalBuilder).LocalType == typeof(ushort)) {
                            yield return new CodeInstruction(next.opcode, propID).WithLabels(cur.labels);
                        } else {
                            yield return cur;
                            yield return next;
                        }
                    } else if ((cur.opcode == OpCodes.Stloc_S || cur.opcode == OpCodes.Ldloc_S) && (cur.operand as LocalBuilder).LocalType == typeof(ushort)) {
                        yield return new CodeInstruction(cur.opcode, propID).WithLabels(cur.labels);
                    } else if (cur.opcode == OpCodes.Ldelem_U2) {
                        yield return new CodeInstruction(OpCodes.Ldelem_U4);
                    } else if (cur.opcode == OpCodes.Ldelema && cur.operand == typeof(PropInstance)) {
                        yield return new CodeInstruction(OpCodes.Ldelema, typeof(EPropInstance));
                    } else if (cur.opcode == OpCodes.Call && cur.operand == getPosition) {
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(EPropInstance), nameof(EPropInstance.Position)));
                    } else if (cur.opcode == OpCodes.Call && cur.operand == overlapQuad) {
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropInstance), nameof(EPropInstance.OverlapQuad)));
                    } else if (cur.opcode == OpCodes.Ldfld && cur.operand == propGrid) {
                        yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(EPropManager), nameof(m_propGrid))).WithLabels(cur.labels);
                    } else if (cur.opcode == OpCodes.Ldfld && cur.operand == m_flags) {
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(EPropInstance), nameof(EPropInstance.m_flags)));
                    } else if (cur.opcode == OpCodes.Call && cur.operand == getInfo) {
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(EPropInstance), nameof(EPropInstance.Info)));
                    } else if (cur.opcode == OpCodes.Call && cur.operand == propRaycast) {
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropInstance), nameof(EPropInstance.RayCast)));
                    } else if (cur.opcode == OpCodes.Ldfld && cur.operand == m_nextGridProp) {
                        yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(EPropInstance), nameof(EPropInstance.m_nextGridProp)));
                    } else if (cur.opcode == OpCodes.Ldc_I4 && (cur.operand as int?).Value == DEFAULT_PROP_LIMIT) {
                        yield return new CodeInstruction(OpCodes.Ldc_I4, MAX_PROP_LIMIT);
                    } else {
                        yield return cur;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> UpdatePropsTranspiler(IEnumerable<CodeInstruction> instructions) {
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Ldarg_3);
            yield return new CodeInstruction(OpCodes.Ldarg_S, 4);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManager), nameof(EPropManager.UpdateProps)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SetPropScaleColor(uint id, ref EPropInstance prop, PropInfo info) {
            if (prop.m_scale == 0f) {
                Randomizer rand = new Randomizer((int)id);
                prop.m_scale = info.m_minScale + rand.Int32(10000u) * (info.m_maxScale - info.m_minScale) * 0.0001f;
                prop.m_color = info.GetColor(ref rand);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> GetRandomPropInfoTranspiler(IEnumerable<CodeInstruction> instructions) {
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManager), nameof(EPropManager.GetRandomPropInfo)));
            yield return new CodeInstruction(OpCodes.Ret);
        }

        private static IEnumerable<CodeInstruction> AfterDeserializeTranspiler(IEnumerable<CodeInstruction> instructions) {
            MethodInfo loadingManagerInstance = AccessTools.PropertyGetter(typeof(Singleton<LoadingManager>), nameof(Singleton<LoadingManager>.instance));
            MethodInfo beginAfterDeserialize = AccessTools.Method(typeof(LoadingProfiler), nameof(LoadingProfiler.BeginAfterDeserialize));
            FieldInfo loadingProfiler = AccessTools.Field(typeof(LoadingManager), nameof(LoadingManager.m_loadingProfilerSimulation));
            using (var codes = instructions.GetEnumerator()) {
                while (codes.MoveNext()) {
                    var cur = codes.Current;
                    if (cur.opcode == OpCodes.Callvirt && cur.operand == beginAfterDeserialize) {
                        yield return cur;
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManagerPatch), nameof(AfterDeserialize)));
                        while (codes.MoveNext()) {
                            cur = codes.Current;
                            if (cur.opcode == OpCodes.Call && cur.operand == loadingManagerInstance && codes.MoveNext()) {
                                var next = codes.Current;
                                if (next.opcode == OpCodes.Ldfld && next.operand == loadingProfiler) {
                                    yield return cur;
                                    yield return next;
                                    break;
                                }
                            }
                        }
                    } else {
                        yield return cur;
                    }
                }
            }
        }

        private static IEnumerable<CodeInstruction> DeserializeTranspiler(IEnumerable<CodeInstruction> instructions) {
            MethodInfo loadingManagerInstance = AccessTools.PropertyGetter(typeof(Singleton<LoadingManager>), nameof(Singleton<LoadingManager>.instance));
            MethodInfo beginDeserialize = AccessTools.Method(typeof(LoadingProfiler), nameof(LoadingProfiler.BeginDeserialize), new Type[] { typeof(DataSerializer), typeof(string) });
            using (var codes = instructions.GetEnumerator()) {
                while (codes.MoveNext()) {
                    var cur = codes.Current;
                    if (cur.opcode == OpCodes.Callvirt && cur.operand == beginDeserialize) {
                        yield return cur;
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManagerPatch), nameof(Deserialize)));
                        while (codes.MoveNext()) {
                            cur = codes.Current;
                            if (cur.opcode == OpCodes.Call && cur.operand == loadingManagerInstance) {
                                yield return cur;
                                break;
                            }
                        }
                    } else {
                        yield return cur;
                    }
                }
            }
        }

        private static IEnumerable<CodeInstruction> SerializeTranspiler(IEnumerable<CodeInstruction> instructions) {
            MethodInfo loadingManagerInstance = AccessTools.PropertyGetter(typeof(Singleton<LoadingManager>), nameof(Singleton<LoadingManager>.instance));
            MethodInfo beginDeserialize = AccessTools.Method(typeof(LoadingProfiler), nameof(LoadingProfiler.BeginSerialize), new Type[] { typeof(DataSerializer), typeof(string) });
            using (var codes = instructions.GetEnumerator()) {
                while (codes.MoveNext()) {
                    var cur = codes.Current;
                    if (cur.opcode == OpCodes.Callvirt && cur.operand == beginDeserialize) {
                        yield return cur;
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(EPropManagerPatch), nameof(Serialize)));
                        while (codes.MoveNext()) {
                            cur = codes.Current;
                            if (cur.opcode == OpCodes.Call && cur.operand == loadingManagerInstance) {
                                yield return cur;
                                break;
                            }
                        }
                    } else {
                        yield return cur;
                    }
                }
            }
        }

        private static void AfterDeserialize(DataSerializer s) {
            Singleton<LoadingManager>.instance.WaitUntilEssentialScenesLoaded();
            PrefabCollection<PropInfo>.BindPrefabs();
            PropManager instance = Singleton<PropManager>.instance;
            RefreshAutomaticProps();
            EPropInstance[] buffer = m_props.m_buffer;
            int len = buffer.Length;
            DistrictManager district = Singleton<DistrictManager>.instance;
            DistrictPark[] parks = district.m_parks.m_buffer;
            for (int i = 1; i < len; i++) {
                if (buffer[i].m_flags != 0) {
                    PropInfo info = buffer[i].Info;
                    if (!(info is null)) {
                        if (info.m_requireHeightMap) buffer[i].m_flags |= EPropInstance.CONFORMFLAG;
                        buffer[i].m_infoIndex = (ushort)info.m_prefabDataIndex;
                        if (buffer[i].m_scale == 0) {
                            Randomizer randomizer = new Randomizer(i);
                            buffer[i].m_scale = info.m_minScale + randomizer.Int32(10000u) * (info.m_maxScale - info.m_minScale) * 0.0001f;
                            buffer[i].m_color = info.GetColor(ref randomizer);
                        }
                    }
                    if (!buffer[i].Blocked) {
                        parks[district.GetPark(buffer[i].Position)].m_propCount++;
                    }
                }
            }
            instance.m_propCount = (int)(m_props.ItemCount() - 1u);
        }

        private static void Deserialize(DataSerializer s) {
            PropManager instance = Singleton<PropManager>.instance;
            EnsureCapacity(instance);
            EPropInstance[] buffer = m_props.m_buffer;
            uint[] propGrid = m_propGrid;
            m_props.ClearUnused();
            SimulationManager.UpdateMode updateMode = Singleton<SimulationManager>.instance.m_metaData.m_updateMode;
            bool assetEditor = updateMode == SimulationManager.UpdateMode.NewAsset || updateMode == SimulationManager.UpdateMode.LoadAsset;
            for (int i = 0; i < propGrid.Length; i++) {
                propGrid[i] = 0;
            }
            EncodedArray.UShort uShort = EncodedArray.UShort.BeginRead(s);
            for (int i = 1; i < DEFAULT_PROP_LIMIT; i++) {
                buffer[i].m_flags = uShort.Read();
            }
            uShort.EndRead();
            PrefabCollection<PropInfo>.BeginDeserialize(s);
            for (int i = 1; i < DEFAULT_PROP_LIMIT; i++) {
                if (buffer[i].m_flags != 0) {
                    buffer[i].m_infoIndex = (ushort)PrefabCollection<PropInfo>.Deserialize(true);
                }
            }
            PrefabCollection<PropInfo>.EndDeserialize(s);
            EncodedArray.Short @short = EncodedArray.Short.BeginRead(s);
            for (int i = 1; i < DEFAULT_PROP_LIMIT; i++) {
                if (buffer[i].m_flags != 0) {
                    buffer[i].m_posX = @short.Read();
                } else {
                    buffer[i].m_posX = 0;
                }
            }
            @short.EndRead();
            EncodedArray.Short short2 = EncodedArray.Short.BeginRead(s);
            for (int i = 1; i < DEFAULT_PROP_LIMIT; i++) {
                if (buffer[i].m_flags != 0) {
                    buffer[i].m_posZ = short2.Read();
                } else {
                    buffer[i].m_posZ = 0;
                }
            }
            short2.EndRead();
            EncodedArray.UShort uShort2 = EncodedArray.UShort.BeginRead(s);
            for (int i = 1; i < DEFAULT_PROP_LIMIT; i++) {
                if (buffer[i].m_flags != 0) {
                    buffer[i].m_angle = uShort2.Read();
                } else {
                    buffer[i].m_angle = 0;
                }
            }
            uShort2.EndRead();
            ESerializableData.IntegratedPropDeserialize(buffer);
            buffer = m_props.m_buffer;
            int len = buffer.Length;
            for (int i = 1; i < len; i++) {
                buffer[i].m_nextGridProp = 0;
                if (buffer[i].m_flags != 0) {
                    InitializeProp((uint)i, ref buffer[i], assetEditor);
                } else {
                    m_props.ReleaseItem((uint)i);
                }
            }
        }

        private static void Serialize(DataSerializer s) {
            EPropInstance[] buffer = m_props.m_buffer;
            EncodedArray.UShort uShort = EncodedArray.UShort.BeginWrite(s);
            for (int i = 1; i < DEFAULT_PROP_LIMIT; i++) {
                uShort.Write(buffer[i].m_flags);
            }
            uShort.EndWrite();
            try {
                PrefabCollection<PropInfo>.BeginSerialize(s);
                for (int i = 1; i < DEFAULT_PROP_LIMIT; i++) {
                    if (buffer[i].m_flags != 0) {
                        PrefabCollection<PropInfo>.Serialize(buffer[i].m_infoIndex);
                    }
                }
            } finally {
                PrefabCollection<PropInfo>.EndSerialize(s);
            }
            EncodedArray.Short @short = EncodedArray.Short.BeginWrite(s);
            for (int i = 1; i < DEFAULT_PROP_LIMIT; i++) {
                if (buffer[i].m_flags != 0) {
                    @short.Write(buffer[i].m_posX);
                }
            }
            @short.EndWrite();
            EncodedArray.Short short2 = EncodedArray.Short.BeginWrite(s);
            for (int i = 1; i < DEFAULT_PROP_LIMIT; i++) {
                if (buffer[i].m_flags != 0) {
                    short2.Write(buffer[i].m_posZ);
                }
            }
            short2.EndWrite();
            EncodedArray.UShort uShort2 = EncodedArray.UShort.BeginWrite(s);
            for (int i = 1; i < DEFAULT_PROP_LIMIT; i++) {
                if (buffer[i].m_flags != 0) {
                    uShort2.Write(buffer[i].m_angle);
                }
            }
            uShort2.EndWrite();
        }

        internal void Enable(Harmony harmony) {
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), "Awake"),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(AwakeTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::Awake");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), "Awake"),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.AfterTerrainUpdate)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(AfterTerrainUpdateTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::AfterTerrainUpdate");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.AfterTerrainUpdate)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.CalculateGroupData)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(CalculateGroupDataTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::CalculateGroupData");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.CalculateGroupData)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.CheckLimits)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(CheckLimitsTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::CheckLimits");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.CheckLimits)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), "EndRenderingImpl"),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(EndRenderingImplTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::EndRenderingImpl");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), "EndRenderingImpl"),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.OverlapQuad)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(OverlapQuadTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::OverlapQuad");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.OverlapQuad)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.PopulateGroupData)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(PopulateGroupDataTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::PopulateGroupData");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.PopulateGroupData)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.RayCast)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(RayCastTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::RayCast");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.RayCast)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.GetRandomPropInfo)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(GetRandomPropInfoTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::GetRandomPropInfo");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.GetRandomPropInfo)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.SampleSmoothHeight)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(SampleSmoothHeightTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::SampleSmoothHeight");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.SampleSmoothHeight)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), "SimulationStepImpl"),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(SimulationStepImplTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::SimulationStepImpl");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), "SimulationStepImpl"),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.TerrainUpdated)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(TerrainUpdatedTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::TerrainUpdated");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.TerrainUpdated)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.UpdateData)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(UpdateDataTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::UpdateData");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.UpdateData)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.UpdateProps)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(UpdatePropsTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::UpdateProps");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager), nameof(PropManager.UpdateProps)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager.Data), nameof(PropManager.Data.Deserialize)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(DeserializeTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::Data::Deserialize");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager.Data), nameof(PropManager.Data.Deserialize)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager.Data), nameof(PropManager.Data.AfterDeserialize)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(AfterDeserializeTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::Data::AfterDeserialize");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager.Data), nameof(PropManager.Data.AfterDeserialize)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(PropManager.Data), nameof(PropManager.Data.Serialize)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EPropManagerPatch), nameof(SerializeTranspiler))));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch PropManager::Data::Serialize");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(PropManager.Data), nameof(PropManager.Data.Serialize)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
        }

        internal void Disable(Harmony harmony) {
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), "Awake"), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), nameof(PropManager.AfterTerrainUpdate)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), nameof(PropManager.CalculateGroupData)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), nameof(PropManager.CheckLimits)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), "EndRenderingImpl"), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), nameof(PropManager.OverlapQuad)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), nameof(PropManager.RayCast)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), nameof(PropManager.GetRandomPropInfo)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), nameof(PropManager.PopulateGroupData)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), nameof(PropManager.SampleSmoothHeight)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), "SimulationStepImpl"), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), nameof(PropManager.TerrainUpdated)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), nameof(PropManager.UpdateData)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager), nameof(PropManager.UpdateProps)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager.Data), nameof(PropManager.Data.Deserialize)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager.Data), nameof(PropManager.Data.AfterDeserialize)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(PropManager.Data), nameof(PropManager.Data.Serialize)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
        }
    }
}
