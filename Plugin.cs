using BepInEx;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MoroccanCouch
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        AssetBundle _assetBundle;
        public static Plugin Instance;

        public InventoryItem Inv_MoroccanCouch;
        public TileObject moroccanCouchTile;
        public TileObjectSettings moroccanCouchTileSettings;

        public int furnitureCount = 1;
        public bool furnitureLoaded;
        public int firstID;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        void Start()
        {
            LoadAssetBundle("moroccancouch");
            BuildFurniture();
        }

        void Update()
        {
            AssignFurniture();
        }

        void BuildFurniture()
        {
            //var prefab = _assetBundle.LoadAsset<GameObject>("Inv_OpalShovel");
            Inv_MoroccanCouch = _assetBundle.LoadAsset<GameObject>("Inv_MoroccanCouch").GetComponent<InventoryItem>();
            moroccanCouchTile = _assetBundle.LoadAsset<GameObject>("396 MoroccanCouch").GetComponent<TileObject>();
            moroccanCouchTileSettings = _assetBundle.LoadAsset<GameObject>("396 _MoroccanCouch").GetComponent<TileObjectSettings>();

            _assetBundle.Unload(false);
            Logger.LogInfo("Asset Bundle Loaded: " + Inv_MoroccanCouch.gameObject.name);
            Logger.LogInfo("Asset Bundle Loaded: " + moroccanCouchTile.gameObject.name);
            Logger.LogInfo("Asset Bundle Loaded: " + moroccanCouchTileSettings.gameObject.name);
        }

        void AssignFurniture()
        {
            if (NetworkMapSharer.share.localChar is null) return;
            if (furnitureLoaded) return;
            Logger.LogInfo("Assigning Furniture...");

            Array.Resize<InventoryItem>(ref Inventory.inv.allItems, Inventory.inv.allItems.Length + furnitureCount);
            firstID = Inventory.inv.allItems.Length - 1;

            Inventory.inv.allItems[firstID] = Inv_MoroccanCouch;
            Logger.LogInfo($"Loaded: {Inv_MoroccanCouch.name}");

            Array.Resize<TileObject>(ref WorldManager.manageWorld.allObjects, WorldManager.manageWorld.allObjects.Length + furnitureCount);
            firstID = WorldManager.manageWorld.allObjects.Length - 1;

            WorldManager.manageWorld.allObjects[firstID] = moroccanCouchTile;
            Logger.LogInfo($"Loaded: {moroccanCouchTile.name}");

            Array.Resize<TileObjectSettings>(ref WorldManager.manageWorld.allObjectSettings, WorldManager.manageWorld.allObjectSettings.Length + furnitureCount);
            firstID = WorldManager.manageWorld.allObjectSettings.Length - 1;

            WorldManager.manageWorld.allObjectSettings[firstID] = moroccanCouchTileSettings;
            Logger.LogInfo($"Loaded: {moroccanCouchTileSettings.name}");

            Logger.LogInfo("Furniture Loaded.");
            furnitureLoaded = true;
        }

        private Stream GetEmbeddedAssetBundle(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var presumed = assembly.GetManifestResourceNames().ToList().Find(resource => resource.Contains(name));

            if (!string.IsNullOrEmpty(presumed)) return assembly.GetManifestResourceStream(presumed);

            Logger.LogError($"Unable to find any embedded resource with name: {name}.");
            return null;
        }

        private void LoadAssetBundle(string name)
        {
            var resource = GetEmbeddedAssetBundle(name);
            _assetBundle = AssetBundle.LoadFromStream(resource);

            if (_assetBundle != null) return;

            Logger.LogError("Unable to load embedded asset bundle.");
            return;
        }

        private void UnloadAssetBundle()
        {
            _assetBundle.Unload(true);
        }

        void OnDestroy()
        {
            UnloadAssetBundle();
        }
    }
}
