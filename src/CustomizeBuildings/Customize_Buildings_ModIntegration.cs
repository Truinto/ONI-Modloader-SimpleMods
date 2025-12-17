using HarmonyLib;
using System;

namespace CustomizeBuildings
{
    /// <summary>
    /// This class can be copied by other mods to easily read values from the mod config of CustomizeBuildings
    /// </summary>
    public static class Customize_Buildings_ModIntegration
    {
        static object ConfigState = null;
        public static bool IntegrationActive { get; private set; } = false;
        static bool _initializationRun = false;

        /// <summary>
        /// Fetches a reference to the config state object to later read values from it
        /// </summary>
        public static void Init()
        {
            if (_initializationRun)
                return;
            _initializationRun = true;

            ///fetch the config class type
            var CustomizeBuildings_CustomizeBuildingsState = Type.GetType("CustomizeBuildings.CustomizeBuildingsState, CustomizeBuildings");
            if (CustomizeBuildings_CustomizeBuildingsState == null)
            {
                Debug.Log("CustomizeBuildings types not found.");
                return;
            }
            ///fetch the method info of the static getter method
            var m_GetModConfigState = AccessTools.Method(CustomizeBuildings_CustomizeBuildingsState.GetType(), "GetModConfigState");
            if (m_GetModConfigState == null)
            {
                Debug.LogWarning("CustomizeBuildings.CustomizeBuildingsState.GetModConfigState method not found.");
                return;
            }
            ///fetch the mod config state object 
            ConfigState = m_GetModConfigState.Invoke(null, null);
            IntegrationActive = ConfigState != null;
        }

        /// <summary>
        /// wrapper that allows fetching a config state value of type T
        /// Can only reliably be called after all mods are loaded, e.g. during a non-transpiler patch or in a building/entity definition method
        /// </summary>
        /// <typeparam name="T">type of the property to fetch</typeparam>
        /// <param name="propertyName">name of the property</param>
        /// <param name="value">fetched value of the property. default(T) if not found or the types dont match</param>
        /// <returns>bool if the property was found successfully</returns>
        public static bool TryGetConfigValue<T>(string propertyName, out T value)
        {
            value = default(T);
            Init();
            if (!IntegrationActive)
                return false;

            Traverse property = Traverse.Create(ConfigState).Property(propertyName);
            if (!property.PropertyExists())
            {
                Debug.LogWarning("Mod Config State did not have a property with the name: " + propertyName);
                return false;

            }
            object propertyValue = property.GetValue();
            var foundType = propertyValue.GetType();
            var T_Type = typeof(T);
            if (foundType != T_Type)
            {
                Debug.LogWarning("Mod Config State had a property with the name: " + propertyName + ", but it was typeOf " + foundType.Name + ", instead of the expected " + T_Type.Name);
                return false;
            }

            value = (T)propertyValue;
            return true;
        }
    }
}
