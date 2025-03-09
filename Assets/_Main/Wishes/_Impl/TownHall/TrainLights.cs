using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainLights : MonoBehaviour
{
    [SerializeField] private GameObject[] trainHereGOs;
    [SerializeField] private GameObject[] trainAwayGOs;

    private void Update()
    {
        var isHere = TrainBehaviour.Instance.IsHere;
        trainHereGOs.SetActiveOnce(isHere);
        trainAwayGOs.SetActiveOnce(!isHere);
    }
}