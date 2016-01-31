using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
  [Header("Permanent Game Tuning")]
  public int CorridorWidth;
  public float FractionOfDecoratedGroundTiles;
  public float RuneTimeUntilDangerous;
  public float PlayerSpeedIncreasePerLeaf;
  public float PlayerStartSpeed;

  [Header("Level 1 Tuning")]
  public int Level1FloorsizeFactor;
  public int Level1NumberOfLeaves;

  [Header("Next Level Tuning")]
  public int FloorsizeFactorIncreasePerLevel;
  public int LeafIncreasePerLevel;

  [Header("Prefabs")]
  public GameObject CarrotPrefab;
  public GameObject GroundPrefab;
  public GameObject LeafPrefab;
  public GameObject WallPrefab;
  public GameObject Player;

  [HideInInspector]
  public int CurrentFloorsizeFactor;
  [HideInInspector]
  public int CurrentNumberOfLeaves;
  [HideInInspector]
  public int CurrentCarrots;

  [HideInInspector]
  public int Width;
  [HideInInspector]
  public int Height;

  public Dictionary<string, GameObject> Tiles;

  public const float TileSize = 3f;

  void Start()
  {
    ScoreBox.NextLevel(this);

    Width = CurrentFloorsizeFactor;
    Height = CurrentFloorsizeFactor;

    // Put the player in the center of the map.
    Player.transform.position = new Vector3((Width - 1) / 2 * TileSize, (Height - 1) / 2 * TileSize, 0f);
    ScoreBox.LeftRunes = 0;
    ScoreBox.LeavesRemaining = 0;

    Tiles = new Dictionary<string, GameObject>();
    for (int y = 0; y < Height; y++)
    {
      for (int x = 0; x < Width; x++)
      {
        var coord = x.ToString() + "|" + y.ToString();
        if (IsGoodSpotForMapBoundary(x, y))
        {
          // Place where there must be walls.
          Tiles[coord] = Place(WallPrefab, x, y);
          Tiles[coord].GetComponent<Wall>().IsOutermostWall = true;
        }
        else if (IsGoodSpotForObstacle(x, y))
        {
          // Places where there must be walls.
          Tiles[coord] = Place(WallPrefab, x, y);
        }
        else
        {
          // Places available for plain ground or leaves.
          Tiles[coord] = null;
        }
      }
    }

    var prefabsToAdd = new Queue<GameObject>();
    for (int i = 0; i<CurrentNumberOfLeaves; i++)
    {
      prefabsToAdd.Enqueue(LeafPrefab);
    }
    for (int i = 0; i<CurrentCarrots; i++)
    {
      prefabsToAdd.Enqueue(CarrotPrefab);
    }

    var allCoords = new List<string>(Tiles.Keys);
    while (prefabsToAdd.Count > 0)
    {
      int index = UnityEngine.Random.Range(0, allCoords.Count - 1);
      string toSplit = allCoords[index];

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

      if (IsEmptyAndAvailable(toSplit) && IsEmptyAndAvailable(adjacentCoords[0]) && IsEmptyAndAvailable(adjacentCoords[1]) && IsEmptyAndAvailable(adjacentCoords[2]) && IsEmptyAndAvailable(adjacentCoords[3]))
      {
        var prefab = prefabsToAdd.Dequeue();
        Tiles[toSplit] = Place(prefab, x, y);
        if (prefab.GetComponent<Leaf>() != null)
        {
          ScoreBox.LeavesRemaining++;
        }
      }
    }

    // Add plain ground in any remaining empty spots.
    foreach (var coord in allCoords)
    {
      if (Tiles[coord] == null)
      {
        string[] coords = coord.Split(new Char[] { '|' });
        int x = Convert.ToInt32(coords[0], 10);
        //Int32.TryParse(coords[0], x);
        int y = Convert.ToInt32(coords[1], 10);
        //Int32.TryParse(coords[1], y);


        Tiles[coord] = Place(GroundPrefab, x, y);
        var ground = Tiles[coord].GetComponent<Ground>();
        ground.IsDecorated = UnityEngine.Random.value < FractionOfDecoratedGroundTiles;
        ground.Map = this;
      }
    }
  }

  GameObject Place(GameObject prefab, int x, int y)
  {
    return (GameObject)GameObject.Instantiate(prefab, new Vector3(x * TileSize, y * TileSize, 0f), Quaternion.identity);
  }

  bool IsGoodSpotForMapBoundary(int x, int y)
  {
    int xDist = Math.Abs(x - (Width - 1) / 2);
    int yDist = Math.Abs(y - (Height - 1) / 2);
    int totalDist = xDist + yDist;
    return totalDist >= Math.Floor(.5 * CurrentFloorsizeFactor + UnityEngine.Random.Range(0, 2)) - 2;
  }

  bool IsGoodSpotForObstacle(int x, int y)
  {
    int xDist = Math.Abs(x - (Width - 1) / 2);
    int yDist = Math.Abs(y - (Height - 1) / 2);
    int totalDist = xDist + yDist;
    return (totalDist + UnityEngine.Random.Range(1, 8)) % (12 + CorridorWidth) == 0;

  }

  bool IsEmptyAndAvailable(string coord)
  {
    return Tiles.ContainsKey(coord) && Tiles[coord] == null;
  }
}

