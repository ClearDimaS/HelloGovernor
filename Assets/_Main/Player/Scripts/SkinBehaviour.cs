using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class SkinBehaviour : MonoBehaviour
{
    [SerializeField] protected MeshRenderer[] mrs;
    [SerializeField] protected SkinnedMeshRenderer[] smrs;
    [SerializeField] protected Material lockedMaterial;
    [SerializeField] protected Material unlockedMaterial;
    
    [Button]
    protected void GatherMRs()
    {
        mrs = GetComponentsInChildren<MeshRenderer>();
        smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    [Button]
    protected void SetupPrefab()
    {
        foreach (var mr in mrs)
        {
            mr.shadowCastingMode = ShadowCastingMode.Off;
        }
        foreach (var smr in smrs)
        {
            smr.shadowCastingMode = ShadowCastingMode.Off;
            smr.quality = SkinQuality.Bone1;
        }

        var animator = GetComponentInChildren<Animator>();
        animator.cullingMode = AnimatorCullingMode.CullCompletely;
    }

    public void SetLocked(bool isLocked)
    {
        Material mat = isLocked ? lockedMaterial : unlockedMaterial;
        foreach (var mr in mrs)
        {
            mr.material = mat;
        }
        foreach (var smr in smrs)
        {
            smr.material = mat;
        }
    }
}
