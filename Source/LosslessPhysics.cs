using UnityEngine;
using HarmonyLib;

[KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
[DefaultExecutionOrder(100)]
public class LosslessPhysics : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);

        var harmony = new Harmony("com.metalbass.LosslessPhysics");
        harmony.PatchAll();
    }
}


[HarmonyPatch(typeof(TimeWarp), "fixedDeltaTime", MethodType.Getter)]
class TimeWarp_fixedDeltaTime_return_50Hz
{
    static void Postfix(ref float __result)
    {
        __result = 1f / 50f;
    }
}


[HarmonyPatch(typeof(TimeWarp), "updateRate")]
class TimeWarp_updateRate_set_Time_fixedDeltaTime_to_50Hz
{
    static void Postfix()
    {
        Time.fixedDeltaTime = 1f / 50f;
    }
}


[HarmonyPatch(typeof(Planetarium), "FixedUpdate")]
class Planetarium_FixedUpdate_uses_TimeScale_One_during_TimeWarp_mode_LOW
{
    static void Prefix(out double __state)
    {
        __state = Planetarium.TimeScale;

        if (TimeWarp.fetch?.Mode == TimeWarp.Modes.LOW)
        {
            Planetarium.TimeScale = 1;
        }
    }

    static void Postfix(double __state)
    {
        Planetarium.TimeScale = __state;
    }
}
