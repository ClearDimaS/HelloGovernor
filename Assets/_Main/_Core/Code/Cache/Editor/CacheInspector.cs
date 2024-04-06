using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class CacheInspector : EditorWindow
    {
        private string originalText;
        private string selectedText;
        private string selectedPath;
        private string selectedFileName;
        Vector2 scrollPos;
        private bool wantToRemoveAll;

        private LocalCacheManager LocalCacheManager 
        {
            get 
            {
                if (localCacheManager == null)
                    localCacheManager = new LocalCacheManager();
                return localCacheManager;
            }
        }
        private LocalCacheManager localCacheManager;

        [MenuItem("Tools/Cache Inspector")]
        static void ShowWindow()
        {
            var size = new Vector2(1500f, 1500f);

            var window = (CacheInspector)GetWindow(typeof(CacheInspector), true, "EditorGUILayout.CacheInspector");
            window.Show();

            window.maxSize = size;
            window.position = new Rect(Screen.width / 2, Screen.height / 2, size.x, size.y);
            window.titleContent = new GUIContent("Cache Inspector v0");
        
            window.ShowModal();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"saved at: ");
            GUILayout.TextField($"{LocalCacheManager.SavePath}");
            EditorGUILayout.EndHorizontal();
    
            GUILayout.Label("");

            if (wantToRemoveAll)
                DrawRemoveAllConfirm();
            else if(string.IsNullOrEmpty(selectedPath))
                DrawSelect();
            else
                DrawEdit();
        }

        private void DrawSelect()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button($"Clear All")) 
            {
                wantToRemoveAll = true;
            }
            GUILayout.Label("");
            GUILayout.Label("");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(50);

            EditorGUILayout.LabelField($"Select a file to edit");

            GUILayout.Space(35);


            var files = GetFiles(LocalCacheManager.SavePath);
            EditorGUILayout.BeginHorizontal();
            scrollPos =
                EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < files.Length; i++)
            {
                int index = i;

                if (GUILayout.Button($"{files[index].Item2}"))
                {
                    selectedPath = files[index].Item1;
                    selectedFileName = files[index].Item2;
                    originalText = localCacheManager.LoadRaw(selectedPath);
                    selectedText = originalText;
                }
            }

            if (files.Length == 0)
            {
                EditorGUILayout.LabelField($"No files founs sorry :(");
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawRemoveAllConfirm()
        {
            GUILayout.Label("Are you sure you want to delete System32 folder and python2 from your OS?");
            GUILayout.Space(30);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button($"No"))
            {
                wantToRemoveAll = false;
            }
            GUILayout.Label("");
            if (GUILayout.Button($"Yes"))
            {
                localCacheManager.ClearAll();
                wantToRemoveAll = false;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawEdit()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("<="))
            {
                Leave();
            }
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (!ReferenceEquals(selectedText, originalText))
            {
                EditorGUILayout.LabelField("UNSAVED CHANGES*");
            }
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            GUILayout.Label("");
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.LabelField(selectedFileName);
            GUILayout.Space(30);

            EditorGUILayout.BeginHorizontal();
            scrollPos =
                EditorGUILayout.BeginScrollView(scrollPos);
  
            selectedText = EditorGUILayout.TextArea(selectedText, GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(40);
            if (GUILayout.Button("Save"))
            {
                LocalCacheManager.SaveRaw(selectedPath, selectedText);
                originalText = selectedText;
            }
            if (GUILayout.Button("Remove"))
            {
                ClearSelected(selectedPath);
                Leave();
            }
        }

        private void Leave() 
        {
            selectedText = "";
            selectedPath = "";
            selectedFileName = "";
            originalText = "";
        }

        private void ClearSelected(string file) 
        {
            LocalCacheManager.ClearRaw(file);
        }

        private (string, string)[] GetFiles(string dir) 
        {
            var files = Directory.GetFiles(dir).Select(x => (x, Path.GetFileNameWithoutExtension(x)));

            var directories = Directory.GetDirectories(dir);
            foreach (var subDir in directories)
            {
                var newFiles = files.ToList();
                newFiles.AddRange(GetFiles(subDir));
                files = newFiles;
            }
            if (!files.Any())
                return new (string, string)[0];

            return files.ToArray();
        }
    }
    #endif