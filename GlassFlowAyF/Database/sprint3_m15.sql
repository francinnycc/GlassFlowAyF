START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    ALTER TABLE `Cotizaciones` ADD `Activa` tinyint(1) NOT NULL DEFAULT TRUE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    ALTER TABLE `Cotizaciones` ADD `Congelada` tinyint(1) NOT NULL DEFAULT FALSE;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    ALTER TABLE `Cotizaciones` ADD `DisenoIAId` int NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    ALTER TABLE `Cotizaciones` ADD `FechaCongelacion` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    ALTER TABLE `Cotizaciones` ADD `SnapshotHash` varchar(64) CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE TABLE `Notificaciones` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `DestinatarioRol` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
        `Titulo` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
        `Mensaje` varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
        `Url` varchar(300) CHARACTER SET utf8mb4 NULL,
        `Tipo` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `SolicitudCotizacionId` int NULL,
        `Leida` tinyint(1) NOT NULL,
        `Fecha` datetime(6) NOT NULL,
        CONSTRAINT `PK_Notificaciones` PRIMARY KEY (`Id`)
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE TABLE `OrdenesPreliminares` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `CotizacionId` int NOT NULL,
        `SolicitudCotizacionId` int NOT NULL,
        `Numero` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
        `Estado` varchar(40) CHARACTER SET utf8mb4 NOT NULL,
        `FechaCreacion` datetime(6) NOT NULL,
        CONSTRAINT `PK_OrdenesPreliminares` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_OrdenesPreliminares_Cotizaciones_CotizacionId` FOREIGN KEY (`CotizacionId`) REFERENCES `Cotizaciones` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_OrdenesPreliminares_SolicitudesCotizacion_SolicitudCotizacio~` FOREIGN KEY (`SolicitudCotizacionId`) REFERENCES `SolicitudesCotizacion` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE TABLE `RevisionesCliente` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `CotizacionId` int NOT NULL,
        `SolicitudCotizacionId` int NOT NULL,
        `DisenoIAId` int NULL,
        `UsuarioId` varchar(450) CHARACTER SET utf8mb4 NOT NULL,
        `Decision` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
        `Comentario` varchar(2000) CHARACTER SET utf8mb4 NULL,
        `EstadoSolicitudAnterior` varchar(40) CHARACTER SET utf8mb4 NULL,
        `EstadoSolicitudNuevo` varchar(40) CHARACTER SET utf8mb4 NULL,
        `EstadoCotizacionAnterior` varchar(40) CHARACTER SET utf8mb4 NULL,
        `EstadoCotizacionNuevo` varchar(40) CHARACTER SET utf8mb4 NULL,
        `VersionCotizacion` char(36) COLLATE ascii_general_ci NOT NULL,
        `AceptoTerminos` tinyint(1) NOT NULL,
        `DireccionIp` varchar(45) CHARACTER SET utf8mb4 NULL,
        `UserAgent` varchar(300) CHARACTER SET utf8mb4 NULL,
        `FechaUtc` datetime(6) NOT NULL,
        `SnapshotJson` longtext CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_RevisionesCliente` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_RevisionesCliente_AspNetUsers_UsuarioId` FOREIGN KEY (`UsuarioId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_RevisionesCliente_Cotizaciones_CotizacionId` FOREIGN KEY (`CotizacionId`) REFERENCES `Cotizaciones` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_RevisionesCliente_DisenosIA_DisenoIAId` FOREIGN KEY (`DisenoIAId`) REFERENCES `DisenosIA` (`Id`) ON DELETE SET NULL,
        CONSTRAINT `FK_RevisionesCliente_SolicitudesCotizacion_SolicitudCotizacionId` FOREIGN KEY (`SolicitudCotizacionId`) REFERENCES `SolicitudesCotizacion` (`Id`) ON DELETE RESTRICT
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE TABLE `RevisionesFotografias` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `SolicitudFotografiaId` int NOT NULL,
        `Estado` varchar(30) CHARACTER SET utf8mb4 NOT NULL,
        `Observaciones` varchar(1500) CHARACTER SET utf8mb4 NULL,
        `Autor` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
        `FechaRevision` datetime(6) NOT NULL,
        `AnalisisIA` varchar(5000) CHARACTER SET utf8mb4 NULL,
        `ModeloIA` varchar(100) CHARACTER SET utf8mb4 NULL,
        CONSTRAINT `PK_RevisionesFotografias` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_RevisionesFotografias_SolicitudFotografias_SolicitudFotograf~` FOREIGN KEY (`SolicitudFotografiaId`) REFERENCES `SolicitudFotografias` (`Id`) ON DELETE CASCADE
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
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE INDEX `IX_Cotizaciones_DisenoIAId` ON `Cotizaciones` (`DisenoIAId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE INDEX `IX_Notificaciones_DestinatarioRol_Leida` ON `Notificaciones` (`DestinatarioRol`, `Leida`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE UNIQUE INDEX `IX_OrdenesPreliminares_CotizacionId` ON `OrdenesPreliminares` (`CotizacionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE UNIQUE INDEX `IX_OrdenesPreliminares_Numero` ON `OrdenesPreliminares` (`Numero`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE INDEX `IX_OrdenesPreliminares_SolicitudCotizacionId` ON `OrdenesPreliminares` (`SolicitudCotizacionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE UNIQUE INDEX `IX_RevisionesCliente_CotizacionId_VersionCotizacion` ON `RevisionesCliente` (`CotizacionId`, `VersionCotizacion`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE INDEX `IX_RevisionesCliente_DisenoIAId` ON `RevisionesCliente` (`DisenoIAId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE INDEX `IX_RevisionesCliente_FechaUtc` ON `RevisionesCliente` (`FechaUtc`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE INDEX `IX_RevisionesCliente_SolicitudCotizacionId` ON `RevisionesCliente` (`SolicitudCotizacionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE INDEX `IX_RevisionesCliente_UsuarioId` ON `RevisionesCliente` (`UsuarioId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    CREATE INDEX `IX_RevisionesFotografias_SolicitudFotografiaId` ON `RevisionesFotografias` (`SolicitudFotografiaId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    ALTER TABLE `Cotizaciones` ADD CONSTRAINT `FK_Cotizaciones_DisenosIA_DisenoIAId` FOREIGN KEY (`DisenoIAId`) REFERENCES `DisenosIA` (`Id`) ON DELETE SET NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    UPDATE `Cotizaciones` SET `Activa` = 0 WHERE `Estado` = 'Rechazada';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    UPDATE `Cotizaciones` SET `Congelada` = 1, `FechaCongelacion` = COALESCE(`FechaRespuestaCliente`, NOW()) WHERE `Estado` = 'Aprobada';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    UPDATE `SolicitudesCotizacion` SET `Estado` = 'Aprobado por el Cliente' WHERE `Estado` = 'Aprobado';

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    INSERT INTO `OrdenesPreliminares` (`CotizacionId`, `SolicitudCotizacionId`, `Numero`, `Estado`, `FechaCreacion`)
    SELECT c.`Id`, c.`SolicitudCotizacionId`, CONCAT('GF-OP-', YEAR(NOW()), '-M', LPAD(c.`Id`, 5, '0')), 'Preliminar', NOW()
    FROM `Cotizaciones` c
    WHERE c.`Estado` = 'Aprobada'
      AND NOT EXISTS (SELECT 1 FROM `OrdenesPreliminares` o WHERE o.`CotizacionId` = c.`Id`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20261107234002_Modulo15RevisionCliente') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20261107234002_Modulo15RevisionCliente', '8.0.13');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

