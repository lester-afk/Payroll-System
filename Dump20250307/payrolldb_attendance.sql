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
-- Table structure for table `attendance`
--

DROP TABLE IF EXISTS `attendance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `attendance` (
  `a_num` int NOT NULL AUTO_INCREMENT,
  `employee_id` varchar(255) NOT NULL,
  `a_date` date NOT NULL,
  `a_timeIn` time DEFAULT NULL,
  `a_timeOut` time DEFAULT NULL,
  `a_period` varchar(100) DEFAULT NULL,
  `a_statusIn` varchar(100) DEFAULT NULL,
  `a_statusOut` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`a_num`),
  UNIQUE KEY `idx_employee_period` (`employee_id`,`a_date`,`a_period`),
  CONSTRAINT `attendance_ibfk_1` FOREIGN KEY (`employee_id`) REFERENCES `employee` (`employee_id`)
) ENGINE=InnoDB AUTO_INCREMENT=69 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `attendance`
--

LOCK TABLES `attendance` WRITE;
/*!40000 ALTER TABLE `attendance` DISABLE KEYS */;
INSERT INTO `attendance` VALUES (1,'2024-11-15','2024-11-08','19:10:00',NULL,'1st Period','Late',NULL),(3,'2024-11-15','2024-11-08','19:29:00','19:29:00','2nd Period','On Time','On Time'),(4,'2024-11-15','2024-11-08','19:33:00',NULL,'3rd Period','On Time',NULL),(5,'2024-11-15','2024-11-08','19:35:00','19:36:00','4th Period','On Time','On Time'),(6,'2024-10-9','2024-11-08','21:55:00',NULL,'1st Period','On Time',NULL),(7,'2024-10-9','2024-11-08','22:49:00','22:51:00','2nd Period','On Time','On Time'),(9,'2024-10-9','2024-11-09','00:47:00',NULL,'3rd Period','Early In',NULL),(22,'2024-10-8','2024-11-09','16:45:00',NULL,'1st Period','On Time',NULL),(23,'2024-10-8','2024-11-09','16:45:00','16:47:00','2nd Period','On Time','Under Time'),(24,'2024-10-9','2024-11-09','16:51:00','16:51:00','1st Period','On Time','Over Time'),(25,'2024-10-9','2024-11-09','16:52:00','16:53:00','2nd Period','On Time','Under Time'),(27,'2024-10-8','2024-11-09','17:01:00','17:01:00','3rd Period','On Time','Under Time'),(28,'2024-11-17','2024-11-09','18:25:00','18:27:00','1st Period','Early In','Under Time'),(30,'2024-11-17','2024-11-09','18:30:00','18:31:00','2nd Period','On Time','Under Time'),(31,'2024-11-17','2024-11-18','10:43:00',NULL,'2nd Period','Early In',NULL),(32,'2024-11-15','2024-02-05','08:05:00','17:00:00','Morning','Late','On Time'),(33,'2024-11-15','2024-02-06','08:00:00','16:45:00','Morning','On Time','Undertime'),(34,'2024-11-15','2024-02-07',NULL,NULL,'Morning','Absent','Absent'),(35,'2024-11-15','2024-02-08','08:00:00','17:00:00','Morning','On Time','On Time'),(36,'2024-11-15','2024-02-09','08:10:00','17:00:00','Morning','Late','On Time'),(37,'2024-10-10','2025-02-04','16:00:00','16:02:00','1st Period','On Time','Under Time'),(38,'2024-10-4','2025-02-04','16:43:00',NULL,'1st Period','Early In',NULL),(39,'2024-10-5','2025-02-04','16:43:00',NULL,'1st Period','Early In',NULL),(40,'2024-10-9','2025-02-04','16:45:00',NULL,'1st Period','Early In',NULL),(41,'2024-10-8','2025-02-05','04:40:00','04:40:00','1st Period','Early In','Under Time'),(42,'2024-10-8','2025-02-05','04:42:00','10:44:00','2nd Period','Under Time','Under Time'),(43,'2024-10-8','2025-02-05','04:43:00','10:44:00','3rd Period','Under Time','Under Time'),(44,'2024-10-8','2025-02-05','09:53:00','09:54:00','4th Period','Early In','Under Time'),(45,'2024-10-10','2025-02-05','10:51:00','10:54:00','1st Period','On Time','Under Time'),(46,'2024-11-16','2025-02-05','10:53:00','10:53:00','1st Period','On Time','Under Time'),(47,'2024-10-6','2025-02-05','13:18:00','13:19:00','1st Period','On Time','Under Time'),(48,'2024-11-15','2025-02-05','12:53:00','13:57:00','1st Period','Late','Under Time'),(49,'2024-11-15','2025-02-05','14:59:00','15:00:00','2nd Period','On Time','Under Time'),(50,'2024-11-17','2025-02-05','15:01:00',NULL,'1st Period','On Time',NULL),(51,'2024-11-17','2025-02-05','15:01:00','15:01:00','2nd Period','Early In','Under Time'),(52,'2024-11-17','2025-02-05','15:02:00','15:02:00','3rd Period','Early In','Under Time'),(53,'2024-11-15','2025-02-05','15:04:00','15:04:00','3rd Period','Early In','Under Time'),(54,'2024-10-6','2025-02-05','16:49:00','16:49:00','2nd Period','Late','Over Time'),(55,'2024-10-6','2025-02-05','16:51:00','16:57:00','3rd Period','On Time','Under Time'),(56,'2024-10-6','2025-02-05','17:19:00','17:21:00','4th Period','On Time','Under Time'),(57,'2024-11-17','2025-02-06','11:00:00','11:46:00','1st Period','Late','Under Time'),(58,'2024-11-17','2025-02-08','15:05:00',NULL,'1st Period','Early In',NULL),(59,'2024-11-17','2025-02-08','15:05:00','15:07:00','2nd Period','Early In','Under Time'),(60,'2024-11-17','2025-02-11','12:00:00','14:00:00','1st Period','Excuse','Excuse'),(61,'2024-11-17','2025-02-11','12:49:00','12:53:00','2nd Period','Early In','Under Time'),(62,'2024-11-17','2025-02-11','13:23:00','13:23:00','3rd Period','Early In','Under Time'),(63,'2024-10-9','2025-02-11','13:40:00','13:41:00','1st Period','Early In','Under Time'),(64,'2024-10-9','2025-02-14','11:24:00',NULL,'1st Period','Early In',NULL),(65,'2024-11-17','2025-02-14','11:27:00','11:27:00','1st Period','On Time','Under Time'),(66,'2024-10-4','2025-02-18','00:56:00','01:02:00','1st Period','Early In','Under Time'),(67,'2024-10-9','2025-03-07','22:00:00','22:20:00','1st Period','Excuse','Excuse'),(68,'2024-11-15','2025-03-07','08:00:00','17:00:00','1st Period','Excuse','Excuse');
/*!40000 ALTER TABLE `attendance` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-07 10:16:40
