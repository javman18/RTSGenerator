using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditorInternal;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
public class TagLayerManager
{
    private static int maxTags = 10000;
    private static int maxLayers = 31;
    private static int maxSortingLayers = 31;
    public static bool CreateTag(string tagName)
    {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        if (tagsProp.arraySize >= maxTags)
        {

            return false;
        }

        if (!PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName))
        {
            int index = tagsProp.arraySize;

            tagsProp.InsertArrayElementAtIndex(index);
            SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);

            sp.stringValue = tagName;

            tagManager.ApplyModifiedProperties();

            return true;
        }

        return false;
    }

    public static bool TagExists(string tagName)
    {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);


        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        return PropertyExists(tagsProp, 0, maxTags, tagName);
    }

    public static bool CreateLayer(string layerName)
    {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layersProp = tagManager.FindProperty("layers");
        if (!PropertyExists(layersProp, 0, maxLayers, layerName))
        {
            SerializedProperty sp;

            for (int i = 8, j = maxLayers; i < j; i++)
            {
                sp = layersProp.GetArrayElementAtIndex(i);
                if (sp.stringValue == "")
                {

                    sp.stringValue = layerName;


                    tagManager.ApplyModifiedProperties();
                    return true;
                }

            }
        }

        return false;
    }

    public static bool LayerExists(string layerName)
    {

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);


        SerializedProperty layersProp = tagManager.FindProperty("layers");
        return PropertyExists(layersProp, 0, maxLayers, layerName);
    }

    private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
    {
        for (int i = start; i < end; i++)
        {
            SerializedProperty t = property.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(value))
            {
                return true;
            }
        }
        return false;
    }
}
