/**
 * <h3>License</h3>
 * 
 * <pre>
 * Copyright (c) 2011-2015 Bit Stadium GmbH
 * 
 * Version 1.1.0
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 * </pre>
 *
 * @author Christoph Wendt
 **/

package net.hockeyapp.unity;

import net.hockeyapp.android.Constants;
import net.hockeyapp.android.CrashManager;
import net.hockeyapp.android.CrashManagerListener;
import net.hockeyapp.android.FeedbackManager;
import net.hockeyapp.android.UpdateManager;
import android.annotation.TargetApi;
import android.app.Activity;
import android.os.Build;

public class HockeyUnityPlugin {

	//region CONFIGURE AND START MODULES
	//---------------------------------------------------------------------------------------
	/**
	 * Enables crash reporting, feedback, and app updates.
	 * 
	 * @param currentActivity			the context needed for starting this manager.
	 * @param serverURL					the URL of the HockeyApp instance.
	 * @param appID						the app identifier of your app.
	 * @param updateManagerEnabled		if true, the update manager is enabled.
	 * @param autoSendEnabled			if true, crashes will be sent without presenting a confirmation dialog.
	 */
	@Deprecated
	@TargetApi(Build.VERSION_CODES.GINGERBREAD)

	public static void startHockeyAppManager(final Activity currentActivity, final String serverURL, 
			final String appID, final boolean updateManagerEnabled, final boolean autoSendEnabled) {
		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				if (updateManagerEnabled) {
					registerUpdateManager(currentActivity, serverURL, appID);
				}
				registerCrashManager(currentActivity, serverURL, appID, autoSendEnabled);
				registerFeedbackManager(currentActivity, serverURL, appID);
			}
		});
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	/**
	 *  Configures and starts the UpdateManager module.
	 * 
	 * @param currentActivity	the context needed for starting this manager.
	 * @param serverURL			the URL of the HockeyApp instance.
	 * @param appID				the app identifier of your app.
	 */
	public static void registerUpdateManager(final Activity currentActivity, final String serverURL, final String appID){
		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				UpdateManager.register(currentActivity, serverURL, appID, null, true);
			}
		});
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	/**
	 * Configures and starts the crash reporting module.
	 * 
	 * @param currentActivity		the context needed for starting this manager.
	 * @param serverURL				the URL of the HockeyApp instance.
	 * @param appID					the app identifier of your app.
	 * @param autoSendEnabled		if true, crashes will be sent without presenting a confirmation dialog.
	 */
	public static void registerCrashManager(final Activity currentActivity, final String serverURL, final String appID,	final boolean autoSendEnabled){
		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				CrashManager.register(currentActivity, serverURL, appID,
						new CrashManagerListener() {
					public boolean shouldAutoUploadCrashes() {
						return autoSendEnabled;
					}
				});
			}
		});
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	/**
	 * Configures and starts the feedback module.
	 * 
	 * @param currentActivity		the context needed for starting this manager.
	 * @param serverURL				the URL of the HockeyApp instance.
	 * @param appID					the app identifier of your app.
	 */
	public static void registerFeedbackManager(final Activity currentActivity, final String serverURL, final String appID){
		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				FeedbackManager.register(currentActivity, serverURL, appID, null);
			}
		});
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	/**
	 * Enables crash reporting, feedback, and app updates. If you don't want to enable all features or if you need more options to configure them, use specific register methods instead.
	 * 
	 * @param currentActivity	the context needed for starting this manager.
	 * @param serverURL			the URL of the HockeyApp instance.
	 * @param appID				the app identifier of your app.
	 */
	public static void registerAll(final Activity currentActivity, final String serverURL, 
			final String appID) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				registerUpdateManager(currentActivity, serverURL, appID);	
				registerCrashManager(currentActivity, serverURL, appID, true);
				registerFeedbackManager(currentActivity, serverURL, appID);
			}
		});
	}
	//---------------------------------------------------------------------------------------
	//endregion
	
	//region METADATA
	//---------------------------------------------------------------------------------------
	
	
	/**
	 * @return the version of your app.
	 */
	public static String getVersionCode() {
		return Constants.APP_VERSION;
	}

	/**
	 * @return the version name of your app
	 */
	public static String getVersionName() {
		return Constants.APP_VERSION_NAME;
	}

	/**
	 * @return the name of the base HockeyApp SDK.
	 */
	public static String getSdkName() {
		return Constants.SDK_NAME;
	}

	/**
	 * @return the version of the base HockeyApp SDK.
	 */
	public static String getSdkVersion() {
		return Constants.SDK_VERSION;
	}
	//---------------------------------------------------------------------------------------
	//endregion

	//region FEEDBACK MANAGER
	//---------------------------------------------------------------------------------------
	@Deprecated
	/**
	 * Shows a feedback form. This should be called after {@link HockeyUnityPlugin#registerFeedbackManager(Activity, String, String)}.
	 * 
	 * @param currentActivity	the context needed for starting this manager.
	 * @param serverURL			the URL of the HockeyApp instance.
	 * @param appID				the app identifier of your app.
	 */
	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public static void startFeedbackForm(final String appID,
			final Activity currentActivity) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				FeedbackManager.showFeedbackActivity(currentActivity);
			}
		});
	}

	/**
	 * Shows a feedback form. This should be called after {@link HockeyUnityPlugin#registerFeedbackManager(Activity, String, String)}.
	 * 
	 * @param currentActivity	the context needed for starting this manager.
	 */
	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public static void startFeedbackForm(final Activity currentActivity) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				FeedbackManager.showFeedbackActivity(currentActivity);
			}
		});
	}
	//---------------------------------------------------------------------------------------
	//endregion
}
