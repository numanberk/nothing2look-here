using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[InitializeOnLoad]
public class ColoredHierarchy : EditorWindow
{
    private static Vector2 offset = new Vector2(20, 1);

    private static Dictionary<string, (Color bgColor, Color textColor)> objectColors = new Dictionary<string, (Color, Color)>();
    private static List<string> objectNames = new List<string>();

    static ColoredHierarchy()
    {
        LoadColors(); // Load saved settings
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if (obj != null && objectColors.TryGetValue(obj.name, out var colors))
        {
            Rect bgRect = new Rect(selectionRect.x, selectionRect.y, selectionRect.width + 50, selectionRect.height);
            Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);

            EditorGUI.DrawRect(bgRect, colors.bgColor);
            EditorGUI.LabelField(offsetRect, obj.name, new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = colors.textColor },
                fontStyle = FontStyle.Bold
            });
        }
    }

    [MenuItem("Tools/Hierarchy Color Settings")]
    public static void ShowWindow()
    {
        GetWindow<ColoredHierarchy>("Hierarchy Colors");
    }

    private void OnGUI()
    {
        GUILayout.Label("Hierarchy Object Colors", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Object"))
        {
            objectNames.Add("NewObject");
            objectColors["NewObject"] = (Color.white, Color.black);
        }

        int removeIndex = -1; // Store the index to remove later

        for (int i = 0; i < objectNames.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            objectNames[i] = EditorGUILayout.TextField("Object Name:", objectNames[i]);

            if (!objectColors.ContainsKey(objectNames[i]))
                objectColors[objectNames[i]] = (Color.white, Color.black);

            var (bgColor, textColor) = objectColors[objectNames[i]];

            bgColor = EditorGUILayout.ColorField("Background Color:", bgColor);
            textColor = EditorGUILayout.ColorField("Text Color:", textColor);

            objectColors[objectNames[i]] = (bgColor, textColor);

            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                removeIndex = i; // Mark for removal
            }

            EditorGUILayout.EndVertical();
        }

        if (removeIndex >= 0) // Remove after the loop
        {
            objectColors.Remove(objectNames[removeIndex]);
            objectNames.RemoveAt(removeIndex);
        }

        if (GUILayout.Button("Apply"))
        {
            SaveColors();
            EditorApplication.RepaintHierarchyWindow();
        }
    }


    private static void LoadColors()
    {
        int count = EditorPrefs.GetInt("HierarchyObjectCount", 0);
        objectNames.Clear();
        objectColors.Clear();

        for (int i = 0; i < count; i++)
        {
            string name = EditorPrefs.GetString($"HierarchyObjectName_{i}", "");
            string bgColorStr = EditorPrefs.GetString($"HierarchyObjectBgColor_{i}", "#FFFFFF");
            string textColorStr = EditorPrefs.GetString($"HierarchyObjectTextColor_{i}", "#000000");

            if (ColorUtility.TryParseHtmlString(bgColorStr, out Color bgColor) &&
                ColorUtility.TryParseHtmlString(textColorStr, out Color textColor))
            {
                objectNames.Add(name);
                objectColors[name] = (bgColor, textColor);
            }
        }
    }

    private static void SaveColors()
    {
        EditorPrefs.SetInt("HierarchyObjectCount", objectNames.Count);

        for (int i = 0; i < objectNames.Count; i++)
        {
            EditorPrefs.SetString($"HierarchyObjectName_{i}", objectNames[i]);
            var (bgColor, textColor) = objectColors[objectNames[i]];
            EditorPrefs.SetString($"HierarchyObjectBgColor_{i}", "#" + ColorUtility.ToHtmlStringRGB(bgColor));
            EditorPrefs.SetString($"HierarchyObjectTextColor_{i}", "#" + ColorUtility.ToHtmlStringRGB(textColor));
        }
    }
}
