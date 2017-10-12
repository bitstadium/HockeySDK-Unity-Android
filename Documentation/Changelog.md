## Changelog

### 5.0.0

Upgrade to HockeySDK for Android 5.0.2.

This release comes with one major breaking change. HockeySDK 5.0.0 raises the minimum API level to 15.
In addition, we no longer support restricting builds by device id. The reason is that, with Android O, `ANDROID_ID` no longer ensures a consistent way of identification of a user.

To be ready for Android O, HockeySDK-Android now limits the `WRITE_EXTERNAL_STORAGE` permission with the `maxSdkVersion` filter. In some use cases, e.g. where an app contains a dependency that requires this permission, `maxSdkVersion` makes it impossible for those dependencies to grant or request the permission. The solution for those cases is to declare the `tools:node="replace"` manifest merging strategy later in the dependency tree:

```<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" tools:node="replace"/>```

#### Full changelog
 
In addition, this release contains the following changes:

* [IMPROVEMENT] Support for Android O.
* [IMPROVEMENT] Code scans no longer trigger warnings related to usage of `ANDROID_ID` as we are no longer using it.
* [IMPROVEMENT] The SDK supports Android Strict Mode way better as it no longer violates it. 
* [IMPROVEMENT] We've improved the way we send Feedback attachments.
* [IMPROVEMENT] The SDK no longer caches information about in-app updates to make sure updates are available immediately. The iOS SDK has behaved like this for a while and we decided to align the behavior across SDKs.
* [IMPROVEMENT] Add the ability to when to show the UpdateFragment [#280](https://github.com/bitstadium/HockeySDK-Android/issues/280).
* [IMPROVEMENT] Retrieving the last crash details is now asynchronous.
* [BUGFIX] Metrics no longer leaks a connection.
* [BUGFIX] It's no longer possible to circumvent the login UI by pressing the backbutton under certain circumstances [#278](https://github.com/bitstadium/HockeySDK-Android/pull/278).
* [BUGFIX] Fix a crash in MetricsManager [#279](https://github.com/bitstadium/HockeySDK-Android/pull/279).
* [BUGFIX] Fix authentication by email [#288](https://github.com/bitstadium/HockeySDK-Android/pull/288).
* [BUGFIX] Fix a regression that was introduced in 5.0.0-beta.1 that prevented attaching screenshots to work [#289](https://github.com/bitstadium/HockeySDK-Android/pull/289).
* [BUGFIX] Fix Feedback notifications on Android [#290](https://github.com/bitstadium/HockeySDK-Android/pull/290).
* [IMPROVEMENT] Add the ability to when to show the UpdateFragment [#280](https://github.com/bitstadium/HockeySDK-Android/issues/280).
* [IMPROVEMENT] `CrashManagerListener` now has `onNoCrashesFound()` to notify you in case no new crashes were found  [#280](https://github.com/bitstadium/HockeySDK-Android/issues/280).
* [DEPRECATION] We've removed the `onCrashesFound` callback in `CrashManagerListener` as it has been deprecated since HockeySDK 3.0.0.
* [Bugfix] Fixes a NPE in `FeedbackActivity`. [#303](https://github.com/bitstadium/HockeySDK-Android/pull/303)
* [Bugfix] Fixes a potential deadlock in `CrashManager`.[#https://github.com/bitstadium/HockeySDK-Android/pull/308]
* [Improvement] Fix potential NPE when calling `MetricsManager.sessionTrackingEnabled()` before calling `MetricsManager.register(...)`. [#310](https://github.com/bitstadium/HockeySDK-Android/pull/310)
* * [Bugfix] Fix a bug in the Italian translation. [#296](https://github.com/bitstadium/HockeySDK-Android/pull/296)
* [Improvement] Use different timestamp format for crash date and app start time. [#297](https://github.com/bitstadium/HockeySDK-Android/pull/297)

### 1.1.6

Upgrade to HockeySDK for Android 4.1.5

* [FIX] Fix a resource leak in Sender.
* [FIX] Fix possibility to bypass authentication.
* [FIX] Fix the progress bar when sending feedback.
* [FIX] Fix the dates in the Feedback UI.
* [FIX] Fix ConcurrentModificationException in metrics feature.
* [FIX] Fix potential crash related to multi-threading in Channel.
* [FIX] Fix the focus in the Feedback UI, this also improves accessibility.
* [IMPROVEMENT] Fix some strict mode violations.
* [IMPROVEMENT] Improve accessibility for Feedback attachments.
* [IMPROVEMENT] Send batched events when the app goes into background.
* [IMPROVEMENT] Automatically add the sdk to the often already existent Plugins folder 

### 1.1.5

* [FIX] Fixes session tracking by explicitly starting one.

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
