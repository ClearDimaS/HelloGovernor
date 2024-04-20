using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField] protected List<SerializedScreenData> screenDatas;

    protected Dictionary<EScreenType, SerializedScreenData> screenDatasDict = new ();
    private Dictionary<Type, UI_Panel> panelsDict = new ();

    private LinkedList<UI_Element> elementsStack = new ();
    
    protected override void OnCreated()
    {
        base.OnCreated();
        var UIElements = transform.GetComponentsInDirectChildren<UI_Element>();
        
        panelsDict = UIElements.Where(x => x.GetType().IsSubclassOf(typeof(UI_Panel))).Select(x => (UI_Panel)x).
            ToDictionary(x => x.GetType(), x => x);
        screenDatasDict = screenDatas.ToDictionary(x => x.type, x => x);

        foreach (var panel in panelsDict)
        {
            panel.Value.gameObject.SetActive(false);
        }

        Debug.Log($"panels count: {panelsDict.Count}");
        foreach (var screen in screenDatasDict)
        {
            screen.Value.screen.gameObject.SetActive(false);
        }
    }

    public void OpenScreen(EScreenType screenType)
    {
        foreach (var uiElement in elementsStack)
        {
            uiElement.Hide();
        }
        elementsStack.Clear();
        
        var data = screenDatasDict[screenType];
        
        data.screen.SetOrder(elementsStack.Count);
        elementsStack.AddLast(data.screen);
        data.screen.Show();
        
        foreach (var element in data.elements)
        {
            element.SetOrder(elementsStack.Count);
            elementsStack.AddLast(element);
            element.Show();
        }
    }
    
    public void CloseScreen(EScreenType screenType)
    {
        var data = screenDatasDict[screenType];
        if (!elementsStack.Contains(data.screen))
        {
            return;
        }
        
        data.screen.Hide();
        elementsStack.Remove(data.screen);
        
        foreach (var element in data.elements)
        {
            elementsStack.Remove(element);
            element.Hide();
        }
    }

    public void OpenPanel<T>() where T : UI_Panel
    {
        var panel = panelsDict[typeof(T)];
        
        panel.SetOrder(elementsStack.Count);
        elementsStack.AddLast(panel);
        panel.Show();
    }
    
    public T GetPanel<T>() where T : UI_Panel
    {
        var panel = panelsDict[typeof(T)];
        return panel as T;
    }
    
    public void ClosePanel<T>() where T : UI_Panel
    {
        var panel = panelsDict[typeof(T)];
        
        elementsStack.Remove(panel);
        panel.Hide();
    }
}
