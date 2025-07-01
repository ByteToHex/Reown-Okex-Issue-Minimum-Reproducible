package com.turbolabo.persistentnotification;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.os.IBinder;
import android.os.Handler;
import android.os.Looper;
import android.util.Log;

import androidx.core.app.NotificationCompat;

public class BoxxGameNotificationService extends Service {

    private static final String CHANNEL_ID = "BoxxGameNotificationChannel";
    private static final int NOTIFICATION_ID = 1; // Must be > 0 for startForeground
    private static final String TAG = "BoxxNotificationService";

    // Default values, can be overridden by intent extras
    private String notificationTitle = "Boxx";
    private String notificationDescription = " ";

    private Handler handler;
    private Runnable activityCheckRunnable;
    private static final long CHECK_INTERVAL_MS = 5000; // Check every 5 seconds

    @Override
    public void onCreate() {
        super.onCreate();
        Log.d(TAG, "Service onCreate");

        handler = new Handler(Looper.getMainLooper());
        activityCheckRunnable = new Runnable() {
            @Override
            public void run() {
                // Check if the Unity activity is still active
                if (PersistentNotification.unityActivity == null ||
                        PersistentNotification.unityActivity.isFinishing() ||
                        PersistentNotification.unityActivity.isDestroyed()) {
                    Log.d(TAG, "Unity Activity is no longer active. Stopping service.");
                    stopSelf(); // Stop the service
                } else {
                    // If activity is still active, schedule the next check
                    handler.postDelayed(this, CHECK_INTERVAL_MS);
                }
            }
        };
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        Log.d(TAG, "Service onStartCommand");

        if (intent != null) {
            // Retrieve title and description from the intent if they exist
            notificationTitle = intent.getStringExtra("title") != null ? intent.getStringExtra("title") : notificationTitle;
            notificationDescription = intent.getStringExtra("description") != null ? intent.getStringExtra("description") : notificationDescription;
        }

        // Build the notification to display for the foreground service
        Notification notification = buildNotification(this, notificationTitle, notificationDescription);

        // Start the service in the foreground, associating it with the notification
        startForeground(NOTIFICATION_ID, notification);

        // Start the periodic check for Unity activity status
        handler.postDelayed(activityCheckRunnable, CHECK_INTERVAL_MS);

        // We want this service to continue running until it is explicitly stopped
        return START_STICKY; // Service will be restarted if killed by system
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        Log.d(TAG, "Service onDestroy");
        // Remove any pending callbacks to prevent memory leaks
        if (handler != null && activityCheckRunnable != null) {
            handler.removeCallbacks(activityCheckRunnable);
        }
        // Ensure the notification is removed when the service is stopped
        NotificationManager notificationManager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
        if (notificationManager != null) {
            notificationManager.cancel(NOTIFICATION_ID);
        }
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null; // This service does not provide binding
    }

    /**
     * Builds the Notification object for the foreground service.
     * @param context The context to build the notification.
     * @param title The title for the notification.
     * @param description The description text for the notification.
     * @return The constructed Notification object.
     */
    private Notification buildNotification(Context context, String title, String description) {
        // Create an Intent that will open the main launcher activity of your application
        // when the notification is tapped. This ensures tapping the notification brings
        // the Unity game back to the foreground.
        Intent notificationIntent = context.getPackageManager().getLaunchIntentForPackage(context.getPackageName());
        if (notificationIntent != null) {
            notificationIntent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_SINGLE_TOP);
        } else {
            Log.e(TAG, "Could not get launch intent for package: " + context.getPackageName() + ". Falling back to unityActivity.getClass().");
            // Fallback if getLaunchIntentForPackage fails (less common, but good for robustness)
            // This requires PersistentNotification.unityActivity to be non-null and correctly set.
            if (PersistentNotification.unityActivity != null) {
                notificationIntent = new Intent(context, PersistentNotification.unityActivity.getClass());
                notificationIntent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP | Intent.FLAG_ACTIVITY_SINGLE_TOP);
            } else {
                // If unityActivity is also null, the notification won't be able to launch an activity.
                // This scenario should ideally be prevented by ensuring receiveUnityActivity is called early.
                notificationIntent = new Intent(); // Empty intent if no activity can be launched
            }
        }

        PendingIntent pendingIntent = PendingIntent.getActivity(context, 0, notificationIntent,
                PendingIntent.FLAG_UPDATE_CURRENT | (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M ? PendingIntent.FLAG_IMMUTABLE : 0));

        // Build the notification using NotificationCompat.Builder
        NotificationCompat.Builder builder = new NotificationCompat.Builder(context, CHANNEL_ID)
                .setSmallIcon(android.R.drawable.ic_dialog_info) // IMPORTANT: Replace with your app's actual small icon (e.g., R.drawable.ic_my_app_notification)
                .setContentTitle(title)
                .setContentText(description)
                .setPriority(NotificationCompat.PRIORITY_LOW) // Match channel importance
                .setOngoing(true) // Crucial for ongoing notification associated with foreground service
                .setAutoCancel(false) // Notification persists after tap
                .setContentIntent(pendingIntent);
        Log.i("BoxxGameNotification", "Building notification.");
        return builder.build();
    }
}