using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Violoncello.Services;

namespace SecretHostel.DreamRogue {
   public class LevelGenerationState : LevelViewModel.State {
      private LevelGenerationStateConfig _config;

      private List<Material> floorTilesMaterials;
      private Dictionary<string, GameObject> floorTilesPrefabs;

      private const string fullTileKey = "full";
      private const string cornerTileKey = "corner";
      private const string peninsulaTileKey = "peninsula";

      private LevelGenerationState(LevelViewModel viewModel, LevelGenerationStateConfig config, IAssetsLoader assetsLoader) : base(viewModel) {
         _config = config;

         floorTilesPrefabs = new() {
            { fullTileKey, assetsLoader.Load<GameObject>("Models", "floor-tile-full") },
            { cornerTileKey, assetsLoader.Load<GameObject>("Models", "floor-tile-corner") },
            { peninsulaTileKey, assetsLoader.Load<GameObject>("Models", "floor-tile-peninsula") }
         };

         floorTilesMaterials = new() {
            { assetsLoader.Load<Material>("Materials", "floor-white") }, 
            { assetsLoader.Load<Material>("Materials", "floor-lightgray") },
            { assetsLoader.Load<Material>("Materials", "floor-gray") },
            { assetsLoader.Load<Material>("Materials", "floor-darkgray") }
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

         CreateFloor2x2TilesNoAlloc(floor, 0.2f); // Doing it in 2 turns results in more beautiful patterns
         CreateFloor1x2TilesNoAlloc(floor, 0.1f);
         CreateFloor2x1TilesNoAlloc(floor, 0.1f);

         CreateFloor2x2TilesNoAlloc(floor, 0.1f);
         CreateFloor1x2TilesNoAlloc(floor, 0.1f);
         CreateFloor2x1TilesNoAlloc(floor, 0.1f);

         for (int i = 0; i < room.Size.x; i++) {
            for (int j = 0; j < room.Size.y; j++) {
               var tile = floor[i, j];
               var tilePosition = new Vector3(i + offset.x, 0f, j + offset.y);

               PlaceTile(tile.Prefab, tilePosition, Quaternion.Euler(tile.EulerRotation), tile.Material);
            }
         }
      }

      private void CreateFloor1x1TilesNoAlloc(GameObjectTile[,] floor) {
         var width = floor.GetLength(0);
         var height = floor.GetLength(1);

         for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
               var prefab = floorTilesPrefabs[fullTileKey];
               var material = floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)];

               floor[x, y] = new GameObjectTile(prefab, new Vector2Int(x, y), new Vector3(270f, 0f, 0f), material);
            }
         }
      }

      private void CreateFloor2x2TilesNoAlloc(GameObjectTile[,] floor, float concentration) {
         var width = floor.GetLength(0);
         var height = floor.GetLength(1);

         var bigTilesAmount = Mathf.RoundToInt(width * height / 4f * concentration);

         var possiblePositions = new List<Vector2Int>();

         for (int x = 0; x < width - 1; x++) {
            for (int y = 0; y < height - 1; y++) {
               if (floor[x, y].Prefab != floorTilesPrefabs[fullTileKey] ||
                   floor[x, y + 1].Prefab != floorTilesPrefabs[fullTileKey] ||
                   floor[x + 1, y].Prefab != floorTilesPrefabs[fullTileKey] ||
                   floor[x + 1, y + 1].Prefab != floorTilesPrefabs[fullTileKey]
               ) {
                  continue;
               }

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

            var prefab = floorTilesPrefabs[cornerTileKey];
            var material = floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)];

            floor[bottomLeftPosition.x, bottomLeftPosition.y] = new GameObjectTile(prefab, bottomLeftPosition, new Vector3(270f, 270f, 0f), material);
            floor[bottomLeftPosition.x + 1, bottomLeftPosition.y] = new GameObjectTile(prefab, bottomLeftPosition, new Vector3(270f, 180f, 0f), material);
            floor[bottomLeftPosition.x, bottomLeftPosition.y + 1] = new GameObjectTile(prefab, bottomLeftPosition, new Vector3(270f, 0f, 0f), material);
            floor[bottomLeftPosition.x + 1, bottomLeftPosition.y + 1] = new GameObjectTile(prefab, bottomLeftPosition, new Vector3(270f, 90f, 0f), material);
         }
      }

      private void CreateFloor1x2TilesNoAlloc(GameObjectTile[,] floor, float concentration) {
         var width = floor.GetLength(0);
         var height = floor.GetLength(1);

         var longTilesAmount = Mathf.RoundToInt(width * height / 4f * concentration);

         var possiblePositions = new List<Vector2Int>();

         for (int x = 0; x < width; x++) {
            for (int y = 0; y < height - 1; y++) {
               if (floor[x, y].Prefab != floorTilesPrefabs[fullTileKey]) {
                  continue;
               }

               if (floor[x, y + 1].Prefab != floorTilesPrefabs[fullTileKey]) {
                  continue;
               }

               possiblePositions.Add(new(x, y));
            }
         }

         for (int i = 0; i < longTilesAmount; i++) {
            if (possiblePositions.Count == 0) {
               break;
            }

            var bottomPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

            for (int y = bottomPosition.y - 1; y <= bottomPosition.y + 1; y++) {
               possiblePositions.Remove(new(bottomPosition.x, y));
            }

            var prefab = floorTilesPrefabs[peninsulaTileKey];
            var material = floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)];

            floor[bottomPosition.x, bottomPosition.y] = new GameObjectTile(prefab, bottomPosition, new Vector3(270f, 0f, 0f), material);
            floor[bottomPosition.x, bottomPosition.y + 1] = new GameObjectTile(prefab, bottomPosition, new Vector3(270f, 180f, 0f), material);
         }
      }

      private void CreateFloor2x1TilesNoAlloc(GameObjectTile[,] floor, float concentration) {
         var width = floor.GetLength(0);
         var height = floor.GetLength(1);

         var longTilesAmount = Mathf.RoundToInt(width * height / 4f * concentration);

         var possiblePositions = new List<Vector2Int>();

         for (int x = 0; x < width - 1; x++) {
            for (int y = 0; y < height; y++) {
               if (floor[x, y].Prefab != floorTilesPrefabs[fullTileKey]) {
                  continue;
               }

               if (floor[x + 1, y].Prefab != floorTilesPrefabs[fullTileKey]) {
                  continue;
               }

               possiblePositions.Add(new(x, y));
            }
         }

         for (int i = 0; i < longTilesAmount; i++) {
            if (possiblePositions.Count == 0) {
               break;
            }

            var leftPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

            for (int x = leftPosition.x - 1; x <= leftPosition.x + 1; x++) {
               possiblePositions.Remove(new(x, leftPosition.y));
            }

            var prefab = floorTilesPrefabs[peninsulaTileKey];
            var material = floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)];

            floor[leftPosition.x, leftPosition.y] = new GameObjectTile(prefab, leftPosition, new Vector3(270f, 90f, 0f), material);
            floor[leftPosition.x + 1, leftPosition.y] = new GameObjectTile(prefab, leftPosition, new Vector3(270f, 270f, 0f), material);
         }
      }

      private void GenerateFloorExits(Room room, Vector2 position) {
         var prefab = floorTilesPrefabs[fullTileKey];
         var rotation = Quaternion.Euler(270f, 0f, 0f);

         if (room.Exits.HasFlag(Exits.Top)) {
            var x = position.x;
            var z = position.y + room.Size.y / 2;

            PlaceTile(prefab, new Vector3(x, 0, z), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
            PlaceTile(prefab, new Vector3(x - 1, 0, z), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
         }

         if (room.Exits.HasFlag(Exits.Bottom)) {
            var x = position.x;
            var z = position.y - room.Size.y / 2 - 1;

            PlaceTile(prefab, new Vector3(x, 0, z), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
            PlaceTile(prefab, new Vector3(x - 1, 0, z), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
         }

         if (room.Exits.HasFlag(Exits.Left)) {
            var x = position.x - room.Size.x / 2 - 1;
            var z = position.y;

            PlaceTile(prefab, new Vector3(x, 0, z), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
            PlaceTile(prefab, new Vector3(x, 0, z - 1), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
         }

         if (room.Exits.HasFlag(Exits.Right)) {
            var x = position.x + room.Size.x / 2;
            var z = position.y;

            PlaceTile(prefab, new Vector3(x, 0, z), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
            PlaceTile(prefab, new Vector3(x, 0, z - 1), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
         }
      }

      private void PlaceTile(GameObject prefab, Vector3 position, Quaternion rotation, Material material) {
         var tile = Object.Instantiate(prefab, position, rotation, GroundTilemap.transform);

         tile.transform.localScale = Vector3.one;
         tile.isStatic = true;

         var meshRenderer = tile.GetComponent<MeshRenderer>();

         meshRenderer.material = material;
      }

      private class GameObjectTile {
         public Vector3 EulerRotation { get; }
         public Vector2Int GridPosition { get; }
         public GameObject Prefab { get; }
         public Material Material { get; }

         public GameObjectTile(GameObject prefab, Vector2Int gridPosition, Vector3 eulerRotation, Material material) {
            GridPosition = gridPosition;
            Prefab = prefab;
            EulerRotation = eulerRotation;
            Material = material;
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
