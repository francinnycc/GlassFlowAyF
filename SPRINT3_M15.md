# Módulo 15 — Revisión y Aprobación del Cliente

Responsable: Francinny Cedeño Carrillo

El cliente revisa la cotización (Sprint 2) junto a la propuesta visual (diseño IA, `DisenoIA`) y registra su decisión: **aprobar**, **solicitar modificación** o **rechazar**. Solo una propuesta **"Aprobado por el Cliente"** puede generar pedido o avanzar a producción.

## Base de datos (migración `Modulo15RevisionCliente`)

| Tabla / columna | Propósito |
|---|---|
| `Cotizaciones.Congelada`, `FechaCongelacion`, `SnapshotHash` | Al aprobar, la versión queda bloqueada; hash SHA-256 del snapshot aprobado |
| `Cotizaciones.Activa` | `false` al rechazar |
| `Cotizaciones.DisenoIAId` | Propuesta visual revisada |
| `RevisionesCliente` | Bitácora inmutable: usuario, decisión, comentario, estados anterior→nuevo, versión, T&C, **IP**, user-agent, **fecha UTC**, snapshot JSON. **Índice único `(CotizacionId, VersionCotizacion)`**: una versión recibe una sola decisión |
| `OrdenesPreliminares` | Orden `GF-OP-yyyy-XXXXXX` creada al aprobar (única por cotización) |
| `Notificaciones` | Avisos y tareas internas para Administrador, Cotizador y Operaciones |

La migración también: marca congeladas las cotizaciones ya aprobadas, desactiva las rechazadas, renombra el estado `Aprobado` → `Aprobado por el Cliente` y crea la orden preliminar de las aprobaciones existentes.

> Nota: la migración incluye la tabla `RevisionesFotografias` (modelo `RevisionFotografia` que ya estaba en el código pero no tenía migración).

### Actualizar MySQL
1. Respaldar la base.
2. Ejecutar `GlassFlowAyF/Database/sprint3_m15.sql` (idempotente) en Workbench con el esquema `glassflow_af`, **o** desde `GlassFlowAyF/`: `dotnet ef database update --context ApplicationDbContext`.
3. Opcional en `appsettings`/secretos: `Notificaciones:CotizadorEmail` y `Notificaciones:OperacionesEmail`.

## Máquina de estados

```
Cotización  Pendiente ──Aprobar──▶ Aprobada (Congelada) ──▶ OrdenPreliminar ──▶ Compra / Producción
            Pendiente ──Modificar─▶ Cambios solicitados ──Ajuste (M12)──▶ Pendiente (nueva versión)
            Pendiente ──Rechazar──▶ Rechazada (Activa = false)
Solicitud   Cotizado ─▶ Aprobado por el Cliente ─▶ En producción ─▶ Instalación programada ─▶ …
            Cotizado ─▶ Solicitud de Modificación ─▶ (ajuste) Cotizado
            Cotizado ─▶ Propuesta Rechazada por el Cliente
```

El cliente solo puede decidir sobre una cotización **Pendiente, activa, no congelada, vigente** y con la **misma `Version`** que tenía al abrir la página. Cada decisión genera una nueva `Version` y el índice único de la bitácora bloquea duplicados aunque haya doble clic o dos pestañas.

## Endpoints (MVC, `RevisionClienteController`)

| Método | Ruta | Rol | Escenario |
|---|---|---|---|
| GET | `/RevisionCliente/Revisar/{solicitudId}` | Cliente dueño / Admin | 1.1 panel integrado · 1.2 Empty State |
| GET | `/RevisionCliente/DetallePropuesta/{cotizacionId}` | Cliente dueño / Admin | 1.3 fichas técnicas (también embebido en el modal) |
| POST | `/RevisionCliente/Aprobar` | Cliente | 2.1 · 2.2 (T&C obligatorio) · 2.3 notificaciones |
| POST | `/RevisionCliente/SolicitarModificacion` | Cliente | 3.1 · 3.3 |
| POST | `/RevisionCliente/Rechazar` | Cliente | 3.2 · 3.3 |
| GET | `/RevisionCliente/Historial/{solicitudId}` | Admin | Bitácora con IP, fecha y snapshot |
| GET/POST | `/RevisionCliente/Notificaciones`, `MarcarLeida` | Admin | Bandeja interna |

Todos los POST usan token antifalsificación y verifican que la solicitud pertenezca al cliente. La lógica vive en `Services/RevisionClienteService.cs`; los correos (`Services/NotificacionService.cs`) se envían después del commit y un fallo SMTP no revierte la decisión.

Cambios en módulos existentes:
- `ComprasController.Confirmar`: exige cotización aprobada **y congelada** con solicitud en el flujo aprobado (`PuedeGenerarPedido`).
- `CotizacionesController.CambiarEstado`: exige aprobación congelada **y** orden preliminar para avanzar a producción.
- `EditCotizacion` (M12): no permite ajustar versiones congeladas o rechazadas.
- Se eliminó `ResponderCotizacion`; `VerCotizacion`, `Details` y `MisSolicitudes` enlazan al nuevo panel.

## Pruebas

```powershell
dotnet test GlassFlowAyF.Tests/GlassFlowAyF.Tests.csproj -c Release
```

32 pruebas (xUnit + SQLite relacional en memoria):
- **Servicio**: consulta completa, Empty State, detalle técnico, acceso ajeno/admin; aprobar (congela, estados, orden, auditoría IP/fecha/T&C/snapshot); sin T&C; doble aprobación; índice único; vencida/ajena; notificaciones a 3 roles + correo al cliente; fallo SMTP; modificación (bloquea aprobación, tarea al cotizador); rechazo (desactiva, motivo); comentario vacío/espacios/corto con el mensaje exacto.
- **Integración**: IP real del request, rechazo vacío desde el controlador, admin consulta panel e historial, cliente ajeno → 403, roles y antiforgery por acción, **bloqueo de pedidos no aprobados** (incluido un "Aprobada" manual sin congelar), pedido tras aprobación, avance a producción, cotización congelada no editable.

## Recorrido manual
1. Cliente con solicitud sin cotización → **Mis solicitudes → Revisar propuesta** muestra el mensaje de elaboración.
2. Admin genera cotización y diseño IA. Cliente abre el panel, revisa **Ver detalles técnicos**.
3. Intentar **Rechazar** sin texto → área en rojo y alerta. Solicitar modificación con texto.
4. Admin ve la notificación, ajusta la cotización (M12); vuelve a Pendiente.
5. Cliente **Aprueba**, marca T&C. Verificar orden preliminar, versión congelada, desaparición de "Ajustar cotización" y bitácora en **Historial** (IP y fecha).
6. Abrir la propuesta en dos pestañas y aprobar en ambas: la segunda debe pedir recargar.
