# Compilar Android localmente

Requiere Visual Studio 2022 actualizado con .NET MAUI/Android, SDK .NET 9 y las cargas de trabajo Android compatibles. Abre `MiAgendaUTN/MiAgendaUTN/MiAgendaUTN.csproj` en Visual Studio o usa la terminal de VS Code/Antigravity.

```powershell
.\build-android.ps1
```

El script restaura y compila Debug exclusivamente para `net9.0-android`. No intenta compilar iOS/MacCatalyst/Tizen ni instalar nada en un dispositivo. La aplicación puede probarse desde Visual Studio seleccionando un emulador o dispositivo Android; el SDK y el emulador pueden administrarse con Android Studio.

El APK de prueba firmado se genera junto al proyecto, en `bin/Debug/net9.0-android/`, con nombre terminado en `-Signed.apk`. Para un emulador usa el APK universal o el correspondiente a su arquitectura; para un teléfono usa la arquitectura adecuada. Las claves de firma Debug son locales y no sirven para publicar una versión de producción.

Se comprobaron compilación, firma APK y alineación ZIP de 16 KB. El APK fue instalado y arrancó en un emulador AOSP Android 14/API 34 x86_64 nuevo, en modo avión. No se probaron dispositivos reales, Android con páginas de memoria de 16 KB ni las otras plataformas. Quedan pendientes los flujos completos de interfaz y datos. Los destinos .NET/MAUI conservados requieren otra migración antes de uso en producción; consulta la [política de soporte](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).

Los cambios permanecen en una rama de revisión con PR; no se integran en la principal de UTN. `.gitignore` excluye salidas y claves/configuración local.

## Exportación Android recuperada

Android utiliza ahora `Android.Graphics.Pdf.PdfDocument`, con división de líneas y páginas, en vez de cargar los binarios nativos de escritorio de QuestPDF. QuestPDF se conserva para las otras plataformas, que no se probaron. El APK Debug incluye sus ensamblados para poder instalarse sin Fast Deployment de Visual Studio.

La prueba Debug generó un PDF de 11 páginas con acentos y descripción larga, lo abrió con PdfRenderer y renderizó su primera página. También generó un DOCX validado con OpenXmlValidator y comprobó que conserva el título. No se abrió el panel de compartir ni se probaron todos los formatos/dispositivos. La prueba no agrega actividades a la agenda.

Con el APK Debug instalado en un emulador propio:

```powershell
.\test-android.ps1 -Serial emulator-5580
```

Los archivos de la prueba se guardan en `cache/compatibility-smoke` dentro de la aplicación. La entrada de comprobación se compila únicamente en Debug; Release no la contiene.
