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
-- Table structure for table `payroll`
--

DROP TABLE IF EXISTS `payroll`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payroll` (
  `pr_num` int NOT NULL AUTO_INCREMENT,
  `pr_id` varchar(255) NOT NULL,
  `employee_id` varchar(255) DEFAULT NULL,
  `pr_date` date DEFAULT NULL,
  `pr_cutoff` varchar(50) DEFAULT NULL,
  `pr_workedhours` decimal(6,2) DEFAULT NULL,
  `pr_grossincome` decimal(12,2) DEFAULT NULL,
  `pr_deductions` decimal(12,2) DEFAULT NULL,
  `pr_netincome` decimal(12,2) DEFAULT NULL,
  `pr_lates_minutes` smallint DEFAULT NULL,
  `pr_lates_deduction` decimal(12,2) DEFAULT NULL,
  `pr_undertime_minutes` smallint DEFAULT NULL,
  `pr_undertime_deduction` decimal(12,2) DEFAULT NULL,
  `pr_absences_days` smallint DEFAULT NULL,
  `pr_absence_deduction` decimal(12,2) DEFAULT NULL,
  `pr_job_status` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`pr_id`),
  UNIQUE KEY `pr_num` (`pr_num`),
  KEY `employee_id` (`employee_id`),
  CONSTRAINT `payroll_ibfk_1` FOREIGN KEY (`employee_id`) REFERENCES `employee` (`employee_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payroll`
--

LOCK TABLES `payroll` WRITE;
/*!40000 ALTER TABLE `payroll` DISABLE KEYS */;
INSERT INTO `payroll` VALUES (3,'PRR-2025-3-3','2024-10-10','2025-03-07','1st Period',NULL,3250.00,0.00,3250.00,NULL,NULL,NULL,NULL,NULL,NULL,'Regular'),(1,'PRT-2025-3-1','2024-10-4','2025-03-07','1st Period',0.00,0.00,0.00,0.00,0,0.00,0,0.00,0,0.00,'Teaching'),(2,'PRT-2025-3-2','2024-10-5','2025-03-07','1st Period',0.00,0.00,0.00,0.00,0,0.00,0,0.00,0,0.00,'Teaching');
/*!40000 ALTER TABLE `payroll` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-07 10:16:42
