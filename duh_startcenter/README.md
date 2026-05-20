# NXStartCenter C# Migration

Dies ist eine C#-/WinForms-Migration der Python/Tkinter-App.

## Zielplattform

- .NET 10 Windows Desktop
- C# 14
- Visual Studio 2026 oder neuer mit .NET Desktop Development Workload

Microsoft dokumentiert C# 14 als aktuelle C#-Version für .NET 10.

## Start in Visual Studio

1. `NXStartCenter.csproj` öffnen.
2. NuGet-Restore ist nicht nötig; es werden nur .NET-Bordmittel genutzt.
3. Startprojekt: `NXStartCenter`.
4. `data/config.json` prüfen und lokale Pfade anpassen.
5. App starten.

## Migrierte Funktionen

- Laden/Speichern von `data/config.json`
- Kunden, NX-Versionen und Maschinen aus der bekannten Ordnerstruktur ermitteln
- NX Native starten
- NX Kundenumgebung über `app/start_routine.bat` starten
- Postbuilder starten
- Maschinenordner öffnen
- Fork öffnen
- VS Code für den Postprocessor-Ordner öffnen
- Lizenzdatei im konfigurierten Editor öffnen
- LMTools starten
- Neues Kunden-/Maschinenprojekt mit Standardordnern und `.dat`/`README.md` anlegen
- Einfache Dark-/Light-Theme-Umschaltung

## Hinweise

Die ursprüngliche Python-App referenziert einige `resources/*`-Dateien, die im Upload nicht enthalten waren. Diese Migration prüft daher fehlende Dateien robuster und legt die Kernstruktur ohne diese Zusatzressourcen an.
