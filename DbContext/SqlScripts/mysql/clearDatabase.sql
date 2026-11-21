USE `sql-travel`;

/* Remove stored procedures */
DROP PROCEDURE IF EXISTS `sql-travel`.`gstusr_spLogin`;
DROP PROCEDURE IF EXISTS `sql-travel`.`supusr_spDeleteAll`;

/* Remove roles */
DROP ROLE IF EXISTS 'gstUsrRole';
DROP ROLE IF EXISTS 'usrRole';
DROP ROLE IF EXISTS 'supUsrRole';
DROP ROLE IF EXISTS 'dboRole';

/* Remove users */
DROP USER IF EXISTS 'gstusr'@'%';
DROP USER IF EXISTS 'usr'@'%';
DROP USER IF EXISTS 'supusr'@'%';
DROP USER IF EXISTS 'dbo'@'%';

/* Flush privileges after user/role changes */
FLUSH PRIVILEGES;

/* Remove views */
DROP VIEW IF EXISTS `sql-travel`.`gstusr_vwInfoDb`;
DROP VIEW IF EXISTS `sql-travel`.`gstusr_vwInfoFriends`;
DROP VIEW IF EXISTS `sql-travel`.`gstusr_vwInfoPets`;
DROP VIEW IF EXISTS `sql-travel`.`gstusr_vwInfoQuotes`;

/* Drop tables in the right order to avoid FK conflicts */
DROP TABLE IF EXISTS `sql-travel`.`supusr_FriendDbMQuoteDbM`;
DROP TABLE IF EXISTS `sql-travel`.`supusr_Pets`;
DROP TABLE IF EXISTS `sql-travel`.`supusr_Quotes`;
DROP TABLE IF EXISTS `sql-travel`.`supusr_Friends`;
DROP TABLE IF EXISTS `sql-travel`.`supusr_Addresses`;
DROP TABLE IF EXISTS `sql-travel`.`dbo_Users`;
DROP TABLE IF EXISTS `sql-travel`.`__EFMigrationsHistory`;

