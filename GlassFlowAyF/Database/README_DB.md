# Base de Datos - GlassFlow A&F

## Descripción

Este directorio contiene la base de datos utilizada por GlassFlow A&F.

GlassFlow A&F es un sistema desarrollado para apoyar la gestión de
solicitudes, cotizaciones, productos, materiales, compras, facturación,
instalaciones y recomendaciones de diseño asistidas por inteligencia
artificial para Soluciones A&F.

## Tecnologías

- MySQL 8
- ASP.NET Core MVC
- C#
- Entity Framework Core
- ASP.NET Core Identity

## Base de datos

Nombre:

glassflow_af

Archivo:

glassflow_af.sql

El archivo contiene la estructura de la base de datos, relaciones,
claves foráneas y datos de prueba necesarios para ejecutar el proyecto.

## Instalación

### 1. Abrir MySQL Workbench

Conectarse a la instancia local de MySQL.

### 2. Importar la base de datos

Opción recomendada:

Server -> Data Import

Seleccionar:

Import from Self-Contained File

y buscar:

Database/glassflow_af.sql

Luego ejecutar:

Start Import

El script también contiene la creación del esquema `glassflow_af`.

### 3. Verificar

Ejecutar:

USE glassflow_af;

SHOW TABLES;

La base debe mostrar las tablas correspondientes al sistema GlassFlow A&F.

### 4. Configurar la aplicación

Cada desarrollador debe configurar localmente su propia cadena de conexión
a MySQL.

Ejemplo:

Server=localhost;Port=3306;Database=glassflow_af;User=root;Password=SU_PASSWORD;

IMPORTANTE:

No subir contraseñas reales, claves de API ni credenciales privadas al
repositorio de GitHub.

### 5. Ejecutar el proyecto

Abrir la solución en Visual Studio.

Restaurar los paquetes NuGet si es necesario.

Compilar y ejecutar la aplicación.

## Migraciones

La base contiene el historial de migraciones de Entity Framework Core
utilizado por GlassFlow A&F.

No es necesario volver a crear la base mediante migraciones después de
importar correctamente `glassflow_af.sql`.

## Equipo

Proyecto universitario desarrollado para Soluciones A&F.

Universidad Fidélitas
Curso: Diseño y Desarrollo de Sistemas