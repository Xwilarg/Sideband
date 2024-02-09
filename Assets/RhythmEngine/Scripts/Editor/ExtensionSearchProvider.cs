#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RhythmEngine
{
    public class ExtensionSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private static List<Type> GetInheritedClasses(Type type) => Assembly.GetAssembly(type).GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(type)).ToList();

        private Action<Type> _onSelectEntry;

        public void Init(Action<Type> callback)
        {
            _onSelectEntry = callback;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var searchList = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Extensions"))
            };

            var extensionTypes = GetInheritedClasses(typeof(RhythmEngineExtension));
            extensionTypes.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

            foreach (var extensionType in extensionTypes)
            {
                var entry = new SearchTreeEntry(new GUIContent(extensionType.Name, EditorGUIUtility.FindTexture("cs Script Icon")))
                {
                    level = 1,
                    userData = extensionType
                };
                searchList.Add(entry);
            }

            return searchList;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            _onSelectEntry?.Invoke((Type) searchTreeEntry.userData);
            return true;
        }
    }
}
#endif
