using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrackApp.ClientLayer.Friends;
using TrackApp.ClientLayer;
using System.Text;

namespace TrackApp.ClientLayer.Extensions
{
    public static class Extensions
    {
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (int i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }

        public static List<string> ResizeIfNeeded(this List<string> notifList, int maxCountList, int stringIndexOfListIndex, bool sort=false)
        {
            if (notifList.Count <= maxCountList)
                return notifList;

            //sort by the index so only the lowest indexes will be cut off
            if (sort) // format username#[add|remove]#index
                notifList.Sort((a, b) =>
               {
                   var a_index = a.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[stringIndexOfListIndex];
                   var b_index = b.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[stringIndexOfListIndex];
                   return a_index.CompareTo(b_index); 
               });

            //cut the oldest elements of the list
            int remainingElementsStartingIndex = notifList.Count - maxCountList;

            var newList = notifList.GetRange(remainingElementsStartingIndex, maxCountList);
            for(int i = 0; i < newList.Count; i++)
            {
                string[] items = newList[i].Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0]); // format username#[add|remove]#index

                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < items.Length - 1; j++)
                    sb.Append(items[j]).Append(ClientConsts.CONCAT_SPECIAL_CHARACTER);

                sb.Append((Int32.Parse(items[items.Length - 1]) - remainingElementsStartingIndex).ToString()); // map index to the new value

                newList[i] = sb.ToString();
            }

            return newList;
        }
             
        public static void AddIndexedString(this List<string> list, string item)
        {
            if (list == null)
                list = new List<string>();

            item += ClientConsts.CONCAT_SPECIAL_CHARACTER + list.Count; // format username#[add|remove]#index
            list.Add(item);
        }

    }
}
