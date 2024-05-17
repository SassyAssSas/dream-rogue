using System;
using UnityEngine;

namespace Violoncello.Utilities {
   public static class Assert {
      private const string NotNullState = "Not Null";
      private const string NullState = "Null";

      private const string NotInterfaceState = "Not Interace";
      private const string InterfaceState = "Null";

      private const string PositiveState = "Positive";
      private const string NegativeState = "Negative";

      private const string TrueState = "True";
      private const string FalseState = "False";

      public static Assertion That(bool expression) {
         return expression
            ? Assertion.FromSuccess(TrueState, expression)
            : Assertion.FromFail(TrueState, FalseState, expression);
      }

      public static Assertion IsNull(object value) {
         return value == null
          ? Assertion.FromSuccess(NullState, value)
          : Assertion.FromFail(NullState, NotNullState, value);
      }

      public static Assertion IsNotNull(object value) {

         return value != null
          ? Assertion.FromSuccess(NotNullState, value)
          : Assertion.FromFail(NotNullState, NullState, value);
      }

      public static Assertion IsInterface(Type value) {
         if (value == null) {
            return Assertion.FromFail(InterfaceState, NullState, value);
         }

         if (value.IsInterface) {
            return Assertion.FromSuccess(InterfaceState, value);
         }

         return Assertion.FromFail(InterfaceState, NotInterfaceState, value);
      }

      public static Assertion IsNegative(long value) {
         return value < 0
             ? Assertion.FromSuccess(NegativeState, value)
             : Assertion.FromFail(NegativeState, PositiveState, value);
      }

      public static Assertion IsNegative(double value) {
         return value < 0
             ? Assertion.FromSuccess(NegativeState, value)
             : Assertion.FromFail(NegativeState, PositiveState, value);
      }

      public static Assertion IsNegative(decimal value) {
         return value < 0
             ? Assertion.FromSuccess(NegativeState, value)
             : Assertion.FromFail(NegativeState, PositiveState, value);
      }

      public static Assertion IsPositive(long value) {
         return value > 0
             ? Assertion.FromSuccess(PositiveState, value)
             : Assertion.FromFail(PositiveState, NegativeState, value);
      }

      public static Assertion IsPositive(double value) {
         return value > 0
             ? Assertion.FromSuccess(PositiveState, value)
             : Assertion.FromFail(PositiveState, NegativeState, value);
      }

      public static Assertion IsPositive(decimal value) {
         return value > 0
             ? Assertion.FromSuccess(PositiveState, value)
             : Assertion.FromFail(PositiveState, NegativeState, value);
      }
   }
}
