# SaveGameSync (SGS)

**SaveGameSync (SGS)** es una aplicación para Windows diseñada para realizar copias de seguridad y sincronizar automáticamente las partidas guardadas de videojuegos.

El objetivo principal es evitar la pérdida de partidas después de reinstalar Windows, cambiar de PC, eliminar accidentalmente un archivo o cualquier otro problema que pueda hacer desaparecer el progreso de un juego.

SGS detectará los cambios en los archivos de guardado, conservará versiones anteriores y permitirá sincronizarlos con servicios de almacenamiento en la nube.

## 🎮 Objetivos del proyecto

SaveGameSync busca ofrecer:

* Copias de seguridad automáticas de partidas.
* Detección de cambios en los archivos de guardado.
* Historial de versiones.
* Sincronización con la nube.
* Restauración de partidas.
* Detección de conflictos entre copias.
* Integración con plataformas como Steam.
* Funcionamiento en segundo plano.
* Protección contra la pérdida accidental de partidas.

## 🚧 Estado del proyecto

**En desarrollo**

El proyecto se encuentra en una etapa inicial. Las funcionalidades se irán implementando y probando progresivamente.

### Funcionalidades previstas

* [x] ~~Estructura inicial de la aplicación~~
* [x] ~~Base de datos SQLite~~
* [ ] Gestión de juegos
* [ ] Configuración de ubicaciones de partidas
* [x] ~~Detección automática de cambios~~
* [x] ~~Cálculo de SHA-256~~
* [x] ~~Versionado de partidas~~
* [x] ~~Copias de seguridad locales~~
* [ ] Integración con Google Drive
* [ ] Subida automática de partidas
* [ ] Restauración de partidas
* [ ] Detección y gestión de conflictos
* [ ] Integración con Steam
* [ ] Sincronización en segundo plano
* [ ] Icono en la bandeja del sistema de Windows
* [ ] Instalador

## 🧱 Arquitectura

El proyecto está diseñado de forma modular para permitir agregar nuevas funcionalidades y servicios de almacenamiento sin tener que modificar el núcleo de la aplicación.

```text
SaveGameSync
├── SaveGameSync.App
├── SaveGameSync.Core
├── SaveGameSync.Infrastructure
└── SaveGameSync.GoogleDrive
```

### Componentes

**SaveGameSync.App**

Interfaz gráfica y funcionamiento principal de la aplicación.

**SaveGameSync.Core**

Contiene la lógica principal del programa y las reglas de negocio.

**SaveGameSync.Infrastructure**

Manejo de base de datos, sistema de archivos y otros servicios internos.

**SaveGameSync.GoogleDrive**

Integración con Google Drive y sincronización de archivos con la nube.

## 💾 Sistema de copias

SGS no debe limitarse a mantener una única copia de cada partida.

Cuando un archivo cambie, el sistema podrá crear una nueva versión para conservar el historial.

Ejemplo:

```text
Dark Souls III/
├── 2026-10-01_18-42-12/
│   └── DS30000.sl2
├── 2026-10-01_22-15-04/
│   └── DS30000.sl2
└── 2026-10-02_01-32-51/
    └── DS30000.sl2
```

Esto permitirá recuperar una versión anterior si una partida se corrompe o se sobrescribe accidentalmente.

## ☁️ Sincronización en la nube

La primera integración prevista será **Google Drive**.

La aplicación deberá poder:

1. Detectar una modificación en una partida.
2. Crear una nueva versión.
3. Verificar el archivo mediante SHA-256.
4. Subir la nueva versión a Google Drive.
5. Mantener las versiones anteriores.
6. Restaurar una versión cuando sea necesario.

En el futuro podrán añadirse otros servicios de almacenamiento.

## ⚠️ Gestión de conflictos

SGS no deberá sobrescribir automáticamente una partida cuando existan diferentes versiones entre el equipo local y la nube.

Por ejemplo:

```text
Partida local
2026-10-02 18:30
SHA-256: AAA...

Partida en la nube
2026-10-02 20:15
SHA-256: BBB...
```

En una situación de conflicto, SGS deberá informar al usuario y permitirle decidir qué versión conservar o restaurar.

Los archivos locales y datos sensibles estarán excluidos mediante `.gitignore`.

## 🛠️ Tecnologías

* **C#**
* **.NET**
* **WPF**
* **SQLite**
* **Google Drive API**
* **Git**
* **GitHub**

## 📁 Estructura del proyecto

```text
SaveGameSync/
├── src/
│   ├── SaveGameSync.App/
│   ├── SaveGameSync.Core/
│   ├── SaveGameSync.Infrastructure/
│   └── SaveGameSync.GoogleDrive/
│
├── tests/
│   ├── SaveGameSync.Core.Tests/
│   └── SaveGameSync.Infrastructure.Tests/
│
├── docs/
│
├── .gitignore
├── README.md
├── LICENSE
└── SaveGameSync.sln
```

## 📌 Filosofía del proyecto

SaveGameSync debe priorizar:

* **Seguridad de los datos**
* **No perder partidas**
* **Versionado**
* **Transparencia**
* **Facilidad de restauración**
* **Modularidad**
* **Código mantenible**

El objetivo es que una reinstalación de Windows no vuelva a significar perder horas de progreso.

---

**SaveGameSync (SGS)**
Proyecto personal de código abierto en desarrollo.
