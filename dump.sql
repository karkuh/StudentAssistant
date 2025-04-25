-- MySQL dump 10.13  Distrib 8.0.32, for Win64 (x86_64)
--
-- Host: localhost    Database: studentassistant
-- ------------------------------------------------------
-- Server version	8.0.35

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
-- Table structure for table `saved_student`
--

DROP TABLE IF EXISTS `saved_student`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `saved_student` (
  `id_student` int NOT NULL,
  `login` varchar(55) NOT NULL,
  `password` varchar(55) NOT NULL,
  `group_name` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`id_student`),
  UNIQUE KEY `login_UNIQUE` (`login`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `saved_student`
--

LOCK TABLES `saved_student` WRITE;
/*!40000 ALTER TABLE `saved_student` DISABLE KEYS */;
INSERT INTO `saved_student` VALUES (31,'a','a','TR-12');
/*!40000 ALTER TABLE `saved_student` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `statuses`
--

DROP TABLE IF EXISTS `statuses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `statuses` (
  `id_status` int NOT NULL AUTO_INCREMENT,
  `percent` int NOT NULL,
  PRIMARY KEY (`id_status`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `statuses`
--

LOCK TABLES `statuses` WRITE;
/*!40000 ALTER TABLE `statuses` DISABLE KEYS */;
INSERT INTO `statuses` VALUES (1,0),(2,25),(3,50),(4,75),(5,100);
/*!40000 ALTER TABLE `statuses` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Temporary view structure for view `student_subject_teacher_view`
--

DROP TABLE IF EXISTS `student_subject_teacher_view`;
/*!50001 DROP VIEW IF EXISTS `student_subject_teacher_view`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `student_subject_teacher_view` AS SELECT 
 1 AS `id_student`,
 1 AS `id_subject`,
 1 AS `name`,
 1 AS `full_name`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `students`
--

DROP TABLE IF EXISTS `students`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `students` (
  `id_student` int NOT NULL AUTO_INCREMENT,
  `login` varchar(55) NOT NULL,
  `password` varchar(55) NOT NULL,
  `group_name` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`id_student`),
  UNIQUE KEY `login` (`login`)
) ENGINE=InnoDB AUTO_INCREMENT=32 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `students`
--

LOCK TABLES `students` WRITE;
/*!40000 ALTER TABLE `students` DISABLE KEYS */;
INSERT INTO `students` VALUES (1,'ivan_ivanov','securepass123','ТР-11'),(2,'karkuh','password','ТР-12'),(3,'oleksandr_kovalenko','strongpassword','ТР-13'),(4,'olga_kozlova','mypassword789','ТР-14'),(5,'mykola_sydorenko','secret1234','ТР-15'),(7,'vlad_rydenko','12345678','ТР-12'),(20,'mariia_petrova','myp@ssw0rd','ТР-14'),(21,'serhii_koval','confidential789','ТР-15'),(22,'olena_sydorova','password!123','ТР-13'),(23,'andrii_pavlov','strong_pass','ТР-14'),(24,'nataliia_ivanova','safePWD321','ТР-15'),(25,'oleg_kuznetsov','hiddenpass','ТР-13'),(26,'anna_mykhailova','mypassword987','ТР-14'),(27,'dmytro_vorobiov','secure123','ТР-15'),(28,'iryna_kosova','password456','ТР-13'),(29,'karkuh33','12345678','ТР-12'),(30,'karkuhkarkuh','password','TR-12'),(31,'a','a','TR-12');
/*!40000 ALTER TABLE `students` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `subjects`
--

DROP TABLE IF EXISTS `subjects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `subjects` (
  `id_subject` int NOT NULL AUTO_INCREMENT,
  `name` varchar(55) NOT NULL,
  `id_teacher` int NOT NULL,
  PRIMARY KEY (`id_subject`),
  KEY `subjects_ibfk_1` (`id_teacher`),
  CONSTRAINT `subjects_ibfk_1` FOREIGN KEY (`id_teacher`) REFERENCES `teachers` (`id_teacher`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `subjects`
--

LOCK TABLES `subjects` WRITE;
/*!40000 ALTER TABLE `subjects` DISABLE KEYS */;
INSERT INTO `subjects` VALUES (1,'DevOps',1),(2,'Комп`ютерні мережі',2),(3,'Екологічний моніторинг',3),(4,'Проектування та використання баз даних',4),(5,'Технології розробки програмного забезпечення',5),(6,'Комп`ютерне моделювання',6),(7,'Чисельні методи',7),(11,'Екологічний моніторинг',6),(13,'Матан',4),(14,'БД',4),(16,'Курсова',11),(17,'Чисельні методи',11),(18,'DevOps',4),(19,'ІАД',1),(20,'ШІ',11),(21,'Англ',12),(22,'Сис Аналіз',1),(23,'САПР',1),(24,'ТПО',11),(25,'Еко ',1),(26,'ІНТЕРФЕЙСИ',1),(27,'ІНФ БЕЗ',1);
/*!40000 ALTER TABLE `subjects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `subjects_for_one_student`
--

DROP TABLE IF EXISTS `subjects_for_one_student`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `subjects_for_one_student` (
  `id_subjects_for_one_student` int NOT NULL AUTO_INCREMENT,
  `id_student` int NOT NULL,
  `id_subject` int NOT NULL,
  PRIMARY KEY (`id_subjects_for_one_student`),
  UNIQUE KEY `id_student` (`id_student`,`id_subject`),
  KEY `subjectsforonestudent_ibfk_2` (`id_subject`),
  CONSTRAINT `subjects_for_one_student_ibfk_1` FOREIGN KEY (`id_student`) REFERENCES `students` (`id_student`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `subjects_for_one_student_ibfk_2` FOREIGN KEY (`id_subject`) REFERENCES `subjects` (`id_subject`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=48 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `subjects_for_one_student`
--

LOCK TABLES `subjects_for_one_student` WRITE;
/*!40000 ALTER TABLE `subjects_for_one_student` DISABLE KEYS */;
INSERT INTO `subjects_for_one_student` VALUES (1,1,1),(2,1,2),(3,1,4),(4,1,5),(5,1,6),(7,2,3),(9,2,6),(10,2,7),(36,2,16),(38,2,18),(11,3,1),(12,3,3),(13,3,4),(14,3,5),(15,3,7),(16,4,1),(17,4,3),(18,4,4),(19,4,5),(20,4,7),(21,5,2),(22,5,3),(23,5,4),(24,5,6),(25,5,7),(39,31,19),(40,31,20),(41,31,21),(42,31,22),(43,31,23),(44,31,24),(45,31,25),(46,31,26),(47,31,27);
/*!40000 ALTER TABLE `subjects_for_one_student` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tasks`
--

DROP TABLE IF EXISTS `tasks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tasks` (
  `id_task` int NOT NULL AUTO_INCREMENT,
  `id_student` int NOT NULL,
  `id_subject` int NOT NULL,
  `id_status` int NOT NULL DEFAULT (1),
  `task` varchar(255) NOT NULL,
  `deadline` date DEFAULT NULL,
  `max_score` decimal(5,2) NOT NULL,
  `earned_score` decimal(5,2) DEFAULT NULL,
  PRIMARY KEY (`id_task`),
  KEY `id_student` (`id_student`),
  KEY `id_subject` (`id_subject`),
  KEY `id_status` (`id_status`),
  CONSTRAINT `tasks_ibfk_1` FOREIGN KEY (`id_student`) REFERENCES `students` (`id_student`) ON DELETE CASCADE,
  CONSTRAINT `tasks_ibfk_2` FOREIGN KEY (`id_subject`) REFERENCES `subjects` (`id_subject`) ON DELETE CASCADE,
  CONSTRAINT `tasks_ibfk_3` FOREIGN KEY (`id_status`) REFERENCES `statuses` (`id_status`)
) ENGINE=InnoDB AUTO_INCREMENT=83 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tasks`
--

LOCK TABLES `tasks` WRITE;
/*!40000 ALTER TABLE `tasks` DISABLE KEYS */;
INSERT INTO `tasks` VALUES (1,1,1,4,'Лабораторна робота №1','2023-09-29',100.00,NULL),(2,1,2,5,'Лабораторна робота №1','2023-09-28',100.00,98.00),(3,1,4,1,'Лабораторна робота №1','2023-09-17',100.00,NULL),(4,1,5,5,'Лекція №1','2023-09-25',50.00,49.00),(5,1,6,1,'Лекція №1','2023-09-22',50.00,NULL),(6,2,2,1,'Лабораторна робота №1','2023-12-04',100.00,NULL),(7,2,3,4,'Лабораторна робота №1','2023-09-28',100.00,NULL),(8,2,5,5,'Лекція №1','2023-09-25',50.00,45.00),(9,2,6,4,'Лекція №1','2023-09-22',50.00,NULL),(11,3,1,5,'Лабораторна робота №1','2023-09-29',100.00,80.00),(12,3,3,2,'Лабораторна робота №1','2023-09-28',100.00,NULL),(13,3,4,1,'Лабораторна робота №1','2023-09-17',100.00,NULL),(14,3,5,4,'Лекція №1','2023-09-25',50.00,NULL),(15,3,7,1,'Лекція №1','2023-09-27',50.00,NULL),(16,4,1,5,'Лабораторна робота №1','2023-09-29',100.00,60.00),(17,4,3,1,'Лабораторна робота №1','2023-09-28',100.00,NULL),(18,4,4,1,'Лабораторна робота №1','2023-09-17',100.00,NULL),(19,4,5,1,'Лекція №1','2023-09-25',50.00,NULL),(20,4,7,1,'Лекція №1','2023-09-27',50.00,NULL),(21,5,2,1,'Лабораторна робота №1','2023-09-28',100.00,NULL),(22,5,3,1,'Лабораторна робота №1','2023-09-28',100.00,NULL),(23,5,4,1,'Лабораторна робота №1','2023-09-17',100.00,NULL),(24,5,6,1,'Лекція №1','2023-09-22',50.00,NULL),(25,5,7,1,'Лекція №1','2023-09-27',50.00,NULL),(53,2,2,3,'Додаткове завдання','2023-10-28',100.00,NULL),(54,2,2,3,'Лабораторна робота №2','2023-10-28',100.00,NULL),(57,2,2,5,'Лабораторна робота №3','2023-11-08',100.00,95.00),(61,1,2,3,'Ваше завдання','2023-12-23',100.00,NULL),(63,2,5,3,'завдання №2-4','2023-12-23',25.00,NULL),(65,2,7,5,'Подивитися відео','2023-12-23',100.00,88.00),(67,2,1,5,'asdas','2023-12-24',10.00,9.50),(74,2,16,5,'Написати круту записку ','2024-01-25',100.00,100.00),(75,2,16,5,'здати записку','2024-01-09',100.00,100.00),(76,31,19,4,'5 лаба','2024-03-18',0.00,NULL),(77,31,20,4,'8 лаба','2024-03-18',0.00,NULL),(78,31,21,1,'дз 20,03','2024-03-27',0.00,NULL),(79,31,23,1,'1 лаба','2024-03-27',0.00,NULL),(80,31,24,1,'3 лаба','2024-03-27',0.00,NULL),(81,31,20,1,'9 лаба','2024-03-28',0.00,NULL),(82,31,25,1,'підготуватися до кр','2024-03-28',0.00,NULL);
/*!40000 ALTER TABLE `tasks` ENABLE KEYS */;
UNLOCK TABLES;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `update_deadline_after_deadline_is_null` BEFORE INSERT ON `tasks` FOR EACH ROW BEGIN
    IF NEW.deadline IS  NULL THEN
        SET NEW.deadline = DATE_ADD(curdate(), INTERVAL 14 DAY);
    END IF;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50017 DEFINER=`root`@`localhost`*/ /*!50003 TRIGGER `update_grade_after_status_change` BEFORE UPDATE ON `tasks` FOR EACH ROW BEGIN
    IF NEW.id_status <> 5 THEN
        SET NEW.earned_score = null;
    END IF;
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Temporary view structure for view `teacher_view`
--

DROP TABLE IF EXISTS `teacher_view`;
/*!50001 DROP VIEW IF EXISTS `teacher_view`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `teacher_view` AS SELECT 
 1 AS `id_teacher`,
 1 AS `full_name`*/;
SET character_set_client = @saved_cs_client;

--
-- Table structure for table `teachers`
--

DROP TABLE IF EXISTS `teachers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `teachers` (
  `id_teacher` int NOT NULL AUTO_INCREMENT,
  `last_name` varchar(55) NOT NULL,
  `first_name` varchar(55) NOT NULL,
  `middle_name` varchar(55) NOT NULL,
  PRIMARY KEY (`id_teacher`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `teachers`
--

LOCK TABLES `teachers` WRITE;
/*!40000 ALTER TABLE `teachers` DISABLE KEYS */;
INSERT INTO `teachers` VALUES (1,'Цей','Во','Да'),(2,'Отрох','Сергій','Іванович'),(3,'Полягушко','Любов','Григорівна'),(4,'Сегеда','Ірина','Василівна'),(5,'Тихоход','Володимир','Олександрович'),(6,'Шушура','Олексій','Миколайович'),(7,'Залевська','Ольга','Валеріївна'),(11,'Софієнко','Антон','Юрійович'),(12,'Шиліна','хз','хз');
/*!40000 ALTER TABLE `teachers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'studentassistant'
--

--
-- Dumping routines for database 'studentassistant'
--
/*!50003 DROP FUNCTION IF EXISTS `CheckEarnedScore` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `CheckEarnedScore`(task text, earned_score DECIMAL(5, 2)) RETURNS varchar(255) CHARSET utf8mb4
    DETERMINISTIC
BEGIN
    DECLARE result VARCHAR(255);
    
    IF earned_score IS NOT NULL THEN
        SET result = CONCAT(task, ' --> ', earned_score);
    ELSE
        SET result = CONCAT(task, ' --> ❌');
    END IF;

    RETURN result;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP FUNCTION IF EXISTS `CheckingForWorkCompletion` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `CheckingForWorkCompletion`(id_task int, task text, deadline date, earned_score DECIMAL(5, 2)) RETURNS varchar(255) CHARSET utf8mb4
    DETERMINISTIC
BEGIN
    DECLARE result VARCHAR(255);
    
    IF earned_score IS NOT NULL THEN
        SET result = CONCAT(id_task,' --> ', task, ' --> завдання виконано');
    ELSE
		if deadline - curdate() >= 0 then 
			SET result = CONCAT(id_task,' --> ',task, ' --> до дедлайну днів: ', deadline - curdate());
		else
			SET result = CONCAT(id_task,' --> ',task, ' --> дедлайн протермінований на днів: ',  curdate() - deadline);
		END IF;
    END IF;

    RETURN result;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `AvgScoreBySubject` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `AvgScoreBySubject`(in nm VARCHAR(255), in teacherId int, out result int)
BEGIN
set @subject= 0;
SELECT 
    id_subject
INTO @subject FROM
    subjects
WHERE
    name = nm;

SELECT 
ROUND(avg((earned_score / max_score * 100))) into result
FROM
    tasks
WHERE
    id_subject = @subject
        AND earned_score IS NOT NULL;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `GreatestScore` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `GreatestScore`(in nm VARCHAR(255))
BEGIN
set @subject= 0;
SELECT 
    id_subject
INTO @subject FROM
    subjects
WHERE
    name = nm;

SELECT 
    id_student,
    max_score,
    earned_score,
    CONCAT(ROUND(earned_score / max_score * 100),
            '%') AS 'percent'
FROM
    tasks
WHERE
    id_subject = @subject
        AND earned_score IS NOT NULL
ORDER BY earned_score / max_score DESC
LIMIT 1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `InfoAboutSubject` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `InfoAboutSubject`(in nm VARCHAR(255))
BEGIN
declare countStudents int;

SELECT 
    COUNT(*)
INTO countStudents FROM
    subjects_for_one_student
        JOIN
    subjects ON subjects_for_one_student.id_subject = subjects.id_subject
WHERE
    subjects.name = nm; 

SELECT 
    id_subject,
    name,
    first_name,
    middle_name,
    last_name,
    countStudents
FROM
    subjects
        JOIN
    teachers ON subjects.id_teacher = teachers.id_teacher
WHERE
    name = nm;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `password_checking` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `password_checking`()
BEGIN
  -- Оголошення змінних для курсора
  DECLARE done INT DEFAULT FALSE;
  DECLARE student_id INT;
  DECLARE student_login VARCHAR(255);
  DECLARE student_password VARCHAR(255);
  DECLARE student_group_name VARCHAR(255);
  DECLARE change_password BOOLEAN DEFAULT FALSE;
  
  -- Оголошення курсора
  DECLARE cur CURSOR FOR
    SELECT id_student, login, password, group_name
    FROM students;
  
  -- Обробка результатів запиту
  DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = true;
  
  -- Відкриття курсора
  OPEN cur;
  
  read_loop: LOOP
    FETCH NEXT FROM cur INTO student_id, student_login, student_password, student_group_name;
    
    IF done THEN
      LEAVE read_loop;
    END IF;
    
    -- Додана бізнес-логіка: перевірка, чи потрібно змінити пароль
    IF CHAR_LENGTH(student_password) < 10 THEN
      SET change_password = TRUE;
    ELSE
      SET change_password = FALSE;
    END IF;
    
    -- Виведення інформації про студента та інформації, чи потрібно змінити пароль
    SELECT student_id, student_login, student_password, student_group_name, change_password;
  END LOOP;
  
  -- Закриття курсора
  CLOSE cur;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `SubjectsOfOneStudent` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8mb4 */ ;
/*!50003 SET character_set_results = utf8mb4 */ ;
/*!50003 SET collation_connection  = utf8mb4_0900_ai_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `SubjectsOfOneStudent`(in id int)
BEGIN
 select id_subject, name, full_name from student_subject_teacher_view where id_student = id;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;

--
-- Final view structure for view `student_subject_teacher_view`
--

/*!50001 DROP VIEW IF EXISTS `student_subject_teacher_view`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `student_subject_teacher_view` AS select `sbfo`.`id_student` AS `id_student`,`sbfo`.`id_subject` AS `id_subject`,`sb`.`name` AS `name`,`tc`.`full_name` AS `full_name` from ((`subjects_for_one_student` `sbfo` join `subjects` `sb` on((`sbfo`.`id_subject` = `sb`.`id_subject`))) join `teacher_view` `tc` on((`sb`.`id_teacher` = `tc`.`id_teacher`))) */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `teacher_view`
--

/*!50001 DROP VIEW IF EXISTS `teacher_view`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `teacher_view` AS select `teachers`.`id_teacher` AS `id_teacher`,concat(`teachers`.`last_name`,' ',left(`teachers`.`first_name`,1),'.',left(`teachers`.`middle_name`,1),'.') AS `full_name` from `teachers` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-03-14 16:56:40
