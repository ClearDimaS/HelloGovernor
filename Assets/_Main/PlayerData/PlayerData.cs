using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public DateTime registrationDate = DateTime.Today;
    public int skinIndex;
    public int money;
}

public class PlayerDataRepository : Repository<PlayerData>
{
    
}