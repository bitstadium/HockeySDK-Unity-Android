package net.hockeyapp.unity;

import net.hockeyapp.android.Constants;
import net.hockeyapp.android.CrashManager;
import net.hockeyapp.android.CrashManagerListener;
import net.hockeyapp.android.FeedbackManager;
import net.hockeyapp.android.UpdateManager;
import android.annotation.TargetApi;
import android.app.Activity;
import android.os.Build;

/**
 * <h3>License</h3>
 * 
 * <pre>
 * Copyright (c) 2011-2015 Bit Stadium GmbH
 * 
 * Version 1.0.5
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

public class HockeyUnityPlugin {

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public static void startHockeyAppManager(final String appID,
			final Activity currentActivity, final boolean updateManagerEnabled) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				if (updateManagerEnabled) {
					UpdateManager.register(currentActivity, appID);
				}
				CrashManager.register(currentActivity, appID);
			}
		});
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public static void startHockeyAppManager(final String appID,
			final Activity currentActivity, final boolean updateManagerEnabled,
			final boolean autoSendEnabled) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				if (updateManagerEnabled) {
					UpdateManager.register(currentActivity, appID);
				}
				CrashManager.register(currentActivity, appID,
						new CrashManagerListener() {
							public boolean shouldAutoUploadCrashes() {
								return autoSendEnabled;
							}
						});
			}
		});
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public static void startHockeyAppManager(final Activity currentActivity,
			final String serverURL, final String appID,
			final boolean updateManagerEnabled, final boolean autoSendEnabled) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				if (updateManagerEnabled) {
					UpdateManager.register(currentActivity, serverURL, appID, null);
				}
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
	public static void startFeedbackForm(final String appID,
			final Activity currentActivity) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				FeedbackManager.register(currentActivity, appID);
				FeedbackManager.showFeedbackActivity(currentActivity);
			}
		});
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public static void startFeedbackForm(final Activity currentActivity,
			final String serverURL, final String appID) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				FeedbackManager.register(currentActivity, serverURL, appID, null);
				FeedbackManager.showFeedbackActivity(currentActivity);
			}
		});
	}

	public static String getAppVersion() {

		return Constants.APP_VERSION;
	}

}
