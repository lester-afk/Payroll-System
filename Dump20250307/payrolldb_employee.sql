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
-- Table structure for table `employee`
--

DROP TABLE IF EXISTS `employee`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `employee` (
  `e_num` int NOT NULL AUTO_INCREMENT,
  `employee_id` varchar(255) NOT NULL,
  `employee_fname` varchar(100) NOT NULL,
  `employee_mname` varchar(100) DEFAULT NULL,
  `employee_lname` varchar(100) NOT NULL,
  `employee_contact` varchar(100) DEFAULT NULL,
  `employee_email` varchar(100) DEFAULT NULL,
  `employee_address` varchar(255) DEFAULT NULL,
  `employee_picture` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`employee_id`),
  UNIQUE KEY `e_num` (`e_num`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `employee`
--

LOCK TABLES `employee` WRITE;
/*!40000 ALTER TABLE `employee` DISABLE KEYS */;
INSERT INTO `employee` VALUES (14,'2024-10-10','Francia','','Millena','09843672324','millena@gmail.com','Albay','C:\\Users\\Resu\\Desktop\\Payroll_System\\Payroll  System\\Payroll  System\\bin\\Debug\\Photo\\female2.jpg'),(4,'2024-10-4','Sonny','A','Maceda','09057043693','maceda@gmail.com','Legazpi City','C:\\Users\\Resu\\Desktop\\Payroll_System\\Payroll  System\\Payroll  System\\bin\\Debug\\Photo\\dean.jpg'),(5,'2024-10-5','Dante','A','Aringo','09301962353','aringo@gmail.com','Legazpi City','C:\\Users\\Resu\\Desktop\\Payroll_System\\Payroll  System\\Payroll  System\\bin\\Debug\\Photo\\Principal.jpg'),(6,'2024-10-6','Jay','M.','Benaraba','09734889878','benaraba@gmail.com','Guinobatan','C:\\Users\\Resu\\Desktop\\Payroll_System\\Payroll  System\\Payroll  System\\bin\\Debug\\Photo\\1.jpg'),(7,'2024-10-7','Darwin','B','Darca','09834238789','darca@gmail.com','Legazpi City','C:\\Users\\Resu\\Desktop\\Payroll_System\\Payroll  System\\Payroll  System\\bin\\Debug\\Photo\\2.jpg'),(8,'2024-10-8','Perla','R','Ala','09834898799','ala@gmail.com','Legazpi City','C:\\Users\\Resu\\Desktop\\Payroll_System\\Payroll  System\\Payroll  System\\bin\\Debug\\Photo\\female1.jpg'),(9,'2024-10-9','Hannilyn','B','Asaytuno','09734789789','asaytuno@gmail.com','Legazpi City','C:\\Users\\Resu\\Desktop\\Payroll_System\\Payroll  System\\Payroll  System\\bin\\Debug\\Photo\\female4.jpeg'),(15,'2024-11-15','Ariel','B','Abaleta','09237222511','abaleta@gmail.com','Legazpi City','C:\\Users\\Resu\\Desktop\\Payroll_System\\Payroll  System\\Payroll  System\\bin\\Debug\\Photo\\1.jpg'),(16,'2024-11-16','Rosenda','Emlano','Florin','09723871772','roseflorin@gmail.com','Legazpi City',NULL),(18,'2024-11-17','Trecy','V','Ortonio','09232134432','ortonio@gmail.com','Legazpi City','C:\\Users\\Resu\\Desktop\\Payroll_System\\Payroll  System\\Payroll  System\\bin\\Debug\\Photo\\1.jpg');
/*!40000 ALTER TABLE `employee` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-07 10:16:49
