using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Violoncello.Extensions;
using Violoncello.Services;

namespace SecretHostel.DreamRogue {
   public class LevelGenerationState : LevelViewModel.State {
      private LevelGenerationStateConfig _config;

      private List<Material> floorTilesMaterials;
      private Dictionary<string, GameObject> floorTilesPrefabs;

      private const string fullTileKey = "full";
      private const string cornerTileKey = "corner";
      private const string peninsulaTileKey = "peninsula";

      private List<Bounds> allRoomsBounds;

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

         allRoomsBounds = new();
      }

      public override void EnterState() {
         GenerateLevelBox();
      }

      private void GenerateLevelBox() {
         var levelSize = new Vector2Int(6, 6);
         var chunks = new ChunkType[levelSize.x, levelSize.y];

         var chunkSize = new Vector2Int(12, 12);
         var offset = new Vector2(0.5f, 0.5f);

         Set1x1ChunksNoAlloc(chunks);
         Set2x2ChunksNoAlloc(chunks, 0.1f);
         Set1x2ChunksNoAlloc(chunks, 0.1f);
         Set2x1ChunksNoAlloc(chunks, 0.1f);
         Set2x2BottomLeftCornerChunksNoAlloc(chunks, 0.1f);
         Set2x2BottomRightCornerChunksNoAlloc(chunks, 0.1f);
         Set2x2TopLeftCornerChunksNoAlloc(chunks, 0.1f);
         Set2x2TopRightCornerChunksNoAlloc(chunks, 0.1f);

         for (int x = 0; x < levelSize.x; x++) {
            for (int y = 0; y < levelSize.y; y++) {
               var position = new Vector2(x + x * chunkSize.x, y + y * chunkSize.y) + offset;

               var chunk = new Chunk(position, chunkSize, type: chunks[x, y]);

               SpawnChunk(chunk);
            }
         }
      }

      private void Set1x1ChunksNoAlloc(ChunkType[,] chunks) {
         var width = chunks.GetLength(0);
         var height = chunks.GetLength(1);

         for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
               chunks[x, y] = ChunkType.Normal;
            }
         }
      }

      private void Set2x2ChunksNoAlloc(ChunkType[,] chunks, float concentration) {
         var width = chunks.GetLength(0);
         var height = chunks.GetLength(1);

         var bigRoomsAmount = Mathf.RoundToInt(width * height / 4f * concentration);

         var possiblePositions = new List<Vector2Int>();

         for (int x = 0; x < width - 1; x++) {
            for (int y = 0; y < height - 1; y++) {
               if (chunks[x, y] != ChunkType.Normal ||
                   chunks[x, y + 1] != ChunkType.Normal ||
                   chunks[x + 1, y] != ChunkType.Normal ||
                   chunks[x + 1, y + 1] != ChunkType.Normal
               ) {
                  continue;
               }

               possiblePositions.Add(new(x, y));
            }
         }

         for (int i = 0; i < bigRoomsAmount; i++) {
            if (possiblePositions.Count == 0) {
               break;
            }

            var bottomLeftPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

            for (int x = bottomLeftPosition.x - 1; x <= bottomLeftPosition.x + 1; x++) {
               for (int y = bottomLeftPosition.y - 1; y <= bottomLeftPosition.y + 1; y++) {
                  possiblePositions.Remove(new(x, y));
               }
            }

            chunks[bottomLeftPosition.x, bottomLeftPosition.y] = ChunkType.BottomLeftCorner;
            chunks[bottomLeftPosition.x + 1, bottomLeftPosition.y] = ChunkType.BottomRightCorner;
            chunks[bottomLeftPosition.x, bottomLeftPosition.y + 1] = ChunkType.TopLeftCorner;
            chunks[bottomLeftPosition.x + 1, bottomLeftPosition.y + 1] = ChunkType.TopRightCorner;
         }
      }

      private void Set1x2ChunksNoAlloc(ChunkType[,] chunks, float concentration) {
         var width = chunks.GetLength(0);
         var height = chunks.GetLength(1);

         var bigRoomsAmount = Mathf.RoundToInt(width * height / 4f * concentration);

         var possiblePositions = new List<Vector2Int>();

         for (int x = 0; x < width; x++) {
            for (int y = 0; y < height - 1; y++) {
               if (chunks[x, y] != ChunkType.Normal ||
                   chunks[x, y + 1] != ChunkType.Normal
               ) {
                  continue;
               }

               possiblePositions.Add(new(x, y));
            }
         }

         for (int i = 0; i < bigRoomsAmount; i++) {
            if (possiblePositions.Count == 0) {
               break;
            }

            var bottomLeftPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

            for (int x = bottomLeftPosition.x - 1; x <= bottomLeftPosition.x + 1; x++) {
               for (int y = bottomLeftPosition.y - 1; y <= bottomLeftPosition.y + 1; y++) {
                  possiblePositions.Remove(new(x, y));
               }
            }

            chunks[bottomLeftPosition.x, bottomLeftPosition.y] = ChunkType.BottomPeninsula;
            chunks[bottomLeftPosition.x, bottomLeftPosition.y + 1] = ChunkType.TopPeninsila;
         }
      }

      private void Set2x1ChunksNoAlloc(ChunkType[,] chunks, float concentration) {
         var width = chunks.GetLength(0);
         var height = chunks.GetLength(1);

         var bigRoomsAmount = Mathf.RoundToInt(width * height / 4f * concentration);

         var possiblePositions = new List<Vector2Int>();

         for (int x = 0; x < width - 1; x++) {
            for (int y = 0; y < height; y++) {
               if (chunks[x, y] != ChunkType.Normal ||
                   chunks[x + 1, y] != ChunkType.Normal
               ) {
                  continue;
               }

               possiblePositions.Add(new(x, y));
            }
         }

         for (int i = 0; i < bigRoomsAmount; i++) {
            if (possiblePositions.Count == 0) {
               break;
            }

            var bottomLeftPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

            for (int x = bottomLeftPosition.x - 1; x <= bottomLeftPosition.x + 1; x++) {
               for (int y = bottomLeftPosition.y - 1; y <= bottomLeftPosition.y + 1; y++) {
                  possiblePositions.Remove(new(x, y));
               }
            }

            chunks[bottomLeftPosition.x, bottomLeftPosition.y] = ChunkType.LeftPeninsula;
            chunks[bottomLeftPosition.x + 1, bottomLeftPosition.y] = ChunkType.RightPeninsula;
         }
      }

      private void Set2x2BottomLeftCornerChunksNoAlloc(ChunkType[,] chunks, float concentration) {
         var width = chunks.GetLength(0);
         var height = chunks.GetLength(1);

         var bigRoomsAmount = Mathf.RoundToInt(width * height / 4f * concentration);

         var possiblePositions = new List<Vector2Int>();

         for (int x = 0; x < width - 1; x++) {
            for (int y = 0; y < height - 1; y++) {
               if (chunks[x, y] != ChunkType.Normal ||
                   chunks[x, y + 1] != ChunkType.Normal ||
                   chunks[x + 1, y] != ChunkType.Normal
               ) {
                  continue;
               }

               possiblePositions.Add(new(x, y));
            }
         }

         for (int i = 0; i < bigRoomsAmount; i++) {
            if (possiblePositions.Count == 0) {
               break;
            }

            var bottomLeftPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

            for (int x = bottomLeftPosition.x - 1; x <= bottomLeftPosition.x + 1; x++) {
               for (int y = bottomLeftPosition.y - 1; y <= bottomLeftPosition.y + 1; y++) {
                  possiblePositions.Remove(new(x, y));
               }
            }

            chunks[bottomLeftPosition.x, bottomLeftPosition.y] = ChunkType.BottomLeftCorner;
            chunks[bottomLeftPosition.x + 1, bottomLeftPosition.y] = ChunkType.RightPeninsula;
            chunks[bottomLeftPosition.x, bottomLeftPosition.y + 1] = ChunkType.TopPeninsila;
         }
      }

      private void Set2x2BottomRightCornerChunksNoAlloc(ChunkType[,] chunks, float concentration) {
         var width = chunks.GetLength(0);
         var height = chunks.GetLength(1);

         var bigRoomsAmount = Mathf.RoundToInt(width * height / 4f * concentration);

         var possiblePositions = new List<Vector2Int>();

         for (int x = 0; x < width - 1; x++) {
            for (int y = 0; y < height - 1; y++) {
               if (chunks[x, y] != ChunkType.Normal ||
                   chunks[x + 1, y] != ChunkType.Normal ||
                   chunks[x + 1, y + 1] != ChunkType.Normal
               ) {
                  continue;
               }

               possiblePositions.Add(new(x, y));
            }
         }

         for (int i = 0; i < bigRoomsAmount; i++) {
            if (possiblePositions.Count == 0) {
               break;
            }

            var bottomLeftPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

            for (int x = bottomLeftPosition.x - 1; x <= bottomLeftPosition.x + 1; x++) {
               for (int y = bottomLeftPosition.y - 1; y <= bottomLeftPosition.y + 1; y++) {
                  possiblePositions.Remove(new(x, y));
               }
            }

            chunks[bottomLeftPosition.x, bottomLeftPosition.y] = ChunkType.LeftPeninsula;
            chunks[bottomLeftPosition.x + 1, bottomLeftPosition.y] = ChunkType.BottomPeninsula;
            chunks[bottomLeftPosition.x + 1, bottomLeftPosition.y + 1] = ChunkType.TopPeninsila;
         }
      }

      private void Set2x2TopLeftCornerChunksNoAlloc(ChunkType[,] chunks, float concentration) {
         var width = chunks.GetLength(0);
         var height = chunks.GetLength(1);

         var bigRoomsAmount = Mathf.RoundToInt(width * height / 4f * concentration);

         var possiblePositions = new List<Vector2Int>();

         for (int x = 0; x < width - 1; x++) {
            for (int y = 0; y < height - 1; y++) {
               if (chunks[x, y] != ChunkType.Normal ||
                   chunks[x, y + 1] != ChunkType.Normal ||
                   chunks[x + 1, y + 1] != ChunkType.Normal
               ) {
                  continue;
               }

               possiblePositions.Add(new(x, y));
            }
         }

         for (int i = 0; i < bigRoomsAmount; i++) {
            if (possiblePositions.Count == 0) {
               break;
            }

            var bottomLeftPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

            for (int x = bottomLeftPosition.x - 1; x <= bottomLeftPosition.x + 1; x++) {
               for (int y = bottomLeftPosition.y - 1; y <= bottomLeftPosition.y + 1; y++) {
                  possiblePositions.Remove(new(x, y));
               }
            }

            chunks[bottomLeftPosition.x, bottomLeftPosition.y] = ChunkType.BottomPeninsula;
            chunks[bottomLeftPosition.x, bottomLeftPosition.y + 1] = ChunkType.LeftPeninsula;
            chunks[bottomLeftPosition.x + 1, bottomLeftPosition.y + 1] = ChunkType.RightPeninsula;
         }
      }

      private void Set2x2TopRightCornerChunksNoAlloc(ChunkType[,] chunks, float concentration) {
         var width = chunks.GetLength(0);
         var height = chunks.GetLength(1);

         var bigRoomsAmount = Mathf.RoundToInt(width * height / 4f * concentration);

         var possiblePositions = new List<Vector2Int>();

         for (int x = 0; x < width - 1; x++) {
            for (int y = 0; y < height - 1; y++) {
               if (chunks[x + 1, y] != ChunkType.Normal ||
                   chunks[x, y + 1] != ChunkType.Normal ||
                   chunks[x + 1, y + 1] != ChunkType.Normal
               ) {
                  continue;
               }

               possiblePositions.Add(new(x, y));
            }
         }

         for (int i = 0; i < bigRoomsAmount; i++) {
            if (possiblePositions.Count == 0) {
               break;
            }

            var bottomLeftPosition = possiblePositions[Random.Range(0, possiblePositions.Count)];

            for (int x = bottomLeftPosition.x - 1; x <= bottomLeftPosition.x + 1; x++) {
               for (int y = bottomLeftPosition.y - 1; y <= bottomLeftPosition.y + 1; y++) {
                  possiblePositions.Remove(new(x, y));
               }
            }

            chunks[bottomLeftPosition.x + 1, bottomLeftPosition.y] = ChunkType.BottomPeninsula;
            chunks[bottomLeftPosition.x, bottomLeftPosition.y + 1] = ChunkType.LeftPeninsula;
            chunks[bottomLeftPosition.x + 1, bottomLeftPosition.y + 1] = ChunkType.RightPeninsula;
         }
      }

      private void RemoveRoomImpossibleExits(Chunk room) {
         room.Exits &= ~GetExcludeExits(room);
      }

      private Exits GetExcludeExits(Chunk room) {
         var roomPosition = room.Position.ToXZYVector3();
         var roomSize = room.Size.ToXZYVector3();

         var roomHalfWidth = roomSize.But(z: 0) / 2;
         var roomHalfLength = roomSize.But(x: 0) / 2;

         var offset = 6f;

         var checkPositions = new Dictionary<Exits, Vector3> {
            { Exits.Top, roomPosition + (roomHalfLength + Vector3.forward * offset) },
            { Exits.Bottom, roomPosition - (roomHalfLength + Vector3.forward * offset) },
            { Exits.Right, roomPosition + (roomHalfWidth + Vector3.right * offset) },
            { Exits.Left, roomPosition - (roomHalfWidth + Vector3.right * offset) }
         };

         Exits excludeExits = 0;

         Debug.Log($"Room at {roomPosition}");

         foreach (var pair in checkPositions) {
            var exit = pair.Key;
            var position = pair.Value;

            var bounds = new Bounds(position, Vector3.one);

            var result = allRoomsBounds.Any((b) => b.Intersects(bounds));

            if (result) {
               excludeExits |= exit;
            }

            Debug.Log($"{exit} => {result}");
         }

         return excludeExits;
      }

      private void SpawnChunk(Chunk chunk) {
         var offset = chunk.Position - chunk.Size / 2;

         var size = chunk.Type switch {
            ChunkType.BottomLeftCorner => chunk.Size + Vector2Int.one,
            ChunkType.BottomRightCorner => chunk.Size + Vector2Int.up,
            ChunkType.TopLeftCorner => chunk.Size + Vector2Int.right,

            ChunkType.LeftPeninsula => chunk.Size + Vector2Int.right,
            ChunkType.BottomPeninsula => chunk.Size + Vector2Int.up,

            _ => chunk.Size
         } ;

         var floor = new GameObjectTile[size.x, size.y];

         CreateFloor1x1TilesNoAlloc(floor);

         CreateFloor2x2TilesNoAlloc(floor, 0.2f); // Doing it in 2 turns results in more beautiful patterns
         CreateFloor1x2TilesNoAlloc(floor, 0.1f);
         CreateFloor2x1TilesNoAlloc(floor, 0.1f);

         CreateFloor2x2TilesNoAlloc(floor, 0.1f);
         CreateFloor1x2TilesNoAlloc(floor, 0.1f);
         CreateFloor2x1TilesNoAlloc(floor, 0.1f);

         for (int i = 0; i < size.x; i++) {
            for (int j = 0; j < size.y; j++) {
               var tile = floor[i, j];
               var tilePosition = new Vector3(i + offset.x, 0f, j + offset.y);

               PlaceTile(tile.Prefab, tilePosition, Quaternion.Euler(tile.EulerRotation), tile.Material);
            }
         }

        // GenerateFloorExits(chunk);
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

      private void GenerateFloorExits(Chunk room) {
         var prefab = floorTilesPrefabs[fullTileKey];
         var rotation = Quaternion.Euler(270f, 0f, 0f);

         if (room.Exits.HasFlag(Exits.Top)) {
            var x = room.Position.x;
            var z = room.Position.y + room.Size.y / 2;

            PlaceTile(prefab, new Vector3(x, 0, z), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
            PlaceTile(prefab, new Vector3(x - 1, 0, z), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
         }

         if (room.Exits.HasFlag(Exits.Bottom)) {
            var x = room.Position.x;
            var z = room.Position.y - room.Size.y / 2 - 1;

            PlaceTile(prefab, new Vector3(x, 0, z), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
            PlaceTile(prefab, new Vector3(x - 1, 0, z), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
         }

         if (room.Exits.HasFlag(Exits.Left)) {
            var x = room.Position.x - room.Size.x / 2 - 1;
            var z = room.Position.y;

            PlaceTile(prefab, new Vector3(x, 0, z), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
            PlaceTile(prefab, new Vector3(x, 0, z - 1), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
         }

         if (room.Exits.HasFlag(Exits.Right)) {
            var x = room.Position.x + room.Size.x / 2;
            var z = room.Position.y;

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

      private class Chunk {
         public Vector2 Position { get; set; }
         public Vector2Int Size { get; set; }
         public Exits Exits { get; set; }
         public ChunkType Type { get; set; }

         public Chunk(Vector2 position, Vector2Int size, Exits exits = default, ChunkType type = default) {
            Position = position;
            Size = size;
            Exits = exits;
            Type = type;
         }
      }

      [System.Flags]
      private enum Exits {
         Top = 1,
         Bottom = 2,
         Left = 4,
         Right = 8
      }

      private enum ChunkType {
         Normal,
         TopLeftCorner,
         TopRightCorner,
         BottomLeftCorner,
         BottomRightCorner,
         LeftPeninsula,
         RightPeninsula,
         TopPeninsila,
         BottomPeninsula
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
