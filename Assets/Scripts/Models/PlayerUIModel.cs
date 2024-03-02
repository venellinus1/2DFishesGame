using System;
using UnityEngine;
[System.Serializable]
public class PlayerUIModel
{
    private int collectedFish;
    public int CollectedFish
    {
        get => collectedFish;
        set
        {
            if (collectedFish != value)
            {
                collectedFish = value;
                NotifyDataChanged();
            }
        }
    }

    private int attemptsCount;
    public int AttemptsCount
    {
        get => attemptsCount;
        set
        {
            if (attemptsCount != value)
            {
                attemptsCount = value;
                NotifyDataChanged();
            }
        }
    }

    public event Action OnDataChanged;

    protected void NotifyDataChanged()
    {
        OnDataChanged?.Invoke();
    }
}
