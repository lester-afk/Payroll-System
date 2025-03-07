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
-- Table structure for table `schedule`
--

DROP TABLE IF EXISTS `schedule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `schedule` (
  `sched_num` int NOT NULL AUTO_INCREMENT,
  `employee_id` varchar(255) DEFAULT NULL,
  `sched_day` varchar(100) DEFAULT NULL,
  `sched_timeIn` time DEFAULT NULL,
  `sched_timeOut` time DEFAULT NULL,
  `sched_period` varchar(100) DEFAULT NULL,
  `sched_semester` varchar(50) DEFAULT NULL,
  `sched_type` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`sched_num`),
  UNIQUE KEY `sched_num` (`sched_num`),
  KEY `employee_id` (`employee_id`),
  KEY `idx_sched_period` (`sched_period`),
  CONSTRAINT `schedule_ibfk_1` FOREIGN KEY (`employee_id`) REFERENCES `employee` (`employee_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=90 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `schedule`
--

LOCK TABLES `schedule` WRITE;
/*!40000 ALTER TABLE `schedule` DISABLE KEYS */;
INSERT INTO `schedule` VALUES (30,'2024-10-9','Friday','22:00:00','22:20:00','1st Period','1st Semester','Teaching Load'),(31,'2024-10-9','Friday','22:50:00','23:00:00','1st Period','1st Semester','Regular Load'),(32,'2024-10-9','Sunday','01:05:00','01:25:00','3rd Period','1st Semester','Regular Load'),(33,'2024-10-7','Monday','11:12:00','22:12:00','1st Period','1st Semester','Teaching Load'),(36,'2024-10-8','Monday','16:00:00','16:30:00','2nd Period','1st Semester','Regular Load'),(45,'2024-10-8','Saturday','16:56:00','17:00:00','3rd Period','1st Semester','Regular Load'),(46,'2024-11-17','Thursday','08:00:00','17:00:00','1st Period','1st Semester','Teaching Load'),(47,'2024-11-17','Monday','13:00:00','17:00:00','1st Period','1st Semester','Teaching Load'),(48,'2024-11-17','Saturday','20:00:00','21:00:00','1st Period','1st Semester','Regular Load'),(49,'2024-11-17','Monday','18:26:00','18:26:00','2nd Period','1st Semester','Regular Load'),(50,'2024-11-15','Monday','08:00:00','17:00:00','1st Period','1st Semester','Regular'),(51,'2024-11-15','Tuesday','08:00:00','17:00:00','1st Period','1st Semester','Regular'),(52,'2024-11-15','Wednesday','08:00:00','17:00:00','1st Period','1st Semester','Regular'),(53,'2024-11-15','Thursday','08:00:00','17:00:00','1st Period','1st Semester','Regular'),(54,'2024-11-15','Friday','08:00:00','17:00:00','1st Period','1st Semester','Regular'),(55,'2024-10-10','Tuesday','16:00:00','16:30:00','1st Period','1st Semester','Regular Load'),(56,'2024-10-10','Tuesday','17:00:00','17:30:00','2nd Period','1st Semester','Regular Load'),(57,'2024-10-4','Tuesday','17:00:00','19:00:00','1st Period','1st Semester','Regular Load'),(58,'2024-10-5','Tuesday','19:00:00','21:00:00','1st Period','1st Semester','Regular Load'),(59,'2024-10-9','Tuesday','18:42:00','20:42:00','1st Period','1st Semester','Regular Load'),(60,'2024-10-8','Wednesday','10:50:00','11:50:00','1st Period','1st Semester','Regular Load'),(61,'2024-10-8','Wednesday','12:39:00','13:39:00','2nd Period','1st Semester','Regular Load'),(62,'2024-10-8','Wednesday','10:41:00','11:41:00','3rd Period','1st Semester','Regular Load'),(63,'2024-10-8','Wednesday','12:41:00','13:41:00','4th Period','1st Semester','Regular Load'),(64,'2024-11-16','Wednesday','10:51:00','11:51:00','1st Period','1st Semester','Regular Load'),(65,'2024-11-16','Wednesday','11:52:00','12:52:00','2nd Period','1st Semester','Regular Load'),(66,'2024-11-16','Wednesday','13:52:00','14:52:00','3rd Period','1st Semester','Regular Load'),(67,'2024-10-10','Wednesday','15:43:00','18:43:00','1st Period','1st Semester','Regular Load'),(68,'2024-10-10','Wednesday','13:43:00','14:43:00','2nd Period','1st Semester','Regular Load'),(69,'2024-10-6','Wednesday','13:17:00','14:17:00','1st Period','1st Semester','Regular Load'),(70,'2024-10-6','Wednesday','15:18:00','16:18:00','2nd Period','1st Semester','Regular Load'),(71,'2024-11-15','Wednesday','13:00:00','14:00:00','1st Period','1st Semester','Regular Load'),(72,'2024-11-15','Wednesday','15:00:00','16:00:00','2nd Period','1st Semester','Regular Load'),(73,'2024-11-17','Wednesday','15:00:00','15:30:00','1st Period','1st Semester','Regular Load'),(74,'2024-11-17','Wednesday','16:00:00','17:00:00','2nd Period','1st Semester','Regular Load'),(75,'2024-11-17','Wednesday','17:01:00','18:01:00','3rd Period','1st Semester','Regular Load'),(76,'2024-11-15','Wednesday','17:04:00','18:04:00','3rd Period','1st Semester','Regular Load'),(77,'2024-10-6','Wednesday','17:00:00','17:30:00','3rd Period','1st Semester','Regular Load'),(78,'2024-10-6','Wednesday','17:04:00','18:04:00','4th Period','1st Semester','Regular Load'),(79,'2024-11-17','Wednesday','17:40:00','18:40:00','4th Period','1st Semester','Regular Load'),(80,'2024-11-17','Thursday','11:00:00','12:00:00','1st Period','1st Semester','Regular Load'),(81,'2024-11-17','Saturday','12:00:00','13:00:00','1st Period','1st Semester','Regular Load'),(82,'2024-11-17','Saturday','16:00:00','17:00:00','2nd Period','1st Semester','Regular Load'),(83,'2024-11-17','Tuesday','13:00:00','14:45:00','1st Period','1st Semester','Regular Load'),(84,'2024-11-17','Tuesday','13:48:00','15:46:00','2nd Period','1st Semester','Regular Load'),(85,'2024-11-17','Tuesday','14:00:00','15:00:00','3rd Period','1st Semester','Regular Load'),(86,'2024-10-9','Tuesday','13:40:00','14:40:00','1st Period','1st Semester','Regular Load'),(87,'2024-10-9','Friday','11:30:00','12:30:00','1st Period','1st Semester','Regular Load'),(88,'2024-10-9','Friday','12:16:00','13:15:00','2nd Period','1st Semester','Regular Load'),(89,'2024-11-17','Friday','11:26:00','12:26:00','1st Period','1st Semester','Regular Load');
/*!40000 ALTER TABLE `schedule` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-07 10:16:56
