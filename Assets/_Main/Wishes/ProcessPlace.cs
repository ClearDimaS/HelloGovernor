using System;
using UnityEngine;

public class SimpleProcessPlace : ProcessPlace
{

}

public abstract class ProcessPlace : MonoBehaviour
{
    private CitizenController processed;
    public Vector3 Position => transform.position;

    public void SetOwner(CitizenController citizen)
    {
        if (processed != null)
        {
            throw new NotImplementedException("setting owner of busy place");
        }
        processed = citizen;
    }
    
    public void LeavePlace(CitizenController citizen)
    {
        if (processed != citizen)
        {
            throw new NotImplementedException("leaving place which doesnt belong to me");
        }
        processed = null;
    }
}