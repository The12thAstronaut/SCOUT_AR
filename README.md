# MRTK3-Testing-2023
 Repository for testing out MRTK3 and prototyping new designs.

This project makes use of NASA LRO LOLA height data found at http://imbrium.mit.edu/BROWSE/LOLA_GDR/CYLINDRICAL/ELEVATION.

This repository does not include high-resolution moon height maps due to their total combined size of 2 GB. Using the project without them is not recommended and will cause errors.
Access and download the files here: bit.ly/moon-height-maps
After downloading the files, import them into Assets/Data/Height Maps, then inside Unity's import settings for each file, set max size to 16384 and compression to none.
Then, find the MoonTerrainMap material and set the Color-#-# textures to the matching terrain files.
