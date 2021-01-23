using Klei.AI;
using KSerialization;
using UnityEngine;

public class IncubatorX : StorageLocker, ISim4000ms
{
    private KBatchedAnimTracker tracker;
    private Storage storage;
    [MyCmpGet]
    private Operational operational;

    protected override void OnPrefabInit()
    {
        base.OnPrefabInit();
        storage = this.GetComponent<Storage>();
    }

    private void RefreshSong()
    {
        if (!this.operational.IsOperational) return;

        foreach (GameObject egg in storage.items)
        {
            if (!(bool)((UnityEngine.Object)egg)) continue;
            IncubationMonitor.Instance smi = egg.GetSMI<IncubationMonitor.Instance>();
            if (smi == null) continue;
            if (!smi.HasSongBuff()) smi.ApplySongBuff();
        }
    }

    public void Sim4000ms(float dt)
    {
        RefreshSong();
    }
}