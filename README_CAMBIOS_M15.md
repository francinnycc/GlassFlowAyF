# GlassFlow A&F — Cambios del Módulo 15 (Revisión y Aprobación del Cliente)

**Responsable:** Francinny Cedeño Carrillo
**Fecha:** 07/11/2026
**Base:** entrega Sprint 2 (M10 Validación técnica, M12 Cotización asistida)

El cliente revisa la cotización junto a la propuesta visual (diseño IA) y decide: **aprobar**, **solicitar modificación** o **rechazar**. Solo una propuesta **"Aprobado por el Cliente"** puede convertirse en compra o avanzar a producción.

Detalle técnico completo (tablas, máquina de estados, endpoints, plan de pruebas): ver [`SPRINT3_M15.md`](SPRINT3_M15.md).

---

## 1. Resumen de cambios

### Funcionalidad nueva
| Historia | Qué hace |
|---|---|
| HU 1.1 | Panel integrado: cotización detallada (costos, impuesto, descuento, vigencia, condiciones) junto a la propuesta visual y una tabla de especificaciones vs. montos |
| HU 1.2 | Si no hay cotización: vista "en proceso de elaboración" con el mensaje exacto del criterio |
| HU 1.3 | Modal de detalles técnicos: vidrio/material, perfilería, herrajes, garantías, renders y fotografías, condiciones comerciales |
| HU 2.1 | Aprobar: cotización **Aprobada + congelada**, proyecto **"Aprobado por el Cliente"**, se genera **orden preliminar** `GF-OP-yyyy-XXXXXX` |
| HU 2.2 | Modal de confirmación con aceptación obligatoria de términos; se registran fecha, IP, usuario y decisión |
| HU 2.3 | Notificaciones internas (Administrador, Cotizador, Operaciones) y correos, incluido al cliente con los siguientes pasos |
| HU 3.1 | Solicitar modificación: guarda observaciones, estado **"Solicitud de Modificación"**, bloquea la aprobación y crea tarea para el cotizador |
| HU 3.2 | Rechazar: estado **"Propuesta Rechazada por el Cliente"**, motivo en bitácora, propuesta desactivada |
| HU 3.3 | Comentario obligatorio: textarea en rojo y alerta exacta; también se valida en servidor |

### Seguridad y consistencia
- Solo el cliente dueño de la solicitud puede decidir; todos los POST con token antifalsificación.
- Control de versión: una pestaña vieja o un doble clic no generan una segunda decisión. Además hay un **índice único** en la base para ello.
- Bitácora inmutable `RevisionesCliente` (IP, user-agent, fecha UTC, estados anterior → nuevo, snapshot JSON de la cotización).
- **Regla crítica:** `Compras/Confirmar` y el avance a "En producción" exigen cotización aprobada, congelada y con orden preliminar.
- Una cotización congelada o rechazada ya no se puede ajustar (M12).

### Correcciones posteriores
- **Mis compras:** ahora muestra las propuestas aprobadas que todavía no se confirman como compra, con estado *"Pendiente de confirmar compra"* y botón **Confirmar compra**. Antes la lista aparecía vacía hasta que el cliente confirmaba el pago.
- **Usuario administrador de prueba:** `admin10@glassflowaf.com`, creado al iniciar la app en *Development*. La contraseña está en `appsettings.Development.json` → `SeedAdmin10:Password`. No se crea en producción.

---

## 2. Archivos

### Nuevos
| Archivo | Descripción |
|---|---|
| `Models/EstadosRevision.cs` | Constantes de estados y mensajes del módulo |
| `Models/RevisionCliente.cs` | Bitácora de decisiones |
| `Models/OrdenPreliminar.cs` | Orden preliminar generada al aprobar |
| `Models/Notificacion.cs` | Notificaciones internas |
| `Models/ViewModels/RevisionPropuestaViewModel.cs` | Modelo del panel de revisión |
| `Data/ApplicationDbContext.Modulo15.cs` | DbSets, relaciones e índices |
| `Services/IRevisionClienteService.cs`, `Services/RevisionClienteService.cs` | Lógica de estados, validaciones, auditoría |
| `Services/NotificacionService.cs` | Notificaciones internas y correos |
| `Controllers/RevisionClienteController.cs` | Revisar, Detalle, Aprobar, Modificar, Rechazar, Historial, Notificaciones |
| `Views/RevisionCliente/*` | Panel, Empty State, modales, bitácora, notificaciones |
| `wwwroot/js/revision-cliente.js` | Validación de comentario, T&C y anti doble envío |
| `Migrations/20261107234002_Modulo15RevisionCliente.*` | Migración EF |
| `Database/sprint3_m15.sql` | Script SQL idempotente de la migración |
| `GlassFlowAyF.Tests/` | Proyecto de pruebas xUnit (33 pruebas) |
| `SPRINT3_M15.md` | Documentación técnica |

### Modificados
| Archivo | Cambio |
|---|---|
| `Models/Cotizacion.cs` | `Congelada`, `FechaCongelacion`, `Activa`, `DisenoIAId`, `SnapshotHash` |
| `Data/ApplicationDbContext.cs` | Clase `partial` + llamada a la configuración del M15 |
| `Program.cs` | Registro de servicios, forwarded headers (IP real), seed de admin10 |
| `Controllers/CotizacionesController.cs` | Se eliminó `ResponderCotizacion`; `CambiarEstado` y `SincronizarEstado` usan los nuevos estados |
| `Controllers/CotizacionesController.Ajustes.cs` | No se ajustan cotizaciones congeladas/rechazadas |
| `Controllers/ComprasController.cs` | Regla `PuedeGenerarPedido`; Mis compras muestra aprobaciones pendientes |
| `Views/Cotizaciones/VerCotizacion.cshtml`, `Details.cshtml`, `MisSolicitudes.cshtml` | Enlaces al panel de revisión y nuevos estados |
| `Views/Compras/MisCompras.cshtml` | Tarjetas "Pendiente de confirmar compra" |
| `Views/Shared/_Layout.cshtml` | Menú *Notificaciones* con contador para el administrador |
| `appsettings.json` | Sección `Notificaciones` (correos de cotizador y operaciones) |
| `appsettings.Development.json` | `SeedAdmin10:Password` |
| `GlassFlowAyF.sln` | Incluye el proyecto de pruebas |

---

## 3. Instalación / actualización

### Requisitos
.NET 8 SDK y MySQL 8+.

### Base de datos
1. **Hacer respaldo** de `glassflow_af`.
2. Aplicar la migración, con **una** de estas opciones:
   - Workbench: ejecutar `GlassFlowAyF/Database/sprint3_m15.sql` sobre el esquema `glassflow_af`.
   - Consola, desde `GlassFlowAyF/`:
     ```powershell
     dotnet ef database update --context ApplicationDbContext
     ```
3. La migración también actualiza los datos existentes:
   - el estado `Aprobado` pasa a `Aprobado por el Cliente`;
   - las cotizaciones aprobadas quedan congeladas y reciben su orden preliminar;
   - las rechazadas quedan inactivas.

> ⚠️ **Si la base ya tiene otra versión del Módulo 15** (migración `AgregarModulo15Propuestas`, tablas `DecisionesPropuesta`/`Notificaciones` con otra estructura), hay que revertirla antes de aplicar esta. Esa versión y esta no son compatibles en la misma base.
>
> La migración también crea `RevisionesFotografias`, un modelo que ya existía en el código pero no tenía migración.

### Configuración opcional
En `appsettings.json` o en los secretos de usuario:
```json
"Notificaciones": {
  "CotizadorEmail": "cotizador@empresa.com",
  "OperacionesEmail": "operaciones@empresa.com"
}
```
Además se notifica por correo a todos los usuarios con rol Administrador. Si el correo SMTP no está configurado, la decisión se guarda igual y el fallo solo queda en el log.

### Ejecutar
```powershell
cd GlassFlowAyF
dotnet run --launch-profile http
```
La app queda en http://localhost:5002

### Pruebas
```powershell
dotnet test GlassFlowAyF.Tests/GlassFlowAyF.Tests.csproj
```
Resultado actual: **33 de 33 correctas**. Usan SQLite en memoria, así que no tocan la base real.

---

## 4. Recorrido para probar

1. **Cliente sin cotización:** *Mis solicitudes → Revisar propuesta* muestra el mensaje "en proceso de elaboración".
2. **Administrador:** genera la cotización (y, si se desea, el diseño IA).
3. **Cliente:** abre el panel y revisa *Ver detalles técnicos*.
4. **Cliente:** intenta *Rechazar* sin texto. Debe ver el campo en rojo y la alerta.
5. **Cliente:** envía *Solicitar modificación* con texto.
6. **Administrador:** ve la tarea en *Notificaciones* y ajusta la cotización, que vuelve a Pendiente.
7. **Cliente:** *Aprueba* y acepta los términos. Debe ver la orden preliminar y la versión congelada.
8. **Cliente:** *Mis compras* muestra "Pendiente de confirmar compra". Con *Confirmar compra* elige el método de pago.
9. **Administrador:** *Bitácora* de la solicitud muestra cada decisión con IP y fecha.
10. **Doble aprobación:** abrir la propuesta en dos pestañas y aprobar en ambas. La segunda debe pedir recargar.

---

## 5. Usuarios de prueba (solo Development)
| Usuario | Rol | Contraseña |
|---|---|---|
| `admin10@glassflowaf.com` | Administrador | ver `appsettings.Development.json` → `SeedAdmin10` |

Cambiar o eliminar esta contraseña antes de usar el proyecto fuera del entorno local.
