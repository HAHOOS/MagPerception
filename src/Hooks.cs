﻿using System.Collections.Generic;

using Il2CppSLZ.Marrow;

namespace NEP.MagPerception
{
    public static class Hooks
    {
        [HarmonyLib.HarmonyPatch(typeof(Magazine), nameof(Magazine.OnGrab))]
        public static class OnMagAttached
        {
            public static Magazine CurrentMagazine { get; private set; } = null;

            public static Hand HoldingHand { get; private set; } = null;

            private readonly static List<Magazine> PatchedMags = [];

            private static Grip.HandDelegate HandDelegate = null;

            public static void Detached(Hand hand)
            {
                if (CurrentMagazine == null)
                    return;

                if (hand?.IsPartOfLocalPlayer() == true)
                    MagPerceptionManager.Instance.OnMagazineDetached(CurrentMagazine);

                if (HandDelegate != null)
                    CurrentMagazine.grip.remove_detachedHandDelegate(HandDelegate);

                CurrentMagazine = null;
                HoldingHand = null;
            }

            public static void Postfix(Hand hand, Magazine __instance)
            {
                if (hand?.IsPartOfLocalPlayer() == false || __instance?.IsMagazineMine() == false)
                    return;

                MagPerceptionManager.Instance.OnMagazineAttached(__instance);
                CurrentMagazine = __instance;
                HoldingHand = hand;
                HandDelegate = (Grip.HandDelegate)Detached;
                __instance.grip.add_detachedHandDelegate(HandDelegate);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnTriggerGripAttached))]
        public static class OnGunAttached
        {
            public static void Postfix(Hand hand, Gun __instance)
            {
                if (hand?.IsPartOfLocalPlayer() == false || __instance?.IsGunMine() == false)
                    return;

                MagPerceptionManager.Instance.OnGunAttached(__instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnTriggerGripDetached))]
        public static class OnGunDetached
        {
            public static void Postfix(Hand hand, Gun __instance)
            {
                if (__instance == null)
                    return;

                if (hand?.IsPartOfLocalPlayer() == false || __instance?.IsGunMine() == false)
                    return;

                MagPerceptionManager.Instance.OnGunDetached();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.EjectCartridge))]
        public static class OnGunEjectRound
        {
            public static void Postfix(Gun __instance)
            {
                if (__instance?.IsGunMine() == false)
                    return;

                MagPerceptionManager.Instance.OnGunEjectRound();
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnMagazineInserted))]
        public static class OnMagazineInserted
        {
            public static void Postfix(Gun __instance)
            {
                if (__instance?.IsGunMine() == false)
                    return;

                MagPerceptionManager.Instance.OnMagazineInserted(__instance);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnMagazineRemoved))]
        public static class OnMagazineRemoved
        {
            public static void Postfix(Gun __instance)
            {
                if (__instance?.IsGunMine() == false)
                    return;

                MagPerceptionManager.Instance.OnMagazineInserted(__instance);
            }
        }
    }
}