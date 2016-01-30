using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
  public GameObject GroundPrefab;
  public GameObject WallPrefab;
  public GameObject RitualPointPrefab;
  public GameObject PlayerPrefab;

  const int minCorridorWidth = 4;
  const int floorsizeFactor = 20;
  const int numberOfRituals = 4;

  public const float CameraZ = -25f;
  public const float TileSize = 3f;

  const int Height = floorsizeFactor;
  const int Width = floorsizeFactor;

  void Start()
  {
    List<String> validGround = new List<String>();
    Place(PlayerPrefab, (Width - 1) / 2, (Height - 1) / 2);

    for (int y = 0; y < Height; y++)
    {
      for (int x = 0; x < Width; x++)
      {
        if (IsEdge(x, y))
        {
          Place(WallPrefab, x, y);
        }
        else
        {
          Place(GroundPrefab, x, y);
          validGround.Add(x.ToString() + "|" + y.ToString());
        }
      }
    }

    for (int i = 0; i < numberOfRituals; i++)
    {
      int index = UnityEngine.Random.Range(0, validGround.Count - 1);
      string toSplit = validGround[index].ToString();

      string[] coords = toSplit.Split(new Char[] { '|' });
      int x = Convert.ToInt32(coords[0], 10);
      //Int32.TryParse(coords[0], x);
      int y = Convert.ToInt32(coords[1], 10);
      //Int32.TryParse(coords[1], y);

      List<String> adjacentCoords = new List<string>();
      adjacentCoords.Add(((int)(x + 1)).ToString() + "|" + y.ToString());
      adjacentCoords.Add(((int)(x - 1)).ToString() + "|" + y.ToString());
      adjacentCoords.Add(x.ToString() + "|" + ((int)(y + 1)).ToString());
      adjacentCoords.Add(x.ToString() + "|" + ((int)(y - 1)).ToString());

      if (validGround.Contains(adjacentCoords[0]) && validGround.Contains(adjacentCoords[1]) && validGround.Contains(adjacentCoords[2]) && validGround.Contains(adjacentCoords[3]))
      {
        Place(RitualPointPrefab, x, y);
      }
    }

  }

  static GameObject Place(GameObject prefab, int x, int y)
  {
    return (GameObject)GameObject.Instantiate(prefab, GetPos(x, y), Quaternion.identity);
  }

  static bool IsEdge(int x, int y)
  {
    int xDist = Math.Abs(x - (Width - 1) / 2);
    int yDist = Math.Abs(y - (Height - 1) / 2);
    int totalDist = xDist + yDist;

    //return totalDist == 8;
    return ((totalDist + UnityEngine.Random.Range(1, 8)) % 8 == 0);
  }

  static Vector3 GetPos(int x, int y)
  {
    return new Vector3(x * TileSize - (TileSize / 2f), y * TileSize - (TileSize / 2f), 0f);
  }
}
