using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Violoncello.Services;

namespace SecretHostel.DreamRogue {
   public class LevelGenerationState : LevelViewModel.State {
      private LevelGenerationStateConfig _config;

      private Dictionary<string, GameObject> _floorTiles = new();

      private readonly Vector3 FloorTileSize = new(1f, 1f, 0.5f);

      private LevelGenerationState(LevelViewModel viewModel, LevelGenerationStateConfig config, IAssetsLoader assetsLoader) : base(viewModel) {
         _config = config;

         var tile = assetsLoader.Load<GameObject>("Models", "floor-tile-full");

         _floorTiles = new() {
            { "1x1", tile }
         };
      }

      public override void EnterState() {
         var startRoom = new Room(new Vector2Int(16, 12), (Exits)0b_1111);
         SpawnRoom(startRoom, new Vector3(0.5f, 0.5f));

         
      }

      private void SpawnRoom(Room room, Vector2 position) {
         var offset = position - room.Size / 2;

         for (int i = 0; i < room.Size.x; i++) {
            for (int j = 0; j < room.Size.y; j++) {
               var x = i + offset.x;
               var z = j + offset.y;

               PlaceTile(new Vector3(x, 0, z), _floorTiles["1x1"]);
            }
         }

         if (room.Exits.HasFlag(Exits.Top)) {
            var x = position.x;
            var z = position.y + room.Size.y / 2;

            PlaceTile(new Vector3(x, 0, z), _floorTiles["1x1"]);
            PlaceTile(new Vector3(x - 1, 0, z), _floorTiles["1x1"]);
         }

         if (room.Exits.HasFlag(Exits.Bottom)) {
            var x = position.x;
            var z = position.y - room.Size.y / 2 - 1;

            PlaceTile(new Vector3(x, 0, z), _floorTiles["1x1"]);
            PlaceTile(new Vector3(x - 1, 0, z), _floorTiles["1x1"]);
         }

         if (room.Exits.HasFlag(Exits.Left)) {
            var x = position.x - room.Size.x / 2 - 1;
            var z = position.y;

            PlaceTile(new Vector3(x, 0, z), _floorTiles["1x1"]);
            PlaceTile(new Vector3(x, 0, z - 1), _floorTiles["1x1"]);
         }

         if (room.Exits.HasFlag(Exits.Right)) {
            var x = position.x + room.Size.x / 2;
            var z = position.y;

            PlaceTile(new Vector3(x, 0, z), _floorTiles["1x1"]);
            PlaceTile(new Vector3(x, 0, z - 1), _floorTiles["1x1"]);
         }
      }

      private void PlaceTile(Vector3 position, GameObject prefab) {
         var tile = UnityEngine.Object.Instantiate(prefab, GroundTilemap.transform);

         tile.transform.localScale = FloorTileSize;
         tile.transform.position = position;
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

      [Flags]
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
