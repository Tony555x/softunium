using System;

namespace Harduni.Statuses;

public static class StatusRouter
{
    public static Status? CreateStatus(string typeName)
    {
        return typeName switch
        {
            nameof(DecayStatus) => new DecayStatus(),
            nameof(PersistentAtkStatus) => new PersistentAtkStatus(),
            nameof(PersistentDefStatus) => new PersistentDefStatus(),
            nameof(PersistentRegenStatus) => new PersistentRegenStatus(),
            nameof(PersistentPoisonousStatus) => new PersistentPoisonousStatus(),
            _ => null
        };
    }
}
