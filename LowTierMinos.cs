using UMM;
using UnityEngine;
using HarmonyLib;
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace LowTierMinos
{
    [UKPlugin("ltg.Minos", "Low Tier Minos Speech", "1.0.0", "Replaces Minos Prime's intro with a low tier one.\nOriginal audio: https://www.youtube.com/watch?v=qz5EHyaLhkI", true, false)]
    public class LowTier : UKMod
    {
        private static Harmony harmony;

        internal static AssetBundle LowTierMinosBundle;

        public override void OnModLoaded()
        {
            Debug.Log("end thyself (low tier minos starting)");

            //load the asset bundle
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "lowtierminos";
            {
                LowTierMinosBundle = AssetBundle.LoadFromFile(Path.Combine(ModPath(), resourceName));
            }

            //start harmonylib to swap assets
            harmony = new Harmony("ltg.Minos");
            harmony.PatchAll();
        }

        public static string ModPath()
        {
            return Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(@"\"));
        }

        public override void OnModUnload()
        {
            harmony.UnpatchSelf();
            base.OnModUnload();
        }

        private static SubtitledAudioSource.SubtitleDataLine MakeLine(string subtitle, float time)
        {
            var sub = new SubtitledAudioSource.SubtitleDataLine();
            sub.subtitle = subtitle;
            sub.time = time;
            return sub;
        }

        //use map info to inject data
        [HarmonyPatch(typeof(StockMapInfo), "Awake")]
        internal class Patch00
        {
            static void Postfix(StockMapInfo __instance)
            {
                //try to find dialog in scene and replace it
                foreach (var source in Resources.FindObjectsOfTypeAll<AudioSource>())
                {
                    if (source.clip)
                    {
                        bool replaced = false;
                        var subtitles = new List<SubtitledAudioSource.SubtitleDataLine>();
                        if (source.clip.GetName() == "mp_intro2")
                        {
                            Debug.Log("Replacing minos intro");
                            source.clip = LowTierMinosBundle.LoadAsset<AudioClip>("lowtierminos.ogg");
                            replaced = true;

                            subtitles.Add(MakeLine("You are a worthless", 0f));
                            subtitles.Add(MakeLine("BITCH ASS machine,", 1.2f));
                            subtitles.Add(MakeLine("Your life LITERALLY is as valuable as a", 3.5f));
                            subtitles.Add(MakeLine("summer ant.", 6.0f));
                            subtitles.Add(MakeLine("I'm just gonna stomp you,", 7.6f));
                            subtitles.Add(MakeLine("and you're gonna keep coming back.", 9.25f));
                            subtitles.Add(MakeLine("I'mma seal up all my cracks,", 11.1f));
                            subtitles.Add(MakeLine("you're gonna keep coming back.", 13.3f));
                            subtitles.Add(MakeLine("Why?", 15.5f));
                            subtitles.Add(MakeLine("'Cause you smellin' the blood,", 16.1f));
                            subtitles.Add(MakeLine("you worthless", 17.6f));
                            subtitles.Add(MakeLine("BITCH ASS machine.", 18.6f));
                            subtitles.Add(MakeLine("You're gonna stay on my dick until you DIE.", 20.6f));
                            subtitles.Add(MakeLine("You serve no purpose in life, your purpose in life is to be on my stream sucking on my dick daily.", 23.1f));
                            subtitles.Add(MakeLine("Your purpose in life is to be in that chat BLOWING a dick daily,", 29.3f));
                            subtitles.Add(MakeLine("your life is nothing, you serve ZERO purpose!", 32.7f));
                            subtitles.Add(MakeLine("YOU SHOULD KILL YOURSELF", 35.9f));
                            subtitles.Add(MakeLine("NOW!", 44.0f));
                        }
                        //update subtitles if needed
                        if (replaced)
                        {
                            var subsource = source.GetComponent<SubtitledAudioSource>();
                            Traverse field = Traverse.Create(subsource).Field("subtitles");
                            (field.GetValue() as SubtitledAudioSource.SubtitleData).lines = subtitles.ToArray();
                        }
                    }
                }


            }
        }
    }
}
