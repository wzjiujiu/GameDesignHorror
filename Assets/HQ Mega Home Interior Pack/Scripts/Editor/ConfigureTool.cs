/*
    TODO:
        1. Clean-up
        2. Restructure
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace WFDM
{
    public class ConfigureTool : EditorWindow
    {
        enum InitializeState { Success, ObjectNull, NotInScene }

        #region CLASSES

        abstract class Piece
        {
            public string originalPrefabFolder = "";
            public string originalPrefabName = "";

            public bool foldout;

            public GameObject gameObject = null;
            public bool init = false;

            public Dictionary<string, object> parameters = new Dictionary<string, object>();

            public T GetParameter<T>(string name)
            {
                if (!parameters.ContainsKey(name)) { return default; }
                var val = parameters[name];

                if (val is T)
                {
                    return (T)val;
                }
                try
                {
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default;
                }
            }
            public bool SetParameter<T>(string name, T val)
            {
                if (!parameters.ContainsKey(name))
                {
                    parameters.Add(name, val);
                    return true;
                }
                else
                {

                    if (parameters[name] is T)
                    {
                        var pre = parameters[name];
                        parameters[name] = val;

                        return !((T)parameters[name]).Equals(pre);
                    }
                    else
                    {
                        Debug.LogError("The already stored wall option value is not of the same type as the new requested to set value.");
                    }
                }
                return false;
            }

            public abstract void Initialize();
        }

        private class Wall : Piece
        {
            public enum WallType { Door, InnerCorner, OuterCorner, Straight, Window }

            public const string RootFolderName = "Modular Walls/";
            public List<WallProperty> wallProperties = new List<WallProperty>();

            public Wall(GameObject obj)
            {
                gameObject = obj;
            }

            public override void Initialize()
            {
                if (replaced != null)
                {
                    this.foldout = replaced.foldout;
                    for (int i = 0; i < wallProperties.Count; i++)
                    {
                        wallProperties[i].SetMaterial(replaced.wallProperties[i].matIndex);
                        if (wallProperties[i].baseboard)
                        {
                            wallProperties[i].SetCeilingBaseboard(replaced.wallProperties[i].ceilingBaseboard);
                            wallProperties[i].SetFloorBaseboard(replaced.wallProperties[i].floorBaseboard);
                        }
                    }

                    switch (GetParameter<WallType>("WallType"))
                    {
                        case WallType.Door:
                            {
                                int doorMaterialIndex = replaced.GetParameter<int>("DoorMaterialIndex");
                                var doorMaterials = GetParameter<Material[]>("DoorMaterialVariants");

                                SetObjectMaterial(gameObject, doorMaterials[doorMaterialIndex], true, "Doorway/Door *");
                                SetParameter("DoorMaterialIndex", doorMaterialIndex);
                                break;
                            }
                        case WallType.Window:
                            int windowMaterialIndex = replaced.GetParameter<int>("WindowMaterialIndex");
                            var windowMaterials = GetParameter<Material[]>("WindowMaterialVariants");

                            SetObjectMaterial(gameObject, windowMaterials[windowMaterialIndex], true, "Window", "Window/Openable Window");
                            SetParameter("WindowMaterialIndex", windowMaterialIndex);
                            break;
                    }

                    replaced = null;
                }
                else
                {
                    for (int i = 0; i < wallProperties.Count; i++)
                    {
                        wallProperties[i].matIndex = Array.FindIndex(materialVariants, m => m == wallProperties[i].GetWallMaterial());
                        if (wallProperties[i].baseboard)
                        {
                            wallProperties[i].SetCeilingBaseboard(wallProperties[i].baseboard.Find("Ceiling").gameObject.activeSelf);
                            wallProperties[i].SetFloorBaseboard(wallProperties[i].baseboard.Find("Floor").gameObject.activeSelf);
                        }
                    }
                }
            }
        }
        class WallProperty
        {
            public readonly string name;
            public readonly GameObject obj;

            public readonly bool initialized = false;

            readonly MeshRenderer renderer;
            public int matIndex = -1;

            public readonly Transform baseboard = null;
            public bool ceilingBaseboard = true, floorBaseboard = true;

            public WallProperty(GameObject root, string name, string customName = default)
            {
                this.name = customName != default ? customName : name;

                if (string.IsNullOrEmpty(this.name))
                {
                    obj = root;
                }
                else
                {
                    var child = root.transform.Find(name);
                    if (!child)
                    {
                        Debug.LogErrorFormat("Wall by the name {0} is not found.", name);
                        return;
                    }
                    obj = child.gameObject;
                }

                renderer = obj.GetComponent<MeshRenderer>();
                baseboard = obj.transform.Find("Baseboards");

                if (baseboard)
                {
                    ceilingBaseboard = baseboard.Find("Ceiling").gameObject.activeSelf;
                    floorBaseboard = baseboard.Find("Floor").gameObject.activeSelf;
                }

                initialized = true;
            }

            public void SetMaterial(int materialIndex)
            {
                SetObjectMaterial(obj, materialVariants[materialIndex], true, "");
                matIndex = materialIndex;
            }
            public Material GetWallMaterial()
            {
                return renderer.sharedMaterial;
            }

            public void SetCeilingBaseboard(bool state)
            {
                if (!baseboard) { return; }

                ceilingBaseboard = state;
                baseboard.Find("Ceiling").gameObject.SetActive(state);
            }
            public void SetFloorBaseboard(bool state)
            {
                if (!baseboard) { return; }

                floorBaseboard = state;
                baseboard.Find("Floor").gameObject.SetActive(state);
            }
        }

        private class Block : Piece
        {
            public enum PlaceSide { Left, Right, Front, Back }

            public const string RootFolderName = "Block/";

            public Block.PlaceSide side = Block.PlaceSide.Left;

            public BlockTile[] tiles = null;

            public Block(GameObject obj)
            {
                gameObject = obj;
            }

            public override void Initialize()
            {
                var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();

                renderers = renderers.Where(r => r != null).ToArray();

                tiles = new BlockTile[renderers.Length];
                for (int i = 0; i < renderers.Length; i++)
                {
                    var index = Array.FindIndex(materialVariants, m => m == renderers[i].sharedMaterial);
                    tiles[i] = new BlockTile(renderers[i], index);
                }
            }

            public void SetMaterial(int materialIndex, int tileIndex)
            {
                SetObjectMaterial(tiles[tileIndex].renderer, materialVariants[materialIndex], true);
                tiles[tileIndex].matIndex = materialIndex;
            }

            public GameObject PlaceNeighbor()
            {
                var newBlock = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(GetUnityPath(Path.Combine(originalPrefabFolder, originalPrefabName + ".prefab")), typeof(GameObject)));
                newBlock.transform.position = gameObject.transform.position;

                var meshSize = GetBounds();

                var pos = newBlock.transform.position;
                switch (side)
                {
                    case PlaceSide.Left:
                        pos.x -= meshSize.size.x;
                        break;
                    case PlaceSide.Right:
                        pos.x += meshSize.size.x;
                        break;
                    case PlaceSide.Front:
                        pos.z += meshSize.size.z;
                        break;
                    case PlaceSide.Back:
                        pos.z -= meshSize.size.z;
                        break;
                }
                
                newBlock.transform.position = pos;

                return newBlock;
            }
            public Bounds GetBounds()
            {
                Renderer[] renderers = new Renderer[tiles.Length];
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i] = tiles[i].renderer;
                }

                return GetMeshBounds(gameObject.transform.position, renderers);
            }
        }
        class BlockTile
        {
            public Renderer renderer;
            public int matIndex;

            public BlockTile(Renderer renderer, int matIndex)
            {
                this.renderer = renderer;
                this.matIndex = matIndex;
            }
        }

        #endregion

        readonly List<Piece> selectedPieces = new List<Piece>();

        public static string RootFolder;

        static Material[] materialVariants;
        static string[] materialNames;

        //MULTI SELECTION PROPERTIES
        bool individual = true;
        int groupMatIndex = 0;
        bool groupCeilingBaseboard = false, groupFloorBaseboard = false;
        Block.PlaceSide groupSide = Block.PlaceSide.Front;
        //MULTI SELECTION PROPERTIES

        Color defaultGUIColor;

        bool reload = false, targetsInitialized;

        int replaceIndex = -1;
        static Wall replaced = null;

        Vector2 scrollPosition;
        bool focus = false;

        bool selectingWalls = true;

        [MenuItem("Window/WFDM/Configure Tool")]
        public static void OpenWindow()
        {
            var window = EditorWindow.GetWindow<ConfigureTool>("Configure Tool");
            window.Show();
        }

        #region UNITY METHODS

        private void OnEnable()
        {
            defaultGUIColor = GUI.color;

            replaceIndex = -1;
            Selection.selectionChanged += ManageTargets;
            SceneView.duringSceneGui += DrawHandles;

            if (Selection.objects.Length > 0)
            {
                ManageTargets();
            }
        }
        private void OnDisable()
        {
            Selection.selectionChanged -= ManageTargets;
            SceneView.duringSceneGui -= DrawHandles;
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space();

            GUILayout.Label("Beta 1.01", EditorStyles.centeredGreyMiniLabel);
            GUILayout.Label("This plugin is still in development", EditorStyles.centeredGreyMiniLabel);
            GUILayout.Label("Experiencing any issues? Please contact watchfinddo@gmail.com", EditorStyles.centeredGreyMiniLabel);

            GUILayout.Label("-", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(selectingWalls);
            var s = new GUIStyle(EditorStyles.miniButtonLeft) { fixedHeight = 35, fontSize = 14 };
            if (GUILayout.Button("Walls", s))
            {
                selectingWalls = true;
                Selection.objects = null;

                return;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(!selectingWalls);
            s = new GUIStyle(EditorStyles.miniButtonRight) { fixedHeight = 35, fontSize = 14 };
            if (GUILayout.Button("Blocks", s))
            {
                selectingWalls = false;
                Selection.objects = null;

                return;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (!targetsInitialized)
            { ManageTargets(); EditorGUILayout.EndScrollView(); return; }

            reload = false;

            //DRAW MULTI SELECT BUTTONS
            if (selectedPieces.Count > 1)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(individual);
                if (GUILayout.Button("Individual", EditorStyles.miniButtonLeft)) { individual = true; }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!individual);
                if (GUILayout.Button("Group", EditorStyles.miniButtonRight)) { individual = false; }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            //IF SELECTED COUNT IS 1 OR AN INDIVIDUAL EDITING IS ENABLED
            if (individual || selectedPieces.Count == 1)
            {
                for (int i = 0; i < selectedPieces.Count; i++)
                {
                    if (selectedPieces.Count > 1)
                    {
                        selectedPieces[i].foldout = EditorGUILayout.Foldout(selectedPieces[i].foldout, new GUIContent(selectedPieces[i].gameObject.name));
                    }
                    if (selectedPieces[i].foldout || selectedPieces.Count == 1)
                    {
                        if (selectedPieces[i].gameObject != null)
                        {
                            if (selectingWalls)
                            {
                                DrawWall((Wall)selectedPieces[i]);
                            }
                            else
                            {
                                DrawBlock((Block)selectedPieces[i]);
                            }
                        }
                        if (reload)
                        {
                            EditorGUILayout.EndScrollView();
                            return;
                        }
                        EditorGUILayout.Space();
                    }
                }
            }

            //IF SELECTED COUNT IS MORE THAN 1
            else if (selectedPieces.Count > 1)
            {
                var style = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { wordWrap = true };
                GUILayout.Label($"Note: Multiple {(selectingWalls ? "walls" : "blocks")} are selected. Changing any properties will apply the change to all selected {(selectingWalls ? "walls" : "blocks")}.", style);

                EditorGUILayout.Space();

                if (materialNames != null)
                {
                    var _groupMatIndex = groupMatIndex;
                    groupMatIndex = EditorGUILayout.Popup(groupMatIndex, materialNames);
                    if (groupMatIndex != _groupMatIndex)
                    {
                        for (int i = 0; i < selectedPieces.Count; i++)
                        {
                            if (selectingWalls)
                            {
                                var wall = (Wall)selectedPieces[i];
                                for (int p = 0; p < wall.wallProperties.Count; p++)
                                {
                                    wall.wallProperties[p].SetMaterial(groupMatIndex);
                                }
                            }
                            else
                            {
                                var block = (Block)selectedPieces[i];
                                for (int t = 0; t < block.tiles.Length; t++)
                                {
                                    block.SetMaterial(groupMatIndex, t);
                                }
                            }
                        }
                    }
                }
                if (selectingWalls)
                {
                    EditorGUILayout.Space();

                    GUILayout.Label("Baseboards", EditorStyles.miniLabel);

                    EditorGUILayout.BeginHorizontal();

                    EditorGUI.BeginChangeCheck();
                    groupCeilingBaseboard = EditorGUILayout.Toggle("Ceiling", groupCeilingBaseboard);
                    if (EditorGUI.EndChangeCheck())
                    {
                        for (int i = 0; i < selectedPieces.Count; i++)
                        {
                            List<GameObject> baseboardsToUndo = new List<GameObject>();
                            for (int p = 0; p < ((Wall)selectedPieces[i]).wallProperties.Count; p++)
                            {
                                if (((Wall)selectedPieces[i]).wallProperties[p].baseboard == null) { continue; }

                                baseboardsToUndo.Add(((Wall)selectedPieces[i]).wallProperties[p].baseboard.Find("Ceiling").gameObject);
                            }
                            Undo.RecordObjects(baseboardsToUndo.ToArray(), "GroupWalls_BaseboardsChanged");
                            for (int p = 0; p < ((Wall)selectedPieces[i]).wallProperties.Count; p++)
                            {
                                if (((Wall)selectedPieces[i]).wallProperties[p].baseboard)
                                {
                                    ((Wall)selectedPieces[i]).wallProperties[p].SetCeilingBaseboard(groupCeilingBaseboard);
                                }
                            }
                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    groupFloorBaseboard = EditorGUILayout.Toggle("Floor", groupFloorBaseboard);
                    if (EditorGUI.EndChangeCheck())
                    {
                        for (int i = 0; i < selectedPieces.Count; i++)
                        {
                            List<GameObject> baseboardsToUndo = new List<GameObject>();
                            for (int p = 0; p < ((Wall)selectedPieces[i]).wallProperties.Count; p++)
                            {
                                if (((Wall)selectedPieces[i]).wallProperties[p].baseboard == null) { continue; }

                                baseboardsToUndo.Add(((Wall)selectedPieces[i]).wallProperties[p].baseboard.Find("Floor").gameObject);
                            }
                            Undo.RecordObjects(baseboardsToUndo.ToArray(), "GroupWalls_BaseboardsChanged");
                            for (int p = 0; p < ((Wall)selectedPieces[i]).wallProperties.Count; p++)
                            {
                                if (((Wall)selectedPieces[i]).wallProperties[p].baseboard)
                                {
                                    ((Wall)selectedPieces[i]).wallProperties[p].SetFloorBaseboard(groupFloorBaseboard);
                                }
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();
                    groupSide = (Block.PlaceSide)EditorGUILayout.EnumPopup(groupSide);
                    if (EditorGUI.EndChangeCheck())
                    {
                        for (int i = 0; i < selectedPieces.Count; i++)
                        {
                            ((Block)selectedPieces[i]).side = groupSide;
                        }
                    }
                    if (GUILayout.Button("Place"))
                    {
                        GameObject[] a = new GameObject[selectedPieces.Count];
                        for (int i = 0; i < selectedPieces.Count; i++)
                        {
                            a[i] = ((Block)selectedPieces[i]).PlaceNeighbor();
                        }
                        Selection.objects = a;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        #endregion

        #region DRAW METHODS

        public void DrawHandles(SceneView sceneView)
        {
            if (selectingWalls) { return; }

            for (int i = 0; i < selectedPieces.Count; i++)
            {
                var obj = selectedPieces[i].gameObject;

                if (selectedPieces[i] == null || obj == null)
                {
                    continue;
                }
                var pos = obj.transform.position;
                var blc = (Block)selectedPieces[i];
                var siz = blc.GetBounds();

                switch (blc.side)
                {
                    case Block.PlaceSide.Left:
                        Handles.DrawLine(pos, pos - new Vector3(siz.size.x / 2, 0, 0));
                        break;
                    case Block.PlaceSide.Right:
                        Handles.DrawLine(pos, pos + new Vector3(siz.size.x / 2, 0, 0));
                        break;
                    case Block.PlaceSide.Front:
                        Handles.DrawLine(pos, pos + new Vector3(0, 0, siz.size.z / 2));
                        break;
                    case Block.PlaceSide.Back:
                        Handles.DrawLine(pos, pos - new Vector3(0, 0, siz.size.z / 2));
                        break;
                }
                HandleUtility.Repaint();

            }
        }

        void DrawBlock(Block block)
        {
            if (!DrawInitState(block.init)) { return; }

            bool applyToAll = block.GetParameter<bool>("applyToAll");

            if (block.tiles.Length > 1)
            {
                GUI.color = applyToAll ? new Color(0.43f, 0.67f, 0.92f) : defaultGUIColor;
                if (GUILayout.Button("Apply to all"))
                {
                    block.SetParameter("applyToAll", applyToAll ^ true);

                    bool allSame = true; int matIndex = 0;
                    for (int i = 0; i < block.tiles.Length; i++)
                    {
                        if (i == 0)
                        {
                            matIndex = block.tiles[i].matIndex;
                            continue;
                        }
                        if (matIndex != block.tiles[i].matIndex)
                        {
                            allSame = false;
                            break;
                        }
                    }
                    block.SetParameter("allIndex", allSame ? matIndex : 0);
                }
                GUI.color = defaultGUIColor;
            }
            if (selectedPieces.Count == 1)
            {
                GUILayout.Label(block.gameObject.name, EditorStyles.largeLabel);
            }
            if (block.tiles.Length > 1 && applyToAll)
            {
                EditorGUILayout.Space();
                if (block.SetParameter("allIndex", EditorGUILayout.Popup(block.GetParameter<int>("allIndex"), materialNames)))
                {
                    for (int i = 0; i < block.tiles.Length; i++)
                    {
                        block.SetMaterial(block.GetParameter<int>("allIndex"), i);
                    }
                }
            }
            else
            {
                for (int i = 0; i < block.tiles.Length; i++)
                {
                    GUILayout.Label(block.tiles[i].renderer.name.Replace('_', ' '), EditorStyles.miniLabel);

                    EditorGUI.BeginChangeCheck();
                    block.tiles[i].matIndex = EditorGUILayout.Popup(block.tiles[i].matIndex, materialNames);
                    if (EditorGUI.EndChangeCheck())
                    {
                        block.SetMaterial(block.tiles[i].matIndex, i);
                    }
                }
            }
            GUILayout.Label("Place new Block", EditorStyles.miniLabel);

            EditorGUILayout.BeginHorizontal();
            block.side = (Block.PlaceSide)EditorGUILayout.EnumPopup(block.side);
            if (GUILayout.Button("Place"))
            {
                Selection.activeObject = block.PlaceNeighbor();
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawWall(Wall wall)
        {
            if (!DrawInitState(wall.init)) { return; }

            if (selectedPieces.Count == 1)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(wall.gameObject.name, EditorStyles.largeLabel);
            }

            if (GUILayout.Button("Focus", EditorStyles.miniButton, GUILayout.Width(76.5f)))
            {
                focus = true;

                var selected = Selection.objects;
                Selection.activeObject = wall.gameObject;

                SceneView.FrameLastActiveSceneView();

                Selection.objects = selected;

                focus = false;
            }

            if (selectedPieces.Count == 1)
            {
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.EnumPopup("Type", wall.GetParameter<Wall.WallType>("WallType"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            if (GUILayout.Button("Shift UVs"))
            {
                ShiftUV(wall);
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            wall.SetParameter("Paneled", EditorGUILayout.Toggle("Paneled", wall.GetParameter<bool>("Paneled")));
            if (EditorGUI.EndChangeCheck())
            {
                TogglePanel(wall);
            }

            for (int i = 0; i < wall.wallProperties.Count; i++)
            {
                EditorGUILayout.Space();
                DrawWallProperty(wall.wallProperties[i]);
            }

            EditorGUILayout.Space();

            switch (wall.GetParameter<Wall.WallType>("WallType"))
            {
                case Wall.WallType.Door:
                    var doorMaterials = wall.GetParameter<Material[]>("DoorMaterialVariants");
                    if (wall.SetParameter("DoorMaterialIndex", EditorGUILayout.Popup("Door Material", wall.GetParameter<int>("DoorMaterialIndex"), GetMaterialNames(doorMaterials))))
                    {
                        SetObjectMaterial(wall.gameObject, doorMaterials[wall.GetParameter<int>("DoorMaterialIndex")], true, "Doorway/Door *");
                    }
                    break;
                case Wall.WallType.Window:
                    var windowMaterials = wall.GetParameter<Material[]>("WindowMaterialVariants");
                    if (wall.SetParameter("WindowMaterialIndex", EditorGUILayout.Popup("Window Material", wall.GetParameter<int>("WindowMaterialIndex"), GetMaterialNames(windowMaterials))))
                    {
                        SetObjectMaterial(wall.gameObject, windowMaterials[wall.GetParameter<int>("WindowMaterialIndex")], true, "Window", "Window/Openable Window");
                    }

                    break;
                case Wall.WallType.Straight:
                    //var splitting = wall.GetParameter<bool>("Splitting");

                    //GUI.color = splitting ? new Color(0.43f, 0.67f, 0.92f) : defaultGUIColor;
                    //if (GUILayout.Button(splitting ? "Splitting" : "Split"))
                    //{
                    //    splitting ^= true;
                    //    wall.SetParameter("Splitting", splitting);
                    //}
                    //GUI.color = defaultGUIColor;

                    //if (splitting)
                    //{
                    //    List<string> options = new List<string>();
                    //    DirectoryInfo[] straightFolders = wall.GetParameter<DirectoryInfo[]>("StraightFolders");

                    //    for (int i = 0; i < straightFolders.Length; i++)
                    //    {
                    //        int dirIndex = wall.GetParameter<int>("DirectoryIndex");
                    //        if (dirIndex == i)
                    //        {
                    //            continue;
                    //        }
                    //        else if (dirIndex < i)
                    //        {
                    //            options.Add(straightFolders[i].Name);
                    //        }
                    //        else
                    //        {
                    //            break;
                    //        }
                    //    }
                    //    wall.SetParameter("SplitOption", EditorGUILayout.Popup(wall.GetParameter<int>("SplitOption"), options.ToArray()));
                    //}
                    break;
            }
        }
        void DrawWallProperty(WallProperty property)
        {
            GUILayout.Label(property.name, EditorStyles.miniLabel);

            var _matIndex = property.matIndex;
            property.matIndex = EditorGUILayout.Popup(property.matIndex, materialNames);
            if (property.matIndex != _matIndex)
            {
                property.SetMaterial(property.matIndex);

            }
            if (property.baseboard)
            {
                EditorGUILayout.Space();
                GUILayout.Label("Baseboards", EditorStyles.miniLabel);

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                property.ceilingBaseboard = EditorGUILayout.Toggle("Ceiling", property.ceilingBaseboard);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(property.baseboard.Find("Ceiling").gameObject, "Ceiling baseboards active state changed " + property.baseboard.GetInstanceID());
                    property.SetCeilingBaseboard(property.ceilingBaseboard);
                }

                EditorGUI.BeginChangeCheck();
                property.floorBaseboard = EditorGUILayout.Toggle("Floor", property.floorBaseboard);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(property.baseboard.Find("Floor").gameObject, "Floor baseboards active state changed " + property.baseboard.GetInstanceID());
                    property.SetFloorBaseboard(property.floorBaseboard);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        bool DrawInitState(bool init)
        {
            if (!init)
            {
                var s = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { wordWrap = true };

                GUILayout.Label("This object is not initialized. Place prefab to the scene and select the object.", s);
                GUILayout.Label("...", EditorStyles.centeredGreyMiniLabel);
                return false;
            }

            EditorGUILayout.Space();
            return true;
        }

        #endregion

        #region WALL SPECIFIC METHODS

        void ShiftUV(Wall wall)
        {
            var dir = new DirectoryInfo(wall.originalPrefabFolder);
            var prefabs = dir.GetFiles("*.prefab");

            int index = 0;
            for (int i = 0; i < prefabs.Length; i++)
            {
                if (prefabs[i].Name.Contains(wall.originalPrefabName))
                {
                    index = i;
                    break;
                }
            }
            index++;
            if (index == prefabs.Length)
            {
                index = 0;
            }

            Replace(wall, prefabs[index].FullName);
        }
        void TogglePanel(Wall wall)
        {
            var dir = new DirectoryInfo(wall.originalPrefabFolder).Parent;
            var path = Path.Combine(dir.FullName, wall.GetParameter<bool>("Paneled") ? "Paneled" : "Plain");

            dir = new DirectoryInfo(path);
            var files = dir.GetFiles();


            int fileIndex = -1;
            for (int i = 0; i < files.Length; i++)
            {
                var prefix = GetPrefix(wall);
                if (files[i].Name.Contains(prefix))
                {
                    fileIndex = i;
                    break;
                }
            }
            if (fileIndex == -1)
            {
                Debug.LogError("Could not replace object...");
                return;
            }
            Replace(wall, files[fileIndex].FullName);
        }
        void Replace(Wall wall, string newObjName)
        {
            replaced = wall;

            var nw = AssetDatabase.LoadAssetAtPath(GetUnityPath(newObjName), typeof(GameObject));
            var newWall = (GameObject)PrefabUtility.InstantiatePrefab(nw);

            newWall.transform.position = wall.gameObject.transform.position;
            newWall.transform.rotation = wall.gameObject.transform.rotation;

            var hierarchyIndex = wall.gameObject.transform.GetSiblingIndex();
            if (wall.gameObject.transform.parent)
            {
                newWall.transform.SetParent(wall.gameObject.transform.parent);
            }
            else
            {
                hierarchyIndex--;
            }

            replaceIndex = -1; var selected = Selection.objects;
            for (int i = 0; i < selected.Length; i++)
            {
                if ((GameObject)selected[i] == wall.gameObject)
                {
                    replaceIndex = i;
                    break;
                }
            }

            if (replaceIndex == -1)
            { Debug.LogError("Replace Index could not be set. The object is not found."); return; }

            UnityEngine.Object.DestroyImmediate(wall.gameObject);
            newWall.transform.SetSiblingIndex(hierarchyIndex);

            selected[replaceIndex] = newWall;
            Selection.objects = selected;

            reload = true; targetsInitialized = false;
        }

        #endregion

        #region SETUP AND INITIALIZATION METHODS

        void ManageTargets()
        {
            if (focus) { return; }

            targetsInitialized = false;

            var objs = Selection.objects;

            if (objs.Length == 0)
            {
                selectedPieces.Clear(); Repaint(); return;
            }

            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] == null || objs[i].GetType() != typeof(GameObject))
                {
                    continue;
                }
                var obj = (GameObject)objs[i];

                if (PrefabUtility.GetOutermostPrefabInstanceRoot(objs[i]) != obj)
                {
                    continue;
                }

                var toReplace = replaceIndex == i;

                if (selectedPieces.Find(w => w.gameObject == obj) != null)
                { continue; }

                var path = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(objs[i]));

                if (selectingWalls && !path.Contains(Wall.RootFolderName) || !selectingWalls && !path.Contains(Block.RootFolderName))
                {
                    continue;
                }
                if (selectingWalls)
                {
                    Piece wall = new Wall(obj);
                    var state = Initialize(ref wall);

                    wall.Initialize();

                    switch (state)
                    {
                        case InitializeState.Success:
                            wall.init = true;
                            if (toReplace)
                            {
                                selectedPieces[replaceIndex] = wall;
                                replaceIndex = -1;
                            }
                            else
                            {
                                selectedPieces.Add(wall);
                            }
                            break;
                        case InitializeState.ObjectNull:
                            Debug.LogErrorFormat("TThe selected gameObject is null.");
                            break;
                        case InitializeState.NotInScene:
                            Debug.LogErrorFormat("Could not initialize selected wall. Wall name: {0}", obj.name);
                            break;
                    }
                }
                else
                {
                    Piece block = new Block(obj);
                    var state = Initialize(ref block);

                    block.Initialize();

                    if (state == InitializeState.Success)
                    {
                        block.init = true;
                        selectedPieces.Add(block);
                    }
                }
            }
            #region REMOVE

            Queue<int> toRemoveIndexes = new Queue<int>();
            for (int i = 0; i < selectedPieces.Count; i++)
            {
                bool found = false;
                for (int o = 0; o < objs.Length; o++)
                {
                    if (objs[o].GetType() != typeof(GameObject))
                    {
                        continue;
                    }
                    if ((GameObject)objs[o] == selectedPieces[i].gameObject)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    toRemoveIndexes.Enqueue(i);
                }
            }
            Piece[] remove = new Piece[toRemoveIndexes.Count]; int count = toRemoveIndexes.Count;
            for (int i = 0, c = count; i < c; i++)
            {
                int index = toRemoveIndexes.Dequeue();
                remove[i] = selectedPieces[index];
            }
            for (int i = 0, c = count; i < c; i++)
            {
                selectedPieces.Remove(remove[i]);
            }
            targetsInitialized = true; Repaint();

            #endregion
        }
        InitializeState Initialize(ref Piece piece)
        {
            if (piece.gameObject == null)
            {
                return InitializeState.ObjectNull;
            }

            piece.originalPrefabFolder = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(piece.gameObject));
            if (piece.originalPrefabFolder == "" || PrefabUtility.IsPartOfPrefabAsset(piece.gameObject))
            {
                return InitializeState.NotInScene;
            }

            piece.originalPrefabName = new FileInfo(piece.originalPrefabFolder).Name;
            piece.originalPrefabFolder = piece.originalPrefabFolder.Replace(piece.originalPrefabName, "");

            piece.originalPrefabName = piece.originalPrefabName.Replace(".prefab", "");

            piece.gameObject.name = piece.originalPrefabName;

            var pieceRoot = selectingWalls ? Wall.RootFolderName : Block.RootFolderName;
            RootFolder = piece.originalPrefabFolder.Substring(0, piece.originalPrefabFolder.IndexOf(pieceRoot) + pieceRoot.Length);

            InitializeMaterials(RootFolder);

            if (selectingWalls)
            {
                piece.SetParameter("Paneled", piece.originalPrefabName.Contains("Paneled"));

                var dir = new DirectoryInfo(Path.Combine(RootFolder, "Prefabs"));

                var wall = (Wall)piece;

                var types = dir.GetDirectories();
                for (int i = 0; i < types.Length; i++)
                {
                    if (wall.originalPrefabFolder.Contains(types[i].Name))
                    {
                        wall.parameters.Add("WallType", (Wall.WallType)i);
                        break;
                    }
                }
                wall.wallProperties = new List<WallProperty>();
                switch (wall.GetParameter<Wall.WallType>("WallType"))
                {
                    case Wall.WallType.Door:
                        {
                            wall.wallProperties.Add(new WallProperty(wall.gameObject, "Wall_Side_1"));
                            wall.wallProperties.Add(new WallProperty(wall.gameObject, "", "Doorway Sides"));
                            wall.wallProperties.Add(new WallProperty(wall.gameObject, "Wall_Side_2"));

                            var doorway = wall.gameObject.transform.Find("Doorway");
                            string doorPath = null; GameObject door = null;
                            foreach (Transform child in doorway)
                            {
                                if (child.name.Contains("Door "))
                                {
                                    door = child.gameObject;
                                    doorPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromOriginalSource(door));
                                    break;
                                }
                            }
                            if (!string.IsNullOrEmpty(doorPath))
                            {
                                dir = new DirectoryInfo(doorPath);
                                var doorMaterialPaths = new DirectoryInfo(Path.Combine(dir.Parent.Parent.FullName, "Materials")).GetFiles("*.mat");

                                Material[] doorMaterials = new Material[doorMaterialPaths.Length];
                                for (int i = 0; i < doorMaterialPaths.Length; i++)
                                {
                                    doorMaterials[i] = AssetDatabase.LoadAssetAtPath<Material>(GetUnityPath(doorMaterialPaths[i].FullName));
                                }
                                wall.parameters.Add("DoorMaterialVariants", doorMaterials);
                                wall.parameters.Add("DoorMaterialIndex", Array.FindIndex(doorMaterials, m => m == door.GetComponent<Renderer>().sharedMaterial));
                            }
                            break;
                        }
                    case Wall.WallType.InnerCorner:
                        {
                            dir = new DirectoryInfo(wall.originalPrefabFolder);
                            wall.parameters.Add("InnerSeperate", dir.Parent.Name != "Predefined");
                            if (wall.GetParameter<bool>("InnerSeperate"))
                            {
                                wall.parameters.Add("InnerStart", dir.Parent.Name == "Start");
                                wall.wallProperties.Add(new WallProperty(wall.gameObject, "", "Inner Part Wall"));
                            }
                            else
                            {
                                wall.wallProperties.Add(new WallProperty(wall.gameObject, "", "Start Wall"));
                                wall.wallProperties.Add(new WallProperty(wall.gameObject, "End Wall"));
                            }
                            break;
                        }
                    case Wall.WallType.OuterCorner:
                        {
                            wall.wallProperties.Add(new WallProperty(wall.gameObject, "", "Outer Corner"));
                            break;
                        }
                    case Wall.WallType.Straight:
                        {
                            wall.wallProperties.Add(new WallProperty(wall.gameObject, "", "Straight Wall"));

                            wall.parameters.Add("StraightFolders", new DirectoryInfo(wall.originalPrefabFolder).Parent.Parent.GetDirectories());
                            for (int i = 0; i < wall.GetParameter<DirectoryInfo[]>("StraightFolders").Length; i++)
                            {
                                if (new DirectoryInfo(wall.originalPrefabFolder).Parent.Name.Contains(wall.GetParameter<DirectoryInfo[]>("StraightFolders")[i].Name))
                                {
                                    wall.parameters.Add("DirectoryIndex", i);
                                    break;
                                }
                            }
                            break;
                        }
                    case Wall.WallType.Window:
                        {
                            wall.wallProperties.Add(new WallProperty(wall.gameObject, "", "Window Wall"));

                            var window = wall.gameObject.transform.Find("Window");
                            var windowPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromOriginalSource(window));

                            if (!string.IsNullOrEmpty(windowPath))
                            {
                                dir = new DirectoryInfo(windowPath);
                                var windowMaterialPaths = new DirectoryInfo(Path.Combine(dir.Parent.Parent.FullName, "Materials")).GetFiles("*.mat");

                                Material[] windowMaterials = new Material[windowMaterialPaths.Length];
                                for (int i = 0; i < windowMaterialPaths.Length; i++)
                                {
                                    windowMaterials[i] = AssetDatabase.LoadAssetAtPath<Material>(GetUnityPath(windowMaterialPaths[i].FullName));
                                }
                                wall.parameters.Add("WindowMaterialVariants", windowMaterials);
                                wall.parameters.Add("WindowMaterialIndex", Array.FindIndex(windowMaterials, m => m == window.GetComponent<Renderer>().sharedMaterial));
                            }
                            break;
                        }
                }
                piece = wall;
            }
            else
            {
                if (Selection.objects.Length > 1 && !individual)
                {
                    ((Block)piece).side = groupSide;
                }
            }

            return InitializeState.Success;
        }
        void InitializeMaterials(string root)
        {
            var materialsFolder = new DirectoryInfo(Path.Combine(root, "Materials"));
            var materialFiles = materialsFolder.GetFiles("*.mat", SearchOption.AllDirectories);

            materialVariants = new Material[materialFiles.Length]; materialNames = new string[materialFiles.Length];
            for (int i = 0; i < materialFiles.Length; i++)
            {
                materialVariants[i] = AssetDatabase.LoadAssetAtPath<Material>(GetUnityPath(materialFiles[i].FullName));
                materialNames[i] = materialVariants[i].name;
            }
        }

        #endregion

        #region UTILITIES

        #region GETTERS

        string GetPrefix(Wall wall)
        {
            var prefix = wall.originalPrefabName.Substring(wall.originalPrefabName.Length - 3, 3);
            if (!prefix.Contains("-"))
            {
                if (wall.GetParameter<Wall.WallType>("WallType") == Wall.WallType.Straight)
                {
                    prefix = wall.originalPrefabName.Substring(wall.originalPrefabName.Length - 1, 1);
                }
                else if (wall.GetParameter<Wall.WallType>("WallType") == Wall.WallType.InnerCorner)
                {
                    if (wall.GetParameter<bool>("InnerSeperate"))
                    {
                        prefix = wall.originalPrefabName.Substring(wall.originalPrefabName.Length - 2, 2);
                    }
                    else
                    {
                        Debug.LogError("Could not parse prefix.");
                        return null;
                    }
                }
            }
            return prefix;
        }
        public static int GetMaterialIndex(string name)
        {
            return Array.FindIndex(materialNames, x => x == name);
        }
        public static string GetUnityPath(string path)
        {
            var startIndex = path.IndexOf("Assets");
            var newPath = path.Remove(0, startIndex);

            return newPath;
        }
        static bool GetChildObject(string name, GameObject root, out Transform obj)
        {
            obj = null;
            if (name.Contains("*"))
            {
                var partial = name.Replace("*", "");
                foreach (Transform child in root.transform)
                {
                    if (child.name.Contains(partial))
                    {
                        obj = child;
                        return true;
                    }
                }
            }
            else
            {
                obj = root.transform.Find(name);
                if (obj)
                {
                    return true;
                }
            }
            return false;
        }
        string[] GetMaterialNames(Material[] materials)
        {
            string[] names = new string[materials.Length];
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = materials[i].name;
            }
            return names;
        }
        public static Bounds GetMeshBounds(Vector3 center, Renderer[] renderers)
        {
            var bounds = new Bounds(center, Vector3.one);
            foreach (Renderer renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            return bounds;
        }

        #endregion

        #region SETTERS

        public static void SetObjectMaterial(GameObject root, Material material, bool applyUndo, params string[] paths)
        {
            if (root == null) { return; }

            for (int i = 0; i < paths.Length; i++)
            {
                var obj = root.transform;
                if (paths[i].Contains("/"))
                {
                    var path = paths[i].Split('/');
                    for (int p = 0; p < path.Length; p++)
                    {
                        GetChildObject(path[p], obj.gameObject, out obj);
                    }
                }
                else if (paths[i].Length > 0)
                {
                    GetChildObject(paths[i], obj.gameObject, out obj);
                }
                //Else if path is empty, then use root gameObject.

                var renderer = obj.gameObject.GetComponent<Renderer>();
                if (renderer)
                {
                    SetObjectMaterial(renderer, material, applyUndo);
                }
            }
        }
        public static void SetObjectMaterial(Renderer renderer, Material material, bool applyUndo)
        {
            if (renderer == null) { return; }

            if (applyUndo)
            {
                Undo.RecordObject(renderer, "Material_Changed_" + renderer.GetInstanceID());
            }
            renderer.sharedMaterial = material;
        }

        #endregion

        #endregion
    }
}