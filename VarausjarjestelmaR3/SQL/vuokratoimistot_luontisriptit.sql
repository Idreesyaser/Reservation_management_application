-- --------------------------------------------------------
-- Verkkotietokone:              127.0.0.1
-- Palvelinversio:               11.2.2-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Versio:              12.3.0.6589
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for vuokratoimistot
DROP DATABASE IF EXISTS `vuokratoimistot`;
CREATE DATABASE IF NOT EXISTS `vuokratoimistot` /*!40100 DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci */;
USE `vuokratoimistot`;

-- Dumping structure for taulu vuokratoimistot.asiakas
CREATE TABLE IF NOT EXISTS `asiakas` (
  `asiakasID` int(11) NOT NULL AUTO_INCREMENT,
  `nimi` varchar(50) NOT NULL,
  `puhelin` varchar(15) NOT NULL,
  `katuosoite` varchar(50) NOT NULL,
  `postinumero` varchar(5) NOT NULL,
  `postitoimipaikka` varchar(30) NOT NULL,
  `sahkoposti` varchar(40) NOT NULL,
  PRIMARY KEY (`asiakasID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Dumping data for table vuokratoimistot.asiakas: ~0 rows (suunnilleen)

-- Dumping structure for taulu vuokratoimistot.asiakkaan_varaus
CREATE TABLE IF NOT EXISTS `asiakkaan_varaus` (
  `varausID` int(11) NOT NULL AUTO_INCREMENT,
  `varaus_alkaa` date NOT NULL,
  `varaus_paattyy` date NOT NULL,
  `huoneen_numeroID` int(11) NOT NULL,
  `varauspvm` date NOT NULL,
  `lisatiedot` varchar(50) NOT NULL,
  `asiakasID` int(11) NOT NULL,
  `tyontekijaID` int(11) NOT NULL,
  PRIMARY KEY (`varausID`),
  KEY `huoneen_numeroID` (`huoneen_numeroID`),
  KEY `asiakasID` (`asiakasID`),
  KEY `tyontekijaID` (`tyontekijaID`),
  CONSTRAINT `asiakkaan_varaus_ibfk_1` FOREIGN KEY (`huoneen_numeroID`) REFERENCES `huoneet` (`huoneen_numeroID`),
  CONSTRAINT `asiakkaan_varaus_ibfk_2` FOREIGN KEY (`asiakasID`) REFERENCES `asiakas` (`asiakasID`),
  CONSTRAINT `asiakkaan_varaus_ibfk_3` FOREIGN KEY (`tyontekijaID`) REFERENCES `tyontekija` (`tyontekijaID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Dumping data for table vuokratoimistot.asiakkaan_varaus: ~0 rows (suunnilleen)

-- Dumping structure for taulu vuokratoimistot.huoneet
CREATE TABLE IF NOT EXISTS `huoneet` (
  `huoneen_numeroID` int(11) NOT NULL AUTO_INCREMENT,
  `nimi` varchar(40) NOT NULL,
  `hinta` double(8,2) NOT NULL,
  `alv_prosentti` double(8,2) NOT NULL,
  `hlo_maara` int(11) NOT NULL,
  `toimipisteID` int(11) NOT NULL,
  PRIMARY KEY (`huoneen_numeroID`),
  KEY `toimipisteID` (`toimipisteID`),
  CONSTRAINT `huoneet_ibfk_1` FOREIGN KEY (`toimipisteID`) REFERENCES `toimipiste` (`toimipisteID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Dumping data for table vuokratoimistot.huoneet: ~0 rows (suunnilleen)

-- Dumping structure for taulu vuokratoimistot.lasku
CREATE TABLE IF NOT EXISTS `lasku` (
  `laskuID` int(11) NOT NULL AUTO_INCREMENT,
  `laskutustapa` varchar(20) NOT NULL,
  `alv_euroina` double(8,2) NOT NULL,
  `veroton_summa` double(8,2) NOT NULL,
  `loppusumma` double(8,2) NOT NULL,
  `asiakasID` int(11) NOT NULL,
  `varausID` int(11) NOT NULL,
  PRIMARY KEY (`laskuID`),
  KEY `asiakasID` (`asiakasID`),
  KEY `varausID` (`varausID`),
  CONSTRAINT `lasku_ibfk_1` FOREIGN KEY (`asiakasID`) REFERENCES `asiakas` (`asiakasID`),
  CONSTRAINT `lasku_ibfk_2` FOREIGN KEY (`varausID`) REFERENCES `asiakkaan_varaus` (`varausID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Dumping data for table vuokratoimistot.lasku: ~0 rows (suunnilleen)

-- Dumping structure for taulu vuokratoimistot.palvelu
CREATE TABLE IF NOT EXISTS `palvelu` (
  `palveluID` int(11) NOT NULL AUTO_INCREMENT,
  `tuote` varchar(30) NOT NULL,
  `palvelun_hinta` double(8,2) NOT NULL,
  `alv_prosentti` double(8,2) NOT NULL,
  `maara` int(11) NOT NULL,
  `toimipisteID` int(11) DEFAULT NULL,
  PRIMARY KEY (`palveluID`),
  KEY `toimipisteID` (`toimipisteID`),
  CONSTRAINT `palvelu_ibfk_1` FOREIGN KEY (`toimipisteID`) REFERENCES `toimipiste` (`toimipisteID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Dumping data for table vuokratoimistot.palvelu: ~0 rows (suunnilleen)

-- Dumping structure for taulu vuokratoimistot.toimipiste
CREATE TABLE IF NOT EXISTS `toimipiste` (
  `toimipisteID` int(11) NOT NULL AUTO_INCREMENT,
  `paikkakunta` varchar(30) NOT NULL,
  `toimipiste_nimi` varchar(30) NOT NULL,
  `katuosoite` varchar(50) NOT NULL,
  `postinumero` varchar(5) NOT NULL,
  `postitoimipaikka` varchar(30) NOT NULL,
  `puhelin` varchar(15) NOT NULL,
  `yritysID` int(11) NOT NULL,
  PRIMARY KEY (`toimipisteID`),
  KEY `yritysID` (`yritysID`),
  CONSTRAINT `toimipiste_ibfk_1` FOREIGN KEY (`yritysID`) REFERENCES `yritys` (`yritysID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Dumping data for table vuokratoimistot.toimipiste: ~0 rows (suunnilleen)

-- Dumping structure for taulu vuokratoimistot.toimipisteen_tyontekija
CREATE TABLE IF NOT EXISTS `toimipisteen_tyontekija` (
  `toimipisteID` int(11) NOT NULL,
  `tyontekijaID` int(11) NOT NULL,
  PRIMARY KEY (`toimipisteID`,`tyontekijaID`),
  KEY `tyontekijaID` (`tyontekijaID`),
  CONSTRAINT `toimipisteen_tyontekija_ibfk_1` FOREIGN KEY (`toimipisteID`) REFERENCES `toimipiste` (`toimipisteID`),
  CONSTRAINT `toimipisteen_tyontekija_ibfk_2` FOREIGN KEY (`tyontekijaID`) REFERENCES `tyontekija` (`tyontekijaID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Dumping data for table vuokratoimistot.toimipisteen_tyontekija: ~0 rows (suunnilleen)

-- Dumping structure for taulu vuokratoimistot.tyontekija
CREATE TABLE IF NOT EXISTS `tyontekija` (
  `tyontekijaID` int(11) NOT NULL AUTO_INCREMENT,
  `nimi` varchar(40) NOT NULL,
  `osoite` varchar(50) NOT NULL,
  `puhelin` varchar(15) NOT NULL,
  `kayttajaID` varchar(50) NOT NULL,
  `salasana` varchar(30) NOT NULL,
  `kaytto_oikeus` int(1) NOT NULL,
  `yritysID` int(11) NOT NULL,
  PRIMARY KEY (`tyontekijaID`),
  UNIQUE KEY `kayttajaID` (`kayttajaID`),
  UNIQUE KEY `salasana` (`salasana`),
  KEY `yritysID` (`yritysID`),
  CONSTRAINT `tyontekija_ibfk_1` FOREIGN KEY (`yritysID`) REFERENCES `yritys` (`yritysID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Dumping data for table vuokratoimistot.tyontekija: ~0 rows (suunnilleen)

-- Dumping structure for taulu vuokratoimistot.varauksen_palvelut
CREATE TABLE IF NOT EXISTS `varauksen_palvelut` (
  `as_palveluvarauksenID` int(11) NOT NULL AUTO_INCREMENT,
  `palveluID` int(11) NOT NULL,
  `kpl` int(11) NOT NULL,
  `varausID` int(11) NOT NULL,
  PRIMARY KEY (`as_palveluvarauksenID`),
  KEY `palveluID` (`palveluID`),
  KEY `varausID` (`varausID`),
  CONSTRAINT `varauksen_palvelut_ibfk_1` FOREIGN KEY (`palveluID`) REFERENCES `palvelu` (`palveluID`),
  CONSTRAINT `varauksen_palvelut_ibfk_2` FOREIGN KEY (`varausID`) REFERENCES `asiakkaan_varaus` (`varausID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Dumping data for table vuokratoimistot.varauksen_palvelut: ~0 rows (suunnilleen)

-- Dumping structure for taulu vuokratoimistot.yritys
CREATE TABLE IF NOT EXISTS `yritys` (
  `yritysID` int(11) NOT NULL AUTO_INCREMENT,
  `yrityksen_nimi` varchar(30) NOT NULL,
  `katuosoite` varchar(50) NOT NULL,
  `postinumero` varchar(5) NOT NULL,
  `postitoimipaikka` varchar(30) NOT NULL,
  `email` varchar(40) NOT NULL,
  `puhelin` varchar(15) NOT NULL,
  PRIMARY KEY (`yritysID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Dumping data for table vuokratoimistot.yritys: ~0 rows (suunnilleen)

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
