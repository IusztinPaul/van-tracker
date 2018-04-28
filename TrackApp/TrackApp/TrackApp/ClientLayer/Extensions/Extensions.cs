using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TrackApp.ClientLayer.Friends;
using TrackApp.ClientLayer;

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

        public static List<string> ResizeIfNeeded(this List<string> notifList, bool sort=false)
        {
            if (notifList.Count <= NotificationPage.NOTIFICATION_MAX_NUMBER)
                return notifList;

            //sort by the index so only the lowest indexes will be cut off
            if (sort) // format username#[add|remove]#index
                notifList.Sort((a, b) =>
               {
                   var a_index = a.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[2];
                   var b_index = b.Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0])[2];
                   return a_index.CompareTo(b_index); 
               });

            //cut the oldest elements of the list
            int remainingElementsStartingIndex = notifList.Count - NotificationPage.NOTIFICATION_MAX_NUMBER;

            var newList = notifList.GetRange(remainingElementsStartingIndex, NotificationPage.NOTIFICATION_MAX_NUMBER);
            for(int i = 0; i < newList.Count; i++)
            {
                string[] items = newList[i].Split(ClientConsts.CONCAT_SPECIAL_CHARACTER[0]); // format username#[add|remove]#index

                string newListItem = items[0] + ClientConsts.CONCAT_SPECIAL_CHARACTER + items[1] + ClientConsts.CONCAT_SPECIAL_CHARACTER;
                newListItem += (Int32.Parse(items[2]) - remainingElementsStartingIndex).ToString(); // map index to the new value

                newList[i] = newListItem;
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
