using System.Collections.Generic;
using UnityEngine;
using System;

namespace FishingGame.Services
{
    public class FishingRTPService : MonoBehaviour, IFishingRTPRNGService
    {
        private Queue<bool> nextCasts = new Queue<bool>();
        [SerializeField]
        private int successRate = 3;

        public int SuccessRate
        {
            get { return successRate; }
            set
            {
                if (value > 0 && value < 10)
                    successRate = value;
                else
                    throw new ArgumentOutOfRangeException();
            }

        }

        public FishingRTPService()
        {
            ResetCasts(successRate);
        }

        public bool TryFishCollecting(int rareness)
        {
            if (nextCasts.Count == 0)
            {
                ResetCasts(successRate);
            }
            return nextCasts.Dequeue();
        }
        /// <summary>
        /// fill in the queue with a list of casts bools eg {true,true,true,false,false,false,false,false,false,false} 
        /// thus ensuring there will be guaranteed return to player 
        /// </summary>
        /// <param name="successRate"></param>
        private void ResetCasts(int minSuccessRate)
        {
            System.Random rng = new System.Random(GenerateOrRetrieveSeed());

            if (minSuccessRate < 0 || minSuccessRate > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(minSuccessRate), "Success rate must be between 0 and 10");
            }

            //randomly choose a success rate between minSuccessRate and 10
            int chosenSuccessRate = rng.Next(minSuccessRate, 11);

            List<bool> casts = new List<bool>();

            for (int i = 0; i < chosenSuccessRate; i++)
            {
                casts.Add(true);
            }

            for (int i = chosenSuccessRate; i < 10; i++)
            {
                casts.Add(false);
            }

            Shuffle(casts);

            nextCasts.Clear();
            foreach (bool cast in casts)
            {
                nextCasts.Enqueue(cast);
            }
        }


        private void Shuffle<T>(List<T> list)
        {
            System.Random rng = new System.Random(GenerateOrRetrieveSeed());
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private int GenerateOrRetrieveSeed()
        {
            return DateTime.Now.GetHashCode();
        }
    }
}