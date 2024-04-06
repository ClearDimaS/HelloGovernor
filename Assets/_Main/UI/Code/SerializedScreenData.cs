using System;
using System.Collections.Generic;

[Serializable]
public class SerializedScreenData
{
    public EScreenType type;
    public UI_Screen screen;
    public List<UI_Element> elements;
}