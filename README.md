# UnityEditorTimeTracker
This editor extension in Unity tracks your current session time as well as the total time you have spent in Unity.
The tracked time is saved in a JSON file called EditorTimeLog.json which is saved in the Root folder of your Project.


## Authors

- [@JostSchienke](https://github.com/JostSchienke)


## Unity Version

#### Tested Unity version

```
Here are all Unity versions that are currently tested: 
```

| Version | Tested     | Works                |
| :-------- | :------- | :------------------------- |
| `Unity 6000.0.5f1`| `TRUE` | `TRUE` |
| `Unity 2022.3.12f1` | `TRUE` | `TRUE` |
| `Unity 2021.3.25f1` | `TRUE` | `TRUE` |

## Installation
- Download the newest version and check if your current version of Unity works with the Time Tracker.
- Import the Time Tracker using Unitys Package Importer, from here the Programm will do the Rest.
- The Time Tracker will create a JSON file in the root folder of your Unity Project and log your times in there. 

## Current Bugs
- Using Unity Network for Gameobjects or the Quantum console resets the Timer when Building the Games

## Fixed Bugs
- On every Reload of the Editor, the Current Editor gets Resetet and starts at 0. see: [Issue 1](https://github.com/JostSchienke/UnityEditorTimeTracker/issues/1)
