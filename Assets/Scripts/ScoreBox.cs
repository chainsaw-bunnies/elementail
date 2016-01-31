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
    string leavesText;
    if (LeavesRemaining > 0)
    {
      leavesText = "Hop " + LeavesRemaining + " Leaves";
    }
    else
    {
      leavesText = "Ritual Completed!";
    }

    Text.text = leavesText;
  }
}
