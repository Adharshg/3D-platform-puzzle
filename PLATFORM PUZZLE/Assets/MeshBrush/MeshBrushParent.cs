using UnityEngine;
using System.Collections;

namespace MeshBrush {
    public class MeshBrushParent : MonoBehaviour {

        Transform[] meshes;
        Component[] meshFilters;
        Matrix4x4 myTransform;

        Hashtable materialToMesh;

        MeshFilter filter;
        Renderer curRenderer;

        Material[] materials;

        CombineUtility.MeshInstance instance;
        CombineUtility.MeshInstance[] instances;

        ArrayList objects;
        ArrayList elements;

        void Start() { Destroy(this); }

#if UNITY_EDITOR
        public void FlagMeshesAsStatic()
        {
            meshes = GetComponentsInChildren<Transform>();
            foreach (Transform _t in meshes)
            {
                _t.gameObject.isStatic = true;
            }
        }

        public void UnflagMeshesAsStatic()
        {
            meshes = GetComponentsInChildren<Transform>();
            foreach (Transform _t in meshes)
            {
                _t.gameObject.isStatic = false;
            }
        }

        public void DeleteAllMeshes()
        {
            DestroyImmediate(gameObject);
        }

        public void CombinePaintedMeshes(bool autoSelect, MeshFilter[] meshFilters)
        {
            if (meshFilters == null || meshFilters.Length == 0)
            {
                Debug.LogError("MeshBrush: The meshFilters array you passed in as a parameter to the CombinePaintedMeshes function is empty or null... Combining action cancelled!");
                return;
            }

            myTransform = transform.worldToLocalMatrix;
            materialToMesh = new Hashtable();

            int totalVertCount = 0;
            for (long i = 0 ; i < meshFilters.LongLength ; i++)
            {
                filter = (MeshFilter)meshFilters[i];

                totalVertCount += filter.sharedMesh.vertexCount;

                if (totalVertCount > 64000)
                {
                    if (UnityEditor.EditorUtility.DisplayDialog("Warning!", "You are trying to combine a group of meshes whose total vertex count exceeds Unity's built-in limit.\n\nThe process has been aborted to prevent the accidental deletion of all painted meshes and numerous disturbing error messages printed to the console.\n\nConsider splitting your meshes into smaller groups and combining them separately.\n\n=> You can do that for example based on the circle brush's area (press the combine meshes key in the scene view), or via multiple MeshBrush instances to form various painting sets and combine them individually; see the help section in the inspector for more detailed infos!", "Okay"))
                    {
                        return;
                    }
                }
            }

            for (long i = 0 ; i < meshFilters.LongLength ; i++)
            {
                filter = (MeshFilter)meshFilters[i];
                curRenderer = meshFilters[i].GetComponent<Renderer>();

                instance = new CombineUtility.MeshInstance();
                instance.mesh = filter.sharedMesh;

                if (curRenderer != null && curRenderer.enabled && instance.mesh != null)
                {
                    instance.transform = myTransform * filter.transform.localToWorldMatrix;

                    materials = curRenderer.sharedMaterials;
                    for (int m = 0 ; m < materials.Length ; m++)
                    {
                        instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);

                        objects = (ArrayList)materialToMesh[materials[m]];
                        if (objects != null)
                        {
                            objects.Add(instance);
                        }
                        else
                        {
                            objects = new ArrayList();
                            objects.Add(instance);
                            materialToMesh.Add(materials[m], objects);
                        }
                    }

                    DestroyImmediate(curRenderer.gameObject);
                }
            }

            foreach (DictionaryEntry de in materialToMesh)
            {
                elements = (ArrayList)de.Value;
                instances = (CombineUtility.MeshInstance[])elements.ToArray(typeof(CombineUtility.MeshInstance));

                GameObject go = new GameObject("Combined mesh");
                go.transform.parent = transform;
                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localPosition = Vector3.zero;
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                go.AddComponent<SaveCombinedMesh>();
                go.GetComponent<Renderer>().material = (Material)de.Key;
                go.isStatic = true;

                filter = go.GetComponent<MeshFilter>();
                filter.mesh = CombineUtility.Combine(instances, false);

                if (autoSelect)
                    UnityEditor.Selection.activeObject = go;
            }
            gameObject.isStatic = true;
        }
#endif
    }
}

// Copyright (C) 2015, Raphael Beck
