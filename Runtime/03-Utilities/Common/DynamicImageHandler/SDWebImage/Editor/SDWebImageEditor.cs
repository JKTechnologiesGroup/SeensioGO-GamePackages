// // 
// // SDWebImageEditor.cs
// // SDWebImage
// //
// // Created by Abdalla Tawfik
// // Copyright © 2018 RIZMY Studio. All rights reserved.
// //

// using UnityEngine;
// using UnityEngine.UI;
// using UnityEditor;
// using System;

// [CustomEditor(typeof(SDWebImage))]
// public class SDWebImageEditor : Editor {

//     private SDWebImage component;
//     private Texture logo;
//     private static bool optionsFoldout;
//     private static bool loadingIndicatorOptionsFoldout;

//     public void OnEnable() {
//         component = (SDWebImage)target;
//         logo = (Texture)Resources.Load("editor-logo");
//     }

//     public override void OnInspectorGUI() {
//         var content = new GUIContent();
//         float inspectorWidth = EditorGUIUtility.currentViewWidth - 35;

//         // SDWebImage Logo
//         EditorGUILayout.Space();
//         GUILayout.Label(logo, new GUILayoutOption[] { GUILayout.Width(inspectorWidth), GUILayout.Height((logo.height * inspectorWidth) / logo.width), GUILayout.ExpandWidth(true) });
//         EditorGUILayout.Space();

//         // Image URL
//         content = new GUIContent("Image URL", "Url of a web image to be loaded");
//         component.imageURL = EditorGUILayout.TextField(content, component.imageURL);

//         // Placeholder Texture
//         content = new GUIContent("Placeholder", "Placeholder Texture to be used while loading the web image");
//         component.placeholderImage = (Texture2D)EditorGUILayout.ObjectField(content, component.placeholderImage, typeof(Texture2D), false, GUILayout.MaxHeight(16));

//         // Preserve Aspect
//         content = new GUIContent("Preserve Aspect", "Sets whether or not the image will preserve its aspect ratio (Image Component only)");
//         component.preserveAspect = EditorGUILayout.Toggle(content, component.preserveAspect);


//         EditorGUILayout.Space();


//         // SDWebImage Options
//         EditorGUILayout.BeginVertical();

//         optionsFoldout = GUILayout.Toggle(optionsFoldout, "SDWebImage Options", "Foldout", GUILayout.ExpandWidth(false));

//         if (optionsFoldout) {
//             // Auto Download
//             content = new GUIContent("Auto Download", "Sets whether or not the image will start loading automatically using the inspector image url");
//             component.autoDownload = EditorGUILayout.Toggle(content, component.autoDownload);

//             // Memory Cache
//             content = new GUIContent("Memory Cache", "Sets whether or not the image will be cached in memory");
//             component.memoryCache = EditorGUILayout.Toggle(content, component.memoryCache);

//             // Disk Cache
//             content = new GUIContent("Disk Cache", "Sets whether or not the image will be cached in disk");
//             component.diskCache = EditorGUILayout.Toggle(content, component.diskCache);
//         }

//         EditorGUILayout.EndVertical();


//         EditorGUILayout.Space();


//         // Loading Indicator Options
//         EditorGUILayout.BeginVertical();

//         loadingIndicatorOptionsFoldout = GUILayout.Toggle(loadingIndicatorOptionsFoldout, "Loading Indicator Options", "Foldout", GUILayout.ExpandWidth(false));

//         if (loadingIndicatorOptionsFoldout) {
//             // Show Loading Indicator
//             content = new GUIContent("Show", "Sets whether or not the loading indicator will be shown while loading the web image");
//             component.showLoadingIndicator = EditorGUILayout.Toggle(content, component.showLoadingIndicator);

//             EditorGUI.BeginDisabledGroup(!component.showLoadingIndicator);

//             // Loading Indicator Type
//             content = new GUIContent("Type", "Sets loading indicator type");
//             component.loadingIndicatorType = (SDWebImage.LoadingIndicatorType)EditorGUILayout.EnumPopup(content, component.loadingIndicatorType);
//             HandleSelectedIndicatorType(component.loadingIndicatorType);

//             // Loading Indicator Scale
//             content = new GUIContent("Scale", "Sets loading indicator scale");
//             component.loadingIndicatorScale = EditorGUILayout.Slider(content, component.loadingIndicatorScale, 0.0f, 10.0f);
//             if (component.loadingIndicator != null) {
//                 component.loadingIndicator.GetComponent<RectTransform>().localScale = Vector3.one * component.loadingIndicatorScale;
//             }

//             // Loading Indicator Color
//             content = new GUIContent("Color", "Sets loading indicator color");
//             component.loadingIndicatorColor = EditorGUILayout.ColorField(content, component.loadingIndicatorColor);
//             if (component.loadingIndicator != null) {
//                 component.loadingIndicator.GetComponent<Image>().color = component.loadingIndicatorColor;
//             }

//             EditorGUI.EndDisabledGroup();
//         }

//         EditorGUILayout.EndVertical();
//     }

//     private void HandleSelectedIndicatorType(SDWebImage.LoadingIndicatorType loadingIndicatorType) {
//         string loadingIndicatorPrefabsPath = "Assets/SDWebImage/Loading Indicators/Prefabs";
//         string loadingIndicatorPrefabName = GetLoadingIndicatorPrefabName(loadingIndicatorType);

//         if (String.IsNullOrEmpty(loadingIndicatorPrefabName)) {
//             DestroyImmediate(component.loadingIndicator);
//             return;
//         }

//         if (component.loadingIndicator != null && component.loadingIndicator.name == loadingIndicatorPrefabName) {
//             return;
//         }

//         DestroyImmediate(component.loadingIndicator);

//         string selectedLoadingIndicatorPath = loadingIndicatorPrefabsPath + "/" + loadingIndicatorPrefabName + ".prefab";
//         GameObject loadingIndicatorPrefab = AssetDatabase.LoadAssetAtPath(selectedLoadingIndicatorPath, typeof(GameObject)) as GameObject;

//         if (loadingIndicatorPrefab != null) {
//             GameObject loadingIndicator = (GameObject)PrefabUtility.InstantiatePrefab(loadingIndicatorPrefab);
//             loadingIndicator.name = loadingIndicatorPrefabName;
//             loadingIndicator.transform.SetParent(component.transform);
//             loadingIndicator.GetComponent<RectTransform>().localPosition = Vector3.zero;

//             component.loadingIndicator = loadingIndicator;
//         } else {
//             Debug.LogWarning("Make sure that " + loadingIndicatorPrefabName + " prefab is found in the following path <b>" + selectedLoadingIndicatorPath + "</b> then try again!");
//         }
//     }

//     private string GetLoadingIndicatorPrefabName(SDWebImage.LoadingIndicatorType loadingIndicatorType) {
//         switch (loadingIndicatorType) {
//             case SDWebImage.LoadingIndicatorType.RoundedRect:
//                 return "Rounded Rect Indicator";
//             case SDWebImage.LoadingIndicatorType.Circle:
//                 return "Circle Indicator";
//             case SDWebImage.LoadingIndicatorType.Circles:
//                 return "Circles Indicator";
//         }

//         return "";
//     }
// }
