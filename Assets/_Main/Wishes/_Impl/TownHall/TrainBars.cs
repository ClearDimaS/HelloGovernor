using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class TrainBars : MonoBehaviour
{
    [SerializeField] private Transform bar;
    [SerializeField] private Vector3 hereRot;
    [SerializeField] private Vector3 awayRot;
    [SerializeField] private bool isHere;
    [SerializeField] private NavMeshObstacle obstacle;
    [SerializeField] private BoxCollider box;
    
    private void Update()
    {
        var newIsHere = TrainBehaviour.Instance.IsHere;
        if (newIsHere != isHere)
        {
            isHere = newIsHere;
            if (isHere)
            {
                SetHere();
            }
            else
            {
                SetAway();
            }
        }
    }

    private void SetHere()
    {
        obstacle.enabled = true;
        box.enabled = true;
        bar.transform.DOLocalRotate(hereRot, 0.3f);
    }

    private void SetAway()
    {
        obstacle.enabled = false;
        box.enabled = false;
        bar.transform.DOLocalRotate(awayRot, 0.3f);
    }
}