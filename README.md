# MiAgendaUTN

<!-- local-development:start -->
## Desarrollo local

Consulta [RUNNING.md](RUNNING.md) para ubicar cada componente, abrirlo en su IDE, compilarlo y conocer las comprobaciones realizadas y los requisitos pendientes.
<!-- local-development:end -->

Guía para compilar e ejecutar el proyecto. MiAgendaUTN es una app .NET MAUI (Blazor Hybrid) que corre en Windows y Android.

## Requisitos
- .NET SDK 9.0 (verificar con `dotnet --info`).
- Workload de .NET MAUI instalado: `dotnet workload install maui`.
- Para Android: Android SDK/Emulador (instalado con Visual Studio o `dotnet workload install maui`).



## Estructura del repo
- Solución: `MiAgendaUTN.sln`
- Proyecto MAUI: `MiAgendaUTN/MiAgendaUTN.csproj`
- Target frameworks: `net9.0-android`, `net9.0-ios`, `net9.0-maccatalyst`, y en Windows `net9.0-windows10.0.19041.0`.
- NuGet:
  - Microsoft.Maui.Controls / BlazorWebView.Maui
  - Microsoft.Extensions.Logging.Debug (9.0.5)
  - QuestPDF (2024.10.0)
  - DocumentFormat.OpenXml (3.0.2)

## Pasos:
1. Asegurarse de tener el SDK: `dotnet --list-sdks`.
2. Instalar: `dotnet workload install maui`.
3. Restaurar paquetes: `dotnet restore MiAgendaUTN.sln`.

## Ejecución rápida

- Android (emulador/dispositivo):
  - Tener un emulador iniciado o un dispositivo conectado (`adb devices`).
  - `dotnet build -t:Run -f net9.0-android .\MiAgendaUTN\MiAgendaUTN.csproj`

En Visual Studio: abrir `MiAgendaUTN.sln`, seleccionar la plataforma (Windows/Android).

## Compilar sin ejecutar
- Toda la solución: `dotnet build MiAgendaUTN.sln -c Debug`
- Proyecto específico: `dotnet build .\MiAgendaUTN\MiAgendaUTN.csproj -c Debug`

## Publicar binarios
- Android (APK por defecto):
  - `dotnet publish .\MiAgendaUTN\MiAgendaUTN.csproj -f net9.0-android -c Release -o .\artifacts\android`
  - El APK queda en `artifacts\android`.

## Solución de problemas
- “No se reconoce el framework/target”: instalo workloads y verifico con `dotnet workload list`.
- Android no instala o no corre: abro un emulador con Android API 24 o superior o conecto dispositivo (`adb devices`).
- Windows no arranca: verificar que MAUI está instalado.
- Restore falla: `dotnet nuget locals all --clear` y luego `dotnet restore`.

## Notas

- Las dependencias NuGet se restauran automáticamente con `dotnet restore`/`build`.
- Para desarrollo con Hot Reload y depuración, se usó Visual Studio 2022 con la carga de trabajo de MAUI.

<!-- verified-android:start -->
## Compilación Android

Usa `build-android.ps1` y consulta [RUNNING.md](RUNNING.md) para generar el APK de prueba y abrirlo desde Visual Studio. Las otras plataformas y la ejecución en dispositivo siguen pendientes.
<!-- verified-android:end -->
