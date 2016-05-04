using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class Root : MonoBehaviour
{
  [Header("References")]
  public Image Fade;

  [Header("Tuning")]
  public int ClearTilesNearCenter;
  public int CorridorWidth;
  public float FractionOfDecoratedTiles;
  public int Level1Size;
  public int SizeIncreasePerLevel;
  public int Level1RitualPointCount;
  public float PercentageOfSuperCarrots;
  public int RitualPointIncreasePerLevel;
  public float RuneTimeUntilDangerous;
  public float Level1HopDuration;
  public float HopDurationIncreasePerLevel;
  public float HopPauseDuration;

  [Header("Data")]
  public Pickup Carrot;
  public Pickup SuperCarrot;
  public RuneSet[] RuneSets;
  public TileSet[] TileSets;
  public SoundBank SoundBank;
  public SpriteBank SpriteBank;
  public GameObject PlayerPrefab;
  public RuntimeAnimatorController RitualPointAnimation;

  [HideInInspector] public MusicPlayer MusicPlayer;
  [HideInInspector] public Player Player;
  [HideInInspector] public PlayerCamera PlayerCamera;
  [HideInInspector] public SoundPlayer SoundPlayer;
  [HideInInspector] public Tile[,] Tiles;

  void Start()
  {
    var start = System.DateTime.UtcNow;

    GameStatus.Level = GameStatus.LevelsCompleted;
    GameStatus.Root = this;
    GameStatus.RitualPointsRemaining = Level1RitualPointCount + RitualPointIncreasePerLevel * GameStatus.Level;
    GameStatus.TileSetIndex = GameStatus.Level % TileSets.Length;

    var size = Level1Size + SizeIncreasePerLevel * GameStatus.Level;
    Tiles = new Tile[size, size];
    var numPickups = Mathf.RoundToInt(Random.Range(GameStatus.RitualPointsRemaining * 0.33f, GameStatus.RitualPointsRemaining * 0.66f));
    var tileSet = TileSets[GameStatus.TileSetIndex];

    // Map generation: Place the outermost wall, obstacles, and passable ground.
    for (int x = 0; x < size; x++)
    {
      for (int y = 0; y < size; y++)
      {
        if (ShouldPlaceOuterWall(x, y, size))
        {
          Tiles[x, y] = MakeOuterWall(x, y, tileSet);
        }
        else if (ShouldPlaceObstacle(x, y, size, CorridorWidth))
        {
          Tiles[x, y] = MakeObstacle(x, y, tileSet);
        }
        else
        {
          Tiles[x, y] = MakeGroundTile(x, y, tileSet, Random.value < FractionOfDecoratedTiles);
        }
      }
    }

    // Map generation: Place ritual points at random coordinates which are clear in all four directions.
    for (int i = 0; i < GameStatus.RitualPointsRemaining; i++)
    {
      GetRandomClearTile(size, ClearTilesNearCenter, Tiles).AddRitualPoint(RitualPointAnimation);
    }

    // Map generation: Place pickups at random coordinates which are clear in all four directions.
    for (int i = 0; i < numPickups; i++)
    {
      GetRandomClearTile(size, ClearTilesNearCenter, Tiles).AddPickup(Random.value < PercentageOfSuperCarrots ? SuperCarrot : Carrot);
    }

    // Place all tiles in a single container.
    var tilesRoot = new GameObject("Tiles");
    foreach (var tile in Tiles)
    {
      tile.transform.SetParent(tilesRoot.transform);
    }

    MusicPlayer = gameObject.AddComponent<MusicPlayer>();
    Player = ((GameObject)Instantiate(PlayerPrefab, new Vector3((size - 1) / 2, (size - 1) / 2, 0f), Quaternion.identity)).GetComponent<Player>();
    PlayerCamera = GameObject.Find("Camera").AddComponent<PlayerCamera>();
    SoundPlayer = gameObject.AddComponent<SoundPlayer>();

    MusicPlayer.Play(tileSet.Music);

    Debug.Log("Level Generation: " + (System.DateTime.UtcNow - start).TotalMilliseconds.ToString("N0") + "ms");
  }

  static Tile GetRandomClearTile(int size, int avoidCenterDistance, Tile[,] tiles)
  {
    while (true)
    {
      var x = Random.Range(1, size - 1);
      var y = Random.Range(1, size - 1);

      // Don't return positions too close to the center.
      var distanceToCenter = ManhattanDistanceFromCenter(x, y, size);
      if (distanceToCenter < avoidCenterDistance)
      {
        continue;
      }

      // Don't return positions with an obstacle or pickup that is too close.
      var someTiles = new[] { tiles[x, y], tiles[x - 1, y], tiles[x + 1, y], tiles[x, y - 1], tiles[x, y + 1] };
      if (someTiles.Any(t => t.IsObstacle || t.IsPickup || t.IsRitualPoint))
      {
        continue;
      }

      return tiles[x, y];
    }
  }

  static float ManhattanDistanceFromCenter(int x, int y, int size)
  {
    var xDist = Mathf.Abs(x - (size - 1) / 2);
    var yDist = Mathf.Abs(y - (size - 1) / 2);
    return xDist + yDist;
  }

  static bool ShouldPlaceOuterWall(int x, int y, int size)
  {
    return ManhattanDistanceFromCenter(x, y, size) >= Mathf.Floor(.5f * size + Random.Range(0, 2)) - 2;
  }

  static bool ShouldPlaceObstacle(int x, int y, int size, int corridorWidth)
  {
    return (ManhattanDistanceFromCenter(x, y, size) + Random.Range(1, 8)) % (12 + corridorWidth) == 0;
  }

  static Tile MakeTile(int x, int y)
  {
    var go = new GameObject("Tile");
    go.transform.position = new Vector3(x, y, 0);
    return go.AddComponent<Tile>();
  }

  static Tile MakeGroundTile(int x, int y, TileSet tileSet, bool decorate)
  {
    var sprite = (decorate && tileSet.DecoratedBackground.Any() ? tileSet.DecoratedBackground : tileSet.PlainBackground).RandomElement();
    var tile = MakeTile(x, y);
    tile.AddSprite(sprite);
    return tile;
  }

  static Tile MakeObstacle(int x, int y, TileSet tileSet)
  {
    var tile = MakeTile(x, y);
    tile.AddSprite(tileSet.PlainBackground.RandomElement());
    tile.AddObstacle(tileSet.Obstacle.RandomElement());
    return tile;
  }

  static Tile MakeOuterWall(int x, int y, TileSet tileSet)
  {
    var tile = MakeTile(x, y);
    tile.AddSprite(tileSet.PlainBackground.RandomElement());
    tile.AddObstacle(tileSet.OuterWall.RandomElement());
    return tile;
  }

  public void ExitLevel(string fadeCompleteScene)
  {
    GameStatus.Root.Fade.color = new Color(0f, 0f, 0f, 0f);
    GameStatus.Root.Fade.DOFade(1f, 0.5f).SetUpdate(true).OnComplete(() => SceneManager.LoadScene(fadeCompleteScene));
  }
}
