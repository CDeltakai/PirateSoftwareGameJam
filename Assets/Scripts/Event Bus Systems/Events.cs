using System;
using UnityEngine;

public interface IEvent { }

public struct NPCKilledEvent : IEvent
{
    public StageEntity npc;
    public NPCKilledEvent(StageEntity npc)
    {
        this.npc = npc;
    }
}
