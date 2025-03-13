using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRoot : CulledRoot
{
    [SerializeField] protected int chunkSize = 10;
    [SerializeField] protected Vector2 center;
    [SerializeField] protected Vector2 worldSizeXZ = new Vector2(100, 100);
    [SerializeField] protected List<CullableChunk> chunks;
    public override bool IsVisible => true;

    protected override void OnAwake()
    {
        base.OnAwake();
        var xIndex = 0;
        for (int x = 0; x < worldSizeXZ.x; x+=chunkSize)
        {
            var zIndex = 0;
            for (int z = 0; z < worldSizeXZ.y; z+=chunkSize)
            {
                var center = new Vector2(x + chunkSize / 2, z + chunkSize / 2);
                chunks.Add(new CullableChunk(
                    center, 
                    new Vector2(chunkSize, chunkSize), 
                    new List<CulledBehaviour>())
                );
                zIndex++;
            }
            xIndex++;
        }
    }

    public override void AddCulledBehaviour(CulledBehaviour culledBehaviour)
    {
        base.AddCulledBehaviour(culledBehaviour);
        var chunk = GetNearestChunk(culledBehaviour.transform);
    }

    public override void RemoveCulledBehaviour(CulledBehaviour culledBehaviour)
    {
        base.RemoveCulledBehaviour(culledBehaviour);
    }
    
    private CullableChunk GetNearestChunk(Transform target)
    {
        CullableChunk nearest = null;
        var minDist = Mathf.Infinity;
        foreach (var chunk in chunks)
        {
            var diffSqr = (target.position - new Vector3(chunk.center.x, 0, chunk.center.y)).sqrMagnitude;
            if (diffSqr < minDist)
            {
                minDist = diffSqr;
                nearest = chunk;
            }
        }

        return nearest;
    }
}

[Serializable]
public class CullableChunk
{
    public Vector2 center;
    public Vector2 size;
    public List<CulledBehaviour> behaviours;

    public CullableChunk(Vector2 center, Vector2 size, List<CulledBehaviour> behaviours)
    {
        this.center = center;
        this.size = size;
        this.behaviours = behaviours;
    }

    public void UpdateChunk(bool visible)
    {
        foreach (var behaviour in behaviours)
        {
            behaviour.UpdateCulled(visible);
        }
    }
    
    public void UpdateChunkInvisible(bool visible)
    {
        foreach (var behaviour in behaviours)
        {
            behaviour.UpdateCulled(visible);
        }
    }
}