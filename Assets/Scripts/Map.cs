using UnityEngine;
using System.Collections;
using System;

public class Map : MonoBehaviour
{
  public GameObject GroundPrefab;
  public GameObject WallPrefab;
  public GameObject RitualPointPrefab;
  public GameObject PlayerPrefab;

  const float CameraZ = -25f;
  const float TileSize = 3f;
  const int Width = 25;
  const int Height = 25;

	void Start()
  {
    Place(PlayerPrefab, (Width-1)/2, (Height-1)/2);

    for (int y = 0; y < Height; y++)
    {
      for (int x = 0; x < Width; x++)
      {
        if (x == 5 && y == 5)
        {
          Place(RitualPointPrefab, x, y);
        }
        else if (IsEdge(x, y))
        {
          Place(WallPrefab, x, y);
        }
        else
        {
          Place(GroundPrefab, x, y);
        }
      }
    }
	}

  static GameObject Place(GameObject prefab, int x, int y)
  {
    return (GameObject)GameObject.Instantiate(prefab, GetPos(x, y), Quaternion.identity);
  }

  static bool IsEdge(int x, int y)
  {
    return x == 0 || y == 0 || x == Width - 1 || y == Height - 1;
  }

  static Vector3 GetPos(int x, int y)
  {
    return new Vector3(x * TileSize - (TileSize / 2f), y * TileSize - (TileSize / 2f), 0f);
  }
}
