using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBox : MonoBehaviour
{
  public Text Text;

  public static int LeavesRemaining;
  public static int LeftRunes;

  void Update()
  {
    Text.text = "Leaves Remaining: " + LeavesRemaining;
  }
}
