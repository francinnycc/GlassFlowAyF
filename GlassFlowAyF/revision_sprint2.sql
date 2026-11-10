START TRANSACTION;

ALTER TABLE `Cotizaciones` ADD `Version` char(36) COLLATE ascii_general_ci NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

CREATE TABLE `ValidacionesTecnicas` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `SolicitudCotizacionId` int NOT NULL,
    `Autor` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
    `Fecha` datetime(6) NOT NULL,
    `Resultado` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
    `Complejidad` varchar(10) CHARACTER SET utf8mb4 NOT NULL,
    `Observaciones` varchar(1500) CHARACTER SET utf8mb4 NOT NULL,
    `RecomiendaVisita` tinyint(1) NOT NULL,
    `InformacionSolicitada` varchar(1500) CHARACTER SET utf8mb4 NULL,
    `RespuestaCliente` varchar(2000) CHARACTER SET utf8mb4 NULL,
    `FechaRespuesta` datetime(6) NULL,
    `Version` char(36) COLLATE ascii_general_ci NOT NULL,
    CONSTRAINT `PK_ValidacionesTecnicas` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ValidacionesTecnicas_SolicitudesCotizacion_SolicitudCotizaci~` FOREIGN KEY (`SolicitudCotizacionId`) REFERENCES `SolicitudesCotizacion` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_ValidacionesTecnicas_SolicitudCotizacionId` ON `ValidacionesTecnicas` (`SolicitudCotizacionId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261005030136_ValidacionTecnicaYAjustesCotizacion', '8.0.13');

COMMIT;

START TRANSACTION;

ALTER TABLE `AspNetUsers` ADD `AlergiasMedicamentos` varchar(1000) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `AspNetUsers` ADD `Cedula` varchar(15) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

ALTER TABLE `AspNetUsers` ADD `Medicamentos` varchar(1000) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `AspNetUsers` ADD `NumeroContactoEmergencia` varchar(30) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `AspNetUsers` ADD `PadecimientosEnfermedades` varchar(1000) CHARACTER SET utf8mb4 NULL;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261005163008_AgregarDatosSaludYContactoEmergencia', '8.0.13');

COMMIT;

START TRANSACTION;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261005184229_AgregarHistorialMedidas', '8.0.13');

COMMIT;

START TRANSACTION;

ALTER TABLE `VisitasTecnicas` ADD `FechaFinalizacion` datetime(6) NULL;

ALTER TABLE `VisitasTecnicas` ADD `ResultadoVisita` varchar(1000) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `VisitasTecnicas` ADD `VisitaCompletada` tinyint(1) NOT NULL DEFAULT FALSE;

CREATE TABLE `HistorialMedidas` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `SolicitudCotizacionId` int NOT NULL,
    `Ancho` decimal(10,2) NULL,
    `Alto` decimal(10,2) NULL,
    `Profundidad` decimal(10,2) NULL,
    `Cantidad` int NOT NULL,
    `UsuarioRegistro` varchar(100) CHARACTER SET utf8mb4 NULL,
    `FechaRegistro` datetime(6) NOT NULL,
    `Observaciones` varchar(500) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_HistorialMedidas` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_HistorialMedidas_SolicitudesCotizacion_SolicitudCotizacionId` FOREIGN KEY (`SolicitudCotizacionId`) REFERENCES `SolicitudesCotizacion` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_HistorialMedidas_SolicitudCotizacionId` ON `HistorialMedidas` (`SolicitudCotizacionId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261005234005_AgregarFechaFinalizacionVisita', '8.0.13');

COMMIT;

START TRANSACTION;

CREATE TABLE `VisitaTecnicaFotografias` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `VisitaTecnicaId` int NOT NULL,
    `RutaArchivo` varchar(500) CHARACTER SET utf8mb4 NOT NULL,
    `NombreOriginal` varchar(255) CHARACTER SET utf8mb4 NULL,
    `TipoContenido` varchar(100) CHARACTER SET utf8mb4 NULL,
    `FechaCarga` datetime(6) NOT NULL,
    CONSTRAINT `PK_VisitaTecnicaFotografias` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_VisitaTecnicaFotografias_VisitasTecnicas_VisitaTecnicaId` FOREIGN KEY (`VisitaTecnicaId`) REFERENCES `VisitasTecnicas` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX `IX_VisitaTecnicaFotografias_VisitaTecnicaId` ON `VisitaTecnicaFotografias` (`VisitaTecnicaId`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261007033709_AgregarFotografiasVisitaTecnica', '8.0.13');

COMMIT;

START TRANSACTION;

ALTER TABLE `VisitasTecnicas` ADD `TecnicoId` varchar(255) CHARACTER SET utf8mb4 NULL;

CREATE INDEX `IX_VisitasTecnicas_TecnicoId` ON `VisitasTecnicas` (`TecnicoId`);

ALTER TABLE `VisitasTecnicas` ADD CONSTRAINT `FK_VisitasTecnicas_AspNetUsers_TecnicoId` FOREIGN KEY (`TecnicoId`) REFERENCES `AspNetUsers` (`Id`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261009104708_AgregarTecnicoIdVisita', '8.0.13');

COMMIT;

START TRANSACTION;

ALTER TABLE `SolicitudesCotizacion` ADD `UbicacionInstalacion` varchar(100) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

ALTER TABLE `SolicitudesCotizacion` ADD `UbicacionPersonalizada` varchar(100) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `SolicitudesCotizacion` ADD `UnidadMedida` varchar(2) CHARACTER SET utf8mb4 NOT NULL DEFAULT '';

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261009141044_AgregarMedidasYUbicacionSolicitud', '8.0.13');

COMMIT;

