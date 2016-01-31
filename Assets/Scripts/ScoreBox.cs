using UnityEngine;
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
    string leavesText = "Rituals: " + (LeavesRemaining == 0 ? "Done" : LeavesRemaining.ToString());
    string scoreText = "Points: " + (Score > 1000000 ? "Wow." : Score.ToString("N0"));

    Text.text = string.Join("\r\n", new[] { levelText, leavesText, scoreText });
  }

  public static void ResetLevel()
  {
    Score = 0;
    Level = 0;
  }

  public static void NextLevel(Map map)
  {
    var floorSizeFactor = map.Level1FloorsizeFactor + (map.FloorsizeFactorIncreasePerLevel * Level);
    var numLeaves = map.Level1NumberOfLeaves + (map.LeafIncreasePerLevel * Level);

    map.CurrentFloorsizeFactor = floorSizeFactor;
    map.CurrentNumberOfLeaves = numLeaves;
    map.CurrentCarrots = Mathf.RoundToInt(Random.Range(numLeaves * 0.33f, numLeaves * 0.66f));
    Level++;
  }
}
