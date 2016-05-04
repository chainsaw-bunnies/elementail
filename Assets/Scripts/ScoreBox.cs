using UnityEngine;
using UnityEngine.UI;

public class ScoreBox : MonoBehaviour
{
  public Text Text;

  void Update()
  {
    var level   = "Level "    + (GameStatus.Level + 1);
    var rituals = "Rituals: " + (GameStatus.RitualPointsRemaining == 0 ? "Done" : GameStatus.RitualPointsRemaining.ToString());
    var score   = "Score: "   + (GameStatus.Score.ToString("N0"));
    Text.text = string.Join("\r\n", new[] { level, rituals, score });
  }
}
