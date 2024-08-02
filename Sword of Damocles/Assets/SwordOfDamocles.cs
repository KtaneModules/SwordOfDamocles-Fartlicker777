using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class SwordOfDamocles : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;

   public KMSelectable Button;
   bool Activated;

   public GameObject Sword; //Z -0.0265 -> -0.0792
   public GameObject SwordRope;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   int ModsNotSolved;
   int Frames;
   int Strikes;
   bool BlowUp;
   bool ModCheck;
   bool Started;
   bool StopBlowUp;

   int ModsForFortyPercent;
   int SolvesSinceStrike;

   bool BossModulePresent;
   public static string[] ignoredModules = null;
   int StoredSolves;

   Coroutine Idle;

   int Solved;

   void Awake () {
      ModuleId = ModuleIdCounter++;
      /*
      foreach (KMSelectable object in keypad) {
          object.OnInteract += delegate () { keypadPress(object); return false; };
      }
      */

      Button.OnInteract += delegate () { Press(); return false; };

      if (ignoredModules == null) {
         ignoredModules = GetComponent<KMBossModule>().GetIgnoredModules("Sword of Damocles", new string[] {
                "14",
                "42",
                "501",
                "A>N<D",
                "Bamboozling Time Keeper",
                "Black Arrows",
                "Brainf---",
                "The Board Walk",
                "Busy Beaver",
                "Don't Touch Anything",
                "Floor Lights",
                "Forget Any Color",
                "Forget Enigma",
                "Forget Ligma",
                "Forget Everything",
                "Forget Infinity",
                "Forget It Not",
                "Forget Maze Not",
                "Forget Me Later",
                "Forget Me Not",
                "Forget Perspective",
                "Forget The Colors",
                "Forget Them All",
                "Forget This",
                "Forget Us Not",
                "Iconic",
                "Keypad Directionality",
                "Kugelblitz",
                "Multitask",
                "OmegaDestroyer",
                "OmegaForest",
                "Organization",
                "Password Destroyer",
                "Purgatory",
                "Reporting Anomalies",
                "RPS Judging",
                "Security Council",
                "Shoddy Chess",
                "Simon Forgets",
                "Simon's Stages",
                "Souvenir",
                "Speech Jammer",
                "Tallordered Keys",
                "The Time Keeper",
                "Timing is Everything",
                "The Troll",
                "Turn The Key",
                "The Twin",
                "Übermodule",
                "Ultimate Custom Night",
                "The Very Annoying Button",
                "WAR",
                "Whiteout"
            });
         }
      }

   private void Start () {
      Idle = StartCoroutine(Sway());
      if (Bomb.GetSolvableModuleNames().Count(x => ignoredModules.Contains(x)) > 0) {
         BossModulePresent = true;
      }
   }

   IEnumerator Sway () {
      var duration = 1f;
      var elapsed = 0f;
      while (true) {
         elapsed = 0;
         duration = 1f;
         duration += Rnd.Range(-.1f, .1f);
         while (elapsed < duration) {
            SwordRope.transform.localEulerAngles = new Vector3(90, 0, Mathf.Lerp(0, .13f, elapsed / duration));
            yield return null;
            elapsed += Time.deltaTime;
         }
         elapsed = 0;
         duration = 1f;
         duration += Rnd.Range(-.1f, .1f);
         while (elapsed < duration) {
            SwordRope.transform.localEulerAngles = new Vector3(90, 0, Mathf.Lerp(.13f, 0, elapsed / duration));
            yield return null;
            elapsed += Time.deltaTime;
         }
         elapsed = 0;
         duration = 1f;
         duration += Rnd.Range(-.1f, .1f);
         while (elapsed < duration) {
            SwordRope.transform.localEulerAngles = new Vector3(90, 0, Mathf.Lerp(0, -.13f, elapsed / duration));
            yield return null;
            elapsed += Time.deltaTime;
         }
         elapsed = 0;
         duration = 1f;
         duration += Rnd.Range(-.1f, .1f);
         while (elapsed < duration) {
            SwordRope.transform.localEulerAngles = new Vector3(90, 0, Mathf.Lerp(-.13f, 0, elapsed / duration));
            yield return null;
            elapsed += Time.deltaTime;
         }
      }
   }

   void Press () {
      if (ModuleSolved && !BossModulePresent) {
         return;
      }
      Activated = true;
      GetComponent<KMBombModule>().HandlePass();
      ModuleSolved = true;
      StartCoroutine(PushIn());
      if (BossModulePresent && StoredSolves > 0) {
         SolveRandomMod();
         StoredSolves--;
      }
      
   }

   IEnumerator PushIn () {
      Button.AddInteractionPunch();
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
      for (int i = 0; i < 5; i++) {
         Button.transform.localPosition += new Vector3(0, -0.001F, 0);
         yield return new WaitForSeconds(0.005F);
      }
      yield return new WaitForSeconds(0.005F);
      for (int i = 0; i < 5; i++) {
         Button.transform.localPosition += new Vector3(0, 0.001F, 0);
         yield return new WaitForSeconds(0.005F);
      }
   }

   void SolveRandomMod () {
      Restart:
      int InitSolved = Bomb.GetSolvedModuleNames().Count();
      int AntiSoftlock = 0; //In case shit like morseamaze is green and that's the only mod left or something;
      KMBombModule Random = GetComponent<Transform>().parent.parent.GetComponentsInChildren<KMBombModule>().PickRandom();
      //KMStatusLightParent StatusLightFromMod = Random.GetComponentInChildren<KMStatusLightParent>();


      string weed = Random.ModuleDisplayName;
      /*int AntiSoftlock = 0;

      List<string> Solvable = Bomb.GetSolvableModuleNames().ToList();
      string[] SolvedCalc = Bomb.GetSolvedModuleNames().ToArray();

      for (int i = 0; i < SolvedCalc.Length; i++) {
         Solvable.Remove(SolvedCalc[i]);
      }

      //Debug.Log(Solvable.Count());
      //return;
      */
      if (Bomb.GetSolvableModuleNames().Count() - Bomb.GetSolvedModuleNames().Count() == 0) {
         return;
      }

      do {
         Random = GetComponent<Transform>().parent.parent.GetComponentsInChildren<KMBombModule>().PickRandom();
         weed = Random.ModuleDisplayName;
         //StatusLightFromMod = Random.GetComponentInChildren<KMStatusLightParent>();
         //AntiSoftlock++;
         AntiSoftlock++;
      } while (IsSLGreen(Random) && AntiSoftlock < 1000);
      /*if (AntiSoftlock >= 10000) { !Solvable.Contains(weed) && AntiSoftlock < 10000 && 
         return;
      }*/
      Random.HandlePass();
      if (InitSolved + 1 != Bomb.GetSolvedModuleNames().Count()) {
         goto Restart;
      }
      Solved++;
      Debug.LogFormat("[Sword of Damocles #{0}] Solving \"{1}\".", ModuleId, weed);
      //Debug.Log(Random.ModuleDisplayName);
   }

   bool IsSLGreen (KMBombModule mod) {
      //Debug.Log(L);
      Debug.Log("Trying mod " + mod.ModuleDisplayName);
      Transform SL = mod.transform.Find("Status Light");
      if (SL == null) {
         Debug.Log("Could not find Status Light");
         return false;
      }
      Transform L = SL.transform.Find("statusLight(Clone)");
      if (L == null) {
         L = SL.transform.Find("StatusLight(Clone)");
      }
      if (L == null) { //Indeterminate, so we pass as true in case it's a mod that has no SL.
         Debug.Log("This module apparently does not have a status light");
         return false;
      }
      if (false) {
         StartCoroutine(DeathAnim()); //I HATE Blananas2!
      }
      return L.Find("Component_LED_PASS").gameObject.activeSelf;
   }

   IEnumerator Death () {
      var duration = .133f;
      var elapsed = 0f;
      while (!BlowUp) {
         //yield return new WaitForSeconds(.133f);
         while (elapsed < duration) { //For testing with magic stopwatch since I don't know which one gets sped up
            yield return null;
            elapsed += Time.deltaTime;
         }
         elapsed = 0f;
         /*Frames++;
         Debug.Log(BlowUp);
         if (Frames % 4 == 0 && !BlowUp) {
            BlowUp = 108 == Rnd.Range(0, 10000);
         }*/
         BlowUp = 108 == Rnd.Range(0, 10000);
      }
   }

   IEnumerator DeathAnim () {
      StopCoroutine(Idle);
      var duration = .1f;
      var elapsed = 0f;


      while (elapsed < duration) {
         Sword.transform.localPosition = new Vector3(-3f, Mathf.Lerp(-1.021831f, -1.03418f, elapsed / duration), -0.8654426f);
         yield return null;
         elapsed += Time.deltaTime;
      }

      while (true) {
         yield return new WaitForSeconds(.01f);
         GetComponent<KMBombModule>().HandleStrike();
      }
   }

   void Update () {
      if (!ModuleSolved) {
         if (Bomb.GetSolvableModuleNames().Count() > ModsForFortyPercent) {
            ModsForFortyPercent = Mathf.RoundToInt(Bomb.GetSolvableModuleNames().Count() * .4f);
         }
         if (ModsForFortyPercent < Bomb.GetSolvedModuleNames().Count() && SolvesSinceStrike < Bomb.GetSolvedModuleNames().Count()) {
            GetComponent<KMBombModule>().HandleStrike();
            while (SolvesSinceStrike < Bomb.GetSolvedModuleNames().Count()) {
               SolvesSinceStrike++;
            }
         }
         return;
      }
      if (!ModCheck) {
         //ModsNotSolved = Bomb.GetSolvableModuleNames().Count() - Bomb.GetSolvedModuleNames().Count();
         Solved = Bomb.GetSolvedModuleNames().Count();
         Strikes = Bomb.GetStrikes();
         ModCheck = true;
      }

      if (Strikes < Bomb.GetStrikes() && !Started) {
         Started = true;
         StartCoroutine(Death());
      }

      if (BlowUp && !StopBlowUp) {
         StartCoroutine(DeathAnim());
         StopBlowUp = true;
      }

      if (Solved < Bomb.GetSolvedModuleNames().Count()) {
         Solved++;
         if (!BossModulePresent) {
            SolveRandomMod();
         }
         else {
            StoredSolves++;
         }
      }
      
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} UltraSolve to gain the fortune of a king, but at a cost.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
      Button.OnInteract();
   }

   IEnumerator TwitchHandleForcedSolve () {
      yield return ProcessTwitchCommand("UltraPeanor");
   }
}
