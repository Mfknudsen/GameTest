using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.AI
{
    public class SoldierAIDirector : MonoBehaviour
    {
        public Transform Player;
        public List<SoldierAI> All = new List<SoldierAI>();
        List<SoldierAI> Close = new List<SoldierAI>();
        List<SoldierAI> Far = new List<SoldierAI>();

        List<SoldierAI> CandidatesFromCloseToFar = new List<SoldierAI>();
        List<SoldierAI> CandidatesFromFarToClose = new List<SoldierAI>();

        public int CloseCycle = 10;
        int CloseAdd;
        public int FarCycle = 30;
        int FarAdd;

        public float MaxDistance = 25;

        void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player").transform;
            SoldierAI[] a = GameObject.FindObjectsOfType<SoldierAI>();
            All.AddRange(a);

            for (int i = 0; i < All.Count; i++)
            {
                float Distnace = Vector3.Distance(Player.position, All[i].transform.position);

                if (Distnace < MaxDistance)
                    Close.Add(All[i]);
                else
                    Far.Add(All[i]);

                All[i].PlayerTarget = Player;
            }
        }

        void Update()
        {
            CloseAdd++;
            FarAdd++;

            if (CloseAdd > CloseCycle)
            {
                CloseAdd = 0;

                for (int i = 0; i < Close.Count; i++)
                {
                    Close[i].Update();

                    float Distance = Vector3.Distance(Player.position, Close[i].transform.position);

                    if (Distance > MaxDistance)
                        CandidatesFromCloseToFar.Add(Close[i]);
                }

                for (int i = 0; i < CandidatesFromCloseToFar.Count; i++)
                {
                    Close.Remove(CandidatesFromCloseToFar[i]);
                    Far.Add(CandidatesFromCloseToFar[i]);
                }

                return;
            }
            if (FarAdd > FarCycle)
            {
                FarAdd = 0;
                for (int i = 0; i < Far.Count; i++)
                {
                    Far[i].Update();

                    float Distance = Vector3.Distance(Player.position, Far[i].transform.position);

                    if (Distance < MaxDistance)
                        CandidatesFromFarToClose.Add(Far[i]);
                }

                for (int i = 0; i < CandidatesFromFarToClose.Count; i++)
                {
                    Close.Remove(CandidatesFromFarToClose[i]);
                    Far.Add(CandidatesFromFarToClose[i]);
                }
            }
        }
    }
}