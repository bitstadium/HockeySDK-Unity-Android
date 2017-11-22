package net.hockeyapp.unity;

import android.app.Activity;

import net.hockeyapp.android.Constants;
import net.hockeyapp.android.CrashManager;
import net.hockeyapp.android.CrashManagerListener;
import net.hockeyapp.android.FeedbackManager;
import net.hockeyapp.android.LoginManager;
import net.hockeyapp.android.UpdateManager;
import net.hockeyapp.android.metrics.MetricsManager;
import net.hockeyapp.android.utils.HockeyLog;

import java.lang.reflect.Method;
import java.util.concurrent.ExecutionException;

@SuppressWarnings({"unused", "WeakerAccess"})
public class HockeyUnityPlugin {

    //region CONFIGURE AND START MODULES
    //---------------------------------------------------------------------------------------

    /**
     * Enables crash reporting, feedback, user metrics, login, and app updates.
     *
     * @param currentActivity      the context needed for starting this manager.
     * @param serverURL            the URL of the HockeyApp instance.
     * @param appID                the app identifier of your app.
     * @param secret               the app secret of your app used for authentication.
     * @param loginMode            the login mode used for authentication.
     * @param updateManagerEnabled if true, the update manager is enabled.
     * @param userMetricsEnabled   if true, the metrics manager is enabled.
     * @param autoSendEnabled      if true, crashes will be sent without presenting a confirmation dialog.
     */
    public static void startHockeyAppManager(final Activity currentActivity, final String serverURL,
                                             final String appID, final String secret, final int loginMode,
                                             final boolean updateManagerEnabled, final boolean userMetricsEnabled, final boolean autoSendEnabled) {
        currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (updateManagerEnabled) {
                    registerUpdateManager(currentActivity, serverURL, appID);
                }
                if (userMetricsEnabled) {
                    registerMetricsManager(currentActivity, appID);
                }
                registerCrashManager(currentActivity, serverURL, appID, autoSendEnabled);
                registerFeedbackManager(currentActivity, serverURL, appID);
                registerLoginManager(currentActivity, serverURL, appID, secret, loginMode);
            }
        });
    }

    /**
     * Configures and starts the UpdateManager module.
     *
     * @param currentActivity the context needed for starting this manager.
     * @param serverURL       the URL of the HockeyApp instance.
     * @param appID           the app identifier of your app.
     */
    public static void registerUpdateManager(final Activity currentActivity, final String serverURL, final String appID) {
        currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                UpdateManager.register(currentActivity, serverURL, appID, null, true);
            }
        });
    }

    /**
     * Configures and starts the crash reporting module.
     *
     * @param currentActivity the context needed for starting this manager.
     * @param serverURL       the URL of the HockeyApp instance.
     * @param appID           the app identifier of your app.
     * @param autoSendEnabled if true, crashes will be sent without presenting a confirmation dialog.
     */
    public static void registerCrashManager(final Activity currentActivity, final String serverURL,
                                            final String appID, final boolean autoSendEnabled) {
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

    /**
     * Configures the login module.
     *
     * @param currentActivity the context needed for starting this manager.
     * @param serverURL       the URL of the HockeyApp instance.
     * @param appID           the app identifier of your app.
     * @param secret          the URL of the HockeyApp instance.
     * @param loginMode       the app identifier of your app.
     */
    public static void registerLoginManager(final Activity currentActivity, final String serverURL,
                                            final String appID, final String secret, final int loginMode) {
        currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                LoginManager.register(currentActivity, appID, secret, serverURL, loginMode, currentActivity.getClass());
            }
        });
    }

    /**
     * Starts the login module.
     *
     * @param currentActivity the context needed for starting this manager.
     */
    public static void performAuthentication(final Activity currentActivity) {
        currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                LoginManager.verifyLogin(currentActivity, currentActivity.getIntent());
            }
        });
    }

    /**
     * Configures and starts the feedback module.
     *
     * @param currentActivity the context needed for starting this manager.
     * @param serverURL       the URL of the HockeyApp instance.
     * @param appID           the app identifier of your app.
     */
    public static void registerFeedbackManager(final Activity currentActivity, final String serverURL, final String appID) {
        currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                FeedbackManager.register(currentActivity, serverURL, appID, null);
            }
        });
    }

    /**
     * Configures and starts the metrics module.
     *
     * @param currentActivity the context needed for starting this manager.
     * @param appID           the app identifier of your app.
     */
    public static void registerMetricsManager(final Activity currentActivity, final String appID) {
        currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                MetricsManager.register(currentActivity.getApplication(), appID);

                // Unity's awake calls after android activity shown.
                // We force start session to avoid missing it.
                try {
                    Method getInstance = MetricsManager.class.getDeclaredMethod("getInstance");
                    getInstance.setAccessible(true);
                    MetricsManager instance = (MetricsManager) getInstance.invoke(null);
                    Method updateSession = MetricsManager.class.getDeclaredMethod("updateSession");
                    updateSession.setAccessible(true);
                    updateSession.invoke(instance);
                } catch (Throwable ignored) {
                }
            }
        });
    }

    /**
     * Enables crash reporting, feedback, and app updates. If you don't want to enable all features
     * or if you need more options to configure them, use specific register methods instead.
     *
     * @param currentActivity the context needed for starting this manager.
     * @param serverURL       the URL of the HockeyApp instance.
     * @param appID           the app identifier of your app.
     */
    public static void registerAll(final Activity currentActivity, final String serverURL,
                                   final String appID) {
        currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                registerUpdateManager(currentActivity, serverURL, appID);
                registerCrashManager(currentActivity, serverURL, appID, true);
                registerFeedbackManager(currentActivity, serverURL, appID);
                registerMetricsManager(currentActivity, appID);
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
     * @return the device's model manufacturer name.
     */
    public static String getManufacturer() {
        return Constants.PHONE_MANUFACTURER;
    }

    /**
     * @return the device's model name.
     */
    public static String getModel() {
        return Constants.PHONE_MODEL;
    }

    /**
     * @return the unique identifier for device, not dependent on package or device.
     */
    public static String getDeviceIdentifier() {
        try {
            return Constants.getDeviceIdentifier().get();
        } catch (InterruptedException | ExecutionException e) {
            HockeyLog.error("Couldn't get device identifier", e);
            return null;
        }
    }

    //---------------------------------------------------------------------------------------
    //endregion

    //region FEEDBACK MANAGER
    //---------------------------------------------------------------------------------------

    /**
     * Shows a feedback form. This should be called after {@link HockeyUnityPlugin#registerFeedbackManager(Activity, String, String)}.
     *
     * @param currentActivity the context needed for starting this manager.
     */
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

    //region UPDATE MANAGER
    //---------------------------------------------------------------------------------------

    /**
     * Checks for version update and presents update alert if newer version is available.
     *
     * @param currentActivity the context needed to show update alert.
     * @param serverURL       the URL of the HockeyApp instance.
     * @param appID           the app identifier of your app.
     */
    public static void checkForUpdate(final Activity currentActivity, final String serverURL, final String appID) {
        UpdateManager.unregister();
        registerUpdateManager(currentActivity, serverURL, appID);
    }
    //---------------------------------------------------------------------------------------
    //endregion
}
