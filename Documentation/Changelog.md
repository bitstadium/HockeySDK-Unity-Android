###Changelog:###

### 1.1.4
Upgrade to HockeySDK for Android 4.1.4

* [IMPROVEMENT] Minor bugfixes
* [IMPROVEMENT] FeedbackActivity now uses accessibility labels.


### 1.1.3
* [IMPROVEMENT] Update to HockeySDK Android version 4.1.3
* [FIX] Thanks to Ivan Matkov, it's no longer possible to avoid providing login information and circumvent authentication. [#208](https://github.com/bitstadium/HockeySDK-Android/pull/208)
* [FIX] Thanks to Guillaume Perrot, the google play store detection was fixed for emulators running Android Nougat. [#209](https://github.com/bitstadium/HockeySDK-Android/pull/209)
* [IMPROVEMENT] It's now possible to scroll within the `FeedbackActivity` while the keyboard is up. Previously, when providing a lot of feedback, the keyboard could hide the submit-button. [#207](https://github.com/bitstadium/HockeySDK-Android/pull/207)
* [IMPROVEMENT] In case the app is offline, the Update feature will no longer log the IOException to avoid confusion. [#209](https://github.com/bitstadium/HockeySDK-Android/pull/209)

### 1.1.2
* [BUGFIX] Installing an app through HockeyApp would falsely report this as a store installation on Android Nougat
* [BUGFIX] Workaround an issue when installing updates and targeting SDK version 24
* [BUGFIX] Added user contributed localizations for Simplified Chinese and Russian
* [UPDATE] Plugin now uses HockeySDK Android 4.1.2

### 1.1.1
* [BUGFIX] Fix bug where report for managed exceptions didn't contain a `CrashReporter Key`. The key is needed to get proper user statistics on the portal
* [UPDATE] Plugin now uses HockeySDK Android 4.0.1

### 1.1.0
* [NEW] User Metrics (user and session tracking)
* [NEW] Trigger version update check explicitly
* [NEW] Authentication
* [BUGFIX] Avoid app crash when first launching app without internet connection
* [UPDATE] Plugin now uses HockeySDK Android 4.0.0
* [UPDATE] Minor bugfixes

### 1.0.8:

- Update SDK to use HockeySDK 3.6.2 for Android
- Fix minor bug

### 1.0.7:

- Update SDK to use HockeySDK 3.6.1 for Android
- Fix minor bug
	
### 1.0.6:

- Append SDK and App information to crash reports
	* SDK name
	* SDK version
	* App version name
