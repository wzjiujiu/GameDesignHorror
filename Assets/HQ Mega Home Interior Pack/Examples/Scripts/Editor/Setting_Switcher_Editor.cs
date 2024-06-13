using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Setting_Switcher))]
public class Setting_Switcher_Editor : Editor
{
    SerializedProperty settings = null;
    ReorderableList list = null;

    SerializedProperty enabled = null;

    private void OnEnable()
    {
        settings = serializedObject.FindProperty("settings");
        list = new ReorderableList(serializedObject, settings, true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Settings");
            },
            elementHeightCallback = (int index) =>
            {
                return (17+5) * 6 + 5;
            },
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var elm = settings.GetArrayElementAtIndex(index);

                Rect elmRect = new Rect(rect) { height = 17 };

                EditorGUI.ObjectField(elmRect, elm.FindPropertyRelative("setting"), new GUIContent("Setting"));
                elmRect.y += 17 + 5;
                elm.FindPropertyRelative("ambientColor").colorValue = EditorGUI.ColorField(elmRect, new GUIContent("Ambient Color"), elm.FindPropertyRelative("ambientColor").colorValue);
                elmRect.y += 17 + 5;
                EditorGUI.ObjectField(elmRect, elm.FindPropertyRelative("skybox"), new GUIContent("Skybox"));
                elmRect.y += 17 + 5;
                elm.FindPropertyRelative("rotation").floatValue = EditorGUI.Slider(elmRect, new GUIContent("Rotation"), elm.FindPropertyRelative("rotation").floatValue, 0, 360);
                elmRect.y += 17 + 5;
                elm.FindPropertyRelative("exposure").floatValue = EditorGUI.FloatField(elmRect, new GUIContent("Exposure"), elm.FindPropertyRelative("exposure").floatValue);
                elmRect.y += 17 + 5;
                if (GUI.Button(elmRect, enabled.intValue == index ? "Update" : "Enable"))
                {
                    EnableSetting(index);
                }
            }
        };
        enabled = serializedObject.FindProperty("enabled");
    }

    public override void OnInspectorGUI()
    {
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    void EnableSetting(int index)
    {
        for (int i = 0; i < settings.arraySize; i++)
        {
            var elm = settings.GetArrayElementAtIndex(i);
            GameObject obj = (GameObject)elm.FindPropertyRelative("setting").objectReferenceValue;
            obj.SetActive(i == index);
        }
        RenderSettings.ambientLight = settings.GetArrayElementAtIndex(index).FindPropertyRelative("ambientColor").colorValue;
        RenderSettings.skybox = (Material)settings.GetArrayElementAtIndex(index).FindPropertyRelative("skybox").objectReferenceValue;

        if (RenderSettings.skybox)
        {
            RenderSettings.skybox.SetFloat("_Rotation", settings.GetArrayElementAtIndex(index).FindPropertyRelative("rotation").floatValue);
        }

        enabled.intValue = index;
    }
}