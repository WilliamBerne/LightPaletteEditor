# LightPaletteEditor

This editor is used in Unity3D, to control diffuse, emission, fog, and light, so that the artists can design scenes more easily. 

There are 9 palettes, each can be saved or loaded at any time. They are stored in a txt file, this also makes it easier to transfer data between co-workers. 

With the same 3D assets, the artist can edit different themes by just recoloring the scene in the game.

In this version, I used tags to define the lights and objects which will be controlled by this editor. If needed, I use other methods to distinguish different objects.

How to use:

1.Put LightPaletteEditor.cs under ProjectName\Assets\Editor.
2.Refresh Unity editor UI.
3.Open the Window pulldown menu, find Light Palette Editor, run it.
4.   DirectionalLightColor
    AmbientLightColor;
    FogColor
    LinearFogStar
    LinearFogEnd

    BgColor
    BloomIntesity
    
    FloorDiffuseColor
    FloorDyeColor1
     FloorDyeColor2
 FloorReflectionColor
    FloorEmissionColor

    OutsideWallLightColor
    PillarLightColor1
    PillarLightColor2
     WallEmissionColor1 
     WallEmissionColor2

     PropEmissionColor1 
    PropEmissionColor2
    GateEmissionColor1
