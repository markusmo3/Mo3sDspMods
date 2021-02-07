using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Mo3sDspMods {
    [BepInPlugin("io.github.markusmo3.dsp.smolsplitters", "smol splitters", "1.0.0")]
    public class SmolSplittersPlugin : BaseUnityPlugin {

        internal void Awake() {
            var harmony = new Harmony("io.github.markusmo3.dsp.smolsplitters");
            Harmony.CreateAndPatchAll(typeof(SplitterColliderPatch));
        }

        [HarmonyPatch(typeof(PrefabDesc))]
        private class SplitterColliderPatch {

            private static ManualLogSource myLogSource = new ManualLogSource("PatchSplitterCollider");
            static SplitterColliderPatch() {
                BepInEx.Logging.Logger.Sources.Add(myLogSource);
            }

            [HarmonyPostfix]
            [HarmonyPatch("ReadPrefab")]
            public static void Postfix(PrefabDesc __instance, ref GameObject _prefab, ref GameObject _colliderPrefab) {
                // myLogSource.LogInfo("YEP logging works!");
                SplitterDesc componentInChildren6 = __instance.prefab.GetComponentInChildren<SplitterDesc>(true);
                if (componentInChildren6 != null) {
                    // this prefab is a splitter!

                    // doesnt work...
                    // scaleMesh(__instance.mesh, new Vector3(0.5f, 0.5f, 0.5f));
                    // for (int i = 0; i < __instance.meshes.Length; i++) {
                    //     scaleMesh(__instance.meshes[i], new Vector3(0.5f, 0.5f, 0.5f));
                    // }
                    // for (int i = 0; i < __instance.lodMeshes.Length; i++) {
                    //     scaleMesh(__instance.lodMeshes[i], new Vector3(0.5f, 0.5f, 0.5f));
                    // }
                    // __instance.prefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    // __instance.colliderPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    // componentInChildren6.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                    var buildCol = __instance.buildCollider;
                    buildCol.ext *= 0.3f;
                    __instance.buildCollider = buildCol;
                    for (int i = 0; i < __instance.buildColliders.Length; i++) {
                        var col = __instance.buildColliders[i];
                        col.ext *= 0.3f;
                        __instance.buildColliders[i] = col;
                    }
                    for (int i = 0; i < __instance.colliders.Length; i++) {
                        var col = __instance.colliders[i];
                        col.ext *= 0.5f;
                        __instance.colliders[i] = col;
                    }
                }
            }

        }

        private static void scaleMesh(Mesh mesh, Vector3 scaleFactor, bool recalculateNormals = true) {
            Vector3[] baseVertices = mesh.vertices;
            var vertices = new Vector3[baseVertices.Length];
            for (var i = 0; i < vertices.Length; i++) {
                var vertex = baseVertices[i];
                vertex.x *= scaleFactor.x;
                vertex.y *= scaleFactor.y;
                vertex.z *= scaleFactor.z;
                vertices[i] = vertex;
            }
            mesh.vertices = vertices;
            if (recalculateNormals)
                mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

    }
}