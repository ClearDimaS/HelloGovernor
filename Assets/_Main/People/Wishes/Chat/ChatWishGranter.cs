using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class ChatWishGranter : WishGranter
{
    [Inject] private ChatGroupsPool groupsPool;
    [Inject] private GameConfig gameConfig;
    
    public override EWish Type => EWish.Chat;
    public override float FullProgressTime => gameConfig.chatDuration;
    public override int Reward => 0;

    protected HashSet<CitizenController> groupedCitizens = new ();
    private List<ChatGroup> activeChatGroups = new ();

    protected override void UpdateProcessed(CitizenController citizen)
    {
        base.UpdateProcessed(citizen);
        if (!groupedCitizens.Contains(citizen))
        {
            foreach (var group in activeChatGroups)
            {
                if (group.CanAdd(citizen))
                {
                    group.Add(citizen);
                    groupedCitizens.Add(citizen);
                }
            }
            
            if (!groupedCitizens.Contains(citizen))
            {
                ChatGroup group = groupsPool.GetElement();
                group.Initialize(Random.Range(gameConfig.chatGroupSizeMinMax.x, gameConfig.chatGroupSizeMinMax.x));
                activeChatGroups.Add(group);
                
                group.Add(citizen);
                groupedCitizens.Add(citizen);
            }
        }
    }

    protected override void OnRemoveFromProcessed(CitizenController citizen)
    {
        base.OnRemoveFromProcessed(citizen);
        if (groupedCitizens.Contains(citizen))
        {
            groupedCitizens.Remove(citizen);
            var group = activeChatGroups.First(x => x.HasCitizen(citizen));
            group.Remove(citizen);
            if (!group.HasAnyone())
            {
                activeChatGroups.Remove(group);
                groupsPool.Pool(group);
            }
        }
    }

    protected override bool CanAddProgress(CitizenController citizen)
    {
        return citizen.IsChatting;
    }
    
    protected override Vector3 GetQueuePlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }

    protected override Vector3 GetProcessPlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }

    protected override Vector3 GetExitPlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }

    public override bool CanAddOneMore()
    {
        return true;
    }
}