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
-- Table structure for table `job`
--

DROP TABLE IF EXISTS `job`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `job` (
  `job_num` int NOT NULL AUTO_INCREMENT,
  `employee_id` varchar(255) NOT NULL,
  `job_status` varchar(100) DEFAULT NULL,
  `job_department` varchar(100) DEFAULT NULL,
  `job_title` varchar(100) DEFAULT NULL,
  `job_salary` decimal(12,2) DEFAULT NULL,
  `job_hourly_rate` decimal(12,2) DEFAULT NULL,
  `job_date_hired` date DEFAULT NULL,
  PRIMARY KEY (`job_num`),
  KEY `employee_id` (`employee_id`),
  CONSTRAINT `job_ibfk_1` FOREIGN KEY (`employee_id`) REFERENCES `employee` (`employee_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `job`
--

LOCK TABLES `job` WRITE;
/*!40000 ALTER TABLE `job` DISABLE KEYS */;
INSERT INTO `job` VALUES (1,'2024-10-10','Regular','','Canteen In-Charge',6500.00,NULL,'2024-08-05'),(2,'2024-10-4','Regular','College','Dean',15000.00,180.00,'2024-08-05'),(3,'2024-10-5','Regular','Senior High School','Principal',15000.00,180.00,'2024-08-05'),(4,'2024-10-6','Part Time','College','College Instructor',NULL,165.00,'2024-08-05'),(5,'2024-10-8','Regular','College','SAO Head',15000.00,180.00,'2024-08-05'),(6,'2024-10-9','Part Time','College','Instructor',NULL,120.00,'2024-11-09'),(7,'2024-11-17','Regular','College','Instructor',20000.00,200.00,'2024-11-09'),(8,'2024-11-15','Regular','Finance','Accountant',50000.00,250.00,'2023-01-01'),(9,'2024-11-16','Regular','College','Registrar',10000.00,NULL,'2025-02-05'),(12,'2024-10-7','Part Time','College','College Instructor',NULL,250.00,'2025-02-27');
/*!40000 ALTER TABLE `job` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-07 10:16:31
