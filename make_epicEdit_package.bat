MKDIR Builds
MKDIR Builds\EpicEdit
COPY Builds\Debug\EpicEdit.exe Builds\EpicEdit\EpicEdit.exe
COPY Builds\Debug\EpicEdit.exe.config Builds\EpicEdit\
COPY Builds\Debug\*.dll Builds\EpicEdit\
MKDIR Builds\EpicEdit\Data
COPY OutbreakData\Shaders\defaultAmbient.fx Builds\EpicEdit\Data\
COPY OutbreakData\testguiskin.adf Builds\EpicEdit\Data\
COPY OutbreakData\Textures\noTexture.png Builds\EpicEdit\Data\
COPY OutbreakData\Textures\gui*.* Builds\EpicEdit\Data\
COPY OutbreakData\Textures\modelEditor\*.* Builds\EpicEdit\Data\
COPY OutbreakData\UI\modelEditor\*.* Builds\EpicEdit\Data\
