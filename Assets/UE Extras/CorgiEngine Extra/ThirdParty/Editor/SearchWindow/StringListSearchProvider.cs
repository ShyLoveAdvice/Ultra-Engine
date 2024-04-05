using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using System;

namespace MoreMountains.Tools
{
    public class StringListSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private string[] _listItems;
        private Action<string> _onSetIndexCallback;
        private string _searchWindowTitle;
        private Texture2D _indentationIcon;
        public StringListSearchProvider(string[] items, Action<string> callback, string title)
        {
            _listItems = items;
            _onSetIndexCallback = callback;
            _searchWindowTitle = title;

            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, Color.clear);
            _indentationIcon.Apply();
        }
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
            searchList.Add(new SearchTreeGroupEntry(new GUIContent(_searchWindowTitle)));

            List<string> sortedListItems = _listItems.ToList();
            sortedListItems.Sort((a, b) =>
            {
                string[] splitsA = a.Split("/");
                string[] splitsB = b.Split("/");
                for (int i = 0; i < splitsA.Length; i++)
                {
                    if (i >= splitsB.Length)
                    {
                        return 1;
                    }
                    int value = splitsA[i].CompareTo(splitsB[i]);
                    if (value != 0)
                    {
                        if (splitsA.Length != splitsB.Length && (i == splitsA.Length - 1 || i == splitsB.Length - 1))
                        {
                            return splitsA.Length < splitsB.Length ? 1 : -1;
                        }
                        return value;
                    }
                }
                return 0;

            });

            List<string> groups = new List<string>();
            foreach (string item in sortedListItems)
            {
                string[] entryTitle = item.Split("/");
                string groupName = "";
                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];
                    if (!groups.Contains(groupName))
                    {
                        searchList.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }
                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last(), _indentationIcon));
                entry.level = entryTitle.Length;
                entry.userData = entryTitle.Last();
                searchList.Add(entry);
            }
            return searchList;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            _onSetIndexCallback?.Invoke((string)SearchTreeEntry.userData);
            return true;
        }
    }
}
