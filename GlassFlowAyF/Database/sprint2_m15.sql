START TRANSACTION;

ALTER TABLE `Materiales` ADD `FichaTecnicaUrl` varchar(500) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `Materiales` ADD `Garantia` varchar(500) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `Materiales` ADD `Herrajes` varchar(500) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `Cotizaciones` ADD `Congelada` tinyint(1) NOT NULL DEFAULT FALSE;

ALTER TABLE `Cotizaciones` ADD `CongeladaPorId` varchar(255) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `Cotizaciones` ADD `FechaCongelacion` datetime(6) NULL;

ALTER TABLE `Cotizaciones` ADD `IpCongelacion` varchar(64) CHARACTER SET utf8mb4 NULL;

ALTER TABLE `Cotizaciones` ADD `PlanosUrl` varchar(500) CHARACTER SET utf8mb4 NULL;

CREATE TABLE `DecisionesPropuesta` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `SolicitudCotizacionId` int NOT NULL,
    `CotizacionId` int NOT NULL,
    `DisenoIAId` int NULL,
    `ClienteId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `Decision` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
    `Comentario` varchar(2000) CHARACTER SET utf8mb4 NULL,
    `AceptaTerminos` tinyint(1) NOT NULL,
    `FechaHora` datetime(6) NOT NULL,
    `DireccionIP` varchar(64) CHARACTER SET utf8mb4 NULL,
    `VersionCotizacion` char(36) COLLATE ascii_general_ci NOT NULL,
    `EstadoAnterior` varchar(40) CHARACTER SET utf8mb4 NULL,
    `EstadoNuevo` varchar(40) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_DecisionesPropuesta` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_DecisionesPropuesta_AspNetUsers_ClienteId` FOREIGN KEY (`ClienteId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_DecisionesPropuesta_Cotizaciones_CotizacionId` FOREIGN KEY (`CotizacionId`) REFERENCES `Cotizaciones` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_DecisionesPropuesta_DisenosIA_DisenoIAId` FOREIGN KEY (`DisenoIAId`) REFERENCES `DisenosIA` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_DecisionesPropuesta_SolicitudesCotizacion_SolicitudCotizacio~` FOREIGN KEY (`SolicitudCotizacionId`) REFERENCES `SolicitudesCotizacion` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE TABLE `Notificaciones` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `UsuarioId` varchar(255) CHARACTER SET utf8mb4 NULL,
    `RolDestino` varchar(40) CHARACTER SET utf8mb4 NULL,
    `Titulo` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `Mensaje` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `Tipo` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
    `Referencia` varchar(100) CHARACTER SET utf8mb4 NULL,
    `Leida` tinyint(1) NOT NULL,
    `FechaCreacion` datetime(6) NOT NULL,
    CONSTRAINT `PK_Notificaciones` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Notificaciones_AspNetUsers_UsuarioId` FOREIGN KEY (`UsuarioId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;

UPDATE `Materiales` SET `FichaTecnicaUrl` = NULL, `Garantia` = NULL, `Herrajes` = NULL
WHERE `Id` = 1;
SELECT ROW_COUNT();


UPDATE `Materiales` SET `FichaTecnicaUrl` = NULL, `Garantia` = NULL, `Herrajes` = NULL
WHERE `Id` = 2;
SELECT ROW_COUNT();


UPDATE `Materiales` SET `FichaTecnicaUrl` = NULL, `Garantia` = NULL, `Herrajes` = NULL
WHERE `Id` = 3;
SELECT ROW_COUNT();


UPDATE `Materiales` SET `FichaTecnicaUrl` = NULL, `Garantia` = NULL, `Herrajes` = NULL
WHERE `Id` = 4;
SELECT ROW_COUNT();


CREATE INDEX `IX_Cotizaciones_CongeladaPorId` ON `Cotizaciones` (`CongeladaPorId`);

CREATE INDEX `IX_DecisionesPropuesta_ClienteId` ON `DecisionesPropuesta` (`ClienteId`);

CREATE INDEX `IX_DecisionesPropuesta_CotizacionId` ON `DecisionesPropuesta` (`CotizacionId`);

CREATE INDEX `IX_DecisionesPropuesta_DisenoIAId` ON `DecisionesPropuesta` (`DisenoIAId`);

CREATE INDEX `IX_DecisionesPropuesta_SolicitudCotizacionId_FechaHora` ON `DecisionesPropuesta` (`SolicitudCotizacionId`, `FechaHora`);

CREATE INDEX `IX_Notificaciones_Leida_FechaCreacion` ON `Notificaciones` (`Leida`, `FechaCreacion`);

CREATE INDEX `IX_Notificaciones_UsuarioId` ON `Notificaciones` (`UsuarioId`);

ALTER TABLE `Cotizaciones` ADD CONSTRAINT `FK_Cotizaciones_AspNetUsers_CongeladaPorId` FOREIGN KEY (`CongeladaPorId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20261107213350_AgregarModulo15Propuestas', '8.0.13');

COMMIT;

