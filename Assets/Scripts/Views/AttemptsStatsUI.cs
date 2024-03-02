using UnityEngine;
using TMPro;
using System.Collections.Generic;


public class AttemptsStatsUI : MonoBehaviour, IAttemptsStatsUI
{
    [SerializeField]
    private TextMeshProUGUI attemptsText; 

    private Queue<string> attempts = new Queue<string>(10);

    public void AddAttempt(int fishRareness, string text)
    {        
        if (attempts.Count >= 10)
        {
            //remove the oldest attempt if the queue already has 10 items
            attempts.Dequeue();
        }

        attempts.Enqueue($"{text} fish with rareness: {fishRareness}");

        UpdateUI();
    }

    private void UpdateUI()
    {
        attemptsText.text = "Last 10 Attempts:\n";
        foreach (var attempt in attempts)
        {
            attemptsText.text += attempt + "\n";
        }
    }
    private void OnEnable()
    {
        PlayerControllerV1.OnAttemptAdded += AddAttempt;
    }

    private void OnDisable()
    {
        PlayerControllerV1.OnAttemptAdded -= AddAttempt;
    }
}

