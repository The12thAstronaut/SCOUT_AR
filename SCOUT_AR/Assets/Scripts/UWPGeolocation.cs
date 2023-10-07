using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

#if WINDOWS_UWP
using Windows.Devices.Geolocation;
#endif

public class UWPGeolocation {
#if WINDOWS_UWP
    private static uint _desireAccuracyInMetersValue = 0;
    
    public static async void GetLocation(Action<Geoposition> callbackLocationData, Action<string> callbackFailed)
    {
        var accessStatus = await Geolocator.RequestAccessAsync();
        switch (accessStatus)
        {
            case GeolocationAccessStatus.Allowed:
                // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = _desireAccuracyInMetersValue };

                // Subscribe to StatusChanged event to get updates of location status changes
                geolocator.StatusChanged += OnStatusChanged;

                // Carry out the operation
                Geoposition pos = await geolocator.GetGeopositionAsync();

                callbackLocationData(pos);
                // notify user: Location updated
                break;

            case GeolocationAccessStatus.Denied:
                // notify user: Access to location is denied
                callbackFailed("Access to location is denied");
                break;

            case GeolocationAccessStatus.Unspecified:
                // notify user: Unspecified error
                callbackFailed("Unspecified error");
                break;
        }
    }

    private static void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
    {
        switch (args.Status)
        {
            case PositionStatus.Ready:
                // Location platform is providing valid data.
                break;

            case PositionStatus.Initializing:
                // Location platform is attempting to acquire a fix. 
                break;

            case PositionStatus.NoData:
                // Location platform could not obtain location data.
                break;

            case PositionStatus.Disabled:
                // The permission to access location data is denied by the user or other policies.
                break;

            case PositionStatus.NotInitialized:
                // The location platform is not initialized. This indicates that the application 
                // has not made a request for location data.
                break;

            case PositionStatus.NotAvailable:
                // The location platform is not available on this version of the OS.
                break;
        }
    }
#endif
}