-- Initial setup for Volcanion Auth database
CREATE DATABASE IF NOT EXISTS volcanion_auth;
USE volcanion_auth;

-- Create additional user if needed
CREATE USER IF NOT EXISTS 'volcanion_user'@'%' IDENTIFIED BY 'volcanion_password';
GRANT ALL PRIVILEGES ON volcanion_auth.* TO 'volcanion_user'@'%';

-- Enable binary logging for replication (if needed)
-- SET GLOBAL log_bin_trust_function_creators = 1;

FLUSH PRIVILEGES;

-- You can add initial tables here if needed
-- CREATE TABLE users (
--     id INT AUTO_INCREMENT PRIMARY KEY,
--     username VARCHAR(255) NOT NULL,
--     email VARCHAR(255) NOT NULL,
--     created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
-- );
