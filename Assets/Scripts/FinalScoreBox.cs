using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FinalScoreBox : MonoBehaviour
{
  public Text Text;

  void Update()
  {
    Text.text = "Final Score: " + GameStatus.Score.ToString("N0");
  }
}
