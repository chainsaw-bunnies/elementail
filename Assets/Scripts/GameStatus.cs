using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class GameStatus
{
  public static Root Root;
  public static int Level;
  public static int LevelsCompleted;
  public static int RitualPointsRemaining;
  public static int Score;
  public static int TileSetIndex;

  public static void ActivateRitualPoint()
  {
    RitualPointsRemaining--;
    Score += 100 * Level;
  }

  public static void PlaceRune()
  {
    Score += 10;
  }
}
