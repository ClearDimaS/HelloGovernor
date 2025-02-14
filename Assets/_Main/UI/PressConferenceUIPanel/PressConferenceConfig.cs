using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PressConference", menuName = "Configs/PressConference/Topics")]
public class PressConferenceConfig : ScriptableObject
{
    public PressConferenceTopicData[] datas;
}

[Serializable]
public class PressConferenceTopicData
{
    public string[] sentences;
}