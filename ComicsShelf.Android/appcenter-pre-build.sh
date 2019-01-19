#!/usr/bin/env bash
#
# For Xamarin Android or iOS, change the applicationID located in AndroidManifest.xml and Info.plist. 
# AN IMPORTANT THING: YOU NEED DECLARE ComicsShelfApplicationID ENVIRONMENT VARIABLE IN APP CENTER BUILD CONFIGURATION.
echo "APPLY PRODUCTION VALUES ON MANIFEST FILES"

if [ ! -n "$ComicsShelfApplicationID" ]
then
    echo "You need define the ComicsShelfApplicationID variable in App Center"
    exit
fi

ANDROID_MANIFEST_FILE=$APPCENTER_SOURCE_DIRECTORY/ComicsShelf.Android/Properties/AndroidManifest.xml
if [ -e "$ANDROID_MANIFEST_FILE" ]
then
    echo "Updating applicationID to $ComicsShelfApplicationID in AndroidManifest.xml"
    sed -i '' 's/{YOUR_MICROSOFT_APPLICATION_ID}/'$ComicsShelfApplicationID'/' $ANDROID_MANIFEST_FILE
fi
echo "ANDROID_MANIFEST_FILE CONTENT:"
cat $ANDROID_MANIFEST_FILE

INFO_PLIST_FILE=$APPCENTER_SOURCE_DIRECTORY/ComicsShelf.iOS/Info.plist
if [ -e "$INFO_PLIST_FILE" ]
then
    echo "Updating applicationID to $ComicsShelfApplicationID in Info.plist"
    plutil -replace CFBundleURLTypes.0.CFBundleURLSchemes.0 -string $ComicsShelfApplicationID $INFO_PLIST_FILE
fi
echo "INFO_PLIST_FILE CONTENT:"
cat $INFO_PLIST_FILE
