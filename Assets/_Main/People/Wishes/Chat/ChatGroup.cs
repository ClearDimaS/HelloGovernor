using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ChatGroup : MonoBehaviour, IResetable
{
    protected float socialDistancing = 1f;
    protected List<CitizenController> citizens = new ();
    protected int capacity;
    
    public void Initialize(int capacity)
    {
        this.capacity = capacity;
    }

    private void Update()
    {
        foreach (var citizen in citizens)
        {
            if (!citizen.IsChatting)
            {
                foreach (var other in citizens)
                {
                    if (other != citizen)
                    {
                        if ((citizen.transform.position - other.transform.position).magnitude < socialDistancing)
                        {
                            citizen.Walker.MoveToTarget(citizen.transform.position, () => SetChatting(citizen));
                        }
                        else
                        {
                            citizen.Walker.MoveToTarget(other.transform.position, () => SetChatting(citizen));
                        }
                    }
                }
            }
        }
    }
    
    public bool CanAdd(CitizenController citizen)
    {
        return citizens.Count < capacity;
    }

    public void Add(CitizenController citizen)
    {
        citizens.Add(citizen);
    }
    
    public void Remove(CitizenController citizen)
    {
        citizen.transform.DOKill();
        citizen.StopChatting();
        citizens.Remove(citizen);
    }

    public bool HasAnyone()
    {
        return citizens.Count > 0;
    }

    public void OnReset()
    {
        
    }

    public void OnPool()
    {

    }

    public bool HasCitizen(CitizenController citizen)
    {
        return citizens.Contains(citizen);
    }
    
    private void SetChatting(CitizenController citizen)
    {
        citizen.SetChatting();
        if (citizens.Count > 0)
        {
            foreach (var other in citizens)
            {
                if (citizen != other && other.IsChatting)
                {
                    var dir = (other.transform.position - citizen.transform.position).normalized;
                    var rot = Quaternion.LookRotation(dir, Vector3.up);
                    citizen.transform.DORotateQuaternion(rot, 0.3f);   
                    
                    var rotOther = Quaternion.LookRotation(-dir, Vector3.up);
                    other.transform.DORotateQuaternion(rotOther, 0.3f);   
                }
                break;
            }
        }
    }
}