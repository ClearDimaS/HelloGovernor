using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ChunkRoot : CulledRoot
{
    [SerializeField] protected int chunkSize = 10;
    [SerializeField] protected int chunksCountPerAxis;
    [SerializeField] protected Vector2 center;
    [SerializeField] protected Vector2 worldSizeXZ = new Vector2(100, 100);
    [SerializeField] protected List<CullableChunk> chunks;
    private Dictionary<CulledBehaviour, CullableChunk> chunksDict = new ();
    public override bool IsVisible => true;
    private Vector2Int nearestChunkIndex;
    protected Camera camera;
    private Transform LookCenter => camera.transform;

    private static ChunkRoot instance;
    public static ChunkRoot Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ChunkRoot>(true);
            }

            return instance;
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        var xIndex = 0;
        for (int x = 0; x < worldSizeXZ.x; x+=chunkSize)
        {
            var zIndex = 0;
            for (int z = 0; z < worldSizeXZ.y; z+=chunkSize)
            {
                var c = new Vector2(x + chunkSize / 2, z + chunkSize / 2) 
                    + center 
                    - new Vector2(worldSizeXZ.x/2f, worldSizeXZ.y/2f);
                chunks.Add(new CullableChunk(
                    c, 
                    new Vector2(chunkSize, chunkSize), 
                    new Vector2Int(xIndex, zIndex),
                    new List<CulledBehaviour>())
                );
                zIndex++;
            }
            xIndex++;
        }
        chunksCountPerAxis = xIndex;

        camera = Camera.main;
        nearestChunkIndex = CaclculateCurrentChunk();
    }

    private Vector2Int CaclculateCurrentChunk()
    {
        return GetNearestChunk(LookCenter).index;
    }

    public override void UpdateCall(float deltaTime)
    {
        base.UpdateCall(deltaTime);
        var currentChunk = chunks[Get_1D_IndexFrom_2D(nearestChunkIndex)];
        var diff = new Vector2(LookCenter.position.x, LookCenter.position.z) - currentChunk.center;
        if (Mathf.Abs(diff.x) > chunkSize || Mathf.Abs(diff.y) > chunkSize)
        {
            nearestChunkIndex = CaclculateCurrentChunk();
        }

        currentChunk.UpdateChunk(true);
        UpdateChunkIfAny(nearestChunkIndex + new Vector2Int(0, 1));
        UpdateChunkIfAny(nearestChunkIndex + new Vector2Int(1, 0));
        UpdateChunkIfAny(nearestChunkIndex + new Vector2Int(1, 1));
        UpdateChunkIfAny(nearestChunkIndex + new Vector2Int(-1, 1));
        UpdateChunkIfAny(nearestChunkIndex + new Vector2Int(1, -1));
        UpdateChunkIfAny(nearestChunkIndex + new Vector2Int(-1, -1));
        UpdateChunkIfAny(nearestChunkIndex + new Vector2Int(0, -1));
        UpdateChunkIfAny(nearestChunkIndex + new Vector2Int(-1, 0));
    }

    private void UpdateChunkIfAny(Vector2Int index2D)
    {
        if (index2D.x < 0 || index2D.y < 0 || index2D.x >chunksCountPerAxis || index2D.y > chunksCountPerAxis)
        {
            return;
        }

        var index = Get_1D_IndexFrom_2D(index2D);
        chunks[index].UpdateChunk(true);
    }
    
    private void LateUpdateChunkIfAny(Vector2Int index2D)
    {
        if (index2D.x < 0 || index2D.y < 0 || index2D.x >chunksCountPerAxis || index2D.y > chunksCountPerAxis)
        {
            return;
        }

        var index = Get_1D_IndexFrom_2D(index2D);
        chunks[index].LateUpdateChunk(true);
    }

    private int Get_1D_IndexFrom_2D(Vector2Int vector2Int)
    {
        return nearestChunkIndex.x + nearestChunkIndex.y * chunksCountPerAxis;
    }

    public override void LateUpdateCall(float deltaTime)
    {
        base.LateUpdateCall(deltaTime);
        
        var currentChunk = chunks[Get_1D_IndexFrom_2D(nearestChunkIndex)];
        currentChunk.UpdateChunk(true);
        LateUpdateChunkIfAny(nearestChunkIndex + new Vector2Int(0, 1));
        LateUpdateChunkIfAny(nearestChunkIndex + new Vector2Int(1, 0));
        LateUpdateChunkIfAny(nearestChunkIndex + new Vector2Int(1, 1));
        LateUpdateChunkIfAny(nearestChunkIndex + new Vector2Int(-1, 1));
        LateUpdateChunkIfAny(nearestChunkIndex + new Vector2Int(1, -1));
        LateUpdateChunkIfAny(nearestChunkIndex + new Vector2Int(-1, -1));
        LateUpdateChunkIfAny(nearestChunkIndex + new Vector2Int(0, -1));
        LateUpdateChunkIfAny(nearestChunkIndex + new Vector2Int(-1, 0));
    }

    public override void AddCulledBehaviour(CulledBehaviour culledBehaviour)
    {
        base.AddCulledBehaviour(culledBehaviour);
        var chunk = GetNearestChunk(culledBehaviour.transform);
        chunksDict[culledBehaviour] = chunk;
        chunk.AddBehaviour(culledBehaviour);
    }

    public override void RemoveCulledBehaviour(CulledBehaviour culledBehaviour)
    {
        base.RemoveCulledBehaviour(culledBehaviour);
        if (chunksDict.ContainsKey(culledBehaviour))
        {
            var chunk = chunksDict[culledBehaviour];
            chunksDict.Remove(culledBehaviour);
            chunk.RemoveBehaviour(culledBehaviour);   
        }
    }
    
    private CullableChunk GetNearestChunk(Transform target)
    {
        CullableChunk nearest = null;
        var minDist = Mathf.Infinity;
        foreach (var chunk in chunks)
        {
            var diff = target.position - new Vector3(chunk.center.x, 0, chunk.center.y);
            diff.y = 0f;
            var diffSqr = diff.sqrMagnitude;
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
    public Vector2Int index;
    public Vector2 center;
    public Vector2 size;
    public List<CulledBehaviour> behaviours;

    public CullableChunk(Vector2 center, Vector2 size, Vector2Int index, List<CulledBehaviour> behaviours)
    {
        this.center = center;
        this.size = size;
        this.index = index;
        this.behaviours = behaviours;
    }

    public void UpdateChunk(bool visible)
    {
        foreach (var behaviour in behaviours)
        {
            behaviour.UpdateCulled(visible);
        }
    }
    
    public void LateUpdateChunk(bool visible)
    {
        foreach (var behaviour in behaviours)
        {
            behaviour.LateUpdateCulled(visible);
        }
    }

    public void AddBehaviour(CulledBehaviour culledBehaviour)
    {
        behaviours.Add(culledBehaviour);
    }
    
    public void RemoveBehaviour(CulledBehaviour culledBehaviour)
    {
        behaviours.Remove(culledBehaviour);
    }
}