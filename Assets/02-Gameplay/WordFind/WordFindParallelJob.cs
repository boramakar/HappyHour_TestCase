using HappyTroll;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct WordFindParallelJob : IJobParallelFor
{
    public int columnCount;
    public int rowCount;
    [ReadOnly] public NativeArray<int2> wordIndices;
    [ReadOnly] public NativeArray<char> words;
    [ReadOnly] public NativeArray<char> grid;
    public NativeArray<int3> results;

    #region Implementation of IJobParallelFor

    /// <inheritdoc />
    public void Execute(int index)
    {
       // Extract word
       var wordStartIndex = wordIndices[index].x;
       var wordLength = wordIndices[index].y;
       var wordFound = false;
       
       //Search for word using first char
       for (var i = 0; !wordFound && i < grid.Length; i++)
       {
           // Skip to next cell if content doesn't match the first character of word
           if (grid[i] != words[wordStartIndex]) continue;
           
           // The code block below looks horrible but performs decently well due to a lot of flow interruption opportunities.
           // By separating each block, we can exit out of each step as soon as we determine a bad result
           // and we can also skip following steps completely if we find the result we're looking for.
           //
           // It can be compressed by using some extra data structures and refactoring the core loop to a different method
           // but for the scope of this test case I believe it is not necessary.
           //
           // Recursion is also a very good alternative but again, every recursion can be converted into a loop and vice versa
           // and for this test I simply find it easier to implement using the following method.
           //
           // On top of all the above explanations, below method is less memory intensive and completely avoids any and all
           // potential stack overflow issues.
           
           // Search Right
           if(columnCount - (i % columnCount) <= wordLength) // Search only if there are enough characters to the right
           {
               wordFound = true;
               for (var j = 1; wordFound && j < wordLength; j++)
               {
                   if (words[wordStartIndex + j] == grid[i + j]) continue;
                   
                   wordFound = false;
               }

               if (wordFound)
               {
                   results[index] = new int3(0, i % columnCount, i / columnCount);
               }
           }
           
           // Search Left
           if(!wordFound && (i % columnCount) + 1 >= wordLength) // Search only if there are enough characters to the left
           {
               wordFound = true;
               for (var j = 1; wordFound && j < wordLength; j++)
               {
                   if (words[wordStartIndex + j] == grid[i - j]) continue;
                   
                   wordFound = false;
               }

               if (wordFound)
               {
                   results[index] = new int3(1, i % columnCount, i / columnCount);
               }
           }
       
           // Search Up
           if(!wordFound && rowCount - (i / rowCount) <= wordLength) // Search only if there are enough characters upwards
           {
               wordFound = true;
               for (var j = 1; wordFound && j < wordLength; j++)
               {
                   if (words[wordStartIndex + j] == grid[i + (j * columnCount)]) continue;
                   
                   wordFound = false;
               }

               if (wordFound)
               {
                   results[index] = new int3(2, i % columnCount, i / columnCount);
               }
           }
           
           // Search Down
           if(!wordFound && (i / rowCount) + 1 >= wordLength) // Search only if there are enough characters downwards
           {
               wordFound = true;
               for (var j = 1; wordFound && j < wordLength; j++)
               {
                   if (words[wordStartIndex + j] == grid[i - (j * columnCount)]) continue;
                   
                   wordFound = false;
               }

               if (wordFound)
               {
                   results[index] = new int3(3, i % columnCount, i / columnCount);
               }
           }
       }
       Debug.Log($"{index} - WordFound: {wordFound}");
    }

    #endregion
}