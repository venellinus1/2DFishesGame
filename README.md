# 2D Fishing Game
 
# Game Located at:
https://dfishinggame.web.app

Scene: Assets\Scenes\Tasks

# Network configuration
1. Install Photon PUN for Unity
2. Login to your Photon Dashboard , create new Application and copy its App ID
3. Unity - Editor - Window - Photon Unity Networking - Highlight Server settings:
 - enter your App ID PUN
- Fixed Region: eu 
- Server: ns.photonindustries.io 
- Port: 443
- Protocol: Web Socket Secure

# Scene:
-GameManager- Network Manager - instantiates the Player prefab (FishermanV1)
Network manager is responsible to setup the dependencies for the PlayerControllerV1 (attached to instantiated Prefab). Its using abstractions, thus allowing less coupling between PlayerControllerV1 and its dependencies (wip...).


# Player prefab: Assets\Resources\FishermanV1 prefab
Components of the Player: 
# PlayerControllerV1:
-Move speed - controls the moving speed of the Player 
-LineRenderer - reference to a LineRenderer component, required to draw the fishing line

# PlayerLineDrawing:
-Fish Detection Collider - reference to the box collider of the end of the fishing line- used to detect nearby fishes
-Fish Hook Area - visual representation of the Hook area
-Max Length - lenght of the fishing line
# FishingRTPRNGService 
- provides method TryFishCollecting:
- success rate - minimum guaranteed returns for player (maximum is hardcoded at 10 atm..)

# PlayerUI 
- View, exposes 2 Text fields, used to show on top of player its attempts and success hooks
# AttemptsStatsUI 
- View, exposes 1 Text field, shows last 10 fishes rareness values

# Scripts for the Game are at Assets\Scripts :
# NetworkManager 
- Utilizing Photon PUN for Multiplayer. On start creates the Photon connection, joins room, instantiates the Player prefab
# PlayerController 
- controls Player's movement. Takes the User input for drawing the fishing line and trying to collect nearby fish. Attached to Player prefab together with:
# PlayerLineDrawing 
- service that takes care for drawing the line and network syncing it
# FishingRTPService 
- it provides TryFishCollecting method called by Player. Method decides randomly whether to collect the fish or not, guaranteeing across 10 attempts there will be successRate successfull catches. This is achieved by filling up a List<bool> with successRate amount of "true" entries, then shuffling the entries and storing them into a Queue. 
# FishingRTPRNGService 
- based on FishingRTPService, it uses Queue<int> where the int values represent weights to compare against the current Fish rareness. The weights are filled with guaranteed amount of successRate 5's (5 being the most rare fish ) and rest of the entries are random between 2..5 . Each fishing attempt will compare current weight >  current fish rareness to decide the outcome for the fishing attempt.
The service uses RandomProportional extension to generate random numbers 2..5 not evenly distributed but with decreasing chances proportional to the number value - thus giving the more rare values less chance to show in the Queue. The skew is achieved based on square root function.
# FishMovement 
- takes care of random fish movement within the bounds of the pond, network sync. Fishes are hardcoded on the Scene, with 6 fishes rareness = 5, 3 fishes for each rareness 2,3,4
# PlayerUI, PlayerUIModel 
- MVC architecture, where the model is injected with property injection within PlayerControllerV1 and PlayerUI. 
# AttemptsStatsUI 
- doesnt have dedicated Model (like PlayerUIModel) , it demonstrates different approach of decoupling between PlayerControllerV1 and the AttemptsStatsUI achieved by delegate and event.
