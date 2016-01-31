using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBox : MonoBehaviour
{
  public Text Text;

  public static int LeavesRemaining;

  void Update()
  {
    Text.text = "Leaves Remaining: " + LeavesRemaining;
  }
}
