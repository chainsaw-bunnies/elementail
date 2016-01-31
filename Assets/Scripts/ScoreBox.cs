﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBox : MonoBehaviour
{
  public Text Text;

  public static int Level { get; private set; }
  public static int LeavesRemaining;
  public static int LeftRunes;
  public static int Score;

  void Update()
  {
    string levelText = "Level " + Level; 
    string leavesText = "Rituals: " + (LeavesRemaining == 0 ? "Complete" : LeavesRemaining.ToString() + " Left");
    string scoreText = "Points: " + Score;

    Text.text = string.Join("\r\n", new[] { levelText, leavesText, scoreText });
  }

  public static void ResetLevel()
  {
    Level = 0;
  }

  public static void NextLevel(Map map)
  {
    map.CurrentFloorsizeFactor = map.Level1FloorsizeFactor + (map.FloorsizeFactorIncreasePerLevel * Level);
    map.CurrentNumberOfLeaves = map.Level1NumberOfLeaves + (map.LeafIncreasePerLevel * Level);
    Level++;
  }
}
