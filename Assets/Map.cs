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
  const int Width = 10;
  const int Height = 10;

	void Start()
  {
    var player = (GameObject)GameObject.Instantiate(PlayerPrefab, GetPos(4, 4), Quaternion.identity);
    Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, CameraZ);

    for (int y = 0; y < Height; y++)
    {
      for (int x = 0; x < Width; x++)
      {
        if (IsEdge(x, y))
        {
          PlaceTile(WallPrefab, x, y);
        }
        else
        {
          PlaceTile(GroundPrefab, x, y);
        }
      }
    }
	}

  static GameObject PlaceTile(GameObject prefab, int x, int y)
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
