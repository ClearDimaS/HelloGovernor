using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PressConference", menuName = "Configs/PressConference/Topics")]
public class PressConferenceConfig : ScriptableObject
{
    public PressConferenceTopicData[] datas;

    [Button]
    protected void ReSplitSentences()
    {
        foreach (var data in datas)
        {
            if (data.sentences.Length == 1)
            {
                data.sentences = SplitTextIntoSentences(data.sentences[0]).ToArray();
            }
        }
    }
    
    private List<string> SplitTextIntoSentences(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new List<string>(); // Return an empty list if the input is null or empty
        }

        // Split the text into sentences using '.', followed by a space or end of string
        string[] sentences = text.Split(new[] { ". " }, StringSplitOptions.None);
        
        List<string> result = new List<string>();
        for (int i = 0; i < sentences.Length; i++)
        {
            // Trim any leading/trailing whitespace
            string sentence = sentences[i].Trim();

            // If it's not the last sentence, replace '.' with '...'
            if (i < sentences.Length - 1)
            {
                result.Add(sentence + "...");
            }
            else
            {
                // For the last sentence, add it as is (with the dot)
                if (!sentence.EndsWith("."))
                {
                    sentence += ".";
                }
                result.Add(sentence);
            }
        }

        return result;
    }
}

[Serializable]
public class PressConferenceTopicData
{
    public string title;
    public string[] sentences;
}