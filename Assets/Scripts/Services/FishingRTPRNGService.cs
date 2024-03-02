using System.Collections.Generic;
using UnityEngine;
using System;

namespace FishingGame.Services
{

    public class FishingRTPRNGService : MonoBehaviour, IFishingRTPRNGService
    {
        //Use encapsulation to internally validate the provided successRate
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

        private Queue<int> nextCasts = new Queue<int>();
        private System.Random rng;

        public void Awake()
        {
            rng = new System.Random(GenerateOrRetrieveSeed());
            ResetCasts(successRate);
        }

        public bool TryFishCollecting(int currentFishRareness)
        {
            if (nextCasts.Count == 0)
            {
                ResetCasts(successRate);
            }

            int castRarity = nextCasts.Dequeue();
            //compare current fish rarity with the value from the queue
            Debug.Log($"try fish {currentFishRareness} against cast {castRarity}");
            return currentFishRareness <= castRarity;
        }

        private void ResetCasts(int minSuccessRate)
        {
            if (minSuccessRate < 0 || minSuccessRate > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(minSuccessRate), "Success rate must be between 0 and 10");
            }

            // Randomly choose a success rate between minSuccessRate and 10
            int chosenSuccessRate = rng.Next(minSuccessRate, 11);
            List<int> casts = new List<int>();

            //ensure minimum success rate by adding 5s,
            //if successRate is replaced by chosenSuccessRate then for each batch of 10 casts there will be varying success casts with minimum of successRate
            for (int i = 0; i < successRate; i++)
            {
                casts.Add(5); //5 = sure catch, it guarantees no matter of current fish rareness the required cast rareness will be met
            }

            //fill the rest with values 2..5, with proportionally decreasing probabilities, 
            //over 10000 attemps there will be like 600 5s, 2k 4s, 3k 3s, 4,5k 2s
            for (int i = chosenSuccessRate; i < 10; i++)
            {
                casts.Add(RandomProportional.NextRarity());
            }

            Shuffle(casts);

            nextCasts.Clear();
            foreach (int cast in casts)
            {
                nextCasts.Enqueue(cast);
            }
        }



        private void Shuffle<T>(List<T> list)
        {
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