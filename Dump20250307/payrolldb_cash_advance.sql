-- MySQL dump 10.13  Distrib 8.0.38, for Win64 (x86_64)
--
-- Host: localhost    Database: payrolldb
-- ------------------------------------------------------
-- Server version	8.0.39

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
-- Table structure for table `cash_advance`
--

DROP TABLE IF EXISTS `cash_advance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cash_advance` (
  `ca_num` int NOT NULL AUTO_INCREMENT,
  `ca_id` varchar(255) NOT NULL,
  `employee_id` varchar(255) DEFAULT NULL,
  `ca_amount` decimal(10,2) DEFAULT NULL,
  `ca_date` date DEFAULT NULL,
  `ca_balance` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`ca_id`),
  UNIQUE KEY `ca_num` (`ca_num`),
  KEY `employee_id` (`employee_id`),
  CONSTRAINT `cash_advance_ibfk_1` FOREIGN KEY (`employee_id`) REFERENCES `employee` (`employee_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cash_advance`
--

LOCK TABLES `cash_advance` WRITE;
/*!40000 ALTER TABLE `cash_advance` DISABLE KEYS */;
INSERT INTO `cash_advance` VALUES (2,'CA-20251-1','2024-10-10',1000.00,'2025-01-29',0.00),(3,'CA-20252-3','2024-10-10',3000.00,'2025-02-03',2500.00),(4,'CA-20252-4','2024-11-17',500.00,'2025-02-03',500.00),(5,'CA-20252-5','2024-11-17',1000.00,'2025-02-04',1000.00),(6,'CA004','2024-11-15',8000.00,'2023-01-15',4000.00),(7,'CA005','2024-11-15',5000.00,'2023-02-01',2000.00);
/*!40000 ALTER TABLE `cash_advance` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-07 10:16:51
