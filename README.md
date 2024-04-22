# Théo Kokel technical test for Wonder Partner's

## Test goal and result
The test's goal was to create a simple UI that allows to change the rotation of a 3D model to see it under different angles. The UI look and its elements were presented and prototyped in Figma by the company. The particularity of this test is that the material of the 3D model must be created at runtime by downloading its textures from URLs. The created material must use Unity's standard shader.
As precised by the company, another touch of difficulty here is that the model uses the glTF format and its textures the glTF2.0 packing method. This mainly implies a modification of the Metallic-Roughness texture to fit the standard shader.

The reviewing team kindly sent me their notes and debrief about my work:  https://wonderpartners.notion.site/Th-o-KOKEL-855fd33751d7476b97be796046f9c29f, here are some explanations for the cons:
- I was so focused on the material creation (and its non-working problem) that I totally forgot an essential point: the responsiveness of the UI. I worked with only one simulated screen (Galaxy S10e) and forgot to try with other ones, it is totally my fault.
- The missing of some keywords is due to my lack of knowledge regarding the shader system in Unity, I confess that.
- For my defense, the glTF2 documentation is partly wrong in its explanation of the encoding of the Metallic Roughness texture (https://github.com/sbtron/glTF/blob/30de0b365d1566b1bbd8b9c140f9e995d3203226/specification/2.0/README.md#pbrmetallicroughnessmetallicroughnesstexture), because it mentions that the Metallic is in the R channel but according to the provided correction of the test it is in the B channel. The code modification gives a much better result and proves that it is indeed in B.
 
I unfortunately did not get the role, but the team congratulated me for the layer of complexity and the extended possibilities that my architecture allows, saying that this test was generally given to mid-senior candidates and that my work was better than some candidates' from this level. On the other hand, they also reproached me this complexity, as it surely made me lose a lot of time, as well as implementing unwanted features.
The major cons was obviously that I was not able to create the needed material at the end. I should have try more to overcome the docs' incorrectness.

Anyway, as I mention below I really enjoyed this test. I learned a lot about Unity web requests and the shader system. It also gave me the opportunity to challenge my architecture skills.

The following parts of this readme constitute the readme that was given with the Unity project as my final work for the test, left untouched.

---

## Introduction
This test was very enjoyable to work on. I never used Unity's web requests before and I really liked to get to discover them. As it will be explained later, the test is not 100% complete as the material created for the model is not exactly as it should be. But in term of code I a am very satisfied of what I produced. I tried to create a system that is easily expandable, particularly by using scriptable objects. 
The app has been created and tested with the "Galaxy S10e" simulated screen device.

## Global Architecture
First of all, I need to explain some of my naming conventions/habits:
- "Managers" are objects responsible (generally) of a list of objects and supervising a task/behaviour for them.  They are present in the scene, as they need some references, but there can be only one manager of the same type in the scene.
- "Services" are static classes providing utils and helpers methods. They are mainly called from managers.
- A ScriptableObject inherited class has "SO" at the end of its name. Example: ButtonConfigSO.
That said, we can move on a focus of each classes created for this project.

### ModelManager
The ModelManager is the class responsible of the Models, 3D models for which we want to be able to modify their view. It interacts exclusively with the BottomBarManager and the currently displayed model holding a ModelBehaviour component.
It has a list of displayable models (for the test, only the Damaged Helmet is in the list). The model switching is not implemented but could easily be done in a minimum effort.

### ModelBehaviour
The ModelBehaviour is the base component that a model needs to hold. It is responsible of calling the ViewModifier and MaterialCreator methods when it receives the order from the ModelManager. I preferred to do it that way (ModelManager interacting only with ModelBehaviour) to avoid multiple references and GetComponent inside the manager. The manager tells the behaviour to perform an action, and the behaviour uses the correct attached-component to perform it.

### ViewModifier
This component holds the list of the different views we want our model to have. It is also responsible of performing the view modification. 
A view is defined by a button that will be displayed in the BottomBar, a position and rotation to reach, and a bool to tell the system that the position is the starting one.

### MaterialCreator
As its name says, this component is responsible of creating the model's material. When instantiated by the ModelManager, a model receives the order to create its material. This is where this component takes place. It first tries to find the needed textures on disk, in case they have already been downloaded, and orders their download if not.
To download a texture, we call the TextureDownloadService, especially its coroutine. As the service is static and doesn't inherits from MonoBehaviour, it can't start its own coroutine, so we need to start it in the MaterialBehaviour.
We then use an event triggered when the download is complete (success or fail) to keep tracks of the remaining pending downloads and, if they all are completed, create the material and finally show the model.
We also keep a tracks of the failed downloads, in case we want to implement a "Retry download"-like button, or just to throw in-app result notifications.
Once downloaded, the texture is written into a file on disk by the FileIOService. Note that it might make the editor to freeze a little during the file creation. According to further needs, this writing feature could be easily removed.

### BottomBarManager
This manager is responsible for updating the bottom bar UI alongside the model's view. The bottom bar apparition and its buttons' creation is ordered by the ModelManager when the model is ready to be displayed. When created, an Action from the ModelManager is added as a listener to the button's OnClick event. This is how the link between a button and its corresponding view is done. When clicked, the action will be invoked and the ModelManager will ordered the ModelBehaviour to modify its view.

### BottomBarButton
The BottomBarButton is the main element that compose the bottom bar. As described above, they are the UI element that triggers the model's view modification. The button's appearance, especially its Active/Inactive icons and its Active text, are defined in the ButtonConfig ScriptableObject. This allows to create as much buttons as we want and, above all, it makes the UI created at runtime, preventing so to create a custom BottomBar for each model.

### The services
The project contains 3 Services:
- The FileIOService that is responsible for writing, deleting and loading a file on disk. Exclusively used here for textures, but the service is not restrain to images at all.
- The TextureDownloadService that implements the download coroutine used for textures.
- The TexturePackingService used to perform conversion/transformation to textures and especially their canals in case of different packing method between the downloaded texture and the needed one for the material creation.

### The Scriptable Objects
The system has been created to be as expendable as possible. Scriptable Objects have a key role in that purpose. 5 ScriptableObjects have been created:
- The ButtonConfigSO holds the data of the bottom bar buttons. This data consists in both Active/Inactive icons and the text displayed when the button is Active. A button is declared "Active" when its associated view is the currently displayed view.
- The MaterialConfigSO holds all the data needed to create the material of a model. It consists in the textures' descriptions (Type, Format, PackingMethod and URL), the directory to save those textures once downloaded as well as their name's prefix and finally, a ShaderConfigSO to describe the desired shader for this material.
- The ShaderConfigSO that gathers the description of a shader needed for a material. It consists in different properties such as TextureMaps, Keywords or Colors. I decided to create this SO when I discovered that the same property can be named differently according to its shader. For example, the BaseMap texture property is called "_MainTex" for The standard Shader when it is "baseColorTexture" for the glTFPbrMetallicRoughness shader. With this SO, we can bind a common property to its actual name in the corresponding shader.
- The VisibilityConfigSOs holds the DOTween data such as duration and Easing method needed for the Show/Hide animations.

## How To ?
### Display a new model with different views and buttons
The simple was made to be simple, so adding a new model to be interacted with is possible in a few steps:
- Create a prefab with your 3D model and add the ModelBehaviour component as well as the MaterialCreator (only if you need to download your textures) and the ViewModifier.
- In the MaterialCreator (if added), select your desired MaterialConfigSO or create a new one
- In the ViewModifier, add as much views as you want in the list. Be careful that only one view can be declared as the starting one, there is yet no data validation to ensure it. If needed, create new ButtonConfigSO.
- Add your freshly created prefab into the ModelManager's "ModelPrefabs" list.
- TADA ! Your model is ready to be displayed and gets its view changed.
- Note that for now, as there is no model switching feature, your model must be the first element of the list to be displayed.

## Issues
There is one major issue that annoys me with this project : i did not manage to get the targeted result for tha model's material (as a target I take the imported textured model).
I took the information that the packing of the MetallicRoughness texture was not the same and that it needed some conversion/adaptation, which is, according to my researches and the glTF2.0 docs, putting the G canal of the texture into the A canal (MetallicRoughness is coded on RG__ in glTF2.0 while it is on R__A for the Standard Shader).
I presume that my texture adaptation is not the correct one as there are no reflexions on the model while using it. But after some tests, I discovered that there was reflexions while using the "raw" downloaded glTF2.0 texture but those reflexions appear only after tweaking a variable (no matter what variable, even for a non-metallic related one) in the material's inspector.