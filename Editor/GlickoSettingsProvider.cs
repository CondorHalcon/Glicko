using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace CondorHalcon.Glicko.Editor
{
    /// <summary>
    /// The Glicko settings editor provider.
    /// </summary>
    internal class GlickoSettingsProvider : SettingsProvider
    {
        /// <summary> The serialized settings object. </summary>
        private SerializedObject m_Settings;
        /// <summary> The scroll position of the settings window. </summary>
        internal static Vector2 scrollPosition = Vector2.zero;
        /// <summary>
        /// Creates a new instance of the GlickoSettingsProvider.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="scopes"></param>
        public GlickoSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project)
            : base(path, scopes, new List<string> { "Glicko", "CondorHalcon" }) { }

        /// <summary>
        /// Called when the settings provider is activated.
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="rootElement"></param>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            GlickoSettings settings = GlickoSettings.GetOrCreateSettings();
            if (settings != null)
            {
                m_Settings = new SerializedObject(settings);
            }
        }
        /// <summary>
        /// Called when the settings provider draws the GUI.
        /// </summary>
        /// <param name="searchContext"></param>
        public override void OnGUI(string searchContext)
        {
            if (m_Settings != null)
            {
                m_Settings.Update();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.PropertyField(m_Settings.FindProperty("kDefaultR"), new GUIContent("Default Rating (µ)", "Default / initial rating value"));
                EditorGUILayout.PropertyField(m_Settings.FindProperty("kDefaultRD"), new GUIContent("Default Deviation (φ)", "Default / initial deviation value"));
                EditorGUILayout.PropertyField(m_Settings.FindProperty("kDefaultS"), new GUIContent("Default Volatility (σ)", "Default / initial volatility value"));
                EditorGUILayout.PropertyField(m_Settings.FindProperty("kScale"), new GUIContent("Scale Factor", "Glicko-1 to Glicko-2 scale factor"));
                EditorGUILayout.PropertyField(m_Settings.FindProperty("kSystemConst"), new GUIContent("System Constant (τ)"));
                EditorGUILayout.PropertyField(m_Settings.FindProperty("kConvergence"), new GUIContent("Convergence Constant (ε)"));
                m_Settings.ApplyModifiedPropertiesWithoutUndo();
                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Settings not found.", MessageType.Error);
            }
        }
        /// <summary>
        /// Create a new instance of the GlickoSettingsProvider.
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateGlickoSettingsProvider()
        {
            return new GlickoSettingsProvider("Project/CondorHalcon/GlickoSettings", SettingsScope.Project);
        }
    }
}
