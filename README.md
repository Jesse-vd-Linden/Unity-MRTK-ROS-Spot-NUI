# Unity-MRTK-ROS-Spot-NUI
This Unity implementation allows you to control a robot with the Microsoft HoloLens 2.
This is done by creating a natural user interface using speech, gestures and gaze.

Communicate with you ROS node that controls the robot and customize the messages that are being send.

## Version control and packages
The versions and packages that are used are explained here to make sure reproduction is possible.

Unity Version: 2021.3.28f1

Unity Packages
 * [Mixed Reality Toolkit](https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk2/?view=mrtkunity-2022-05)
    * Version: 2.8.3.0
 * [ROS-TCP-Connector](https://github.com/Unity-Technologies/ROS-TCP-Connector)



## MRTK Packages
* Mixed Reality OpenXR Plugin
* Mixed Reality Toolkit Examples
* Mixed Reality Toolkit Extensions
* Mixed Reality Toolkit Foundation
* Mixed Reality Toolkit Standard Assest
* Mixed Reality Toolkit Tools

## Troubleshoot
- When importing the Unity project, the following error happens:
`Problem detected while importing the Prefab file: 'Packages/com.microsoft.mixedreality.toolkit.foundation/Providers/Oculus/XRSDK/MRTK-Quest/Prefabs/MRTK-Quest_LocalAvatar.prefab'.`
Solution: In the `Project` window, navigate to `Packages/com.microsoft.mixedreality.toolkit.foundation/Providers/Oculus/XRSDK/MRTK-Quest`, right click and select `Reimport`.