using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
  public int CorridorWidth;
  public int FloorsizeFactor;
  public int NumberOfLeaves;
  public GameObject GroundPrefab;
  public GameObject LeafPrefab;
  public GameObject PlayerPrefab;
  public GameObject WallPrefab;

  public int Width { get { return FloorsizeFactor; } }
  public int Height { get { return FloorsizeFactor; } }

  public Dictionary<string, GameObject> Tiles;

  public const float TileSize = 3f;

  void Start()
  {
    // Put the player in the center of the map.
    Place(PlayerPrefab, (Width - 1) / 2, (Height - 1) / 2);

    Tiles = new Dictionary<string, GameObject>();
    for (int y = 0; y < Height; y++)
    {
      for (int x = 0; x < Width; x++)
      {
        var coord = x.ToString() + "|" + y.ToString();
        if (IsEdge(x, y))
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
    var allCoords = new List<string>(Tiles.Keys);

    for (int i = 0; i < NumberOfLeaves; i++)
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


      if (Tiles[adjacentCoords[0]] == null && Tiles[adjacentCoords[1]] == null && Tiles[adjacentCoords[2]] == null && Tiles[adjacentCoords[3]] == null)
      {
        Tiles[toSplit] = Place(LeafPrefab, x, y);
      }
      else
      {
        i--;
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
      }
    }
  }

  GameObject Place(GameObject prefab, int x, int y)
  {
    return (GameObject)GameObject.Instantiate(prefab, new Vector3(x * TileSize, y * TileSize, 0f), Quaternion.identity);
  }

  bool IsEdge(int x, int y)
  {
    int xDist = Math.Abs(x - (Width - 1) / 2);
    int yDist = Math.Abs(y - (Height - 1) / 2);
    int totalDist = xDist + yDist;
    return ((totalDist + UnityEngine.Random.Range(1, 8)) % (12 + CorridorWidth) == 0 || totalDist >= Math.Floor(.5 * FloorsizeFactor + UnityEngine.Random.Range(0, 2)) - 2);
  }
}
