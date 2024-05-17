using System;
using UnityEngine;

namespace Violoncello.Utilities {
   public class Assertion {
      private bool _succeeded;
      public bool Succeeded => _succeeded;

      private string _expectedState;
      public string ExpectedState => _expectedState;

      private string _actualState;
      public string ActualState => _actualState;

      private Assertion() { }

      internal static Assertion FromSuccess(string state, object inspectedObject) {
         var assertion = new Assertion();

         assertion._succeeded = true;
         assertion._expectedState = state;
         assertion._actualState = state;

         return assertion;
      }

      internal static Assertion FromFail(string expectedState, string actualState, object inspectedObject) {
         var assertion = new Assertion();

         assertion._succeeded = false;
         assertion._expectedState = expectedState;
         assertion._actualState = actualState;

         return assertion;
      }

      public static bool operator true(Assertion assertion) => assertion.Succeeded;
      public static bool operator false(Assertion assertion) => !assertion.Succeeded;

      public void Throws() {
         if (!_succeeded) {
            throw new AssertionException($"Value was expected to be {_expectedState}, but was {_actualState}.");
         }
      }

      public void Throws(string exceptionMessage) {
         if (!_succeeded) {
            throw new AssertionException(exceptionMessage);
         }
      }

      public void LogError() {
         if (!_succeeded) {
            Debug.LogError($"Value was expected to be {_expectedState}, but was {_actualState}.");
         }
      }

      public void LogError(string exceptionMessage) {
         if (!_succeeded) {
            Debug.LogError(exceptionMessage);
         }
      }

      public void LogWarning() {
         if (!_succeeded) {
            Debug.LogWarning($"Value was expected to be {_expectedState}, but was {_actualState}.");
         }
      }

      public void LogWarning(string exceptionMessage) {
         if (!_succeeded) {
            Debug.LogWarning(exceptionMessage);
         }
      }

      public void Log() {
         if (!_succeeded) {
            Debug.Log($"Value was expected to be {_expectedState}, but was {_actualState}.");
         }
      }

      public void Log(string exceptionMessage) {
         if (!_succeeded) {
            Debug.Log(exceptionMessage);
         }
      }
   }
}
