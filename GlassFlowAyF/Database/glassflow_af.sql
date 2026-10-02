CREATE DATABASE  IF NOT EXISTS `glassflow_af` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `glassflow_af`;
-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: glassflow_af
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20260807041350_InitialCreate','8.0.13'),('20260807053520_AddIdentityUsuarios','8.0.13'),('20260808211447_ProductosMaterialesFotografias','8.0.13'),('20260811032810_ComprasFacturacionEInstaladores','8.0.13'),('20260811051935_AgregarDisenosIA','8.0.13');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroleclaims`
--

DROP TABLE IF EXISTS `aspnetroleclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroleclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroleclaims`
--

LOCK TABLES `aspnetroleclaims` WRITE;
/*!40000 ALTER TABLE `aspnetroleclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetroleclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroles`
--

DROP TABLE IF EXISTS `aspnetroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroles` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroles`
--

LOCK TABLES `aspnetroles` WRITE;
/*!40000 ALTER TABLE `aspnetroles` DISABLE KEYS */;
INSERT INTO `aspnetroles` VALUES ('0666a418-7867-449c-9334-def688a03724','Instalador','INSTALADOR',NULL),('1e138ba6-eae1-4982-8283-df8d7e0ade46','Cliente','CLIENTE',NULL),('8658ea93-e969-4c79-88be-b8d00ad83f86','Administrador','ADMINISTRADOR',NULL);
/*!40000 ALTER TABLE `aspnetroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserclaims`
--

LOCK TABLES `aspnetuserclaims` WRITE;
/*!40000 ALTER TABLE `aspnetuserclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserlogins`
--

DROP TABLE IF EXISTS `aspnetuserlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderKey` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderDisplayName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserlogins`
--

LOCK TABLES `aspnetuserlogins` WRITE;
/*!40000 ALTER TABLE `aspnetuserlogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserlogins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserroles`
--

DROP TABLE IF EXISTS `aspnetuserroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RoleId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserroles`
--

LOCK TABLES `aspnetuserroles` WRITE;
/*!40000 ALTER TABLE `aspnetuserroles` DISABLE KEYS */;
INSERT INTO `aspnetuserroles` VALUES ('2da29790-1e22-4d44-a5da-f8e9a103dfb1','0666a418-7867-449c-9334-def688a03724'),('9f14b64e-906c-41e1-bc2b-0695ab408c86','0666a418-7867-449c-9334-def688a03724'),('ea5aafe2-bbce-41e8-836f-184f5c11e024','0666a418-7867-449c-9334-def688a03724'),('08ab7c90-b5d7-48fe-85f5-bad855a101b0','1e138ba6-eae1-4982-8283-df8d7e0ade46'),('55a44b57-b63d-4f18-b577-d5495ded4176','1e138ba6-eae1-4982-8283-df8d7e0ade46'),('13ba4756-e4a6-4e93-aa11-8fea8baad3fc','8658ea93-e969-4c79-88be-b8d00ad83f86');
/*!40000 ALTER TABLE `aspnetuserroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusers`
--

DROP TABLE IF EXISTS `aspnetusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusers` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NombreCompleto` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Direccion` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `FechaRegistro` datetime(6) NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusers`
--

LOCK TABLES `aspnetusers` WRITE;
/*!40000 ALTER TABLE `aspnetusers` DISABLE KEYS */;
INSERT INTO `aspnetusers` VALUES ('08ab7c90-b5d7-48fe-85f5-bad855a101b0','cliente','san jose','2026-08-07 00:24:47.123438',1,'cliente2@prueba.com','CLIENTE2@PRUEBA.COM','cliente2@prueba.com','CLIENTE2@PRUEBA.COM',0,'AQAAAAIAAYagAAAAEE9NPuPDfaOdLBX4IkvVK5lZbsCTZm8KvF4/WiPypqUqWAdbDcwUeZxhwaGbdocnfQ==','UDZFUTFXLXVIJQQUCWOM5YPWFWSDRLXL','56c4bfcd-c3c5-4568-8b97-3cef7fa48053','88898888',0,0,NULL,1,0),('13ba4756-e4a6-4e93-aa11-8fea8baad3fc','Administrador GlassFlow',NULL,'2026-08-06 23:49:52.377375',1,'admin@glassflowaf.com','ADMIN@GLASSFLOWAF.COM','admin@glassflowaf.com','ADMIN@GLASSFLOWAF.COM',1,'AQAAAAIAAYagAAAAEEwvhctopTWgJ1yGe5jfu2WN8Cfn91lRf+3L6/e2GL35U/QiKhjIDkErIfE+EE6/nQ==','JF3OGPPKTNQVXGKBYPE55VJGA27SGGIB','224283d3-47f3-4d2e-8531-6311633ace2b',NULL,0,0,NULL,1,0),('2da29790-1e22-4d44-a5da-f8e9a103dfb1','Luis Fernández',NULL,'2026-08-11 00:35:13.762814',1,'instalador3@glassflowaf.com','INSTALADOR3@GLASSFLOWAF.COM','instalador3@glassflowaf.com','INSTALADOR3@GLASSFLOWAF.COM',1,'AQAAAAIAAYagAAAAEP55mywDIojAIe8THo78/jkDXqoSx+i2nYnZxCTn6oS6Q+oJfXVy6bwSh5DZTm7IZQ==','H6AJJ7LWEHCXI4MTKH5PAVO4HBRZC53Y','c8b9628a-76d0-4f5b-aba9-3c8281911e19',NULL,0,0,NULL,1,0),('55a44b57-b63d-4f18-b577-d5495ded4176','Cliente Prueba','Heredia','2026-08-06 23:56:04.767575',1,'cliente@prueba.com','CLIENTE@PRUEBA.COM','cliente@prueba.com','CLIENTE@PRUEBA.COM',0,'AQAAAAIAAYagAAAAECNFBmIJRnrB9OxzLcKYhRWVohE4uzY2u3f+IxCDcHyew20QBK+TinCbAjEa6VMzgA==','3YDPIMZWXLP642DBU2GV2LOXDXOEX3IH','6cbf00bd-868f-44ae-8927-bef67b14f67a','88888888',0,0,NULL,1,0),('9f14b64e-906c-41e1-bc2b-0695ab408c86','Carlos Ramírez',NULL,'2026-08-11 00:35:12.659896',1,'instalador1@glassflowaf.com','INSTALADOR1@GLASSFLOWAF.COM','instalador1@glassflowaf.com','INSTALADOR1@GLASSFLOWAF.COM',1,'AQAAAAIAAYagAAAAEGZnqRpCK+i/uuvcMZ1D/gFOs+YHIu9mMGdfad6nPvTcrqOkgpAwuIQggpiPoE7oQA==','DRB732WXQC3GXNN7JTM7ORRQ72OVULXT','176c7336-f582-4a26-ae70-60c1da6bb74a',NULL,0,0,NULL,1,0),('ea5aafe2-bbce-41e8-836f-184f5c11e024','Andrés Rodríguez',NULL,'2026-08-11 00:35:13.515399',1,'instalador2@glassflowaf.com','INSTALADOR2@GLASSFLOWAF.COM','instalador2@glassflowaf.com','INSTALADOR2@GLASSFLOWAF.COM',1,'AQAAAAIAAYagAAAAEFvU4LREUGMcZUUTwpRpzbQBCKgmS0nrBHDFAYyeKmeIvr5zvsrSOc3wGFEA+K/a8Q==','YQKI5YPJQLHOADMMJOE7XNMGTPGKNJ63','a981e047-4793-4fc5-8404-4c7e75f55d9d',NULL,0,0,NULL,1,0);
/*!40000 ALTER TABLE `aspnetusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusertokens`
--

DROP TABLE IF EXISTS `aspnetusertokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusertokens`
--

LOCK TABLES `aspnetusertokens` WRITE;
/*!40000 ALTER TABLE `aspnetusertokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetusertokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `compras`
--

DROP TABLE IF EXISTS `compras`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `compras` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CotizacionId` int NOT NULL,
  `ClienteId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NumeroOrden` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MetodoPago` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Estado` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Total` decimal(12,2) NOT NULL,
  `FechaCompra` datetime(6) NOT NULL,
  `FechaPago` datetime(6) DEFAULT NULL,
  `Observaciones` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Compras_CotizacionId` (`CotizacionId`),
  KEY `IX_Compras_ClienteId` (`ClienteId`),
  CONSTRAINT `FK_Compras_AspNetUsers_ClienteId` FOREIGN KEY (`ClienteId`) REFERENCES `aspnetusers` (`Id`) ON DELETE RESTRICT,
  CONSTRAINT `FK_Compras_Cotizaciones_CotizacionId` FOREIGN KEY (`CotizacionId`) REFERENCES `cotizaciones` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `compras`
--

LOCK TABLES `compras` WRITE;
/*!40000 ALTER TABLE `compras` DISABLE KEYS */;
INSERT INTO `compras` VALUES (1,1,'08ab7c90-b5d7-48fe-85f5-bad855a101b0','ORD-2026-728EB8','Pago en oficina','Finalizada',350300.00,'2026-08-10 21:58:04.381346','2026-08-10 22:09:01.547293',NULL),(2,3,'08ab7c90-b5d7-48fe-85f5-bad855a101b0','GF-ORD-2026-0041','SINPE Móvil','En producción',323350.00,'2026-08-05 16:00:00.000000','2026-08-05 16:25:00.000000','Pago confirmado. Proyecto enviado al área de producción.');
/*!40000 ALTER TABLE `compras` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cotizaciones`
--

DROP TABLE IF EXISTS `cotizaciones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cotizaciones` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SolicitudCotizacionId` int NOT NULL,
  `CostoMateriales` decimal(12,2) NOT NULL,
  `ManoObra` decimal(12,2) NOT NULL,
  `CostoInstalacion` decimal(12,2) NOT NULL,
  `OtrosCostos` decimal(12,2) NOT NULL,
  `Subtotal` decimal(12,2) NOT NULL,
  `PorcentajeImpuesto` decimal(5,2) NOT NULL,
  `Impuesto` decimal(12,2) NOT NULL,
  `Descuento` decimal(12,2) NOT NULL,
  `Total` decimal(12,2) NOT NULL,
  `Estado` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DetalleTecnico` varchar(1500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Condiciones` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ComentarioCliente` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `FechaCreacion` datetime(6) NOT NULL,
  `FechaVencimiento` datetime(6) NOT NULL,
  `FechaRespuestaCliente` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Cotizaciones_SolicitudCotizacionId` (`SolicitudCotizacionId`),
  CONSTRAINT `FK_Cotizaciones_SolicitudesCotizacion_SolicitudCotizacionId` FOREIGN KEY (`SolicitudCotizacionId`) REFERENCES `solicitudescotizacion` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cotizaciones`
--

LOCK TABLES `cotizaciones` WRITE;
/*!40000 ALTER TABLE `cotizaciones` DISABLE KEYS */;
INSERT INTO `cotizaciones` VALUES (1,3,225000.00,35000.00,50000.00,0.00,310000.00,13.00,40300.00,0.00,350300.00,'Aprobada','Producto: Baranda de vidrio templado. Medidas aproximadas: 1,80 m × 2,15 m.','Cotización sujeta a validación técnica de medidas, disponibilidad de materiales y condiciones del sitio.',NULL,'2026-08-10 21:40:25.261991','2026-08-25 00:00:00.000000','2026-08-10 21:43:02.561476'),(2,6,420000.00,85000.00,75000.00,15000.00,595000.00,13.00,77350.00,20000.00,652350.00,'Enviada','Mampara fabricada en vidrio templado gris humo de 10 mm con perfilería negra.','Incluye fabricación, transporte e instalación. Vigencia de la oferta: 15 días.',NULL,'2026-08-08 09:00:00.000000','2026-08-23 23:59:59.000000',NULL),(3,7,205000.00,45000.00,40000.00,5000.00,295000.00,13.00,38350.00,10000.00,323350.00,'Aprobada','Puerta pivotante fabricada con vidrio templado claro de 12 mm.','Incluye fabricación, herrajes, transporte e instalación.','Cotización aprobada. Favor coordinar instalación.','2026-08-04 10:00:00.000000','2026-08-19 23:59:59.000000','2026-08-05 15:30:00.000000');
/*!40000 ALTER TABLE `cotizaciones` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `disenosia`
--

DROP TABLE IF EXISTS `disenosia`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `disenosia` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SolicitudCotizacionId` int NOT NULL,
  `FotografiaBaseId` int DEFAULT NULL,
  `Estilo` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ColorPerfil` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Ambiente` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Preferencias` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Recomendacion` varchar(5000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PromptGeneracion` varchar(5000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `RutaImagenGenerada` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModeloTexto` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ModeloImagen` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EsFavorito` tinyint(1) NOT NULL,
  `FechaGeneracion` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_DisenosIA_FotografiaBaseId` (`FotografiaBaseId`),
  KEY `IX_DisenosIA_SolicitudCotizacionId` (`SolicitudCotizacionId`),
  CONSTRAINT `FK_DisenosIA_SolicitudesCotizacion_SolicitudCotizacionId` FOREIGN KEY (`SolicitudCotizacionId`) REFERENCES `solicitudescotizacion` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_DisenosIA_SolicitudFotografias_FotografiaBaseId` FOREIGN KEY (`FotografiaBaseId`) REFERENCES `solicitudfotografias` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `disenosia`
--

LOCK TABLES `disenosia` WRITE;
/*!40000 ALTER TABLE `disenosia` DISABLE KEYS */;
INSERT INTO `disenosia` VALUES (1,3,2,'Moderno','Negro mate','Elegante y luminoso',NULL,'Concepto recomendado\n- Baranda de vidrio templado-laminado tipo minimalista con perfilería inferior y/o remate superior en aluminio negro mate. Panel continuo de 1,80 m de ancho x 2,15 m de alto (1 unidad) para mantener una lectura moderna, elegante y luminosa en el ambiente visible en la fotografía.\n\nMateriales / acabados\n- Vidrio: vidrio templado + laminado (vidrio de seguridad). Recomendación visual: vidrio extra claro (low-iron) para máxima transparencia y luminosidad; grosor orientativo: 12–15 mm en función de la rigidez requerida y la normativa local. Posibilidad opcional: intercalario PVB con ligero tono azul (si el cliente desea remarcar “azul”), usando un tono muy sutil para no perder luminosidad.  \n- Perfilería: canal/base shoe en aluminio extruido con acabado negro mate (pintura electroestática/powder-coat).  \n- Remate superior: perfil o pasamano slim rectangular o redondo en negro mate (opcional) para confort táctil y continuidad visual.  \n- Fijaciones y herrajes: tornillería y anclajes ocultos o de bajo perfil en negro mate para coherencia estética; gomas y juntas EPDM negras.  \n\nRazones de diseño\n- Estética: el negro mate aporta contraste elegante frente a los tonos dorados y mármol del espacio en la foto, creando lectura moderna y sobria. El vidrio extra claro preserva la luminosidad del ambiente y la percepción de amplitud que se aprecia en la imagen.  \n- Funcionalidad: la solución laminada-templada garantiza seguridad frente a rotura (comportamiento de seguridad del laminado) y ofrece mejor control acústico si es necesario. El remate superior mejora el confort táctil y la resistencia al uso.  \n- Compatibilidad visual: líneas mínimas y herrajes negros armonizan con los acabados oscuros y elementos dorados del entorno, actuando como elemento neutro que realza flores y reflejos sin competir con ellos. La opción de un intercalario azul muy sutil puede conectar la observación “azul” del cliente sin sacrificar claridad.  \n- Mantenimiento: vidrio extra claro facilita limpieza y evita halos verdosos; perfiles negros mate disimulan marcas de uso y suciedad.\n\nAlternativa\n- Opción “ultra minimal”: baranda sin perfil superior con panel fijado mediante zócalo inferior oculto y anclajes de piso standoff en negro mate (si la estética quiere máxima transparencia). Requiere vidrio de mayor espesor (o reforzado) para reducir pandeo visual.  \n- Opción decorativa: vidrio con serigrafía satinada en franjas discretas o bordes satinados para mayor privacidad sin perder luminosidad; alternativa de laminado con PVB azul más marcado si se desea por estilo.\n\nValidación técnica\n- Estas recomendaciones son orientativas desde el punto de vista de diseño, estética y funcionalidad. Medidas finales, espesores definitivos del vidrio, tipo de laminado/vidrio templado, anclajes, distancias de seguridad, impermeabilización y requisitos normativos (cargas, barandillas, alturas mínimas) deben ser verificados y dimensionados por un técnico calificado o el instalador autorizado antes de la fabricación e instalación.','Moderno | Negro mate | Elegante y luminoso | ','/uploads/disenos-ia/3/90af8970-a372-4579-82e3-56c479943053.jpg','gpt-5-mini','gpt-image-1',1,'2026-08-11 10:07:04.360024'),(2,11,12,'Moderno','Negro mate','Elegante y luminoso','Quiero que se vea moderno','Concepto recomendado\n- Baranda moderna semienmarcada / casi sin marco: paneles de vidrio laminado templado extra–claro (low-iron) montados en canal U inferior de aluminio negro mate o con espigas negras mate, y pasamanos lineal negro mate muy discreto. Paneles segmentados para respetar la estética minimalista y preservar la sensación “flotante” de la escalera visible en la foto, manteniendo máxima transparencia para luminosidad.\n\nMateriales / acabados (orientativo)\n- Vidrio: vidrio laminado templado extra–claro (interlayer PVB o SGP) — opción recomendada por seguridad y retención de fragmentos; alternativa: vidrio templado extra–claro de mayor espesor si lo determina el técnico.  \n- Perfiles y canal: aluminio o extrusión U negro mate, acabado powder-coat mate o anodizado negro mate.  \n- Fijaciones: espigas / herrajes en acero inoxidable AISI 316 con recubrimiento negro PVD mate (si se requiere mayor resistencia a corrosión), juntas EPDM negras.  \n- Pasamanos: tubo o perfil plano de aluminio negro mate, perfil minimalista sobre el vidrio o fijado al borde superior (según normativa aplicable).  \n- Opcional: vidrio extra–claro para máxima luminosidad; interlayer SGP si se prioriza alta seguridad post–rotura.\n\nRazones de diseño\n- Estética moderna y elegante: la transparencia del vidrio extra–claro con herrajes negros mate complementa la escalera flotante y el contraste madera-negro del espacio, logrando un aspecto contemporáneo y sofisticado.  \n- Luminosidad: vidrio extra–claro maximiza paso de luz y evita el tono verdoso del vidrio estándar, manteniendo el ambiente luminoso que se desea.  \n- Minimalismo funcional: fijaciones discretas (U-channel empotrado o espigas pequeñas) conservan la sensación flotante de los peldaños y las líneas limpias del ambiente.  \n- Seguridad y servicio: el vidrio laminado retiene fragmentos en caso de rotura y mejora comportamiento post–impacto; el uso de acero inoxidable y recubrimientos mate asegura durabilidad y baja mantención.  \n- Instalación y acceso: paneles segmentados facilitan manipulación, transporte e instalación en un vano amplio (no inventamos dimensiones desde la foto — se recomienda dividir en paneles manejables).\n\nAlternativa\n- Diseño con postes metálicos negros mate (perfil tubular fino) y paneles de vidrio atornillados: aporta un aspecto industrial/moderno y simplifica anclajes si las condiciones estructurales lo requieren.  \n- Opción sin pasamanos (baranda de vidrio hasta altura reglamentaria): estética más pura, pero dependerá de cumplimiento normativo local y aceptación por parte del técnico/inspector.  \n- Vidrio con banda satinada o detalle serigrafiado horizontal si se desea más definición visual o seguridad para usuarios (reduce totalmente la transparencia en una franja).\n\nAdaptación al espacio visible (foto)\n- Para la escalera flotante mostrada: recomendar paneles fijados por el lateral de los peldaños mediante espigas discretas o un canal bajo empotrado en el plano de apoyo, de modo que no se oculte el efecto “volado” de los peldaños. Mantener distancia de separación suficiente entre vidrio y borde de peldaño para evitar impactos y permitir la iluminación LED existente. Coordinar anchuras, alineaciones y junturas para que los perfiles negros queden reconocibles como línea continua que acompaña la escalera.\n\nValidación técnica\n- Las recomendaciones anteriores son orientativas. Espesores finales del vidrio, tipo exacto de laminado, dimensiones de paneles, sistema de anclaje (U-channel, espigas, tornillería), detalle de pasamanos y requisitos de seguridad y alturas reglamentarias deben ser validados por un técnico o ingeniero estructural local y ajustarse a la normativa vigente aplicable. GlassFlow puede apoyar en la toma de medidas finales y la instalación profesional una vez definido el proyecto técnico.','Moderno | Negro mate | Elegante y luminoso | Quiero que se vea moderno','/uploads/disenos-ia/11/a3294cf1-e87f-4781-99fd-780285afbf17.jpg','gpt-5-mini','gpt-image-1',1,'2026-08-18 11:46:43.970295');
/*!40000 ALTER TABLE `disenosia` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `facturas`
--

DROP TABLE IF EXISTS `facturas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `facturas` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `CompraId` int NOT NULL,
  `NumeroFactura` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaEmision` datetime(6) NOT NULL,
  `Subtotal` decimal(12,2) NOT NULL,
  `Impuesto` decimal(12,2) NOT NULL,
  `Descuento` decimal(12,2) NOT NULL,
  `Total` decimal(12,2) NOT NULL,
  `Estado` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Facturas_CompraId` (`CompraId`),
  CONSTRAINT `FK_Facturas_Compras_CompraId` FOREIGN KEY (`CompraId`) REFERENCES `compras` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `facturas`
--

LOCK TABLES `facturas` WRITE;
/*!40000 ALTER TABLE `facturas` DISABLE KEYS */;
INSERT INTO `facturas` VALUES (1,1,'FAC-2026-00001','2026-08-10 22:09:01.548385',310000.00,40300.00,0.00,350300.00,'Emitida'),(2,2,'FAC-GF-2026-0002','2026-08-05 16:30:00.000000',295000.00,38350.00,10000.00,323350.00,'Emitida');
/*!40000 ALTER TABLE `facturas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `materiales`
--

DROP TABLE IF EXISTS `materiales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `materiales` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Tipo` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Color` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Perfil` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Acabado` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Grosor` decimal(8,2) DEFAULT NULL,
  `PrecioAdicional` decimal(12,2) NOT NULL,
  `Descripcion` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Activo` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `materiales`
--

LOCK TABLES `materiales` WRITE;
/*!40000 ALTER TABLE `materiales` DISABLE KEYS */;
INSERT INTO `materiales` VALUES (1,'Vidrio templado claro 10 mm','Vidrio templado','Transparente','Aluminio negro','Brillante',10.00,0.00,NULL,1),(2,'Vidrio templado bronce 10 mm','Vidrio templado','Bronce','Aluminio negro','Brillante',10.00,25000.00,NULL,1),(3,'Vidrio laminado 12 mm','Vidrio laminado','Transparente','Aluminio natural','Natural',12.00,35000.00,NULL,1),(4,'Vidrio esmerilado 8 mm','Vidrio templado','Esmerilado','Aluminio negro','Mate',8.00,30000.00,NULL,1),(5,'maderiza','Otro','blanco','a','si',100.00,25000.00,'fuerte',1),(6,'Vidrio templado claro 8 mm','Vidrio templado','Transparente','Aluminio negro mate','Brillante',8.00,18000.00,'Vidrio templado transparente para divisiones de baño, puertas y aplicaciones interiores.',1),(7,'Vidrio templado claro 12 mm','Vidrio templado','Transparente','Acero inoxidable','Brillante',12.00,45000.00,'Vidrio de mayor grosor recomendado para barandas y aplicaciones de mayor exigencia.',1),(8,'Vidrio gris humo 10 mm','Vidrio templado','Gris humo','Aluminio negro mate','Brillante',10.00,32000.00,'Vidrio de tonalidad gris para diseños modernos y mayor privacidad visual.',1),(9,'Vidrio satinado 10 mm','Vidrio templado','Satinado','Aluminio natural','Mate',10.00,35000.00,'Vidrio con acabado satinado que proporciona privacidad manteniendo el paso de luz.',1),(10,'Vidrio laminado claro 10+10 mm','Vidrio laminado','Transparente','Acero inoxidable','Brillante',20.00,75000.00,'Vidrio laminado de seguridad para aplicaciones arquitectónicas especiales.',1);
/*!40000 ALTER TABLE `materiales` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `productomateriales`
--

DROP TABLE IF EXISTS `productomateriales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `productomateriales` (
  `ProductoId` int NOT NULL,
  `MaterialId` int NOT NULL,
  PRIMARY KEY (`ProductoId`,`MaterialId`),
  KEY `IX_ProductoMateriales_MaterialId` (`MaterialId`),
  CONSTRAINT `FK_ProductoMateriales_Materiales_MaterialId` FOREIGN KEY (`MaterialId`) REFERENCES `materiales` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_ProductoMateriales_Productos_ProductoId` FOREIGN KEY (`ProductoId`) REFERENCES `productos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `productomateriales`
--

LOCK TABLES `productomateriales` WRITE;
/*!40000 ALTER TABLE `productomateriales` DISABLE KEYS */;
INSERT INTO `productomateriales` VALUES (1,1),(2,1),(3,1),(4,1),(6,1),(7,1),(9,1),(1,2),(2,2),(7,2),(9,2),(2,3),(4,3),(9,3),(10,3),(1,4),(2,4),(3,4),(5,4),(6,4),(8,4),(3,5),(5,5),(8,5),(1,6),(2,6),(6,6),(4,7),(7,7),(9,7),(10,7),(1,8),(2,8),(6,8),(7,8),(9,8),(1,9),(2,9),(3,9),(5,9),(6,9),(7,9),(8,9),(9,9),(4,10),(9,10),(10,10);
/*!40000 ALTER TABLE `productomateriales` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `productos`
--

DROP TABLE IF EXISTS `productos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `productos` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Categoria` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Descripcion` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PrecioBase` decimal(12,2) NOT NULL,
  `Activo` tinyint(1) NOT NULL,
  `FechaCreacion` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000',
  `ImagenUrl` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PermiteInstalacion` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `productos`
--

LOCK TABLES `productos` WRITE;
/*!40000 ALTER TABLE `productos` DISABLE KEYS */;
INSERT INTO `productos` VALUES (1,'Puerta de baño corrediza en vidrio templado','Divisiones de baño','Sistema corredizo para baño fabricado en vidrio templado de seguridad, con herrajes de alta calidad y diseño moderno.',185000.00,1,'2026-01-01 00:00:00.000000','/uploads/productos/95b06013-e5f9-4f3b-b1ba-242c3bfb4479.jpeg',1),(2,'Ventana corrediza en aluminio y vidrio','Ventanas','Ventana corrediza fabricada a medida con perfilería de aluminio y vidrio de alta transparencia.',95000.00,1,'2026-01-01 00:00:00.000000','/uploads/productos/34ffeaa9-8784-4a67-9d0c-0c3569997f52.jpeg',1),(3,'Espejo personalizado para baño','Espejos','Espejo fabricado según las dimensiones del espacio, con acabado pulido y opción de instalación profesional.',65000.00,1,'2026-01-01 00:00:00.000000','/uploads/productos/356a153e-7e32-4a74-ae38-65958528cc7c.jpeg',1),(4,'Baranda de vidrio templado','Barandas','Baranda moderna de vidrio templado para gradas, balcones o terrazas, con sistemas de fijación discretos.',190000.00,1,'2026-01-01 00:00:00.000000','/uploads/productos/baranda-escalera-01.jpg',1),(5,'Espejo decorativo a medida','Espejos','Espejo decorativo personalizado fabricado a medida para salas, dormitorios, recibidores y espacios comerciales.',85000.00,1,'2026-08-10 18:32:00.328567','/uploads/productos/5396e582-10b9-407b-8b7f-ffb407ec3b6e.jpeg',1),(6,'División fija para ducha','Divisiones de baño','Panel fijo de vidrio templado con diseño minimalista para duchas modernas.',145000.00,1,'2026-08-11 12:10:51.000000','/uploads/productos/mampara-oficina-01.jpeg',1),(7,'Puerta pivotante de vidrio','Puertas','Puerta pivotante en vidrio templado con herrajes modernos para espacios residenciales o comerciales.',265000.00,1,'2026-08-11 12:10:51.000000','/uploads/productos/f7559a7b-aad3-4fa2-a227-97f0caeeffed.jpeg',1),(8,'Espejo decorativo con iluminación LED','Espejos','Espejo decorativo personalizado con iluminación LED perimetral para baños, dormitorios o vestidores.',125000.00,1,'2026-08-11 12:10:51.000000','/uploads/productos/espejo-led-bano-01.jpeg',1),(9,'Mampara divisoria para oficina','Estructuras','División arquitectónica de vidrio para oficinas y espacios comerciales con apariencia moderna.',380000.00,1,'2026-08-11 12:10:51.000000','/uploads/productos/mampara-oficina-01.jpeg',1),(10,'Techo de vidrio para terraza','Estructuras','Cubierta de vidrio para terrazas y zonas sociales, diseñada para maximizar la iluminación natural.',675000.00,1,'2026-08-11 12:10:51.000000','/uploads/productos/8d3db03e-9a46-4f7b-8473-3374a58580be.jpeg',1);
/*!40000 ALTER TABLE `productos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `solicitudescotizacion`
--

DROP TABLE IF EXISTS `solicitudescotizacion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `solicitudescotizacion` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `NombreCliente` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Correo` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Telefono` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TipoProducto` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Descripcion` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Ancho` decimal(10,2) DEFAULT NULL,
  `Alto` decimal(10,2) DEFAULT NULL,
  `RequiereInstalacion` tinyint(1) NOT NULL,
  `Direccion` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Observaciones` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Estado` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaSolicitud` datetime(6) NOT NULL,
  `MontoEstimado` decimal(12,2) DEFAULT NULL,
  `Cantidad` int NOT NULL DEFAULT '0',
  `MaterialId` int DEFAULT NULL,
  `ProductoId` int DEFAULT NULL,
  `Profundidad` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_SolicitudesCotizacion_MaterialId` (`MaterialId`),
  KEY `IX_SolicitudesCotizacion_ProductoId` (`ProductoId`),
  CONSTRAINT `FK_SolicitudesCotizacion_Materiales_MaterialId` FOREIGN KEY (`MaterialId`) REFERENCES `materiales` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_SolicitudesCotizacion_Productos_ProductoId` FOREIGN KEY (`ProductoId`) REFERENCES `productos` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `solicitudescotizacion`
--

LOCK TABLES `solicitudescotizacion` WRITE;
/*!40000 ALTER TABLE `solicitudescotizacion` DISABLE KEYS */;
INSERT INTO `solicitudescotizacion` VALUES (1,'cliente','cliente2@prueba.com','88898888','Puerta de baño','11',1.00,1.00,1,'san jose','11','Solicitado','2026-08-07 08:30:55.179104',NULL,0,NULL,NULL,NULL),(2,'cliente','cliente2@prueba.com','88898888','Espejo personalizado','grande',1.80,2.15,1,'san jose','casa azul','Instalación programada','2026-08-10 18:39:06.351213',NULL,1,4,3,10.00),(3,'cliente','cliente2@prueba.com','88898888','Baranda de vidrio templado','Lindo',1.80,2.15,1,'San José, Costa Rica','azul','Finalizado','2026-08-10 21:38:55.198309',350300.00,1,7,4,NULL),(4,'María Fernanda López','cliente@prueba.com','8888-2451','Baranda de vidrio templado','Fabricación e instalación de baranda para escalera interior de vivienda de dos niveles.',4.20,1.10,1,'San Francisco de Heredia, Heredia','Se desea un diseño moderno con perfilería negra y alta transparencia.','En revisión','2026-08-05 09:30:00.000000',395000.00,1,7,4,NULL),(5,'Daniel Vargas Rodríguez','cliente2@prueba.com','8701-5520','División fija para ducha','Panel fijo para ducha principal con vidrio satinado y perfilería minimalista.',1.55,1.95,1,'Mercedes Norte, Heredia','Se requiere confirmar medidas finales y nivel del piso.','Visita programada','2026-08-06 14:20:00.000000',175000.00,1,9,6,NULL),(6,'Andrea Jiménez Solano','cliente@prueba.com','8312-6740','Mampara divisoria para oficina','División de vidrio para separar sala de reuniones y zona administrativa.',5.40,2.40,1,'La Aurora de Heredia, Zona Franca','Diseño corporativo moderno con vidrio gris humo y perfiles negros.','Cotizado','2026-08-07 10:15:00.000000',685000.00,3,8,9,NULL),(7,'Carlos Alberto Solano','cliente2@prueba.com','8955-2218','Puerta pivotante de vidrio','Puerta principal de vidrio para acceso a oficina privada.',1.20,2.20,1,'San Joaquín de Flores, Heredia','Utilizar herrajes de acero inoxidable y vidrio templado de 12 mm.','Instalación programada','2026-08-03 11:00:00.000000',315000.00,1,7,7,NULL),(8,'Sofía Morales Castro','cliente@prueba.com','8790-1142','Espejo decorativo con iluminación LED','Espejo rectangular para baño principal con iluminación LED perimetral.',1.40,0.90,1,'Ulloa, Heredia','Preferencia por iluminación cálida y diseño sin marco visible.','Solicitado','2026-08-11 08:45:00.000000',145000.00,1,5,8,NULL),(9,'cliente','cliente2@prueba.com','88898888','Baranda de vidrio templado','Que se ajuste a la medida',10.00,120.00,1,'san jose','Uruca San Jose','Solicitado','2026-08-18 10:40:30.887583',NULL,1,1,4,10.00),(10,'cliente','cliente2@prueba.com','88898888','Baranda de vidrio templado','Ajustado perfecto',10.00,5.00,1,'san jose','Uruca san jose ','Visita programada','2026-08-18 10:42:30.071179',NULL,1,3,4,5.00),(11,'cliente','cliente2@prueba.com','88898888','Baranda de vidrio templado','Detallado',10.00,5.00,1,'san jose','Uruca','Visita programada','2026-08-18 11:43:41.324654',NULL,1,1,4,5.00);
/*!40000 ALTER TABLE `solicitudescotizacion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `solicitudfotografias`
--

DROP TABLE IF EXISTS `solicitudfotografias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `solicitudfotografias` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SolicitudCotizacionId` int NOT NULL,
  `RutaArchivo` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NombreOriginal` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `TipoContenido` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `FechaCarga` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_SolicitudFotografias_SolicitudCotizacionId` (`SolicitudCotizacionId`),
  CONSTRAINT `FK_SolicitudFotografias_SolicitudesCotizacion_SolicitudCotizaci~` FOREIGN KEY (`SolicitudCotizacionId`) REFERENCES `solicitudescotizacion` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `solicitudfotografias`
--

LOCK TABLES `solicitudfotografias` WRITE;
/*!40000 ALTER TABLE `solicitudfotografias` DISABLE KEYS */;
INSERT INTO `solicitudfotografias` VALUES (1,2,'/uploads/solicitudes/2/e559d721-d2d4-4779-bfff-60d59bf9f2c5.jpeg','images.jpeg','image/jpeg','2026-08-10 18:39:06.448094'),(2,3,'/uploads/solicitudes/3/85cc027d-e139-4102-8d68-d7ae3ee63cde.jpeg','images.jpeg','image/jpeg','2026-08-10 21:38:55.376898'),(3,4,'/uploads/solicitudes/baranda-escalera-01.jpg','baranda-escalera-01.jpg','image/jpeg','2026-08-05 09:40:00.000000'),(4,4,'/uploads/solicitudes/baranda-escalera-02.jpeg','baranda-escalera-02.jpeg','image/jpeg','2026-08-05 09:42:00.000000'),(5,5,'/uploads/solicitudes/ducha-mercedes-01.jpeg','ducha-mercedes-01.jpeg','image/jpeg','2026-08-06 14:25:00.000000'),(6,5,'/uploads/solicitudes/ducha-mercedes-02.jpeg','ducha-mercedes-02.jpeg','image/jpeg','2026-08-06 14:27:00.000000'),(7,6,'/uploads/solicitudes/mampara-oficina-01.jpeg','mampara-oficina-01.jpeg','image/jpeg','2026-08-07 10:20:00.000000'),(8,6,'/uploads/solicitudes/mampara-oficina-02.jpg','mampara-oficina-02.jpg','image/jpeg','2026-08-07 10:22:00.000000'),(9,7,'/uploads/solicitudes/puerta-pivotante-01.jpg','puerta-pivotante-01.jpg','image/jpeg','2026-08-03 11:10:00.000000'),(10,8,'/uploads/solicitudes/espejo-led-bano-01.jpeg','espejo-led-bano-01.jpeg','image/jpeg','2026-08-11 08:50:00.000000'),(11,10,'/uploads/solicitudes/10/0bde350c-007a-41bc-b14e-99006e5c7c5e.jpeg','sinbaranda.jpeg','image/jpeg','2026-08-18 10:42:30.111984'),(12,11,'/uploads/solicitudes/11/d5c49a49-2537-4a14-9e9c-faa2f8e7eece.jpeg','sinbaranda.jpeg','image/jpeg','2026-08-18 11:43:41.519479');
/*!40000 ALTER TABLE `solicitudfotografias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `trabajosinstalacion`
--

DROP TABLE IF EXISTS `trabajosinstalacion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `trabajosinstalacion` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Cliente` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Producto` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Direccion` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FechaInstalacion` datetime(6) NOT NULL,
  `Estado` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MedidasConfirmadas` tinyint(1) NOT NULL,
  `MaterialListo` tinyint(1) NOT NULL,
  `InstalacionRealizada` tinyint(1) NOT NULL,
  `RevisionAcabados` tinyint(1) NOT NULL,
  `LimpiezaFinal` tinyint(1) NOT NULL,
  `Observaciones` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `CompraId` int DEFAULT NULL,
  `FechaFinalizacion` datetime(6) DEFAULT NULL,
  `InstalacionIniciada` tinyint(1) NOT NULL DEFAULT '0',
  `InstaladorId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `SolicitudCotizacionId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_TrabajosInstalacion_CompraId` (`CompraId`),
  KEY `IX_TrabajosInstalacion_InstaladorId` (`InstaladorId`),
  KEY `IX_TrabajosInstalacion_SolicitudCotizacionId` (`SolicitudCotizacionId`),
  CONSTRAINT `FK_TrabajosInstalacion_AspNetUsers_InstaladorId` FOREIGN KEY (`InstaladorId`) REFERENCES `aspnetusers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_TrabajosInstalacion_Compras_CompraId` FOREIGN KEY (`CompraId`) REFERENCES `compras` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_TrabajosInstalacion_SolicitudesCotizacion_SolicitudCotizacio~` FOREIGN KEY (`SolicitudCotizacionId`) REFERENCES `solicitudescotizacion` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `trabajosinstalacion`
--

LOCK TABLES `trabajosinstalacion` WRITE;
/*!40000 ALTER TABLE `trabajosinstalacion` DISABLE KEYS */;
INSERT INTO `trabajosinstalacion` VALUES (1,'cliente','Baranda de vidrio templado','san jose','2026-08-14 09:00:00.000000','Finalizada',0,0,0,0,0,'jkjk',1,'2026-08-18 10:32:06.996844',0,'9f14b64e-906c-41e1-bc2b-0695ab408c86',3),(2,'Carlos Alberto Solano','Puerta pivotante de vidrio','San Joaquín de Flores, Heredia','2026-08-20 09:00:00.000000','Programada',0,0,0,0,0,'Coordinar ingreso con el cliente. Verificar medidas finales antes del montaje. Revisar vidrio templado de 12 mm y herrajes de acero inoxidable antes de iniciar.',2,NULL,0,'ea5aafe2-bbce-41e8-836f-184f5c11e024',7);
/*!40000 ALTER TABLE `trabajosinstalacion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `visitastecnicas`
--

DROP TABLE IF EXISTS `visitastecnicas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `visitastecnicas` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SolicitudCotizacionId` int NOT NULL,
  `FechaHora` datetime(6) NOT NULL,
  `TecnicoAsignado` varchar(120) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Direccion` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Observaciones` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Estado` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_VisitasTecnicas_SolicitudCotizacionId` (`SolicitudCotizacionId`),
  CONSTRAINT `FK_VisitasTecnicas_SolicitudesCotizacion_SolicitudCotizacionId` FOREIGN KEY (`SolicitudCotizacionId`) REFERENCES `solicitudescotizacion` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `visitastecnicas`
--

LOCK TABLES `visitastecnicas` WRITE;
/*!40000 ALTER TABLE `visitastecnicas` DISABLE KEYS */;
INSERT INTO `visitastecnicas` VALUES (1,2,'2026-08-11 18:41:24.712000','da','san jose','azul','Programada'),(2,5,'2026-08-18 10:00:00.000000','Carlos Ramírez','Mercedes Norte, Heredia','Confirmar ancho, altura, nivel del piso y ubicación de anclajes.','Programada'),(3,10,'2026-08-19 10:45:42.370000','Gabriel','san jose','uruca','Programada'),(4,10,'2026-08-19 10:47:27.668000','Carlos Ramirez','san jose','uruca','Programada'),(5,10,'2026-08-19 10:48:39.819000','Carlos Ramírez','san jose','uruca','Programada'),(6,11,'2026-08-19 11:48:29.950000','Carlos Ramírez','san jose','uruca','Programada');
/*!40000 ALTER TABLE `visitastecnicas` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-07 21:45:11
