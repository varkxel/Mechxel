# A Custom SRP Dissertation Project
This project focuses on whether imposing a limitation of one colour per voxel will improve performance.
Currently this isn't benchmarked against anything, but it should be promising:
* Rendering and Texturing is all deferred, with only one GBuffer.
* GBuffer layout is currently materialID, then the 3D normal.
