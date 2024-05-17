using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Violoncello.Services;

namespace SecretHostel.DreamRogue {
   public class LevelGenerationState : LevelViewModel.State {
      private LevelGenerationStateConfig _config;

      private Dictionary<string, GameObject> floorTilesPrefabs = new();

      private const string fullTileKey = "full";
      private const string cornerTileKey = "corner";

      private LevelGenerationState(LevelViewModel viewModel, LevelGenerationStateConfig config, IAssetsLoader assetsLoader) : base(viewModel) {
         _config = config;

         var full = assetsLoader.Load<GameObject>("Models", "floor-tile-full");
         var corner = assetsLoader.Load<GameObject>("Models", "floor-tile-corner");

         floorTilesPrefabs = new() {
            { fullTileKey, full },
            { cornerTileKey, corner }
         };
      }

      public override void EnterState() {
         var startRoom = new Room(new Vector2Int(16, 12), (Exits)0b_1111);
         SpawnRoom(startRoom, new Vector3(0.5f, 0.5f));

         
      }

      private void SpawnRoom(Room room, Vector2 position) {
         var offset = position - room.Size / 2;

         var floor = new GameObjectTile[room.Size.x, room.Size.y];

         CreateFloor1x1TilesNoAlloc(floor);
         CreateFloor2x2TilesNoAlloc(floor);
        
         for (int i = 0; i < room.Size.x; i++) {
            for (int j = 0; j < room.Size.y; j++) {
               var tile = floor[i, j];
               var tilePosition = new Vector3(i + offset.x, 0f, j + offset.y);

               PlaceTile(tile.Prefab, tilePosition, Quaternion.Euler(tile.EulerRotation));
            }
         }
      }

      private void CreateFloor1x1TilesNoAlloc(GameObjectTile[,] floor) {
         var width = floor.GetLength(0);
         var height = floor.GetLength(1);

         for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
               floor[x, y] = new GameObjectTile(floorTilesPrefabs[fullTileKey], new Vector2Int(x, y), new Vector3(270f, 0f, 0f));
            }
         }
      }

      private void CreateFloor2x2TilesNoAlloc(GameObjectTile[,] floor) {
         var width = floor.GetLength(0);
         var height = floor.GetLength(1);

         var bigTilesAmount = Mathf.RoundToInt(width * height / 4f * 0.25f);

         var possiblePositions = new List<Vector2Int>();

         for (int x = 0; x < width - 1; x++) {
            for (int y = 0; y < height - 1; y++) {
               possiblePositions.Add(new(x, y));
            }
         }

         for (int i = 0; i < bigTilesAmount; i++) {
            if (possiblePositions.Count == 0) {
               break;
            }

            var bottomLeftPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

            for (int x = bottomLeftPosition.x - 1; x <= bottomLeftPosition.x + 1; x++) {
               for (int y = bottomLeftPosition.y - 1; y <= bottomLeftPosition.y + 1; y++) {
                  possiblePositions.Remove(new(x, y));
               }
            }

            floor[bottomLeftPosition.x, bottomLeftPosition.y] = new GameObjectTile(floorTilesPrefabs[cornerTileKey], bottomLeftPosition, new Vector3(270f, 270f, 0f));
            floor[bottomLeftPosition.x + 1, bottomLeftPosition.y] = new GameObjectTile(floorTilesPrefabs[cornerTileKey], bottomLeftPosition, new Vector3(270f, 180f, 0f));
            floor[bottomLeftPosition.x, bottomLeftPosition.y + 1] = new GameObjectTile(floorTilesPrefabs[cornerTileKey], bottomLeftPosition, new Vector3(270f, 0f, 0f));
            floor[bottomLeftPosition.x + 1, bottomLeftPosition.y + 1] = new GameObjectTile(floorTilesPrefabs[cornerTileKey], bottomLeftPosition, new Vector3(270f, 90f, 0f));
         }
      }

      private void GenerateFloorExits(Room room, Vector2 position) {
         var prefab = floorTilesPrefabs[fullTileKey];
         var rotation = Quaternion.Euler(270f, 0f, 0f);

         if (room.Exits.HasFlag(Exits.Top)) {
            var x = position.x;
            var z = position.y + room.Size.y / 2;

            PlaceTile(prefab, new Vector3(x, 0, z), rotation);
            PlaceTile(prefab, new Vector3(x - 1, 0, z), rotation);
         }

         if (room.Exits.HasFlag(Exits.Bottom)) {
            var x = position.x;
            var z = position.y - room.Size.y / 2 - 1;

            PlaceTile(prefab, new Vector3(x, 0, z), rotation);
            PlaceTile(prefab, new Vector3(x - 1, 0, z), rotation);
         }

         if (room.Exits.HasFlag(Exits.Left)) {
            var x = position.x - room.Size.x / 2 - 1;
            var z = position.y;

            PlaceTile(prefab, new Vector3(x, 0, z), rotation);
            PlaceTile(prefab, new Vector3(x, 0, z - 1), rotation);
         }

         if (room.Exits.HasFlag(Exits.Right)) {
            var x = position.x + room.Size.x / 2;
            var z = position.y;

            PlaceTile(prefab, new Vector3(x, 0, z), rotation);
            PlaceTile(prefab, new Vector3(x, 0, z - 1), rotation);
         }
      }

      private void PlaceTile(GameObject prefab, Vector3 position, Quaternion rotation) {
         var tile = Object.Instantiate(prefab, position, rotation, GroundTilemap.transform);

         tile.transform.localScale = Vector3.one;
         tile.isStatic = true;
      }

      private class GameObjectTile {
         public Vector3 EulerRotation { get; }
         public Vector2Int GridPosition { get; }
         public GameObject Prefab { get; }

         public GameObjectTile(GameObject prefab, Vector2Int gridPosition, Vector3 eulerRotation) {
            GridPosition = gridPosition;
            Prefab = prefab;
            EulerRotation = eulerRotation;
         }
      }

      private class Room {
         public Vector2Int Size { get; }
         public Exits Exits { get; }
         
         public Room(Vector2Int size, Exits exits) {
            Size = size;
            Exits = exits;
         }
      }

      private class Exit {
         public Vector2 TopRightElementPosition { get; }
         public Vector2 Direction { get; }

         public Exit(Vector2 topRightElementPosition, Vector2 direction) {
            TopRightElementPosition = topRightElementPosition;
            Direction = direction;
         }
      }

      [System.Flags]
      private enum Exits { 
         Top = 1,
         Bottom = 2,
         Left = 4,
         Right = 8
      }

      public class Factory {
         private IAssetsLoader _assetsLoader;
         private LevelGenerationStateConfig _config;

         public Factory(LevelGenerationStateConfig config, IAssetsLoader assetsLoader) {
            _config = config;
            _assetsLoader = assetsLoader;
         }

         public LevelGenerationState Create(LevelViewModel viewModel) {
            return new LevelGenerationState(viewModel, _config, _assetsLoader);
         }
      }
   }
}
