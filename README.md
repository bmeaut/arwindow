# ARWindow

## Beüzemelés

1. Telepítsük a [.NET Framework 4.7.1 Developer Pack](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net471)-ot.
2. Klónozzuk a repót, a `master` ág stabil.
3. UnityHub-hoz adjuk hozzá a projektet (Add project from disk, válasszuk ki a /ArWindow mappát), és nyissuk meg.
4. A `Safe Mode` hibaüzenetnél válasszuk az `Ignore` opciót.
5. A felső menüsávban a `Unity Csproj` pont alatt nyissuk meg a beállításokat (`Open Options`).
6. Ezekkel a gombokkal töltjük be a dependency-ket: `Generate DLLs`, majd `Regenerate project files`.
7. A Visual Studio magától megnyílik. Várjuk meg, hogy lehúzza a NuGet csomagokat.
8. Visszatérve Unity-be, betölti a DLL-eket és eltűnnek a hibák. Szükség esetén újraindíthatjuk Unity-t.
9. Konfiguráljuk a projektet, hogy a nem-verziókezelt, lokális paraméterek is meglegyenek. Készítsünk el az `Assets/Config/LocalSettings.json` fájlt az alábbi felépítéssel: 
	```
	{
		"videoPath": "c:/Users/docszoli/Downloads/00-1.avi",
		"cameraId": 1
	}
	```
10. Nyissuk meg a főjelenetet (`Assets/Scenes/SampleScene`), ami már futtatásra kész.