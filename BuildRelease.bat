del "KoikatuPlugins.zip"
echo F|xcopy "README.md" "PackingFolder\README.md" /Y
for /d %%X in (PackingFolder) do "%PROGRAMFILES%\7-Zip\7z.exe" a -tzip -mx7 "KoikatuPlugins.zip" ".\%%X\*"
