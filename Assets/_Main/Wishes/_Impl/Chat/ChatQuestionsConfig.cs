using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ChatQustions", menuName = "Configs/Chats/Questions")]
public class ChatQuestionsConfig : ScriptableObject
{
    public ChatQuestionData[] questions;
}

[Serializable]
public class ChatQuestionData
{
    public string question;

    public string answer1;
    public string answer2;
}