# ARWindow

Az ARWindow projekt célja egy passzív fényáteresztéssel működő átlátszó kijelző képességeinek bemutatása egy Augmented Reality alkalmazás keretein belül. Az alkalmazás egyidejűleg két felhasználót kezel, akik a kijelző két oldalán állnak. A két személy egyike lesz a megfigyelő, míg a másik az alany. Az ARWindow egyfajta realtime kép filter effektként működik, az alanyra AR objektumokat vetít a megfigyelő nézőpontjából.

Ahhoz, hogy ez működhessen, mindkét szereplő fejpozícióját követni kell, és ha valamelyik elmozdul, a megjelenített objektumot is mozgatni kell ahhoz, hogy betekintési szögtől függetlenül a néző mindig a korrekt helyen lássa a virtuális kiegészítőt. Ennek érdekében két eszközt használunk. Az első egy Kinect szenzor, amely az alanyt követi, és arcfelismerés mellett távolságot is tud mérni, valamint precíz információt nyújt az arc jellegzetes pontjainak helyzetéről is. A másik egy mélységképpel rendelkező kamera, amely a megfigyelőt segít elhelyezni a 3D térben.

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
	Ez a fájl a fejlesztés során használt egyéni konfigurációkat tartalmazza. A jövőben központosíthatjuk vagy ki is törölhetjük, ha már nem lesz szükség a benne található paraméterekre.
10. Nyissuk meg a főjelenetet (`Assets/Scenes/SampleScene`), ami már futtatásra kész.
