using System;
using System.Collections.Generic;
using UnityEngine;


namespace CustomSorting
{
    public static class SortingExtension
    {
        /// <summary>
        /// Sorting Algorithm performance - WORST O(n²) - AVERAGE θ(n²) - BEST Ω(n²) - SPACE O(1)
        /// </summary>
        /// <typeparam name="T">The IList collection type</typeparam>
        /// <param name="collection">The collection to be sorted</param>
        /// <param name="comparer">The comparer to be used for sorting</param>
        /// <exception cref="NullReferenceException">Exception thrown if the collection to be sorted is null</exception>
        public static void SelectionSort<T>(this IList<T> collection, IComparer<T> comparer = null) where T : IComparable<T>
        {
            if (collection == null)
            {
                Debug.LogError("Selection Sort array is not initialized!");
                return;
            }

            comparer ??= Comparer<T>.Default;

            int arrayLength = collection.Count;
            int min;
            T tmp;

            for (int i = 0; i < arrayLength; i++)
            {
                min = i;

                for (int j = i + 1; j < arrayLength; j++)
                {
                    if (comparer.Compare(collection[j], collection[min]) < 0)
                        min = j;
                }

                if (min != i)
                {
                    tmp = collection[i];
                    collection[i] = collection[min];
                    collection[min] = tmp;
                }
            }
        }


        /// <summary>
        /// Sorting Algorithm performance - WORST O(n²) - AVERAGE θ(n²) - BEST Ω(n) - SPACE O(1)
        /// </summary>
        /// <typeparam name="T">The IList collection type</typeparam>
        /// <param name="collection">The collection to be sorted</param>
        /// <param name="comparer">The comparer to be used for sorting</param>
        /// <exception cref="NullReferenceException">Exception thrown if the collection to be sorted is null</exception>
        public static void InsertionSort<T>(this IList<T> collection, IComparer<T> comparer = null) where T : IComparable<T>
        {
            if (collection == null)
            {
                Debug.LogError("Insertion Sort array is not initialized!");
                return;
            }

            comparer ??= Comparer<T>.Default;

            int arrayLength = collection.Count;
            int j;
            T tmp;

            for (int i = 1; i < arrayLength; i++)
            {
                tmp = collection[i];

                for (j = i - 1; j >= 0 && (comparer.Compare(collection[j], tmp) > 0); j--)
                    collection[j + 1] = collection[j];

                collection[j + 1] = tmp;
            }
        }


        /// <summary>
        /// Sorting Algorithm performance - WORST O(nlog(n)) - AVERAGE θ(nlog(n)) - BEST Ω(nlog(n)) - SPACE O(n)
        /// </summary>
        /// <typeparam name="T">The IList collection type</typeparam>
        /// <param name="collection">The collection to be sorted</param>
        /// <param name="comparer">The comparer to be used for sorting</param>
        /// <exception cref="NullReferenceException">Exception thrown if the collection to be sorted is null</exception>
        public static void MergeSort<T>(this IList<T> collection, IComparer<T> comparer = null) where T : IComparable<T>
        {
            if (collection == null)
            {
                Debug.LogError("Merge Sort array is not initialized!");
                return;
            }

            comparer ??= Comparer<T>.Default;

            MergeSortSplit(collection, 0, collection.Count - 1, comparer);
        }


        /// <summary>
        /// Recursively splits the array in 2 subarrays to later merge their elements in order
        /// </summary>
        /// <typeparam name="T">The IList collection type</typeparam>
        /// <param name="collection">The collection to be sorted</param>
        /// <param name="left">The collection left bound index</param>
        /// <param name="right">The collection right bound index</param>
        /// <param name="comparer">The comparer to be used for sorting</param>
        private static void MergeSortSplit<T>(IList<T> collection, int left, int right, IComparer<T> comparer)
        {
            if (left < right)
            {
                int middle = left + (right - left) / 2;

                MergeSortSplit(collection, left, middle, comparer);
                MergeSortSplit(collection, middle + 1, right, comparer);

                MergeSortMerge(collection, left, middle, right, comparer);
            }
        }


        /// <summary>
        /// Merges two arrays in a single ordered one
        /// </summary>
        /// <typeparam name="T">The IList collection type</typeparam>
        /// <param name="collection">The collection to be sorted</param>
        /// <param name="left">The first collection left bound index</param>
        /// <param name="middle">The first collection left bound index</param>
        /// <param name="right">The seconds collection right bound index</param>
        /// <param name="comparer">The comparer to be used for sorting</param>
        private static void MergeSortMerge<T>(IList<T> collection, int left, int middle, int right, IComparer<T> comparer)
        {
            int i, j;
            int leftArrayLength = middle - left + 1;
            int rightArrayLength = right - middle;

            T[] leftTmpArray = new T[leftArrayLength];
            T[] rightTmpArray = new T[rightArrayLength];

            for (i = 0; i < leftArrayLength; ++i)
                leftTmpArray[i] = collection[left + i];

            for (j = 0; j < rightArrayLength; ++j)
                rightTmpArray[j] = collection[middle + 1 + j];

            i = 0;
            j = 0;
            int k = left;

            while (i < leftArrayLength && j < rightArrayLength)
            {
                if (comparer.Compare(leftTmpArray[i], rightTmpArray[j]) < 0)
                    collection[k++] = leftTmpArray[i++];
                else
                    collection[k++] = rightTmpArray[j++];
            }

            while (i < leftArrayLength)
                collection[k++] = leftTmpArray[i++];

            while (j < rightArrayLength)
                collection[k++] = rightTmpArray[j++];
        }
    }
}