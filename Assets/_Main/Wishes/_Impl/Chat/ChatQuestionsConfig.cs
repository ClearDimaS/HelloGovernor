using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ChatQustions", menuName = "Configs/Chats/Questions")]
public class ChatQuestionsConfig : ScriptableObject
{
    public ChatQuestionData[] questions;
}

[Serializable]
public class ChatQuestionData
{
    public LocalizedString questionLocalized;
    public LocalizedString answer1_Localized;
    public LocalizedString answer2_Localized;

    public string question => questionLocalized.GetLocalizedString();

    public string answer1 => answer1_Localized.GetLocalizedString();
    public string answer2 => answer2_Localized.GetLocalizedString();
}

