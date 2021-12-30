﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EManagersLib.Patches {
    internal class EAreaWrapperPatch {

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> SetMaxAreaCountTranspiler(IEnumerable<CodeInstruction> instructions) {
            foreach (var code in instructions) {
                if (code.LoadsConstant(EGameAreaManager.DEFAULTAREACOUNT)) {
                    code.operand = EGameAreaManager.CUSTOMAREACOUNT;
                    yield return code;
                } else {
                    yield return code;
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> GetAreaPriceTranspiler(IEnumerable<CodeInstruction> instructions) => EGameAreaManagerPatch.ReplaceGetTileIndex(instructions);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<CodeInstruction> UnlockAreaImplTranspiler(IEnumerable<CodeInstruction> instructions) => EGameAreaManagerPatch.ReplaceGetTileIndex(instructions);

        internal void Enable(Harmony harmony) {
            try {
                harmony.Patch(AccessTools.PropertySetter(typeof(AreasWrapper), nameof(AreasWrapper.maxAreaCount)),
                    transpiler: new HarmonyMethod(typeof(EAreaWrapperPatch), nameof(SetMaxAreaCountTranspiler)));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch AreaWrapper::set_maxAreaCount");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.PropertySetter(typeof(AreasWrapper), nameof(AreasWrapper.maxAreaCount)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(AreasWrapper), nameof(AreasWrapper.GetAreaPrice)),
                    transpiler: new HarmonyMethod(typeof(EAreaWrapperPatch), nameof(GetAreaPriceTranspiler)));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch AreaWrapper::GetAreaPrice");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(AreasWrapper), nameof(AreasWrapper.GetAreaPrice)),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
            try {
                harmony.Patch(AccessTools.Method(typeof(AreasWrapper), "UnlockAreaImpl"),
                    transpiler: new HarmonyMethod(typeof(EAreaWrapperPatch), nameof(UnlockAreaImplTranspiler)));
            } catch (Exception e) {
                EUtils.ELog("Failed to patch AreaWrapper::UnlockAreaImpl");
                EUtils.ELog(e.Message);
                harmony.Patch(AccessTools.Method(typeof(AreasWrapper), "UnlockAreaImpl"),
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(EUtils), nameof(EUtils.DebugPatchOutput))));
                throw;
            }
        }

        internal void Disable(Harmony harmony) {
            harmony.Unpatch(AccessTools.PropertySetter(typeof(AreasWrapper), nameof(AreasWrapper.maxAreaCount)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(AreasWrapper), nameof(AreasWrapper.GetAreaPrice)), HarmonyPatchType.Transpiler, EModule.HARMONYID);
            harmony.Unpatch(AccessTools.Method(typeof(AreasWrapper), "UnlockAreaImpl"), HarmonyPatchType.Transpiler, EModule.HARMONYID);
        }
    }
}
