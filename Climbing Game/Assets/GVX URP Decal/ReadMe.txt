1- Create your own material
2- From the shader menu, select "GVX", then "ScreenSpaceDecal_URP"
3- customize your material as perfered
4- attach the material to a cube.

PS: You will need to enable the depth texture in the universal render pipeline asset. it's not enabled by default for the game view and builds. 


If you need help, Join the discord server I have for my assets :)
https://discord.gg/9ffUPaB


Tips:
- This shader doesn't support a layering system at the moment. but you can add an ExcludeFromGDecal component to objects that you want to ignore.
- When MSAA is turned off, unity decreases the refresh rate of the depth texture which may cause the decals to look wobbly in some versions.