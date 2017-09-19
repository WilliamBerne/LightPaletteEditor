using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class LightPaletteEditor : EditorWindow
{
    static public LightPaletteEditor instance;
    
    public Color DirectionalLightColor = new Vector4(0.7f, 0.847f, 0.918f, 1f);
    public Color AmbientLightColor;
    public Color FogColor = new Vector4(0.922f, 0.537f, 0.11f, 1f);
    public float LinearFogStart = 20f;
    public float LinearFogEnd = 55f;

    public Color BgColor = new Vector4(0.922f, 0.537f, 0.11f, 1f);
    //public float BloomIntesity = 0.65f;
    
    //public Color FloorDiffuseColor;
    public Color FloorDyeColor1 = Color.white;
    public Color FloorDyeColor2 = Color.white;
    //public Color FloorReflectionColor;
    public Color FloorEmissionColor = Color.white;

    public Color OutsideWallLightColor = Color.white;
    public Color PillarLightColor1 = Color.white;
    public Color PillarLightColor2 = Color.white;
    public Color WallEmissionColor1 = Color.white;
    public Color WallEmissionColor2 = Color.white;

    public Color PropEmissionColor1 = Color.white;
    public Color PropEmissionColor2 = Color.white;
    public Color GateEmissionColor1 = Color.white;

    
    int selGridInt = 0;
    int currentPalettleIndex = 0;
    bool firstTime = true;


    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Light Palette Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(LightPaletteEditor), false, "Palette");

    }

    void Init()
    {
        if (firstTime)
        {
            firstTime = false;
            Debug.Log(EditorApplication.timeSinceStartup + " Start");

            // make sure the text is already exsited
            string path = "Assets/Resources/ScenePalette.txt";
            if (!File.Exists(path))
            {
                InitializePaletteList();
                SaveToFile(); 
            }

            ReadFromFile();
            RestoreFromChosenPalette(currentPalettleIndex);
            MakeChange();
        }
    }

    // Test assigning object
    //Object savedText;

    void OnGUI()
    {
        Init();

        // colors and floats input field
        GUILayout.Space(15);
        GUILayout.Label("Render Settings", EditorStyles.boldLabel);
        DirectionalLightColor = EditorGUILayout.ColorField("Directional Color", DirectionalLightColor);
        AmbientLightColor = EditorGUILayout.ColorField("Ambient Color", AmbientLightColor);
        GUILayout.Space(10);

        GUILayout.Label("Fog Settings", EditorStyles.boldLabel);
        BgColor = EditorGUILayout.ColorField("Background Color", BgColor);
        FogColor =              EditorGUILayout.ColorField("Fog Color", FogColor);
        LinearFogStart =        EditorGUILayout.FloatField("Linear Fog Start", LinearFogStart);
        LinearFogEnd =          EditorGUILayout.FloatField("Linear Fog End", LinearFogEnd);   

        GUILayout.Space(10);
        GUILayout.Label("Floor Settings", EditorStyles.boldLabel);
        FloorDyeColor1 = EditorGUILayout.ColorField("Floor Dye Color 1", FloorDyeColor1);
        FloorDyeColor2 = EditorGUILayout.ColorField("Floor Dye Color 2", FloorDyeColor2);
        FloorEmissionColor = EditorGUILayout.ColorField("Floor Emission Color", FloorEmissionColor);
        
        GUILayout.Space(10);
        GUILayout.Label("Wall Settings", EditorStyles.boldLabel);
        OutsideWallLightColor = EditorGUILayout.ColorField("Outside Wall Color", OutsideWallLightColor);
        PillarLightColor1 = EditorGUILayout.ColorField("Pillar Light Color 1", PillarLightColor1);
        PillarLightColor2 = EditorGUILayout.ColorField("Pillar Light Color 2", PillarLightColor2);
        WallEmissionColor1 = EditorGUILayout.ColorField("Wall Emission Color 1", WallEmissionColor1);
        WallEmissionColor2 = EditorGUILayout.ColorField("Wall Emission Color 2", WallEmissionColor2);

        GUILayout.Space(10);
        GUILayout.Label("Prop Settings", EditorStyles.boldLabel);
        PropEmissionColor1 = EditorGUILayout.ColorField("Prop Emission Color 1", PropEmissionColor1);
        PropEmissionColor2 = EditorGUILayout.ColorField("Prop Emission Color 2", PropEmissionColor2);
        GateEmissionColor1 = EditorGUILayout.ColorField("Gate Emission Color", GateEmissionColor1);

        GUILayout.Space(10);

        ///////////////////////
        // 3x3 palette array
        string[] selStrings = new string[] { "Palette1", "Palette2", "Palette3", "Palette4", "Palette5", "Palette6", "Palette7", "Palette8", "Palette9" };
        GUILayout.BeginVertical("Box");
        selGridInt = GUILayout.SelectionGrid(selGridInt, selStrings, 3);

        // when a palette button was clicked
        if (selGridInt != -1)
        {
            if (currentPalettleIndex == -1)
            {
                Debug.Log("First time use palette list. currentPalettleIndex = " + currentPalettleIndex);
                // if this is the first time the buttons were clicked, only generate new currentPalettleIndex
                currentPalettleIndex = selGridInt;
                InitializePaletteList();

            }
            else if(selGridInt != currentPalettleIndex)
            {
                Debug.Log("old currentPalettleIndex = " + currentPalettleIndex);
                // get saved palettes
                // every time the editor was played, it loses PaletteList. in order to keep PaletteList work, we need to reinitialize it, once it doesnt exist
                if (PaletteList.Count == 0)
                {
                    //InitializePaletteList();
                    GetLightingFromFile();
                }

                SavePalette(currentPalettleIndex);
                currentPalettleIndex = selGridInt;
                RestoreFromChosenPalette(currentPalettleIndex);
                MakeChange();
                Debug.Log("new currentPalettleIndex = " + currentPalettleIndex);
            }
        }

        GUILayout.EndVertical();
        // 3x3 palette array end
        ///////////////////////



        /////////////////////////
        // make change button
        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Make Change", GUILayout.Height(30), GUILayout.MaxWidth(200), GUILayout.ExpandWidth(false)))
        {
            MakeChange();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        // make change button end
        /////////////////////////


        /////////////////////////
        //save & read txt buttons
        GUILayout.Space(5);

        // Test assigning object
        //savedText = EditorGUILayout.ObjectField(savedText, typeof(Object), true);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        // SAVE
        if (GUILayout.Button("Save Palette" + (currentPalettleIndex + 1), GUILayout.Height(30), GUILayout.MaxWidth(150)))
        {
            Debug.Log("saving currentPalettleIndex = " + currentPalettleIndex + ", PaletteList.Count = " + PaletteList.Count);
            MakeChange();
            SavePalette(currentPalettleIndex);
            SaveToFile();
        }
        GUILayout.FlexibleSpace();

        // READ
        if (GUILayout.Button("Restore Palette" + (currentPalettleIndex + 1), GUILayout.Height(30), GUILayout.MaxWidth(150)))
        {
            GetLightingFromFile();
            RestoreFromChosenPalette(currentPalettleIndex);
            MakeChange();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        ShowErrorMessage();

        // save & read end
        ////////////////////////


    }

    void DrawLine(Color col, float start = 0, float end = 3000)
    {
        Handles.color = col;
        Handles.DrawLine(new Vector3(start, 3), new Vector3(end, 3));
    }


    // Error Message timer
    static float showTime = 0;
    static float duration = 4.0f;

    void StartShowErrorMessage()
    {
        showTime = (float)EditorApplication.timeSinceStartup;
        Debug.Log("showTime = " + showTime);
    }

    // Error message will be shown for at least 4s. 
    // But editor window is not refreshed every seconds, so the duration of this error message might be much longer than 4s.
    void ShowErrorMessage()
    {
        if (showTime + duration > (float)EditorApplication.timeSinceStartup)
        {
            EditorGUILayout.HelpBox("None existing Palette data.\nSave the data first, before reading one.\n", MessageType.Error, true);
        }        
    }
    // Timer end //

    
    // change scene & obj settings according to editor values
    public void MakeChange()
    {
        // fog and ambient light
        GameObject.Find("Directional light").GetComponent<Light>().color = DirectionalLightColor;
        RenderSettings.fog = true;
        RenderSettings.fogColor = FogColor;
        RenderSettings.fogStartDistance = LinearFogStart;
        RenderSettings.fogEndDistance = LinearFogEnd;
        RenderSettings.ambientSkyColor = AmbientLightColor;

        // main camera back ground color
        GameObject.FindWithTag("MainCamera").GetComponent<Camera>().backgroundColor = BgColor;

        // floor vertex color
        //ChangeMeshVertexColor();

        // change floor colors
        ChangeLightColor("Floor1", FloorDyeColor1);
        ChangeLightColor("Floor2", FloorDyeColor2);
        ChangeLightColor("FloorEmissionLight", FloorEmissionColor);

        ChangeLightColor("OutsideWallLight", OutsideWallLightColor);
        ChangeLightColor("WallEmissionLight1", WallEmissionColor1);
        ChangeLightColor("WallEmissionLight2", WallEmissionColor2);
        ChangeLightColor("BigPillarLight", PillarLightColor1);
        ChangeLightColor("SmallPillarLight", PillarLightColor2);

        ChangeLightColor("PropEmissionColor1", PropEmissionColor1);
        ChangeLightColor("PropEmissionColor2", PropEmissionColor2);
        ChangeLightColor("GateEmissionColor1", GateEmissionColor1);

    }


    // change lights' color according to given tag name
    void ChangeLightColor(string tag, Color col)
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(tag);

        for (int i = 0; i < gos.Length; i++)
        {
            Light light = gos[i].GetComponent<Light>();
            MeshRenderer renderer = gos[i].GetComponent<MeshRenderer>();
            if (light != null)
            {
                light.color = col;
            }
            else if (renderer != null)
            {
                Color curColor = renderer.sharedMaterial.GetColor("_EmissionColor");
                if (curColor != col)
                {
                    renderer.sharedMaterial.SetColor("_EmissionColor", col);
                }
                else
                    continue;
            }
        }
    }



    // public struct LightingPalette
    //{
    //    public Color DirectionalLightColor = Color.white;
    //    public Color FogColor = Color.white;
    //    public float LinearFogStart = 20;
    //    public float LinearFogEnd = 50;
    //    public Color AmbientLightColor = Color.white;

    //    public Color BgColor = Color.white;

    //    //public Color FloorDiffuseColor;
    //    public Color FloorDyeColor1 = Color.white;
    //    public Color FloorDyeColor2 = Color.white;
    //    //public Color FloorReflectionColor;
    //    public Color FloorEmissionColor = Color.white;

    //    public Color OutsideWallLightColor = Color.white;
    //    public Color PillarLightColor1 = Color.white;
    //    public Color PillarLightColor2 = Color.white;
    //    public Color WallEmissionColor1 = Color.white;
    //    public Color WallEmissionColor2 = Color.white;
    //    public Color PropEmissionColor1 = Color.white;
    //    public Color PropEmissionColor2 = Color.white;
    //    public Color GateEmissionColor1 = Color.white;
    //}

    // class or struct? 
    // structs are value types so when you access a list element you will in fact access an intermediate copy of the element which has been returned by the indexer of the list.

    public class LightingPalette 
    {
        public Color DirectionalLightColor;
        public Color FogColor;
        public float LinearFogStart;
        public float LinearFogEnd;
        public Color AmbientLightColor;

        public Color BgColor;

        //public Color FloorDiffuseColor;
        public Color FloorDyeColor1;
        public Color FloorDyeColor2;
        //public Color FloorReflectionColor;
        public Color FloorEmissionColor;

        public Color OutsideWallLightColor;
        public Color PillarLightColor1;
        public Color PillarLightColor2;
        public Color WallEmissionColor1;
        public Color WallEmissionColor2;
        public Color PropEmissionColor1;
        public Color PropEmissionColor2;
        public Color GateEmissionColor1;
    }

    //[HideInInspector]
    List<LightingPalette> PaletteList = new List<LightingPalette>();

    void InitializePaletteList()
    {
        if (PaletteList.Count < 9) // only recreate the list, if there is no enough elemets in the list
        {
            PaletteList.Clear();
            Debug.Log("!!!!!! InitializePaletteList: PaletteList " + PaletteList.Count);
            for (int i = 0; i < 9; i++)
            {
                LightingPalette currentPalette = new LightingPalette();
                PaletteList.Add(currentPalette);
            }
        }
    }

    public void SavePalette(int index)
    {

        PaletteList[index].DirectionalLightColor = DirectionalLightColor;
        PaletteList[index].FogColor = FogColor;
        PaletteList[index].LinearFogStart = LinearFogStart;
        PaletteList[index].LinearFogEnd = LinearFogEnd;
        PaletteList[index].AmbientLightColor = AmbientLightColor;

        PaletteList[index].BgColor = BgColor;

        //PaletteList[index].FloorDiffuseColor =      FloorDiffuseColor;
        PaletteList[index].FloorDyeColor1 = FloorDyeColor1;
        PaletteList[index].FloorDyeColor2 = FloorDyeColor2;
        //PaletteList[index].FloorReflectionColor =   FloorReflectionColor;
        PaletteList[index].FloorEmissionColor = FloorEmissionColor;

        PaletteList[index].OutsideWallLightColor = OutsideWallLightColor;
        PaletteList[index].PillarLightColor1 = PillarLightColor1;
        PaletteList[index].PillarLightColor2 = PillarLightColor2;
        PaletteList[index].WallEmissionColor1 = WallEmissionColor1;
        PaletteList[index].WallEmissionColor2 = WallEmissionColor2;
        PaletteList[index].PropEmissionColor1 = PropEmissionColor1;
        PaletteList[index].PropEmissionColor2 = PropEmissionColor2;
        PaletteList[index].GateEmissionColor1 = GateEmissionColor1;

        //Debug.Log("!!!!!!!!! Palette newly saved");
    }
    

    // get palette
    public void RestoreFromChosenPalette(int index)
    {
        DirectionalLightColor = PaletteList[index].DirectionalLightColor;
        FogColor = PaletteList[index].FogColor;
        LinearFogStart = PaletteList[index].LinearFogStart;
        LinearFogEnd = PaletteList[index].LinearFogEnd;
        AmbientLightColor = PaletteList[index].AmbientLightColor;

        BgColor = PaletteList[index].BgColor;

        //FloorDiffuseColor =       PaletteList[index].FloorDiffuseColor;
        FloorDyeColor1 = PaletteList[index].FloorDyeColor1;
        FloorDyeColor2 = PaletteList[index].FloorDyeColor2;
        //FloorReflectionColor =    PaletteList[index].FloorReflectionColor;
        FloorEmissionColor = PaletteList[index].FloorEmissionColor;

        OutsideWallLightColor = PaletteList[index].OutsideWallLightColor;
        PillarLightColor1 = PaletteList[index].PillarLightColor1;
        PillarLightColor2 = PaletteList[index].PillarLightColor2;
        WallEmissionColor1 = PaletteList[index].WallEmissionColor1;
        WallEmissionColor2 = PaletteList[index].WallEmissionColor2;
        PropEmissionColor1 = PaletteList[index].PropEmissionColor1;
        PropEmissionColor2 = PaletteList[index].PropEmissionColor2;
        GateEmissionColor1 = PaletteList[index].GateEmissionColor1;
    }

    /// <summary>
    /// write and read data files
    /// </summary>

    // save scene lighting data to a file
    public void SaveToFile()
    {
        //Write some text to the file
        string path = "Assets/Resources/ScenePalette.txt";
        // Create an instance of StreamWriter to write text to a file.
        using (StreamWriter stWriter = new StreamWriter(path, !File.Exists(path))) //false = remove all previous content, true = append
        {
            // head
            stWriter.Write("Date: ");
            stWriter.WriteLine(System.DateTime.Now);
            stWriter.WriteLine("-------------------");

            for (int i = 0; i < 9; i++)
            {
                stWriter.Write(i.ToString() + "\t");
                stWriter.WriteLine(ParseLightingPaletteToString(i));
            }
            stWriter.Close();
        }

        Debug.Log("!!!!!!!!! Palette write to file done !!!!!!!");
    }


    // Get lighting data from file
    void GetLightingFromFile()
    {
        if (File.Exists("Assets/Resources/ScenePalette.txt"))
        {
            ReadFromFile();
        }
        else
        {
            StartShowErrorMessage();
        }
    }

    // read scene lighting data from a file
    void ReadFromFile()
    {
        string path = "Assets/Resources/ScenePalette.txt";
        StreamReader reader = new StreamReader(path);
        string text = reader.ReadToEnd();
        string[] paletteText = text.Split("\n"[0]);

        PaletteList.Clear(); // clear PaletteList first
        for (int i = 2; i < 11; i++) // how many lines(should be 9), each line = 1 palette
        {
            LightingPalette currentPalette = new LightingPalette();
            string[] paletteData = (paletteText[i].Trim()).Split('\t');
            PaletteList.Add(ParseDataToLightingPalette(currentPalette, paletteData));
        }

        Debug.Log("!!!!!!!!!!! Get lighting data from file done !!!!!!!!!!!!!!");

    }

    string ParseLightingPaletteToString(int index)
    {
        string str = ParseColorToString(PaletteList[index].DirectionalLightColor) + "\t" +
              ParseColorToString(PaletteList[index].FogColor) + "\t" +
              PaletteList[index].LinearFogStart.ToString("0.0") + "\t" +
              PaletteList[index].LinearFogEnd.ToString("0.0") + "\t" +
              ParseColorToString(PaletteList[index].AmbientLightColor) + "\t" +

              ParseColorToString(PaletteList[index].BgColor) + "\t" +

              ParseColorToString(PaletteList[index].FloorDyeColor1) + "\t" +
              ParseColorToString(PaletteList[index].FloorDyeColor2) + "\t" +
              ParseColorToString(PaletteList[index].FloorEmissionColor) + "\t" +

              ParseColorToString(PaletteList[index].OutsideWallLightColor) + "\t" +
              ParseColorToString(PaletteList[index].PillarLightColor1) + "\t" +
              ParseColorToString(PaletteList[index].PillarLightColor2) + "\t" +
              ParseColorToString(PaletteList[index].WallEmissionColor1) + "\t" +
              ParseColorToString(PaletteList[index].WallEmissionColor2) + "\t" +
              ParseColorToString(PaletteList[index].PropEmissionColor1) + "\t" +
              ParseColorToString(PaletteList[index].PropEmissionColor2) + "\t" +
              ParseColorToString(PaletteList[index].GateEmissionColor1);

        return str;
    }


    LightingPalette ParseDataToLightingPalette(LightingPalette temp, string[] data)
    {
        int i = 1;
        temp.DirectionalLightColor = ParseStringToColor(data[i++]);
        temp.FogColor = ParseStringToColor(data[i++]);
        temp.LinearFogStart = float.Parse(data[i++]);
        temp.LinearFogEnd = float.Parse(data[i++]);
        temp.AmbientLightColor = ParseStringToColor(data[i++]);

        temp.BgColor = ParseStringToColor(data[i++]);

        //temp.FloorDiffuseColor =      ParseColor(data[i++]);
        temp.FloorDyeColor1 = ParseStringToColor(data[i++]);
        temp.FloorDyeColor2 = ParseStringToColor(data[i++]);
        //temp.FloorReflectionColor =   ParseColor(data[i++]);
        temp.FloorEmissionColor = ParseStringToColor(data[i++]);

        temp.OutsideWallLightColor = ParseStringToColor(data[i++]);
        temp.PillarLightColor1 = ParseStringToColor(data[i++]);
        temp.PillarLightColor2 = ParseStringToColor(data[i++]);
        temp.WallEmissionColor1 = ParseStringToColor(data[i++]);
        temp.WallEmissionColor2 = ParseStringToColor(data[i++]);
        temp.PropEmissionColor1 = ParseStringToColor(data[i++]);
        temp.PropEmissionColor2 = ParseStringToColor(data[i++]);
        temp.GateEmissionColor1 = ParseStringToColor(data[i++]);

        return temp;
    }


    Color ParseStringToColor(string data)
    {
        Color col;

        string[] colorData = (data.Trim()).Split(',');
        col = new Vector4(float.Parse(colorData[0]), float.Parse(colorData[1]), float.Parse(colorData[2]), float.Parse(colorData[3]));

        return col;
    }

    string ParseColorToString(Color col)
    {
        string str = col.r.ToString("0.000") + "," + col.g.ToString("0.000") + "," + col.b.ToString("0.000") + "," + col.a.ToString("0.000");

        return str;
    }

    /// write and read data files end ///
}