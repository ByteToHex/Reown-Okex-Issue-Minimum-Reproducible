package com.turbolabo.persistentnotification;

import android.app.Activity;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Build;
import android.util.Log;

import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

public class PersistentNotification {
    public static Activity unityActivity;
    private static final String CHANNEL_ID = "BoxxGameNotificationChannel";
    private static final int NOTIFICATION_ID = 1;
    private static final String TAG = "PersistentNotification";
    private static final int NOTIFICATION_PERMISSION_REQUEST_CODE = 1001;

    // This method is called by Unity to pass its Activity context to the plugin.
    public static void receiveUnityActivity(Activity tActivity) {
        unityActivity = tActivity;
        if (unityActivity == null) {
            Log.e(TAG, "receiveUnityActivity: Unity Activity is null.");
            return;
        }
        Log.d(TAG, "receiveUnityActivity: Unity Activity received.");

        // Check if the Android version is O or higher
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            CharSequence name = "Boxx Game Notifications";
            String description = "Persistent notification for Boxx game status.";
            int importance = NotificationManager.IMPORTANCE_LOW;
            NotificationChannel channel = new NotificationChannel(CHANNEL_ID, name, importance);
            channel.setDescription(description);
            NotificationManager notificationManager = unityActivity.getSystemService(NotificationManager.class);
            if (notificationManager != null) {
                notificationManager.createNotificationChannel(channel);
                Log.d(TAG, "Notification channel created/updated.");
            } else {
                Log.e(TAG, "Failed to get NotificationManager service for channel creation.");
            }
        }
    }

    /**
     * Requests the POST_NOTIFICATIONS permission from the user for Android 13+.
     * This method should be called early in the app lifecycle.
     */
    public void requestNotificationPermission() {
        if (unityActivity == null) {
            Log.e(TAG, "requestNotificationPermission: Unity Activity is null. Cannot request permission.");
            return;
        }

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) { // API 33+
            if (ContextCompat.checkSelfPermission(unityActivity, android.Manifest.permission.POST_NOTIFICATIONS) != PackageManager.PERMISSION_GRANTED) {
                Log.w(TAG, "POST_NOTIFICATIONS permission not granted. Requesting permission.");
                ActivityCompat.requestPermissions(unityActivity,
                        new String[]{android.Manifest.permission.POST_NOTIFICATIONS},
                        NOTIFICATION_PERMISSION_REQUEST_CODE);
            } else {
                Log.d(TAG, "POST_NOTIFICATIONS permission already granted.");
            }
        } else {
            Log.d(TAG, "POST_NOTIFICATIONS permission not required for this Android version (< API 33).");
        }
    }

    // Displays a persistent notification by starting a Foreground Service.
    // This method now assumes permission has been requested (and ideally granted).
    public void showPersistentNotification(String title, String description) {
        if (unityActivity == null) {
            Log.e(TAG, "showPersistentNotification: Unity Activity is null. Cannot show notification.");
            return;
        }

        // Re-check permission here before showing notification, in case it was denied after request
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
            if (ContextCompat.checkSelfPermission(unityActivity, android.Manifest.permission.POST_NOTIFICATIONS) != PackageManager.PERMISSION_GRANTED) {
                Log.e(TAG, "POST_NOTIFICATIONS permission not granted. Cannot show notification. Call requestNotificationPermission first.");
                return; // Do not proceed if permission is not granted on API 33+
            }
        }

        Log.d(TAG, "POST_NOTIFICATIONS permission granted or not required. Attempting to start service.");

        // Create an Intent to start the BoxxGameNotificationService
        Intent serviceIntent = new Intent(unityActivity, BoxxGameNotificationService.class);
        serviceIntent.putExtra("title", title);
        serviceIntent.putExtra("description", description);

        // Start the service in the foreground
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            unityActivity.startForegroundService(serviceIntent);
        } else {
            unityActivity.startService(serviceIntent);
        }
        Log.d(TAG, "Foreground service start command sent for persistent notification.");
    }

    // Hides the persistent notification by stopping the Foreground Service.
    public void hidePersistentNotification() {
        if (unityActivity == null) {
            Log.w(TAG, "hidePersistentNotification: Unity Activity is null. Cannot hide notification.");
            return;
        }
        // Create an Intent to stop the BoxxGameNotificationService
        Intent serviceIntent = new Intent(unityActivity, BoxxGameNotificationService.class);
        unityActivity.stopService(serviceIntent);
        Log.d(TAG, "Foreground service stop command sent.");
    }

    /**
     * Handles the result of permission requests. This method should be called from the
     * main Activity's onRequestPermissionsResult.
     * @param requestCode The request code passed in ActivityCompat.requestPermissions.
     * @param permissions The requested permissions.
     * @param grantResults The grant results for the corresponding permissions.
     */
    public static void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        if (requestCode == NOTIFICATION_PERMISSION_REQUEST_CODE) {
            if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                Log.d(TAG, "POST_NOTIFICATIONS permission granted!");
                // You could send a message back to Unity here if needed:
                // UnityPlayer.UnitySendMessage("YourGameObject", "OnNotificationPermissionResult", "GRANTED");
            } else {
                Log.w(TAG, "POST_NOTIFICATIONS permission denied.");
                // You could send a message back to Unity here if needed:
                // UnityPlayer.UnitySendMessage("YourGameObject", "OnNotificationPermissionResult", "DENIED");
            }
        }
    }
}
