using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Violoncello.Extensions;
using Violoncello.Services;
using Violoncello.Utilities;

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
         GenerateLevelBoxAsync();
      }

      private void GenerateLevelBoxAsync() {
         var levelSize = new Vector2Int(_config.MaxWidth, _config.MaxHeight);
         var chunks = new ChunkType[levelSize.x, levelSize.y];

         var offset = new Vector2(0.5f, 0.5f);

         CreateLevelShapeNoAlloc(chunks, 20);
         Set2x2ChunksNoAlloc(chunks, 0.1f);
         Set1x2ChunksNoAlloc(chunks, 0.1f);
         Set2x1ChunksNoAlloc(chunks, 0.1f);
         Set2x2BottomLeftCornerChunksNoAlloc(chunks, 0.1f);
         Set2x2BottomRightCornerChunksNoAlloc(chunks, 0.1f);
         Set2x2TopLeftCornerChunksNoAlloc(chunks, 0.1f);
         Set2x2TopRightCornerChunksNoAlloc(chunks, 0.1f);

         SpawnExits(chunks);

         for (int x = 0; x < levelSize.x; x++) {
            for (int y = 0; y < levelSize.y; y++) {
               var position = new Vector2(x + x * _config.ChunkSize, y + y * _config.ChunkSize) + offset;

               var chunk = new Chunk(position, new Vector2Int(_config.ChunkSize, _config.ChunkSize), type: chunks[x, y]);

               SpawnChunk(chunk);
            }
         }
      }

      private void CreateLevelShapeNoAlloc(ChunkType[,] buffer, int targetVolume) {
         var width = buffer.GetLength(0);
         var height = buffer.GetLength(1);

         Assert.That(width * height >= targetVolume)
               .Throws("Target volume value is higher than maximum possible with given matrix width and height values.");

         var previousIterationPositions = new Queue<Vector2Int>();

         var startPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
         buffer[startPosition.x, startPosition.y] = ChunkType.Normal;

         previousIterationPositions.Enqueue(startPosition);

         var currentVolume = 1;

         var directions = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

         while (currentVolume < targetVolume) {
            var currentIterationPositions = new Queue<Vector2Int>();

            while (previousIterationPositions.TryDequeue(out Vector2Int position)) {
               if (currentVolume == targetVolume) {
                  break;
               }

               foreach (var direction in directions) {
                  var newPosition = position + direction;

                  var spawnChance = currentIterationPositions.Count < 3
                     ? 1f
                     : 0.5f;

                  if (Random.value > spawnChance) {
                     continue;
                  }

                  if (newPosition.x < 0 || newPosition.y < 0 || newPosition.x >= width || newPosition.y >= height) {
                     continue;
                  }

                  if (buffer[newPosition.x, newPosition.y] != ChunkType.None) {
                     continue;
                  }

                  buffer[newPosition.x, newPosition.y] = ChunkType.Normal;

                  currentIterationPositions.Enqueue(newPosition);

                  currentVolume++;
               }
            }

            previousIterationPositions = currentIterationPositions;
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

      private void SpawnChunk(Chunk chunk) {
         if (chunk.Type == ChunkType.None) {
            return;
         }

         var offset = chunk.Position - chunk.Size / 2;

         var size = chunk.Type switch {
            ChunkType.BottomLeftCorner => chunk.Size + Vector2Int.one,
            ChunkType.BottomRightCorner => chunk.Size + Vector2Int.up,
            ChunkType.TopLeftCorner => chunk.Size + Vector2Int.right,

            ChunkType.LeftPeninsula => chunk.Size + Vector2Int.right,
            ChunkType.BottomPeninsula => chunk.Size + Vector2Int.up,

            _ => chunk.Size
         };

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
      }

      private void SpawnExits(ChunkType[,] chunks) {
         var width = chunks.GetLength(0);
         var height = chunks.GetLength(1);

         var directions = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

         var prefab = floorTilesPrefabs[fullTileKey];
         var rotation = Quaternion.Euler(270f, 0f, 0f);
         var offset = 0.5f;

         for (int x = 0; x < width; x += 1) {
            var yInit = x == 0 ? 0 : 1;

            for (int y = yInit; y < height; y += 1) {
               foreach (var direction in directions) {
                  var neighborChunkPosition = new Vector2Int(x, y) + direction;

                  if (neighborChunkPosition.x < 0 || neighborChunkPosition.y < 0 || neighborChunkPosition.x >= width || neighborChunkPosition.y >= height) {
                     continue;
                  }

                  if (chunks[neighborChunkPosition.x, neighborChunkPosition.y] == ChunkType.None) {
                     continue;
                  }

                  if (direction == Vector2Int.up) {
                     var tileX = x + x * _config.ChunkSize + offset;
                     var tileZ = y + y * _config.ChunkSize + _config.ChunkSize / 2 + offset;

                     PlaceTile(prefab, new Vector3(tileX, 0, tileZ), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
                     PlaceTile(prefab, new Vector3(tileX - 1, 0, tileZ), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
                     
                     continue;
                  }

                  if (direction == Vector2Int.down) {
                     var tileX = x + x * _config.ChunkSize + offset;
                     var tileZ = y + y * _config.ChunkSize - _config.ChunkSize / 2 - 1 + offset;

                     PlaceTile(prefab, new Vector3(tileX, 0, tileZ), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
                     PlaceTile(prefab, new Vector3(tileX - 1, 0, tileZ), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);

                     continue;
                  }

                  if (direction == Vector2Int.left) {
                     var tileX = x + x * _config.ChunkSize - _config.ChunkSize / 2 - 1 + offset;
                     var tileZ = y + y * _config.ChunkSize + offset;

                     PlaceTile(prefab, new Vector3(tileX, 0, tileZ), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
                     PlaceTile(prefab, new Vector3(tileX, 0, tileZ - 1), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);

                     continue;
                  }

                  if (direction == Vector2Int.right) {
                     var tileX = x + x * _config.ChunkSize + _config.ChunkSize / 2 + offset;
                     var tileZ = y + y * _config.ChunkSize + offset;

                     PlaceTile(prefab, new Vector3(tileX, 0, tileZ), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);
                     PlaceTile(prefab, new Vector3(tileX, 0, tileZ - 1), rotation, floorTilesMaterials[Random.Range(0, floorTilesMaterials.Count)]);

                     continue;
                  }
               }
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
         None,
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
