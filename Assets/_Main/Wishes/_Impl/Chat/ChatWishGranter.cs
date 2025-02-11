using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class ChatWishGranter : UIWishGranter
{
    [Inject] private ChatGroupsPool groupsPool;

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
                group.Initialize(Random.Range(wishesCollectionConfig.chatGroupSizeMinMax.x, wishesCollectionConfig.chatGroupSizeMinMax.x));
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

    protected override Vector3 GetQueuePlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }

    protected override Vector3 GetProcessPlaceFor(CitizenController citizen)
    {
        return citizen.transform.position;
    }

    protected override Quaternion GetProcessRotFor(CitizenController citizen)
    {
        return citizen.transform.rotation;
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