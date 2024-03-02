using System;
public static class RandomProportional 
{
    private static readonly System.Random random = new System.Random();

    public static double CustomSample()
    {
        //generate a uniformly distributed random number and then apply the square root to help with uneven distribution of values
        //another approach is to use AnimationCurve - animationCurve.Evaluate(somevalue) - for finer control of the distributions
        //inverted it so that lower values are found more often than the higher ones
        return 1 - Math.Sqrt(random.NextDouble());
    }
    public static int NextRarity()
    {
        //use CustomSample to generate a value between 0.0 and 1.0, then map to 2 to 5
        double sample = CustomSample();
        //adjust the mapping formula to ensure the upper bound can be reached       
        return (int)Math.Floor(2 + sample * 4); //adjusted multiplier
    }

    /*test routine to confirm the results
    void TestRarityDistribution()
    {
        int[] rarityCounts = new int[4]; 

        int testIterations = 10000; 

        for (int i = 0; i < testIterations; i++)
        {
            int rarity = RandomProportional.NextRarity();

            if (rarity >= 2 && rarity <= 5)
            {
                rarityCounts[rarity - 2]++;
            }
        }
        
        Debug.Log($"Rarity Distribution Test over {testIterations} iterations:");
        for (int i = 0; i < rarityCounts.Length; i++)
        {
            Debug.Log($"Rarity {i + 2}: {rarityCounts[i]} occurrences");
        }
    }*/
}
