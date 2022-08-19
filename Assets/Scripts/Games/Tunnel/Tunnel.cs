using DG.Tweening;
using NaughtyBezierCurves;
using HeavenStudio.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeavenStudio.Games.Loaders
{
    using static Minigames;
    public static class NtrTunnelLoader
    {
        public static Minigame AddGame(EventCaller eventCaller)
        {
            return new Minigame("tunnel", "Tunnel", "B4E6F6", false, false, new List<GameAction>()
            {


                new GameAction("Cowbell",                 delegate { Tunnel.instance.StartCowbell(eventCaller.currentEntity.beat, eventCaller.currentEntity.toggle, eventCaller.currentEntity.length); }, 1f, true, parameters: new List<Param>()
                {
                    new Param("toggle", false, "Driver can stop", "Lets the driver stop if the player makes too many mistakes"),
                }),

                new GameAction("Count In",                 delegate { Tunnel.instance.CountIn(eventCaller.currentEntity.beat,  eventCaller.currentEntity.length); }, 1f, true),


            }
            //new List<string>() {"ntr", "aim"},
            //"ntrcoin", "en",
            //new List<string>() {}
            );
        }
    }
}

namespace HeavenStudio.Games
{
    //using Scripts_CoinToss;
    public class Tunnel : Minigame
    {

        //Right now, you can only throw one coin at a time.
        //..Which makes sense, you only have one coin in the original game

        //Though it would need a bit of code rewrite to make it work with multiple coins

        public static Tunnel instance { get; set; }

        

        [Header("Backgrounds")]
        public SpriteRenderer fg;
        public SpriteRenderer bg;

        Tween bgColorTween;
        Tween fgColorTween;


        [Header("References")]
        public GameObject frontHand;


        [Header("Animators")]
        public Animator cowbellAnimator;

        [Header("Curves")]
        public BezierCurve3D handCurve;



        public PlayerActionEvent cowbell;




        public int driverState;

        public float handStart;
        public float handProgress;
        public bool started;

        private void Awake()
        {
            instance = this;
        }


        private void Start()
        {
            driverState = 0;
            handStart = -1f;


            cowbell = null;
        }

        private void Update()
        {
            if (PlayerInput.Pressed() && !IsExpectingInputNow())
            {
                HitCowbell();
                print("unexpected input");
            }


            //update hand position
            handProgress = Math.Min(Conductor.instance.songPositionInBeats - handStart, 1);


            frontHand.transform.position = handCurve.GetPoint(EasingFunction.EaseOutQuad(0, 1, handProgress));

        }

        private void LateUpdate()
        {
            //nothing
        }


        public void HitCowbell()
        {
            Jukebox.PlayOneShot("count-ins/cowbell");

            handStart = Conductor.instance.songPositionInBeats;
            
            cowbellAnimator.Play("Shake",-1,0);
        }

        public void StartCowbell(float beat, bool driverStops, float length)
        {
            started = true;


            for (int i = 1; i <= length; i++)
            {
                ScheduleInput(beat, i, InputType.STANDARD_DOWN, CowbellSuccess, CowbellMiss, CowbellEmpty);
            }

            //if (coin != null) return;

            //Play sound and animations

            //handAnimator.Play("Throw", 0, 0);
            //Game state says the hand is throwing the coin
            //isThrowing = true;

            //this.audienceReacting = audienceReacting;

            //coin = ScheduleInput(beat, 6f, InputType.STANDARD_DOWN, CatchSuccess, CatchMiss, CatchEmpty);
            //cowbell = ScheduleInput(beat, 0f, InputType.STANDARD_DOWN, CowbellSuccess, CowbellMiss, CowbellEmpty);
            //coin.perfectOnly = true;
        }

        public void CowbellSuccess(PlayerActionEvent caller, float state)
        {
            HitCowbell();
        }


        public void CowbellMiss(PlayerActionEvent caller)
        {
            //HitCowbell();
        }

        public void CowbellEmpty(PlayerActionEvent caller)
        {
            //HitCowbell();
        }


        
        public void CountIn(float beat, float length)
        {
            for (int i = 0; i <= length; i++)
            {
                if(i % 2 == 0)
                {
                    Jukebox.PlayOneShotGame("tunnel/en/one", beat+i);
                    print("cueing one at " + (beat + i));
                }
                else
                {
                    Jukebox.PlayOneShotGame("tunnel/en/two", beat+i);
                    print("cueing two at " + (beat + i));
                }
                
            }

        }



    }
}
