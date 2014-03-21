## Introduction

The HockeyAppUnity-Android plugin implements support for using HockeyApp in your Unity3D-Android builds. It easily lets you keep track of crashes that have been caused by your scripts or native Java code.

## Installation & Setup

The following steps illustrate how to integrate the HockeyAppUnity-Android plugin:

1. Copy the **Plugins** folder into the **Assets** directory of your Unity3D project.

2. Create an empty game object an add the **HockeyAppAndroid.cs** as one of its components.

3. Select the game object in the **Hierachy** pane and fill in the App ID provided by HockeyApp (Inspector window). You will also have to enter a package identifier: Make sure that **Package ID** equals the package name of your HockeyApp app.  If you want to get more precise information about exceptions in your Unity3D scripts you can also check the **Exception Logging** property.

4. You are now ready to build the project: Select **File -> Build Settings...** and switch to **Android** in the platform section. Check **Development Build** and **Script Debugging**.

5. That's it: Build your app / Android project as usual.