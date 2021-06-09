using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
   public static Action<bool> GameOver;
   public static Action CheckIfShapeCanBePlaced;

   public static Action MoveShapeToStartPosition;

   public static Action RequestNewShapes;

   public static Action SetShapeInactive;

   public static Action<int> AddScores;
}
