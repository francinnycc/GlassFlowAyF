# Sprint 2 — M10 Validación técnica y M12 Cotización asistida

## Funcionalidad

- HU 10.1: el administrador registra el resultado formal de la revisión y sus observaciones desde el detalle de la solicitud. No se valida una solicitud sin producto, material, medidas, cantidad o dirección de instalación.
- HU 10.2: clasificación obligatoria Baja, Media o Alta.
- HU 10.3: solicitud de información concreta al cliente. El propietario responde desde el mismo detalle; se guardan respuesta y fecha, y la solicitud vuelve a revisión. Para fotografías adicionales puede incluir un enlace en la respuesta. No se sobrescriben respuestas ni se abre una segunda pregunta mientras exista una pendiente.
- HU 10.4: recomendación persistente de visita y enlace al formulario existente de M11 con la solicitud seleccionada. La recomendación no agenda automáticamente una visita.
- HU 10.5: historial de revisiones sucesivas con autor, fecha, complejidad, observaciones, recomendación e intercambio con el cliente.
- HU 12.1: propuesta inicial con precio de producto y adicional de material multiplicados por la cantidad. Mano de obra e instalación siguen siendo importes globales editables del proyecto; no se inventa una tarifa por metro cuadrado.
- HU 12.2: botón **Ajustar cotización** en la propuesta. Permite modificar costos, impuesto, descuento, detalle, condiciones y vencimiento en propuestas pendientes o con cambios solicitados, sin compra. Al guardar, vuelve a Pendiente para una nueva respuesta del cliente y actualiza el monto de la solicitud. Se conserva el último comentario del cliente.
- HU 12.3: se conserva el desglose y la descarga Excel existentes. El servidor recalcula los importes, redondea el impuesto a dos decimales y rechaza descuentos excesivos. Un control de versión impide aceptar o sobrescribir una propuesta modificada desde otra sesión.

Los formularios nuevos requieren autenticación, rol y token antifalsificación. Las respuestas verifican la propiedad de la solicitud. Los formularios de solicitud y cotización tienen una lista explícita de campos editables para impedir la inserción de estados, validaciones o relaciones mediante campos manipulados.

## Actualización de MySQL 8

En una instalación existente, no volver a importar el volcado completo: contiene datos de demostración.

1. Hacer respaldo de la base que se actualizará.
2. Abrir `GlassFlowAyF/Database/sprint2_m10_m12.sql` en Workbench.
3. Seleccionar el esquema `glassflow_af` como predeterminado y ejecutar el archivo completo (incluye instrucciones DELIMITER).
4. Comprobar `ValidacionesTecnicas`, la columna `Cotizaciones.Version` y el nuevo registro en `__EFMigrationsHistory`.

El script generado por EF comprueba el historial antes de aplicar cada migración. Incluye la migración previa de soporte porque el volcado del repositorio termina en `AgregarDisenosIA`. No modifica registros comerciales existentes. Requiere permisos de creación de tablas, alteración y rutinas. Como MySQL confirma DDL implícitamente, un fallo intermedio requiere revisar el esquema antes de reintentar.

Para una base nueva: importar primero `Database/glassflow_af.sql` y luego ejecutar este script. El volcado usa nombres en minúscula y fue exportado para Windows; conservar el mismo criterio `lower_case_table_names` al restaurarlo. El script EF usa los nombres del modelo, igual que las migraciones anteriores.

Alternativa, desde la carpeta del proyecto y con la conexión local configurada:

```powershell
dotnet ef database update --context ApplicationDbContext
```

Las credenciales se configuran mediante secretos de usuario o `ConnectionStrings__DefaultConnection`; no se incluyen credenciales nuevas. El proveedor se configura explícitamente para MySQL 8, lo que permite generar migraciones sin conectarse al servidor.

## Verificación

Desde la raíz del repositorio:

```powershell
dotnet build GlassFlowAyF/GlassFlowAyF.sln
dotnet test GlassFlowAyF.Tests/GlassFlowAyF.Tests.csproj
```

Las pruebas usan una base SQLite relacional en memoria y ejercitan los controladores y la persistencia. Cubren solicitud/respuesta/revalidación, acceso de otro cliente, información incompleta, preguntas pendientes, generación por cantidad, recálculo, cambios solicitados, ajustes, aprobación, vencimiento, descuentos y concurrencia. La compilación incluye las vistas Razor. La migración y el SQL se generan con el proveedor MySQL; no se aplicaron sobre la base personal del usuario.

Recorrido manual tras actualizar MySQL:

1. Como administrador, abrir una solicitud enviada, registrar complejidad Alta y solicitar una medida adicional.
2. Como su cliente, abrir Mis solicitudes → Detalle y responder. Otro cliente no debe poder responder usando esa URL.
3. Como administrador, registrar una nueva validación y recomendar visita. Verificar ambas entradas del historial y que **Programar visita recomendada** lleva a M11 con la solicitud correcta.
4. Generar una cotización y verificar importes, desglose y Excel.
5. Como cliente, solicitar cambios. Como administrador, ajustar costos y guardar. Verificar que vuelve a Pendiente y el monto actualizado aparece en la solicitud.
6. Mantener una propuesta abierta como cliente, editarla desde otra sesión e intentar aprobar la versión antigua: debe pedir recargar. Aprobar la versión actual y verificar que desaparece la opción de ajuste.

Las propuestas preliminares mantienen el comportamiento existente: pueden generarse sujetas a la validación técnica indicada en sus condiciones. La validación formal se registra por separado y no se deduce de un cambio manual de estado.
