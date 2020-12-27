USE cooking;
drop table if exists commande;
drop table if exists composition;
drop table if exists produit;
drop table if exists recette;
drop table IF EXISTS client;
drop table if exists fournisseur;

CREATE TABLE `cooking`.`client` (
	`nom` VARCHAR(20) NOT NULL,
	`numeroC` VARCHAR(10) NOT NULL,
    `Id_CdR` VARCHAR(20) NULL,
    `solde` DOUBLE DEFAULT 0,
	PRIMARY KEY(`numeroC`),
    INDEX `F_CdR_idx` (`Id_CdR` ASC)
);

create table `cooking`.`fournisseur`(
`numero_fournisseur` varchar(10) not null,
`nom_fournisseur` varchar(20) not null,
primary key(`numero_fournisseur`));

CREATE TABLE `cooking`.`recette` (
	`nom_recette` VARCHAR(50) NOT NULL,
    `type` VARCHAR(10) NOT NULL,
    `prix` INT NOT NULL,
    `count` INT NOT NULL,
    `rémunération` INT NOT NULL,
    `descriptif` VARCHAR(256) NULL,
    `Id_CdR` VARCHAR(6) NOT NULL,
    PRIMARY KEY(`nom_recette`),
	 constraint `Id_CdR_recette` FOREIGN KEY (`Id_CdR`)
		REFERENCES `cooking`.`client`(`Id_CdR`)
		ON DELETE CASCADE
		ON UPDATE NO ACTION
    );
    
CREATE TABLE `cooking`.`commande` (
	`id_Commande` VARCHAR(9),
	`numeroC` VARCHAR(10) NOT NULL,
	`nom_recette` VARCHAR(40) NOT NULL,
    `annee` VARCHAR(4) NOT NULL,
	`mois` VARCHAR(2) NOT NULL,
    `jour` VARCHAR(2) NOT NULL,
	PRIMARY KEY(`id_Commande`),
	 CONSTRAINT `numeroCcommande` FOREIGN KEY(`numeroC`)
		REFERENCES `cooking`.`client`(`numeroC`)
		ON DELETE CASCADE 
		ON UPDATE NO ACTION,
	 CONSTRAINT `nom_recetteCommande` FOREIGN KEY(`nom_recette`)
		REFERENCES `cooking`.`recette`(`nom_recette`)
		ON DELETE CASCADE
		ON UPDATE NO ACTION
);

create table`cooking`.`produit`(
`nom_produit` varchar(20) not null,
`categorie` varchar(20) not null,
`stock_actuel` double not null CHECK (stock_actuel >= 0),
`stock_min` double not null CHECK (stock_min >= 0),
`stock_max` double not null CHECK (stock_max >= 0),
`unite` varchar(5) not null,
`numero_fournisseur` varchar(10) not null,
primary key(`nom_produit`),
 constraint `numero_fournisseurProduits` foreign key (`numero_fournisseur`)
	references `cooking`.`fournisseur` (`numero_fournisseur`)
	on delete no action
	on update no action );
    
create table `cooking`.`composition`(
`nom_recette` varchar(50) not null,
`nom_produit` varchar(20) not null,
`quantite` double null,
primary key(`nom_recette`,`nom_produit`),
 constraint `nom_recetteCompo` foreign key (`nom_recette`)
	references `cooking`.`recette` (`nom_recette`)
	on delete no action
	on update no action,
 constraint `nom_produitCompo` foreign key (`nom_produit`)
	references `cooking`.`produit` (`nom_produit`)
	on delete no action
	on update no action );