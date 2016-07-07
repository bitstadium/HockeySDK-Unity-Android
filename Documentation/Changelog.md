###Changelog:###

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
