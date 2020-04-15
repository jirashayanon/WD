-- phpMyAdmin SQL Dump
-- version 4.4.14
-- http://www.phpmyadmin.net
--
-- Host: 127.0.0.1
-- Generation Time: Dec 14, 2016 at 04:20 PM
-- Server version: 5.6.26
-- PHP Version: 5.6.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `photolibrary`
--

DELIMITER $$
--
-- Procedures
--
DROP PROCEDURE IF EXISTS `ga`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `ga`(IN `fa` int)
    NO SQL
select * from hgainformation

where hgainformation.HGAId = fa$$

DROP PROCEDURE IF EXISTS `spLoadDefectHGA`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `spLoadDefectHGA`(IN `hgaIdparam` BIGINT(20))
BEGIN

SELECT hdata.Id, hgaoqa.ViewId, hview.View, hgaoqa.Defect, hgaoqa.PositionRatioX, hgaoqa.PositionRatioY
FROM hgainspectiondata hdata
	INNER JOIN hgaoqa on hdata.Id = hgaoqa.InspectionDataId
    INNER JOIN hgainspectionview hview on hview.Id = hgaoqa.ViewId
WHERE hdata.HGAId = hgaIdparam;
    
END$$

DROP PROCEDURE IF EXISTS `spSearchHGASerialNumber`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSearchHGASerialNumber`(IN `hgaSNparam` VARCHAR(20))
BEGIN
	SELECT * FROM vw_hgainfo_hgainspect
    WHERE SerialNumber = hgaSNparam;
END$$

DROP PROCEDURE IF EXISTS `spSearchPackId`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSearchPackId`(IN `packIdparam` VARCHAR(20))
BEGIN

	SELECT * FROM vw_hgainfo_hgainspect

    WHERE PackId = packIdparam;

END$$

DROP PROCEDURE IF EXISTS `spSearchTrayId`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `spSearchTrayId`(IN `trayIdparam` VARCHAR(20))
BEGIN
	SELECT * FROM vw_hgainfo_hgainspect
    WHERE ProcessTrayId = trayIdparam;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `hgainformation`
--

DROP TABLE IF EXISTS `hgainformation`;
CREATE TABLE IF NOT EXISTS `hgainformation` (
  `HGAId` bigint(20) NOT NULL,
  `ProcessTrayId` varchar(20) COLLATE utf8_unicode_ci NOT NULL,
  `Position` int(11) NOT NULL,
  `SerialNumber` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `SuspensionLotId` varchar(20) COLLATE utf8_unicode_ci NOT NULL,
  `SliderLotId` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `PackId` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `Status` varchar(15) COLLATE utf8_unicode_ci NOT NULL COMMENT 'None\nGood\nReject'
) ENGINE=InnoDB AUTO_INCREMENT=92 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `hgainformation`
--

INSERT INTO `hgainformation` (`HGAId`, `ProcessTrayId`, `Position`, `SerialNumber`, `SuspensionLotId`, `SliderLotId`, `PackId`, `Status`) VALUES
(2, '1', 1, 'Se11SJF93F', 'SUS65S9J', 'SLI891KF', 'PAC1FB932', 'Unknown'),
(3, '2', 2, 'SE12siFP2', 'SUS65S9J', 'SLI891KF', 'PAC1FB932', 'Unknown'),
(4, '3', 1, 'SE21212121', 'SUS820sf', 'SLI9029E', 'PAC21218a9', 'Unknown'),
(5, '3', 2, 'SE2222FAF3', 'SUS820sf', 'SLI9029E', 'PAC21218a9', 'Unknown'),
(6, 'Univer1', 1, 'IMHFVR40GG', '835JO3U3EI', 'GVPFJIZBEU', '4', 'None'),
(7, 'Univer1', 2, 'O2QC8N7CBH', 'OWXJJR7WBM', 'J8R39E1A9M', '4', 'None'),
(8, 'Univer1', 3, 'P8SX8AAWFT', 'G7U62FFT42', 'EHH9BBW7VO', '4', 'None'),
(9, 'Univer1', 4, '94Z9YVT9G8', 'LEJ0CKXSIW', '9GSTLC9H6D', '4', 'None'),
(10, 'Univer1', 5, 'ETCHS7TTUJ', 'EEAKMZ35UR', '6MU17CKC41', '4', 'None'),
(11, 'Univer1', 6, 'HNY9VMPQ5C', 'JSKIQQFE5O', '8LS6VJMTTS', '4', 'Reject'),
(12, 'Univer1', 7, 'L1N4O961SW', 'WUXHANNZ69', 'HJ3CDPS11X', '4', 'None'),
(13, 'Univer1', 8, '6OVZXM4HJE', 'J5UEOIZEAM', 'PK70SXOON2', '4', 'None'),
(14, 'Univer1', 9, '1WMQUOC0UV', '2KFL3KBQ0C', 'DDCSMG9PDF', '4', 'None'),
(15, 'Univer1', 10, '6P6Z8YAWNZ', 'QDUVTC999Y', '0GKX7KJYAQ', '4', 'None'),
(16, 'Univer2', 1, 'EKEB0Y94TS', '1890CL4RIS', 'GZ5ZW89UY0', '4', 'None'),
(17, 'Univer2', 2, 'LL8G8QTLA0', 'MTQ3FRROB0', 'U0CWAR1ML9', '4', 'None'),
(18, 'Univer2', 3, '2EVYAE5E94', 'FTUOLNWX4W', 'GOW7MAB7RI', '4', 'None'),
(19, 'Univer2', 4, 'Z8FP7D099B', 'RMDKWA5ZHL', 'JK0KK0EIML', '4', 'None'),
(20, 'Univer2', 5, 'RR36BQIQPI', 'D0IYXYT0MA', 'PNLN6IVL47', '4', 'None'),
(21, 'Univer2', 6, '5ZL999O918', '6SJ90IC8KE', 'T8YF1NPEYD', '4', 'Good'),
(22, 'Univer2', 7, '698ILULURW', 'MQVWIOYFS2', 'ZEOUH3ELQN', '4', 'None'),
(23, 'Univer2', 8, 'H6AM9V5BYF', '0XMPPJI0OW', 'Y6S8IM3WI3', '4', 'None'),
(24, 'Univer2', 9, '9XX6U2T7FU', 'XOEDAD659J', '8FLSR194VL', '4', 'None'),
(25, 'Univer2', 10, '11HWK41ZJH', 'PUZQQAQEWH', 'BHCF9C8JC7', '4', 'None'),
(26, 'Univer3', 1, 'KS95FCVEI6', '0DPN7OO5D7', 'X9H5193I33', '4', 'None'),
(27, 'Univer3', 2, '9705PYY0KR', 'PFI04ZY9EM', '42Q0A0WF9Q', '4', 'None'),
(28, 'Univer3', 3, '1GIDT8Y5R9', 'K52X4LZAXZ', '4W8GESM8J3', '4', 'None'),
(29, 'Univer3', 4, 'D8GMBED0CN', 'YV4ZZ82I3O', '7Q3DJL6DBB', '4', 'None'),
(30, 'Univer3', 5, 'HE3N97P8WB', 'E7V2KH6HYH', 'JPX542APA5', '4', 'None'),
(31, 'Univer3', 6, 'IEMQ88QIBQ', 'Y4W02V6O44', '9K1DHJ8J4R', '4', 'None'),
(32, 'Univer3', 7, 'DOH9MERAZX', 'F8TUTF0EE0', 'WG37ST4D9F', '4', 'None'),
(33, 'Univer3', 8, 'R08LHHVNIZ', 'PCA41Z7EAS', 'MU2Q0ONKD8', '4', 'None'),
(34, 'Univer3', 9, '07VV3F1KD8', 'U84B7GKUZZ', 'LO1JD4O3YG', '4', 'None'),
(35, 'Univer3', 10, 'NM1PDPI48J', '2C7NP92XL4', '3BYSNXWOAZ', '4', 'None'),
(36, 'Univer4', 1, '8U4BYREPX3', 'RYLGHH8QT6', 'P97WL5H9TL', '5', 'None'),
(37, 'Univer4', 2, 'QEMHHTTTE6', 'BPZNR2YGR3', 'THY25YRUTD', '5', 'None'),
(38, 'Univer4', 3, 'TIOMPHI0Q4', 'CEQZ0WU9MJ', '4975UR3SVI', '5', 'None'),
(39, 'Univer4', 4, 'M2C5F0DDDS', '9AA0QXWKGL', '1TJ72JUDVP', '5', 'None'),
(40, 'Univer4', 5, 'R5DG6HB3IM', '57PKSQNDXH', '5A0KF1IV59', '5', 'None'),
(41, 'Univer4', 6, 'VCSBCHL9X9', 'EPPH39C739', 'DVEFRH9EAN', '5', 'None'),
(42, 'Univer4', 7, 'SAKGWJHWY8', 'Z908NGS0SP', 'J7ZGZM8Z1C', '5', 'None'),
(43, 'Univer4', 8, 'RU31D94HLV', 'BXK9I94V0Y', 'S9FF6L15JS', '5', 'None'),
(44, 'Univer4', 9, '6BYAJ3N45E', 'BVZKFSQP0O', 'FHBVNHCQ64', '5', 'None'),
(45, 'Univer4', 10, 'TPEDH3BV58', 'R5R6HC197V', '34UQRBYG0N', '5', 'None'),
(46, 'Univer5', 1, '750Y95WIC3', 'FBZQ3PV60B', '838K59GKOS', '5', 'None'),
(47, 'Univer5', 2, 'I7QY192EJM', 'GJR6MD25GS', 'E9HHCR7DQJ', '5', 'None'),
(48, 'Univer5', 3, 'ZSO5VQJ0GM', 'KB7JOOBTHG', 'WYC9R0AC4H', '5', 'None'),
(49, 'Univer5', 4, 'T606ESPYH9', 'SEAZMT9MPX', '2ZVDZC1JX4', '5', 'None'),
(50, 'Univer5', 5, 'DA491NHU18', 'X9BODB0JC9', 'UJMKQM5T5Y', '5', 'None'),
(51, 'Univer5', 6, 'J8LH1549HM', '09MJNOGEHD', '23T6SV99GC', '5', 'None'),
(52, 'Univer5', 7, 'JV6PFYC389', 'WP39FUA3CZ', '18JLZAPB38', '5', 'None'),
(53, 'Univer5', 8, '5MB6II5WB3', 'VQEKVQSJQ2', 'MA97PBB07N', '5', 'None'),
(54, 'Univer5', 9, '2KUWFC6MR6', 'B3UU6G9YI2', '6HT49J260E', '5', 'None'),
(55, 'Univer5', 10, '9S8TUI70P6', '84IGRMZ91V', '0R4QQWRTP1', '5', 'None'),
(56, 'Univer6', 1, 'K72CDMR101', '03J8MBQU8W', 'MINVW6B9VA', '5', 'None'),
(57, 'Univer6', 2, 'YGVZM6IQO9', 'U5ZCMC05R2', 'DBXOPF40FL', '5', 'None'),
(58, 'Univer6', 3, 'FLC237LMYQ', '6679M0GFN5', 'Z1EK1UQBSS', '5', 'None'),
(59, 'Univer6', 4, 'YSDI298CRE', 'GQEB3L14AL', 'K18LUMHI6J', '5', 'None'),
(60, 'Univer6', 5, 'WWR0QK4VCS', 'P782HEA6KD', 'ZP15H2OYJK', '5', 'None'),
(61, 'Univer6', 6, 'U973O32R5O', '56Y2NLZB6E', 'GK86MEKH68', '5', 'None'),
(62, 'Univer6', 7, 'X5CXR47B2Y', 'C99368GUMR', '2XPX1LBTPY', '5', 'None'),
(63, 'Univer6', 8, 'LUIUK8ZAON', 'T0C1ZO3E3D', 'E9M0Z2AL5V', '5', 'None'),
(64, 'Univer6', 9, '9BMBWD67SF', 'Z7W9ENR4HB', 'DQ902USXS1', '5', 'None'),
(65, 'Univer6', 10, '122JN5CMCE', 'TVVKJRZJ75', 'ESZAKTHB9Z', '5', 'None'),
(66, 'Univer7', 1, 'D5OSUB6BWN', 'UMPYOJU3R2', 'L455583CDB', '6', 'None'),
(67, 'Univer7', 2, 'ME5TB6TARI', '9I3KVE91JJ', 'S1JRDC5U0A', '6', 'None'),
(68, 'Univer7', 3, 'VLLR3SV1GL', 'FJOE8LSXLM', 'G2RYNKDG8B', '6', 'None'),
(69, 'Univer7', 4, 'ZKE4FV21LQ', 'FR5BA3LDSX', 'JQ1P8ILI5Z', '6', 'None'),
(70, 'Univer7', 5, 'CRA6819Q9T', 'LRIUVSPQ9U', 'LYWMXXFH82', '6', 'None'),
(71, 'Univer7', 6, '9DCL9D5U9Q', '39JSFT74Z6', 'O8UFXS6LEW', '6', 'None'),
(72, 'Univer7', 7, 'WNC97JGO1R', 'KUVUYJY8CH', 'QH64NRT7A3', '6', 'None'),
(73, 'Univer7', 8, 'HE4EOA9ROS', 'FUHM82XH2G', 'ZCSLJEXXYN', '6', 'None'),
(74, 'Univer7', 9, 'XQWZ42S0OP', 'URSPWE5VHZ', 'SW0A4738ZW', '6', 'None'),
(75, 'Univer7', 10, 'NFOP9H9CVO', '360BGXFT97', '8TA0GCVJAU', '6', 'None'),
(76, 'Univer8', 1, 'UAJ1NAB0A0', 'WI327OV1OS', '32F0AG3LM2', '6', 'None'),
(77, 'Univer8', 2, 'U26QG604EE', '4KXUIOVVMK', 'O8L7BKUIFH', '6', 'None'),
(78, 'Univer8', 3, 'AOVR9E9V6T', 'N2XP90MJB9', '0U85H61FBX', '6', 'None'),
(79, 'Univer8', 4, '9IV7M5TUKJ', 'JRMBFOR72Y', 'DNN048PV4R', '6', 'None'),
(80, 'Univer8', 5, '7GCNDIZAHR', 'MDBB6ITAQV', 'DF07OUV9MJ', '6', 'None'),
(81, 'Univer8', 6, '6LFWDENJWJ', 'SYXIG9LYPC', '7LWQNHNIMD', '6', 'None'),
(82, 'Univer8', 7, 'UV95UJ5UTS', 'OJ19C94C1E', 'ISNPC5U1HX', '6', 'None'),
(83, 'Univer8', 8, '39XDR0WOXV', 'URYJYVTWTP', '3HJ1020VUM', '6', 'None'),
(84, 'Univer8', 9, '9978NRNYLD', 'GJOW74ZDTA', 'RUD86D9THE', '6', 'None'),
(85, 'Univer8', 10, 'QP2UYFPEBZ', '9LODF7DY8D', 'ROMXL7VOJQ', '6', 'None'),
(86, 'TRAY1234', 1, 'HW7K093P', 'SUSP1234', 'SLI12345', 'Pack1', 'Good'),
(87, 'TRAY1234', 2, 'HW7K966S', 'SUSP1234', 'SLI12345', 'Pack1', 'Reject'),
(88, 'TRAY1234', 3, 'HW7K962F', 'SUSP1234', 'SLI12345', 'Pack1', 'None'),
(89, 'TRAY1234', 4, 'HW7K732R', 'SUSP1234', 'SLI12345', 'Pack1', 'None'),
(90, 'TRAY1234', 5, 'HW7K747J', 'SUSP1234', 'SLI12345', 'Pack1', 'None'),
(91, 'TRAY1234', 6, 'HW7K954D', 'SUSP1234', 'SLI12345', 'Pack1', 'None');

-- --------------------------------------------------------

--
-- Table structure for table `hgainspectiondata`
--

DROP TABLE IF EXISTS `hgainspectiondata`;
CREATE TABLE IF NOT EXISTS `hgainspectiondata` (
  `Id` bigint(20) NOT NULL,
  `HGAId` bigint(20) NOT NULL,
  `InspectionMachineId` int(11) NOT NULL,
  `Datetime` datetime NOT NULL,
  `StatusFromMachine` varchar(20) COLLATE utf8_unicode_ci NOT NULL COMMENT 'None\nGood\nReject',
  `StatusFromOQA` varchar(20) COLLATE utf8_unicode_ci NOT NULL COMMENT 'None\nGood\nReject'
) ENGINE=InnoDB AUTO_INCREMENT=516 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `hgainspectiondata`
--

INSERT INTO `hgainspectiondata` (`Id`, `HGAId`, `InspectionMachineId`, `Datetime`, `StatusFromMachine`, `StatusFromOQA`) VALUES
(5, 2, 3, '2015-08-11 13:45:56', 'Good', 'Good'),
(6, 6, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(7, 6, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(8, 6, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(9, 6, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(10, 6, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(11, 6, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(12, 7, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(13, 7, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(14, 7, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(15, 7, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(16, 7, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(17, 7, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(18, 8, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(19, 8, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(20, 8, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(21, 8, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(22, 8, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(23, 8, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(24, 9, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(25, 9, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(26, 9, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(27, 9, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(28, 9, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(29, 9, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(30, 10, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(31, 10, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(32, 10, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(33, 10, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(34, 10, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(35, 10, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(36, 11, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(37, 11, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(38, 11, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(39, 11, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(40, 11, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(41, 11, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(42, 12, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(43, 12, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(44, 12, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(45, 12, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(46, 12, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(47, 12, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(48, 13, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(49, 13, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(50, 13, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(51, 13, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(52, 13, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(53, 13, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(54, 14, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(55, 14, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(56, 14, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(57, 14, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(58, 14, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(59, 14, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(60, 15, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(61, 15, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(62, 15, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(63, 15, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(64, 15, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(65, 15, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(66, 16, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(67, 16, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(68, 16, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(69, 16, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(70, 16, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(71, 16, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(72, 17, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(73, 17, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(74, 17, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(75, 17, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(76, 17, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(77, 17, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(78, 18, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(79, 18, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(80, 18, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(81, 18, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(82, 18, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(83, 18, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(84, 19, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(85, 19, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(86, 19, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(87, 19, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(88, 19, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(89, 19, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(90, 20, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(91, 20, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(92, 20, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(93, 20, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(94, 20, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(95, 20, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(96, 21, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(97, 21, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(98, 21, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(99, 21, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(100, 21, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(101, 21, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(102, 22, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(103, 22, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(104, 22, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(105, 22, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(106, 22, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(107, 22, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(108, 23, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(109, 23, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(110, 23, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(111, 23, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(112, 23, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(113, 23, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(114, 24, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(115, 24, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(116, 24, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(117, 24, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(118, 24, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(119, 24, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(120, 25, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(121, 25, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(122, 25, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(123, 25, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(124, 25, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(125, 25, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(126, 26, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(127, 26, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(128, 26, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(129, 26, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(130, 26, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(131, 26, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(132, 27, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(133, 27, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(134, 27, 7, '2015-08-13 00:00:00', 'Reject', 'None'),
(135, 27, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(136, 27, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(137, 27, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(138, 28, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(139, 28, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(140, 28, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(141, 28, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(142, 28, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(143, 28, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(144, 29, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(145, 29, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(146, 29, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(147, 29, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(148, 29, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(149, 29, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(150, 30, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(151, 30, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(152, 30, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(153, 30, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(154, 30, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(155, 30, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(156, 31, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(157, 31, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(158, 31, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(159, 31, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(160, 31, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(161, 31, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(162, 32, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(163, 32, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(164, 32, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(165, 32, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(166, 32, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(167, 32, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(168, 33, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(169, 33, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(170, 33, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(171, 33, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(172, 33, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(173, 33, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(174, 34, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(175, 34, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(176, 34, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(177, 34, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(178, 34, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(179, 34, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(180, 35, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(181, 35, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(182, 35, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(183, 35, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(184, 35, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(185, 35, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(186, 36, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(187, 36, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(188, 36, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(189, 36, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(190, 36, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(191, 36, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(192, 37, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(193, 37, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(194, 37, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(195, 37, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(196, 37, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(197, 37, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(198, 38, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(199, 38, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(200, 38, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(201, 38, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(202, 38, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(203, 38, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(204, 39, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(205, 39, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(206, 39, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(207, 39, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(208, 39, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(209, 39, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(210, 40, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(211, 40, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(212, 40, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(213, 40, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(214, 40, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(215, 40, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(216, 41, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(217, 41, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(218, 41, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(219, 41, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(220, 41, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(221, 41, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(222, 42, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(223, 42, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(224, 42, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(225, 42, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(226, 42, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(227, 42, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(228, 43, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(229, 43, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(230, 43, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(231, 43, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(232, 43, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(233, 43, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(234, 44, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(235, 44, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(236, 44, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(237, 44, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(238, 44, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(239, 44, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(240, 45, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(241, 45, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(242, 45, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(243, 45, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(244, 45, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(245, 45, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(246, 46, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(247, 46, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(248, 46, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(249, 46, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(250, 46, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(251, 46, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(252, 47, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(253, 47, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(254, 47, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(255, 47, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(256, 47, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(257, 47, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(258, 48, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(259, 48, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(260, 48, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(261, 48, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(262, 48, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(263, 48, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(264, 49, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(265, 49, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(266, 49, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(267, 49, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(268, 49, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(269, 49, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(270, 50, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(271, 50, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(272, 50, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(273, 50, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(274, 50, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(275, 50, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(276, 51, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(277, 51, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(278, 51, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(279, 51, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(280, 51, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(281, 51, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(282, 52, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(283, 52, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(284, 52, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(285, 52, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(286, 52, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(287, 52, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(288, 53, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(289, 53, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(290, 53, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(291, 53, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(292, 53, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(293, 53, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(294, 54, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(295, 54, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(296, 54, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(297, 54, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(298, 54, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(299, 54, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(300, 55, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(301, 55, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(302, 55, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(303, 55, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(304, 55, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(305, 55, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(306, 56, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(307, 56, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(308, 56, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(309, 56, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(310, 56, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(311, 56, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(312, 57, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(313, 57, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(314, 57, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(315, 57, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(316, 57, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(317, 57, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(318, 58, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(319, 58, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(320, 58, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(321, 58, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(322, 58, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(323, 58, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(324, 59, 5, '2015-08-13 00:00:00', 'Good', 'None'),
(325, 59, 6, '2015-08-13 00:00:00', 'Good', 'None'),
(326, 59, 7, '2015-08-13 00:00:00', 'Good', 'None'),
(327, 59, 8, '2015-08-13 00:00:00', 'Good', 'None'),
(328, 59, 9, '2015-08-13 00:00:00', 'Good', 'None'),
(329, 59, 10, '2015-08-13 00:00:00', 'Good', 'None'),
(330, 60, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(331, 60, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(332, 60, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(333, 60, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(334, 60, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(335, 60, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(336, 61, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(337, 61, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(338, 61, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(339, 61, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(340, 61, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(341, 61, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(342, 62, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(343, 62, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(344, 62, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(345, 62, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(346, 62, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(347, 62, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(348, 63, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(349, 63, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(350, 63, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(351, 63, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(352, 63, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(353, 63, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(354, 64, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(355, 64, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(356, 64, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(357, 64, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(358, 64, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(359, 64, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(360, 65, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(361, 65, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(362, 65, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(363, 65, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(364, 65, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(365, 65, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(366, 66, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(367, 66, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(368, 66, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(369, 66, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(370, 66, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(371, 66, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(372, 67, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(373, 67, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(374, 67, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(375, 67, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(376, 67, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(377, 67, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(378, 68, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(379, 68, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(380, 68, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(381, 68, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(382, 68, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(383, 68, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(384, 69, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(385, 69, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(386, 69, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(387, 69, 8, '2015-08-14 00:00:00', 'Reject', 'None'),
(388, 69, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(389, 69, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(390, 70, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(391, 70, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(392, 70, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(393, 70, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(394, 70, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(395, 70, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(396, 71, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(397, 71, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(398, 71, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(399, 71, 8, '2015-08-14 00:00:00', 'Reject', 'None'),
(400, 71, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(401, 71, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(402, 72, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(403, 72, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(404, 72, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(405, 72, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(406, 72, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(407, 72, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(408, 73, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(409, 73, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(410, 73, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(411, 73, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(412, 73, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(413, 73, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(414, 74, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(415, 74, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(416, 74, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(417, 74, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(418, 74, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(419, 74, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(420, 75, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(421, 75, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(422, 75, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(423, 75, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(424, 75, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(425, 75, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(426, 76, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(427, 76, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(428, 76, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(429, 76, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(430, 76, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(431, 76, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(432, 77, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(433, 77, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(434, 77, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(435, 77, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(436, 77, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(437, 77, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(438, 78, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(439, 78, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(440, 78, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(441, 78, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(442, 78, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(443, 78, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(444, 79, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(445, 79, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(446, 79, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(447, 79, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(448, 79, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(449, 79, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(450, 80, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(451, 80, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(452, 80, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(453, 80, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(454, 80, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(455, 80, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(456, 81, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(457, 81, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(458, 81, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(459, 81, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(460, 81, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(461, 81, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(462, 82, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(463, 82, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(464, 82, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(465, 82, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(466, 82, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(467, 82, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(468, 83, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(469, 83, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(470, 83, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(471, 83, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(472, 83, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(473, 83, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(474, 84, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(475, 84, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(476, 84, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(477, 84, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(478, 84, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(479, 84, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(480, 85, 5, '2015-08-14 00:00:00', 'Good', 'None'),
(481, 85, 6, '2015-08-14 00:00:00', 'Good', 'None'),
(482, 85, 7, '2015-08-14 00:00:00', 'Good', 'None'),
(483, 85, 8, '2015-08-14 00:00:00', 'Good', 'None'),
(484, 85, 9, '2015-08-14 00:00:00', 'Good', 'None'),
(485, 85, 10, '2015-08-14 00:00:00', 'Good', 'None'),
(486, 86, 3, '2015-08-25 00:00:00', 'Good', 'Good'),
(487, 86, 7, '2015-08-25 00:00:00', 'Good', 'Good'),
(488, 87, 3, '2015-08-25 00:00:00', 'Good', 'Reject'),
(489, 87, 7, '2015-08-25 00:00:00', 'Good', 'None'),
(490, 88, 3, '2015-08-25 00:00:00', 'Good', 'None'),
(491, 88, 7, '2015-08-25 00:00:00', 'Good', 'None'),
(492, 89, 3, '2015-08-25 00:00:00', 'Good', 'None'),
(493, 89, 7, '2015-08-25 00:00:00', 'Good', 'None'),
(494, 90, 3, '2015-08-25 00:00:00', 'Good', 'None'),
(495, 90, 7, '2015-08-25 00:00:00', 'Good', 'None'),
(496, 91, 3, '2015-08-25 00:00:00', 'Good', 'None'),
(497, 91, 7, '2015-08-25 00:00:00', 'Good', 'None'),
(498, 86, 9, '2015-08-25 00:00:00', 'Good', 'None'),
(499, 86, 8, '2015-08-25 00:00:00', 'Good', 'None'),
(500, 86, 4, '2015-08-25 00:00:00', 'Good', 'None'),
(501, 87, 9, '2015-08-25 00:00:00', 'Good', 'None'),
(502, 87, 8, '2015-08-25 00:00:00', 'Good', 'None'),
(503, 87, 4, '2015-08-25 00:00:00', 'Good', 'None'),
(504, 88, 9, '2015-08-25 00:00:00', 'Good', 'None'),
(505, 88, 8, '2015-08-25 00:00:00', 'Good', 'None'),
(506, 88, 4, '2015-08-25 00:00:00', 'Good', 'None'),
(507, 89, 9, '2015-08-25 00:00:00', 'Good', 'None'),
(508, 89, 8, '2015-08-25 00:00:00', 'Good', 'None'),
(509, 89, 4, '2015-08-25 00:00:00', 'Good', 'None'),
(510, 90, 9, '2015-08-25 00:00:00', 'Good', 'None'),
(511, 90, 8, '2015-08-25 00:00:00', 'Good', 'None'),
(512, 90, 4, '2015-08-25 00:00:00', 'Good', 'None'),
(513, 91, 9, '2015-08-25 00:00:00', 'Good', 'None'),
(514, 91, 8, '2015-08-25 00:00:00', 'Good', 'None'),
(515, 91, 4, '2015-08-25 00:00:00', 'Good', 'None');

-- --------------------------------------------------------

--
-- Table structure for table `hgainspectionmachine`
--

DROP TABLE IF EXISTS `hgainspectionmachine`;
CREATE TABLE IF NOT EXISTS `hgainspectionmachine` (
  `InspectionMachineId` int(11) NOT NULL,
  `Machine` varchar(45) COLLATE utf8_unicode_ci NOT NULL,
  `Module` varchar(45) COLLATE utf8_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `hgainspectionmachine`
--

INSERT INTO `hgainspectionmachine` (`InspectionMachineId`, `Machine`, `Module`) VALUES
(3, 'ADDM', 'Left'),
(4, 'ADDM', 'Right'),
(5, 'ADM', 'Left'),
(6, 'ADM', 'Right'),
(7, 'ASMC', 'Left'),
(8, 'ASMC', 'Right'),
(9, 'AVOR', 'Left'),
(10, 'AVOR', 'Right');

-- --------------------------------------------------------

--
-- Table structure for table `hgainspectionview`
--

DROP TABLE IF EXISTS `hgainspectionview`;
CREATE TABLE IF NOT EXISTS `hgainspectionview` (
  `id` int(11) NOT NULL,
  `InspectionMachineId` int(11) DEFAULT NULL,
  `View` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `FileType` varchar(5) COLLATE utf8_unicode_ci DEFAULT NULL
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `hgainspectionview`
--

INSERT INTO `hgainspectionview` (`id`, `InspectionMachineId`, `View`, `FileType`) VALUES
(3, 3, 'FrontView', 'bmp'),
(4, 7, 'TopView2', 'jpeg'),
(5, 9, '45degView', 'bmp'),
(6, 8, 'TopView3', 'bmp'),
(7, 4, 'BackView', 'bmp');

-- --------------------------------------------------------

--
-- Table structure for table `hgamachinedefect`
--

DROP TABLE IF EXISTS `hgamachinedefect`;
CREATE TABLE IF NOT EXISTS `hgamachinedefect` (
  `Id` int(11) NOT NULL,
  `InspectionDataId` bigint(20) NOT NULL,
  `Result` varchar(20) COLLATE utf8_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `hgaoqa`
--

DROP TABLE IF EXISTS `hgaoqa`;
CREATE TABLE IF NOT EXISTS `hgaoqa` (
  `Id` bigint(20) NOT NULL,
  `InspectionDataId` bigint(20) NOT NULL,
  `ViewId` int(11) NOT NULL,
  `Defect` varchar(45) COLLATE utf8_unicode_ci NOT NULL,
  `PositionRatioX` double NOT NULL COMMENT 'No space. Ex. TopView, BackView.',
  `PositionRatioY` double NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=143 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

--
-- Dumping data for table `hgaoqa`
--

INSERT INTO `hgaoqa` (`Id`, `InspectionDataId`, `ViewId`, `Defect`, `PositionRatioX`, `PositionRatioY`) VALUES
(141, 488, 3, 'MIX', 0.749460260584218, 0.357142857142857);

-- --------------------------------------------------------

--
-- Stand-in structure for view `vw_hgainfo_hgainspect`
--
DROP VIEW IF EXISTS `vw_hgainfo_hgainspect`;
CREATE TABLE IF NOT EXISTS `vw_hgainfo_hgainspect` (
`HGAId` bigint(20)
,`ProcessTrayId` varchar(20)
,`Position` int(11)
,`SerialNumber` varchar(20)
,`SuspensionLotId` varchar(20)
,`SliderLotId` varchar(20)
,`PackId` varchar(20)
,`Status` varchar(15)
,`InspectionDataId` bigint(20)
,`InspectionMachineId` int(11)
,`Machine` varchar(45)
,`Module` varchar(45)
,`Datetime` datetime
,`StatusFromMachine` varchar(20)
,`StatusFromOQA` varchar(20)
);

-- --------------------------------------------------------

--
-- Stand-in structure for view `vw_machineview`
--
DROP VIEW IF EXISTS `vw_machineview`;
CREATE TABLE IF NOT EXISTS `vw_machineview` (
`InspectionMachineId` int(11)
,`Machine` varchar(45)
,`Module` varchar(45)
,`ViewId` int(11)
,`View` varchar(20)
,`FileType` varchar(5)
);

-- --------------------------------------------------------

--
-- Structure for view `vw_hgainfo_hgainspect`
--
DROP TABLE IF EXISTS `vw_hgainfo_hgainspect`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `vw_hgainfo_hgainspect` AS select `hgainfo`.`HGAId` AS `HGAId`,`hgainfo`.`ProcessTrayId` AS `ProcessTrayId`,`hgainfo`.`Position` AS `Position`,`hgainfo`.`SerialNumber` AS `SerialNumber`,`hgainfo`.`SuspensionLotId` AS `SuspensionLotId`,`hgainfo`.`SliderLotId` AS `SliderLotId`,`hgainfo`.`PackId` AS `PackId`,`hgainfo`.`Status` AS `Status`,`hgainspectdata`.`Id` AS `InspectionDataId`,`hgainspectdata`.`InspectionMachineId` AS `InspectionMachineId`,`hgainspectionmachine`.`Machine` AS `Machine`,`hgainspectionmachine`.`Module` AS `Module`,`hgainspectdata`.`Datetime` AS `Datetime`,`hgainspectdata`.`StatusFromMachine` AS `StatusFromMachine`,`hgainspectdata`.`StatusFromOQA` AS `StatusFromOQA` from ((`hgainformation` `hgainfo` left join `hgainspectiondata` `hgainspectdata` on((`hgainfo`.`HGAId` = `hgainspectdata`.`HGAId`))) left join `hgainspectionmachine` on((`hgainspectionmachine`.`InspectionMachineId` = `hgainspectdata`.`InspectionMachineId`)));

-- --------------------------------------------------------

--
-- Structure for view `vw_machineview`
--
DROP TABLE IF EXISTS `vw_machineview`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `vw_machineview` AS select `him`.`InspectionMachineId` AS `InspectionMachineId`,`him`.`Machine` AS `Machine`,`him`.`Module` AS `Module`,`hiv`.`id` AS `ViewId`,`hiv`.`View` AS `View`,`hiv`.`FileType` AS `FileType` from (`hgainspectionmachine` `him` join `hgainspectionview` `hiv` on((`him`.`InspectionMachineId` = `hiv`.`InspectionMachineId`)));

--
-- Indexes for dumped tables
--

--
-- Indexes for table `hgainformation`
--
ALTER TABLE `hgainformation`
  ADD PRIMARY KEY (`HGAId`),
  ADD KEY `HGAId` (`HGAId`);

--
-- Indexes for table `hgainspectiondata`
--
ALTER TABLE `hgainspectiondata`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `HGAId` (`HGAId`),
  ADD KEY `hgainspectiondata_ibfk_2_idx` (`InspectionMachineId`);

--
-- Indexes for table `hgainspectionmachine`
--
ALTER TABLE `hgainspectionmachine`
  ADD PRIMARY KEY (`InspectionMachineId`);

--
-- Indexes for table `hgainspectionview`
--
ALTER TABLE `hgainspectionview`
  ADD PRIMARY KEY (`id`),
  ADD KEY `hgainspection_view_ibfk_1_idx` (`InspectionMachineId`);

--
-- Indexes for table `hgamachinedefect`
--
ALTER TABLE `hgamachinedefect`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InspectionId` (`InspectionDataId`);

--
-- Indexes for table `hgaoqa`
--
ALTER TABLE `hgaoqa`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InspectionId` (`InspectionDataId`),
  ADD KEY `hgaoqa_ibfk_2_idx` (`ViewId`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `hgainformation`
--
ALTER TABLE `hgainformation`
  MODIFY `HGAId` bigint(20) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=92;
--
-- AUTO_INCREMENT for table `hgainspectiondata`
--
ALTER TABLE `hgainspectiondata`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=516;
--
-- AUTO_INCREMENT for table `hgainspectionview`
--
ALTER TABLE `hgainspectionview`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=8;
--
-- AUTO_INCREMENT for table `hgamachinedefect`
--
ALTER TABLE `hgamachinedefect`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT for table `hgaoqa`
--
ALTER TABLE `hgaoqa`
  MODIFY `Id` bigint(20) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=143;
--
-- Constraints for dumped tables
--

--
-- Constraints for table `hgainspectiondata`
--
ALTER TABLE `hgainspectiondata`
  ADD CONSTRAINT `hgainspectiondata_ibfk_1` FOREIGN KEY (`HGAId`) REFERENCES `hgainformation` (`HGAId`),
  ADD CONSTRAINT `hgainspectiondata_ibfk_2` FOREIGN KEY (`InspectionMachineId`) REFERENCES `hgainspectionmachine` (`InspectionMachineId`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `hgainspectionview`
--
ALTER TABLE `hgainspectionview`
  ADD CONSTRAINT `hgainspection_view_ibfk_1` FOREIGN KEY (`InspectionMachineId`) REFERENCES `hgainspectionmachine` (`InspectionMachineId`) ON DELETE NO ACTION ON UPDATE NO ACTION;

--
-- Constraints for table `hgamachinedefect`
--
ALTER TABLE `hgamachinedefect`
  ADD CONSTRAINT `hgamachinedefect_ibfk_1` FOREIGN KEY (`InspectionDataId`) REFERENCES `hgainspectiondata` (`Id`);

--
-- Constraints for table `hgaoqa`
--
ALTER TABLE `hgaoqa`
  ADD CONSTRAINT `hgaoqa_ibfk_1` FOREIGN KEY (`InspectionDataId`) REFERENCES `hgainspectiondata` (`Id`),
  ADD CONSTRAINT `hgaoqa_ibfk_2` FOREIGN KEY (`ViewId`) REFERENCES `hgainspectionview` (`id`) ON DELETE NO ACTION ON UPDATE NO ACTION;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
