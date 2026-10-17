using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public static class AvatarUtility
    {
        /// <summary>仅用于合并材料</summary>
        public static int combine_texture_max = 512;
        public static string combine_diffuse_texture = "_MainTex";
        public static string combine_default_shader = "Mobile/Diffuse";

        /// <summary>
        /// 将 SkinnedMeshRenderers 组合在一起并共享一个骨架。合并材质会减少调用次数，但它会增加内存的大小
        /// <para><paramref name="skeleton"/>将网格合并到此骨骼（一个游戏对象）</para>
        /// <para><paramref name="meshes"/>需要合并的网格</para>
        /// <para><paramref name="combine"/>是否合并材质</para>
        /// </summary>
        public static void CombineSkinObject(GameObject skeleton, SkinnedMeshRenderer[] meshes, bool combine = false)
        {
            // 获取骨骼的所有骨骸
            List<Transform> transforms = new List<Transform>();
            transforms.AddRange(skeleton.GetComponentsInChildren<Transform>(true));

            List<Material> materials = new List<Material>();
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            List<Transform> bones = new List<Transform>();

            // 以下信息仅用于合并材料（bool combine = true）
            List<Vector2[]> oldUV = null;
            Material newMaterial = null;
            Texture2D newDiffuseTex = null;

            // 从网格中收集信息
            for (int i = 0; i < meshes.Length; i++)
            {
                SkinnedMeshRenderer smr = meshes[i];
                materials.AddRange(smr.materials);

                for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
                {
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = smr.sharedMesh;
                    ci.subMeshIndex = sub;
                    combineInstances.Add(ci);
                }

                for (int j = 0; j < smr.bones.Length; j++)
                {

                    for (int tBase = 0; tBase < transforms.Count; tBase++)
                    {
                        if (smr.bones[j].name.Equals(transforms[tBase].name))
                        {
                            bones.Add(transforms[tBase]);
                            break;
                        }
                    }
                }
            }

            // 合并材质
            if (combine)
            {
                newMaterial = new Material(Shader.Find(combine_default_shader));
                oldUV = new List<Vector2[]>();
                // 合并贴图
                List<Texture2D> Textures = new List<Texture2D>();
                for (int i = 0; i < materials.Count; i++)
                {
                    Textures.Add(materials[i].GetTexture(combine_diffuse_texture) as Texture2D);
                }

                newDiffuseTex = new Texture2D(combine_texture_max, combine_texture_max, TextureFormat.RGBA32, true);
                Rect[] uvs = newDiffuseTex.PackTextures(Textures.ToArray(), 0);
                newMaterial.mainTexture = newDiffuseTex;

                // 重置 uv 
                Vector2[] uva, uvb;
                for (int j = 0; j < combineInstances.Count; j++)
                {
                    uva = combineInstances[j].mesh.uv;
                    uvb = new Vector2[uva.Length];
                    for (int k = 0; k < uva.Length; k++)
                    {
                        // 计算合并后的 uv 位置（根据偏移计算）
                        uvb[k] = new Vector2((uva[k].x * uvs[j].width) + uvs[j].x, (uva[k].y * uvs[j].height) + uvs[j].y);
                    }
                    oldUV.Add(combineInstances[j].mesh.uv); // 记录原 uv 
                    combineInstances[j].mesh.uv = uvb;           // 应用计算后的 uv 
                }
            }

            // 创建一个新的 SkinnedMeshRenderer
            SkinnedMeshRenderer oldSKinned = skeleton.GetComponent<SkinnedMeshRenderer>();
            if (oldSKinned != null)
            {
                GameObject.DestroyImmediate(oldSKinned);
            }
            SkinnedMeshRenderer r = skeleton.AddComponent<SkinnedMeshRenderer>();
            r.sharedMesh = new Mesh();
            r.sharedMesh.name = "merge";
            r.sharedMesh.CombineMeshes(combineInstances.ToArray(), combine, false);// Combine meshes
            r.bones = bones.ToArray();// Use new bones

            if (combine)
            {
                r.material = newMaterial;
                for (int i = 0; i < combineInstances.Count; i++)
                {
                    combineInstances[i].mesh.uv = oldUV[i];
                }
            }
            else
            {
                r.materials = materials.ToArray();
            }
        }

        /// <summary>与目标共享骨骼
        /// <para>切换衣服等绑定骨骼的模型，与主模型骨骼绑定，已同步动画</para>
        /// </summary>
        public static void ShareSkeletonInstanceWith(SkinnedMeshRenderer selfSkin, GameObject target)
        {
            Transform[] newBones = new Transform[selfSkin.bones.Length];
            for (int i = 0; i < selfSkin.bones.GetLength(0); ++i)
            {
                GameObject bone = selfSkin.bones[i].gameObject;

                // 目标的SkinnedMeshRenderer.bones保存的只是目标mesh相关的骨骼,要获得目标全部骨骼,可以通过查找的方式.
                newBones[i] = FindChildRecursion(target.transform, bone.name);
            }

            selfSkin.bones = newBones;
        }

        // 递归查找
        public static Transform FindChildRecursion(Transform t, string name)
        {
            foreach (Transform child in t)
            {
                if (child.name == name)
                {
                    return child;
                }
                else
                {
                    Transform ret = FindChildRecursion(child, name);
                    if (ret != null)
                        return ret;
                }
            }
            return null;
        }
    }
}