-- Requêtes création de client

INSERT INTO `cooking`.`client` (`nom`, `numeroC`, `Id_CdR`, `solde`) 
VALUES ('Enzo','0651274137','CDR1',0);
INSERT INTO `cooking`.`client` (`nom`, `numeroC`, `Id_CdR`, `solde`) 
VALUES ('Mathieu','0685770715','CDR2',0);
INSERT INTO `cooking`.`client` (`nom`, `numeroC`, `Id_CdR`, `solde`) 
VALUES ('Jeremie','0668609179',null,300);
INSERT INTO `cooking`.`client` (`nom`, `numeroC`, `Id_CdR`, `solde`) 
VALUES ('Benjamin','0760145928','CDR3',0);
INSERT INTO `cooking`.`client` (`nom`, `numeroC`, `Id_CdR`, `solde`) 
VALUES ('Ikhlass','0665952360',null,0);
INSERT INTO `cooking`.`client` (`nom`, `numeroC`, `Id_CdR`, `solde`) 
VALUES ('Joakim','0669067091','CDR4',0);
INSERT INTO `cooking`.`client` (`nom`, `numeroC`, `Id_CdR`, `solde`) 
VALUES ('Amine','0698455421',null,0);

-- Requêtes création de fournisseur 

INSERT INTO `cooking`.`fournisseur` (`numero_fournisseur`, `nom_fournisseur`) 
VALUES ('0123456789','Rungis');
INSERT INTO `cooking`.`fournisseur` (`numero_fournisseur`, `nom_fournisseur`) 
VALUES ('0198765432','EcoBio');

-- Requêtes création de produit 

INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('oignon','legumes',500.0,10.0,5000.0,'g','0123456789');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('epinard','legumes',10,1,50,'kg','0123456789');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('huile d olive','huiles',5,1,10,'L','0198765432');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('feuille filo','feuille de pate',50,10,100,'nbr','0198765432');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('oeufs','aliment',100,20,200,'nbr','0198765432');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('feta','fromage',500,300,5000,'g','0123456789');

INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('pomme de terre','legume',200,100,2000,'g','0123456789');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('salade','legume',500,100,2000,'g','0123456789');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('radis','legume',500,100,2000,'g','0123456789');

INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('farine','produit usuel',2000,1000,10000,'g','0198765432');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('sucre','produit usuel',2000,1000,10000,'g','0198765432');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('beurre','produit usuel',2000,1000,5000,'g','0198765432');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('levure','produit usuel',1000,500,5000,'g','0198765432');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('chocolat','produit usuel',2000,1000,10000,'g','0198765432');

INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('banane','fruit',20,10,100,'nbr','0123456789');

INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('pomme','fruit',20,10,100,'nbr','0123456789');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('orange','fruit',20,10,100,'nbr','0123456789');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('kiwi','fruit',20,10,100,'nbr','0123456789');
INSERT INTO `cooking`.`produit` (`nom_produit`,`categorie`,`stock_actuel`,`stock_min`,`stock_max`,`unite`,`numero_fournisseur`)
VALUES ('yaourt','aliment',20,10,100,'nbr','0198765432');

-- Requêtes création de recette

INSERT INTO `cooking`.`recette`(`nom_recette`,`type`,`prix`,`count`,`rémunération`,`descriptif`,`Id_CdR`)
VALUES ('Spanakopita','plat',30,0,2,'La spanakópita dans la cuisine grecque est un friand aux épinards. Il s agit d une pâte, de type brik fourrée aux épinards, de la féta, des oignons ou de l échalote et de l œuf, le tout assaisonné','CDR1');

INSERT INTO `cooking`.`recette`(`nom_recette`,`type`,`prix`,`count`,`rémunération`,`descriptif`,`Id_CdR`)
VALUES ('Omelette aux chips et oignons séchés','plat',20,0,2,'La recette facile de notre créateur pour recycler les chips qui ont perdu de leur croustillant. Cette omelette aux chips va en étonner plus d un. Un plat idéal pour régaler toute la famille.','CDR2');

INSERT INTO `cooking`.`recette`(`nom_recette`,`type`,`prix`,`count`,`rémunération`,`descriptif`,`Id_CdR`)
VALUES ('Madeleine enrobe de chocolat','dessert',15,0,2,'De gourmandes madeleines enveloppes finement de chocolat noir pour finir le repas en beaute','CDR3');

INSERT INTO `cooking`.`recette`(`nom_recette`,`type`,`prix`,`count`,`rémunération`,`descriptif`,`Id_CdR`)
VALUES ('Banane au chocolat','dessert',15,0,2,'Le gout de la banane sublime par un delicieux chocolat fondant','CDR3');

INSERT INTO `cooking`.`recette`(`nom_recette`,`type`,`prix`,`count`,`rémunération`,`descriptif`,`Id_CdR`)
VALUES ('Salade de fruits au yaourt','dessert',15,0,2,'Un melange de saveur estival sur un lit de yaourt onctueux','CDR3');


-- Requêtes création de composition
-- Spanakopita
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Spanakopita','oignon',4);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Spanakopita','epinard',1);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Spanakopita','huile d olive',0.02);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Spanakopita','feuille filo',6);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Spanakopita','oeufs',5);

-- omelette
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Omelette aux chips et oignons séchés','oeufs',12);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Omelette aux chips et oignons séchés','pomme de terre',200);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Omelette aux chips et oignons séchés','oignon',100);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Omelette aux chips et oignons séchés','beurre',100);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Omelette aux chips et oignons séchés','salade',50);

-- Madeleine
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Madeleine enrobe de chocolat','oeufs',2);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Madeleine enrobe de chocolat','farine',100);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Madeleine enrobe de chocolat','sucre',70);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Madeleine enrobe de chocolat','beurre',100);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Madeleine enrobe de chocolat','levure',1);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Madeleine enrobe de chocolat','chocolat',100);

-- Banane choco
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Banane au chocolat','banane',1);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Banane au chocolat','chocolat',100);

-- Salade de fruits
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Salade de fruits au yaourt','banane',1);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Salade de fruits au yaourt','pomme',1);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Salade de fruits au yaourt','orange',1);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Salade de fruits au yaourt','kiwi',2);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Salade de fruits au yaourt','sucre',50);
INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)
VALUES ('Salade de fruits au yaourt','yaourt',1);

-- Requetes commande

INSERT INTO `cooking`.`commande` (`id_Commande`,`numeroC`,`nom_recette`,`annee`,`mois`,`jour`) 
VALUES ('C15H30J0','0668609179','Spanakopita','2020','05','11');
INSERT INTO `cooking`.`commande` (`id_Commande`,`numeroC`,`nom_recette`,`annee`,`mois`,`jour`) 
VALUES ('C17H40J0','0668609179','Spanakopita','2020','05','11');
INSERT INTO `cooking`.`commande` (`id_Commande`,`numeroC`,`nom_recette`,`annee`,`mois`,`jour`) 
VALUES ('C18H10J0','0668609179','Banane au chocolat','2020','05','06');
INSERT INTO `cooking`.`commande` (`id_Commande`,`numeroC`,`nom_recette`,`annee`,`mois`,`jour`) 
VALUES ('C19H50J0','0668609179','Salade de fruits au yaourt','2020','05','05');
INSERT INTO `cooking`.`commande` (`id_Commande`,`numeroC`,`nom_recette`,`annee`,`mois`,`jour`) 
VALUES ('C20H30J0','0668609179','Madeleine enrobe de chocolat','2020','05','11');

