using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public DateTime registrationDate = DateTime.Today;
    public int levelIndex = 0;
    public int LevelNumber => levelIndex + 1;
}

public class PlayerDataRepository : Repository<PlayerData>
{
    
}