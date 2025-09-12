CREATE DATABASE IF NOT EXISTS estoque_rolos 
  CHARACTER SET utf8mb4 
  COLLATE utf8mb4_unicode_ci;

USE estoque_rolos;

CREATE TABLE IF NOT EXISTS rolos (
  Code VARCHAR(30) PRIMARY KEY,
  Descricao VARCHAR(50) NOT NULL,
  Milimetragem DECIMAL(6,2) NOT NULL, 
  MOQ INT NOT NULL,                   
  Estoque INT NOT NULL,               
  MetragemWIP INT NOT NULL           
);


