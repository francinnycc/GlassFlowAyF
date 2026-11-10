START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260928223357_AgregarModuloSoporte') THEN

    CREATE TABLE `SolicitudesSoporte` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `UsuarioId` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
        `Tipo` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `Categoria` varchar(80) CHARACTER SET utf8mb4 NOT NULL,
        `Asunto` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
        `Mensaje` varchar(2000) CHARACTER SET utf8mb4 NOT NULL,
        `Estado` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        `FechaCreacion` datetime(6) NOT NULL,
        `FechaRespuesta` datetime(6) NULL,
        `Respuesta` varchar(3000) CHARACTER SET utf8mb4 NULL,
        `RespondidoPorId` longtext CHARACTER SET utf8mb4 NULL,
        `Calificacion` int NULL,
        `ComentarioCalificacion` varchar(1000) CHARACTER SET utf8mb4 NULL,
        `FechaCalificacion` datetime(6) NULL,
        CONSTRAINT `PK_SolicitudesSoporte` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_SolicitudesSoporte_AspNetUsers_UsuarioId` FOREIGN KEY (`UsuarioId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260928223357_AgregarModuloSoporte') THEN

    CREATE INDEX `IX_SolicitudesSoporte_UsuarioId` ON `SolicitudesSoporte` (`UsuarioId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260928223357_AgregarModuloSoporte') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260928223357_AgregarModuloSoporte', '8.0.13');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261005030136_ValidacionTecnicaYAjustesCotizacion') THEN

    ALTER TABLE `Cotizaciones` ADD `Version` char(36) COLLATE ascii_general_ci NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261005030136_ValidacionTecnicaYAjustesCotizacion') THEN

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

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261005030136_ValidacionTecnicaYAjustesCotizacion') THEN

    CREATE INDEX `IX_ValidacionesTecnicas_SolicitudCotizacionId` ON `ValidacionesTecnicas` (`SolicitudCotizacionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261005030136_ValidacionTecnicaYAjustesCotizacion') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20261005030136_ValidacionTecnicaYAjustesCotizacion', '8.0.13');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;
