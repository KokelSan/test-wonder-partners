Readme sections to write:

- Global architecture : scriptableObjects for maximum config possibilities and extensibility

- Material major issue (no reflexions while inverting G and A canals for MetallicRoughness but reflexions while using the glTF2.0 texture ONLY when tweaking whatever variable in inspector...)

- "Animations" with DOTween, but easily replaceable by animator 

- Classes and components focus:
  - ModelManager
  - ModelBehaviour
  - ViewModifier
  - MaterialCreator
  - BottomBarManager
  - BottomBarButton
  - The services
  - The ScriptableObjects

- Flow to add a new model with new textures and different views

- What does the system can do or facilitate that wasn't required ?
  - Buttons extensibility: not restrained to Left-Front-Right buttons (modulo a little work on the canvas/layout group/size of elements)
  - Idem for views that are configurable for each model
  - Models carousel/showroom: multiple models can be registered in the list. Models switching could be implemented in a minimum effort
  - Shader conversion: with shader config scriptable objects, a new shader can easily be configured. A new one will only require to write specific texture transformation code in the TexturePackingService
  - File writing after download & loading on start: useful for debugging what is downloaded and to save download time. A force re-download feature could be easily implemented

- Ideas not implemented:
  - Animated "Downloading textures" text
  - Proper notifications to inform of the download results and progress