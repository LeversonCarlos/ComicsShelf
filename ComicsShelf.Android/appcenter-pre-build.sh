#!/usr/bin/env bash
echo "APPLY PRODUCTION VALUES ON MANIFEST FILES"

# VALIDATE
if [ ! -n "$ComicsShelfApplicationID" ]
then
    echo "You need define the ComicsShelfApplicationID variable in App Center"
    exit
fi

echo "versionBuild:"
printf -v versionBuild "%03d" $APPCENTER_BUILD_ID
echo $versionBuild

ANDROID_MANIFEST_FILE=$APPCENTER_SOURCE_DIRECTORY/ComicsShelf.Android/Properties/AndroidManifest.xml
if [ -e "$ANDROID_MANIFEST_FILE" ]
then
    sed -i 's/{YOUR_MICROSOFT_APPLICATION_ID}/'$ComicsShelfApplicationID'/' $ANDROID_MANIFEST_FILE
    sed -i 's/versionCode="[0-9.]*/&'$versionBuild'/' $ANDROID_MANIFEST_FILE
    sed -i 's/versionName="[0-9.]*/&.'$versionBuild'/' $ANDROID_MANIFEST_FILE
fi
echo "ANDROID_MANIFEST_FILE:"
echo $ANDROID_MANIFEST_FILE
echo "ANDROID_MANIFEST_FILE CONTENT:"
cat $ANDROID_MANIFEST_FILE

INFO_PLIST_FILE=$APPCENTER_SOURCE_DIRECTORY/ComicsShelf.iOS/Info.plist
if [ -e "$INFO_PLIST_FILE" ]
then
   plutil -replace CFBundleURLTypes.0.CFBundleURLSchemes.0 -string 'msal'$ComicsShelfApplicationID $INFO_PLIST_FILE
fi
echo "INFO_PLIST_FILE CONTENT:"
cat $INFO_PLIST_FILE
