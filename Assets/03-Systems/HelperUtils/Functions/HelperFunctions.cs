using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class HelperFunctions
{
    public static char GetRandomLetter()
    {
        return (char) Random.Range('A', 'Z');
    }
    
    public static char GetRandomLetterByFrequency(List<Tuple<float, char>> frequencyTable)
    {
        var randomValue = Random.value;
        foreach (var pair in frequencyTable.Where(pair => randomValue <= pair.Item1))
        {
            return pair.Item2;
        }

        return frequencyTable[0].Item2;
    }

    public static Vector3 CalculatePositionOffset(Vector3 startPosition, Vector3 offsetDirectionPoint, float offsetMagnitude)
    {
        var direction = offsetDirectionPoint - startPosition;
        direction.Normalize();
        return startPosition + direction * offsetMagnitude;
    }

    public static IEnumerator AutoMovement(Transform transform, float movementSpeed)
    {
        while (true)
        {
            yield return null;
            transform.localPosition += Vector3.back * (movementSpeed * Time.deltaTime);
        }
    }
}