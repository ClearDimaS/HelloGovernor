using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class JumpingItem : CulledBehaviour
{
    [Inject] private PlayerController player;

    [SerializeField] private Vector2Int moneyCountMinMax = new Vector2Int(1, 3);
    [SerializeField] private Vector2Int moneyAfterJumpsMinMax = new Vector2Int(3, 7);
    [SerializeField] private int minPlaces = 2;
    [SerializeField] private Transform[] places;
    [SerializeField] private Transform root;
    [SerializeField] private float interactionRadius = 1.3f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private bool lookFwd = true;
    protected int placeIndex;
    protected int dir = 1;
    protected bool isInteracting;
    private int placesCount;
    protected int placeStart;
    protected int jumpsCounter;
    protected int moneyAfterJumps;

    protected override void OnAwake()
    {
        base.OnAwake();
        placesCount = Random.Range(minPlaces, places.Length+1);
        placeStart = Random.Range(0, places.Length - placesCount);
        placeIndex = placeStart;
        root.position = places[placeIndex].position;
        root.rotation = places[placeIndex].rotation;
        moneyAfterJumps = 1;
    }

    protected override void OnUpdate(bool visible)
    {
        base.OnUpdate(visible);
        if (visible && !isInteracting)
        {
            var diff = root.position - player.transform.position;
            diff.y = 0f;
            if (diff.magnitude < interactionRadius)
            {
                Interact();
            }
        }
    }

    private void Interact()
    {
        isInteracting = true;
        var t = 0f;
        var start = places[placeIndex];
        if (dir == 1)
        {
            if (placeIndex + 1 >= placesCount)
            {
                dir = -1;
            }
        }
        else
        {
            if (placeIndex - 1 < placeStart)
            {
                dir = 1;
            }
        }
        placeIndex+=dir;
        var next = places[placeIndex];
        var diff = next.position - start.position;
        var time = diff.magnitude / speed;

        var startPos = start.position;
        var endPos = next.position;
        var startRot = start.rotation;
        var endRot = next.rotation;
        var moveDir = diff.normalized;
        var moveRot = Quaternion.LookRotation(moveDir);

        jumpsCounter++;
        DOTween.To(() => t, x => t = x, 1f, time).OnUpdate(() =>
            {
                var heightT = Mathf.PingPong(t, 0.5f) * 2f;
                var height = heightT * jumpHeight;
                root.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;
                var targetRot = Quaternion.Lerp(startRot, endRot, t);

                if (lookFwd)
                {
                    root.rotation = Quaternion.Lerp(targetRot, moveRot, heightT);   
                }
                else
                {
                    root.rotation = targetRot;
                }
                
            })
            .OnComplete(() =>
            {
                root.position = endPos;
                root.rotation = endRot;
                isInteracting = false;
                if (jumpsCounter >= moneyAfterJumps)
                {
                    jumpsCounter = 0;
                    moneyAfterJumps = Random.Range(moneyAfterJumpsMinMax.x, moneyAfterJumpsMinMax.y+1);
                    var moneyCount = Random.Range(moneyCountMinMax.x, moneyCountMinMax.y + 1);
                    CurrencyStackBehaviour.SpawnSingleCurrency(moneyCount, endPos);
                }
            }).SetEase(Ease.InOutCirc);
    }
}
